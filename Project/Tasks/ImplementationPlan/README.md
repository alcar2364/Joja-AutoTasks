# Implementation Plan Folder Rules

This folder stores implementation plans, execution checklists, audit artifacts,
and the canonical implementation-issue tracker for Joja AutoTasks.

## Required Structure

- Keep phase execution checklists and other atomic execution checklist artifacts
  as `.md` files directly in this folder.
- Store audit and triage artifacts under `Audits/`.
- Store active and archived implementation-issue tracking artifacts under
  `ImplementationIssues/`.
- Do not recreate `Deferments/`; that legacy tracking system is retired.

## Naming Guidance

- Use clear, descriptive filenames that reflect the artifact intent.
- For phase checklists, prefer `Phase N - ...` naming for natural sort order.
- For audit artifacts, keep the existing phase-oriented descriptive names and
  place them under `Audits/`.

## Canonical Tracking Rules

- New unresolved implementation issues belong in the
  `ImplementationIssues/` system.
- Legacy `DEF-###` identifiers remain valid as compatibility identifiers and
  should be preserved in `legacy_id` fields when an issue originated in the
  retired deferment system.
- Historical deferment records that never received GitHub issue numbers belong
  in `ImplementationIssues/ImplementationIssuesArchive.md`.

## Scope

- This rule applies to all future implementation plan, checklist, audit, and
  implementation-issue tracking artifacts.
