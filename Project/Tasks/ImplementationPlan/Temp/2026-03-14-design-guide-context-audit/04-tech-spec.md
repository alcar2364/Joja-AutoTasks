# Tech Spec - Section Coverage Matrix and Placement Proposals

## Objective

Map design-guide features/sections to implementation phases and identify missing explicit coverage in planning artifacts.

## Section-to-Phase Coverage (Proposed Canonical Matrix)

- Section 01 Product Definition -> Cross-cutting acceptance scope; enforced by Phase 21.17 release criteria and all phases.
- Section 02 System Architecture -> Cross-cutting boundary contract; operationally anchored in Phase 1 lifecycle wiring plus all stage gates.
- Section 03 Deterministic Identifier Model -> Primary Phase 2; extended by Phase 5 (generator IDs), Phase 6 (rule IDs), Phase 7 (persistence continuity).
- Section 04 Core Data Model -> Phase 2 model types; Phase 3 state store model ownership.
- Section 05 Task Generation and Evaluation Engine -> Phase 5 generators + Phase 6 rule evaluation runtime.
- Section 06 Task Builder Rule Serialization -> Phase 10 rule-definition UX + Phase 7 persistence serialization.
- Section 07 Rule Evaluation Model -> Phase 6 primary; Phase 12 diagnostics support.
- Section 08 State Store Command Model -> Phase 3 primary.
- Section 09 Persistence Model -> Phase 7 primary; supports Phase 11 history reads.
- Section 10 UI Data Binding Model -> Phase 4 binding foundation; Phase 8/9 surface application; Phase 11 history navigation binding.
- Section 10A View Model Architecture -> Phase 4 infrastructure; feature-specific VMs staged into Phase 8/9/10/11/12.
- Section 11 Daily Snapshot Ledger -> Phase 7 persistence write/read model; Phase 11 browsing consumers.
- Section 12 Engine Update Cycle -> Split ownership: Phase 1 lifecycle hooks, Phase 5 trigger sources, Phase 6 queue/evaluation pass, Phase 7 save/day persistence interactions, Phase 12 manual trigger diagnostics.
- Section 13 Built-in Task Generators -> Phase 5 primary.
- Section 14 Task Builder Wizard UX -> Phase 10 primary.
- Section 15 Configuration System -> Phase 1 load/bootstrap, Phase 4 config view-model boundary hardening, Phase 8 full configuration menu ownership, Phase 12 debug tuning controls.
- Section 16 Error Handling and Rule Validation -> Split ownership: Phase 3 command safety/invariants, Phase 5 generator fault isolation, Phase 6 rule evaluation failure handling, Phase 7 persistence recovery, Phase 12 diagnostics/logging ergonomics.
- Section 17 Debug and Development Tools -> Phase 12 primary.
- Section 18 Versioning and Migration Strategy -> Phase 7 primary.
- Section 19 Performance Guardrails -> Cross-cutting stage-gate checks (G5) and per-phase verification criteria.
- Section 20 UI System Design -> Phase 8 menu architecture, Phase 9 HUD architecture, Phase 10 wizard flow ownership, Phase 11 history UI depth, Phase 12 debug UX.

## Missing/Weakly Represented Features and Best Placement

1. Section 20.8 Notification and Toast System (V1 native HUD message routing)

- Best placement: Phase 9 (owner) with dependency on Phase 3/8 event routing boundaries.
- Reason: user-visible HUD feedback; explicit ownership boundary says view-model layer should not handle toast event channel.

1. Section 20.10 First-run onboarding

- Best placement: Phase 8 (menu ownership) with persistence dependency in Phase 7 (StoreUserState.OnboardingAcknowledged).
- Reason: onboarding entry points and sample-rule links are menu-surface concerns.

1. Section 20.9 Gamepad/controller support verification

- Best placement: Phase 8 and Phase 9 verification substeps (no heavy implementation expected).
- Reason: design states StardewUI gives built-in support; planning should still include explicit validation criteria.

1. Section 12 engine queue/trigger lifecycle specifics

- Best placement: explicit sub-mapping in Section 21 under Phases 1, 5, 6, 7, 12.
- Reason: prevents future checklists from overloading one phase with full engine-cycle ownership.

1. Section 16 failure-domain ownership

- Best placement: explicit sub-mapping table in Section 21 to phases 3, 5, 6, 7, 12.
- Reason: current phase summaries do not clearly claim all failure-domain responsibilities.

No new phase required.
