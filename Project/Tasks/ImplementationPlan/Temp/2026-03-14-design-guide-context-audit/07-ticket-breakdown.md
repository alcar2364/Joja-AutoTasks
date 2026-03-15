# Ticket Breakdown - Docs-Only Update Execution Plan

## Ticket 1 - Reconcile Phase Scheduling Metadata

- Scope:
  - Align canonical issue scheduling/duplicate metadata across ImplementationIssues index/records for #100, #159, #86.
- In scope:
  - ImplementationIssues index row updates.
  - issue-100 and issue-86/issue-159 record metadata notes using one canonical duplicate model.
- Out of scope:
  - Runtime code changes.
- Dependencies:
  - None.

## Ticket 2 - Add Explicit Capability Ownership for Section 20.8 and 20.10

- Scope:
  - Add explicit phase/domain ownership notes in Section 21 for V1 toast routing and onboarding.
- In scope:
  - Section 21 capability map or phase text edits only.
- Out of scope:
  - UI runtime implementation details.
- Dependencies:
  - Ticket 1 recommended first for issue consistency.

## Ticket 3 - Add Cross-Cutting Mapping Table for Sections 12, 15, 16, 19

- Scope:
  - Add concise ownership matrix in Section 21 to prevent future checklist drift.
- In scope:
  - Documentation table and references only.
- Out of scope:
  - Any phase reorder.
- Dependencies:
  - Ticket 2 optional; can run in parallel if wording does not collide.

## Ticket 4 - Phase 4 Checklist Reconciliation

- Scope:
  - Align target issue list and explicit exclusions to the reconciled issue set.
- In scope:
  - Phase 4 checklist metadata and gate statements.
- Out of scope:
  - Step-by-step technical implementation expansion beyond phase scope.
- Dependencies:
  - Ticket 1 required.

## Ticket 5 - Checklist Governance Hardening

- Scope:
  - Update checklist-creation instruction + skill to enforce phase mapping and issue consistency checks.
- In scope:
  - .github/instructions/atomic-commit-execution-checklist-creation.instructions.md
  - .github/skills/atomic-commit-execution-checklist-creation/SKILL.md
- Out of scope:
  - Agent ecosystem redesign.
- Dependencies:
  - Ticket 1 required; Ticket 2/3 recommended to reference finalized mapping.

## Proposed Execution Order

1. Ticket 1
1. Ticket 2 and Ticket 3
1. Ticket 4
1. Ticket 5

## Definition of Done for This Breakdown

- All planning docs reference one canonical schedule for Phase 4 carryover issues.
- #86 is retained as historical merged-reference only (non-active), and #159 is the sole active tracker for that merged scope.
- Section 20.8 and 20.10 are explicitly represented in implementation planning ownership.
- Future checklist creation guidance includes mandatory design-section and issue-index consistency checks.
