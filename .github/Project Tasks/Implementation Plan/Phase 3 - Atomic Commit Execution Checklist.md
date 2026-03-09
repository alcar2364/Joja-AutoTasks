# Phase 3 - Atomic Commit Execution Checklist #

Purpose: execute Phase 3 (State Store) in strict, linear, atomic commits with explicit file/symbol scope.

Note: If a sub-step expands beyond the stated file scope, stop and open a follow-up commit.

## Guardrails (Must Stay True) ##

    * [ ] Phase 3 only — No Phase 4 (ViewModels), Phase 5 (Generators/Engine), Phase 6 (Rules), Phase 7 (Persistence), or Phase 8 (UI) work.
    * [ ] All mutations via commands only — No direct state dictionary mutation outside command handlers.
    * [ ] Command handlers must be deterministic — Same input + same state = same result, always.
    * [ ] Command handlers must be side-effect free during command handling — No logging, game state queries, or persistence writes in handlers.
    * [ ] Snapshots are read-only — UI cannot mutate canonical state via snapshots.
    * [ ] Engine/user field separation enforced — Engine updates preserve user pins; user updates preserve engine progress.
    * [ ] No "Reducer" suffix — Banned by C# style contract; use CommandHandler, StateController, TransitionApplier, or StateMutator.
    * [ ] Preserve all Phase 1 and Phase 2 invariants — signal-only OnSaving, throttled OnUpdateTicked, deterministic identifiers, bounded logging.
    * [ ] Manual task counter ownership begins in Phase 3 — Phase 2 deferred this; Phase 3 owns manual ID issuance.
    * [ ] No UI/ViewModel work — Snapshot subscription belongs to Phase 4.
    * [ ] No engine/evaluator/generator work — Task generation belongs to Phase 5+.
    * [ ] No persistence save/load — Persistence wiring belongs to Phase 7.
    * [ ] TaskId format remains canonical — Underscore-delimited forms (`BuiltIn_*`, `TaskBuilder_*`, `ManualTask_{N}`).
    * [ ] DayKey format remains canonical — `Year{N}-{Season}{D}` style (e.g., `Year1-Summer15`).
    * [ ] No batch transactions or undo history — Explicitly out of scope for V1.
    * [ ] No dismissed task tracking — Deferred to V2.
    * [ ] Dictionary lookups only — No per-frame scans or heavy reconciliation in Phase 3.

## 1) Command Model Infrastructure ##

Step goal:

    * [ ] Define command types representing all state mutation intents.

### 1A - Add base command infrastructure ###

    * [x] Action: add `IStateCommand` interface or `StateCommandBase` abstract class defining command contract.
    * [x] Scope: `StateStore/Commands/IStateCommand.cs` or `StateStore/Commands/StateCommandBase.cs`.
    * [x] Verify: interface/base compiles and establishes command contract pattern; uses most restrictive access level (prefer internal).
    * [x] Commit message: `phase3(step1A): add base command infrastructure`
    * [x] Must include: command contract definition only.
    * [x] Must exclude: concrete command implementations, handler logic.

### 1B - Add AddOrUpdateTaskCommand ###

    * [x] Action: add command representing task creation or engine/user update intent.
    * [x] Scope: `StateStore/Commands/AddOrUpdateTaskCommand.cs`.
    * [x] Verify: command type compiles with required fields (TaskId, task data, source type); constructor guards prevent invalid construction.
    * [x] Commit message: `phase3(step1B): add AddOrUpdateTaskCommand`
    * [x] Must include: command type with required fields and constructor guards only.
    * [x] Must exclude: handler implementation, validation logic beyond constructor guards.

### 1C - Add CompleteTaskCommand ###

    * [x] Action: add command representing task completion intent.
    * [x] Scope: `StateStore/Commands/CompleteTaskCommand.cs`.
    * [x] Verify: command type compiles with TaskId and completion day fields.
    * [x] Commit message: `phase3(step1C): add CompleteTaskCommand`
    * [x] Must include: command type only.
    * [x] Must exclude: handler implementation.

### 1D - Add UncompleteTaskCommand ###

    * [x] Action: add command representing task un-completion intent.
    * [x] Scope: `StateStore/Commands/UncompleteTaskCommand.cs`.
    * [x] Verify: command type compiles with TaskId field.
    * [x] Commit message: `phase3(step1D): add UncompleteTaskCommand`
    * [x] Must include: command type only.
    * [x] Must exclude: handler implementation.

### 1E - Add RemoveTaskCommand ###

    * [ ] Action: add command representing task removal intent.
    * [ ] Scope: `StateStore/Commands/RemoveTaskCommand.cs`.
    * [ ] Verify: command type compiles with TaskId field.
    * [ ] Commit message: `phase3(step1E): add RemoveTaskCommand`
    * [ ] Must include: command type only.
    * [ ] Must exclude: handler implementation, expiration logic.

### 1F - Add PinTaskCommand ###

    * [ ] Action: add command representing user pin intent.
    * [ ] Scope: `StateStore/Commands/PinTaskCommand.cs`.
    * [ ] Verify: command type compiles with TaskId field.
    * [ ] Commit message: `phase3(step1F): add PinTaskCommand`
    * [ ] Must include: command type only.
    * [ ] Must exclude: handler implementation.

### 1G - Add UnpinTaskCommand ###

    * [ ] Action: add command representing user unpin intent.
    * [ ] Scope: `StateStore/Commands/UnpinTaskCommand.cs`.
    * [ ] Verify: command type compiles with TaskId field.
    * [ ] Commit message: `phase3(step1G): add UnpinTaskCommand`
    * [ ] Must include: command type only.
    * [ ] Must exclude: handler implementation.

## Step 1 Completion ##

    * [ ] All substeps in Step 1 complete (1A, 1B, 1C, 1D, 1E, 1F, 1G).

## 2) State Container and Task Records ##

Step goal:

    * [ ] Create internal task storage structure distinct from domain model.

### 2A - Add TaskRecord internal storage structure ###

    * [ ] Action: create internal `TaskRecord` structure distinct from `TaskObject` for state storage.
    * [ ] Scope: `StateStore/Models/TaskRecord.cs`.
    * [ ] Verify: TaskRecord compiles with fields for TaskId, status, progress, metadata, user flags (IsPinned), creation day, completion day; uses most restrictive access level (internal).
    * [ ] Commit message: `phase3(step2A): add TaskRecord internal storage structure`
    * [ ] Must include: internal record type only.
    * [ ] Must exclude: conversion to/from TaskObject, handler logic.

### 2B - Add field separation model ###

    * [ ] Action: define which fields are engine-controlled vs user-controlled in TaskRecord.
    * [ ] Scope: `StateStore/Models/TaskRecord.cs` (comments/documentation) or separate `FieldOwnership.cs` helper.
    * [ ] Verify: clear documentation of field ownership boundaries exists.
    * [ ] Commit message: `phase3(step2B): document engine vs user field ownership`
    * [ ] Must include: field ownership documentation or helper types only.
    * [ ] Must exclude: handler implementation.

### 2C - Add state dictionary container with version tracking ###

    * [ ] Action: add internal `Dictionary<TaskId, TaskRecord>` and version counter for snapshot invalidation.
    * [ ] Scope: `StateStore/StateContainer.cs` or embedded in `StateStore.cs`.
    * [ ] Verify: container compiles with dictionary and version increment logic; uses most restrictive access level.
    * [ ] Commit message: `phase3(step2C): add state dictionary container with version tracking`
    * [ ] Must include: dictionary, version counter, basic accessor patterns only.
    * [ ] Must exclude: command processing logic, snapshot generation.

## Step 2 Completion ##

    * [ ] All substeps in Step 2 complete (2A, 2B, 2C).

## 3) Command Handler Implementation ##

Step goal:

    * [ ] Implement deterministic state transformation logic for each command.

### 3A - Add command handler infrastructure ###

    * [ ] Action: add `ICommandHandler<TCommand>` interface or `CommandHandlerBase<TCommand>` abstract class.
    * [ ] Scope: `StateStore/Handlers/ICommandHandler.cs` or `StateStore/Handlers/CommandHandlerBase.cs`.
    * [ ] Verify: handler contract compiles and establishes deterministic transformation pattern; uses most restrictive access level.
    * [ ] Commit message: `phase3(step3A): add command handler infrastructure`
    * [ ] Must include: handler contract definition only.
    * [ ] Must exclude: concrete handler implementations.

### 3B - Implement AddOrUpdateTaskCommandHandler with field separation ###

    * [ ] Action: implement handler for AddOrUpdateTaskCommand with engine/user field separation logic.
    * [ ] Scope: `StateStore/Handlers/AddOrUpdateTaskCommandHandler.cs`.
    * [ ] Verify: if task does not exist → create new TaskRecord; if task exists and command is engine-sourced → update engine fields only, preserve user fields; if task exists and command is user-sourced → update user fields only, preserve engine fields; handler is deterministic and side-effect free.
    * [ ] Commit message: `phase3(step3B): implement AddOrUpdateTaskCommandHandler with field separation`
    * [ ] Must include: complete handler implementation with field separation logic.
    * [ ] Must exclude: snapshot publishing, persistence.

### 3C - Implement CompleteTaskCommandHandler ###

    * [ ] Action: implement handler for CompleteTaskCommand.
    * [ ] Scope: `StateStore/Handlers/CompleteTaskCommandHandler.cs`.
    * [ ] Verify: handler sets TaskStatus to Completed and records completion day; handler is deterministic and side-effect free.
    * [ ] Commit message: `phase3(step3C): implement CompleteTaskCommandHandler`
    * [ ] Must include: handler implementation only.
    * [ ] Must exclude: snapshot publishing, persistence, UI feedback.

### 3D - Implement UncompleteTaskCommandHandler ###

    * [ ] Action: implement handler for UncompleteTaskCommand.
    * [ ] Scope: `StateStore/Handlers/UncompleteTaskCommandHandler.cs`.
    * [ ] Verify: handler sets TaskStatus to Incomplete and clears completion day; handler is deterministic and side-effect free.
    * [ ] Commit message: `phase3(step3D): implement UncompleteTaskCommandHandler`
    * [ ] Must include: handler implementation only.
    * [ ] Must exclude: snapshot publishing, UI feedback.

### 3E - Implement RemoveTaskCommandHandler ###

    * [ ] Action: implement handler for RemoveTaskCommand.
    * [ ] Scope: `StateStore/Handlers/RemoveTaskCommandHandler.cs`.
    * [ ] Verify: handler removes TaskRecord from dictionary; handler is deterministic and side-effect free.
    * [ ] Commit message: `phase3(step3E): implement RemoveTaskCommandHandler`
    * [ ] Must include: handler implementation only.
    * [ ] Must exclude: expiration logic, day-boundary behavior.

### 3F - Implement PinTaskCommandHandler ###

    * [ ] Action: implement handler for PinTaskCommand.
    * [ ] Scope: `StateStore/Handlers/PinTaskCommandHandler.cs`.
    * [ ] Verify: handler sets IsPinned flag to true; handler is deterministic and side-effect free.
    * [ ] Commit message: `phase3(step3F): implement PinTaskCommandHandler`
    * [ ] Must include: handler implementation only.
    * [ ] Must exclude: snapshot publishing, UI feedback.

### 3G - Implement UnpinTaskCommandHandler ###

    * [ ] Action: implement handler for UnpinTaskCommand.
    * [ ] Scope: `StateStore/Handlers/UnpinTaskCommandHandler.cs`.
    * [ ] Verify: handler sets IsPinned flag to false; handler is deterministic and side-effect free.
    * [ ] Commit message: `phase3(step3G): implement UnpinTaskCommandHandler`
    * [ ] Must include: handler implementation only.
    * [ ] Must exclude: snapshot publishing, UI feedback.

## Step 3 Completion ##

    * [ ] All substeps in Step 3 complete (3A, 3B, 3C, 3D, 3E, 3F, 3G).

## 4) Snapshot Model ##

Step goal:

    * [ ] Create read-only projections for external consumption.

### 4A - Add TaskView read-only projection ###

    * [ ] Action: add `TaskView` read-only record/class mirroring TaskObject fields needed for UI.
    * [ ] Scope: `StateStore/Models/TaskView.cs`.
    * [ ] Verify: TaskView compiles with immutable fields, exposes no mutators; uses most restrictive access level (prefer internal or public readonly).
    * [ ] Commit message: `phase3(step4A): add TaskView read-only projection`
    * [ ] Must include: read-only view type only.
    * [ ] Must exclude: snapshot container, generation logic.

### 4B - Add TaskSnapshot immutable collection wrapper ###

    * [ ] Action: add `TaskSnapshot` containing `IReadOnlyList<TaskView>` and version number.
    * [ ] Scope: `StateStore/Models/TaskSnapshot.cs`.
    * [ ] Verify: TaskSnapshot compiles with immutable collection; exposes no mutators.
    * [ ] Commit message: `phase3(step4B): add TaskSnapshot immutable collection wrapper`
    * [ ] Must include: snapshot container type only.
    * [ ] Must exclude: generation logic.

### 4C - Add snapshot generation from state dictionary ###

    * [ ] Action: add method to project `Dictionary<TaskId, TaskRecord>` to `TaskSnapshot`.
    * [ ] Scope: `StateStore/StateStore.cs` or separate `SnapshotProjector.cs`.
    * [ ] Verify: method produces defensive copy with stable ordering (by TaskId or creation day); no shared references with canonical state.
    * [ ] Commit message: `phase3(step4C): add snapshot generation from state dictionary`
    * [ ] Must include: projection logic with defensive copy only.
    * [ ] Must exclude: event publishing, subscription handling.

## Step 4 Completion ##

    * [ ] All substeps in Step 4 complete (4A, 4B, 4C).

## 5) State Store Public API ##

Step goal:

    * [ ] Assemble the complete State Store with command dispatch and snapshot publishing.

### 5A - Add StateStore class shell with constructor injection ###

    * [ ] Action: add `StateStore` class with dependencies declared via constructor.
    * [ ] Scope: `StateStore/StateStore.cs`.
    * [ ] Verify: class compiles with constructor accepting ICommandHandler dependencies and private state container field; uses most restrictive access level for internal members.
    * [ ] Commit message: `phase3(step5A): add StateStore class shell with constructor injection`
    * [ ] Must include: class shell, constructor, field declarations only.
    * [ ] Must exclude: command processing pipeline, event wiring.

### 5B - Wire command routing to handlers ###

    * [ ] Action: add internal command dispatch logic routing commands to appropriate handlers.
    * [ ] Scope: `StateStore/StateStore.cs`.
    * [ ] Verify: commands route correctly to handlers, state updates applied; routing is deterministic.
    * [ ] Commit message: `phase3(step5B): wire command routing to handlers`
    * [ ] Must include: command dispatch logic only.
    * [ ] Must exclude: snapshot publishing, external API methods.

### 5C - Wire snapshot generation and SnapshotChanged event ###

    * [ ] Action: add `public event Action<TaskSnapshot>? SnapshotChanged;` and wire snapshot generation after state changes.
    * [ ] Scope: `StateStore/StateStore.cs`.
    * [ ] Verify: event fires after successful command processing with current snapshot; event declaration is public.
    * [ ] Commit message: `phase3(step5C): wire snapshot generation and SnapshotChanged event`
    * [ ] Must include: event declaration and invocation logic only.
    * [ ] Must exclude: subscription handling (Phase 4 responsibility).

### 5D - Add public command dispatch methods ###

    * [ ] Action: add public methods for dispatching commands (e.g., `Dispatch(IStateCommand command)` or individual methods per command type).
    * [ ] Scope: `StateStore/StateStore.cs`.
    * [ ] Verify: public API surface is minimal and explicit; methods are public, implementation details are private/internal.
    * [ ] Commit message: `phase3(step5D): add public command dispatch methods`
    * [ ] Must include: public dispatch methods only.
    * [ ] Must exclude: internal implementation changes.

## Step 5 Completion ##

    * [ ] All substeps in Step 5 complete (5A, 5B, 5C, 5D).

## 6) Day Boundary Behavior ##

Step goal:

    * [ ] Implement day-keyed task expiration logic.

### 6A - Add expired task detection ###

    * [ ] Action: add logic to identify day-keyed tasks (TaskId contains day component) that are expired relative to current day.
    * [ ] Scope: `StateStore/DayBoundary/ExpirationDetector.cs` or embedded in `StateStore.cs`.
    * [ ] Verify: logic correctly identifies daily tasks past their expiration day; detection is deterministic.
    * [ ] Commit message: `phase3(step6A): add expired task detection logic`
    * [ ] Must include: expiration detection logic only.
    * [ ] Must exclude: removal execution.

### 6B - Add day-transition cleanup handler ###

    * [ ] Action: add handler or method to remove expired tasks on day start.
    * [ ] Scope: `StateStore/DayBoundary/DayTransitionHandler.cs` or `StateStore.cs`.
    * [ ] Verify: expired tasks removed from state, snapshot regenerated; handler is deterministic and side-effect free.
    * [ ] Commit message: `phase3(step6B): add day-transition cleanup handler`
    * [ ] Must include: cleanup handler only.
    * [ ] Must exclude: lifecycle wiring.

### 6C - Wire day-transition trigger into State Store ###

    * [ ] Action: add public method for day-start event (e.g., `OnDayStarted(DayKey newDay)`).
    * [ ] Scope: `StateStore/StateStore.cs`.
    * [ ] Verify: method triggers expiration cleanup and snapshot rebuild; method is public.
    * [ ] Commit message: `phase3(step6C): wire day-transition trigger into State Store`
    * [ ] Must include: public day-transition method only.
    * [ ] Must exclude: lifecycle coordinator wiring (Step 8).

## Step 6 Completion ##

    * [ ] All substeps in Step 6 complete (6A, 6B, 6C).

## 7) Manual Task ID Issuance ##

Step goal:

    * [ ] Implement manual task counter ownership (deferred from Phase 2).

### 7A - Add internal manual task counter state ###

    * [ ] Action: add private counter field for tracking next manual task ID.
    * [ ] Scope: `StateStore/StateStore.cs` or `StateStore/Models/ManualTaskCounter.cs`.
    * [ ] Verify: counter field compiles and initializes to deterministic start value (e.g., 1); uses most restrictive access level (private).
    * [ ] Commit message: `phase3(step7A): add internal manual task counter state`
    * [ ] Must include: counter field only.
    * [ ] Must exclude: increment logic, persistence.

### 7B - Add IssueNextManualTaskId method ###

    * [ ] Action: add method to generate next manual TaskId using counter.
    * [ ] Scope: `StateStore/StateStore.cs`.
    * [ ] Verify: method produces TaskId in canonical `ManualTask_{N}` format; method is deterministic.
    * [ ] Commit message: `phase3(step7B): add IssueNextManualTaskId method`
    * [ ] Must include: ID issuance method only.
    * [ ] Must exclude: counter persistence.

### 7C - Wire manual ID issuance into AddOrUpdateTaskCommand flow ###

    * [ ] Action: add logic to call IssueNextManualTaskId when command creates manual task without pre-assigned ID.
    * [ ] Scope: `StateStore/Handlers/AddOrUpdateTaskCommandHandler.cs` or `StateStore.cs`.
    * [ ] Verify: manual tasks receive unique sequential IDs; integration is deterministic.
    * [ ] Commit message: `phase3(step7C): wire manual ID issuance into AddOrUpdateTaskCommand`
    * [ ] Must include: ID issuance integration only.
    * [ ] Must exclude: counter persistence.

### 7D - Add counter increment logic with deterministic sequencing ###

    * [ ] Action: ensure counter increments deterministically (no race conditions, stable ordering).
    * [ ] Scope: `StateStore/StateStore.cs`.
    * [ ] Verify: sequential calls produce sequential IDs; no threading issues introduced.
    * [ ] Commit message: `phase3(step7D): add deterministic counter increment logic`
    * [ ] Must include: increment logic only.
    * [ ] Must exclude: persistence (Phase 7).

## Step 7 Completion ##

    * [ ] All substeps in Step 7 complete (7A, 7B, 7C, 7D).

## 8) Integration with Lifecycle ##

Step goal:

    * [ ] Connect State Store to lifecycle coordinator.

### 8A - Add State Store to bootstrap composition container ###

    * [ ] Action: wire State Store into dependency injection container.
    * [ ] Scope: `Startup/BootstrapContainer.cs`.
    * [ ] Verify: State Store registered as singleton and resolved correctly; registration compiles.
    * [ ] Commit message: `phase3(step8A): wire State Store into bootstrap composition`
    * [ ] Must include: DI registration only.
    * [ ] Must exclude: lifecycle wiring.

### 8B - Wire State Store initialization into lifecycle coordinator ###

    * [ ] Action: add State Store initialization call in lifecycle coordinator's startup flow.
    * [ ] Scope: `Lifecycle/LifecycleCoordinator.cs`.
    * [ ] Verify: State Store initialized on game launch or save load; initialization is deterministic.
    * [ ] Commit message: `phase3(step8B): wire State Store initialization into lifecycle`
    * [ ] Must include: initialization hookup only.
    * [ ] Must exclude: teardown logic.

### 8C - Wire State Store teardown on return-to-title ###

    * [ ] Action: add State Store cleanup/disposal call in lifecycle coordinator's teardown flow.
    * [ ] Scope: `Lifecycle/LifecycleCoordinator.cs`.
    * [ ] Verify: State Store cleared when returning to title screen; teardown is safe and deterministic.
    * [ ] Commit message: `phase3(step8C): wire State Store teardown on return-to-title`
    * [ ] Must include: teardown/disposal hookup only.
    * [ ] Must exclude: persistence save logic (Phase 7).

## Step 8 Completion ##

    * [ ] All substeps in Step 8 complete (8A, 8B, 8C).

## 9) Phase 3 Verification Tests ##

Step goal:

    * [ ] Lock State Store invariants with comprehensive test coverage.

### 9A - Add command validation tests ###

    * [ ] Action: add tests for command construction, required fields, and invariants.
    * [ ] Scope: `Tests/StateStore/Commands/CommandValidationTests.cs`.
    * [ ] Verify: tests fail if commands allow invalid construction (null TaskId, negative progress, etc.); all command types covered.
    * [ ] Commit message: `phase3(step9A): add command validation tests`
    * [ ] Must include: command construction guard tests only.
    * [ ] Must exclude: handler tests.

### 9B - Add command handler determinism tests ###

    * [ ] Action: add tests verifying same command + same state = same result.
    * [ ] Scope: `Tests/StateStore/Handlers/CommandHandlerDeterminismTests.cs`.
    * [ ] Verify: repeated handler invocations produce identical state; all handlers covered.
    * [ ] Commit message: `phase3(step9B): add command handler determinism tests`
    * [ ] Must include: determinism assertions only.
    * [ ] Must exclude: integration tests.

### 9C - Add engine/user field separation tests ###

    * [ ] Action: add tests verifying engine updates preserve user pins and user updates preserve engine progress.
    * [ ] Scope: `Tests/StateStore/Handlers/FieldSeparationTests.cs`.
    * [ ] Verify: tests fail if field separation is violated; AddOrUpdateTaskCommandHandler covered comprehensively.
    * [ ] Commit message: `phase3(step9C): add engine/user field separation tests`
    * [ ] Must include: field separation assertions for AddOrUpdateTaskCommandHandler only.
    * [ ] Must exclude: snapshot tests.

### 9D - Add snapshot immutability tests ###

    * [ ] Action: add tests verifying snapshots are defensive copies and mutations do not affect canonical state.
    * [ ] Scope: `Tests/StateStore/Models/SnapshotImmutabilityTests.cs`.
    * [ ] Verify: tests fail if snapshot allows mutation of canonical state; defensive copy verified.
    * [ ] Commit message: `phase3(step9D): add snapshot immutability tests`
    * [ ] Must include: immutability assertions only.
    * [ ] Must exclude: publishing tests.

### 9E - Add snapshot publishing tests ###

    * [ ] Action: add tests verifying SnapshotChanged event fires correctly after state changes.
    * [ ] Scope: `Tests/StateStore/SnapshotPublishingTests.cs`.
    * [ ] Verify: event fires with correct snapshot after command processing; event subscription and invocation verified.
    * [ ] Commit message: `phase3(step9E): add snapshot publishing tests`
    * [ ] Must include: event subscription and assertion tests only.
    * [ ] Must exclude: UI subscription tests (Phase 4).

### 9F - Add day boundary behavior tests ###

    * [ ] Action: add tests verifying expired daily tasks are removed on day transition.
    * [ ] Scope: `Tests/StateStore/DayBoundary/DayBoundaryTests.cs`.
    * [ ] Verify: tests confirm day-keyed tasks expire correctly; expiration detection and removal verified.
    * [ ] Commit message: `phase3(step9F): add day boundary behavior tests`
    * [ ] Must include: expiration detection and removal tests only.
    * [ ] Must exclude: persistence tests.

### 9G - Add manual ID counter tests ###

    * [ ] Action: add tests verifying manual task IDs are unique, sequential, and deterministic.
    * [ ] Scope: `Tests/StateStore/ManualTaskCounterTests.cs`.
    * [ ] Verify: counter produces non-colliding IDs in correct format; sequential calls produce sequential IDs.
    * [ ] Commit message: `phase3(step9G): add manual ID counter tests`
    * [ ] Must include: counter determinism and uniqueness tests only.
    * [ ] Must exclude: persistence tests (Phase 7).

### 9H - Add State Store boundary tests ###

    * [ ] Action: add tests verifying State Store enforces mutation-only-via-commands boundary.
    * [ ] Scope: `Tests/StateStore/StateStoreBoundaryTests.cs`.
    * [ ] Verify: tests confirm no direct state mutation paths exist; boundary enforcement verified.
    * [ ] Commit message: `phase3(step9H): add State Store boundary tests`
    * [ ] Must include: boundary enforcement assertions only.
    * [ ] Must exclude: performance tests.

### 9I - Add integration tests for lifecycle wiring ###

    * [ ] Action: add tests verifying State Store integrates correctly with lifecycle coordinator.
    * [ ] Scope: `Tests/Lifecycle/LifecycleCoordinatorIntegrationTests.cs`.
    * [ ] Verify: initialization and teardown execute correctly; lifecycle flow verified.
    * [ ] Commit message: `phase3(step9I): add lifecycle integration tests`
    * [ ] Must include: integration tests for init/teardown only.
    * [ ] Must exclude: full game simulation.

## Step 9 Completion ##

    * [ ] All substeps in Step 9 complete (9A, 9B, 9C, 9D, 9E, 9F, 9G, 9H, 9I).

## 10) Phase 3 Completion Gate ##

Step goal:

    * [ ] Verify implementation is complete, tests pass, and contracts are satisfied.

### 10A - Run clean build and full test suite ###

    * [ ] Action: execute clean build and run all Phase 3 tests.
    * [ ] Scope: no source changes expected.
    * [ ] Verify: build succeeds without warnings, all Phase 3 tests pass.
    * [ ] Commit message: `phase3(step10A): record successful build and test completion`
    * [ ] Must include: build log or test output confirmation.
    * [ ] Must exclude: opportunistic code edits.

### 10B - Audit guardrails and checklist completion ###

    * [ ] Action: review implementation against each guardrail from checklist start.
    * [ ] Scope: this checklist file.
    * [ ] Verify: all guardrails preserved, no scope drift.
    * [ ] Commit message: `phase3(step10B): audit guardrails and mark checklist complete`
    * [ ] Must include: guardrail review notes.
    * [ ] Must exclude: new implementation work.

### 10C - Review atomic commit boundaries and defer list ###

    * [ ] Action: confirm all substeps were atomic and review deferred items.
    * [ ] Scope: this checklist file.
    * [ ] Verify: deferred items documented for future phases.
    * [ ] Commit message: `phase3(step10C): review atomic boundaries and defer list`
    * [ ] Must include: defer list and boundary review notes.
    * [ ] Must exclude: production code changes.

## Step 10 Completion ##

    * [ ] All substeps in Step 10 complete (10A, 10B, 10C).


## Deferred Items ##

** Deferred to Phase 4 **
    -Determine if two TaskObject Properties are ambiguous and should be refactored:
        - TaskSourceType
        - SourceIdentifier
    -Both terms are used for different purposes but could be confusing. TaskSourceType indicates
    what type of source produced this task (BuiltIn, TaskBuilder, or Manual), while SourceIdentifier
    indicated "which specific source instance?" For example, a BuiltIn task might have
    TaskSourceType=BuiltIn and SourceIdentifier=DailyLuckTask, while a TaskBuilder task might have
    TaskSourceType=TaskBuilder and SourceIdentifier=QuestGiver_123. If we keep both, we should
    ensure their purposes are clearly documented and consider renaming for clarity (e.g.,
    TaskOriginType and TaskSourceId).

**Deferred to Phase 4 (ViewModels):**
    - Actual subscription to `SnapshotChanged` event
    - INPC property updates from snapshots
    - UI-local state (selection, filters, scroll)

**Deferred to Phase 5+ (Generators/Engine):**
    - Task generation logic producing commands
    - Built-in task generators
    - Deadline field population
    - Task-type ordering/comparison

**Deferred to Phase 6 (Rule Engine):**
    - Task Builder rule evaluation
    - Rule-driven command generation

**Deferred to Phase 7 (Persistence):**
    - Save/load of State Store state
    - Manual task counter persistence across sessions
    - Version migration logic
    - Baseline value storage

**Deferred to Phase 8+ (Menu/HUD):**
    - UI interactions dispatching commands
    - Visual feedback on state changes

**Deferred to V2:**
    - Batch command transactions
    - Undo history
    - Dismissed task tracking
    - Multiplayer synchronization

## Key Planning Decisions ##

**TaskRecord vs TaskObject:** Created lightweight internal `TaskRecord` as State Store's storage
structure, separate from `TaskObject` domain model. Keeps store implementation details isolated from
domain contracts.

**TaskView Definition:** `TaskView` is a read-only record mirroring `TaskObject` fields needed for
UI consumption. Prevents accidental mutation and enforces snapshot immutability contract.

**Command Handler Naming:** Use `CommandHandler` suffix consistently (e.g.,
`AddOrUpdateTaskCommandHandler`). "Reducer" is banned by C# style contract; "CommandHandler" is
clear, explicit, and action-oriented.

**Namespace Structure:** Use `JojaAutoTasks.StateStore` as primary namespace with subfolders:
    - `StateStore/Commands/` — all command types (6 command types + base infrastructure)
    - `StateStore/Handlers/` — all command handlers
    - `StateStore/Models/` — TaskRecord, TaskView, TaskSnapshot
    - `StateStore/DayBoundary/` — expiration logic
    - `StateStore/StateStore.cs` — main API class
