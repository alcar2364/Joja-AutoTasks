# Core Flows - Planning Artifact Alignment

## Flow 1: Design Change -> Phase Assignment

1. Detect design-guide section updates.
2. Map each changed capability to existing phase owner in Section 21.
3. If capability is cross-cutting, map primary owner + dependent phase(s).
4. Update Section 21 capability mapping notes before checklist edits.

## Flow 2: Phase Assignment -> ImplementationIssues Scheduling

1. Compare Section 21 phase targets with ImplementationIssues scheduled_target.
2. Resolve conflicts by explicit retargeting in issue records/index (no implicit interpretation).
3. Keep duplicates merged/closed consistently across index and records.

## Flow 3: Scheduling -> Checklist Scope

1. Pull active issues for the target phase from ImplementationIssues index.
2. Validate checklist target issue set against Section 21 carryover notes.
3. Ensure checklist includes both in-scope issue work and explicit out-of-scope exclusions.

## Flow 4: Checklist Governance -> Future Inheritance

1. Keep checklist-creation instruction and skill aligned with Section 21 carryover policy.
2. Require explicit design-section-to-phase mapping check during checklist drafting.
3. Require explicit issue-scheduling conflict check before publishing checklist.

## Decision Points

- If a capability is missing from explicit phase text but present in design sections: add to existing best-fit phase.
- If ImplementationIssues scheduling differs from Section 21: update issue scheduling metadata or revise Section 21 note; do not leave unresolved.
- If capability is V2/Later in design notes but has near-term issue scheduling: preserve canonical source rule and add explicit retarget action.
