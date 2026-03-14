
# Section 21 — Implementation Plan #

## 21.1 Purpose ##

This section defines the implementation plan for building the Automatic Task
Manager mod. The goal is to translate the architecture and design decisions
defined throughout this document into a structured development roadmap.

The implementation plan prioritizes:

    - correctness and deterministic behavior
    - architectural stability
    - incremental feature delivery
    - testability and debugging visibility

Implementation must follow the architectural boundaries defined in
Section 2 and the domain models defined in Sections 3 and 4.

The plan is organized into implementation phases. Each phase introduces
new subsystems while maintaining a working mod state.

## 21.2 Implementation Strategy ##

The mod uses a no-drop, architecture-first delivery strategy with right-sized
staging. Core feature intent remains unchanged; sequencing controls complexity.

Delivery stages:

    - Now: build Phase 1 through Phase 10 plus the minimum Phase 11 and
    Phase 12 slices required for Version 1 viability.
    - Next: complete the remaining Phase 11 and Phase 12 breadth that is
    not required to ship Version 1 safely.
    - Later: deliver post-Version-1 expansion work (for example,
    statistics dashboards) without changing Version 1 scope commitments.

Non-negotiable constraints:

    - Task Builder is a core differentiator and must remain on the critical
    path.
    - Both UI surfaces (HUD and full menu) are required for Version 1.
    - No capability in Phase 8 through Phase 12 is dropped; deferral is
    sequencing only.
    - Code and documentation must be updated together, with variances
    recorded and justified.

Each phase must end in a runnable checkpoint and pass the stage gates in
Section 21.3.2 before promotion.

## 21.3 Phase Overview ##

The implementation is divided into the following phases.

| Phase | Focus | Delivery stage |
| ---- | ---- | ---- |
| Phase 1 | Project skeleton and lifecycle | Now |
| Phase 2 | Core domain model | Now |
| Phase 3 | State store | Now |
| Phase 4 | View model infrastructure | Now |
| Phase 5 | Built-in task generators | Now |
| Phase 6 | Rule evaluation engine | Now |
| Phase 7 | Persistence system | Now |
| Phase 8 | Menu dashboard | Now |
| Phase 9 | HUD interface | Now |
| Phase 10 | Task Builder wizard | Now |
| Phase 11 | History browsing UI | Now + Next |
| Phase 12 | Debug and developer tooling | Now + Next |

Later phases depend on stable completion of earlier phases.

### 21.3.1 No-drop capability mapping (Version 1 + Phase 8-12) ###

| Capability | Phase owner | Version 1 requirement | Stage | Notes |
| ---- | ---- | ---- | ---- | ---- |
| Menu task list + details split view | Phase 8 | Required | Now | Must read the same snapshot model as HUD. |
| Manual task creation and editing from menu | Phase 8 | Required | Now | Command-dispatched only, no direct state mutation. |
| Configuration controls in full menu | Phase 8 | Required | Now | GMCM remains entry surface only. |
| HUD active-task visibility and scrolling | Phase 9 | Required | Now | Always-available surface with bounded interactions. |
| HUD completed-task visibility toggle | Phase 9 | Required | Now | Treated as UI-local state, not canonical task state. |
| HUD to menu launch path | Phase 9 | Required | Now | Required for dual-surface workflow continuity. |
| Task Builder wizard rule-definition flow | Phase 10 | Required | Now | Core differentiator; rule authoring remains menu-owned. |
| Task Builder step validation and serialization | Phase 10 | Required | Now | Produces `RuleDefinition` artifacts only. |
| History day browsing and day navigation | Phase 11 | Required | Now | Baseline browse and day switch must ship in Version 1. |
| History filtering and quick-jump depth | Phase 11 | Required | Next | Deferred for hardening only, not dropped. |
| Rule evaluation diagnostics and trigger tooling | Phase 12 | Required | Now | Needed for deterministic validation and supportability. |
| Debug UX depth and tuning ergonomics | Phase 12 | Required | Next | Deferred polish with no capability removal. |

### 21.3.2 Stage gates ###

All gates are mandatory at stage boundaries.

| Gate | Objective | Minimum pass criteria |
| ---- | ---- | ---- |
| G1 Boundary integrity | Preserve command/snapshot architecture | UI and Task Builder paths do not mutate canonical task state directly. |
| G2 Differentiator viability | Keep Task Builder shippable | Wizard can define, validate, serialize, and persist rules end-to-end. |
| G3 Dual-surface parity | Keep HUD and menu behavior aligned | HUD and menu consume the same snapshot and expose coherent task data. |
| G4 Persistence safety | Protect save stability | Versioning, migrations, and reconstruction behavior are validated. |
| G5 Performance envelope | Keep runtime cost bounded | Evaluation remains event-driven and HUD/menu rebuilds are change-driven. |
| G6 Scope budget | Control complexity without feature drops | Deferrals are stage-only and all deferred capabilities remain mapped. |
| G7 Doc-sync closure | Keep docs and code aligned | Design sections are updated first, Architecture Map reconciled second, and variances logged. |

### 21.3.3 Code-doc synchronization and variance policy ###

Synchronization order:

1. Update canonical design guide sections first.
2. Reconcile Architecture Map wording and references against the updated
canonical sections.
3. Record any accepted divergence in the variance register with explicit
justification and closure target.

Variance register template:

| Variance ID | Canonical source | Variant artifact | Justification | Decision | Closure target | Status |
| ---- | ---- | ---- | ---- | ---- | ---- | ---- |
| VAR-XXX | Design guide section | Code or secondary doc reference | Why divergence exists | Keep/update/deprecate | Planned closure milestone | Open/closed |

Seeded baseline variance entry:

| Variance ID | Canonical source | Variant artifact | Justification | Decision | Closure target | Status |
| ---- | ---- | ---- | ---- | ---- | ---- | ---- |
| VAR-001 | Identifier naming in design docs (`RuleID`, `SubjectID`) | Current code symbols and file paths (`RuleId`, `SubjectId`; `Domain/Identifiers/RuleId.cs`, `Domain/Identifiers/SubjectId.cs`) | Legacy docs and checklist wording still use older acronym style | Keep code canonical and update legacy docs incrementally | Doc-sync closure before Version 1 RC | Open |
| VAR-002 | Section 03 / Section 21 identifier naming (`Manual_N`) | Test files using `ManualTask_N` | Legacy naming split; `Manual_N` is canonical; `ManualTask_N` appears in some test fixtures | Keep `Manual_N` canonical; update test fixtures incrementally | Doc-sync closure before Version 1 RC | Open |
| VAR-003 | Section 07 rule evaluation model ("commands only" wording) | Pre-fix wording in Section 07 that said "commands or actions" | Pre-fix divergence; resolved by T1 doc fix in this phase | Resolved by T1 doc fix | Closed by T1 in this phase | Closed |
| VAR-004 | Section 09 persistence model (history exclusion in §9.12) | Pre-fix wording excluded historical task ledger from V1 | Pre-fix divergence; resolved by T1/T5 doc fixes adding ledger to `SaveData` and removing the stale §9.12 exclusion | Resolved by T1/T5 Section 09 doc fixes | Closed by T1/T5 in this phase | Closed |

**Note:** Findings that represent bugs to fix (rather than accepted
divergences) are tracked in the `ImplementationIssues` folder
(`Project/Tasks/ImplementationPlan/ImplementationIssues/`), not in this
variance register.

## 21.4 Phase 1 — Project Skeleton and Lifecycle ##

Phase 1 establishes the basic mod structure and lifecycle integration.

Responsibilities:

    - Create SMAPI entry point
    - Initialize logging and configuration
    - Establish subsystem initialization order
    - Hook required game events

Key systems initialized in this phase:

    - lifecycle coordinator
    - event dispatcher
    - configuration loader

Example lifecycle hooks:

    - `OnSaveLoaded`
    - `OnDayStarted`
    - `OnReturnedToTitle`
    - `OnSaving`
    - `OnUpdateTicked`

This phase must ensure the mod loads safely without any task logic yet.

## 21.5 Phase 2 — Core Domain Model ##

Phase 2 implements the domain objects defined in the core data model.

Reference: Section 4.

Key types include:

    - `TaskId`
    - `TaskObject`
    - `TaskCategory`
    - `TaskStatus`
    - `DayKey`

Deterministic identifier rules defined in Section 3 must be implemented at this
stage. fileciteturn1file19

Phase 2 does not introduce a stored per-task priority field in `TaskObject` or
daily snapshots.

This phase should include helper utilities for:

    - deterministic TaskId generation
    - day key construction

Deterministic task-type sort mechanism implementation (derived ordering map,
fallback chain, comparer, and comparer tests) is deferred to Phase 5+ when
task generator/task-type coverage is stable.

Future customization of ordering should use user-configurable ordering/sort
profiles rather than stored per-task priorities.

Phase 2 localization scope constraint:

    - Phase 2 may define documentation and architecture guardrails for
    localization boundaries only.
    - Any translation change that impacts existing Phase 1 or Phase 2
    implementation behavior is explicitly deferred to Phase 3+.
    - No additional implementation checklist expansion is introduced in this
    section for post-Phase 2 localization work.

No UI or engine logic should exist yet.

## 21.6 Phase 3 — State Store ##

Phase 3 introduces the centralized State Store defined in Section 4.9.

Responsibilities:

    - maintain active tasks
    - store progress and completion states
    - manage pinned tasks
    - publish immutable snapshots for UI

The store must enforce command-based mutation.

Example command types:

    - `AddTask`
    - `UpdateTaskProgress`
    - `CompleteTask`
    - `PinTask`
    - `UnpinTask`

The State Store becomes the authoritative runtime representation of tasks.

## 21.7 Phase 4 — View Model Infrastructure ##

Phase 4 introduces the View Model layer defined in Section 10A.

Responsibilities:

    - Implement base view model with INPC via PropertyChanged.SourceGenerator
    - Implement `HudViewModel` and `TaskListViewModel` as initial consumers
    - Subscribe to `SnapshotChanged` (Section 8.12) and project snapshot
    data into bindable properties
    - Establish the command dispatch pattern from view models back to the
    State Store

This phase validates the State Store → ViewModel → UI binding pipeline
before any StarML layout work begins. View models should be testable
without a running game.

## 21.8 Phase 5 — Built-in Task Generators ##

Phase 5 introduces built-in automatic task generators.

Reference: Section 5. fileciteturn1file17

Initial generators should be implemented for the most stable gameplay systems:

    - crop care generator
    - animal care generator
    - machine output generator
    - calendar reminders
    - quest progress

Generators must operate using the Generation/Evaluation Context defined in
Section 5.3 rather than querying the game directly. fileciteturn1file17

Outputs from generators must pass through the shared task pipeline.

## 21.9 Phase 6 — Rule Evaluation Engine ##

Phase 6 implements the rule engine that evaluates Task Builder rules.

Reference: Section 7. fileciteturn1file16

Key components:

    - rule runtime model
    - rule trigger mapping
    - condition tree evaluation
    - progress model computation
    - task normalization pipeline

This phase must implement:

    - two-phase rule evaluation
    - baseline capture semantics
    - deterministic task identity generation

Rules should be testable independently of the UI.

## 21.10 Phase 7 — Persistence System ##

Phase 7 implements the persistence model.

Reference: Section 9. fileciteturn1file13

Responsibilities:

    - save rule definitions
    - save manual tasks
    - save runtime baseline values
    - save store-level user state
    - migrate save data between schema versions

Persistence must follow the minimal storage principle:

only essential state is stored; derived task state is reconstructed by the
engine.

Save/load lifecycle must align with the mod lifecycle defined in Section 2.5.

## 21.11 Phase 8 — Menu Dashboard ##

Phase 8 implements the main mod menu.

Stage intent:

    - Now: ship the required Version 1 management surface.
    - Next: deepen non-blocking workflow polish without dropping capability.

The menu is implemented before the HUD because it exercises the full
StardewUI menu pipeline (layout, interaction, tabs, navigation) and
validates the ViewModel binding layer on a simpler interaction surface
than the always-on HUD drawable.

Menu capabilities include:

    - full task list browsing
    - history entry points and baseline integration wiring
    - manual task creation
    - task editing
    - configuration controls
    - statistics access (V2)

Detailed history browsing interactions are delivered in Phase 11
(Now baseline, Next depth).

The menu is also responsible for launching the Task Builder wizard.

The menu uses the same store snapshot model as the HUD.

## 21.12 Phase 9 — HUD Interface ##

Phase 9 introduces the in-game HUD interface.

Stage intent:

    - Now: ship the required Version 1 in-game surface.
    - Next: tune interaction ergonomics while preserving parity.

The HUD uses StardewUI's `IViewDrawable` API
(`viewEngine.CreateDrawableFromAsset()`) as described in Section
20.6.1.

HUD responsibilities:

    - display active tasks
    - allow task selection
    - show task details
    - support scrolling
    - optionally show completed tasks

The HUD must consume immutable snapshots from the State Store.

It must never mutate task objects directly.

## 21.13 Phase 10 — Task Builder Wizard ##

Phase 10 implements the Task Builder UI system.

Stage intent:

    - Now: ship the core differentiator with full rule-definition flow.
    - Next: add polish and template-depth improvements without changing
    command/snapshot boundaries.

Reference: Section 6. fileciteturn1file15

The wizard must guide users through:

1. selecting triggers
2. defining conditions
3. configuring progress tracking
4. selecting task presentation settings
5. choosing persistence behavior

Each step must validate rule inputs before allowing progression.

The final rule must serialize into the `RuleDefinition` structure.

Boundary constraint:

    - Task Builder in Now is limited to rule-definition authoring,
    validation, and persistence.
    - Task Builder must not directly create, complete, or mutate runtime
    task entities.
    - Runtime task changes occur only after rule evaluation through
    command/state-store boundaries.

## 21.14 Phase 11 — History Browsing UI ##

Phase 11 introduces the history browsing UI.

Stage intent:

    - Now: baseline day browsing and day navigation required for Version 1.
    - Next: filtering and quick-jump depth hardening.

The system reads data from the Daily Snapshot Ledger defined in Section 11.

Capabilities include:

    - viewing previous days' task lists
    - browsing completed and incomplete tasks for a selected day
    - date navigation and quick jump to a specific day
    - filtering by category or task type

Statistics and analytics (V2): Completion statistics, productivity
trends, and aggregate dashboards are deferred to Version 2. The Daily
Snapshot Ledger provides the data foundation. Statistics must always
derive from historical snapshots rather than duplicating stored data.

## 21.15 Phase 12 — Debug and Development Tools ##

Phase 12 implements debugging and developer tooling.

Stage intent:

    - Now: minimum deterministic diagnostics and manual trigger tooling.
    - Next: developer-experience and debug UI depth hardening.

Reference: Section 17.

Capabilities may include:

    - rule evaluation logging
    - debug overlay for task engine state
    - runtime inspection of rule outputs
    - manual triggering of evaluation cycles
    - configuration overrides for HUD layout and UI behavior

Debug tools should help verify:

    - deterministic task identity
    - rule evaluation correctness
    - baseline capture behavior
    - engine performance boundaries

These tools are critical for validating complex Task Builder rules during
development.

## 21.16 Testing Strategy ##

Testing should occur continuously throughout implementation.

Recommended testing methods:

    - unit testing rule evaluation logic
    - deterministic ID verification tests
    - simulated evaluation contexts
    - save/load integrity tests
    - in-game validation for UI behavior
    - stage-gate checks for G1 through G7 at Now to Next boundaries

Special attention should be given to:

    - rule baseline capture correctness
    - persistence migration safety
    - collision handling in the task pipeline

## 21.17 Version 1 Release Criteria ##

Version 1 of the mod is considered ready when the following systems are stable:

    - deterministic task generation
    - built-in automatic tasks
    - Task Builder rule creation
    - manual task management
    - HUD display
    - menu dashboard
    - persistence and save migration
    - daily history snapshots

Features explicitly excluded from Version 1 include:

    - multiplayer synchronization
    - task dismissal or snoozing
    - user-defined task priorities
    - cross-save task sharing

Future versions may expand the system once the Version 1 architecture has
proven stable.
