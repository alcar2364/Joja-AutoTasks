# Section 21 — Implementation Plan

## 21.1 Purpose

This section defines the implementation plan for building the Automatic Task Manager mod. The goal is to translate the architecture and design decisions defined throughout this document into a structured development roadmap.

The implementation plan prioritizes:

- correctness and deterministic behavior
- architectural stability
- incremental feature delivery
- testability and debugging visibility

Implementation must follow the architectural boundaries defined in Section 2 and the domain models defined in Sections 3 and 4.

The plan is organized into implementation phases. Each phase introduces new subsystems while maintaining a working mod state.

## 21.2 Implementation Strategy

The mod uses a no-drop, architecture-first delivery strategy with right-sized staging. Core feature intent remains unchanged; sequencing controls complexity.

Delivery stages:

- Now: build Phase 1 through Phase 10 plus the minimum Phase 11 and Phase 12 slices required for Version 1 viability.
- Next: complete the remaining Phase 11 and Phase 12 breadth that is not required to ship Version 1 safely.
- Later: deliver post-Version-1 expansion work (for example, statistics dashboards) without changing Version 1 scope commitments.

Non-negotiable constraints:

- Task Builder is a core differentiator and must remain on the critical path.
- Both UI surfaces (HUD and full menu) are required for Version 1.
- No capability in Phase 8 through Phase 12 is dropped; deferral is sequencing only.
- Code and documentation must be updated together, with variances recorded and justified.

Canonical precedence for implementation planning:

- This section is canonical for phase sequencing, Version 1 boundaries, and delivery-stage intent.
- `ImplementationIssues` scheduled-target metadata is canonical for issue-tracker scheduling and remains authoritative until separately retargeted.
- If sequencing or boundary metadata diverges, update `ImplementationIssues` records explicitly; do not reinterpret scope implicitly.

Each phase must end in a runnable checkpoint and pass the stage gates in Section 21.3.2 before promotion.

## 21.3 Phase Overview

The implementation is divided into the following phases.

| Phase    | Focus                          | Delivery stage |
| -------- | ------------------------------ | -------------- |
| Phase 1  | Project skeleton and lifecycle | Now            |
| Phase 2  | Core domain model              | Now            |
| Phase 3  | State store                    | Now            |
| Phase 4  | View model infrastructure      | Now            |
| Phase 5  | Built-in task generators       | Now            |
| Phase 6  | Rule evaluation engine         | Now            |
| Phase 7  | Persistence system             | Now            |
| Phase 8  | Menu dashboard                 | Now            |
| Phase 9  | HUD interface                  | Now            |
| Phase 10 | Task Builder wizard            | Now            |
| Phase 11 | History browsing UI            | Now + Next     |
| Phase 12 | Debug and developer tooling    | Now + Next     |

Later phases depend on stable completion of earlier phases.

### 21.3.1 No-drop capability mapping (Version 1 + Phase 8-12)

| Capability | Phase owner | Version 1 requirement | Stage | Notes |
| --- | --- | --- | --- | --- |
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

### 21.3.2 Stage gates

All gates are mandatory at stage boundaries.

| Gate | Objective | Minimum pass criteria |
| --- | --- | --- |
| G1 Boundary integrity | Preserve command/snapshot architecture | UI and Task Builder paths do not mutate canonical task state directly. |
| G2 Differentiator viability | Keep Task Builder shippable | Wizard can define, validate, serialize, and persist rules end-to-end. |
| G3 Dual-surface parity | Keep HUD and menu behavior aligned | HUD and menu consume the same snapshot and expose coherent task data. |
| G4 Persistence safety | Protect save stability | Versioning, migrations, and reconstruction behavior are validated. |
| G5 Performance envelope | Keep runtime cost bounded | Evaluation remains event-driven and HUD/menu rebuilds are change-driven. |
| G6 Scope budget | Control complexity without feature drops | Deferrals are stage-only and all deferred capabilities remain mapped. |
| G7 Doc-sync closure | Keep docs and code aligned | Design sections are updated first, Architecture Map reconciled second, and variances logged. |

### 21.3.3 Code-doc synchronization and variance policy

Synchronization order:

1. Update canonical design guide sections first.
2. Reconcile Architecture Map wording and references against the updated canonical sections.
3. Record any accepted divergence in the variance register with explicit justification and closure target.

Variance register template:

| Variance ID | Canonical source | Variant artifact | Justification | Decision | Closure target | Status |
| --- | --- | --- | --- | --- | --- | --- |
| VAR-XXX | Design guide section | Code or secondary doc reference | Why divergence exists | Keep/update/deprecate | Planned closure milestone | Open/closed |

Seeded baseline variance entry:

| Variance ID | Canonical source | Variant artifact | Justification | Decision | Closure target | Status |
| --- | --- | --- | --- | --- | --- | --- |
| VAR-001 | Identifier naming in design docs (`RuleID`, `SubjectID`) | Current code symbols and file paths (`RuleId`, `SubjectId`; `Domain/Identifiers/RuleId.cs`, `Domain/Identifiers/SubjectId.cs`) | Legacy docs and checklist wording still use older acronym style | Keep code canonical and update legacy docs incrementally | Doc-sync closure before Version 1 RC | Open |
| VAR-002 | Section 03 / Section 21 identifier naming (`Manual_N`) | Test files using `ManualTask_N` | Legacy naming split; `Manual_N` is canonical; `ManualTask_N` appears in some test fixtures | Keep `Manual_N` canonical; update test fixtures incrementally | Doc-sync closure before Version 1 RC | Open |
| VAR-003 | Section 07 rule evaluation model ("commands only" wording) | Pre-fix wording in Section 07 that said "commands or actions" | Pre-fix divergence; resolved by T1 doc fix in this phase | Resolved by T1 doc fix | Closed by T1 in this phase | Closed |
| VAR-004 | Section 09 persistence model (history exclusion in §9.12) | Pre-fix wording excluded historical task ledger from V1 | Pre-fix divergence; resolved by T1/T5 doc fixes adding ledger to `SaveData` and removing the stale §9.12 exclusion | Resolved by T1/T5 Section 09 doc fixes | Closed by T1/T5 in this phase | Closed |

**Note:** Findings that represent bugs to fix (rather than accepted divergences) are tracked in the `ImplementationIssues` folder (`Project/Tasks/ImplementationPlan/ImplementationIssues/`), not in this variance register.

## 21.4 Phase 1 — Project Skeleton and Lifecycle

Phase 1 establishes the basic mod structure and lifecycle integration.

Design coverage:

- Section 02 system architecture composition-root responsibilities
- Section 12 engine update-cycle lifecycle hooks and safe startup/teardown boundaries (partial)
- Section 15 configuration bootstrap and early-load requirements (partial)

Responsibilities:

- Create SMAPI entry point
- Initialize logging and configuration
- Establish subsystem initialization order
- Hook required game events

Key systems initialized in this phase:

- lifecycle coordinator
- event dispatcher
- configuration loader
- startup composition root and runtime container

Named classes/systems introduced or expanded in this phase:

- `ModEntry`
- `LifecycleCoordinator`
- `EventDispatcher`
- `ConfigLoader`
- `ModConfig`
- `BootstrapContainer`
- `ModRuntime`

Example lifecycle hooks:

- `OnSaveLoaded`
- `OnDayStarted`
- `OnReturnedToTitle`
- `OnSaving`
- `OnUpdateTicked`

This phase must ensure the mod loads safely without any task logic yet.

## 21.5 Phase 2 — Core Domain Model

Phase 2 implements the domain objects defined in the core data model.

Reference: Section 4.

Design coverage:

- Section 03 deterministic identifier model
- Section 04 core task-domain model
- Section 10 and Section 20 localization-neutral binding payload constraints (guardrail only; no UI implementation)

Key types include:

- `TaskId`
- `RuleId`
- `SubjectId`
- `TaskObject`
- `TaskCategory`
- `TaskStatus`
- `TaskSourceType`
- `DayKey`

Named classes/systems introduced or expanded in this phase:

- `TaskId`
- `RuleId`
- `SubjectId`
- `DayKey`
- `TaskIdFactory`
- `TaskIdFormat`
- `DayKeyFactory`
- `IdentifierUtility`
- `TaskObject`
- `TaskCategory`
- `TaskStatus`
- `TaskSourceType`

Deterministic identifier rules defined in Section 3 must be implemented at this stage. fileciteturn1file19

Phase 2 does not introduce a stored per-task priority field in `TaskObject` or daily snapshots.

This phase should include helper utilities for:

- deterministic TaskId generation
- day key construction

Deterministic task-type sort mechanism implementation (derived ordering map, fallback chain, comparer, and comparer tests) is owned by Phase 5 once task generator/task-type coverage is stable enough to finalize the Version 1 ordering contract.

Future customization of ordering should use user-configurable ordering/sort profiles rather than stored per-task priorities.

Phase 2 localization scope constraint:

- Phase 2 may define documentation and architecture guardrails for localization boundaries only.
- Any translation behavior change is deferred to UI/view-model phases, consistent with Section 2.6, Section 10.15, and Section 20.12. Backend and engine layers remain localization-neutral per Section 5.13.
- No additional implementation checklist expansion is introduced in this section for post-Phase 2 localization work.

No UI or engine logic should exist yet.

## 21.6 Phase 3 — State Store

Phase 3 introduces the centralized State Store defined in Section 4.9.

Design coverage:

- Section 04.9 State Store and snapshot read model
- Section 08 state-store command model
- Section 16 command/state validation and consistency boundaries (partial)
- Section 19 linear mutation/projection performance constraints (partial)

Responsibilities:

- maintain active tasks
- store progress and completion states
- manage pinned tasks
- publish immutable snapshots for UI

The store must enforce command-based mutation.

Canonical command types for this phase:

- `AddOrUpdateTaskCommand`
- `CompleteTaskCommand`
- `UncompleteTaskCommand`
- `RemoveTaskCommand`
- `PinTaskCommand`
- `UnpinTaskCommand`

Named classes/systems introduced or expanded in this phase:

- `StateStore`
- `StateContainer`
- `SnapshotProjector`
- `TaskRecord`
- `TaskView`
- `TaskSnapshot`
- `SnapshotChangedEventArgs`
- `AddOrUpdateTaskCommand`
- `CompleteTaskCommand`
- `UncompleteTaskCommand`
- `RemoveTaskCommand`
- `PinTaskCommand`
- `UnpinTaskCommand`
- command handler set under `State/Handlers`

The State Store becomes the authoritative runtime representation of tasks.

## 21.7 Phase 4 — View Model Infrastructure

Phase 4 introduces the View Model layer defined in Section 10A.

Design coverage:

- Section 10 UI data-binding model (foundational live-snapshot consumption only)
- Section 10A view-model architecture foundation
- Section 15 configuration-system binding boundary (partial)
- Section 20 shared snapshot-parity and binding-surface rules (partial)

Responsibilities:

- Implement base view model with INPC via PropertyChanged.SourceGenerator
- Create and implement `HudViewModel` as an initial live snapshot consumer.
- Create and implement `TaskListViewModel` as an initial live snapshot consumer.
- Create and implement `ConfigViewModel` as an in-scope foundational ViewModel for this phase.
- Subscribe to `SnapshotChanged` (Section 8.12) and project snapshot data into bindable properties
- Establish the command dispatch pattern from view models back to the State Store

Named classes/systems introduced or expanded in this phase:

- `UiViewModelBase` (or renamed base ViewModel foundation)
- `UISnapshotSubscriptionManager`
- `HudViewModel`
- `TaskListViewModel`
- `TaskItemViewModel` or equivalent row/item projection model
- `ConfigViewModel`

Phase 4 incorporates deferred carryover from earlier phases through `ImplementationIssues` beginning at this stage, including:

- #100 localization/translation runtime behavior (retargeted to Phase 4)
- #106 terminology clarification
- #107 snapshot subscription wiring
- #108 INPC projection behavior
- #109 UI-local state ownership boundaries
- #159 config loader exception hardening (with #86 treated as duplicate and merged into #159)

This phase validates the State Store → ViewModel → UI binding pipeline before any StarML layout work begins. View models should be testable without a running game.

### 21.7.1 Carryover mapping for Phases 5-9 (ImplementationIssues)

Carryover mapping in this section is explicit and based on active issue records.

Active-record carryover ID set:

- #103-#127, #131, #132
- #128-#130 are not represented as active records currently.

Phase 5 carryover (`ImplementationIssues`):

- #104, #105, #110, #111, #112, #113, #126, #131

Phase 6 carryover (`ImplementationIssues`):

- #103, #114, #115, #127, #132

Phase 7 carryover (`ImplementationIssues`):

- #116, #117, #118, #119

Phase 8 carryover (`ImplementationIssues`):

- #120, #121, #124
- Planning boundary note: Version 1 treats #124 as Later/V2+ scope. `ImplementationIssues` scheduled-target metadata remains canonical until separately retargeted.

Phase 9 carryover (`ImplementationIssues`):

- #122, #123, #125
- Planning boundary note: Version 1 treats #122 and #123 as Later/V2+ scope. `ImplementationIssues` scheduled-target metadata remains canonical until separately retargeted.
- Planning boundary note: #125 remains Later/V2+ scope in this plan.

### 21.7.2 Cross-cutting ownership and dependency mapping

This mapping locks ownership for cross-section capabilities without changing phase order.

| Source section capability | Phase owner | Dependency chain | Delivery stage | Notes |
| --- | --- | --- | --- | --- |
| Section 20.8 toast routing and notification presentation | Phase 9 | Phase 4 ViewModel snapshot projection and command feedback pathways | Now | Toast routing is HUD-owned delivery work and remains out of Phase 4 implementation scope. |
| Section 20.10 onboarding flows and first-run UX guidance | Phase 8 | Phase 7 persistence of `OnboardingAcknowledged` and related user-state restoration | Now | Onboarding implementation is menu-owned; persistence preconditions are Phase 7-owned. |
| Section 12 engine update-cycle queueing, scheduling, and deterministic task-emission flow | Phases 5, 6, and 7 | Phase 1 lifecycle hooks; Phase 3 command boundaries; Phase 4 UI consumption contracts | Now | Phase 5 owns built-in generator emission, Phase 6 owns rule-driven evaluation scheduling, and Phase 7 coordinates save/load interactions. |
| Section 15 configuration system and full configuration menu | Phases 1, 8, 9, and 12 | Phase 1 config bootstrap; Phase 7 persistence/versioning; Phase 8 full menu controls; Phase 9 HUD-facing presentation config; Phase 12 debug tuning | Now + Next | Configuration concerns are cross-phase and should not be collapsed into wizard ownership. |
| Section 16 error handling, validation, and failure isolation | Phases 3, 6, 7, and 12 | Phase 3 command/state validation; Phase 6 rule/generator validation; Phase 7 persistence recovery; Phase 12 diagnostics | Now + Next | Validation and recovery are cross-cutting contracts, not a history-only concern. |
| Section 17 diagnostics, debug tooling, and developer controls | Phase 12 | Phase 6 evaluation telemetry; Phase 7 persistence metadata; Phase 8 menu-hosted debug navigation; Phase 9 HUD overlay hooks | Now + Next | Debug UX depth remains staged, but the owner section is Section 17. |
| Section 18 versioning and migration strategy | Phase 7 | Phase 15 config schema/version surface; Phase 10/6 rule-definition and runtime compatibility | Now | Migration strategy is persistence-owned even when individual domains version independently. |
| Section 19 performance guardrails and G5 acceptance constraints | Phases 5, 6, 8, 9, and 12 | Generator/runtime budgets; UI reconciliation costs; diagnostic overhead controls | Now + Next | Section 19 is a cross-cutting constraint set, not a debug-tools section. |

## 21.8 Phase 5 — Built-in Task Generators

Phase 5 introduces built-in automatic task generators.

Design coverage:

- Section 05 task generation and evaluation engine (built-in generator slice)
- Section 12 engine update-cycle trigger and emission flow (partial)
- Section 13 built-in task generators
- Section 19 performance guardrails for generator scans and emission costs (partial)

Reference: Section 5. fileciteturn1file17

Minimum Version 1 generator scope must follow the normative built-in set defined in Section 13.10.

Initial generators should be implemented for the most stable gameplay systems:

- crop care generator
- animal care generator
- machine output generator
- calendar reminders
- quest progress

Generators must operate using the Generation/Evaluation Context defined in Section 5.3 rather than querying the game directly. fileciteturn1file17

Named classes/systems introduced or expanded in this phase:

- `EvaluationContext`
- `IGameStateProvider`
- `ITaskGenerator`
- `GeneratedTask`
- built-in generator implementations for the committed Version 1 generator set
- V1 generator identities: `BuiltIn.Crops.Water`, `BuiltIn.Crops.Harvest`, `BuiltIn.Animals.Pet`, `BuiltIn.Animals.Milk`, `BuiltIn.Machines.Harvest`, `BuiltIn.Calendar.Festival`, `BuiltIn.Calendar.Birthday`, `BuiltIn.Quest.Progress`
- command emission into `StateStore` through the Phase 3 command boundary

Outputs from generators must pass through the shared task pipeline.

## 21.9 Phase 6 — Rule Evaluation Engine

Phase 6 implements the rule engine that evaluates Task Builder rules.

Design coverage:

- Section 05 shared evaluation pipeline (rule-driven slice)
- Section 06 runtime consumption of serialized rule definitions (partial)
- Section 07 rule evaluation model
- Section 12 queued evaluation and dirty-flag scheduling (partial)
- Section 16 rule validation and failure isolation (partial)
- Section 19 runtime performance constraints (partial)

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

Named classes/systems introduced or expanded in this phase:

- `RuleDefinition` runtime consumption pipeline
- serialized rule submodels: `Metadata`, `Trigger`, `ConditionTree`, `ProgressModel`, `OutputModel`
- `RuleRuntimeRecord`
- rule runtime cache
- trigger mapping and condition-tree evaluation pipeline
- baseline capture and completion-reconciliation logic
- rule-driven command emission through the Phase 3 command boundary

Rules should be testable independently of the UI.

## 21.10 Phase 7 — Persistence System

Phase 7 implements the persistence model.

Design coverage:

- Section 09 persistence model
- Section 11 daily snapshot ledger capture and storage (persistence slice)
- Section 18 versioning and migration strategy
- Section 03 identifier continuity constraints for persisted IDs and `DayKey` values (partial)
- Section 15 config version/load behavior (partial)

Reference: Section 9. fileciteturn1file13

Responsibilities:

- save rule definitions
- save manual tasks
- save runtime baseline values
- save store-level user state
- persist Daily Snapshot Ledger state required by Section 11 and Section 9.14
- migrate save data between schema versions

Named classes/systems introduced or expanded in this phase:

- `SaveData`
- `StoreUserState`
- persisted `RuleRuntimeData`
- `DailySnapshotLedger`
- `DailyTaskSnapshot`
- `HistoricalTaskRecord`
- persisted `ManualTaskCounter` continuity support
- persisted onboarding/user-state flags such as `OnboardingAcknowledged`
- config/rule/store migration pipeline

Persisted identifier and ledger formats in this phase must follow the canonical Section 03 `TaskId` and `DayKey` rules.

Persistence must follow the minimal storage principle:

only essential state is stored; derived task state is reconstructed by the engine.

Save/load lifecycle must align with the mod lifecycle defined in Section 2.5.

## 21.11 Phase 8 — Menu Dashboard

Phase 8 implements the main mod menu.

Design coverage:

- Section 10 menu binding behavior (menu slice)
- Section 10A menu-surface ViewModel catalog (partial)
- Section 15 full StardewUI configuration menu (partial)
- Section 20 menu-system design
- Section 14 wizard-launch hosting shell (partial)
- Section 11 history entry-point integration (partial)

Stage intent:

- Now: ship the required Version 1 management surface.
- Next: deepen non-blocking workflow polish without dropping capability.

The menu is implemented before the HUD because it exercises the full StardewUI menu pipeline (layout, interaction, tabs, navigation) and validates the ViewModel binding layer on a simpler interaction surface than the always-on HUD drawable.

Menu capabilities include:

- full task list browsing
- history entry points and baseline integration wiring
- manual task creation
- task editing
- configuration controls
- statistics access (V2)

ViewModel classes created or expanded in this phase:

- `MenuShellViewModel` (or equivalent root menu binding context)
- `TaskListViewModel` (expanded for full menu tasks-section behavior)
- `TaskDetailViewModel`
- `ManualTaskEditorViewModel`
- `ConfigViewModel` (expanded for full menu configuration controls)

Detailed history browsing interactions are delivered in Phase 11 (Now baseline, Next depth).

The menu is also responsible for launching the Task Builder wizard.

The menu uses the same store snapshot model as the HUD.

Named classes/systems introduced or expanded in this phase:

- menu navigation shell and section routing infrastructure
- full StardewUI configuration menu wiring over `ConfigViewModel`
- task-selection, detail-panel, and manual-task editor menu composition

## 21.12 Phase 9 — HUD Interface

Phase 9 introduces the in-game HUD interface.

Design coverage:

- Section 10 HUD binding behavior (HUD slice)
- Section 10A HUD ViewModel catalog (partial)
- Section 20 HUD-system design
- Section 08 `ToastRequested` / outbound effect routing consumption boundary (partial)
- Section 19 HUD reconciliation and redraw guardrails (partial)

Stage intent:

- Now: ship the required Version 1 in-game surface.
- Next: tune interaction ergonomics while preserving parity.

The HUD uses StardewUI's `IViewDrawable` API (`viewEngine.CreateDrawableFromAsset()`) as described in Section 20.6.1.

HUD responsibilities:

- display active tasks
- allow task selection
- show task details
- support scrolling
- optionally show completed tasks

ViewModel classes created or expanded in this phase:

- `HudViewModel` (expanded for full HUD interaction behavior)
- `HudTaskRowViewModel` or equivalent HUD row/item ViewModel role

The HUD must consume immutable snapshots from the State Store.

It must never mutate task objects directly.

Named classes/systems introduced or expanded in this phase:

- HUD `IViewDrawable` integration and drawable lifecycle wiring
- HUD row reconciliation and bounded interaction wiring
- native toast/effect routing consumption outside the binding layer

## 21.13 Phase 10 — Task Builder Wizard

Phase 10 implements the Task Builder UI system.

Design coverage:

- Section 06 task-builder rule serialization authoring surface
- Section 14 task-builder wizard UX
- Section 16 wizard-time validation and identity-affecting edit safeguards (partial)
- Section 20 menu-hosted wizard integration (partial)

Stage intent:

- Now: ship the core differentiator with full rule-definition flow.
- Next: add polish and template-depth improvements without changing command/snapshot boundaries.

Reference: Section 6 and Section 14. fileciteturn1file15

The wizard must guide users through:

1. selecting triggers
2. defining conditions
3. configuring progress tracking
4. selecting task presentation settings
5. choosing persistence behavior

Each step must validate rule inputs before allowing progression.

The final rule must serialize into the `RuleDefinition` structure.

Named classes/systems introduced or expanded in this phase:

- `RuleDefinition`
- `Metadata`
- `Trigger`
- `ConditionTree`
- `ProgressModel`
- `OutputModel`

ViewModel classes created in this phase:

- `WizardViewModel`
- `WizardStepViewModel` (one or more concrete step models as needed)
- `WizardPreviewViewModel`

Boundary constraint:

- Task Builder in Now is limited to rule-definition authoring, validation, and persistence.
- Task Builder must not directly create, complete, or mutate runtime task entities.
- Runtime task changes occur only after rule evaluation through command/state-store boundaries.

## 21.14 Phase 11 — History Browsing UI

Phase 11 introduces the history browsing UI.

Design coverage:

- Section 11 daily snapshot ledger query and browse model
- Section 10A history-facing ViewModel and ledger-access architecture (partial)
- Section 20 history-browsing surface design (partial)
- Section 09 persisted ledger query contract (partial)
- Section 03 canonical `DayKey` and identifier formatting constraints for history navigation (partial)

Stage intent:

- Now: baseline day browsing and day navigation required for Version 1.
- Next: filtering and quick-jump depth hardening.

The system reads data through the read-only history-query interface over the Daily Snapshot Ledger defined in Section 11.

Capabilities include:

- viewing previous days' task lists
- browsing completed and incomplete tasks for a selected day
- date navigation for day-to-day browsing

ViewModel classes created in this phase:

- `HistoryViewModel`

Named classes/systems introduced or expanded in this phase:

- `IDailySnapshotLedger`
- `HistoryViewModel`
- `HistoricalTaskRowViewModel` or equivalent history-row projection role
- history-day navigation and empty-state query model over persistence-owned ledger records

History navigation and lookup in this phase must use the canonical Section 03 `DayKey` format rather than introducing a UI-local history key format.

Filtering and quick-jump depth are Next-stage scope for Phase 11 and are not part of the Version 1 baseline.

Statistics and analytics (V2): Completion statistics, productivity trends, and aggregate dashboards are deferred to Version 2. The Daily Snapshot Ledger provides the data foundation. Statistics must always derive from historical snapshots rather than duplicating stored data.

## 21.15 Phase 12 — Debug and Development Tools

Phase 12 implements debugging and developer tooling.

Design coverage:

- Section 17 debug and development tools
- Section 10A debug-facing ViewModel architecture (partial)
- Section 16 validation diagnostics and operator-visible failure reporting (partial)
- Section 19 performance instrumentation and diagnostic overhead controls (partial)
- Section 12 manual trigger tooling and evaluation-cycle observability (partial)

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

ViewModel classes created in this phase:

- `DebugViewModel`

Named classes/systems introduced or expanded in this phase:

- `ModLogger`
- `LogEvents`
- debug overlay, task-state inspector, and developer command surfaces

Debug tools should help verify:

- deterministic task identity
- rule evaluation correctness
- baseline capture behavior
- engine performance boundaries

These tools are critical for validating complex Task Builder rules during development.

## 21.16 Testing Strategy

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

## 21.17 Version 1 Release Criteria

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

Future versions may expand the system once the Version 1 architecture has proven stable.
