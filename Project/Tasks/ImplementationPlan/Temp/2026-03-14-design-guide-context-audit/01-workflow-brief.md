# Workflow Brief - Design Guide Context Audit (Docs-Only)

## Goal

Prepare deterministic, minimal documentation updates so implementation planning artifacts stay aligned with current design-guide sequencing and no-drop stage policy.

## Scope

- Analyze mapping from Design Guide Sections 01-20 into Implementation Plan phases (Section 21).
- Identify missing or weakly represented capabilities in planning artifacts.
- Propose update set for Section 21, ImplementationIssues index, Phase 4 checklist, and checklist-governing instruction/skill sources.
- Do not change runtime code, tests, or execution state.

## High-Value Findings

- Planning conflict: Section 21 retargets issue #100 to Phase 4, but ImplementationIssues still schedules #100 as Phase 3+.
- Planning conflict: Section 21 treats #86 as duplicate/merged into #159, but ImplementationIssues still keeps #86 as an active scheduled row.
- Capability coverage gap in explicit planning ownership: Section 20 defines V1 toast routing and first-run onboarding, but Section 21 phase text does not explicitly assign these capabilities.
- Cross-cutting sections (12, 15, 16) need explicit phase ownership matrix entries to reduce checklist drift.

## Sources Used

- Design guide root and Section 21 implementation plan.
- Section-specific capability docs for sections 10A, 11, 12, 15, 16, 20.
- ImplementationIssues index and selected issue records (#100, #159, #86, #122, #123, #124).
- Workflow docs under Project/Planning/Workflows.
- Checklist creation governance files (instruction + skill).

## Scope Decision

No new phase is required. Missing capabilities can be placed into existing phases via explicit ownership mapping and staged notes.
