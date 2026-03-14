# Section 10 — UI Data Binding Model #

## 10.1 Purpose ##

The UI Data Binding Model defines how user interface systems interact with the
task system.

The UI must be able to:

    - Display task lists
    - Show task details
    - Allow user interactions such as completion and pinning
    - Navigate task history
    - Update automatically when task state changes

The UI must **never mutate task state directly**.

All UI interactions must emit commands to the State Store.

Command and mutation-boundary semantics are defined in Section 8.6 and
Section 8.7.

## 10.2 UI Interaction Principles ##

The UI layer follows several architectural principles.

### Read-Only Data Access ###

UI components receive immutable snapshots of task data.

### Command-Based Mutations ###

User actions produce commands that are sent to the State Store.

Command ownership and reducer behavior are defined in Section 8.6 and
Section 8.7.

#### Snapshot Rendering ####

The UI renders the latest published snapshot of task state.

#### Loose Coupling ####

UI components do not reference the Evaluation Engine or persistence systems.

## 10.3 Snapshot Subscription Model ##

UI components subscribe to task snapshots published by the State Store.

The View Model layer (Section 10A) is the primary consumer of these
snapshots. View Models receive snapshot change events, diff against
current state, and update INPC properties so that StardewUI's binding
engine detects and renders changes automatically.

For the HUD, `HudViewModel` is the binding boundary between the
published task snapshot and the StardewUI drawable. It projects
snapshot data into bindable HUD state such as task rows, summary
display values, and HUD-local interaction state.

See Section 10A for the complete View Model architecture, including
INPC implementation strategy, collection binding, and lifecycle rules.

Conceptual flow:

State Store  
→ Publish Snapshot  
→ UI Subscribers Receive Update  
→ UI Re-renders

The snapshot contains a read-only list of task views.

Conceptual structure:

TaskSnapshot

    - `IReadOnlyList<TaskView>`

Each TaskView represents the UI-facing projection of a task.

## 10.4 Task View Structure ##

TaskView contains only fields required for rendering.

Conceptual fields:

TaskView

    - `TaskID`
    - `Title`
    - `Description`
    - `Category`
    - `Status`
    - `ProgressCurrent`
    - `ProgressTarget`
    - `DeadlineFields`
    - `Icon`
    - `UserFlags`

Derived fields may also be included for convenience:

    - `ProgressPercent`
    - `DaysRemaining`
    - `IsOverdue`

These fields are derived from the underlying `TaskRecord`.

Note: ProgressCurrent and ProgressTarget are tracking metrics for display and HUD
rendering. Task completion is determined by completion conditions, not solely by
progress saturation. See Section 4.4.1 for details.

## 10.5 HUD Task Display ##

The HUD displays the current day's active tasks.

HUD responsibilities:

    - Render incomplete and completed tasks
    - Provide quick completion toggles
    - Allow scrolling through the task list
    - Show summary progress

The HUD receives its data from the latest snapshot.

The HUD binds through `HudViewModel`, which translates the latest
snapshot into bindable HUD state for StardewUI rendering.

The HUD never modifies task state directly.

Native V1 toast notifications are not part of the HUD data-binding
model. They use a separate UI notification path described in
Section 10A and Section 20.

## 10.6 Task Menu Interface ##

The Task Menu provides a larger interface for task management.

Capabilities include:

    - Viewing task history
    - Inspecting detailed task information
    - Editing manual tasks
    - Creating Task Builder rules
    - Navigating between days

The menu queries snapshots and history data rather than accessing store
state directly.

## 10.7 Task Selection Model ##

UI components may maintain a local selection state.

Example:

SelectedTaskID

Selection state belongs to the UI layer and is **not stored in the State Store**.

This prevents UI-specific concerns from polluting task state.

## 10.8 User Interaction Commands ##

User actions are translated into commands sent to the State Store.

Examples include:

    - `CompleteTaskCommand`
    - `UncompleteTaskCommand`
    - `PinTaskCommand`
    - `UnpinTaskCommand`
    - `RemoveTaskCommand`
    - `CreateManualTaskCommand`
    - `EditManualTaskCommand`

Command flow:

UI Interaction  
→ Command Created  
→ Command Sent to State Store  
→ Reducer Applies Change  
→ New Snapshot Published  
→ UI Updates

## 10.9 UI Refresh Behavior ##

The UI automatically updates when a new snapshot is published.

Typical refresh cycle:

1. Snapshot received
2. UI invalidates cached display data
3. UI components re-render visible elements

This ensures UI always reflects the current task state.

Toast notifications follow a separate event-driven path and do not
participate in snapshot reconciliation or HUD binding updates.

## 10.10 Scroll and Pagination Behavior ##

Task lists may contain more tasks than can be displayed at once.

UI components manage:

    - Scroll position
    - Pagination state
    - Visible task window

These values remain local to the UI.

The task snapshot always contains the full task list.

## 10.11 History Navigation ##

The Task Menu allows users to browse tasks from previous days.

History navigation operates using stored daily snapshots.

Conceptual model:

TaskHistory

    - `Dictionary<DayKey, TaskSnapshot>`

Selecting a day loads the corresponding snapshot for display.

History snapshots are read-only.

## 10.12 UI Performance Considerations ##

UI systems must avoid unnecessary rendering work.

Guidelines include:

    - Only redraw when snapshot version changes
    - Reuse UI layout calculations where possible
    - Avoid recomputing derived values already provided by TaskView

Snapshots provide stable data to support efficient rendering.

## 10.13 Debug UI Support ##

The UI layer may include developer debugging tools.

Examples:

    - `Task ID display toggle`
    - `Rule source visualization`
    - `Command log viewer`
    - `Evaluation timing metrics`

Debug UI components must also rely on snapshots rather than store mutation.

## 10.14 Version 1 Constraints ##

Version 1 UI functionality is intentionally limited.

Excluded features include:

    - Task drag-and-drop ordering
    - Task grouping
    - Advanced filtering
    - Multi-player UI synchronization

The initial UI focuses on stable task display and basic interactions.

## 10.15 Localization Resolution and Fallback ##

Localization resolution occurs in the UI binding/render boundary.

Rules:

    - View models and UI adapters resolve localized strings via SMAPI `I18n`
    from stable localization keys and arguments.
    - Task snapshots remain locale-neutral and continue to represent canonical
    state from the State Store.
    - Localized display strings must not be fed back into identity,
    comparison, or mutation logic.

Missing-key fallback behavior:

    - If a localization key is missing, UI should display a deterministic
    fallback string (prefer explicit fallback text if provided; otherwise
    display the unresolved key token).
    - Missing-key warnings should be logged in a bounded/deduplicated manner
    to avoid log spam.

This preserves the snapshot boundary while allowing locale-aware rendering.
