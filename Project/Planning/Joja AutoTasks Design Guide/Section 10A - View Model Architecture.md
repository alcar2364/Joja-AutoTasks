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

Native notifications are a separate UI integration concern. They may be triggered from backend events through the outbound game-events/effect queue, but they are not part of the StardewUI binding-context/view-model layer.

## 10A.2 Architecture Position

The View Model layer sits between the State Store and the UI surfaces.

Conceptual flow:

```text
State Store
  → Snapshot Published (C# event)
  → View Model receives `SnapshotChanged`
  → View Model updates INPC properties
  → StardewUI binding engine detects changes
  → UI re-renders affected elements
```

Live task surfaces consume State Store snapshots, while history, configuration, and debug sections layer read-only ledger/configuration data onto the same binding architecture without creating alternate state ownership.

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

public partial class HudTaskRowViewModel
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

Fields that never change after construction (e.g., `TaskId`) do not need `[Notify]` and can remain as regular properties.

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

The following view models are expected for Version 1.

### HUD View Models

`HudViewModel`

```text
- The primary binding context for the HUD drawable
- Subscribes to live snapshot updates
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

Naming note:

```text
- `HudTaskRowViewModel` is the canonical Version 1 role name in the design guide.
- An implementation may temporarily use an equivalent HUD row/item ViewModel name
  (for example `TaskItemViewModel`) during transition, but the role remains the
  Phase 9 HUD row/item projection model defined in Section 21.
```

Toast/effect routing note:

```text
- No toast/event view model participates in V1.
- `ToastRequested` does not flow into `HudViewModel` or other view
  models.
- Toast/effect payloads are routed through the outbound
  game-events/effect queue and consumed by native game integration
  outside the binding layer.
```

#### Menu View Models

`MenuShellViewModel` (or equivalent root menu binding context)

```text
- Owns current menu section/tab selection across Tasks, Task Builder,
History, Configuration, and Debug
- Coordinates the section-specific view models used by the menu surface
```

`TaskListViewModel`

```text
- Exposes the full task list for the menu Tasks section
- Owns menu-local state: selected task ID, active filters, sort mode,
search text
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
- Exposes mod configuration fields for the full StardewUI configuration
menu
- May be opened through the GMCM entry surface, which acts only as a
thin launcher into the full menu experience
- Reads from and writes to the `ModConfig` instance
```

`DebugViewModel`

```text
- Exposes debug toggles and live tuning fields for supported HUD/menu
diagnostics and layout parameters
- Updates debug/runtime tuning values without mutating canonical task
state
```

## 10A.6 Snapshot Subscription Model

View models MUST NOT hide backend-event subscription lifetime inside themselves for non-snapshot event channels. The View Model subscription model is specifically for `SnapshotChanged`.

Live task view models subscribe directly to `SnapshotChanged`, hold the resulting subscription lifetime, and release it during disposal.

`ToastRequested` and other outbound effect/game-event channels are not part of this model and must remain outside the view-model layer.

Recommended pattern:

- The State Store exposes `SnapshotChanged` for bindable UI reconciliation.
- Each live task view model subscribes during initialization and unsubscribes during disposal.
- On notification, the view model reconciles its current bindable state against the newly published snapshot.

Why this pattern:

- Keeps the binding boundary simple: snapshots in, INPC/collection updates out.
- Matches the canonical teardown rule in Section 2.5: dispose view models and unsubscribe from `SnapshotChanged`.
- Prevents the outbound effect/game-event path from leaking into the binding layer.

Recommended snapshot flow:

1. View Model subscribes to `SnapshotChanged`.
2. View Model receives `TaskSnapshot`.
3. View Model diffs snapshot against internal view state and updates INPC/collection properties (reconciling `ObservableCollection<T>`).
4. StardewUI detects INPC/collection changes and re-renders.

Responsibilities that stay outside the View Model subscription model:

- Native game API calls and conversions to platform-specific actions (for example `Game1.addHUDMessage`).
- Outbound game-events/effect queue ownership and draining.
- Any non-snapshot event routing such as toast/effect delivery.

View models therefore own a single, explicit snapshot subscription path and remain free of toast/effect routing responsibilities.

For the HUD, `HudViewModel` subscribes to `SnapshotChanged` and reconciles the HUD task rows directly. For the menu, the relevant menu view models subscribe to the same live snapshot source for task presentation. Native notifications never pass through the HUD binding context.

## 10A.7 UI-Local State

Certain state belongs exclusively to the UI layer and MUST NOT be stored in the State Store or persisted.

Examples of UI-local state:

```text
- Current selection (selected task ID)
- Scroll position
- HUD collapsed/expanded state
- Completed-task visibility toggles
- Active filter/sort settings (unless explicitly configured)
- Current menu section/tab
- Wizard current step
- Current history day
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

Task Builder view models follow the same "intent out, snapshot back" rule but emit rule-definition and persistence intents only. Config and debug view models update their respective config/debug services rather than dispatching task-state commands.

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

This lifecycle must honor the teardown sequence in Section 2.5: view models are disposed before the HUD drawable is torn down and before the State Store is cleared.

Disposal MUST release any timers, UI-owned event handlers, or other local resources and MUST unsubscribe from `SnapshotChanged` before the view model is discarded.

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
- Menu filtering/sorting/searching and baseline history day navigation
are in-scope for V1; deeper browsing polish and statistics views are
staged later
- Toast/effect handling remains outside the View Model layer and does
  not add extra event subscriptions beyond `SnapshotChanged`
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

## Implementation Plan Traceability

Primary phase owner(s):

- Phase 4 — View Model Infrastructure

Also referenced in:

- Phase 8 — Menu Dashboard
- Phase 9 — HUD Interface
- Phase 10 — Task Builder Wizard
- Phase 11 — History Browsing UI
- Phase 12 — Debug and Development Tools

Canonical implementation mapping lives in Section 21.
