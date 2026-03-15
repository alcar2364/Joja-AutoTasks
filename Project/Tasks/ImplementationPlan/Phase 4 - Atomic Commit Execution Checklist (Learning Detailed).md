# Phase 4 - Atomic Commit Execution Checklist (Learning Detailed)

Purpose: This is a separate, finer-grained execution checklist derived from the Phase 4 implementation plan. It is optimized for step-by-step learning and atomic implementation flow.

## Planning Consistency Checks (Pre-Publish)

- [x] Section 21 mapping check completed: Phase 4 scope is View model infrastructure and command/snapshot binding pipeline.
- [x] ImplementationIssues schedule check completed: #100, #106, #108, #109, #159 are scheduled to Phase 4 and remain in scope.
- [x] Merged duplicate check completed: #159 is canonical active tracker for merged #86 scope.
- [x] Active issue status note captured: #107 is already marked Resolved in issue record and is treated here as verification/hardening, not net-new implementation.

## Guardrails (Must Stay True)

- [ ] Phase 4 only. Do not pull in Phase 5+ generator/rule/persistence/UI-rendering work.
- [ ] ViewModels never mutate canonical state directly. Canonical writes must route through command dispatch.
- [ ] Snapshot projection is deterministic. Same snapshot input must produce same ViewModel state.
- [ ] No direct game API calls in ViewModels.
- [ ] Keep code changes small and traceable; prefer atomic commits per substep.
- [ ] Maintain existing architecture boundaries: State ownership in StateStore, lifecycle forwarding in Lifecycle, UI state projection in ViewModels.
- [ ] Comprehensive command surface may be scaffolded now and trimmed later after real usage proves what is unnecessary.

## Phase Overview

### Phase Goal

- [ ] Deliver a testable (without running game runtime) State Store -> ViewModel -> UI binding pipeline with explicit command dispatch path back to StateStore.

### Scope Anchors

- [ ] Base ViewModel foundation for INPC aligned to PropertyChanged.SourceGenerator direction.
- [ ] HudViewModel and TaskListViewModel as initial consumers.
- [ ] ConfigViewModel included in Phase 4 scope.
- [ ] SnapshotChanged subscription and projection into bindable properties.
- [ ] Explicit command dispatch adapter pattern from ViewModels to StateStore.

### Out of Scope

- [ ] No StarML layout implementation.
- [ ] No broad visual redesign.
- [ ] No generator/rule engine implementation.
- [ ] No persistence expansion outside #159 regression/hardening checks.

## 1) Baseline And Naming Alignment

Step goal:

- [ ] Align existing ViewModel files to locked Phase 4 naming and boundary conventions before adding new behavior.

### 1A - Confirm and lock namespace/boundary baseline

- [ ] Action: Review current files under UI/ViewModels and confirm namespace and architecture boundary intent in comments where clarity is needed.
- [ ] Scope: UI/ViewModels/UiViewModelBase.cs, UI/ViewModels/HudViewModel.cs, UI/ViewModels/TaskListViewModel.cs, UI/ViewModels/TaskItemViewModel.cs
- [ ] Verify: Namespace is consistent across files and no direct game API usage exists.
- [ ] Suggested commit: phase4(1A): lock ViewModel namespace and boundary baseline
- [ ] Must include: explicit boundary clarity for learning context.
- [ ] Must exclude: behavior changes.

### 1B - Rename base class to ViewModelBase

- [ ] Action: Rename UiViewModelBase to ViewModelBase and update all references.
- [ ] Scope: UI/ViewModels/UiViewModelBase.cs (rename file and class), dependent ViewModel files
- [ ] Verify: Build compiles with no stale symbol references.
- [ ] Suggested commit: phase4(1B): rename UiViewModelBase to ViewModelBase
- [ ] Must include: symbol and file rename coherence.
- [ ] Must exclude: unrelated refactors.

### 1C - Lock naming conventions in-code

- [ ] Action: Align method/property naming in touched ViewModels with Phase 4 decisions (ApplySnapshot, Execute<Verb><Target>, Is/Has/Can prefixes, plural collections).
- [ ] Scope: UI/ViewModels/HudViewModel.cs, UI/ViewModels/TaskListViewModel.cs, UI/ViewModels/TaskItemViewModel.cs
- [ ] Verify: Naming is consistent and readable.
- [ ] Suggested commit: phase4(1C): align ViewModel naming conventions
- [ ] Must include: ApplySnapshot naming adoption where applicable.
- [ ] Must exclude: new features.

## Step 1 Completion

- [ ] All substeps in Step 1 complete (1A, 1B, 1C).

## 2) Subscription And Dispatch Abstractions

Step goal:

- [ ] Introduce explicit abstractions so ViewModels are testable and decoupled from runtime wiring.

### 2A - Create snapshot subscription abstraction

- [ ] Action: Add ISnapshotSubscriptionSource contract.
- [ ] Scope: UI/ViewModels/ISnapshotSubscriptionSource.cs
- [ ] Verify: Interface compiles and can be mocked in tests.
- [ ] Suggested commit: phase4(2A): add ISnapshotSubscriptionSource contract
- [ ] Must include: minimal contract for subscribe/unsubscribe token ownership.
- [ ] Must exclude: concrete behavior changes.

### 2B - Add StateStore-backed snapshot source adapter

- [ ] Action: Implement StateStoreSnapshotSubscriptionSource using existing UISnapshotSubscriptionManager behavior.
- [ ] Scope: UI/ViewModels/StateStoreSnapshotSubscriptionSource.cs, UI/UISnapshotSubscriptionManager.cs (only if adapter support needed)
- [ ] Verify: Adapter can subscribe and dispose deterministically.
- [ ] Suggested commit: phase4(2B): add StateStore snapshot subscription adapter
- [ ] Must include: deterministic token disposal path.
- [ ] Must exclude: snapshot payload transformation logic.

### 2C - Create command dispatch abstraction

- [ ] Action: Add IViewModelCommandDispatcher with comprehensive command surface for now.
- [ ] Scope: UI/ViewModels/IViewModelCommandDispatcher.cs
- [ ] Verify: Interface covers at least manual complete/uncomplete/add/update/delete and task-builder add/update/delete intent.
- [ ] Suggested commit: phase4(2C): add ViewModel command dispatcher contract
- [ ] Must include: broad command surface with note that trimming is expected later.
- [ ] Must exclude: direct StateStore dependencies in interface.

### 2D - Add StateStore-backed command adapter

- [ ] Action: Implement StateStoreCommandDispatcher bridging ViewModel calls to StateStore command dispatch.
- [ ] Scope: UI/ViewModels/StateStoreCommandDispatcher.cs
- [ ] Verify: Adapter compiles and routes each method to expected command path.
- [ ] Suggested commit: phase4(2D): implement StateStore command adapter for ViewModels
- [ ] Must include: command-only mutation path.
- [ ] Must exclude: UI projection logic.

## Step 2 Completion

- [ ] All substeps in Step 2 complete (2A, 2B, 2C, 2D).

## 3) Runtime Wiring (No UI Rendering)

Step goal:

- [ ] Wire new adapters through composition root without expanding runtime behavior surface beyond Phase 4 needs.

### 3A - Extend runtime composition objects

- [ ] Action: Add adapter instances to startup composition and runtime container.
- [ ] Scope: Startup/BootstrapContainer.cs, Startup/ModRuntime.cs
- [ ] Verify: Runtime exposes adapter dependencies required by ViewModels.
- [ ] Suggested commit: phase4(3A): wire viewmodel adapters into runtime composition
- [ ] Must include: initialization order safety.
- [ ] Must exclude: lifecycle behavior changes unrelated to adapter availability.

### 3B - Validate ModEntry lifecycle safety with adapter availability

- [ ] Action: Confirm ModEntry initialization still creates state store before any subscription source usage.
- [ ] Scope: ModEntry.cs
- [ ] Verify: No null/runtime ordering regressions.
- [ ] Suggested commit: phase4(3B): validate modentry adapter lifecycle ordering
- [ ] Must include: minimal guard comments if helpful.
- [ ] Must exclude: feature additions.

## Step 3 Completion

- [ ] All substeps in Step 3 complete (3A, 3B).

## 4) HudViewModel Projection

Step goal:

- [ ] Make HudViewModel use explicit ApplySnapshot projection and deterministic bindable updates.

### 4A - Refactor snapshot callback to ApplySnapshot

- [ ] Action: Keep subscription callback thin and route logic to ApplySnapshot(TaskSnapshot).
- [ ] Scope: UI/ViewModels/HudViewModel.cs
- [ ] Verify: Callback path and ApplySnapshot output are equivalent for same snapshot.
- [ ] Suggested commit: phase4(4A): route hud snapshot handling through ApplySnapshot
- [ ] Must include: deterministic count computation from snapshot only.
- [ ] Must exclude: command dispatch behavior.

### 4B - INPC behavior validation pass

- [ ] Action: Ensure property updates trigger change notifications as expected under selected INPC approach.
- [ ] Scope: UI/ViewModels/HudViewModel.cs, UI/ViewModels/ViewModelBase.cs
- [ ] Verify: Property changes observable via INotifyPropertyChanged subscribers.
- [ ] Suggested commit: phase4(4B): confirm hud INPC notification behavior
- [ ] Must include: no-op update behavior consideration.
- [ ] Must exclude: UI rendering work.

## Step 4 Completion

- [ ] All substeps in Step 4 complete (4A, 4B).

## 5) TaskListViewModel Projection And Action Methods

Step goal:

- [ ] Implement deterministic list projection and explicit command-forwarding methods.

### 5A - Add bindable task item collection

- [ ] Action: Introduce bindable collection property for task items.
- [ ] Scope: UI/ViewModels/TaskListViewModel.cs
- [ ] Verify: Collection is observable and initialized safely.
- [ ] Suggested commit: phase4(5A): add bindable task item collection to task list view model
- [ ] Must include: collection naming per convention.
- [ ] Must exclude: command wiring.

### 5B - Implement ApplySnapshot list projection

- [ ] Action: Map TaskSnapshot.TaskViews to TaskItemViewModel instances with deterministic ordering.
- [ ] Scope: UI/ViewModels/TaskListViewModel.cs, UI/ViewModels/TaskItemViewModel.cs
- [ ] Verify: Same snapshot yields same list state and ordering.
- [ ] Suggested commit: phase4(5B): implement deterministic task list snapshot projection
- [ ] Must include: update strategy notes (replace-all or reconcile) documented.
- [ ] Must exclude: non-snapshot data dependencies.

### 5C - Add command forwarding methods (comprehensive initial set)

- [ ] Action: Add Execute<Verb><Target> methods that route through IViewModelCommandDispatcher.
- [ ] Scope: UI/ViewModels/TaskListViewModel.cs
- [ ] Verify: Methods call dispatcher and do not mutate canonical state locally.
- [ ] Suggested commit: phase4(5C): add task list command forwarding methods
- [ ] Must include: manual complete/uncomplete/add/update/delete and task-builder add/update/delete entrypoints.
- [ ] Must exclude: direct StateStore calls in ViewModel.

### 5D - Add trimming marker for later simplification

- [ ] Action: Add TODO/comment marker noting command surface may be trimmed after implementation evidence.
- [ ] Scope: UI/ViewModels/IViewModelCommandDispatcher.cs, UI/ViewModels/TaskListViewModel.cs
- [ ] Verify: Future simplification intent is explicit for maintainers.
- [ ] Suggested commit: phase4(5D): document future command-surface trimming pass
- [ ] Must include: rationale linked to learning-first broad coverage strategy.
- [ ] Must exclude: immediate command removals.

## Step 5 Completion

- [ ] All substeps in Step 5 complete (5A, 5B, 5C, 5D).

## 6) ConfigViewModel Introduction

Step goal:

- [ ] Add ConfigViewModel as a Phase 4 consumer with clear boundary behavior.

### 6A - Add ConfigViewModel with bindable config projection

- [ ] Action: Create ConfigViewModel with bindable properties from ModConfig.
- [ ] Scope: UI/ViewModels/ConfigViewModel.cs
- [ ] Verify: Property values project correctly from source config object.
- [ ] Suggested commit: phase4(6A): add config view model bindable projection
- [ ] Must include: naming consistency and no direct game API usage.
- [ ] Must exclude: persistence write logic.

### 6B - Add config update dispatch path (if in scope)

- [ ] Action: Route config-change intent through an explicit boundary method (dispatcher or dedicated adapter).
- [ ] Scope: UI/ViewModels/ConfigViewModel.cs, wiring files as needed
- [ ] Verify: Config update path is explicit and testable.
- [ ] Suggested commit: phase4(6B): add config update command boundary path
- [ ] Must include: boundary clarity for learning.
- [ ] Must exclude: schema/migration expansion.

## Step 6 Completion

- [ ] All substeps in Step 6 complete (6A, 6B).

## 7) Carryover Issue Resolution Pass

Step goal:

- [ ] Resolve or explicitly re-defer Phase 4 carryover issues with rationale.

### 7A - Issue 106 terminology clarification

- [ ] Action: Clarify TaskSourceType vs SourceIdentifier usage in touched ViewModel/adapter code and note result in issue record.
- [ ] Scope: relevant UI/ViewModels files, ImplementationIssues/Records/issue-106.md
- [ ] Verify: Terminology usage is internally consistent.
- [ ] Suggested commit: phase4(7A): clarify TaskSourceType and SourceIdentifier terminology

### 7B - Issue 100 localization/translation runtime behavior

- [ ] Action: Introduce minimal localization-ready abstraction points in ViewModels and document runtime behavior expectations.
- [ ] Scope: UI/ViewModels/ILocalizationProvider.cs, UI/ViewModels/DefaultLocalizationProvider.cs, affected ViewModels, ImplementationIssues/Records/issue-100.md
- [ ] Verify: User-facing text generation paths are localizable and testable.
- [ ] Suggested commit: phase4(7B): add localization-ready viewmodel abstraction points

### 7C - Issue 108 INPC projection behavior

- [ ] Action: Confirm projection updates raise expected notifications and remain deterministic.
- [ ] Scope: HudViewModel, TaskListViewModel, ConfigViewModel, issue-108 record updates
- [ ] Verify: INPC behavior documented and validated in tests.
- [ ] Suggested commit: phase4(7C): harden INPC projection behavior and evidence

### 7D - Issue 109 UI-local state ownership

- [ ] Action: Identify UI-local state fields (selection/filter/scroll placeholders) and keep canonical state in snapshots.
- [ ] Scope: TaskListViewModel and related UI-local state holders, issue-109 record updates
- [ ] Verify: No UI-local canonical state leakage.
- [ ] Suggested commit: phase4(7D): establish UI-local state ownership boundaries

### 7E - Issue 159 config loader hardening regression pass

- [ ] Action: Verify current ConfigLoader exception handling remains compliant and does not regress while adding ConfigViewModel paths.
- [ ] Scope: Configuration/ConfigLoader.cs, Tests/Configuration/ConfigLoaderTests.cs, Tests/Configuration/ConfigLoaderMigrationSafetyTests.cs, issue-159 record updates
- [ ] Verify: Fatal CLR exceptions are not swallowed; fallback behavior and structured logging remain intact.
- [ ] Suggested commit: phase4(7E): reconfirm configloader exception hardening invariants

### 7F - Issue 107 resolved-state verification

- [ ] Action: Record verification that #107 remains satisfied after refactors.
- [ ] Scope: ImplementationIssues/Records/issue-107.md (notes section), affected subscription files
- [ ] Verify: No regression in subscription lifecycle behavior.
- [ ] Suggested commit: phase4(7F): verify no regression for resolved snapshot subscription issue

## Step 7 Completion

- [ ] All substeps in Step 7 complete (7A, 7B, 7C, 7D, 7E, 7F).

## 8) Unit Test Checklist (AI-Assisted)

Step goal:

- [ ] Create deterministic ViewModel tests that validate projection, INPC, and dispatch boundaries.

### 8A - Add test doubles and fixtures

- [ ] Action: Create reusable test doubles for snapshot source, command dispatcher, and localization/config dependencies.
- [ ] Scope: Tests/UI/ViewModels/ViewModelTestDoubles.cs
- [ ] Verify: Test doubles are reusable across ViewModel test classes.
- [ ] Suggested commit: phase4(8A): add viewmodel test doubles and fixtures

### 8B - HudViewModel tests

- [ ] Action: Add projection + INPC tests for HudViewModel.
- [ ] Scope: Tests/UI/ViewModels/HudViewModelTests.cs
- [ ] Verify: Counts and notifications match snapshot transitions.
- [ ] Suggested commit: phase4(8B): add hud viewmodel projection and INPC tests

### 8C - TaskListViewModel tests

- [ ] Action: Add projection ordering + command forwarding tests.
- [ ] Scope: Tests/UI/ViewModels/TaskListViewModelTests.cs
- [ ] Verify: Deterministic list state and dispatcher call coverage.
- [ ] Suggested commit: phase4(8C): add task list projection and dispatch tests

### 8D - ConfigViewModel tests

- [ ] Action: Add bindable config projection tests and boundary checks.
- [ ] Scope: Tests/UI/ViewModels/ConfigViewModelTests.cs
- [ ] Verify: Config projection and update path behavior are deterministic.
- [ ] Suggested commit: phase4(8D): add config viewmodel tests

### 8E - Pipeline integration tests

- [ ] Action: Add integration tests for StateStore -> Snapshot -> ViewModel.
- [ ] Scope: Tests/UI/ViewModels/ViewModelPipelineIntegrationTests.cs
- [ ] Verify: End-to-end projection behavior without running game runtime.
- [ ] Suggested commit: phase4(8E): add snapshot pipeline integration tests

## Step 8 Completion

- [ ] All substeps in Step 8 complete (8A, 8B, 8C, 8D, 8E).

## 9) Documentation And Closeout

Step goal:

- [ ] Keep planning and issue artifacts synchronized as implementation proceeds.

### 9A - Sync checklist outcomes to implementation issues

- [ ] Action: Update issue records with concrete progress notes and closure evidence.
- [ ] Scope: Project/Tasks/ImplementationPlan/ImplementationIssues/Records/issue-100.md, issue-106.md, issue-107.md, issue-108.md, issue-109.md, issue-159.md
- [ ] Verify: Each relevant issue has current status and implementation notes.
- [ ] Suggested commit: phase4(9A): sync phase4 checklist outcomes to issue records

### 9B - Sync broad plan if scope is trimmed

- [ ] Action: If command surface or step scope is trimmed during implementation, update the broad plan to match reality.
- [ ] Scope: .clavix/outputs/phase-4-implementation-plan/tasks.md
- [ ] Verify: Broad plan reflects actual accepted Phase 4 scope.
- [ ] Suggested commit: phase4(9B): reconcile broad plan with trimmed implementation scope

## Step 9 Completion

- [ ] All substeps in Step 9 complete (9A, 9B).

## Final Review Gate (Before Marking Phase 4 Complete)

- [ ] Guardrails remain true.
- [ ] ViewModel -> command path exists and is boundary-safe.
- [ ] Snapshot projection for HudViewModel and TaskListViewModel is deterministic.
- [ ] ConfigViewModel is present and boundary-safe.
- [ ] Phase 4 carryover issues have explicit resolution or explicit re-deferral notes.
- [ ] Unit tests for projection/INPC/dispatch are present and passing.
- [ ] No out-of-scope phase work was introduced.

---

Generated as a separate detailed checklist companion to the implementation plan in .clavix/outputs/phase-4-implementation-plan/tasks.md
