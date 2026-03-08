
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

The mod should be implemented using an incremental architecture-first strategy.

Key principles:

    - implement core domain models first
    - introduce the State Store before UI systems
    - implement the task engine before Task Builder UI
    - verify deterministic task identity early
    - ensure persistence works before advanced features

Each phase should produce a functional checkpoint that can run in-game.

## 21.3 Phase Overview ##

The implementation is divided into the following phases.

| Phase | Focus |
| ------ | ------------------------------- |
| Phase 1 | Project skeleton and lifecycle |
| Phase 2 | Core domain model |
| Phase 3 | State store |
| Phase 4 | View model infrastructure |
| Phase 5 | Built‑in task generators |
| Phase 6 | Rule evaluation engine |
| Phase 7 | Persistence system |
| Phase 8 | Menu dashboard |
| Phase 9 | HUD interface |
| Phase 10 | Task Builder wizard |
| Phase 11 | History browsing UI |
| Phase 12 | Debug and developer tooling |

Later phases depend on stable completion of earlier phases.

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

    - `TaskID`
    - `TaskObject`
    - `TaskCategory`
    - `TaskStatus`
    - `DayKey`

Deterministic identifier rules defined in Section 3 must be implemented at this
stage. fileciteturn1file19

Phase 2 does not introduce a stored per-task priority field in `TaskObject` or
daily snapshots.

This phase should include helper utilities for:

    - deterministic TaskID generation
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

Phase 3 introduces the centralized State Store defined in Section 4.9. fileciteturn1file18

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

The menu is implemented before the HUD because it exercises the full
StardewUI menu pipeline (layout, interaction, tabs, navigation) and
validates the ViewModel binding layer on a simpler interaction surface
than the always-on HUD drawable.

Menu capabilities include:

    - full task list browsing
    - history browsing
    - manual task creation
    - task editing
    - configuration controls
    - statistics access (V2)

The menu is also responsible for launching the Task Builder wizard.

The menu uses the same store snapshot model as the HUD.

## 21.12 Phase 9 — HUD Interface ##

Phase 9 introduces the in-game HUD interface.

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

Reference: Section 6. fileciteturn1file15

The wizard must guide users through:

1. selecting triggers
2. defining conditions
3. configuring progress tracking
4. selecting task presentation settings
5. choosing persistence behavior

Each step must validate rule inputs before allowing progression.

The final rule must serialize into the `RuleDefinition` structure.

## 21.14 Phase 11 — History Browsing UI ##

Phase 11 introduces the history browsing UI.

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

## 21.15 Testing Strategy ##

Testing should occur continuously throughout implementation.

Recommended testing methods:

    - unit testing rule evaluation logic
    - deterministic ID verification tests
    - simulated evaluation contexts
    - save/load integrity tests
    - in-game validation for UI behavior

Special attention should be given to:

    - rule baseline capture correctness
    - persistence migration safety
    - collision handling in the task pipeline

## 21.16 Version 1 Release Criteria ##

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
