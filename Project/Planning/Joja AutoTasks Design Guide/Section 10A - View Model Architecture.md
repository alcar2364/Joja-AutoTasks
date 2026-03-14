# Section 10A — View Model Architecture

## 10A.1 Purpose

The View Model layer bridges the State Store snapshots (Section 8) and the StardewUI data binding system.

StardewUI uses a Model-View-Whatever (MVW) binding model where StarML views bind to C# context objects. For bindings to reflect state changes automatically, context objects must implement `INotifyPropertyChanged` (INPC). Collection properties that add or remove items must use `ObservableCollection<T>` or another `INotifyCollectionChanged` type.

The State Store publishes read-only snapshots (Section 8.5). These snapshots are immutable data — they cannot directly serve as binding contexts because they do not implement INPC.

The View Model layer solves this by:

```text
- Subscribing to State Store snapshot changes
- Diffing new snapshots against current view model state
- Updating INPC-enabled properties so StardewUI reacts automatically
- Owning UI-local state (selection, scroll position, filters) that does
not belong in the State Store
```

Native notifications are a separate UI integration concern. They may
listen to backend events, but they are not part of the StardewUI
binding-context/view-model layer.

## 10A.2 Architecture Position ##

The View Model layer sits between the State Store and the UI surfaces.

Conceptual flow:

```text
State Store
  → Snapshot Published (C# event)
  → View Model receives snapshot
  → View Model updates INPC properties
  → StardewUI binding engine detects changes
  → UI re-renders affected elements
```

View Models are consumed by StarML views through the binding context mechanism described in the StardewUI documentation.

View Models MUST NOT:

```text
- mutate canonical task state directly
- bypass the command dispatch boundary defined in Section 8.6
- contain rule evaluation or generation logic
- access the game state directly (that is the engine's job)
```

View Models MAY:

```text
- dispatch commands to the State Store on behalf of UI interactions
- maintain UI-local state (selection, filters, scroll, collapsed state)
- compute derived display values from snapshot data
- implement `Update(TimeSpan)` for StardewUI frame-tick dispatch when
animations or time-sensitive display updates are needed
```

## 10A.3 INPC Implementation Strategy

StardewUI recommends using a source generator or compile-time weaver to automate INPC boilerplate.

Recommended approach for JAT:

**PropertyChanged.SourceGenerator** (NuGet)

This library:

```text
- leaves no footprint (no extra assemblies in build output)
- handles dependent properties automatically
- requires only the `partial` class keyword and `[Notify]` attribute on
backing fields
- is free and MIT-licensed
```

Example:

```cs
using PropertyChanged.SourceGenerator;

public partial class TaskRowViewModel
{
[Notify] private string title = "";
[Notify] private string status = "";
[Notify] private int progressCurrent;
[Notify] private int progressTarget;

// Derived property — auto-detected by source generator
public float ProgressPercent =>
    ProgressTarget > 0
        ? (float)ProgressCurrent / ProgressTarget
        : 0f;
}
```

Fields that never change after construction (e.g., `TaskID`) do not need `[Notify]` and can remain as regular properties.

## 10A.4 Collection Binding Strategy

Task lists displayed in the HUD and menu are dynamic collections. Items may be added, removed, or reordered when a new snapshot arrives.

StardewUI requires `INotifyCollectionChanged` for efficient list updates. The recommended collection type is:

`ObservableCollection<T>`

View Models that expose task lists MUST use `ObservableCollection<T>` to avoid full view-tree rebuilds on every snapshot change.

When a new snapshot arrives, the View Model should reconcile the `ObservableCollection` against the new snapshot data:

```text
- add new items
- remove items no longer present
- update existing items in place (via INPC on the item view models)
- reorder if sort criteria changed
```

This reconciliation strategy avoids replacing the entire collection on every update, which would cause StardewUI to rebuild all repeated views.

## 10A.5 View Model Catalog

The following View Models are expected for Version 1.

### HUD View Models

`HudViewModel`

```text
- The primary binding context for the HUD drawable
- Subscribes to State Store snapshot updates
- Reconciles HUD task rows against the latest snapshot
- Owns HUD-local state such as scroll position, collapse state, and selection
- Dispatches commands for HUD interactions such as completion and pinning
- Does NOT call game APIs directly
```

`HudTaskRowViewModel`

```text
- Represents a single task row in the HUD
- Fields: title, status, progress, category icon, completion affordance
```

`UiNotificationBridge`

```text
- Thin UI integration component for native V1 notifications
- Subscribes to backend notification events such as StateStore.ToastRequested
- Translates backend notification payloads into native game UI calls such as Game1.addHUDMessage()
- Separate from StardewUI binding contexts and task-row reconciliation
```

#### Menu View Models ####

`TaskListViewModel`

```text
- Exposes the full task list for the menu Tasks section
- Owns menu-local state: selected task ID, active filters, sort mode
- Publishes the selected task's detail data
```

`TaskDetailViewModel`

```text
- Exposes detail fields for the currently selected task
- Updated when selection changes in `TaskListViewModel`
```

`HistoryViewModel`

```text
- Exposes day-keyed historical snapshots for the History section
- Owns the selected day key for navigation
- Queries the Daily Snapshot Ledger (Section 11)
```

`ManualTaskEditorViewModel`

```text
- Exposes fields for creating or editing a manual task
- Validates input before dispatching create/edit commands
```

#### Wizard View Models

`WizardViewModel`

```text
- Manages wizard step navigation (current step, can-next, can-back)
- Contains step-specific sub-view-models
```

`WizardStepViewModel` (one per step type)

```text
- Exposes fields and validation for a single wizard step
- Produces partial rule definition data
```

`WizardPreviewViewModel`

```text
- Exposes a preview of what the rule would produce as a task
- Updated live as the user builds the rule
```

#### Configuration View Models

`ConfigViewModel`

```text
- Exposes mod configuration fields for the StardewUI configuration menu
- Reads from and writes to the `ModConfig` instance
```

## 10A.6 Snapshot Subscription Model

View Models MUST NOT own subscriptions to the State Store. Subscription ownership belongs to UI-layer host objects (for example, `HudHost`, `MenuHost`) that manage native lifecycle and game-API responsibilities.

Recommended pattern:

- The State Store exposes event sources (for example `SnapshotChanged`, `ToastRequested`) which are wrapped by `UiSnapshotSubscriptionManager` and `UiToastSubscriptionManager` to provide `Subscribe(...) -> IDisposable` tokens.
- A UI host subscribes during initialization and disposes the returned token during teardown or replacement (for example on DayStarted recreation).
- The host forwards incoming snapshots and toast events to its View Model by calling a well-defined method (for example `viewModel.OnSnapshotReceived(TaskSnapshot)` or `viewModel.OnToastReceived(ToastEvent)`).

Why this pattern:

- Centralizes subscription lifetime in the UI host (single place to dispose/replace subscriptions when UI surfaces are recreated).
- Keeps View Models focused and testable: view models become pure converters/update handlers that accept snapshots via public methods instead of hiding subscription side-effects.
- Avoids accidental memory leaks and duplicate subscriptions when view-model instances are replaced (e.g., daily recreation flows).

Recommended host-to-viewmodel flow:

1. Host subscribes to State Store via `UiSnapshotSubscriptionManager.Subscribe(OnSnapshotReceived)`.
2. Host receives `TaskSnapshot` and calls `viewModel.OnSnapshotReceived(snapshot)`.
3. View Model diffs snapshot against internal view state and updates INPC/collection properties (reconciling `ObservableCollection<T>`).
4. StardewUI detects INPC/collection changes and re-renders.

Example subscription ownership responsibilities (hosts):

- Subscribe / unsubscribe to backend events.
- Own native game API calls and conversions to platform-specific actions (for example `Game1.addHUDMessage`).
- Dispose subscriptions before replacing UI surfaces or view models.

View Models therefore implement explicit update entry points (for example `OnSnapshotReceived`, `OnToastReceived`) and remain free of subscription side-effects.

For the HUD, `HudViewModel` is the snapshot subscriber. Native
notifications use a separate event subscription path through
`UiNotificationBridge` and do not pass through the HUD binding context.

## 10A.7 UI-Local State

Certain state belongs exclusively to the UI layer and MUST NOT be stored in the State Store or persisted.

Examples of UI-local state:

```text
- Current selection (selected task ID)
- Scroll position
- HUD collapsed/expanded state
- Active filter/sort settings (unless explicitly configured)
- Wizard current step
- Menu-local search text
```

UI-local state is owned by View Models and is destroyed when the View Model is disposed.

Exception: HUD position (drag coordinates) and HUD collapsed state MAY be persisted through the configuration system (Section 15) since they represent user preferences, not task state.

## 10A.8 Command Dispatch from View Models

View Models dispatch commands to the State Store on behalf of UI interactions.

Example flow for completing a task:

```text
User clicks checkbox → HudViewModel.CompleteTask(taskId)
  → Dispatches CompleteTaskCommand to State Store
  → Reducer applies change
  → New snapshot published
  → HudViewModel receives snapshot via event
  → HudViewModel updates task row status
  → StardewUI re-renders checkbox as completed
```

View Models MUST NOT modify their own state optimistically before the snapshot confirms the change. The snapshot is the source of truth.

## 10A.9 View Model Lifecycle

View Models are created when their UI surface is activated and disposed when it is deactivated.

HUD View Model lifecycle:

```text
- Created when the HUD surface/drawable is initialized by UI composition/startup wiring
- Disposed when the player returns to title or HUD is destroyed
```

Menu View Model lifecycle:

```text
- Created when the menu is opened
- Disposed when the menu is closed
```

Wizard View Model lifecycle:

```text
- Created when the wizard is launched
- Disposed when the wizard completes or is cancelled
```

Disposal MUST unsubscribe from State Store events to prevent memory leaks and stale updates.

`UiNotificationBridge` follows the same general subscription hygiene for
its own backend event listeners, but it is not itself a view model.

## 10A.10 StardewUI Update Tick Integration

StardewUI dispatches frame ticks to context objects that implement a `void Update(TimeSpan elapsed)` method.

View Models MAY use this mechanism for:

```text
- animation state (e.g., completion sparkle timers)
- time-sensitive derived values (e.g., "2 hours remaining" countdown)
- debounced search/filter input
```

View Models MUST NOT use `Update` for heavy computation or game state queries.

## 10A.11 Version 1 Constraints

Version 1 View Model constraints:

```text
- View Models are single-threaded (main game thread only)
- No async/await patterns in View Model code
- No multiplayer View Model synchronization
- Collection reconciliation uses simple linear diff (adequate for
expected task counts in V1)
```

## 10A.12 Historical Data Access Pattern

### IDailySnapshotLedger Interface

HistoryViewModel accesses historical data through a dedicated read-only interface:

```text
IDailySnapshotLedger
  GetSnapshot(DayKey)   → DailyTaskSnapshot?        // null if no entry for that day
  GetRecordedDays()     → IReadOnlyList<DayKey>     // all days with entries, newest first
  MostRecentDay         → DayKey?                   // null if ledger is empty
```

The ledger is read-only from the view model's perspective. HistoryViewModel never writes to or modifies historical records.

### HistoryViewModel Data Source Contract

HistoryViewModel receives IDailySnapshotLedger via constructor injection, parallel to the UiSnapshotSubscriptionManager pattern for live data.

```text
HistoryViewModel
  - Receives IDailySnapshotLedger via constructor injection
  - Owns SelectedDayKey (UI-local state; not persisted)
  - Initializes SelectedDayKey to IDailySnapshotLedger.MostRecentDay
  - Exposes: DayLabel (string), Tasks (IReadOnlyList<HistoricalTaskRowViewModel>)
  - Exposes: CanGoPrevious (bool), CanGoNext (bool)
  - Commands: GoToPreviousDay(), GoToNextDay()
  - On day change: calls GetSnapshot(SelectedDayKey); if null, exposes empty list + NoDataMessage

HistoricalTaskRowViewModel
  - Fields: Title (string), IsCompleted (bool)
  - V1 only — no progress bar, no category icon
```

### V1 Navigation Rules

- Navigation: Previous/Next day arrows only. Current day label between arrows.
- No calendar picker, no jump-to-date, no scrollable day list in V1.
- Arrows disabled at the most-recent recorded day (→ Next disabled).
- Arrows disabled at the earliest possible day (← Previous disabled).
- Player cannot navigate to today or future days from the History section.

### Navigation Boundary Table

| Condition | ← Previous | → Next |
| --- | --- | --- |
| Ledger is empty | Disabled | Disabled |
| Viewing most recent recorded day | Enabled (if earlier days exist) | Disabled |
| Viewing an intermediate day | Enabled | Enabled |
| Viewing the earliest possible day | Disabled | Enabled |

### Empty State and Missing Day Handling

- If the ledger is empty: show "No history recorded yet." Both arrows disabled.
- If GetSnapshot(DayKey) returns null: show "No data recorded for this day." Navigation arrows remain enabled so the player can skip past the gap.
- The mod does not attempt to reconstruct or infer history for missing days.

### V2 Upgrade Path

- Split-panel layout: left panel shows scrollable day list (most recent first); right panel shows task list for selected day.
- Full task rows: title, category icon, progress bar, completion status.
- Detail panel: selecting a historical task shows full detail.
- GetRecordedDays() is included in the V1 interface contract to support this upgrade.
