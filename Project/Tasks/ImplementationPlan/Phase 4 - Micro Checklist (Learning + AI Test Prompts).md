# Phase 4 - Micro Checklist (Learning + AI Test Prompts)

Purpose: This companion artifact converts the detailed Phase 4 checklist into smaller execution slices and gives ready-to-use prompts for AI-generated unit tests only. You write production code; AI writes test code.

## How To Use This File

- [ ] Pick one micro-step only.
- [ ] Implement production code for that step yourself.
- [ ] Run build.
- [ ] Use the matching AI test prompt from this file.
- [ ] Review generated tests before accepting.
- [ ] Mark step done and move to next.

## Rules For This Learning Workflow

- [ ] Production code is human-authored.
- [ ] Unit tests can be AI-authored.
- [ ] No direct game API calls in ViewModels.
- [ ] Canonical state changes must go through command dispatch boundaries.
- [ ] Keep each micro-step small enough for one focused coding session.

## Micro-Step Sequence (Code-First)

## 1) Naming And Baseline

### 1.1 Rename base class

- [ ] Rename UiViewModelBase to ViewModelBase.
- [ ] Update all references.
- [ ] Build succeeds.

### 1.2 Naming normalization pass

- [ ] Ensure ApplySnapshot naming is used where projection happens.
- [ ] Ensure command methods follow ExecuteVerbTarget naming.
- [ ] Ensure bool properties use Is/Has/Can.

## 2) Snapshot Abstraction

### 2.1 Add subscription interface

- [ ] Create ISnapshotSubscriptionSource.
- [ ] Keep contract minimal and test-friendly.

### 2.2 Add StateStore-backed adapter

- [ ] Create StateStoreSnapshotSubscriptionSource.
- [ ] Ensure deterministic subscribe/dispose behavior.

## 3) Command Abstraction

### 3.1 Add command dispatcher interface

- [ ] Create IViewModelCommandDispatcher with comprehensive initial surface.
- [ ] Include manual add/update/delete and complete/uncomplete.
- [ ] Include task-builder add/update/delete intent methods.

### 3.2 Add StateStore-backed dispatcher

- [ ] Create StateStoreCommandDispatcher.
- [ ] Verify each dispatcher method routes through command boundary.

## 4) Runtime Wiring

### 4.1 Composition root wiring

- [ ] Wire new adapters in BootstrapContainer.
- [ ] Expose adapter dependencies through ModRuntime.

### 4.2 Entry lifecycle safety

- [ ] Confirm runtime initialization order remains safe in ModEntry.
- [ ] Confirm no adapter usage before StateStore exists.

## 5) HudViewModel Projection

### 5.1 Projection method shape

- [ ] Add or finalize ApplySnapshot on HudViewModel.
- [ ] Keep callback thin and route to ApplySnapshot.

### 5.2 INPC behavior

- [ ] Ensure bindable property updates raise expected notifications.
- [ ] Keep computation deterministic from snapshot only.

## 6) TaskListViewModel Projection

### 6.1 Add bindable task collection

- [ ] Add observable/bindable collection property.
- [ ] Initialize safely.

### 6.2 Add list projection

- [ ] Implement ApplySnapshot projection from snapshot task views.
- [ ] Keep deterministic ordering behavior.

### 6.3 Add action forwarding methods

- [ ] Add Execute methods that call dispatcher.
- [ ] Do not call StateStore directly from ViewModel.

### 6.4 Add future-trim marker

- [ ] Add a clear comment noting command surface can be trimmed later.

## 7) ConfigViewModel

### 7.1 Add projection model

- [ ] Create ConfigViewModel from ModConfig.
- [ ] Keep properties bindable and boundary-safe.

### 7.2 Add update intent boundary

- [ ] Add explicit update intent method.
- [ ] Keep persistence/schema behavior out of scope.

## 8) Carryover Issues (Focused Pass)

### 8.1 Issue 106 terminology

- [ ] Clarify TaskSourceType vs SourceIdentifier usage in touched code.

### 8.2 Issue 100 localization readiness

- [ ] Add localization-ready abstraction points for ViewModel-facing text.

### 8.3 Issue 108 INPC evidence

- [ ] Confirm INPC behavior in projection paths.

### 8.4 Issue 109 UI-local state

- [ ] Keep selection/filter/scroll state local, not canonical.

### 8.5 Issue 159 regression check

- [ ] Reconfirm ConfigLoader exception-hardening invariants remain intact.

### 8.6 Issue 107 resolved verification

- [ ] Verify snapshot subscription behavior remains non-regressed.

## AI Test Prompt Pack (Copy/Paste)

Use one prompt at a time after completing the matching production-code micro-step.

## Prompt A - HudViewModel projection tests

Goal: Generate xUnit tests for HudViewModel projection and notifications.

Constraints:

- Test file path: Tests/UI/ViewModels/HudViewModelTests.cs
- Do not modify production code.
- Use deterministic snapshots only.
- Verify ApplySnapshot updates counts correctly.
- Verify property-change notification behavior.
- No game runtime dependency.
- Follow existing test style in this repository.

Deliverables:

- Full test class code only.
- Include any minimal test doubles needed inside test file or shared ViewModelTestDoubles file.

## Prompt B - TaskListViewModel projection tests

Goal: Generate xUnit tests for TaskListViewModel snapshot projection.

Constraints:

- Test file path: Tests/UI/ViewModels/TaskListViewModelTests.cs
- Do not modify production code.
- Use deterministic TaskSnapshot inputs.
- Verify deterministic ordering from snapshot to collection.
- Verify projection updates across multiple snapshots.
- Verify command forwarding methods call dispatcher abstraction.
- No game runtime dependency.

Deliverables:

- Full test class code only.
- Include clear Arrange/Act/Assert structure.

## Prompt C - ConfigViewModel tests

Goal: Generate xUnit tests for ConfigViewModel bindable projection and update-intent boundary behavior.

Constraints:

- Test file path: Tests/UI/ViewModels/ConfigViewModelTests.cs
- Do not modify production code.
- Verify config values project correctly from ModConfig.
- Verify property notifications for changed values.
- Verify update intent method behavior through abstraction boundary.
- No persistence schema or migration assertions beyond Phase 4 scope.

Deliverables:

- Full test class code only.

## Prompt D - Pipeline integration tests

Goal: Generate xUnit integration-style tests for StateStore to Snapshot to ViewModel projection flow.

Constraints:

- Test file path: Tests/UI/ViewModels/ViewModelPipelineIntegrationTests.cs
- Do not modify production code.
- Use StateStore command flow to produce snapshot changes.
- Feed snapshots through testable subscription abstraction.
- Verify ViewModel state matches expected projection outputs.
- No game runtime dependency.

Deliverables:

- Full test class code only.
- Keep tests deterministic and stable.

## Prompt E - Shared test doubles

Goal: Generate reusable test doubles for ViewModel tests.

Constraints:

- Test file path: Tests/UI/ViewModels/ViewModelTestDoubles.cs
- Do not modify production code.
- Provide deterministic fake implementations for:
  - ISnapshotSubscriptionSource
  - IViewModelCommandDispatcher
  - Localization/config helper abstractions if used
- Keep API minimal and explicit.

Deliverables:

- Full helper file code only.

## Quick Done Criteria Per Micro-Step

- [ ] Production code compiles.
- [ ] Tests for that step compile and pass.
- [ ] No architecture guardrail violated.
- [ ] No out-of-scope phase work added.

## Final Phase-4 Learning Exit Criteria

- [ ] HudViewModel, TaskListViewModel, and ConfigViewModel projection paths are implemented.
- [ ] Command dispatch boundary from ViewModels is implemented and explicit.
- [ ] Snapshot subscription lifecycle is deterministic and non-regressed.
- [ ] Carryover issue work has explicit resolution or re-deferral notes.
- [ ] ViewModel tests exist and pass without game runtime.

---

Companion to:

- Project/Tasks/ImplementationPlan/Phase 4 - Atomic Commit Execution Checklist (Learning Detailed).md
- .clavix/outputs/phase-4-implementation-plan/tasks.md
