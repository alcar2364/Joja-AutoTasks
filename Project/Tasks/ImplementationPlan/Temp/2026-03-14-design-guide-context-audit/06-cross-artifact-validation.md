# Cross-Artifact Validation - Findings and Required Updates

## Overall Assessment

Artifacts are directionally coherent, but there are explicit scheduling and ownership seams that need reconciliation before further checklist drafting.

## Findings (Ordered by Severity)

1. Scheduling conflict: issue #100 phase target mismatch

- Evidence:
  - Section 21 maps #100 into Phase 4 carryover.
  - ImplementationIssues index still schedules #100 as Phase 3+.
- Impact: Phase-scoped checklist generation can include/exclude #100 inconsistently.
- Required update: set a single canonical scheduled target for #100 and align Section 21 + index + record.

1. Duplicate-active conflict: #86 and #159 both present as active Phase 4 issues

- Evidence:
  - Section 21 says #159 includes merged duplicate #86.
  - ImplementationIssues index still has active row for #86.
- Impact: audit gates can double-count security hardening work.
- Required update: archive or mark #86 as merged/resolved reference, keep #159 as canonical active tracker.

1. Capability ownership gap: Section 20 V1 toast/onboarding not explicit in Section 21 phase text

- Evidence:
  - Section 20 defines V1 native toast behavior and first-run onboarding requirements.
  - Section 21 phases do not name these capabilities explicitly.
- Impact: checklist authors can miss these V1 capabilities.
- Required update: add explicit ownership in existing phases (Phase 9 for toast behavior, Phase 8 with Phase 7 persistence dependency for onboarding).

1. Workflow naming drift: pre-validation and execute naming mismatch

- Evidence:
  - Workflow folder has prd-validation.md and execution.md, not pre-validation.md or execute.md.
- Impact: future planning prompts can request non-existent workflow artifacts.
- Required update: add alias note or rename guidance in workflow index/readme.

## Required Updates by Target Artifact

### A) Section 21 - Implementation Plan

- Add explicit capability ownership notes for:
  - Section 20.8 V1 toast routing.
  - Section 20.10 onboarding and OnboardingAcknowledged dependency.
- Add concise cross-cutting mapping table for Sections 12, 15, 16, 19.
- Keep no-drop sequencing unchanged.

### B) ImplementationIssuesIndex

- Retarget #100 to the agreed canonical phase (currently implied as Phase 4 by Section 21).
- Retire duplicate-active #86 entry in favor of #159 (or explicitly mark as merged and non-active).
- Add note-level reconciliation for #122/#123/#124 vs V2 boundary wording in Section 21.

### C) Phase 4 Atomic Checklist

- Confirm target issue set only references canonical active rows after index reconciliation.
- Add explicit out-of-scope statement for toast/event routing and onboarding/gamepad implementation work to avoid phase leakage.
- Keep ConfigLoader hardening scoped to exception path hardening only.

### D) Governing checklist-creation sources

- Update instruction and skill to require:
  - section-to-phase coverage check against Section 21
  - issue-scheduling conflict check against ImplementationIssues index
  - merged-duplicate detection before checklist publication

## Open Questions for Maintainer Decision

- Should #100 be definitively Phase 4, or should Section 21 be revised back to Phase 3+ wording?
- Should #86 be archived immediately or kept as non-active compatibility record with explicit merged status?
