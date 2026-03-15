#!/usr/bin/env python3
"""
implementation_issues.py – CI automation for JAT implementation issue tracking.

Subcommands:
  sync-from-issue-event  --event-path PATH
      Read a GitHub issue event payload, create or update the local record in
      ImplementationIssues/Records/, and regenerate the index and archive.

  sync-pr-resolution     --event-path PATH
      Read a merged-PR event payload, find referenced implementation issue numbers,
      and update the matching local records with the resolution PR link.

  sync-local-to-github   --owner ORG --repo NAME --token TOKEN
      Scan local records that have no GitHub issue number and create matching
      GitHub issues via the API, then backfill the record metadata.

All subcommands regenerate ImplementationIssuesIndex.md and
ImplementationIssuesArchive.md after any successful change.

Usage (same as the workflow invocations):
  python scripts/implementation_issues.py sync-from-issue-event --event-path "$GITHUB_EVENT_PATH"
  python scripts/implementation_issues.py sync-pr-resolution    --event-path "$GITHUB_EVENT_PATH"
  python scripts/implementation_issues.py sync-local-to-github  --owner org --repo name --token tok
"""

from __future__ import annotations

import argparse
import json
import os
import re
import sys
import urllib.error
import urllib.request
from datetime import datetime, timezone
from pathlib import Path

# ---------------------------------------------------------------------------
# Repository paths
# ---------------------------------------------------------------------------

_REPO_ROOT = Path(__file__).resolve().parent.parent
_ISSUES_ROOT = _REPO_ROOT / "Project" / "Tasks" / "ImplementationPlan" / "ImplementationIssues"
RECORDS_DIR = _ISSUES_ROOT / "Records"
INDEX_FILE = _ISSUES_ROOT / "ImplementationIssuesIndex.md"
ARCHIVE_FILE = _ISSUES_ROOT / "ImplementationIssuesArchive.md"

# ---------------------------------------------------------------------------
# Label constants
# ---------------------------------------------------------------------------

_LABEL_IMPL_ISSUE = "implementation-issue"

_LABEL_TO_TYPE: dict[str, str] = {
    "issue-type: deferment": "Deferment",
    "issue-type: open-issue": "Open issue",
    "issue-type: ambiguity-question": "Ambiguity / question",
    "issue-type: architecture-concern": "Architecture concern",
    "issue-type: review-follow-up": "Review follow-up",
}

_LABEL_TO_PRIORITY: dict[str, str] = {
    "priority: critical": "Critical",
    "priority: high": "High",
    "priority: medium": "Medium",
    "priority: low": "Low",
}

_TYPE_TO_LABEL: dict[str, str] = {v: k for k, v in _LABEL_TO_TYPE.items()}
_PRIORITY_TO_LABEL: dict[str, str] = {v: k for k, v in _LABEL_TO_PRIORITY.items()}

# ---------------------------------------------------------------------------
# YAML frontmatter helpers
# ---------------------------------------------------------------------------

# Canonical field order matching the existing record format.
_FIELD_ORDER = [
    "issue_number",
    "legacy_id",
    "type",
    "title",
    "summary",
    "created_phase",
    "source",
    "scheduled_target",
    "status",
    "priority",
    "github_url",
    "resolution_pr",
    "created_by",
    "created_at",
    "updated_at",
    "sync_state",
    "notes",
]

# Fields stored as bare integers (no quotes).
_INT_FIELDS = {"issue_number"}


def _parse_frontmatter(text: str) -> tuple[dict[str, str], str]:
    """
    Parse YAML-like frontmatter from a markdown string.

    Returns (fields_dict, body_text).  Multi-line double-quoted values (like
    long notes fields) are read line-by-line until the closing quote is found.
    For index/archive purposes only the first line of a multi-line value is
    stored in the dict; the full raw value is stored under key+"__raw".
    """
    if not text.startswith("---"):
        return {}, text

    lines = text.split("\n")
    end_idx = -1
    for i in range(1, len(lines)):
        if lines[i].rstrip() == "---":
            end_idx = i
            break
    if end_idx == -1:
        return {}, text

    fm_lines = lines[1:end_idx]
    body = "\n".join(lines[end_idx + 1:])

    data: dict[str, str] = {}
    i = 0
    while i < len(fm_lines):
        line = fm_lines[i]
        m = re.match(r"^([A-Za-z_]\w*):\s*(.*)", line)
        if m:
            key = m.group(1)
            raw_val = m.group(2).rstrip()

            if raw_val.startswith('"'):
                # Check whether the closing quote is on the same line.
                # Strip the leading quote, then look for an unescaped trailing quote.
                inner = raw_val[1:]
                if inner.endswith('"') and not inner.endswith('\\"'):
                    # Single-line double-quoted value.
                    value = inner[:-1].replace('\\"', '"')
                    data[key] = value
                elif raw_val == '"':
                    # Starts a multi-line value on the very next lines.
                    collected: list[str] = []
                    i += 1
                    while i < len(fm_lines):
                        next_line = fm_lines[i]
                        if next_line.endswith('"') and not next_line.endswith('\\"'):
                            collected.append(next_line[:-1])
                            break
                        collected.append(next_line)
                        i += 1
                    full_value = "\n".join(collected)
                    data[key + "__raw"] = full_value
                    # Keep only the first non-empty line for easy access.
                    first_line = next((ln for ln in collected if ln.strip()), "")
                    data[key] = first_line
                else:
                    # Incomplete quote on this line: multi-line value started here.
                    collected = [inner]
                    i += 1
                    while i < len(fm_lines):
                        next_line = fm_lines[i]
                        if next_line.endswith('"') and not next_line.endswith('\\"'):
                            collected.append(next_line[:-1])
                            break
                        collected.append(next_line)
                        i += 1
                    full_value = "\n".join(collected)
                    data[key + "__raw"] = full_value
                    first_line = next((ln.strip() for ln in collected if ln.strip()), "")
                    data[key] = first_line
            elif raw_val.startswith("'"):
                # Single-quoted value (always single-line in our records).
                if raw_val.endswith("'") and len(raw_val) >= 2:
                    data[key] = raw_val[1:-1]
                else:
                    data[key] = raw_val[1:]
            else:
                data[key] = raw_val
        i += 1

    return data, body


def _quote_fm_value(key: str, value: str) -> str:
    """
    Return the YAML frontmatter representation of a single field line.
    Integer fields are unquoted; all others are double-quoted.
    """
    if key in _INT_FIELDS:
        return f"{key}: {value}"
    escaped = str(value).replace("\\", "\\\\").replace('"', '\\"')
    return f'{key}: "{escaped}"'


def _dump_frontmatter(data: dict[str, str]) -> str:
    """Serialise a fields dict back to YAML frontmatter text (no trailing newline)."""
    lines = ["---"]
    seen: set[str] = set()
    for key in _FIELD_ORDER:
        if key in data:
            lines.append(_quote_fm_value(key, data[key]))
            seen.add(key)
    # Any extra keys not in the canonical order.
    for key, val in data.items():
        if key not in seen and not key.endswith("__raw"):
            lines.append(_quote_fm_value(key, val))
    lines.append("---")
    return "\n".join(lines)


def _read_record(path: Path) -> tuple[dict[str, str], str]:
    """Read a record file; return (frontmatter_dict, body_text)."""
    text = path.read_text(encoding="utf-8")
    return _parse_frontmatter(text)


def _write_record(path: Path, data: dict[str, str], body: str) -> None:
    """Write a record file from frontmatter dict + body."""
    fm = _dump_frontmatter(data)
    content = fm + "\n\n" + body.lstrip("\n")
    path.write_text(content, encoding="utf-8")


def _update_record_fields(path: Path, updates: dict[str, str]) -> None:
    """
    Update specific frontmatter fields in an existing record file without
    touching the rest of the frontmatter or the body.  This avoids re-writing
    complex multi-line values.
    """
    text = path.read_text(encoding="utf-8")

    # Locate the frontmatter region.
    if not text.startswith("---"):
        return
    end = text.find("\n---\n", 3)
    if end == -1:
        end = text.find("\n---\r\n", 3)
        if end == -1:
            return

    fm_text = text[: end + 1]  # includes opening --- but not the closing ---\n
    rest = text[end + 1:]       # starts with ---\n

    for key, value in updates.items():
        new_line = _quote_fm_value(key, value)
        pattern = re.compile(rf"^{re.escape(key)}:.*$", re.MULTILINE)
        if pattern.search(fm_text):
            fm_text = pattern.sub(new_line, fm_text)
        else:
            # Field not present – insert before the final --- marker.
            fm_text = fm_text.rstrip("\n") + "\n" + new_line + "\n"

    path.write_text(fm_text + rest, encoding="utf-8")


# ---------------------------------------------------------------------------
# Issue body parsing (GitHub Forms and manual markdown templates)
# ---------------------------------------------------------------------------

# Maps normalised section headings to record field names.
_SECTION_MAP = {
    "summary": "summary",
    "created phase": "created_phase",
    "source": "source",
    "scheduled target": "scheduled_target",
    "priority": "priority",
    "notes": "notes",
    "rationale and context": "rationale",
    "impact": "impact",
    "implementation notes": "implementation_notes",
    "acceptance / closing criteria": "acceptance_criteria",
    "history / resolution notes": "history_notes",
}


def _parse_issue_body(body: str) -> dict[str, str]:
    """
    Extract structured fields from a GitHub issue body.

    Supports both GitHub Forms output (### headings) and manual markdown
    templates (### headings under a ## Template Content section).
    """
    fields: dict[str, str] = {}
    if not body:
        return fields

    # Split on h2 (##) or h3 (###) headings.
    parts = re.split(r"^#{2,3}\s+", body, flags=re.MULTILINE)
    for part in parts:
        if not part.strip():
            continue
        nl = part.find("\n")
        if nl == -1:
            continue
        heading = part[:nl].strip().lower()
        content = part[nl:].strip()
        key = _SECTION_MAP.get(heading)
        if key:
            fields[key] = content

    return fields


# ---------------------------------------------------------------------------
# Utility helpers
# ---------------------------------------------------------------------------

def _now_iso() -> str:
    return datetime.now(timezone.utc).strftime("%Y-%m-%dT%H:%M:%S+00:00")


def _today() -> str:
    return datetime.now(timezone.utc).strftime("%Y-%m-%d")


def _normalise_labels(labels_raw: list) -> list[str]:
    return [
        lbl.get("name", "") if isinstance(lbl, dict) else str(lbl)
        for lbl in (labels_raw or [])
    ]


def _type_from_labels(labels: list[str]) -> str:
    for lbl in labels:
        t = _LABEL_TO_TYPE.get(lbl.lower())
        if t:
            return t
    return "Open issue"


def _priority_from_labels(labels: list[str]) -> str | None:
    for lbl in labels:
        p = _LABEL_TO_PRIORITY.get(lbl.lower())
        if p:
            return p
    return None


def _priority_from_body(fields: dict[str, str]) -> str | None:
    raw = fields.get("priority", "").strip()
    for p in ("Critical", "High", "Medium", "Low"):
        if p.lower() in raw.lower():
            return p
    return None


def _record_path(issue_number: int | str) -> Path:
    return RECORDS_DIR / f"issue-{issue_number}.md"


def _strip(s: str | None) -> str:
    return (s or "").strip()


def _load_all_records() -> list[dict[str, str]]:
    """Load all record files and return a list of frontmatter dicts."""
    records: list[dict[str, str]] = []
    for f in sorted(RECORDS_DIR.glob("issue-*.md")):
        try:
            data, _ = _read_record(f)
            if data:
                records.append(data)
        except (OSError, ValueError, UnicodeDecodeError) as exc:
            print(f"Warning: could not parse {f}: {exc}", file=sys.stderr)
    return records


# ---------------------------------------------------------------------------
# Record body builder
# ---------------------------------------------------------------------------

def _build_record_body(fields: dict[str, str]) -> str:
    """Build the markdown body section of a record from parsed issue fields."""

    def section(heading: str, content: str) -> str:
        return (
            f"## {heading}\n\n{content.strip()}\n\n"
            if content and content.strip()
            else f"## {heading}\n\n"
        )

    return (
        "# Implementation Issue Record\n\n"
        + section("Rationale And Context", fields.get("rationale", ""))
        + section("Impact", fields.get("impact", ""))
        + section("Implementation Notes", fields.get("implementation_notes", ""))
        + section("Acceptance / Closing Criteria", fields.get("acceptance_criteria", ""))
        + "## History / Resolution Notes\n"
    )


_BLANK_RECORD_BODY = (
    "# Implementation Issue Record\n\n"
    "## Rationale And Context\n\n"
    "## Impact\n\n"
    "## Implementation Notes\n\n"
    "## Acceptance / Closing Criteria\n\n"
    "## History / Resolution Notes\n"
)


# ---------------------------------------------------------------------------
# Record create / update
# ---------------------------------------------------------------------------

def _create_or_update_record(issue: dict) -> Path:
    """
    Create or update the local record file for a GitHub issue.

    ``issue`` is the issue object from the GitHub event payload.
    """
    number: int = issue["number"]
    labels = _normalise_labels(issue.get("labels", []))
    fields = _parse_issue_body(issue.get("body") or "")

    issue_type = _type_from_labels(labels)
    priority = (
        _priority_from_labels(labels)
        or _priority_from_body(fields)
        or "Low"
    )

    state = issue.get("state", "open")
    new_status = "Resolved" if state == "closed" else "Open"
    now = _now_iso()

    record_file = _record_path(number)

    if record_file.exists():
        # Targeted field update – preserves multi-line values we don't touch.
        updates: dict[str, str] = {
            "type": issue_type,
            "title": issue.get("title", ""),
            "github_url": issue.get("html_url", ""),
            "updated_at": now,
            "sync_state": "github-synced",
        }
        if fields.get("summary"):
            updates["summary"] = fields["summary"]
        if fields.get("scheduled_target"):
            updates["scheduled_target"] = fields["scheduled_target"]
        if fields.get("source"):
            updates["source"] = fields["source"]
        if fields.get("notes"):
            updates["notes"] = fields["notes"]

        # Update status only if not already in a manually-managed state.
        data, _ = _read_record(record_file)
        current_status = _strip(data.get("status"))
        if state == "closed":
            updates["status"] = "Resolved"
        elif current_status not in ("Scheduled", "In progress", "Blocked", "Resolved"):
            updates["status"] = "Open"

        lbl_priority = _priority_from_labels(labels)
        if lbl_priority:
            updates["priority"] = lbl_priority

        _update_record_fields(record_file, updates)
    else:
        # Brand-new record.
        summary = fields.get("summary") or issue.get("title", "")
        title = issue.get("title", "")
        data = {
            "issue_number": str(number),
            "legacy_id": "",
            "type": issue_type,
            "title": title,
            "summary": summary,
            "created_phase": fields.get("created_phase", ""),
            "source": fields.get("source", "GitHub issue"),
            "scheduled_target": fields.get("scheduled_target", ""),
            "status": new_status,
            "priority": priority,
            "github_url": issue.get("html_url", ""),
            "resolution_pr": "",
            "created_by": "automation",
            "created_at": issue.get("created_at", now),
            "updated_at": now,
            "sync_state": "github-synced",
            "notes": fields.get("notes", ""),
        }
        body = _build_record_body(fields)
        RECORDS_DIR.mkdir(parents=True, exist_ok=True)
        _write_record(record_file, data, body)

    return record_file


# ---------------------------------------------------------------------------
# Index / archive regeneration
# ---------------------------------------------------------------------------

def _index_row(data: dict[str, str]) -> str:
    """Format a row for the active index table."""
    issue_num = _strip(data.get("issue_number"))
    issue_type = _strip(data.get("type"))
    summary = _strip(data.get("summary"))
    created_phase = _strip(data.get("created_phase"))
    source = _strip(data.get("source"))
    scheduled_target = _strip(data.get("scheduled_target"))
    status = _strip(data.get("status"))
    priority = _strip(data.get("priority"))
    resolution_pr = _strip(data.get("resolution_pr"))
    notes = _strip(data.get("notes"))
    return (
        f"| {issue_num} | {issue_type} | {summary} | {created_phase} "
        f"| {source} | {scheduled_target} | {status} | {priority} "
        f"| {resolution_pr} | {notes} |"
    )


def _archive_row(data: dict[str, str]) -> str:
    """Format a row for the archive table."""
    issue_num = _strip(data.get("issue_number")) or "-"
    legacy_id = _strip(data.get("legacy_id"))
    issue_type = _strip(data.get("type"))
    summary = _strip(data.get("summary"))
    created_phase = _strip(data.get("created_phase"))
    priority = _strip(data.get("priority"))
    resolution_pr = _strip(data.get("resolution_pr"))
    updated_at = _strip(data.get("updated_at"))
    archived_date = updated_at[:10] if updated_at else ""
    notes = _strip(data.get("notes"))
    return (
        f"| {issue_num} | {legacy_id} | {issue_type} | {summary} "
        f"| {created_phase} | {priority} | {resolution_pr} | {archived_date} | {notes} |"
    )


def _sort_key(data: dict[str, str]) -> int:
    try:
        return int(_strip(data.get("issue_number", "0")))
    except ValueError:
        return 0


def _is_archive_table_end(line: str) -> bool:
    """
    Return True when ``line`` signals the end of the archive data table.

    A table-end line is non-empty, does not begin with a pipe (``|``), and is
    not an HTML comment (``<!--`` … ``-->``).  The markdownlint-disable comment
    that appears between the table header separator and the first data row must
    NOT be treated as a table-end signal, which is why the comment check is
    necessary.
    """
    stripped = line.strip()
    return bool(stripped) and not line.startswith("|") and not line.startswith("<!--")


def _read_legacy_archive_rows() -> list[str]:
    """
    Read the existing archive file and extract rows that have '-' as the
    issue number (legacy DEF-### entries without a real GitHub issue number).
    These rows are preserved verbatim since they have no matching record file.
    """
    if not ARCHIVE_FILE.exists():
        return []
    rows: list[str] = []
    in_table = False
    for line in ARCHIVE_FILE.read_text(encoding="utf-8").splitlines():
        if line.startswith("| Issue |") or line.startswith("| ----- |"):
            in_table = True
            continue
        if in_table and line.startswith("| - |"):
            rows.append(line)
        elif in_table and _is_archive_table_end(line):
            # A non-empty, non-table, non-comment line signals the end of the
            # archive data table.  The markdownlint-disable comment that appears
            # between the table header and the first data row must NOT be treated
            # as a table-end signal.
            in_table = False
    return rows


def regenerate_index() -> None:
    """Regenerate ImplementationIssuesIndex.md from all record files."""
    records = _load_all_records()
    today = _today()

    active = [
        r for r in records
        if _strip(r.get("status")) not in ("Resolved", "Archived")
    ]
    active.sort(key=_sort_key, reverse=True)

    lines = [
        "# Implementation Issues Index",
        "",
        "**Status**: Active source-of-truth for unresolved implementation issues  ",
        f"**Last Updated**: {today}",
        "",
        "## Operational Guidelines",
        "",
        "This file is the active summary view of all unresolved implementation"
        " issues tracked by the project.",
        "",
        "- Add and edit issue details in the individual issue records under"
        " `ImplementationIssues/Records/`.",
        "- GitHub issue number is canonical once assigned.",
        "- `Critical` and `High` items must keep a scheduled target.",
        "- Closed items are archived and removed from this active index.",
        "",
        "---",
        "",
        "## Active Implementation Issues",
        "",
        "| Issue | Type | Summary | Created Phase | Source | Scheduled Target"
        " | Status | Priority | Resolution PR | Notes |",
        "| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |",
    ]
    for r in active:
        lines.append(_index_row(r))

    lines += [
        "",
        "---",
        "",
        f"**Total Active Implementation Issues**: {len(active)}",
    ]

    INDEX_FILE.write_text("\n".join(lines) + "\n", encoding="utf-8")
    print(f"Index regenerated: {len(active)} active issue(s).")


def regenerate_archive() -> None:
    """Regenerate ImplementationIssuesArchive.md from all record files."""
    records = _load_all_records()
    today = _today()

    archived = [
        r for r in records
        if _strip(r.get("status")) in ("Resolved", "Archived")
    ]
    archived.sort(key=_sort_key, reverse=True)

    legacy_rows = _read_legacy_archive_rows()

    lines = [
        "# Implementation Issues Archive",
        "",
        "**Status**: Archive of resolved implementation issues  ",
        f"**Last Updated**: {today}",
        "",
        "| Issue | Legacy ID | Type | Summary | Created Phase | Priority"
        " | Resolution PR | Archived Date | Notes |",
        "| ----- | --------- | ---- | ------- | ------------- | --------"
        " | ------------- | ------------- | ----- |",
        "",
        "<!-- markdownlint-disable MD013 -->",
        "",
    ]
    for r in archived:
        lines.append(_archive_row(r))
    for row in legacy_rows:
        lines.append(row)

    lines += [
        "",
        "<!-- markdownlint-enable MD013 -->",
        "",
        "## Summary",
        "",
        f"**Total Archived Implementation Issues**: {len(archived) + len(legacy_rows)}",
        "",
    ]

    ARCHIVE_FILE.write_text("\n".join(lines) + "\n", encoding="utf-8")
    print(f"Archive regenerated: {len(archived)} resolved + {len(legacy_rows)} legacy.")


# ---------------------------------------------------------------------------
# Subcommand: sync-from-issue-event
# ---------------------------------------------------------------------------

def cmd_sync_from_issue_event(args: argparse.Namespace) -> None:
    """
    Sync a single GitHub issue into the local records.

    Reads the event JSON from ``--event-path``, checks for the
    ``implementation-issue`` label, and creates/updates the matching record.
    """
    with open(args.event_path, encoding="utf-8") as fh:
        event = json.load(fh)

    issue = event.get("issue")
    if not issue:
        print("No 'issue' key in event payload – skipping.", file=sys.stderr)
        return

    labels = _normalise_labels(issue.get("labels", []))
    label_names_lower = [lbl.lower() for lbl in labels]

    if _LABEL_IMPL_ISSUE not in label_names_lower:
        print(
            f"Issue #{issue['number']} does not carry the '{_LABEL_IMPL_ISSUE}' "
            "label – skipping.",
            file=sys.stderr,
        )
        return

    record_file = _create_or_update_record(issue)
    print(f"Synced issue #{issue['number']} → {record_file.relative_to(_REPO_ROOT)}")
    regenerate_index()
    regenerate_archive()


# ---------------------------------------------------------------------------
# Subcommand: sync-pr-resolution
# ---------------------------------------------------------------------------

# Matches GitHub's "magic closing" keywords followed by an issue reference:
# close/closed/closes, fix/fixed/fixes, resolve/resolved/resolves  + #N
_CLOSES_RE = re.compile(
    r"(?:close[sd]?|fix(?:e[sd])?|resolve[sd]?)\s+#(\d+)",
    re.IGNORECASE,
)


def _find_referenced_issues(text: str) -> list[int]:
    """Return issue numbers referenced by closing keywords."""
    return [int(m.group(1)) for m in _CLOSES_RE.finditer(text or "")]


def cmd_sync_pr_resolution(args: argparse.Namespace) -> None:
    """
    Link a merged PR to implementation issue records that it closes.

    Reads the merged-PR event JSON from ``--event-path``, looks for
    "Closes/Fixes/Resolves #N" references in the PR title and body, and
    updates the corresponding record files.
    """
    with open(args.event_path, encoding="utf-8") as fh:
        event = json.load(fh)

    pr = event.get("pull_request")
    if not pr:
        print("No 'pull_request' key in event payload – skipping.", file=sys.stderr)
        return

    pr_number = pr.get("number")
    pr_url = pr.get("html_url", "")
    pr_text = (pr.get("title") or "") + "\n" + (pr.get("body") or "")

    issue_nums = _find_referenced_issues(pr_text)
    if not issue_nums:
        print(f"PR #{pr_number}: no implementation-issue references found – skipping.")
        return

    updated = 0
    for num in issue_nums:
        record_file = _record_path(num)
        if not record_file.exists():
            print(
                f"PR #{pr_number}: no local record for issue #{num} – skipping.",
                file=sys.stderr,
            )
            continue
        _update_record_fields(
            record_file,
            {
                "resolution_pr": pr_url,
                "status": "Resolved",
                "updated_at": _now_iso(),
                "sync_state": "github-synced",
            },
        )
        print(f"Linked PR #{pr_number} → issue #{num}")
        updated += 1

    if updated:
        regenerate_index()
        regenerate_archive()


# ---------------------------------------------------------------------------
# Subcommand: sync-local-to-github
# ---------------------------------------------------------------------------

def _github_api(method: str, path: str, token: str, payload: dict | None = None) -> dict:
    """Make a GitHub REST API request using only stdlib."""
    url = f"https://api.github.com{path}"
    headers = {
        "Authorization": f"Bearer {token}",
        "Accept": "application/vnd.github+json",
        "X-GitHub-Api-Version": "2022-11-28",
        "Content-Type": "application/json",
    }
    data = json.dumps(payload).encode() if payload is not None else None
    req = urllib.request.Request(url, data=data, headers=headers, method=method)
    try:
        with urllib.request.urlopen(req) as resp:
            return json.loads(resp.read().decode())
    except urllib.error.HTTPError as exc:
        body = exc.read().decode()
        raise RuntimeError(f"GitHub API error {exc.code}: {body}") from exc


def _build_github_issue_body(data: dict[str, str]) -> str:
    """Build a GitHub issue body from record frontmatter, using the Forms heading style."""
    summary = _strip(data.get("summary"))
    created_phase = _strip(data.get("created_phase"))
    source = _strip(data.get("source"))
    scheduled_target = _strip(data.get("scheduled_target"))
    priority = _strip(data.get("priority")) or "Low"
    notes = _strip(data.get("notes"))

    lines = [
        "### Summary",
        "",
        summary,
        "",
        "### Created phase",
        "",
        created_phase,
        "",
        "### Source",
        "",
        source,
        "",
        "### Scheduled target",
        "",
        scheduled_target,
        "",
        "### Priority",
        "",
        priority,
        "",
    ]
    if notes:
        lines += ["### Notes", "", notes, ""]
    return "\n".join(lines)


def _is_local_only(data: dict[str, str]) -> bool:
    """Return True when a record has no assigned GitHub issue number or URL."""
    num = _strip(data.get("issue_number"))
    url = _strip(data.get("github_url"))
    return (not num or num == "0") or not url


def cmd_sync_local_to_github(args: argparse.Namespace) -> None:
    """
    Create GitHub issues for local records that do not yet have one.

    A record is considered local-only when its ``issue_number`` is 0,
    empty, or absent, or when its ``github_url`` is empty.
    """
    owner = args.owner
    repo = args.repo
    token = args.token

    records = _load_all_records()

    local_only = [r for r in records if _is_local_only(r)]

    if not local_only:
        print("No local-only records found – nothing to create.")
        return

    updated = 0
    for data in local_only:
        issue_type = _strip(data.get("type")) or "Open issue"
        title = _strip(data.get("title")) or _strip(data.get("summary")) or "Implementation issue"
        body = _build_github_issue_body(data)

        labels = [_LABEL_IMPL_ISSUE]
        type_label = _TYPE_TO_LABEL.get(issue_type)
        if type_label:
            labels.append(type_label)
        priority_label = _PRIORITY_TO_LABEL.get(_strip(data.get("priority")) or "Low")
        if priority_label:
            labels.append(priority_label)

        # Find the record file (match by issue_number or summary).
        num_str = _strip(data.get("issue_number"))
        record_file: Path | None = None
        if num_str and num_str != "0":
            candidate = _record_path(int(num_str))
            if candidate.exists():
                record_file = candidate
        if record_file is None:
            for f in RECORDS_DIR.glob("issue-*.md"):
                d, _ = _read_record(f)
                if _strip(d.get("summary")) == _strip(data.get("summary")):
                    record_file = f
                    break

        try:
            result = _github_api(
                "POST",
                f"/repos/{owner}/{repo}/issues",
                token,
                payload={"title": title, "body": body, "labels": labels},
            )
        except RuntimeError as exc:
            print(f"Failed to create GitHub issue for '{title}': {exc}", file=sys.stderr)
            continue

        new_number: int = result["number"]
        new_url: str = result["html_url"]
        print(f"Created GitHub issue #{new_number}: {title}")

        if record_file and record_file.exists():
            _update_record_fields(
                record_file,
                {
                    "issue_number": str(new_number),
                    "github_url": new_url,
                    "sync_state": "github-synced",
                    "updated_at": _now_iso(),
                },
            )
            # Rename the file to match the new issue number.
            new_path = _record_path(new_number)
            if record_file != new_path:
                record_file.rename(new_path)
                print(f"  Renamed {record_file.name} → {new_path.name}")

        updated += 1

    if updated:
        regenerate_index()
        regenerate_archive()
        print(f"Created {updated} GitHub issue(s). Index and archive updated.")


# ---------------------------------------------------------------------------
# CLI entry point
# ---------------------------------------------------------------------------

def main() -> None:
    parser = argparse.ArgumentParser(
        description="JAT implementation issues CI automation",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog=__doc__,
    )
    sub = parser.add_subparsers(dest="command", required=True)

    p_from = sub.add_parser(
        "sync-from-issue-event",
        help="Sync a GitHub issue event into local records.",
    )
    p_from.add_argument("--event-path", required=True, metavar="PATH",
                        help="Path to the GitHub event JSON file.")

    p_pr = sub.add_parser(
        "sync-pr-resolution",
        help="Link a merged PR to resolved implementation issue records.",
    )
    p_pr.add_argument("--event-path", required=True, metavar="PATH",
                      help="Path to the GitHub event JSON file.")

    p_to = sub.add_parser(
        "sync-local-to-github",
        help="Create GitHub issues for local-only records.",
    )
    p_to.add_argument("--owner", required=True, help="GitHub repository owner.")
    p_to.add_argument("--repo", required=True, help="GitHub repository name.")
    p_to.add_argument("--token", required=True, help="GitHub API token.")

    args = parser.parse_args()

    if args.command == "sync-from-issue-event":
        cmd_sync_from_issue_event(args)
    elif args.command == "sync-pr-resolution":
        cmd_sync_pr_resolution(args)
    elif args.command == "sync-local-to-github":
        cmd_sync_local_to_github(args)
    else:
        parser.print_help()
        sys.exit(1)


if __name__ == "__main__":
    main()
