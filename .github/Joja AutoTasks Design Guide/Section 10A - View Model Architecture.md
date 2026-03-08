# Section 10A — View Model Architecture #

## 10A.1 Purpose ##

The View Model layer bridges the State Store snapshots (Section 8) and
the StardewUI data binding system.

StardewUI uses a Model-View-Whatever (MVW) binding model where StarML
views bind to C# context objects. For bindings to reflect state changes
automatically, context objects must implement `INotifyPropertyChanged`
(INPC). Collection properties that add or remove items must use
`ObservableCollection<T>` or another `INotifyCollectionChanged` type.

The State Store publishes read-only snapshots (Section 8.5). These
snapshots are immutable data — they cannot directly serve as binding
contexts because they do not implement INPC.

The View Model layer solves this by:

```text
- Subscribing to State Store snapshot changes
- Diffing new snapshots against current view model state
- Updating INPC-enabled properties so StardewUI reacts automatically
- Owning UI-local state (selection, scroll position, filters) that does
not belong in the State Store
```

## 10A.2 Architecture Position ##

The View Model layer sits between the State Store and the UI surfaces.

Conceptual flow:

``` text
State Store
  → Snapshot Published (C# event)
  → View Model receives snapshot
  → View Model updates INPC properties
  → StardewUI binding engine detects changes
  → UI re-renders affected elements
```

View Models are consumed by StarML views through the binding context
mechanism described in the StardewUI documentation.

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

## 10A.3 INPC Implementation Strategy ##

StardewUI recommends using a source generator or compile-time weaver to
automate INPC boilerplate.

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

``` cs
using PropertyChanged.SourceGenerator;

public partial class TaskRowViewModel
{
```

[Notify] private string title = "";
[Notify] private string status = "";
[Notify] private int progressCurrent;
[Notify] private int progressTarget;

```text

```

// Derived property — auto-detected by source generator
public float ProgressPercent =>
    ProgressTarget > 0
        ? (float)ProgressCurrent / ProgressTarget
        : 0f;

```text

}

```

Fields that never change after construction (e.g., `TaskID`) do not need
`[Notify]` and can remain as regular properties.

## 10A.4 Collection Binding Strategy ##

Task lists displayed in the HUD and menu are dynamic collections. Items
may be added, removed, or reordered when a new snapshot arrives.

StardewUI requires `INotifyCollectionChanged` for efficient list
updates. The recommended collection type is:

`ObservableCollection<T>`

View Models that expose task lists MUST use `ObservableCollection<T>` to
avoid full view-tree rebuilds on every snapshot change.

When a new snapshot arrives, the View Model should reconcile the
`ObservableCollection` against the new snapshot data:

```text
- add new items
- remove items no longer present
- update existing items in place (via INPC on the item view models)
- reorder if sort criteria changed
```

This reconciliation strategy avoids replacing the entire collection on
every update, which would cause StardewUI to rebuild all repeated views.

## 10A.5 View Model Catalog ##

The following View Models are expected for Version 1.

### HUD View Models ###

`HudViewModel`

```text
- Exposes the filtered/sorted task list for the HUD
- Owns HUD-local state: collapsed, scroll position
- Subscribes to State Store snapshot changes
- Respects HUD configuration (max visible tasks, show completed)
```

`HudTaskRowViewModel`

```text
- Represents a single task row in the HUD
- Fields: title, status, progress, category icon, completion affordance
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

#### Wizard View Models ####

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

#### Configuration View Models ####

`ConfigViewModel`

```text
- Exposes mod configuration fields for the StardewUI configuration menu
- Reads from and writes to the `ModConfig` instance
```

## 10A.6 Snapshot Subscription Model ##

View Models subscribe to State Store changes through a C# event.

The State Store MUST expose a snapshot-changed event:

``` cs
public event Action<TaskSnapshot>? SnapshotChanged;
```

View Models subscribe during initialization and unsubscribe during
disposal.

When a new snapshot arrives:

1. View Model receives the snapshot via the event handler
2. View Model diffs against current state
3. View Model updates INPC properties for changed values
4. StardewUI detects changes and re-renders affected nodes

This event-driven model ensures UI updates happen only when state
actually changes, consistent with the performance guardrails in
Section 19.

## 10A.7 UI-Local State ##

Certain state belongs exclusively to the UI layer and MUST NOT be stored
in the State Store or persisted.

Examples of UI-local state:

```text
- Current selection (selected task ID)
- Scroll position
- HUD collapsed/expanded state
- Active filter/sort settings (unless explicitly configured)
- Wizard current step
- Menu-local search text
```

UI-local state is owned by View Models and is destroyed when the View
Model is disposed.

Exception: HUD position (drag coordinates) and HUD collapsed state MAY
be persisted through the configuration system (Section 15) since they
represent user preferences, not task state.

## 10A.8 Command Dispatch from View Models ##

View Models dispatch commands to the State Store on behalf of UI
interactions.

Example flow for completing a task:

``` text
User clicks checkbox → HudViewModel.CompleteTask(taskId)
  → Dispatches CompleteTaskCommand to State Store
  → Reducer applies change
  → New snapshot published
  → HudViewModel receives snapshot via event
  → HudViewModel updates task row status
  → StardewUI re-renders checkbox as completed
```

View Models MUST NOT modify their own state optimistically before the
snapshot confirms the change. The snapshot is the source of truth.

## 10A.9 View Model Lifecycle ##

View Models are created when their UI surface is activated and disposed
when it is deactivated.

HUD View Model lifecycle:

```text
- Created when the save is loaded and HUD is initialized
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

Disposal MUST unsubscribe from State Store events to prevent memory
leaks and stale updates.

## 10A.10 StardewUI Update Tick Integration ##

StardewUI dispatches frame ticks to context objects that implement a
`void Update(TimeSpan elapsed)` method.

View Models MAY use this mechanism for:

```text
- animation state (e.g., completion sparkle timers)
- time-sensitive derived values (e.g., "2 hours remaining" countdown)
- debounced search/filter input
```

View Models MUST NOT use `Update` for heavy computation or game state
queries.

## 10A.11 Version 1 Constraints ##

Version 1 View Model constraints:

```text
- View Models are single-threaded (main game thread only)
- No async/await patterns in View Model code
- No multiplayer View Model synchronization
- Collection reconciliation uses simple linear diff (adequate for
expected task counts in V1)
```
