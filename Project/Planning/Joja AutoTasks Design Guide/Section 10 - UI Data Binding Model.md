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

The HUD and Menu are both required Version 1 surfaces. They stay
behaviorally aligned by consuming the same published task snapshot and by
routing mutations through the same command path.

UI surfaces may also react to configuration and debug-tuning changes,
but those inputs must not create an alternate source of task truth.

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

Live task UI surfaces render from snapshots published by the State
Store.

The View Model layer (Section 10A) is the primary consumer of these
snapshots. Live task view models subscribe to `SnapshotChanged`,
receive the new snapshot directly, diff against current state, and
update INPC properties so that StardewUI's binding engine detects and
renders changes automatically.

For the HUD, `HudViewModel` is the binding boundary between the
published task snapshot and the StardewUI drawable. It projects
snapshot data into bindable HUD state such as task rows, summary
display values, and HUD-local interaction state. Menu view models
consume the same live snapshot model for task-list/detail presentation
while history uses read-only ledger snapshots (Section 11).

See Section 10A for the complete View Model architecture, including
INPC implementation strategy, collection binding, and lifecycle rules.

Conceptual flow:

```text
State Store
→ Publish Snapshot
→ View Model Receives SnapshotChanged
→ View Model Reconciles Bindable State
→ UI Re-renders
```

Task surfaces also refresh when configuration values or debug tuning
values affecting layout/visibility change, but those refreshes must
preserve the snapshot boundary and must not mutate canonical task state.

The snapshot contains a read-only list of task views.

Conceptual structure:

TaskSnapshot

    - `IReadOnlyList<TaskView>`

Each TaskView represents the UI-facing projection of a task.

## 10.4 Task View Structure ##

TaskView contains only fields required for rendering and remains
locale-neutral.

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

Display-oriented fields such as `Title`, `Description`, and `Icon`
should be understood as locale-neutral presentation payloads. When
localization is required, the snapshot carries stable keys/arguments (or
an equivalent canonical descriptor) and the UI resolves the final text
at binding/render time.

Note: ProgressCurrent and ProgressTarget are tracking metrics for display and HUD
rendering. Task completion is determined by completion conditions, not solely by
progress saturation. See Section 4.4.1 for details.

## 10.5 HUD Task Display ##

The HUD displays the current day's active tasks.

HUD responsibilities:

    - Render incomplete and completed tasks from the shared live snapshot
    - Provide quick completion and pinning toggles
    - Allow scrolling through the task list
    - Maintain HUD-local interaction state such as selection,
    collapse/expand, completed-task visibility, and open-menu
    affordances
    - Show summary progress

The HUD receives its data from the latest snapshot.

The HUD binds through `HudViewModel`, which translates the latest
snapshot into bindable HUD state for StardewUI rendering.

The HUD may also react to layout-affecting configuration/debug values
such as position, size, or text scaling. These values are preferences
and presentation state, not task state.

The HUD never modifies task state directly.

Native V1 toast notifications are not part of the HUD data-binding
model. They bypass the View Model layer and route through the outbound
game-events/effect queue described in Section 20.

## 10.6 Task Menu Interface ##

The Task Menu provides a larger interface for task management.

Capabilities include:

    - Viewing the task list and selected task details together
    - Viewing task history
    - Inspecting detailed task information
    - Editing manual tasks
    - Creating Task Builder rules
    - Filtering, sorting, and searching within the menu
    - Navigating between recorded days
    - Modifying configuration
    - Accessing debug tools

The menu consumes the same live snapshot as the HUD for task browsing,
combines it with read-only ledger data for history, and uses menu-local
state for selection, filters, sorting, search, and navigation.

The GMCM configuration entry surface is only a thin access point into
the full StardewUI configuration menu. It does not create a separate
task binding model.

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

Task Builder UI follows the same general binding pattern but emits
rule-definition and persistence intents rather than direct runtime-task
mutation commands. Configuration and debug interactions write through
their own config/debug boundaries instead of the task command pipeline.

Command flow:

UI Interaction  
→ Command Created  
→ Command Sent to State Store  
→ Reducer Applies Change  
→ New Snapshot Published  
→ UI Updates

## 10.9 UI Refresh Behavior ##

The UI automatically updates when a new snapshot is published and when
surface-affecting configuration or debug-tuning values change.

Typical refresh cycle:

1. Snapshot or config/debug change received
2. Affected view models invalidate or reconcile local projections
3. UI components re-render affected elements

This ensures UI always reflects the current task state and currently
active UI configuration.

Toast notifications follow a separate event-driven path and do not
participate in snapshot reconciliation or HUD binding updates. They are
handled outside the View Model layer through the outbound
game-events/effect queue.

## 10.10 Scroll and Visible-Window Behavior ##

Task lists and history views may contain more items than can be
displayed at once.

UI components manage:

    - Scroll position
    - Visible task window
    - Any menu-local paging or day-navigation state

These values remain local to the UI.

The task snapshot always contains the full live task list. The History
section separately loads read-only day snapshots from the ledger.

## 10.11 History Navigation ##

The Task Menu allows users to browse tasks from previous days.

History navigation operates using stored daily snapshots.

Conceptual model:

TaskHistory

    - `Dictionary<DayKey, TaskSnapshot>`

Selecting a day loads the corresponding snapshot for display.

History snapshots are read-only.

Version 1 baseline history navigation is previous/next day browsing.
Quick-jump depth, richer filtering, and broader history ergonomics are
staged for later hardening slices rather than omitted permanently.

## 10.12 UI Performance Considerations ##

UI systems must avoid unnecessary rendering work.

Guidelines include:

    - Only redraw when snapshot version changes
    - Reuse UI layout calculations where possible
    - Avoid recomputing derived values already provided by TaskView
    - Reuse bound collections and cached layout unless task visibility or
    layout-affecting config/debug inputs change

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

Version 1 UI functionality is intentionally staged rather than
surface-reduced.

Included in Version 1 are both the HUD and the Menu surfaces, shared
snapshot parity, baseline history browsing, and menu-local
filtering/sorting/searching.

Deferred features include:

    - Task drag-and-drop ordering
    - Statistics dashboards and aggregate analytics UI (V2)
    - Expanded custom toast UI and non-auto-complete toast triggers (V2)
    - Deeper history quick-jump/browsing polish beyond baseline day
    navigation
    - Multi-player UI synchronization

## 10.15 Localization Resolution and Fallback ##

Localization resolution occurs in the UI binding/render boundary.

Rules:

    - View models and UI adapters resolve localized strings via SMAPI `I18n`
    from stable localization keys and arguments.
    - Task snapshots remain locale-neutral and continue to represent canonical
    state from the State Store.
    - Localized display strings must not be fed back into identity,
    comparison, or mutation logic.
    - Locale changes should refresh affected UI through normal
    binding/update paths without mutating canonical task state.

Missing-key fallback behavior:

    - If a localization key is missing, UI should display a deterministic
    fallback string (prefer explicit fallback text if provided; otherwise
    display the unresolved key token).
    - Missing-key warnings should be logged in a bounded/deduplicated manner
    to avoid log spam.

This preserves the snapshot boundary while allowing locale-aware rendering.
