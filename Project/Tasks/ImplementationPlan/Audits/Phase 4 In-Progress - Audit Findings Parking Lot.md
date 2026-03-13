# Phase 4 In-Progress - Audit Findings Parking Lot

| **Detail**            | **Description**                                                                                                                                                         |
| --------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Owner:**            | User-owned follow-up queue                                                                                                                                              |
| **Purpose:**          | Hold queued follow-up work for Sections 01-09 audit findings discovered during active Phase 4 implementation without silently folding that work into the current phase. |
| **Artifact Status:**  | Living parking-lot artifact for active Phase 4 work.                                                                                                                    |
| **Review Mode:**      | post                                                                                                                                                                    |
| **Review Status:**    | pending                                                                                                                                                                 |
| **Review Rationale:** | Artifact retargeting should be checked after edit for scope and sequencing fit.                                                                                         |

## Use Constraint

This file is a parking-lot and deferred-resolution queue for findings discovered
during active Phase 4 work. It is not an execution-ready atomic commit
checklist and it is not a default extension of the current Phase 4
implementation pass.

Revisit these queued items after Phase 4 closes, or earlier only when the user
explicitly chooses to pull one item forward.

This is a living artifact and is expected to accumulate additional triage items
before Phase 4 completion.

## Queue Bucket 1 - Tracker Repair Follow-Through

- [x] Complete the `ImplementationIssues` tracker repair so audit-driven
      tracking can return to the canonical system.

### Current State for Bucket 1

- [x] `ImplementationIssues/Records/*.md` are no longer being treated as a
      merge-conflict blocker for this migration follow-up pass.
- [x] `ImplementationIssuesIndex.md` and the corresponding issue records are
      again the canonical home for synchronization updates.
- [x] Future audit-driven tracking changes should go through the
      `ImplementationIssues` system directly instead of a temporary repair
      bucket.

## Queue Bucket 2 - Create and Update Audit Tracking Coverage

- [ ] Resume canonical tracking for all 12 findings using the triage mapping in
      the in-progress triage report.

### Revisit Conditions for Bucket 2

- [ ] Existing issue coverage is updated later for findings 4, 6, 7, 10, and
      11 through issues 104, 105, 111, 112, 113, 114, 115, and 119 as mapped.
- [ ] Existing partial or adjacent coverage is expanded later for finding 1
      with issues 117 and 131, plus a new dedicated record or GitHub issue.
- [ ] New dedicated issue records or GitHub issues are created later for
      findings 2, 3, 5, 8, 9, and 12.
- [ ] Each finding has one clear tracking home, scheduled target, and open/next
      status after synchronization resumes.

## Queue Bucket 3 - Resolve ManualTask Identifier Contract

- [ ] Close the manual-task naming and counter contract split without reopening
      active Phase 4 implementation scope.

### Revisit Conditions for Bucket 3

- [ ] The canonical terminology is explicitly settled between `ManualTask_N`
      and any legacy variants.
- [ ] Persistence, reset, and regression semantics for the manual counter are
      explicitly stated.
- [ ] Sections, code assumptions, and tests point at the same identifier
      contract after the resolution pass.

## Queue Bucket 4 - Resolve Section 07 Command-Only and DayKey Drift

- [ ] Normalize Section 07 wording so it matches the command-only contract and
      fix the DayKey example drift.

### Revisit Conditions for Bucket 4

- [ ] Section 07 no longer says "commands or actions only" where the canonical
      contract is command-only.
- [ ] Section 07 DayKey and TaskID examples match the canonical DayKey format.
- [ ] This cleanup is completed before Phase 6 treats Section 07 wording as
      normative guidance.

## Queue Bucket 5 - Resolve Day-Start Lifecycle Ownership

- [ ] Close the open question about which layer normatively owns day-start
      lifecycle behavior.

### Revisit Conditions for Bucket 5

- [ ] One owning layer is named for day-start lifecycle responsibility.
- [ ] The handoff boundary between lifecycle forwarding and state ownership is
      documented clearly.
- [ ] Phase 4 and later phases can reference the same day-start contract
      without adding local interpretations.

## Queue Bucket 6 - Resolve Phase 5-Facing Contracts

- [ ] Resolve the contracts that Phase 5 and adjacent generator work need before
      behavior becomes harder to unwind.

### Revisit Conditions for Bucket 6

- [ ] Ordering behavior is explicitly defined and aligned with the intended
      implementation path.
- [ ] Deadline data is anchored to a clear core-model home and source of truth.
- [ ] The guaranteed built-in generator set for V1 is explicitly listed or
      bounded.
- [ ] Existing issue coverage for findings 4, 6, and 10 is updated after the
      contract decisions are made.

## Queue Bucket 7 - Resolve Phase 6-Facing Contracts

- [ ] Resolve the rule-boundary contracts before Phase 6 and Phase 10 depend on
      them.

### Revisit Conditions for Bucket 7

- [ ] RuleId value-object and serialization expectations are made consistent
      across the relevant sections.
- [ ] Rule evaluation wording stays command-only and does not reopen action
      terminology drift.
- [ ] Existing issue coverage for finding 11 is updated and new tracking is
      created later for finding 5.

## Queue Bucket 8 - Resolve Phase 7 and Phase 11 Persistence Contracts

- [ ] Resolve the persistence and rehydration contracts that must be stable
      before deeper save/load work proceeds.

### Revisit Conditions for Bucket 8

- [ ] V1 history persistence behavior is explicitly defined across the affected
      sections.
- [ ] Daily baseline persistence ownership and scope are explicitly defined.
- [ ] Rehydration orphan handling is defined for `CompletedTasks`,
      `PinnedTasks`, and any runtime cache state when regenerated tasks differ.
- [ ] Existing issue coverage for finding 7 is updated and new tracking is
      created later for findings 2 and 8.

## Queue Bucket 9 - Resolve Completed-Task Presentation Semantics

- [ ] Close the completed-task presentation gap before Phase 9 and Phase 11
      make incompatible assumptions.

### Revisit Conditions for Bucket 9

- [ ] Completed-task presentation behavior is defined at the level needed for
      Phase 9 UI work.
- [ ] The presentation contract is cross-checked against Phase 11 persistence
      behavior.
- [ ] New dedicated tracking is created later if no existing issue covers this
      finding.

## Queue Maintenance Checkpoint

- [x] Tracker repair is complete; new tracking updates should go through the
      `ImplementationIssues` system directly.
- [ ] All 12 known findings retain a clear queued home in this artifact family.
- [ ] Newly discovered Phase 4 audit findings are added here or to the paired
      triage report before Phase 4 closes.
- [ ] No queued item is treated as part of active Phase 4 implementation unless
      the user explicitly pulls it forward.
