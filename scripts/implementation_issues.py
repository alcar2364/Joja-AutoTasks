from __future__ import annotations

import argparse
import json
import os
import re
from dataclasses import dataclass
from datetime import datetime, timezone
from pathlib import Path
from typing import Iterable
from urllib import error, request


ROOT = Path(__file__).resolve().parents[1]
IMPLEMENTATION_ISSUES_DIR = ROOT / "Project" / "Tasks" / "ImplementationPlan" / "ImplementationIssues"
RECORDS_DIR = IMPLEMENTATION_ISSUES_DIR / "Records"
ACTIVE_INDEX_PATH = IMPLEMENTATION_ISSUES_DIR / "ImplementationIssuesIndex.md"
ARCHIVE_INDEX_PATH = IMPLEMENTATION_ISSUES_DIR / "ImplementationIssuesArchive.md"

TRACKING_LABEL = "implementation-issue"
TYPE_LABEL_PREFIX = "issue-type: "
LEGACY_TYPE_LABEL_PREFIX = "issue-type:"
PRIORITY_LABEL_PREFIX = "priority: "
LEGACY_PRIORITY_LABEL_PREFIX = "priority:"

TYPE_LABELS = {
    "Deferment": "issue-type: deferment",
    "Open issue": "issue-type: open-issue",
    "Ambiguity / question": "issue-type: ambiguity-question",
    "Architecture concern": "issue-type: architecture-concern",
    "Review follow-up": "issue-type: review-follow-up",
}

LOCAL_ID_PATTERN = re.compile(r"LOCAL-(\d{8})-(\d{3})")
SECTION_PATTERN = re.compile(r"^###\s+(?P<name>.+?)\s*$", re.MULTILINE)


@dataclass
class IssueRecord:
    issue_number: str
    legacy_id: str
    issue_type: str
    title: str
    summary: str
    created_phase: str
    source: str
    scheduled_target: str
    status: str
    priority: str
    github_url: str
    resolution_pr: str
    created_by: str
    created_at: str
    updated_at: str
    sync_state: str
    notes: str
    rationale: str
    impact: str
    implementation_notes: str
    acceptance_criteria: str
    history: str

    @property
    def sort_key(self) -> tuple[int, str]:
        priority_order = {"Critical": 0, "High": 1, "Medium": 2, "Low": 3}
        return (priority_order.get(self.priority, 99), self.issue_number)

    @property
    def is_archived(self) -> bool:
        return self.status == "Archived"


def now_iso() -> str:
    return datetime.now(timezone.utc).replace(microsecond=0).isoformat()


def ensure_directories() -> None:
    IMPLEMENTATION_ISSUES_DIR.mkdir(parents=True, exist_ok=True)
    RECORDS_DIR.mkdir(parents=True, exist_ok=True)


def escape_md_table(value: str) -> str:
    return value.replace("|", "\\|").replace("\n", " ").strip()


def sanitize_filename_part(value: str) -> str:
    return re.sub(r"[^A-Za-z0-9._-]+", "-", value).strip("-")


def normalize_multiline(value: str) -> str:
    return value.strip().replace("\r\n", "\n").replace("\r", "\n")


def parse_frontmatter(text: str) -> tuple[dict[str, str], str]:
    if not text.startswith("---\n"):
        raise ValueError("Record file is missing frontmatter")

    parts = text.split("\n---\n", 1)
    if len(parts) != 2:
        raise ValueError("Record file frontmatter is not closed")

    frontmatter_text = parts[0][4:]
    body = parts[1]
    data: dict[str, str] = {}
    for raw_line in frontmatter_text.splitlines():
        line = raw_line.strip()
        if not line or line.startswith("#"):
            continue
        key, _, raw_value = line.partition(":")
        data[key.strip()] = raw_value.strip().strip('"')
    return data, body


def parse_markdown_sections(body: str) -> dict[str, str]:
    matches = list(SECTION_PATTERN.finditer(body))
    sections: dict[str, str] = {}
    for index, match in enumerate(matches):
        start = match.end()
        end = matches[index + 1].start() if index + 1 < len(matches) else len(body)
        sections[match.group("name")] = body[start:end].strip()
    return sections


def load_record(path: Path) -> IssueRecord:
    frontmatter, body = parse_frontmatter(path.read_text(encoding="utf-8"))
    sections = parse_markdown_sections(body)
    return IssueRecord(
        issue_number=frontmatter.get("issue_number", ""),
        legacy_id=frontmatter.get("legacy_id", ""),
        issue_type=frontmatter.get("type", ""),
        title=frontmatter.get("title", ""),
        summary=frontmatter.get("summary", ""),
        created_phase=frontmatter.get("created_phase", ""),
        source=frontmatter.get("source", ""),
        scheduled_target=frontmatter.get("scheduled_target", ""),
        status=frontmatter.get("status", "Open"),
        priority=frontmatter.get("priority", "Low"),
        github_url=frontmatter.get("github_url", ""),
        resolution_pr=frontmatter.get("resolution_pr", ""),
        created_by=frontmatter.get("created_by", ""),
        created_at=frontmatter.get("created_at", ""),
        updated_at=frontmatter.get("updated_at", ""),
        sync_state=frontmatter.get("sync_state", "local-only"),
        notes=frontmatter.get("notes", ""),
        rationale=sections.get("Rationale And Context", ""),
        impact=sections.get("Impact", ""),
        implementation_notes=sections.get("Implementation Notes", ""),
        acceptance_criteria=sections.get("Acceptance / Closing Criteria", ""),
        history=sections.get("History / Resolution Notes", ""),
    )


def dump_record(record: IssueRecord) -> str:
    frontmatter_lines = [
        "---",
        f"issue_number: {record.issue_number}",
        f"legacy_id: \"{record.legacy_id}\"",
        f"type: \"{record.issue_type}\"",
        f"title: \"{record.title}\"",
        f"summary: \"{record.summary}\"",
        f"created_phase: \"{record.created_phase}\"",
        f"source: \"{record.source}\"",
        f"scheduled_target: \"{record.scheduled_target}\"",
        f"status: \"{record.status}\"",
        f"priority: \"{record.priority}\"",
        f"github_url: \"{record.github_url}\"",
        f"resolution_pr: \"{record.resolution_pr}\"",
        f"created_by: \"{record.created_by}\"",
        f"created_at: \"{record.created_at}\"",
        f"updated_at: \"{record.updated_at}\"",
        f"sync_state: \"{record.sync_state}\"",
        f"notes: \"{record.notes}\"",
        "---",
        "",
        "# Implementation Issue Record",
        "",
        "## Rationale And Context",
        "",
        record.rationale.strip(),
        "",
        "## Impact",
        "",
        record.impact.strip(),
        "",
        "## Implementation Notes",
        "",
        record.implementation_notes.strip(),
        "",
        "## Acceptance / Closing Criteria",
        "",
        record.acceptance_criteria.strip(),
        "",
        "## History / Resolution Notes",
        "",
        record.history.strip(),
        "",
    ]
    return "\n".join(frontmatter_lines)


def write_record(record: IssueRecord, path: Path | None = None) -> Path:
    ensure_directories()
    if path is None:
        record_name = f"issue-{record.issue_number}.md" if record.issue_number and record.issue_number != "null" else f"{sanitize_filename_part(record.title)}.md"
        path = RECORDS_DIR / record_name
    path.write_text(dump_record(record), encoding="utf-8")
    return path


def load_all_records() -> list[IssueRecord]:
    if not RECORDS_DIR.exists():
        return []
    records = [load_record(path) for path in sorted(RECORDS_DIR.glob("*.md"))]
    return records


def render_active_index(records: Iterable[IssueRecord]) -> str:
    active_records = sorted((record for record in records if not record.is_archived), key=lambda record: record.sort_key)
    lines = [
        "# Implementation Issues Index",
        "",
        "**Status**: Active source-of-truth for unresolved implementation issues  ",
        f"**Last Updated**: {datetime.now(timezone.utc).date().isoformat()}",
        "",
        "## Operational Guidelines",
        "",
        "This file is the active summary view of all unresolved implementation issues tracked by the project.",
        "",
        "- Add and edit issue details in the individual issue records under `ImplementationIssues/Records/`.",
        "- GitHub issue number is canonical once assigned.",
        "- `Critical` and `High` items must keep a scheduled target.",
        "- Closed items are archived and removed from this active index.",
        "",
        "---",
        "",
        "## Active Implementation Issues",
        "",
        "| Issue | Type | Summary | Created Phase | Source | Scheduled Target | Status | Priority | Resolution PR | Notes |",
        "| ----- | ---- | ------- | ------------- | ------ | ---------------- | ------ | -------- | ------------- | ----- |",
    ]

    for record in active_records:
        lines.append(
            "| {issue} | {issue_type} | {summary} | {created_phase} | {source} | {scheduled_target} | {status} | {priority} | {resolution_pr} | {notes} |".format(
                issue=escape_md_table(record.issue_number),
                issue_type=escape_md_table(record.issue_type),
                summary=escape_md_table(record.summary),
                created_phase=escape_md_table(record.created_phase),
                source=escape_md_table(record.source),
                scheduled_target=escape_md_table(record.scheduled_target),
                status=escape_md_table(record.status),
                priority=escape_md_table(record.priority),
                resolution_pr=escape_md_table(record.resolution_pr),
                notes=escape_md_table(record.notes),
            )
        )

    lines.extend([
        "",
        "---",
        "",
        f"**Total Active Implementation Issues**: {len(active_records)}",
        "",
    ])
    return "\n".join(lines)


def render_archive_index(records: Iterable[IssueRecord]) -> str:
    archived_records = sorted((record for record in records if record.is_archived), key=lambda record: record.issue_number)
    lines = [
        "# Implementation Issues Archive",
        "",
        "**Status**: Archive of resolved implementation issues  ",
        f"**Last Updated**: {datetime.now(timezone.utc).date().isoformat()}",
        "",
        "| Issue | Legacy ID | Type | Summary | Created Phase | Priority | Resolution PR | Archived Date | Notes |",
        "| ----- | --------- | ---- | ------- | ------------- | -------- | ------------- | ------------- | ----- |",
    ]

    for record in archived_records:
        archived_date = record.updated_at.split("T", 1)[0] if record.updated_at else ""
        lines.append(
            "| {issue} | {legacy_id} | {issue_type} | {summary} | {created_phase} | {priority} | {resolution_pr} | {archived_date} | {notes} |".format(
                issue=escape_md_table(record.issue_number),
                legacy_id=escape_md_table(record.legacy_id),
                issue_type=escape_md_table(record.issue_type),
                summary=escape_md_table(record.summary),
                created_phase=escape_md_table(record.created_phase),
                priority=escape_md_table(record.priority),
                resolution_pr=escape_md_table(record.resolution_pr),
                archived_date=escape_md_table(archived_date),
                notes=escape_md_table(record.notes),
            )
        )

    lines.append("")
    return "\n".join(lines)


def rebuild_indexes() -> None:
    records = load_all_records()
    ACTIVE_INDEX_PATH.write_text(render_active_index(records), encoding="utf-8")
    ARCHIVE_INDEX_PATH.write_text(render_archive_index(records), encoding="utf-8")


def parse_deferments_index_rows(index_path: Path) -> list[IssueRecord]:
    text = index_path.read_text(encoding="utf-8")
    rows: list[IssueRecord] = []
    in_table = False
    for line in text.splitlines():
        if line.startswith("| Deferment ID"):
            in_table = True
            continue
        if not in_table:
            continue
        if not line.startswith("|") or line.startswith("| ------------"):
            continue
        parts = [part.strip() for part in line.strip().strip("|").split("|")]
        if len(parts) != 8:
            continue
        legacy_id, description, deferred_from, source_checklist, scheduled_target, _, issue_number, notes = parts
        normalized_issue_number = issue_number.lstrip("#")
        rows.append(
            IssueRecord(
                issue_number=normalized_issue_number,
                legacy_id=legacy_id,
                issue_type="Deferment",
                title=f"Implementation Issue {normalized_issue_number}: {description}",
                summary=description,
                created_phase=deferred_from,
                source=source_checklist,
                scheduled_target=scheduled_target,
                status="Scheduled",
                priority="Low",
                github_url="",
                resolution_pr="",
                created_by="migration",
                created_at=now_iso(),
                updated_at=now_iso(),
                sync_state="github-synced",
                notes=notes,
                rationale="Migrated from the legacy Deferments Index.",
                impact="",
                implementation_notes="",
                acceptance_criteria="Resolve the deferred work and archive the issue.",
                history="Migrated automatically from the legacy deferment system.",
            )
        )
    return rows


def migrate_deferments() -> None:
    ensure_directories()
    deferments_index_path = ROOT / "Project" / "Tasks" / "ImplementationPlan" / "Deferments" / "DefermentsIndex.md"
    for record in parse_deferments_index_rows(deferments_index_path):
        write_record(record, RECORDS_DIR / f"issue-{record.issue_number}.md")
    rebuild_indexes()


def label_names_from_issue(issue: dict) -> set[str]:
    return {label.get("name", "") for label in issue.get("labels", [])}


def issue_type_from_labels(labels: set[str]) -> str:
    for issue_type, label_name in TYPE_LABELS.items():
        legacy_label_name = label_name.replace(TYPE_LABEL_PREFIX, LEGACY_TYPE_LABEL_PREFIX, 1)
        if label_name in labels or legacy_label_name in labels:
            return issue_type
    return "Open issue"


def priority_from_labels(labels: set[str]) -> str:
    for priority in ("Critical", "High", "Medium", "Low"):
        spaced_label = f"{PRIORITY_LABEL_PREFIX}{priority.lower()}"
        legacy_label = f"{LEGACY_PRIORITY_LABEL_PREFIX}{priority.lower()}"
        if spaced_label in labels or legacy_label in labels:
            return priority
    return "Low"


def normalize_priority(value: str) -> str:
    normalized = value.strip().lower()
    priority_map = {
        "critical": "Critical",
        "high": "High",
        "medium": "Medium",
        "low": "Low",
    }
    return priority_map.get(normalized, "")


def parse_issue_form_body(body: str) -> dict[str, str]:
    sections = parse_markdown_sections(body)
    normalized: dict[str, str] = {}
    for key, value in sections.items():
        normalized[key.strip().lower()] = value.strip()
    return normalized


def validate_priority(record: IssueRecord) -> None:
    if record.priority in {"Critical", "High"} and not record.scheduled_target.strip():
        raise ValueError(f"{record.priority} issues must include a scheduled target")


def sync_from_issue_event(event_path: Path) -> None:
    payload = json.loads(event_path.read_text(encoding="utf-8"))
    issue = payload.get("issue")
    if not issue:
        raise ValueError("Event payload does not contain an issue")

    labels = label_names_from_issue(issue)
    if TRACKING_LABEL not in labels:
        return

    sections = parse_issue_form_body(issue.get("body", ""))
    issue_number = str(issue["number"])
    issue_type = issue_type_from_labels(labels)
    priority = priority_from_labels(labels)
    if priority == "Low":
        parsed_priority = normalize_priority(sections.get("priority", ""))
        if parsed_priority:
            priority = parsed_priority
    status = "Archived" if issue.get("state") == "closed" else sections.get("status", "Open")

    record = IssueRecord(
        issue_number=issue_number,
        legacy_id=sections.get("legacy id", ""),
        issue_type=issue_type,
        title=issue.get("title", f"Implementation Issue {issue_number}"),
        summary=sections.get("summary", issue.get("title", "")),
        created_phase=sections.get("created phase", ""),
        source=sections.get("source", "GitHub issue template"),
        scheduled_target=sections.get("scheduled target", ""),
        status=status,
        priority=priority,
        github_url=issue.get("html_url", ""),
        resolution_pr=sections.get("resolution pr", ""),
        created_by=issue.get("user", {}).get("login", ""),
        created_at=issue.get("created_at", now_iso()),
        updated_at=issue.get("updated_at", now_iso()),
        sync_state="github-synced",
        notes=sections.get("notes", ""),
        rationale=sections.get("rationale and context", ""),
        impact=sections.get("impact", ""),
        implementation_notes=sections.get("implementation notes", ""),
        acceptance_criteria=sections.get("acceptance / closing criteria", ""),
        history=sections.get("history / resolution notes", ""),
    )
    validate_priority(record)
    write_record(record, RECORDS_DIR / f"issue-{issue_number}.md")
    rebuild_indexes()


def github_api_request(method: str, url: str, token: str, payload: dict | None = None) -> dict:
    headers = {
        "Accept": "application/vnd.github+json",
        "Authorization": f"Bearer {token}",
        "X-GitHub-Api-Version": "2022-11-28",
        "User-Agent": "joja-auto-tasks-implementation-issues",
    }
    data = json.dumps(payload).encode("utf-8") if payload is not None else None
    req = request.Request(url, data=data, headers=headers, method=method)
    try:
        with request.urlopen(req) as response:
            body = response.read().decode("utf-8")
            return json.loads(body) if body else {}
    except error.HTTPError as exc:
        details = exc.read().decode("utf-8", errors="replace")
        raise RuntimeError(f"GitHub API request failed: {exc.code} {details}") from exc


def render_issue_body(record: IssueRecord) -> str:
    return "\n\n".join(
        [
            "### Summary\n" + record.summary,
            "### Created phase\n" + record.created_phase,
            "### Source\n" + record.source,
            "### Scheduled target\n" + record.scheduled_target,
            "### Priority\n" + record.priority,
            "### Notes\n" + (record.notes or ""),
            "### Rationale and context\n" + (record.rationale or ""),
            "### Impact\n" + (record.impact or ""),
            "### Implementation notes\n" + (record.implementation_notes or ""),
            "### Acceptance / closing criteria\n" + (record.acceptance_criteria or ""),
            "### History / resolution notes\n" + (record.history or ""),
        ]
    )


def create_issue_for_record(record_path: Path, owner: str, repo: str, token: str) -> None:
    record = load_record(record_path)
    if record.issue_number and record.issue_number != "null":
        return

    validate_priority(record)

    labels = [TRACKING_LABEL, TYPE_LABELS.get(record.issue_type, TYPE_LABELS["Open issue"]), f"{PRIORITY_LABEL_PREFIX}{record.priority.lower()}"]
    payload = {
        "title": record.title,
        "body": render_issue_body(record),
        "labels": labels,
    }
    issue = github_api_request("POST", f"https://api.github.com/repos/{owner}/{repo}/issues", token, payload)
    record.issue_number = str(issue["number"])
    record.github_url = issue.get("html_url", "")
    record.sync_state = "github-synced"
    record.updated_at = now_iso()

    new_path = RECORDS_DIR / f"issue-{record.issue_number}.md"
    write_record(record, new_path)
    if new_path != record_path and record_path.exists():
        record_path.unlink()
    rebuild_indexes()


def sync_local_to_github(owner: str, repo: str, token: str) -> None:
    ensure_directories()
    for path in sorted(RECORDS_DIR.glob("*.md")):
        record = load_record(path)
        if not record.issue_number or record.issue_number == "null":
            create_issue_for_record(path, owner, repo, token)


def sync_pr_resolution(event_path: Path) -> None:
    payload = json.loads(event_path.read_text(encoding="utf-8"))
    pull_request = payload.get("pull_request")
    if not pull_request or not pull_request.get("merged"):
        return

    body = pull_request.get("body", "") or ""
    title = pull_request.get("title", "") or ""
    issue_numbers = sorted(set(re.findall(r"#(\d+)", f"{title}\n{body}")))
    if not issue_numbers:
        return

    pr_reference = f"#{pull_request.get('number', '')}"
    for issue_number in issue_numbers:
        record_path = RECORDS_DIR / f"issue-{issue_number}.md"
        if not record_path.exists():
            continue
        record = load_record(record_path)
        record.resolution_pr = pr_reference
        if record.history:
            record.history = f"{record.history}\n\nLinked resolution PR: {pr_reference}".strip()
        else:
            record.history = f"Linked resolution PR: {pr_reference}"
        record.updated_at = now_iso()
        write_record(record, record_path)

    rebuild_indexes()


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(description="Manage Implementation Issues records and indexes.")
    subparsers = parser.add_subparsers(dest="command", required=True)

    subparsers.add_parser("migrate-deferments", help="Migrate legacy deferments into Implementation Issues.")
    subparsers.add_parser("rebuild-indexes", help="Rebuild the active and archive indexes from records.")

    sync_from_issue = subparsers.add_parser("sync-from-issue-event", help="Sync a GitHub issue event into local records.")
    sync_from_issue.add_argument("--event-path", required=True)

    sync_to_github = subparsers.add_parser("sync-local-to-github", help="Create GitHub issues for local-only records.")
    sync_to_github.add_argument("--owner", required=True)
    sync_to_github.add_argument("--repo", required=True)
    sync_to_github.add_argument("--token", required=True)

    sync_pr = subparsers.add_parser("sync-pr-resolution", help="Link merged PRs to tracked implementation issue records.")
    sync_pr.add_argument("--event-path", required=True)

    return parser


def main() -> None:
    parser = build_parser()
    args = parser.parse_args()

    if args.command == "migrate-deferments":
        migrate_deferments()
        return
    if args.command == "rebuild-indexes":
        rebuild_indexes()
        return
    if args.command == "sync-from-issue-event":
        sync_from_issue_event(Path(args.event_path))
        return
    if args.command == "sync-local-to-github":
        sync_local_to_github(args.owner, args.repo, args.token)
        return
    if args.command == "sync-pr-resolution":
        sync_pr_resolution(Path(args.event_path))
        return

    parser.error("Unknown command")


if __name__ == "__main__":
    main()