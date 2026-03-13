# Implementation Issues

This folder is the canonical home for unresolved and archived
implementation issues.

## Purpose

The Implementation Issues system tracks work discovered during phase
checklist creation, implementation, code review, automated review,
and direct developer investigation.

This system replaces the old Deferments-only structure.

## Canonical Model

- GitHub issue number is the canonical identifier once assigned.
- A detailed local record exists under `ImplementationIssues/Records/`
  for each tracked issue.
- `ImplementationIssuesIndex.md` is the active summary view.
- `ImplementationIssuesArchive.md` is the resolved summary view.

## Issue Types

- `Deferment`
- `Open issue`
- `Ambiguity / question`
- `Architecture concern`
- `Review follow-up`

## Statuses

- `Open`
- `Scheduled`
- `In progress`
- `Blocked`
- `Resolved`
- `Archived`

Closed issues are archived immediately.

## Priorities

- `Critical`: workflow blockers, security issues, or build-breaking
  issues that cannot wait until the next phase
- `High`: high technical debt that must be solved in the next phase
  regardless of scope
- `Medium`: issues that should be solved before specific later phase work
- `Low`: no operational impact; handled during normal phase implementation

`Critical` and `High` issues must include a scheduled target.

## Required Summary Fields

- `summary`
- `created_phase`
- `source`
- `scheduled_target`
- `status`
- `priority`

## Source Field

`source` captures where the issue originated. Typical values include:

- `Phase checklist Step 4D`
- `PR #145 review`
- `Automated review`
- `Developer`
- `GitHub issue template`

## Sync Model

- GitHub-first creation is the preferred path.
- Local-first creation is supported for records without an issue number.
- GitHub Actions are the primary automation mechanism.
- `development` is the canonical branch for implementation issue records and sync automation.
- Agentic workflow tooling is fallback-only if standard GitHub Actions
  cannot safely perform a required task.

## Branching Model

- All implementation-issue workflow automation checks out and pushes `development` explicitly.
- Issue events and merged PR events do not write directly to the repository default branch unless that branch is also `development`.
- Promote implementation-issue record changes to `main` through the normal `development` merge flow.

## GitHub Templates

The repository supports both guided issue forms and classic markdown
issue templates.

- Guided templates live in `.github/ISSUE_TEMPLATE/*.yml`.
- Manual markdown templates live in `.github/ISSUE_TEMPLATE/*-manual.md`.
- Both template styles use matching headings so the automation can parse
  either path consistently.

## Choosing A Template

- Use a guided `.yml` form when creating a new issue directly in GitHub and
  you want structured prompts for each field.
- Use a manual `-manual.md` template when you already have issue content from
  a checklist, review, or external note and want a faster paste-and-edit flow.
- The automation treats both styles as equivalent as long as the section
  headings are preserved.
- Type labels are applied by the templates. Priority labels should be added in
  GitHub after creation so the issue carries both the body field and the label.

See `GitHubLabels.md` in this folder for the label catalog and recommended
GitHub UI setup.

## Migration

Legacy `DEF-###` deferments are migrated into this system as
`type: Deferment` records and keep `legacy_id` for compatibility.

## Compatibility Window

The legacy `Deferments/` folder remains temporarily as a compatibility
reference only. New issue tracking work should use this folder and its
workflows.
