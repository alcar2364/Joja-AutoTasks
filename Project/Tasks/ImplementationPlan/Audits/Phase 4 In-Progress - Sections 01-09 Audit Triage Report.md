# Phase 4 In-Progress - Sections 01-09 Audit Triage Report

| **Detail**            | **Description**                                                                                                           |
| --------------------- | ------------------------------------------------------------------------------------------------------------------------- |
| **Context:**          | User is actively implementing Phase 4.                                                                                    |
| **Purpose:**          | Capture Sections 01-09 audit findings that cut across later phases without widening current Phase 4 implementation scope. |
| **Artifact Status:**  | Living triage artifact for active Phase 4 work.                                                                           |
| **Review Mode:**      | post                                                                                                                      |
| **Review Status:**    | pending                                                                                                                   |
| **Review Rationale:** | Artifact retargeting should be checked after edit for scope and sequencing fit.                                           |

## Why This Report Exists

The Sections 01-09 audit surfaced contract and wording problems that span
multiple later phases. Because the user is still in Phase 4, those findings
need to be triaged now so current implementation work does not accidentally lock
in the wrong assumptions.

This report narrows Phase 4 guardrails, but it does not expand current Phase 4
implementation scope. The findings below should be held for later resolution
after current Phase 4 implementation unless explicitly pulled forward by the
user.

This is a living triage artifact. It may grow if more findings are discovered
before Phase 4 completion.

## Immediate Phase 4 Guardrails

Phase 4 should continue under the following temporary guardrails until the
cross-phase contracts are resolved:

- Keep `ManualTask_N` terminology, but do not settle persistence or reset
  semantics for manual identifiers yet.
- Avoid freezing any history storage assumption in view-model or snapshot
  infrastructure.
- Stay passive on day-start ownership; do not let Phase 4 code decide which
  layer owns day-start transitions.
- Preserve incoming snapshot order instead of inventing a new ordering policy.
- Keep baseline and history ownership assumptions out of view-model code.
- Keep completed-task handling minimal; do not over-model presentation or
  retention semantics in Phase 4.
- Keep command-only terminology; do not reintroduce action-oriented wording in
  new Phase 4 documentation.

## Triage Matrix

| **#** | **Finding**                                                                                                                   | **Severity** | **Owning phase / target**                     | **Existing issue coverage**               | **New issue record / GitHub issue later?** |
| ----- | ----------------------------------------------------------------------------------------------------------------------------- | ------------ | --------------------------------------------- | ----------------------------------------- | ------------------------------------------ |
| 1     | Manual task ID/counter contract split across docs, code, and tests (`Manual_` vs `ManualTask_`; reset/regress contradiction). | High         | Post-Phase 4 feeding Phase 7                  | 117 partial / 131 adjacent                | Yes                                        |
| 2     | V1 history persistence contradiction across Sections 01, 02, 09, and 11.                                                      | High         | Phase 7 before Phase 11                       | None                                      | Yes                                        |
| 3     | Day-start lifecycle ownership not normatively closed.                                                                         | Medium       | Post-Phase 4                                  | None                                      | Yes                                        |
| 4     | Ordering behavior underspecified and contradicted by implementation.                                                          | Medium       | Phase 5+                                      | 104 / 105 / 113 with later record updates | No                                         |
| 5     | RuleId serialization/value-object mismatch across Sections 03, 06, and 07.                                                    | High         | Phase 6 before Phase 10 serialization work    | None                                      | Yes                                        |
| 6     | Deadline data not anchored cleanly in the core model.                                                                         | Medium       | Phase 5+                                      | 112 with later record update              | No                                         |
| 7     | Daily baseline persistence ownership/scope contradiction.                                                                     | High         | Phase 7 before Phase 11                       | 119 with later record update              | No                                         |
| 8     | Rehydration orphan handling is undefined for CompletedTasks, PinnedTasks, and runtime cache when regenerated tasks differ.    | High         | Phase 7                                       | None                                      | Yes                                        |
| 9     | Completed-task presentation is under-modeled.                                                                                 | Medium       | Phase 9 with Phase 11 cross-check             | None                                      | Yes                                        |
| 10    | Built-in generator guaranteed V1 set is ambiguous.                                                                            | Medium       | Phase 5                                       | 111 with later record update              | No                                         |
| 11    | Section 07 wording drift: "commands or actions only" vs command-only contract.                                                | High         | Post-Phase 4 feeding Phase 6                  | 114 / 115 with later record updates       | No                                         |
| 12    | Section 07 DayKey/TaskID example mismatches canonical DayKey format.                                                          | Low          | Post-Phase 4 before Phase 6 use of Section 07 | None                                      | Yes                                        |

## Finding Notes

### Finding 1 - Manual task identifier contract

This is the sharpest Phase 4 guardrail because current terminology is already
visible in view-model and checklist work. Phase 4 should standardize on
`ManualTask_N` wording only and leave persistence, reset, and regression
semantics for the later contract-resolution pass.

### Findings 2, 7, and 8 - Persistence and rehydration contracts

These findings are related but should not be collapsed into one decision.
History persistence scope, baseline ownership, and orphan rehydration each need
their own explicit resolution before Phase 11 can safely consume them.

### Findings 3, 11, and 12 - Section 07 normalization work

Section 07 is already affecting downstream interpretation, so wording and
example drift should be resolved before Phase 6 leans on it more heavily. Phase
4 should treat Section 07 as advisory only where it conflicts with the existing
command/snapshot boundary contract.

### Findings 4, 6, and 10 - Phase 5-facing model clarifications

These findings narrow what Phase 5 may assume about ordering, deadline shape,
and built-in generator guarantees. They should be handled before Phase 5 tries
to make those behaviors normative in UI or generator implementation.

### Finding 5 - RuleId contract fit

The RuleId mismatch should be resolved before Phase 6 formalizes rule-driven
evaluation and before Phase 10 serializes rule-bound data. Leaving it open too
long increases the chance of parallel drift in both rule and persistence work.

### Finding 9 - Completed-task presentation semantics

Phase 4 should expose only the minimum bindable shape needed for current work.
Presentation, retention, and mode-specific completed-task behavior should be
resolved later with Phase 9 and then cross-checked against Phase 11 persistence
decisions.

## ImplementationIssues Synchronization Status

The ImplementationIssues tracker repair has been completed. This triage artifact
no longer needs a blocker carve-out for merge-conflicted record files, and
future synchronization should happen through the canonical
`ImplementationIssues` index and record system.

Use this report to guide later issue creation or record updates, but make those
changes in `ImplementationIssuesIndex.md` and the corresponding files under
`ImplementationIssues/Records/`. Preserve `legacy_id` values only where the
finding maps back to a migrated DEF item.

## Later Follow-Up Direction

1. Keep using this report as the current holding artifact for audit findings
   discovered during active Phase 4 work.
2. Add newly discovered findings here if they materially affect later-phase
   contracts or Phase 4 guardrails.
3. Revisit the queued findings after Phase 4 closes, or earlier only if the
   user explicitly pulls one forward.
4. Use this report to seed the eventual post-phase implementation review report
   and later ImplementationIssues updates once Phase 4 is complete and the
   relevant finding is ready for canonical tracking.

## Outcome

The audit findings should now be treated as in-progress Phase 4 triage items
held for later resolution after the current implementation pass unless the user
explicitly pulls them forward. They tighten current guardrails, but they do not
authorize Phase 4 to expand into persistence, ordering policy, rule
serialization, or presentation redesign.
