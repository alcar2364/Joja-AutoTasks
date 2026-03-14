<!-- markdownlint-disable MD013 MD031 MD032 MD040 -->

I have created the following plan after thorough exploration and analysis of the codebase. Follow the below plan verbatim. Trust the files and references. Do not re-verify what's written in the plan. Explore only when absolutely necessary. First implement all the proposed file changes and then I'll review all the changes together at the end.

## Observations

The codebase uses a command/snapshot pattern: `StateStore.Handle()` dispatches to typed handlers, increments `StateContainer.Version`, then calls `SnapshotProjector.Project(_stateContainer)` and fires `SnapshotChanged`. `ExpirationDetector` is the existing `DayBoundary` static-method class to mirror. `UiSnapshotSubscriptionManager` (namespace `JojaAutoTasks.Ui`) is the exact template for the toast manager. `StateStore.OnDayStarted(DayKey)` already exists and must be updated. `LifecycleCoordinator.HandleDayStarted()` currently takes no parameters — it must gain `DayKey` + `int currentTime`. The `EventDispatcherTests` IL-inspection test will need `DispatchTimeChanged` to remain a no-op body.

## Approach

Changes are ordered by dependency: new model types first, then the evaluator, then command/handler changes, then `StateStore` (which depends on all of the above), then `SnapshotProjector`, then the event/lifecycle layer, then `ModEntry`, and finally the UI subscription manager. This bottom-up order ensures each file compiles cleanly before the next layer references it.

---

## Implementation Steps

- [x] Step 1 — Create `ToastType` enum (`file:State/ToastType.cs`)

Create a new file `State/ToastType.cs`. Define `ToastType` as an `internal enum` with one member: `TaskAutoCompleted = 0`. Follow the namespace `JojaAutoTasks.State`.

---

- [ ] Step 2 — Create `ToastEvent` sealed class (`file:State/ToastEvent.cs`)

Create `State/ToastEvent.cs`. Define `ToastEvent` as an `internal sealed class` (matching the sealed-class convention used by `TaskRecord`, `CompleteTaskCommand`, etc.) with:

- `internal ToastType Type { get; }`
- `internal string TaskTitle { get; }`
- A constructor accepting both fields with null-guard on `TaskTitle`.

Namespace: `JojaAutoTasks.State`.

---

- [ ] Step 3 — Create `DeadlineStoredFields` sealed class (`file:State/Models/DeadlineStoredFields.cs`)

Create `State/Models/DeadlineStoredFields.cs`. Define `internal sealed class DeadlineStoredFields` with:

- `internal DayKey DueDayKey { get; }`
- `internal int? ExpiresAtTime { get; }`
- A constructor accepting both fields; guard that `DueDayKey != default`.

Namespace: `JojaAutoTasks.State.Models`. Using: `JojaAutoTasks.Domain.Identifiers`.

---

- [ ] Step 4 — Create `DeadlineFields` sealed class (`file:State/Models/DeadlineFields.cs`)

Create `State/Models/DeadlineFields.cs`. Define `internal sealed class DeadlineFields` with:

- `internal DayKey DueDayKey { get; }`
- `internal int? ExpiresAtTime { get; }`
- `internal int DaysRemaining { get; }`
- `internal bool IsOverdue { get; }`
- `internal bool IsWindowClosed { get; }`
- A constructor accepting all five fields.

Namespace: `JojaAutoTasks.State.Models`. Using: `JojaAutoTasks.Domain.Identifiers`.

---

- [ ] Step 5 — Create `DeadlineEvaluator` static class (`file:State/DayBoundary/DeadlineEvaluator.cs`)

Create `State/DayBoundary/DeadlineEvaluator.cs`. Define `internal static class DeadlineEvaluator` with one method:

```
internal static DeadlineFields Evaluate(
    DeadlineStoredFields stored,
    DayKey currentDay,
    int currentTime) → DeadlineFields
```

Derivation logic (no code, just rules):

- `DaysRemaining` = `stored.DueDayKey.ToSequenceNumber() - currentDay.ToSequenceNumber()`
- `IsOverdue` = `currentDay.ToSequenceNumber() > stored.DueDayKey.ToSequenceNumber()`
- `IsWindowClosed` = `currentDay == stored.DueDayKey AND stored.ExpiresAtTime != null AND currentTime >= stored.ExpiresAtTime.Value`

Namespace: `JojaAutoTasks.State.DayBoundary`. Using: `JojaAutoTasks.Domain.Identifiers`, `JojaAutoTasks.State.Models`.

---

- [ ] Step 6 — Add bootstrap guard policy seam

Create `State/BootstrapGuardPolicy.cs`. Define `internal enum BootstrapGuardPolicy` with three members:

- `Release` — no-op + one deduped warning per session
- `Debug` — throw `InvalidOperationException` fail-fast
- `DebugDiagnostic` — warn on every violating call

This enum is the runtime policy seam that tests can set to validate each mode in one suite.

---

- [ ] Step 7 — Update `CompleteTaskCommand` (`file:State/Commands/CompleteTaskCommand.cs`)

Add `internal bool IsPlayerInitiated { get; }` to the class. Update the constructor to accept `bool isPlayerInitiated = false` as a third parameter (with default). Assign it in the constructor body. No changes to existing `TaskId` or `CompletionDay` fields or their guards.

> **Impact on existing tests:** `SnapshotPublishingTests` constructs `new CompleteTaskCommand(taskId, completionDay)` — the default parameter means these calls remain valid without modification.

---

- [ ] Step 8 — Update `TaskRecord` (`file:State/Models/TaskRecord.cs`)

Add a new field under the `// -- Command-Specific Fields -- //` section (or a new `// -- Deadline Fields -- //` section):

- `internal DeadlineStoredFields? DeadlineStoredFields { get; set; }`

Add `DeadlineStoredFields? deadlineStoredFields = null` as an optional parameter at the end of the constructor, and assign it. Existing callers that omit it will default to `null` (no deadline).

> **Impact on existing tests:** `DayBoundaryTests.CreateTaskRecord(...)` constructs `TaskRecord` directly — add the optional parameter with default `null` so existing call sites compile without change.

---

- [ ] Step 9 — Update `TaskView` (`file:State/Models/TaskView.cs`)

Add a new property:

- `internal DeadlineFields? DeadlineFields { get; }`

Add `DeadlineFields? deadlineFields = null` as an optional parameter at the end of the constructor, and assign it. Existing callers that omit it will default to `null`.

---

- [ ] Step 10 — Update `SnapshotProjector` (`file:State/SnapshotProjector.cs`)

Change the `Project` signature to:

```
internal static TaskSnapshot Project(StateContainer stateContainer, DayKey currentDay, int currentTime)
```

In the `Select` projection lambda, after mapping all existing fields, add:

- If `record.DeadlineStoredFields != null`, call `DeadlineEvaluator.Evaluate(record.DeadlineStoredFields, currentDay, currentTime)` and pass the result as `deadlineFields` to the `TaskView` constructor.
- Otherwise pass `null`.

Add usings for `JojaAutoTasks.Domain.Identifiers` and `JojaAutoTasks.State.DayBoundary`.

---

- [ ] Step 11 — Update `StateStore` (`file:State/StateStore.cs`)

This is the most complex change. Apply in this order:

- [ ] **11a — Add fields:**
- `private DayKey _currentDayKey;`
- `private int _currentTime;`
- `private bool _sessionActive;`
- `private bool _timeContextInitialized;`
- `private bool _bootstrapWarnEmitted;` (for release-mode dedup)
- `private BootstrapGuardPolicy _bootstrapGuardPolicy = BootstrapGuardPolicy.Release;` (default; tests override via an `internal` setter or constructor parameter)

- [ ] **11b — Add `ToastRequested` event:**

```
public event Action<ToastEvent>? ToastRequested;
```

- [ ] **11c — Add `internal void SetBootstrapGuardPolicy(BootstrapGuardPolicy
policy)` for test seam.**

- [ ] **11d — Add `internal void InitializeTimeContext(DayKey currentDay, int currentTime)`:**
- Set `_currentDayKey = currentDay`, `_currentTime = currentTime`, `_timeContextInitialized = true`, `_bootstrapWarnEmitted = false` (reset dedup for new session).

- [ ] **11e — Update `OnDayStarted` signature to `internal void
OnDayStarted(DayKey newDay, int currentTime)`:**
- Call `InitializeTimeContext(newDay, currentTime)` first.
- Then run the existing expiration detection and removal logic.
- When calling `SnapshotProjector.Project(...)`, pass `_currentDayKey` and `_currentTime`.
- Set `_sessionActive = true`.

- [ ] **11f — Add `internal void OnTimeChanged(DayKey currentDay, int currentTime)`:**
- If `!_sessionActive`: return (session gate).
- If `currentDay == _currentDayKey && currentTime == _currentTime`: return (dedupe).
- Update `_currentDayKey = currentDay`, `_currentTime = currentTime`.
- Do NOT call `SnapshotProjector.Project` or fire `SnapshotChanged` here.

- [ ] **11g — Update `OnSaveLoaded`:**
- Set `_sessionActive = true`.
- Call `InitializeTimeContext` with a sentinel/default if no time is available yet (or leave for `LifecycleCoordinator` to call explicitly — per the spec, lifecycle calls `InitializeTimeContext` during save-load).

- [ ] **11h — Update `OnReturnToTitle`:**
- Set `_sessionActive = false`, `_timeContextInitialized = false`, `_bootstrapWarnEmitted = false`.
- Keep existing `_stateContainer.Clear()` and `_manualTaskCounter.Reset()`.

- [ ] **11i — Add bootstrap guard helper method** `private void GuardTimeContextInitialized()`:
- If `_timeContextInitialized`: return.
- Switch on `_bootstrapGuardPolicy`:
  - `Debug`: throw `InvalidOperationException("Projection called before time context was initialized.")`
  - `DebugDiagnostic`: log a warning on every call (inject a logger or use a static warning action — follow existing `ModLogger` pattern; since `StateStore` currently has no logger, add `ModLogger? _logger` as an optional constructor parameter or use a static warning delegate seam)
  - `Release`: if `!_bootstrapWarnEmitted`, emit one warning and set `_bootstrapWarnEmitted = true`; then return (no-op)

> **Note on logger in StateStore:** The existing `StateStore` has no logger. For the bootstrap guard warning, introduce a `private Action<string>? _warnAction` field that can be injected via an `internal void SetWarnAction(Action<string> warn)` method. Tests inject a recording action; production code passes `logger.Warn(...)` from `LifecycleCoordinator` or `BootstrapContainer`. This avoids adding a full `ModLogger` dependency to `StateStore`.

- [ ] **11j — Update `Handle()` for `CompleteTaskCommand` toast emission:**

In the `switch` block, replace the `CompleteTaskCommand` case with:

- [ ] Capture `priorStatus` by calling
      `_stateContainer.TryGet(completeTaskCommand.TaskId, out var existingRecord)`
      before handler execution and reading `existingRecord?.Status`.
- [ ] Call `_completeTaskHandler.Handle(completeTaskCommand, _stateContainer)`.
- [ ] After handler, if `_stateContainer.Version != priorVersion` (state
      changed) AND `priorStatus == TaskStatus.Incomplete` AND
      `completeTaskCommand.IsPlayerInitiated == false`:
  - [ ] Retrieve the updated record to get its `Title`.
  - [ ] Fire `ToastRequested?.Invoke(new
ToastEvent(ToastType.TaskAutoCompleted, title))`.
- [ ] Then fall through to the existing version-check that fires `SnapshotChanged`.

The `SnapshotChanged` invocation must pass `_currentDayKey` and `_currentTime` to `SnapshotProjector.Project`.

- [ ] **11k — Update all `SnapshotProjector.Project(...)` call sites in
      `StateStore`** to pass `_currentDayKey` and `_currentTime`. There are two:
      inside `Handle()` and inside `OnDayStarted()`.

---

- [ ] Step 12 — Add `DispatchTimeChanged` to `IEventDispatcher` and `EventDispatcher`

**`file:Events/IEventDispatcher.cs`:** Add:

```
void DispatchTimeChanged(DayKey currentDay, int currentTime);
```

Add `using JojaAutoTasks.Domain.Identifiers;`.

**`file:Events/EventDispatcher.cs`:** Add the implementation as a no-op body (empty method body — matching the existing pattern of all other dispatch methods). This preserves the IL-inspection test's `nop`/`ret`-only requirement.

> **Impact on `EventDispatcherTests`:** The `DispatchMethods_HaveNoOperationalIlBeyondNopAndReturn` test uses `[Theory][InlineData(...)]` — add `[InlineData(nameof(IEventDispatcher.DispatchTimeChanged))]` to that test's data set. The `DispatcherType_HasNoInstanceFields` test will still pass since no fields are added.

---

- [ ] Step 13 — Update `LifecycleCoordinator` (`file:Lifecycle/LifecycleCoordinator.cs`)

- [ ] **13a — Add session-active tracking field:**
- `private bool _sessionActive;`

- [ ] **13b — Update `HandleSaveLoaded()`:**
- Set `_sessionActive = true`.
- After `_eventDispatcher.DispatchSaveLoaded()` and `_stateStore.OnSaveLoaded()`, call `_stateStore.InitializeTimeContext(currentDay, currentTime)`. Since SMAPI provides the current day/time at `SaveLoaded`, accept `DayKey currentDay, int currentTime` as parameters to `HandleSaveLoaded`, or read them from a helper. Per the spec, lifecycle passes them in — update the signature to `internal void HandleSaveLoaded(DayKey currentDay, int currentTime)` and forward to `_stateStore.InitializeTimeContext(currentDay, currentTime)`.

- [ ] **13c — Update `HandleDayStarted()`:**
- Accept `DayKey newDay, int currentTime` parameters.
- Call `_eventDispatcher.DispatchDayStarted()`.
- Call `_stateStore.OnDayStarted(newDay, currentTime)` (updated signature from Step 11e).
- Set `_sessionActive = true`.

- [ ] **13d — Update `HandleReturnedToTitle()`:**
- Set `_sessionActive = false`.
- Keep existing dispatcher and store calls.

- [ ] **13e — Add `internal void HandleTimeChanged(DayKey currentDay, int currentTime)`:**
- If `!_sessionActive`: return (session gate).
- Call `_eventDispatcher.DispatchTimeChanged(currentDay, currentTime)`.
- Call `_stateStore.OnTimeChanged(currentDay, currentTime)`.

---

- [ ] Step 14 — Update `ModEntry` (`file:ModEntry.cs`)

- [ ] **14a — Register `TimeChanged` hook** in `Entry()`:

```csharp
helper.Events.GameLoop.TimeChanged += OnTimeChanged;
```

- [ ] **14b — Add `OnTimeChanged` handler:**
- Extract `DayKey` from `Game1.Date` (using `DayKeyFactory.Create(Game1.year, Game1.currentSeason, Game1.dayOfMonth)` or the existing `DayKeyFactory` pattern).
- Extract `int currentTime` from `Game1.timeOfDay`.
- Forward to `_runtime.LifecycleCoordinator.HandleTimeChanged(currentDay, currentTime)`.

- [ ] **14c — Update `OnSaveLoaded`:**
- Extract `DayKey` and `currentTime` from game state.
- Forward to `_runtime.LifecycleCoordinator.HandleSaveLoaded(currentDay, currentTime)`.

- [ ] **14d — Update `OnDayStarted`:**
- Extract `DayKey` and `currentTime` from game state.
- Forward to `_runtime.LifecycleCoordinator.HandleDayStarted(newDay, currentTime)`.

- [ ] **14e — Initialize `UiToastSubscriptionManager`** alongside `UiSnapshotSubscriptionManager`:

```csharp
UiToastSubscriptionManager.Initialize(_runtime.StateStore);
```

Add a `_toastSubscriptionToken` field of type `IDisposable?`. Subscribe with a no-op callback (matching the existing snapshot subscription pattern). Dispose in `OnSaving` alongside `_snapshotSubscriptionToken`.

---

- [ ] Step 15 — Create `UiToastSubscriptionManager` (`file:UI/UiToastSubscriptionManager.cs`)

Mirror `file:UI/UiSnapshotSubscriptionManager.cs` exactly:

- Same namespace: `JojaAutoTasks.Ui`
- Same static class pattern
- Same `SnapshotSubscription` / `NoOpSubscription` inner classes (renamed to `ToastSubscription` / `NoOpSubscription`)
- `private static StateStore? _stateStore;`
- `internal static void Initialize(StateStore stateStore)`
- `public static IDisposable Subscribe(Action<ToastEvent> callback)` — subscribes to `_stateStore.ToastRequested`; unsubscribes on `Dispose()`

Using: `JojaAutoTasks.State`.

---

- [ ] Step 16 — Update `DayKeyFactory` usage in `ModEntry`

Verify that `DayKeyFactory` has a `Create(int year, string season, int day)` overload (it already exists per `DayBoundaryTests`). Use it in `ModEntry` to construct `DayKey` from `Game1.year`, `Game1.currentSeason`, and `Game1.dayOfMonth`.

---

## Data flow summary

```mermaid
sequenceDiagram
    participant SMAPI
    participant ModEntry
    participant LC as LifecycleCoordinator
    participant Store as StateStore
    participant Projector as SnapshotProjector
    participant Evaluator as DeadlineEvaluator

    SMAPI->>ModEntry: TimeChanged(timeOfDay)
    ModEntry->>LC: HandleTimeChanged(dayKey, currentTime)
    LC->>LC: session gate check
    LC->>Store: OnTimeChanged(dayKey, currentTime) [context sync only]
    Note over Store: Updates _currentDayKey/_currentTime; no snapshot

    SMAPI->>ModEntry: DayStarted
    ModEntry->>LC: HandleDayStarted(newDay, currentTime)
    LC->>Store: OnDayStarted(newDay, currentTime)
    Store->>Store: InitializeTimeContext(newDay, currentTime)
    Store->>Store: Remove expired tasks
    Store->>Projector: Project(container, _currentDayKey, _currentTime)
    Projector->>Evaluator: Evaluate(stored, currentDay, currentTime)
    Evaluator-->>Projector: DeadlineFields
    Projector-->>Store: TaskSnapshot
    Store-->>Store: SnapshotChanged fires

    Note over Store: Engine-driven completion
    Store->>Store: Handle(CompleteTaskCommand[IsPlayerInitiated=false])
    Store->>Store: Capture priorStatus=Incomplete
    Store->>Store: Apply handler → Status=Completed
    Store->>Store: ToastRequested fires (before SnapshotChanged)
    Store->>Projector: Project(container, _currentDayKey, _currentTime)
    Store-->>Store: SnapshotChanged fires
```

---

## File change summary

| File                                          | Action     | Key change                                                                                                                                               |
| --------------------------------------------- | ---------- | -------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `file:State/ToastType.cs`                     | **Create** | `ToastType` enum                                                                                                                                         |
| `file:State/ToastEvent.cs`                    | **Create** | `ToastEvent` sealed class                                                                                                                                |
| `file:State/BootstrapGuardPolicy.cs`          | **Create** | `BootstrapGuardPolicy` enum (policy seam)                                                                                                                |
| `file:State/Models/DeadlineStoredFields.cs`   | **Create** | Persisted deadline data                                                                                                                                  |
| `file:State/Models/DeadlineFields.cs`         | **Create** | Evaluated deadline read model                                                                                                                            |
| `file:State/DayBoundary/DeadlineEvaluator.cs` | **Create** | Static evaluator, parallel to `ExpirationDetector`                                                                                                       |
| `file:UI/UiToastSubscriptionManager.cs`       | **Create** | Mirrors `UiSnapshotSubscriptionManager`                                                                                                                  |
| `file:State/Commands/CompleteTaskCommand.cs`  | **Modify** | Add `IsPlayerInitiated` (default `false`)                                                                                                                |
| `file:State/Models/TaskRecord.cs`             | **Modify** | Add `DeadlineStoredFields?` (optional, default `null`)                                                                                                   |
| `file:State/Models/TaskView.cs`               | **Modify** | Add `DeadlineFields?` (optional, default `null`)                                                                                                         |
| `file:State/SnapshotProjector.cs`             | **Modify** | Signature + `DeadlineEvaluator.Evaluate()` call                                                                                                          |
| `file:State/StateStore.cs`                    | **Modify** | `ToastRequested`, time-context fields, bootstrap guard, `OnDayStarted` sig, `OnTimeChanged`, `InitializeTimeContext`, toast pre/post check in `Handle()` |
| `file:Events/IEventDispatcher.cs`             | **Modify** | Add `DispatchTimeChanged(DayKey, int)`                                                                                                                   |
| `file:Events/EventDispatcher.cs`              | **Modify** | Add no-op `DispatchTimeChanged`                                                                                                                          |
| `file:Lifecycle/LifecycleCoordinator.cs`      | **Modify** | `HandleTimeChanged`, updated `HandleSaveLoaded`/`HandleDayStarted` signatures, session tracking                                                          |
| `file:ModEntry.cs`                            | **Modify** | `TimeChanged` hook, `UiToastSubscriptionManager.Initialize`, updated `OnSaveLoaded`/`OnDayStarted` forwarding                                            |
