# Deferments Index #

**Status**: Active source-of-truth for unresolved deferments  
**Last Updated**: 2026-03-09

## Operational Guidelines ##

This is the mutable, canonical list of all unresolved deferments across implementation phases.

**Update Behavior**:

- When creating a new deferment during phase work, add a new entry with the next sequential DEF-NNN ID
- When resolving a deferment, move the full entry to `Deferments Archive.md` and remove it from this file
- When a scheduled target phase changes, update the Scheduled/Target column
- Use "Open" exactly when no scheduled phase is determined yet
- Keep Status as "Active" for all entries in this file
- Add clarifying context in Notes column when helpful for understanding scope/context

**Resolution Criteria**:

- A deferment is resolved when the deferred work is completed and verified (tests passing, artifacts created, contracts satisfied)
- Early resolution: If resolved before the scheduled target phase, archive immediately with actual resolution phase
- Late resolution: If unresolved when scheduled phase is reached, either reschedule to a later phase or leave as "Open"
- Open deferments: If a deferment remains unresolved and no clear path exists, document the blocker in Notes

**Column Definitions**:

- **Deferment ID**: Sequential DEF-NNN identifier
- **Deferment**: Brief description of deferred work
- **Deferred From**: Phase where deferment was created
- **Scheduled/Target**: Target phase for resolution, or "Open" if unscheduled
- **Status**: Always "Active" in this file
- **Notes**: Optional clarifying context

---

## Active Deferments ##

| Deferment ID | Deferment | Deferred From | Scheduled/Target | Status | Notes |
|--------------|-----------|---------------|------------------|--------|-------|
| DEF-001 | Localization/translation behavior changes | Phase 2 | Phase 3+ | Active | |
| DEF-002 | Manual task ID issuance/counter ownership | Phase 2 | Phase 3 | Active | |
| DEF-003 | Completion-marking runtime behavior | Phase 2 | Phase 3 | Active | Deferred until State Store command flow exists |
| DEF-004 | RuleId sequential-generation enforcement | Phase 2 | Open | Active | Deferred until RuleId generation exists |
| DEF-005 | Deterministic task-type ordering/comparer implementation | Phase 2 | Phase 5+ | Active | |
| DEF-006 | Deterministic task-type ordering/comparer tests | Phase 2 | Phase 5+ | Active | |
| DEF-007 | TaskSourceType/SourceIdentifier ambiguity resolution | Phase 3 | Phase 4 | Active | Open question on domain model clarity |
| DEF-008 | SnapshotChanged subscription | Phase 3 | Phase 4 | Active | |
| DEF-009 | INPC property updates from snapshots | Phase 3 | Phase 4 | Active | |
| DEF-010 | UI-local state (selection/filters/scroll) | Phase 3 | Phase 4 | Active | |
| DEF-011 | Task generation logic producing commands | Phase 3 | Phase 5 | Active | |
| DEF-012 | Built-in task generators | Phase 3 | Phase 5 | Active | |
| DEF-013 | Deadline field population | Phase 3 | Phase 5+ | Active | |
| DEF-014 | Task-type ordering/comparison | Phase 3 | Phase 5+ | Active | |
| DEF-015 | Task Builder rule evaluation | Phase 3 | Phase 6 | Active | |
| DEF-016 | Rule-driven command generation | Phase 3 | Phase 6 | Active | |
| DEF-017 | Save/load of State Store state | Phase 3 | Phase 7 | Active | |
| DEF-018 | Manual task counter persistence across sessions | Phase 3 | Phase 7 | Active | |
| DEF-019 | Version migration logic | Phase 3 | Phase 7 | Active | |
| DEF-020 | Baseline value storage | Phase 3 | Phase 7 | Active | |
| DEF-021 | UI interactions dispatching commands | Phase 3 | Phase 8/9 | Active | |
| DEF-022 | Visual feedback on state changes | Phase 3 | Phase 8/9 | Active | |
| DEF-023 | Batch command transactions | Phase 3 | Open | Active | |
| DEF-024 | Undo history | Phase 3 | Open | Active | |
| DEF-025 | Dismissed task tracking | Phase 3 | Open | Active | May be V2 scope |
| DEF-026 | Multiplayer synchronization | Phase 3 | Open | Active | May be V2 scope |

---

**Total Active Deferments**: 26
