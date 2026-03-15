# Implementation Plan

**Project**: phase-4-implementation-plan
**Generated**: 2026-03-15T00:00:00Z

## Technical Context & Standards

Detected Stack & Patterns:

- **Architecture**: Command/snapshot state architecture with composition root in Startup and lifecycle forwarding via `LifecycleCoordinator`.
- **Framework**: C# .NET 6 SMAPI mod.
- **UI Layer**: C# ViewModels under `UI/ViewModels` with snapshot subscription through `UI/UISnapshotSubscriptionManager.cs`.
- **State**: `State/StateStore.cs` owns canonical state mutation through command dispatch and emits `SnapshotChanged` events.
- **INPC**: Currently `CommunityToolkit.Mvvm` usage exists; Phase 4 plan requires PropertyChanged.SourceGenerator-based INPC conventions.
- **Tests**: xUnit test suite under `Tests/**` with deterministic naming and boundary-focused assertions.
- **Conventions**: Preserve deterministic ordering/ID behavior, avoid direct state mutation outside `StateStore`, and keep ViewModels free of direct game API dependencies.

---

## Phase 1: Foundation & Contract Alignment

- [ ] **Lock ViewModel naming and namespace baseline in code** (ref: PRD Architecture & Design, carryover #106)
      Task ID: phase-1-foundation-01

  > **Implementation**: Edit `UI/ViewModels/UiViewModelBase.cs`, `UI/ViewModels/HudViewModel.cs`, `UI/ViewModels/TaskListViewModel.cs`, `UI/ViewModels/TaskItemViewModel.cs`.
  > **Details**: Rename base class to `ViewModelBase`, ensure namespace consistency with current codebase conventions (`JojaAutoTasks.Ui.ViewModels`), and apply locked naming rules from PRD (`<Feature>ViewModel`, boolean/collection naming). If namespace casing differs from PRD text, treat codebase convention as canonical and record the clarification in docs.

- [ ] **Introduce explicit ViewModel command dispatch abstraction** (ref: PRD Must-Have #5, carryover #109)
      Task ID: phase-1-foundation-02

  > **Implementation**: Create `UI/ViewModels/IViewModelCommandDispatcher.cs` and `UI/ViewModels/StateStoreCommandDispatcher.cs`.
  > **Details**: Define an interface that exposes only ViewModel-safe command entrypoints (`ExecuteCompleteTask`, `ExecuteUncompleteTask`, `ExecutePinTask`, `ExecuteUnpinTask`, `ExecuteRemoveTask`, and optional `ExecuteAddOrUpdateTask`). Implement adapter over `StateStore` command API. Do not expose raw `StateStore` mutation internals to ViewModels.

- [ ] **Introduce explicit snapshot subscription abstraction for ViewModels** (ref: PRD Must-Have #4, carryover #107)
      Task ID: phase-1-foundation-03

  > **Implementation**: Create `UI/ViewModels/ISnapshotSubscriptionSource.cs` and `UI/ViewModels/StateStoreSnapshotSubscriptionSource.cs`; edit `UI/UISnapshotSubscriptionManager.cs`.
  > **Details**: Keep existing subscription manager behavior as compatibility boundary, but route ViewModels through an interface abstraction so tests can drive snapshot updates without runtime coupling. Preserve deterministic callback ordering and disposal semantics.

- [ ] **Wire new ViewModel dependencies through composition root** (ref: PRD Technical Requirements)
      Task ID: phase-1-foundation-04
  > **Implementation**: Edit `Startup/BootstrapContainer.cs`, `Startup/ModRuntime.cs`, and `ModEntry.cs`.
  > **Details**: Register and expose snapshot subscription and command-dispatch adapters via runtime wiring. Ensure initialization order keeps state store creation before subscription source usage. Keep lifecycle forwarding and state ownership separation unchanged.

## Phase 2: ViewModel Implementations

- [ ] **Migrate base INPC strategy to PropertyChanged.SourceGenerator conventions** (ref: PRD Must-Have #1, carryover #108)
      Task ID: phase-2-viewmodels-01

  > **Implementation**: Edit `UI/ViewModels/ViewModelBase.cs` (post-rename from `UiViewModelBase.cs`) and existing ViewModel files.
  > **Details**: Standardize property-change implementation approach to match PropertyChanged.SourceGenerator usage requirements. Remove or isolate conflicting toolkit-specific assumptions from Phase 4 ViewModels. Preserve external behavior and property names.

- [ ] **Refactor HudViewModel to explicit ApplySnapshot projection entrypoint** (ref: PRD Must-Have #2/#4)
      Task ID: phase-2-viewmodels-02

  > **Implementation**: Edit `UI/ViewModels/HudViewModel.cs`.
  > **Details**: Add `ApplySnapshot(TaskSnapshot snapshot)` as the canonical projection method, called from subscription callback. Keep task count computations deterministic and derived only from snapshot data. Ensure disposal and subscription lifecycle are preserved.

- [ ] **Implement TaskListViewModel snapshot projection and task item projection mapping** (ref: PRD Must-Have #2/#4)
      Task ID: phase-2-viewmodels-03

  > **Implementation**: Edit `UI/ViewModels/TaskListViewModel.cs` and `UI/ViewModels/TaskItemViewModel.cs`.
  > **Details**: Replace no-op projection with deterministic mapping from `TaskSnapshot.TaskViews` into bindable task item view models. Add/update `ApplySnapshot(...)` on list and item layers. Preserve stable ordering based on snapshot order and avoid hidden mutable state not represented in snapshot.

- [ ] **Add command-dispatch methods on TaskListViewModel for task actions** (ref: PRD Must-Have #5)
      Task ID: phase-2-viewmodels-04

  > **Implementation**: Edit `UI/ViewModels/TaskListViewModel.cs`.
  > **Details**: Add explicit methods named with `Execute<Verb><Target>(...)` that forward through `IViewModelCommandDispatcher` for complete/uncomplete/pin/unpin/remove flows. Ensure methods operate on `TaskId` only and do not mutate state directly.

- [ ] **Add ConfigViewModel and include it in Phase 4 ViewModel surface** (ref: PRD Must-Have #3)
      Task ID: phase-2-viewmodels-05
  > **Implementation**: Create `UI/ViewModels/ConfigViewModel.cs`; edit `Startup/ModRuntime.cs` and/or applicable UI wiring entrypoints.
  > **Details**: Expose bindable config-facing properties consistent with `Configuration/ModConfig.cs` and define update methods with explicit naming conventions. Keep config-facing logic isolated from gameplay APIs. If config updates are not state-store commands, document and enforce the boundary explicitly in this class.

## Phase 3: Carryover Issue Integration

- [ ] **Integrate localization-ready projection behavior in ViewModels** (ref: carryover #100)
      Task ID: phase-3-carryover-01

  > **Implementation**: Create `UI/ViewModels/ILocalizationProvider.cs` and `UI/ViewModels/DefaultLocalizationProvider.cs`; edit affected ViewModels where user-visible text is derived.
  > **Details**: Ensure ViewModel-facing user text paths are localization-ready and do not embed hardcoded runtime-only assumptions. Keep localization dependency abstracted for tests.

- [ ] **Finalize terminology clarification artifacts for Phase 4** (ref: carryover #106)
      Task ID: phase-3-carryover-02

  > **Implementation**: Edit `Project/Tasks/ImplementationPlan/Phase 4 - Atomic Commit Execution Checklist.md` and relevant ImplementationIssues records.
  > **Details**: Record any final naming/namespace decisions made during implementation so future phases inherit unambiguous terms.

- [ ] **Harden snapshot subscription lifecycle boundaries for UI-local ownership** (ref: carryover #107/#109)
      Task ID: phase-3-carryover-03

  > **Implementation**: Edit `UI/UISnapshotSubscriptionManager.cs`, `UI/ViewModels/HudViewModel.cs`, `UI/ViewModels/TaskListViewModel.cs`.
  > **Details**: Ensure ownership/disposal of subscriptions is explicit, leak-free, and testable; verify no UI-local hidden state bypasses snapshot projection boundaries.

- [ ] **Reconfirm config loader exception hardening regression behavior** (ref: carryover #159, duplicate #86)
      Task ID: phase-3-carryover-04
  > **Implementation**: Edit `Configuration/ConfigLoader.cs` only if needed; prioritize tests in `Tests/Configuration/ConfigLoaderMigrationSafetyTests.cs` and `Tests/Configuration/ConfigLoaderTests.cs`.
  > **Details**: Verify existing fallback/error-path behavior remains intact while Phase 4 ViewModel/config changes are introduced. Avoid scope creep beyond regression hardening.

## Phase 4: Unit Tests for ViewModel Pipeline

- [ ] **Create ViewModel test folder and baseline fixtures** (ref: PRD testability requirement)
      Task ID: phase-4-testing-01

  > **Implementation**: Create `Tests/UI/ViewModels/` and add `Tests/UI/ViewModels/ViewModelTestDoubles.cs`.
  > **Details**: Add deterministic test doubles for `ISnapshotSubscriptionSource`, `IViewModelCommandDispatcher`, and localization/config dependencies. Keep fixtures runtime-independent.

- [ ] **Add HudViewModel projection and INPC tests** (ref: carryover #108)
      Task ID: phase-4-testing-02

  > **Implementation**: Create `Tests/UI/ViewModels/HudViewModelTests.cs`.
  > **Details**: Verify `ApplySnapshot(...)` updates counts correctly, raises expected property notifications, and handles repeated snapshots deterministically.

- [ ] **Add TaskListViewModel projection and command-forwarding tests** (ref: PRD Must-Have #2/#5)
      Task ID: phase-4-testing-03

  > **Implementation**: Create `Tests/UI/ViewModels/TaskListViewModelTests.cs`.
  > **Details**: Validate snapshot-to-item mapping, stable ordering, INPC notifications, and `Execute<Verb><Target>(...)` forwarding behavior to dispatcher abstraction.

- [ ] **Add ConfigViewModel tests for bindable projection and boundary behavior** (ref: PRD Must-Have #3)
      Task ID: phase-4-testing-04

  > **Implementation**: Create `Tests/UI/ViewModels/ConfigViewModelTests.cs`.
  > **Details**: Validate config projection, INPC behavior, and boundary rules (no direct game API, no hidden state mutations).

- [ ] **Add pipeline integration test: StateStore -> Snapshot -> ViewModel projection** (ref: Phase 4 validation goal)
      Task ID: phase-4-testing-05
  > **Implementation**: Create `Tests/UI/ViewModels/ViewModelPipelineIntegrationTests.cs`.
  > **Details**: Drive `StateStore` command changes, emit snapshots through test subscription source, and assert ViewModels reflect expected bindable state without running game runtime.

## Phase 5: Verification & Documentation Sync

- [ ] **Run focused build/test verification for Phase 4 changes** (ref: review and verification contract)
      Task ID: phase-5-verification-01

  > **Implementation**: Execute repository validation commands and focused test filters.
  > **Details**: Run debug build check and targeted ViewModel/config tests first, then full test project run if focused suite passes. Record failures and required follow-up tasks without broadening implementation scope.

- [ ] **Update Phase 4 implementation artifacts and issue references** (ref: update-docs-on-code-change contract)
      Task ID: phase-5-verification-02
  > **Implementation**: Edit `Project/Tasks/ImplementationPlan/Phase 4 - Atomic Commit Execution Checklist.md` and relevant files in `Project/Tasks/ImplementationPlan/ImplementationIssues/`.
  > **Details**: Reflect completed technical decisions, test coverage state, and explicit resolution/progress notes for #100/#106/#107/#108/#109/#159.

---

Generated by Clavix /clavix-plan
