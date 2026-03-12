# GitHub Labels

This file documents the GitHub labels used by the Implementation Issues
system so repository setup stays consistent with the templates and
automation.

## Required Labels

### Core Tracking Label

| Label | Purpose | Recommended Color |
| ----- | ------- | ----------------- |
| `implementation-issue` | Marks issues that belong to the Implementation Issues system. The sync workflow ignores issues without this label. | `0E8A16` |

### Type Labels

| Label | Purpose | Recommended Color |
| ----- | ------- | ----------------- |
| `issue-type: deferment` | Work intentionally deferred to a later phase or later implementation window. | `C2E0C6` |
| `issue-type: open-issue` | General implementation issue that is not specifically a deferment. | `D4C5F9` |
| `issue-type: ambiguity-question` | Unresolved design, behavior, or terminology question. | `F9D0C4` |
| `issue-type: architecture-concern` | Architectural boundary, ownership, or layering concern. | `BFDADC` |
| `issue-type: review-follow-up` | Work deferred from human or automated review findings. | `FEF2C0` |

### Priority Labels

| Label | Purpose | Recommended Color |
| ----- | ------- | ----------------- |
| `priority: critical` | Blockers, security issues, or build-breaking issues that cannot wait until the next phase. | `B60205` |
| `priority: high` | High technical debt that must be solved in the next phase regardless of scope. | `D93F0B` |
| `priority: medium` | Issues that should be solved before specific later phase work. | `FBCA04` |
| `priority: low` | Issues with no operational impact; handled during normal phase implementation. | `0E8A16` |

## Setup Notes

- Every tracked implementation issue should carry `implementation-issue`.
- Every tracked implementation issue should carry exactly one type label.
- Every tracked implementation issue should carry exactly one priority label.
- Guided issue forms and manual markdown templates apply the type label.
- Priority is captured in the issue body by the templates; add the matching
  priority label manually in GitHub after issue creation.
- The sync automation can read priority from the body, but the priority label
  should still be applied so the GitHub UI remains filterable.

## Repository Setup Checklist

Create labels in this order so the templates and workflow filters are usable as
soon as possible:

1. Create `implementation-issue` first. This is the gating label used by the
  sync workflow.
2. Create all five `issue-type: *` labels next so every template can apply its
  type label immediately.
3. Create the four `priority: *` labels last so maintainers can add them after
  issue creation.

After labels exist in GitHub:

1. Verify all guided templates are visible in the GitHub issue chooser.
2. Verify all manual `-manual.md` templates are visible in the classic issue
  template list.
3. Create one test issue from either template style and confirm it receives
  `implementation-issue` and the expected `issue-type:*` label.
4. Add a `priority:*` label manually to that test issue and confirm the issue
  remains compatible with the local sync model.

## Template Dependencies

Each template depends on the labels below being present in GitHub.

| Template | Required Auto-Applied Labels | Priority Label Applied Later |
| -------- | ---------------------------- | ---------------------------- |
| `deferment.yml` / `deferment-manual.md` | `implementation-issue`, `issue-type: deferment` | one `priority: <level>` label |
| `open-issue.yml` / `open-issue-manual.md` | `implementation-issue`, `issue-type: open-issue` | one `priority: <level>` label |
| `ambiguity-question.yml` / `ambiguity-question-manual.md` | `implementation-issue`, `issue-type: ambiguity-question` | one `priority: <level>` label |
| `architecture-concern.yml` / `architecture-concern-manual.md` | `implementation-issue`, `issue-type: architecture-concern` | one `priority: <level>` label |
| `review-follow-up.yml` / `review-follow-up-manual.md` | `implementation-issue`, `issue-type: review-follow-up` | one `priority: <level>` label |

## GitHub UI Setup

When creating labels in GitHub, use:

- Label name: exact text shown above
- Description: copy the Purpose text from this file
- Color: use the recommended color or an equivalent palette if the repository
  already has a label color convention
