# Phase 4 - Atomic Commit Execution Checklist

| **Detail**             | **Description**                                                                                                                   |
| ---------------------- | --------------------------------------------------------------------------------------------------------------------------------- |
| **Phase:**             | Phase 4 (View Model Infrastructure)                                                                                               |
| **Scope:**             | INPC-based view model foundation, snapshot subscription<br>lifecycle, UI command dispatch, command/snapshot<br>boundary integrity |
| **Target Deferments:** | DEF-007, DEF-008, DEF-009, DEF-010, DEF-032                                                                                       |
| **Status:**            | Draft-Ready for Execution                                                                                                         |

## Guardrails (Must Stay True)

- [ ] **Command/Snapshot Boundary Integrity**: UIViewModel receives snapshots from
      state subscription; never mutates canonical state directly. All state writes
      route through commands.
- [ ] **Deterministic Reconciliation by Stable Key**: Collection/item reconciliation
      uses stable identifiers (TaskId, RuleId, etc.) with sorted iteration; no reliance
      on list order or mutation sequence.
- [ ] **Subscription Lifecycle Ownership**: ViewModels subscribe to SnapshotChanged
      on initialization; unsubscribe on disposal. No dangling subscriptions.
- [ ] **No Optimistic Canonical Mutation**: UI never writes to StateStore, StateContainer,
      or StateHandle directly. Only read canonical state through snapshots.
- [ ] **ManualTask\_{N} Terminology Baseline**: All new property names, comments,
      and UI text use `ManualTask_{N}` wording consistently (not "Manual Rule" or
      "Custom Task").
- [ ] **DEF-032 Exception-Only Boundary**: ConfigLoader catch-path hardening limited
      to exception handling, fallback logic, and structured logging. No config schema
      expansion, version migration logic, or feature-level decision-making in catch paths.

## Phase Overview

### Phase Goal

Implement deterministic, testable UI infrastructure founded on INPC-based view models
that subscribe to snapshot changes and project state into bindable properties. Establish
the command/snapshot boundary integrity pattern and resolve terminology ambiguity
(DEF-007),snapshot subscription lifecycle (DEF-008), INPC/collection reconciliation
mechanics (DEF-009), UI-local state ownership patterns (DEF-010), and ConfigLoader
exception hardening (DEF-032). Phase 4 outputs are testable without running Stardew
Valley.

**Phase scope includes initial consumer view models (HudViewModel, TaskListViewModel)
and additional catalog surface view models (HudTaskRowViewModel, TaskDetailViewModel,
HistoryViewModel, ManualTaskEditorViewModel, ConfigViewModel) with complete coverage
of subscription initialization, snapshot projection, and command dispatch patterns
across all included surfaces.**

### Architecture Components

- **`UIViewModelBase`** — Base class implementing `INotifyPropertyChanged` via `PropertyChanged.SourceGenerator`;
   defines property change notification pattern
- **`HudViewModel`** — HUD surface view model; subscribes to `SnapshotChanged` and
   projects task summary/status data into bindable properties
- **`TaskListViewModel`** — Task list/history surface view model; manages `TaskSnapshot`
   collection with deterministic reconciliation
- **`HudTaskRowViewModel`** — HUD row item view model; projects individual task
   properties with binding support
- **`TaskDetailViewModel`** — Detail surface view model; provides comprehensive
   task information and edit capability binding
- **`HistoryViewModel`** — History/past tasks surface view model; maintains sorted,
   reconciled task history collection
- **`ManualTaskEditorViewModel`** — Editor surface view model for manual task creation/editing;
   manages form state and validation binding
- **`ConfigViewModel`** — Configuration surface view model; projects config state
   and dispatches config changes (if Phase 4 scope includes config UI)
- **`UISnapshotSubscriptionManager`** — Lifecycle coordinator for view model subscription/unsubscription;
   handles per-viewmodel dispose safety with token-based unsubscribe semantics
- **`ConfigLoader Exception Hardening`** — Deterministic fallback logic and structured
   logging in config read/normalize catch paths
- **`UI Command Dispatch`** — Pattern for marshaling user commands (add task, complete,
   pin, etc.) from UI into StateStore command system

### Prerequisites

- **Phase 1** (Lifecycle/Config): ModEntry, lifecycle hooks (OnSaving, UpdateTicked),
   ConfigLoader, logging
- **Phase 2** (State Foundation): StateStore, StateContainer, SnapshotProjector,
   TaskSnapshot and related domain models
- **Phase 3** (Commands/Handlers): Command contracts, command handlers, deterministic
   state mutation pattern

Phase 4 builds on these by wiring UI surface observation into snapshot subscription
and command dispatch.

### Architecture Relationships

**Prior Phases:**

- Phase 1 provides config-ready `ModEntry` and `Lifecycle` signals
- Phase 3 provides command contracts and deterministic handlers
- Phase 3 provides `StateStore` with `SnapshotChanged` event and `TaskSnapshot`
   projections

**This Phase:**

- Implements UI foundation (view models, property binding, subscription lifecycle)
- Establishes snapshot-to-UI projection pattern (read-only snapshots → bindable
   properties)
- Establishes UI-to-command pattern (user action → command → snapshot update → UI
   refresh)
- Resolves in-phase deferments: DEF-007 (terminology), DEF-008 (subscription lifecycle),
   DEF-009 (INPC/reconciliation), DEF-010 (UI state ownership), DEF-032 (exception
   hardening)

**Future Phases:**

- Phase 5 (HUD/Menu UI): Render HudViewModel and TaskListViewModel into game surfaces
- Phase 6 (User Interactions): Wire click/input handlers into command dispatch
- Phase 7+ (Polish/Persistence): Performance optimization, save/load integration,
   extended features

### Design Guide References

- [Phase 21 - Implementation Plan](../../../Project/Planning/Joja%20AutoTasks%20Design%20Guide/Section%2021%20-%20Implementation%20Plan.md)
- [Architecture Map](../../../Project/Planning/Architecture%20Map.md)
- [Design Principles](../../../Project/Planning/Design%20Principles.md)

---

## Step 1: Bootstrap UI Foundation with ModEntry Wiring

### Step Goal

- [x] Add UI hook entry points in ModEntry; wire SnapshotChanged subscription at game startup; establish lifecycle teardown.

### 1A - Add UISnapshotSubscriptionManager with per-subscriber handle semantics

- [x] **Action:** Create `UI/UISnapshotSubscriptionManager.cs` with public static
      Subscribe method that returns an IDisposable token and Unsubscribe method.
      Subscribe method accepts a snapshot action callback, registers it with `StateStore.SnapshotChanged`,
      and returns a handle (`IDisposable`) that unsubscribes **only that specific
      callback** when Dispose is called. `Unsubscribe(IDisposable handle)` is
      alternative explicit form (deprecated in favor of handle.Dispose()).
      Implement per-subscriber unsubscribe guarantee: calling Dispose on returned
      token removes only that subscription, not all subscriptions. Add thread-safety
      (lock) if shared state required; otherwise, document that calls must occur
      on main thread.
- [x] **Scope:** `UI/UISnapshotSubscriptionManager.cs` (new file; new public class
      `UISnapshotSubscriptionManager` with `Subscribe(Action<TaskSnapshot>) : IDisposable`
      and optional `Unsubscribe(IDisposable handle)` methods).
- [x] **Verify:** File compiles; Subscribe returns an IDisposable token; Dispose
      on token removes only that subscription; repeated Subscribe/Dispose cycles
      work correctly; no global unsubscribe-all behavior; no runtime errors when
      Subscribe is called before StateStore initialization (must handle null StateStore
      gracefully with early return or optional check).
- [x] **Suggested commit:** "phase4(step1A): add `UISnapshotSubscriptionManager`
      with per-subscriber handle semantics"
- [x] **Must include:** Subscribe returns IDisposable token; Dispose on token unsubscribes
      only that callback; per-subscriber safety guarantee; hook into `StateStore.SnapshotChanged`
      event; lifecycle documentation.
- [x] **Must exclude:** View model implementations, property binding, UI rendering.

### 1B - Wire UISnapshotSubscriptionManager into ModEntry lifecycle with ordering guard

- [ ] **Action:** In `ModEntry.Entry`, **after `StateStore` and `StateContainer`
      are fully initialized** (verify initialization order: `StateStore`, `SnapshotProjector`,
      `StateContainer` must all be available before Subscribe is called), call `UISnapshotSubscriptionManager.Subscribe`
      with a lambda or delegate that receives `TaskSnapshot` updates. Store returned
      `IDisposable` token. **Add explicit guard assertion**: `StateStore.Instance`
      must not be `null`; if `null`, skip subscription with logged warning and return
      gracefully (do not throw). In `ModEntry.OnSaving`, call `Dispose` on the stored
      token to tear down subscription before game save (disposing token unsubscribes
      only that specific callback).
- [ ] **Scope:** `ModEntry.cs` (Entry and OnSaving methods; StateStore initialization
      ordering guard).
- [ ] **Verify:** Build succeeds; `ModEntry` loads without throwing; `StateStore`
      initialization order guard prevents null-reference during subscription;
      tracing subscription is established during `Entry` and can be verified through  
      log (add minimal log statement when subscription established); token disposal
      properly unsubscribes.
- [ ] **Suggested commit:** "phase4(step1B): wire `UISnapshotSubscriptionManager`
      into `ModEntry` lifecycle with initialization order guard"
- [ ] **Must include:** Call to `Subscribe` in `Entry` after `StateStore` fully
      initialized; explicit null-check guard on `StateStore.Instance`; token disposal
      in `OnSaving`; minimal logging to confirm subscription lifecycle; documented
      initialization order constraint.
- [ ] **Must exclude:** View model implementation, UI rendering, command dispatch
      setup (defer to later steps).

### 1C - Create UIViewModelBase with INotifyPropertyChanged

- [ ] **Action:** Create `UI/ViewModels/UIViewModelBase.cs` as abstract base class implementing `INotifyPropertyChanged`. Use PropertyChanged.SourceGenerator annotation ([ObservableProperty]) on fields to auto-generate property change notifications. Add protected OnPropertyChanged method for manual notification if needed.
- [ ] **Scope:** `UI/ViewModels/UIViewModelBase.cs` (new file; new abstract class `UIViewModelBase : INotifyPropertyChanged`).
- [ ] **Verify:** Build succeeds; INotifyPropertyChanged interface is implemented; PropertyChanged event is available to subscribers; compile-time generation succeeds.
- [ ] **Suggested commit:** `phase4(step1C): add UIViewModelBase with INPC via PropertyChanged.SourceGenerator`
- [ ] **Must include:** INotifyPropertyChanged implementation; PropertyChanged event; [ObservableProperty] annotation support; protected methods for property change notification.
- [ ] **Must exclude:** Concrete view model implementations (HudViewModel, TaskListViewModel); business logic property definitions.

### Step 1 Completion Checkpoint

- [ ] All substeps in Step 1 complete (1A, 1B, 1C).

---

## Step 2: Hardening ConfigLoader Exception Paths (DEF-032)

### Step Goal

- [ ] Add deterministic exception handling, fallback logic, and structured logging to ConfigLoader catch paths. Ensure config read, normalization, and default fallback are robust without expanding config schema or migration logic.

### 2A - Add structured exception logging to ConfigLoader.Load catch path

- [ ] **Action:** In `Configuration/ConfigLoader.cs` Load method, add try-catch block around config file deserialization. In catch block, log exception details (exception type, message, inner exception chain) using ModLogger at Error level with structured context: include file path attempted, migration context if applicable, and fallback action taken. Do not suppress the exception; allow it to propagate after logging, or convert to typed exception with context.
- [ ] **Scope:** `Configuration/ConfigLoader.cs` (Load method; catch block for deserialization exceptions).
- [ ] **Verify:** Build succeeds; exception logging is testable (verify log output format in unit tests); fallback behavior is deterministic (same input → same fallback).
- [ ] **Suggested commit:** `phase4(step2A): add structured logging to ConfigLoader exception paths`
- [ ] **Must include:** Exception logging at Error level with context (file path, exception chain, fallback action); deterministic fallback specification in log message.
- [ ] **Must exclude:** Config schema changes, version migration logic, feature-level decision-making in exception handler.

### 2B - Add default fallback specification for missing/invalid config (DEF-032 scope limit)

- [ ] **Action:** Document explicit fallback order in ConfigLoader.Load: if config file missing, load defaults from ModConfig.DefaultConfig; if file corrupt/unparseable, log and apply defaults; if version mismatch detected, **call only existing migration utilities (e.g., ConfigMigrationHelper.MigrateFromPrior) already defined in ConfigLoader or Configuration package — no new migration/schema/version logic may be introduced in Phase 4**. Apply fallback if migration fails or is not applicable. Implement this order deterministically (not probabilistic or feature-gated). Add code comments documenting fallback precedence and DEF-032 boundary constraint (config read/normalize/fallback only; no new schema version logic).
- [ ] **Scope:** `Configuration/ConfigLoader.cs` (Load method; fallback specification comments and logic; DEF-032 boundary assertion).
- [ ] **Verify:** Fallback is deterministic (same error condition → same fallback every time); code comments clearly document precedence and DEF-032 constraint (only existing utilities allowed); unit tests can verify each fallback path; no new version/schema logic introduced.
- [ ] **Suggested commit:** `phase4(step2B): specify deterministic config fallback order with DEF-032 scope guard`
- [ ] **Must include:** Explicit fallback precedence (missing → corrupt → mismatch); deterministic logic with no probabilistic branches; inline code documentation of DEF-032 boundary (existing utilities only, no new schema/version/migration logic); reference to existing migration utilities only.
- [ ] **Must exclude:** Config schema expansion, new config fields, new migration/version logic, feature migrations.

### 2C - Add unit tests for ConfigLoader exception handling

- [ ] **Action:** In `Tests/Configuration/ConfigLoaderTests.cs`, add test cases for: (1) missing config file → defaults applied, (2) corrupt JSON → exception logged + defaults applied, (3) version mismatch → migration attempted, if successful use migrated config, if failed use defaults. Use mocked file system if available; otherwise use temporary file fixtures. Verify log statements contain expected exception context and fallback action.
- [ ] **Scope:** `Tests/Configuration/ConfigLoaderTests.cs` (new test methods for exception scenarios).
- [ ] **Verify:** All new tests pass; exception paths are exercised; log output assertions are precise (exception type, message, fallback action all logged).
- [ ] **Suggested commit:** `phase4(step2C): add unit tests for ConfigLoader exception scenarios`
- [ ] **Must include:** Test cases for missing config, corrupt JSON, version mismatch; assertions on log output; deterministic fallback verification.
- [ ] **Must exclude:** Production logic changes, config schema updates.

### Step 2 Completion Checkpoint

- [ ] All substeps in Step 2 complete (2A, 2B, 2C).

---

## Step 3: Implement Snapshot Subscription Lifecycle (DEF-008)

### Step Goal

- [ ] Establish SnapshotChanged event subscription pattern; wire view models to receive snapshot updates; implement deterministic unsubscription on dispose.

### 3A - Create SnapshotChangedEventArgs contract

- [ ] **Action:** Create `Events/SnapshotChangedEventArgs.cs` (if not present) with public property `TaskSnapshot CurrentSnapshot` and optional `DateTime Timestamp`. Use this as the event argument type for StateStore.SnapshotChanged event (or define/update StateStore event signature to use this).
- [ ] **Scope:** `Events/SnapshotChangedEventArgs.cs` (new or updated file; public class with TaskSnapshot and Timestamp properties).
- [ ] **Verify:** Compiles; StateStore.SnapshotChanged event (if it exists) uses SnapshotChangedEventArgs as argument type; event is raisable.
- [ ] **Suggested commit:** `phase4(step3A): introduce SnapshotChangedEventArgs contract`
- [ ] **Must include:** SnapshotChangedEventArgs class with TaskSnapshot and Timestamp; public property access.
- [ ] **Must exclude:** Event handling logic, view model wiring.

### 3B - Verify SnapshotChanged event signature compatibility in StateStore

- [ ] **Action:** In `State/StateStore.cs`, ensure public event `EventHandler<SnapshotChangedEventArgs> SnapshotChanged` is declared **before adding or changing any event arguments**. Verify event signature compatibility: existing event signature (if present) must accept SnapshotChangedEventArgs without change. If SnapshotChanged exists with different signature or args (e.g., TaskSnapshot directly, or no args), document the mismatch and resolve by: (A) extending SnapshotChangedEventArgs to wrap existing args, or (B) updating event signature to standard `EventHandler<SnapshotChangedEventArgs>`. Verify it is raised by SnapshotProjector whenever a new snapshot is projected (triggered by command execution or other state mutations). Add code comments documenting event contract and argument flow.
- [ ] **Scope:** `State/StateStore.cs` (SnapshotChanged event declaration, signature verification, and compatibility assertion).
- [ ] **Verify:** Event signature matches `EventHandler<SnapshotChangedEventArgs>` precisely; if signature change required, update event and all publishing/subscribing code; event is raised whenever snapshot changes; subscribers can subscribe and unsubscribe without errors; signature change is minimal (compatibility-preserving if possible).
- [ ] **Suggested commit:** `phase4(step3B): verify and ensure SnapshotChanged event signature compatibility`
- [ ] **Must include:** Public SnapshotChanged event declaration with standard signature; explicit signature compatibility check; event is raised on snapshot updates; clear documentation of event contract and argument flow.
- [ ] **Must exclude:** Snapshot projection logic changes, general command handler modifications (limited to event publishing as required).

### 3C - Implement view model subscription initialization

- [ ] **Action:** In `UI/ViewModels/HudViewModel.cs` and `UI/ViewModels/TaskListViewModel.cs` constructors, call `UISnapshotSubscriptionManager.Subscribe` to register a snapshot change callback. Store callback reference as instance field for unsubscription. Add constructor parameter for StateStore or inject dependency if pattern supports DI.
- [ ] **Scope:** `UI/ViewModels/HudViewModel.cs` and `UI/ViewModels/TaskListViewModel.cs` (constructors; subscription registration).
- [ ] **Verify:** Constructors compile; subscription is registered when view model instantiated; callback is callable (not null).
- [ ] **Suggested commit:** `phase4(step3C): implement view model subscription initialization`
- [ ] **Must include:** Subscription registration in constructor; callback reference stored; no syntax errors.
- [ ] **Must exclude:** Property binding, snapshot projection, UI rendering.

### 3D - Implement deterministic unsubscription on dispose

- [ ] **Action:** Add `IDisposable` implementation to HudViewModel and TaskListViewModel. In Dispose method, call `UISnapshotSubscriptionManager.Unsubscribe` to deregister snapshot callback. Use finalizer pattern (destructor) as safety net if Dispose is not called; log warning in finalizer.
- [ ] **Scope:** `UI/ViewModels/HudViewModel.cs` and `UI/ViewModels/TaskListViewModel.cs` (IDisposable.Dispose method; optional finalizer).
- [ ] **Verify:** IDisposable is implemented; Dispose is callable without error; unsubscription is deterministic (always removes callback when called).
- [ ] **Suggested commit:** `phase4(step3D): add IDisposable with deterministic unsubscription`
- [ ] **Must include:** IDisposable.Dispose method; unsubscription call; thread-safety if view models are multi-threaded.
- [ ] **Must exclude:** Business logic in Dispose; snapshot manipulation.

### Step 3 Completion Checkpoint

- [ ] All substeps in Step 3 complete (3A, 3B, 3C, 3D).

---

## Step 4: Implement View Model Property Projection with INPC (DEF-009)

### Step Goal

- [ ] Implement bindable properties in HudViewModel and TaskListViewModel that project TaskSnapshot data. Establish INPC notification on property changes. Implement deterministic collection reconciliation by stable key.

### 4A - Define HudViewModel properties and snapshot projection

- [ ] **Action:** In `UI/ViewModels/HudViewModel.cs`, define properties (with [ObservableProperty] annotation or manual backing fields and OnPropertyChanged calls) for: ActiveTaskCount, CompletedTaskCount, PinnedTaskCount, LastUpdateTime. Implement `OnSnapshotChanged(TaskSnapshot snapshot)` method that receives snapshot, extracts counts from snapshot.Tasks collection, and updates properties. Use task status and pin state to compute counts deterministically.
- [ ] **Scope:** `UI/ViewModels/HudViewModel.cs` (HudViewModel properties: ActiveTaskCount, CompletedTaskCount, PinnedTaskCount, LastUpdateTime; OnSnapshotChanged method).
- [ ] **Verify:** Properties are bindable (INotifyPropertyChanged notifications trigger); OnSnapshotChanged computes counts correctly; counts match snapshot data deterministically.
- [ ] **Suggested commit:** `phase4(step4A): implement HudViewModel properties with snapshot projection`
- [ ] **Must include:** Observable properties (ActiveTaskCount, CompletedTaskCount, PinnedTaskCount, LastUpdateTime); OnSnapshotChanged implementation; deterministic count computation.
- [ ] **Must exclude:** UI rendering, user input handling, command dispatch.

### 4B - Implement TaskListViewModel collection and reconciliation with explicit procedure

- [ ] **Action:** In `UI/ViewModels/TaskListViewModel.cs`, define property `ObservableCollection<TaskItemViewModel> Tasks` (or compatible bindable collection). Implement `OnSnapshotChanged(TaskSnapshot snapshot)` to reconcile snapshot.Tasks with the view model collection using explicit deterministic procedure: (1) **Extract TaskIds**: collect TaskId keys from snapshot.Tasks (source list) and current Tasks collection (target list); (2) **Compute Delta**: identify added TaskIds (in source, not in target), removed TaskIds (in target, not in source), common TaskIds; (3) **Execute Delta Deterministically**: for each added TaskId (in sorted order), construct TaskItemViewModel and add to collection; for each removed TaskId (in sorted order), remove from collection; for each common TaskId (in sorted order), find existing TaskItemViewModel, update properties in-place (do not remove/re-add); (4) **Verify Order**: after delta execution, iterate Tasks collection and verify TaskId order matches snapshot.Tasks order by TaskId (sorted stable sort). **No semantic reordering—only minimal inserts/removes required by delta.** Document procedure in code comments with step numbers.
- [ ] **Scope:** `UI/ViewModels/TaskListViewModel.cs` (Tasks collection property; OnSnapshotChanged with explicit reconciliation procedure documented in 4 steps).
- [ ] **Verify:** Collection updates correctly when snapshot changes; reconciliation follows 4-step procedure explicitly; reconciliation is deterministic (same snapshot → same collection state every time); no spurious reorders or mutation beyond minimal delta; final TaskId order matches snapshot order.
- [ ] **Suggested commit:** `phase4(step4B): implement TaskListViewModel collection with explicit deterministic reconciliation procedure`
- [ ] **Must include:** Observable collection property; OnSnapshotChanged reconciliation using explicit 4-step procedure (extract, compute delta, execute delta, verify order); TaskId as stable key; deterministic iteration with sorted TaskId order; documented procedure in code comments.
- [ ] **Must exclude:** UI rendering, scrolling logic, pagination.

### 4C - Create TaskItemViewModel for row binding

- [ ] **Action:** Create `UI/ViewModels/TaskItemViewModel.cs` with properties mapping to TaskRecord display: Id (TaskId), Title, Status, IsPinned, CreatedDay, LastModifiedDay. Add constructor accepting TaskRecord to populate properties. Implement INotifyPropertyChanged.
- [ ] **Scope:** `UI/ViewModels/TaskItemViewModel.cs` (new file; public class `TaskItemViewModel : UIViewModelBase`).
- [ ] **Verify:** Constructor accepts TaskRecord; properties are readable; INotifyPropertyChanged is available on property change.
- [ ] **Suggested commit:** `phase4(step4C): create TaskItemViewModel for row binding`
- [ ] **Must include:** Properties for Id, Title, Status, IsPinned, CreatedDay, LastModifiedDay; constructor from TaskRecord; INotifyPropertyChanged.
- [ ] **Must exclude:** UI rendering, command handling.

### 4D - Add unit tests for collection reconciliation

- [ ] **Action:** In `Tests/UI/ViewModels/TaskListViewModelTests.cs`, add test cases: (1) reconciliation adds new items, (2) reconciliation removes deleted items, (3) reconciliation updates modified items in-place, (4) reconciliation is deterministic (same snapshot → same collection state). Use snapshot builder helpers or fixtures to create test snapshots with controlled task lists.
- [ ] **Scope:** `Tests/UI/ViewModels/TaskListViewModelTests.cs` (new file; test class with reconciliation test methods).
- [ ] **Verify:** All reconciliation test cases pass; collection state is deterministic; no spurious updates detected.
- [ ] **Suggested commit:** `phase4(step4D): add unit tests for TaskListViewModel collection reconciliation`
- [ ] **Must include:** Test cases for add, remove, update, and determinism; assertions on collection state after reconciliation.
- [ ] **Must exclude:** Production logic changes.

### 4E - Implement HudTaskRowViewModel property projection

- [ ] **Action:** Create `UI/ViewModels/HudTaskRowViewModel.cs` inheriting from UIViewModelBase. Define properties for individual HUD row display: Id (TaskId), Title, Status, IsPinned, DayCreated. Add constructor accepting TaskRecord. Implement property updates when row-level changes occur.
- [ ] **Scope:** `UI/ViewModels/HudTaskRowViewModel.cs` (new file; public class `HudTaskRowViewModel : UIViewModelBase`).
- [ ] **Verify:** Properties are bindable; constructor accepts TaskRecord; INotifyPropertyChanged is available.
- [ ] **Suggested commit:** `phase4(step4E): implement HudTaskRowViewModel with row property binding`
- [ ] **Must include:** Observable properties for Id, Title, Status, IsPinned, DayCreated; constructor from TaskRecord; INotifyPropertyChanged.
- [ ] **Must exclude:** HUD rendering, row selection/focus logic.

### 4F - Implement TaskDetailViewModel with comprehensive projections

- [ ] **Action:** Create `UI/ViewModels/TaskDetailViewModel.cs` inheriting from UIViewModelBase. Define properties for detailed task view: Id, Title, Description, Status, IsPinned, CreatedDay, LastModifiedDay, SourceIdentifier, TaskSourceType. Add constructor accepting TaskRecord. Implement property updates for all fields.
- [ ] **Scope:** `UI/ViewModels/TaskDetailViewModel.cs` (new file; public class `TaskDetailViewModel : UIViewModelBase`).
- [ ] **Verify:** All properties are readable and bindable; constructor accepts TaskRecord; deterministic property projection from record data.
- [ ] **Suggested commit:** `phase4(step4F): implement TaskDetailViewModel with comprehensive field projections`
- [ ] **Must include:** Observable properties for all TaskRecord fields; constructor from TaskRecord; binding-friendly property layout; SourceIdentifier/TaskSourceType terminology consistency.
- [ ] **Must exclude:** Edit handlers, save/cancel logic.

### 4G - Implement HistoryViewModel with sorted collection of past tasks

- [ ] **Action:** Create `UI/ViewModels/HistoryViewModel.cs` inheriting from UIViewModelBase with property `ObservableCollection<HistoryItemViewModel> CompletedTasks`. Implement `OnSnapshotChanged(TaskSnapshot snapshot)` to reconcile completed tasks using the 4-step deterministic procedure from 4B. Add HistoryItemViewModel for row display.
- [ ] **Scope:** `UI/ViewModels/HistoryViewModel.cs` and `UI/ViewModels/HistoryItemViewModel.cs` (new files; public classes inheriting from UIViewModelBase).
- [ ] **Verify:** Collection updates correctly on snapshot change; reconciliation follows 4-step procedure; final order matches configured sort.
- [ ] **Suggested commit:** `phase4(step4G): implement HistoryViewModel with deterministic completed task reconciliation`
- [ ] **Must include:** Observable collection property; OnSnapshotChanged with explicit reconciliation procedure; sorted iteration; HistoryItemViewModel; deterministic ordering.
- [ ] **Must exclude:** Pagination, archive/purge logic.

### 4H - Implement ManualTaskEditorViewModel with form binding and validation

- [ ] **Action:** Create `UI/ViewModels/ManualTaskEditorViewModel.cs` inheriting from UIViewModelBase. Define properties: TitleInput (string), DescriptionInput (string), IsValid (bool). Add method `OnInputChanged()` that validates form state and updates IsValid. Add method `GetEditCommand()` that constructs AddOrUpdateTaskCommand from current form state.
- [ ] **Scope:** `UI/ViewModels/ManualTaskEditorViewModel.cs` (new file; public class `ManualTaskEditorViewModel : UIViewModelBase`).
- [ ] **Verify:** Form properties are bindable; IsValid updates deterministically; GetEditCommand constructs valid command from form state.
- [ ] **Suggested commit:** `phase4(step4H): implement ManualTaskEditorViewModel with form binding and validation`
- [ ] **Must include:** Observable properties for form inputs; IsValid property; OnInputChanged validation; GetEditCommand for command construction.
- [ ] **Must exclude:** UI rendering, form submission.

### 4I - Implement ConfigViewModel with config state projection (if in Phase 4 scope)

- [ ] **Action:** If config UI is within Phase 4 scope, create `UI/ViewModels/ConfigViewModel.cs` inheriting from UIViewModelBase. Define properties for ModConfig fields (IsHudEnabled, HudPosition, TaskLimit, UpdateFrequencyMinutes). Add constructor accepting ModConfig. Implement DispatchConfigChange method for config updates. If config UI is deferred, create placeholder with documented deferment notes.
- [ ] **Scope:** `UI/ViewModels/ConfigViewModel.cs` (new file; conditional on Phase 4 scope confirmation).
- [ ] **Verify:** Property bindings match ModConfig fields; config dispatch is wired if in scope; deferment documented if not.
- [ ] **Suggested commit:** `phase4(step4I): implement ConfigViewModel with config state projection (if in scope)`
- [ ] **Must include:** Observable properties for config fields; constructor from ModConfig; config dispatch if Phase 4 scope, or documented deferment if not.
- [ ] **Must exclude:** Config file persistence, feature-gated behavior.

### Step 4 Completion Checkpoint

- [ ] All substeps in Step 4 complete (4A, 4B, 4C, 4D, 4E, 4F, 4G, 4H, 4I).

---

## Step 5: Establish UI-Local State Ownership and Command/Snapshot Loop (DEF-010)

### Step Goal

- [ ] Implement UI command dispatch pattern: map user actions to commands, dispatch through StateStore, receive updated snapshots, refresh UI via bindings. Establish clear boundary: UI owns presentation state (selected item, scroll position, form input) but not canonical task state.

### 5A - Create IUICommandDispatcher contract with explicit signatures

- [ ] **Action:** Create `UI/Commands/IUICommandDispatcher.cs` (contract interface) with explicit method signatures (no "..." parameter ellipsis): `DispatchAddTask(string title, string sourceIdentifier)`, `DispatchCompleteTask(TaskId id)`, `DispatchPinTask(TaskId id)`, `DispatchUnpinTask(TaskId id)`, `DispatchRemoveTask(TaskId id)`. Each method signature explicitly lists all parameters required to construct the corresponding IStateCommand (AddOrUpdateTaskCommand, CompleteTaskCommand, PinTaskCommand, UnpinTaskCommand, RemoveTaskCommand from Phase 3). Each method maps UI user action to corresponding IStateCommand and routes through StateStore. Return void (synchronous dispatch; if async dispatch needed defer to post-Phase 4). Document each method's command type mapping and required parameters in XML doc comments.
- [ ] **Scope:** `UI/Commands/IUICommandDispatcher.cs` (new file; public interface `IUICommandDispatcher` with explicit method signatures and XML documentation).
- [ ] **Verify:** Interface compiles; method signatures are explicit and fully specified (no "..."); code that implements interface can unambiguously construct required command types from method parameters; signatures map to existing command types from Phase 3; command type mapping is documented.
- [ ] **Suggested commit:** `phase4(step5A): introduce IUICommandDispatcher contract with explicit signatures`
- [ ] **Must include:** Dispatch methods for add/complete/pin/unpin/remove task actions; explicit method signatures with all required parameters; XML doc comments mapping each method to Phase 3 command type; no "..." ellipsis.
- [ ] **Must exclude:** Command implementation, StateStore access details.

### 5B - Implement UICommandDispatcher with command construction

- [ ] **Action:** Create `UI/Commands/UICommandDispatcher.cs` implementing IUICommandDispatcher. Each dispatch method constructs the corresponding command (e.g., DispatchCompleteTask constructs CompleteTaskCommand), retrieves StateStore reference (from DI or singleton), and executes command. Inject or pass StateStore as dependency.
- [ ] **Scope:** `UI/Commands/UICommandDispatcher.cs` (new file; public class `UICommandDispatcher : IUICommandDispatcher`).
- [ ] **Verify:** All dispatch methods construct correct commands; StateStore reference is available at dispatch time; command execution succeeds without throwing.
- [ ] **Suggested commit:** `phase4(step5B): implement UICommandDispatcher with command routing`
- [ ] **Must include:** Implementation of IUICommandDispatcher methods; command construction and dispatch logic; StateStore integration.
- [ ] **Must exclude:** UI rendering, error handling UI feedback (defer error handling to later phase).

### 5C - Wire IUICommandDispatcher into HudViewModel and TaskListViewModel

- [ ] **Action:** Inject IUICommandDispatcher into HudViewModel and TaskListViewModel constructors. Add public methods (e.g., `MarkTaskComplete(TaskId id)`, `PinTask(TaskId id)`) that call dispatcher methods. Document that these methods are called by UI event handlers (click, etc.) and route user actions to commands.
- [ ] **Scope:** `UI/ViewModels/HudViewModel.cs` and `UI/ViewModels/TaskListViewModel.cs` (dependency injection; public command methods).
- [ ] **Verify:** Constructor accepts IUICommandDispatcher; public methods are callable; dispatch methods are invoked without error.
- [ ] **Suggested commit:** `phase4(step5C): wire IUICommandDispatcher into view models`
- [ ] **Must include:** Dependency injection of IUICommandDispatcher; public methods that route to dispatcher; no direct command construction in view models.
- [ ] **Must exclude:** UI event handler implementation, rendering.

### 5D - Add unit tests for command dispatch flow

- [ ] **Action:** In `Tests/UI/Commands/UICommandDispatcherTests.cs`, add test cases: (1) DispatchCompleteTask constructs and dispatches CompleteTaskCommand correctly, (2) DispatchPinTask constructs and dispatches PinTaskCommand, (3) dispatch methods do not mutate UI state directly (mock StateStore and verify invocations). Use Moq to mock StateStore and verify command dispatch calls.
- [ ] **Scope:** `Tests/UI/Commands/UICommandDispatcherTests.cs` (new file; test class with dispatch method test cases).
- [ ] **Verify:** All dispatch test cases pass; commands are constructed with correct arguments; StateStore is invoked as expected; no direct UI mutation.
- [ ] **Suggested commit:** `phase4(step5D): add unit tests for UICommandDispatcher`
- [ ] **Must include:** Test cases for command construction and dispatch; mocked StateStore; assertions on dispatcher behavior.
- [ ] **Must exclude:** Production logic changes.

### Step 5 Completion Checkpoint

- [ ] All substeps in Step 5 complete (5A, 5B, 5C, 5D).

---

## Step 6: Resolve TaskSourceType Terminology Ambiguity (DEF-007)

### Step Goal

- [ ] **Document, standardize, and resolve in-phase TaskSourceType vs SourceIdentifier ambiguity.** Center the distinction between SourceIdentifier (unique source context identifier) and TaskSourceType (categorical source classification) in Phase 4 design. Establish consistent naming across domain models, commands, and UI. **Explicit Phase 4 scope inclusions**: resolve SourceIdentifier-vs-TaskSourceType naming in TaskRecord, snapshot projections, and UI view model properties. **Explicit Phase 4 exclusions**: no changes to TaskId format, no persistence schema changes, no config version migration logic, no command/snapshot boundary modifications. **Post-Phase Handling**: if full legacy inconsistency cleanup (across all prior Phase 1-3 code) is outside Phase 4 scope, explicitly defer to post-phase `Phase 4 Implementation Review Report` with route to user-owned `Phase 4 - Post-Phase Atomic Commit Execution Checklist` rather than attempting incomplete inline fixes.

### 6A - Document SourceIdentifier and TaskSourceType conceptual distinction

- [ ] **Action:** Create or update `Domain/Identifiers/SourceIdentifier.md` (documentation comment or separate doc) clarifying: SourceIdentifier is the canonical value type that uniquely identifies a rule/directive source (manual, mod, game event, etc.); TaskSourceType is the enum encoding source category (Manual, Mod, GameEvent, etc.) used for filtering and display. SourceIdentifier may include source-specific context (rule ID, event name); TaskSourceType is categorical. Document examples of each and their relationship. Do NOT change any existing domain types, only document distinction.
- [ ] **Scope:** Documentation within `Domain/Identifiers/` subdirectory or inline comments in SourceIdentifier.cs/TaskSourceType.cs; no code changes.
- [ ] **Verify:** Documentation is clear and unambiguous; examples distinguish SourceIdentifier from TaskSourceType; no code changes required.
- [ ] **Suggested commit:** `phase4(step6A): document SourceIdentifier vs TaskSourceType terminology`
- [ ] **Must include:** Clear definitions of SourceIdentifier and TaskSourceType; examples of each; documented relationship.
- [ ] **Must exclude:** Code changes to TaskId format, persistence schema, or config version logic.

### 6B - Audit existing code for terminology consistency

- [ ] **Action:** Review codebase (Phase 1-3 code + Domain models) for usage of "source", "SourceIdentifier", "TaskSourceType", "source type", or similar terms. Document current usage patterns. Identify any inconsistent naming (e.g., "source" used as shorthand when TaskSourceType intended, or vice versa). Create a mapping of current usage to standardized terminology.
- [ ] **Scope:** Code review and documentation of existing usage; no code changes.
- [ ] **Verify:** Audit is complete and thorough; mapping covers all occurrences; inconsistencies are clearly noted.
- [ ] **Suggested commit:** `phase4(step6B): audit terminology usage for consistency`
- [ ] **Must include:** Complete inventory of current usage; identified inconsistencies; standardized terminology mapping.
- [ ] **Must exclude:** Code refactoring (defer to post-phase remediation if needed).

### 6C - Create terminology baseline in ManualTask\_{N} context

- [ ] **Action:** In view models (HudViewModel, TaskListViewModel) and UI command methods, use consistent terminology in comments and property names. When referencing source of a task, use `TaskSourceType` for the enum/category; when referencing unique source identifier, use `SourceIdentifier`. Update any property names or comments to reflect this distinction. Example: `TaskList items should show SourceType property (TaskSourceType enum) by source category; detailed view shows SourceIdentifier for identification.` Document this baseline in code comments.
- [ ] **Scope:** `UI/ViewModels/HudViewModel.cs`, `UI/ViewModels/TaskListViewModel.cs`, `UI/ViewModels/TaskItemViewModel.cs` (property definitions and comments).
- [ ] **Verify:** Property naming and comments consistently use TaskSourceType and SourceIdentifier; terminology matches documentation from step 6A.
- [ ] **Suggested commit:** `phase4(step6C): establish terminology baseline in UI view models`
- [ ] **Must include:** Consistent property/comment naming; terminology alignment with documentation; ManualTask\_{N} wording baseline notes.
- [ ] **Must exclude:** Domain type changes, persistence schema changes, config version changes.

### Step 6 Completion Checkpoint

- [ ] All substeps in Step 6 complete (6A, 6B, 6C).

---

## Step 7: Add Phase 4 Verification Tests

### Step Goal

- [ ] Add comprehensive unit tests locking Phase 4 constraints, testing snapshot subscription lifecycle, view model binding, collection reconciliation, and command dispatch without running the game.

### 7A - Add UISnapshotSubscriptionManager lifecycle tests

- [ ] **Action:** In `Tests/UI/UISnapshotSubscriptionManagerTests.cs`, add test cases: (1) Subscribe registers callback with StateStore.SnapshotChanged, (2) callback is invoked when snapshot changes, (3) dispose on subscription token unsubscribes only that specific subscriber (not all subscribers), (4) repeated Subscribe/Dispose cycles work correctly, (5) Subscribe before StateStore initialized does not throw. **Explicitly verify per-subscriber token disposal**: When subscriber A and subscriber B both call Subscribe, and subscriber A calls Dispose on its returned token, subscriber B's callback remains registered and continues to receive snapshots. Use mock IEventDispatcher or real StateStore (as design allows).
- [ ] **Scope:** `Tests/UI/UISnapshotSubscriptionManagerTests.cs` (new file; test class with subscription lifecycle test methods).
- [ ] **Verify:** All test cases pass; subscription lifecycle is deterministic; edge cases (pre-init calls) are handled gracefully.
- [ ] **Suggested commit:** `phase4(step7A): add UISnapshotSubscriptionManager lifecycle tests`
- [ ] **Must include:** Test cases for subscribe, unsubscribe, callback invocation, edge cases; deterministic behavior assertions.
- [ ] **Must exclude:** Production logic changes, UI rendering.

### 7B - Add HudViewModel and TaskListViewModel binding tests

- [ ] **Action:** In `Tests/UI/ViewModels/HudViewModelTests.cs` and `Tests/UI/ViewModels/TaskListViewModelTests.cs`, add test cases: (1) OnSnapshotChanged updates properties correctly, (2) PropertyChanged event is raised for updated properties, (3) bindings can subscribe to PropertyChanged and receive notifications, (4) collection items are reconciled deterministically. Use fixtures with known TaskSnapshot data.
- [ ] **Scope:** `Tests/UI/ViewModels/HudViewModelTests.cs` and `Tests/UI/ViewModels/TaskListViewModelTests.cs` (binding test methods).
- [ ] **Verify:** All binding test cases pass; property updates and notifications are correct; PropertyChanged is raised for all modified properties.
- [ ] **Suggested commit:** `phase4(step7B): add view model binding and property change tests`
- [ ] **Must include:** Test cases for property updates, PropertyChanged notifications, binding subscriptions; deterministic update verification.
- [ ] **Must exclude:** UI rendering, user input handling.

### 7C - Add snapshot-to-UI boundary tests

- [ ] **Action:** In `Tests/UI/UIBoundaryTests.cs` (new or existing), add integration test cases: (1) snapshot changes are projected to view model properties without mutation, (2) view model command methods dispatch to IUICommandDispatcher, (3) no view model method writes directly to StateStore or canonical state. Mock StateStore and commands; verify isolation.
- [ ] **Scope:** `Tests/UI/UIBoundaryTests.cs` (new file or update existing; boundary test methods).
- [ ] **Verify:** Boundary tests pass; isolation is maintained (no direct state mutation from ViewModels); snapshot projection is unidirectional.
- [ ] **Suggested commit:** `phase4(step7C): add snapshot-to-UI boundary isolation tests`
- [ ] **Must include:** Test cases for snapshot projection isolation; command dispatch verification; no-direct-state-mutation assertions.
- [ ] **Must exclude:** Production refactoring, UI rendering.

### 7D - Verify Phase 4 constraints in test suite

- [ ] **Action:** Add summary test method (e.g., `Phase4ConstraintSummaryTests.cs`) with test case assertions: (1) UIViewModelBase is abstract and implements INotifyPropertyChanged, (2) HudViewModel and TaskListViewModel inherit from UIViewModelBase, (3) all view model properties use [ObservableProperty] annotation or manual OnPropertyChanged calls, (4) UICommandDispatcher exists and implements IUICommandDispatcher, (5) no view model class contains direct StateStore or command handler references (verify via code inspection or reflection tests). Document these as guardrails verification.
- [ ] **Scope:** `Tests/UI/Phase4ConstraintSummaryTests.cs` (new file; constraint verification test methods).
- [ ] **Verify:** All constraint test cases pass; guardrails from section "Guardrails (Must Stay True)" are verified in code.
- [ ] **Suggested commit:** `phase4(step7D): add Phase 4 constraint verification tests`
- [ ] **Must include:** Test assertions for all guardrails; architectural boundary checks; reflection or inspection-based verification if needed.
- [ ] **Must exclude:** Production changes, UI rendering.

### Step 7 Completion Checkpoint

- [ ] All substeps in Step 7 complete (7A, 7B, 7C, 7D).

---

## Step 8: Phase 4 Completion Gate

### Step Goal

- [ ] Verify Phase 4 implementation is complete, deterministic, and contract-aligned. Validate all guardrails. Reconcile deferments. Gate readiness for Phase 5.

### 8A - Run clean build and full test suite

- [ ] **Action:** Run `dotnet clean JojaAutoTasks.sln -c Debug` followed by `dotnet restore JojaAutoTasks.sln` and `dotnet build JojaAutoTasks.csproj -c Debug -p:EnableModDeploy=false -p:EnableModZip=false`. Then run `dotnet test "Tests\JojaAutoTasks.Tests.csproj"`. Verify no compilation errors, no build warnings, and all Phase 1-4 tests pass.
- [ ] **Scope:** No source edits; clean build and test run only.
- [ ] **Verify:** Build succeeds without errors or warnings; all tests pass (target: 110+ passed from prior phases + ~50-60 new Phase 4 tests = ~160-170 total passed, accounting for full coverage of UISnapshotSubscriptionManager, HudViewModel, TaskListViewModel, HudTaskRowViewModel, TaskDetailViewModel, HistoryViewModel, ManualTaskEditorViewModel, ConfigViewModel, UICommandDispatcher, and snapshot/command boundary tests).
- [ ] **Suggested commit:** `phase4(step8A): record clean build and full test suite completion`
- [ ] **Must include:** Build log output showing successful compilation; test summary showing all tests passed.
- [ ] **Must exclude:** Code changes, opportunistic refactoring.

### 8B - Audit guardrails against implementation

- [ ] **Action:** Manually review implementation against each guardrail from the "Guardrails" section: (1) Command/Snapshot Boundary Integrity — verify no view model mutates StateStore/StateContainer directly, all writes route through IUICommandDispatcher; (2) Deterministic Reconciliation — verify TaskListViewModel collection reconciliation uses stable key (TaskId) and sorted iteration; (3) Subscription Lifecycle — verify subscribe in **init**, unsubscribe in Dispose; (4) No Optimistic Mutation — verify UI never writes canonical state; (5) ManualTask\_{N} Terminology — verify all properties and comments use consistent terminology; (6) DEF-032 Exception-Only Boundary — verify ConfigLoader catch paths contain only exception handling, fallback, logging (no schema/migration/feature logic). Document guardrail verification in checklist or audit notes.
- [ ] **Scope:** Code review against guardrails; this checklist file; implementation files (UIViewModelBase, HudViewModel, TaskListViewModel, UICommandDispatcher, ConfigLoader).
- [ ] **Verify:** Each guardrail is preserved in implementation; no violations detected; documentation is clear.
- [ ] **Suggested commit:** `phase4(step8B): audit and confirm guardrails preserved in Phase 4`
- [ ] **Must include:** Explicit check against each guardrail; verification notes or audit log; no guardrail violations.
- [ ] **Must exclude:** Code changes, Phase 5 work.

### 8C - Validate Phase 4 scope boundaries

- [ ] **Action:** Review all files changed during Phase 4 (new files and edits to existing files) to confirm scope matches Phase 4 requirements: UI foundation (view models, subscription, binding), ConfigLoader hardening, command dispatch. Verify no scope expansion into Phase 5 (HUD/menu rendering), Phase 6 (click handlers/input), or unplanned areas. Generate file change summary (use git diff or manual review). Verify all deferments in Phase 4 (DEF-007, 008, 009, 010, 032) are addressed.
- [ ] **Scope:** Code review and file inventory; this checklist; git log or change summary.
- [ ] **Verify:** File changes are within Phase 4 scope; no unintended Phase 5/6 work; all 5 in-phase deferments are covered.
- [ ] **Suggested commit:** `phase4(step8C): finalize scope validation and deferment coverage`
- [ ] **Must include:** File change inventory; scope alignment verification; deferment coverage check.
- [ ] **Must exclude:** Out-of-scope code changes, Phase 5 work.

### 8D - Reconcile deferments with Deferments Index and document findings

- [ ] **Action:** Review `Project/Tasks/Implementation Plan/Deferments Index.md` for any deferments (DEF-NNN) scheduled for Phase 4 or marked "Open" with Phase 4 scope. Match against Phase 4 checklist: DEF-007 (terminology ambiguity — addressed in Step 6), DEF-008 (snapshot subscription lifecycle — addressed in Step 3), DEF-009 (INPC/reconciliation — addressed in Step 4), DEF-010 (UI-local state ownership — addressed in Step 5), DEF-032 (ConfigLoader exception hardening — addressed in Step 2). **Implementer responsibility**: record all findings (analysis notes, reconciliation decisions, unresolved questions) in local audit notes or review artifact. Move resolved deferments from Index to `Project/Tasks/Implementation Plan/Deferments Archive.md` with: Resolved In Phase (Phase 4), Archived Date (YYYY-MM-DD), Resolution Notes (brief summary). If new deferments discovered during Phase 4 (identified in this audit), append to Deferments Index with next sequential DEF-NNN ID. **Gate/Reviewer responsibility:** If architecture/scope issues are found during this gate, reviewer + GodAgent draft `Project/Planning/Phase 4 Implementation Review Report.md` (new file) documenting findings and deferment recommendations. Gate does **not** implement fixes inline; instead, issues are routed to user-owned `Project/Tasks/Implementation Plan/Phase 4 - Post-Phase Atomic Commit Execution Checklist.md` for prioritized remediation.
- [ ] **Scope:** `Project/Tasks/Implementation Plan/Deferments Index.md`, `Project/Tasks/Implementation Plan/Deferments Archive.md`, Phase 4 checklist review, and optional post-phase review report and execution checklist (user-triggered).
- [ ] **Verify:** All in-phase deferments (DEF-007, 008, 009, 010, 032) are mapped to Phase 4 steps; resolved deferments are moved to Archive with context; implementer findings are recorded; any new deferments are appended to Index with sequential DEF-NNN ID; if issues found, review documentation is drafted with clear post-phase routing.
- [ ] **Suggested commit:** `phase4(step8D): reconcile deferments, record findings, and prepare post-phase routing`
- [ ] **Must include:** Mapping of DEF-007/008/009/010/032 to Phase 4 steps; deferment moves to Archive with resolution context; implementer findings/audit notes recorded; any new DEF-NNN entries in Index; if issues found, review report drafted with post-phase routing (not inline fixes).
- [ ] **Must exclude:** Retroactive edits to prior phase deferments without explicit justification; inline fixes for architectural issues (route to post-phase instead).

### Step 8 Completion Checkpoint

- [ ] All substeps in Step 8 complete (8A, 8B, 8C, 8D).

---

## Blocking Issues at Gate (Critical Path)

**If blocking issues are identified during Step 8 gates:**

- Every blocking constraint violation must have a documented resolution path
- Issues must be either resolved before Phase 4 close or explicitly deferred with DEF-NNN entry and post-phase routing
- Reviewer: draft Phase 4 Implementation Review Report if architectural issues surface; do not attempt inline fixes

---

## Final Completion Gate Checklist

- [ ] All substeps 1A through 8D are complete.
- [ ] Implementation scope matches Phase 4 requirements (no unintended expansion into Phase 5/6).
- [ ] All guardrails are preserved and verified in code.
- [ ] Unit tests pass; Phase 1-4 test suite is comprehensive and deterministic (~160-170 total tests).
- [ ] Build succeeds without errors or warnings.
- [ ] All five in-phase deferments (DEF-007, 008, 009, 010, 032) are addressed:
  - DEF-007: Terminology ambiguity resolved (Step 6)
  - DEF-008: Snapshot subscription lifecycle implemented (Step 3)
  - DEF-009: INPC/reconciliation implemented (Step 4)
  - DEF-010: UI-local state ownership established (Step 5)
  - DEF-032: ConfigLoader exception hardening completed (Step 2)
- [ ] Deferments reconciled: resolved deferments moved to Archive with phase/date/notes; any new deferments added to Index with sequential DEF-NNN ID.
- [ ] If scope or architecture issues found, review report drafted and post-phase routing documented (user-owned `Phase 4 - Post-Phase Atomic Commit Execution Checklist.md` for remediation).
- [ ] Phase 4 is ready for Phase 5 (HUD/Menu UI Rendering) to begin.

---

## Post-Phase Artifact Routing (If Findings Discovered)

**If scope/architecture issues are discovered during Step 8D (Deferment Reconciliation) or earlier gates:**

1. **Create Review Report:** `Project/Planning/Phase 4 Implementation Review Report.md`
   - Section: Issue Analysis (categorized by severity: blocking, important, minor)
   - Section: Remediation Guidance (recommended fixes, complexity level, phase for attempted resolution)
   - Section: Evidence (file paths, line number citations, test failures, constraint violations)

2. **Draft Post-Phase Checklist:** `Project/Tasks/Implementation Plan/Phase 4 - Post-Phase Atomic Commit Execution Checklist.md`
   - User-owned execution checklist for addressing issues identified in Phase 4 review
   - Do NOT implement fixes inline in Phase 4 completion gate
   - Route to user for prioritization and scheduling in post-phase atomic execution cycle

3. **Deferment Index Update:** Append new deferments from Phase 4 to `Deferments Index.md` with DEF-NNN ID and phase/scope context

**Note:** This phase assumes clean completion (no blocking issues). If issues are found, this routing ensures structured documentation and user control over post-phase remediation.

---

## Execution Notes

- **Deterministic Architecture:** Maintain snapshot-read, projection-to-bindable-properties, command-dispatch pattern throughout. No optimistic mutation.
- **Testable Without Game:** All Phase 4 components compile and test in isolation; no Stardew Valley runtime required.
- **Atomic Commits:** Commit after each substep (1A, 1B, 1C, etc.) for fine-grained history and easy rollback.
- **Configuration Constraints:** DEF-032 exception hardening must not expand config schema or introduce feature-gated logic.
- **Terminology Baseline:** All new code uses ManualTask\_{N} wording and SourceIdentifier/TaskSourceType distinction consistently.

---

## Implementation Notes -- Reviewer Action Required

**Open questions for Reviewer:**

1. Step 1A says to name the new subscription manager `UISnapshotSubscriptionManager`,  
   however, [C# Contract](/.github/instructions/csharp-style-contract.instructions.md)  
   says `Manager` name is to be avoided without justification. Is `UiSnapshotSubscriptionManager`  
   acceptable given the UI-specific nature, or should we consider an alternative name
   like `UiSnapshotSubscriptionCoordinator` given it is brokering subscriptions?
   Let's discuss this post-review.

**End of Phase 4 - Atomic Commit Execution Checklist**
