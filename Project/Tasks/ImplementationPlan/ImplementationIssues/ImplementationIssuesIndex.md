# Implementation Issues Index

**Status**: Active source-of-truth for unresolved implementation issues  
**Last Updated**: 2026-03-15

## Operational Guidelines

This file is the active summary view of all unresolved implementation issues tracked by the project.

- Add and edit issue details in the individual issue records under `ImplementationIssues/Records/`.
- GitHub issue number is canonical once assigned.
- `Critical` and `High` items must keep a scheduled target.
- Closed items are archived and removed from this active index.

---

## Active Implementation Issues

| Issue | Type | Summary | Created Phase | Source | Scheduled Target | Status | Priority | Resolution PR | Notes |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 159 | Review follow-up | A bare catch block in Configuration/ConfigLoader.cs (line 26) suppresses all exceptions without discrimination during config deserialization. This means fatal CLR exceptions (StackOverflowException, OutOfMemoryException, ThreadAbortException) and genuine deserialization bugs are silently swallowed, falling back to null as if the config file simply didn't exist. | Phase 3 | #86 (merged historical source reference) | Phase 4 | Open | High |  | Canonical active tracker for merged #86 scope; #86 retained as historical merged-reference only. |
| 132 | Deferment | Evaluate renaming `DayKey` to `DateKey` while preserving canonical `Year{N}-{Season}{D}` key format and contracts. | Phase 3 | Phase 3 checklist, Deferred Items (`DayKey` to `DateKey` naming request) | Phase 6 | Open | Low |  | Naming-only clarity proposal; no format change requested |
| 131 | Deferment | Decide the permanent home for `ManualTaskCounter` after generator/UI service boundaries are established. | Phase 3 | Phase 3 checklist, Deferred Items (`ManualTaskCounter` location decision) | Phase 5+ | Open | Low |  | Keep `State/Models/` as working location until decision |
| 127 | Deferment | Evaluate DayKey internal representation by storing year/season/day components alongside canonical string for comparison and calculation ergonomics. | Phase 3 | Phase 3 checklist, Deferred Items (open issue: DayKey representation) | Phase 6 | Open | Low |  | Requires design decision on storage-versus-parse tradeoff |
| 126 | Deferment | Evaluate whether TaskRecord should remain a value type or become a reference type based on mutation patterns and state dictionary semantics. | Phase 3 | Phase 3 checklist, Deferred Items (open issue: TaskRecord type decision) | Phase 5 | Open | Medium |  | Decision required before deeper optimization/refactor work |
| 125 | Deferment | Add multiplayer synchronization for shared task state consistency. | Phase 3 | Phase 3 checklist, Deferred Items (V2) | V2+ | Open | Low |  | May be V2 scope |
| 124 | Deferment | Add dismissed task tracking lifecycle and storage behavior. | Phase 3 | Phase 3 checklist, Deferred Items (V2) | Phase 8 | Open | Low |  | May be V2 scope |
| 123 | Deferment | Add undo history mechanics for command/state reversibility. | Phase 3 | Phase 3 checklist, Deferred Items (V2) | Phase 9 | Open | Low |  |  |
| 122 | Deferment | Add batch command transaction support for grouped atomic state updates. | Phase 3 | Phase 3 checklist, Deferred Items (V2) | Phase 9 | Open | Low |  |  |
| 121 | Deferment | Implement visual feedback tied to state changes in UI surfaces. | Phase 3 | Phase 3 checklist, Deferred Items (Phase 8+ Menu/HUD) | Phase 8/9 | Open | Low |  |  |
| 120 | Deferment | Add UI interactions that dispatch commands through canonical state boundaries. | Phase 3 | Phase 3 checklist, Deferred Items (Phase 8+ Menu/HUD) | Phase 8/9 | Open | Low |  |  |
| 119 | Deferment | Persist baseline values needed for deterministic comparisons across loads. | Phase 3 | Phase 3 checklist, Deferred Items (Phase 7 Persistence) | Phase 7 | Open | Low |  |  |
| 118 | Deferment | Implement version migration logic for persisted schema evolution safety. | Phase 3 | Phase 3 checklist, Deferred Items (Phase 7 Persistence) | Phase 7 | Open | Low |  |  |
| 117 | Deferment | Persist manual task counter across sessions to preserve deterministic manual TaskId continuity. | Phase 3 | Phase 3 checklist, Deferred Items (Phase 7 Persistence) | Phase 7 | Open | Low |  |  |
| 116 | Deferment | Implement persistence save/load for full State Store state across sessions. | Phase 3 | Phase 3 checklist, Deferred Items (Phase 7 Persistence) | Phase 7 | Open | Low |  |  |
| 115 | Deferment | Implement rule-driven command generation from evaluated Task Builder rules. | Phase 3 | Phase 3 checklist, Deferred Items (Phase 6 Rule Engine) | Phase 6 | Open | Low |  |  |
| 114 | Deferment | Implement Task Builder rule evaluation pipeline. | Phase 3 | Phase 3 checklist, Deferred Items (Phase 6 Rule Engine) | Phase 6 | Open | Low |  |  |
| 113 | Deferment | Implement task-type ordering/comparison behavior in runtime usage paths. | Phase 3 | Phase 3 checklist, Deferred Items (Phase 5+ Generators/Engine) | Phase 5+ | Open | Low |  |  |
| 112 | Deferment | Populate deadline field consistently from generator/rule inputs. | Phase 3 | Phase 3 checklist, Deferred Items (Phase 5+ Generators/Engine) | Phase 5+ | Open | Low |  |  |
| 111 | Deferment | Implement built-in task generators with deterministic output and command emission. | Phase 3 | Phase 3 checklist, Deferred Items (Phase 5+ Generators/Engine) | Phase 5 | Open | Low |  |  |
| 110 | Deferment | Implement task generation logic that emits State Store commands instead of direct mutations. | Phase 3 | Phase 3 checklist, Deferred Items (Phase 5+ Generators/Engine) | Phase 5 | Open | Low |  |  |
| 109 | Deferment | Add UI-local state ownership for selection, filters, and scroll while keeping snapshot state canonical. | Phase 3 | Phase 3 checklist, Deferred Items (Phase 4 ViewModels) | Phase 4 | Open | Low |  |  |
| 108 | Deferment | Implement INPC property updates driven from snapshots for deterministic UI refresh behavior. | Phase 3 | Phase 3 checklist, Deferred Items (Phase 4 ViewModels) | Phase 4 | Open | Low |  |  |
| 106 | Deferment | Resolve TaskSourceType versus SourceIdentifier ambiguity and clarify/refactor domain terminology if needed. | Phase 3 | Phase 3 checklist, Deferred Items (Phase 4 ambiguity note) | Phase 4 | Open | Low |  | Open question on domain model clarity |
| 105 | Deferment | Add deterministic task-type ordering/comparer test coverage with comparer implementation. | Phase 2 | Phase 2 checklist, Step 7F (comparer tests defer) | Phase 5+ | Open | Low |  |  |
| 104 | Deferment | Add deterministic task-type ordering/comparer implementation after generator/task-type coverage stabilizes. | Phase 2 | Phase 2 checklist, Step 6A (task-type comparer defer) | Phase 5+ | Open | Low |  |  |
| 103 | Deferment | Enforce sequential RuleId generation once actual RuleId generation flow exists. | Phase 2 | Phase 2 checklist guardrails/final gate (RuleId generation defer) | Phase 6 | Open | Low |  | Deferred until RuleId generation exists |
| 100 | Deferment | Implement localization/translation behavior that changes runtime task content and flow, not just documentation. | Phase 2 | Phase 2 checklist, Step 4D (translation-impacting implementation defer) | Phase 4 | Open | Low |  | Aligned to Phase 4 per Section 21 canonical scheduling reconciliation. |

---

**Total Active Implementation Issues**: 28
