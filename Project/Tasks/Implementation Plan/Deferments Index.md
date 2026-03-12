# Deferments Index

**Status**: Active source-of-truth for unresolved deferments  
**Last Updated**: 2026-03-11

## Operational Guidelines

This is the mutable, canonical list of all unresolved deferments across implementation phases.

**Update Behavior**:

- When creating a new deferment during phase work, add a new entry with the next sequential DEF-NNN ID
- When resolving a deferment, move the full entry to `Deferments Archive.md` and remove it from this file
- When a scheduled target phase changes, update the Scheduled/Target column
- Use "Open" exactly when no scheduled phase is determined yet
- Keep Status as "Active" for all entries in this file
- Add a clear implementation-purpose statement in Deferment Description (what work is deferred and why)
- Record origin checklist evidence in Source Checklist (phase + step/section)
- Add clarifying context in Notes column when helpful for understanding scope/context
- Link to GitHub issue number in Issue column once issue is created

**Resolution Criteria**:

- A deferment is resolved when the deferred work is completed and verified (tests passing, artifacts created, contracts satisfied)
- Early resolution: If resolved before the scheduled target phase, archive immediately with actual resolution phase
- Late resolution: If unresolved when scheduled phase is reached, either reschedule to a later phase or leave as "Open"
- Open deferments: If a deferment remains unresolved and no clear path exists, document the blocker in Notes

**Column Definitions**:

- **Deferment ID**: Sequential DEF-NNN identifier
- **Deferment Description**: Brief but specific statement of deferred work and intended implementation purpose
- **Deferred From**: Phase where deferment was created
- **Source Checklist**: Checklist section/step where deferment was declared
- **Scheduled/Target**: Target phase for resolution, or "Open" if unscheduled
- **Status**: Always "Active" in this file
- **Issue**: GitHub issue number once created (use `TBD` until created)
- **Notes**: Optional clarifying context

---

## Active Deferments

| Deferment ID | Deferment Description                                                                                                                                  | Deferred From | Source Checklist                                                          | Scheduled/Target | Status | Issue | Notes                                                      |
| ------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------ | ------------- | ------------------------------------------------------------------------- | ---------------- | ------ | ----- | ---------------------------------------------------------- |
| DEF-001      | Implement localization/translation behavior that changes runtime task content and flow, not just documentation.                                        | Phase 2       | Phase 2 checklist, Step 4D (translation-impacting implementation defer)   | Phase 3+         | Active | #100  |                                                            |
| DEF-004      | Enforce sequential RuleId generation once actual RuleId generation flow exists.                                                                        | Phase 2       | Phase 2 checklist guardrails/final gate (RuleId generation defer)         | Open             | Active | #103  | Deferred until RuleId generation exists                    |
| DEF-005      | Add deterministic task-type ordering/comparer implementation after generator/task-type coverage stabilizes.                                            | Phase 2       | Phase 2 checklist, Step 6A (task-type comparer defer)                     | Phase 5+         | Active | #104  |                                                            |
| DEF-006      | Add deterministic task-type ordering/comparer test coverage with comparer implementation.                                                              | Phase 2       | Phase 2 checklist, Step 7F (comparer tests defer)                         | Phase 5+         | Active | #105  |                                                            |
| DEF-007      | Resolve TaskSourceType versus SourceIdentifier ambiguity and clarify/refactor domain terminology if needed.                                            | Phase 3       | Phase 3 checklist, Deferred Items (Phase 4 ambiguity note)                | Phase 4          | Active | #106  | Open question on domain model clarity                      |
| DEF-008      | Wire ViewModels to subscribe to SnapshotChanged so snapshot updates propagate into UI state.                                                           | Phase 3       | Phase 3 checklist, Deferred Items (Phase 4 ViewModels)                    | Phase 4          | Active | #107  |                                                            |
| DEF-009      | Implement INPC property updates driven from snapshots for deterministic UI refresh behavior.                                                           | Phase 3       | Phase 3 checklist, Deferred Items (Phase 4 ViewModels)                    | Phase 4          | Active | #108  |                                                            |
| DEF-010      | Add UI-local state ownership for selection, filters, and scroll while keeping snapshot state canonical.                                                | Phase 3       | Phase 3 checklist, Deferred Items (Phase 4 ViewModels)                    | Phase 4          | Active | #109  |                                                            |
| DEF-011      | Implement task generation logic that emits State Store commands instead of direct mutations.                                                           | Phase 3       | Phase 3 checklist, Deferred Items (Phase 5+ Generators/Engine)            | Phase 5          | Active | #110  |                                                            |
| DEF-012      | Implement built-in task generators with deterministic output and command emission.                                                                     | Phase 3       | Phase 3 checklist, Deferred Items (Phase 5+ Generators/Engine)            | Phase 5          | Active | #111  |                                                            |
| DEF-013      | Populate deadline field consistently from generator/rule inputs.                                                                                       | Phase 3       | Phase 3 checklist, Deferred Items (Phase 5+ Generators/Engine)            | Phase 5+         | Active | #112  |                                                            |
| DEF-014      | Implement task-type ordering/comparison behavior in runtime usage paths.                                                                               | Phase 3       | Phase 3 checklist, Deferred Items (Phase 5+ Generators/Engine)            | Phase 5+         | Active | #113  |                                                            |
| DEF-015      | Implement Task Builder rule evaluation pipeline.                                                                                                       | Phase 3       | Phase 3 checklist, Deferred Items (Phase 6 Rule Engine)                   | Phase 6          | Active | #114  |                                                            |
| DEF-016      | Implement rule-driven command generation from evaluated Task Builder rules.                                                                            | Phase 3       | Phase 3 checklist, Deferred Items (Phase 6 Rule Engine)                   | Phase 6          | Active | #115  |                                                            |
| DEF-017      | Implement persistence save/load for full State Store state across sessions.                                                                            | Phase 3       | Phase 3 checklist, Deferred Items (Phase 7 Persistence)                   | Phase 7          | Active | #116  |                                                            |
| DEF-018      | Persist manual task counter across sessions to preserve deterministic manual TaskId continuity.                                                        | Phase 3       | Phase 3 checklist, Deferred Items (Phase 7 Persistence)                   | Phase 7          | Active | #117  |                                                            |
| DEF-019      | Implement version migration logic for persisted schema evolution safety.                                                                               | Phase 3       | Phase 3 checklist, Deferred Items (Phase 7 Persistence)                   | Phase 7          | Active | #118  |                                                            |
| DEF-020      | Persist baseline values needed for deterministic comparisons across loads.                                                                             | Phase 3       | Phase 3 checklist, Deferred Items (Phase 7 Persistence)                   | Phase 7          | Active | #119  |                                                            |
| DEF-021      | Add UI interactions that dispatch commands through canonical state boundaries.                                                                         | Phase 3       | Phase 3 checklist, Deferred Items (Phase 8+ Menu/HUD)                     | Phase 8/9        | Active | #120  |                                                            |
| DEF-022      | Implement visual feedback tied to state changes in UI surfaces.                                                                                        | Phase 3       | Phase 3 checklist, Deferred Items (Phase 8+ Menu/HUD)                     | Phase 8/9        | Active | #121  |                                                            |
| DEF-023      | Add batch command transaction support for grouped atomic state updates.                                                                                | Phase 3       | Phase 3 checklist, Deferred Items (V2)                                    | Open             | Active | #122  |                                                            |
| DEF-024      | Add undo history mechanics for command/state reversibility.                                                                                            | Phase 3       | Phase 3 checklist, Deferred Items (V2)                                    | Open             | Active | #123  |                                                            |
| DEF-025      | Add dismissed task tracking lifecycle and storage behavior.                                                                                            | Phase 3       | Phase 3 checklist, Deferred Items (V2)                                    | Open             | Active | #124  | May be V2 scope                                            |
| DEF-026      | Add multiplayer synchronization for shared task state consistency.                                                                                     | Phase 3       | Phase 3 checklist, Deferred Items (V2)                                    | Open             | Active | #125  | May be V2 scope                                            |
| DEF-027      | Evaluate whether TaskRecord should remain a value type or become a reference type based on mutation patterns and state dictionary semantics.           | Phase 3       | Phase 3 checklist, Deferred Items (open issue: TaskRecord type decision)  | Open             | Active | #126  | Decision required before deeper optimization/refactor work |
| DEF-028      | Evaluate DayKey internal representation by storing year/season/day components alongside canonical string for comparison and calculation ergonomics.    | Phase 3       | Phase 3 checklist, Deferred Items (open issue: DayKey representation)     | Open             | Active | #127  | Requires design decision on storage-versus-parse tradeoff  |
| DEF-029      | Decide the permanent home for `ManualTaskCounter` after generator/UI service boundaries are established.                                               | Phase 3       | Phase 3 checklist, Deferred Items (`ManualTaskCounter` location decision) | Phase 5+         | Active | TBD   | Keep `State/Models/` as working location until decision    |
| DEF-031      | Evaluate renaming `DayKey` to `DateKey` while preserving canonical `Year{N}-{Season}{D}` key format and contracts.                                     | Phase 3       | Phase 3 checklist, Deferred Items (`DayKey` to `DateKey` naming request)  | Open             | Active | TBD   | Naming-only clarity proposal; no format change requested   |
| DEF-032      | Address security issue #86 by replacing broad exception swallowing in `ConfigLoader` with deterministic, explicit error handling and logging behavior. | Phase 3       | GitHub issue #86 backlog deferment reconciliation                         | Phase 4          | Active | #86   | Tracks `[security]` config loader catch-block hardening    |

---

**Total Active Deferments**: 29
