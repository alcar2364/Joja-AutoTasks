# Section 2 — System Architecture #

## 2.1 UI Surface Architecture ##

### HUD (Moderate) ###

The HUD is a lightweight, always-available surface intended for
moment-to-moment tracking.

HUD capabilities:

    - Display active tasks for the day
    - Support scrolling and selection
    - Show task details for the selected task
    - Allow configurable visibility of completed tasks
    - Support optional grouping by category
    - Expandable/Collapsible
    - Can open Menu dashboard from HUD

The HUD is not responsible for:

    - Editing tasks
    - Creating Task Builder rules
    - Complex filtering or analytics
    - Configuration

These belong in the Menu dashboard.

Delivery constraint:

    - HUD and Menu are both required in Version 1 and must stay behaviorally
    aligned through shared snapshot consumption.
    - Phase 8 through Phase 12 capabilities follow the no-drop mapping in
    Section 21.3.1; deferral is depth-only and stage-bounded.

#### Main Menu (Full Dashboard) ####

The main menu is the primary “management UI” for the mod.

Menu capabilities:

    - Full task list management (today + staged history browsing)
    - Task details view
    - Manual task creation/editing
    - Task Builder wizard launch and management of Task Builder rules
    - Statistics dashboard (V2)
    - Categorized task presentation
    - Filtering/sorting/searching (expected in dashboard UI)
    - Task tracking options (turn on/off automatic tracking of built-in tasks)
    - HUD display configuration

History staging note:

    - Baseline day browsing and day navigation ship in the Phase 11 Now
    slice.
    - Filtering and quick-jump depth are delivered in the Phase 11 Next
    slice.

## 2.2 Task Builder UI Model ##

The Task Builder is implemented as a wizard-based interface.

The Now-stage Task Builder scope is rule-definition flow only.

Wizard characteristics:

    - Step-by-step guided creation of rule-driven tasks
    - Step-based rule-definition authoring

    Each wizard step collects:

        * trigger condition (when should the task exist)
        * progress/completion logic (what defines completion)
        * presentation settings (name/category/icon)
        * scheduling/reminders (optional)
        * persistence behavior (daily vs persistent)

Boundary constraints:

    - The wizard dispatches rule-definition intents through command/persistence
    boundaries.
    - The wizard must not directly create, complete, or mutate runtime
    task entities.
    - Runtime task mutation remains owned by the State Store command path
    after rule evaluation.

The wizard approach is chosen to support:

    - High power without overwhelming players
    - Extensibility over time (new steps / new condition types)
    - Validation at each stage (avoid broken rules)

## 2.3 Flexibility-First Engine Constraint ##

Because the Task Builder is intended to be “very powerful,” the engine design prioritizes:

    - extensible condition logic
    - composable rule evaluation
    - stable saved representation of user rules
    - future expansion without breaking saves

Performance strategy will be:

    - snapshotting required game state into a Generation/Evaluation Context
    - caching expensive lookups
    - scheduling evaluation at sensible times (e.g., day start, on menu open,
  periodic tick with throttle, and on relevant events)

The architecture assumes “unlimited tasks,” therefore correctness and stability
matter more than raw speed, but speed is managed via caching and throttling
rather than restricting features.

## 2.4 Core Subsystems ##

Subsystem dependencies are wired using **constructor injection**. Each
subsystem declares its dependencies explicitly. The lifecycle coordinator
(Entry & Lifecycle) constructs and connects all subsystems during
initialization.

This wiring pattern enables:

    - explicit dependency visibility
    - unit testing of individual subsystems with mock dependencies
    - clean disposal ordering on teardown

1. Entry & Lifecycle

    Responsibilities:

    * SMAPI entry point
    * Hook game events
    * Initialize and connect subsystems

    Own global services (logging, config access)

2. Task Domain

    Responsibilities:

    * Pure data model: tasks, statuses, categories, IDs, priorities
    * Shared abstractions used across UI / engine / persistence
    * No SMAPI dependencies

3. Task Engine

    Responsibilities:

    * Produce tasks for a given day
        Based on:
        * built-in generators (automatic tasks)
        * user-defined rules (Task Builder tasks)
        * persisted manual tasks
    * Evaluate progress and determine completion automatically where applicable
    * Maintain stable identifiers so tasks don’t “shuffle” across reloads

4. Task Builder System

    Responsibilities:

    * Store user-defined rules in a stable format
    * Provide rule validation
    * Provide rule evaluation (in engine)
    * Provide wizard authoring UI (in menu)
    * Preserve rule-definition boundaries (no direct runtime task mutation)

5. State Store

    Responsibilities:

    * Single authoritative runtime store

        Stores:

        * today’s tasks
        * progress states
        * user completions/dismissals
        * history snapshots
    * Expose immutable snapshots for UI binding
    * Enforce mutation only through commands/actions

6. Persistence

    Responsibilities:

    * Save/load of mod data per Stardew save
    * Versioned save model
    * Migration pipeline
    * Daily snapshot storage

7. UI (HUD + Menu)

    Responsibilities:

    * Render tasks
    * Provide interactions (scroll/select/day browse/etc.)
    * Provide wizard and dashboard
    * Never directly mutate domain state except via defined commands

8. Analytics & Statistics (V2)

    Note: Statistics and analytics are deferred to Version 2. The Daily
    Snapshot Ledger (Section 11) provides the raw data. Statistics
    computation and the statistics dashboard UI will be designed and
    implemented in a future version.

    Responsibilities (V2):

    * Aggregate history into meaningful stats
    * Provide queryable “stats model” for UI
    * Keep stats derivable from history (avoid fragile duplication)

## 2.5 Event Flow (Mod Lifecycle) ##

### On Save Loaded ###

1. Load persisted mod data
2. Migrate save data to current version (if needed)
3. Hydrate runtime store
4. Generate/refresh tasks for “today” using current context
5. Notify UI models

### On Day Start ###

**Sleep flow (`OnSaving`) — ownership:**

| Step | Owning Layer | Notes |
|------|-------------|-------|
| Receive `OnSaving` signal | `LifecycleCoordinator` | Signal-only forwarding; no data ownership |
| Capture previous day's snapshot | `DailySnapshotLedger` | Reads State Store snapshot; writes to ledger at sleep time |
| Persist all data to disk | Persistence | Rules, manual tasks, baselines, user state, ledger |

**New-day flow (`OnDayStarted`) — ownership:**

| Step | Owning Layer | Notes |
|------|-------------|-------|
| Receive `OnDayStarted` signal | `LifecycleCoordinator` | Signal-only, ordered forwarding |
| Remove expired daily tasks | `StateStore` (via `DayTransitionHandler`) | Owns task expiration; no external commands emitted |
| Reset daily baselines | `StateStore` (via `DayTransitionHandler`) | Clears `CaptureDaily` baseline values |
| Run new-day evaluation pass | Task Engine | Evaluates all sources; emits commands to store |
| Publish refreshed snapshot | `StateStore` | After all commands applied |
| Update HUD/menu | `HudHost` / view models | Receive snapshot via `SnapshotChanged` event |

**Normative note:** The `LifecycleCoordinator` forwards signals in order
without owning any data. The snapshot is captured at sleep time (`OnSaving`),
not at the start of the next day, to ensure it reflects the true final state of
the day.

### On Player Sleep (End of Day) ###

The sleep flow ownership table above is the normative specification for
end-of-day processing.

### On Relevant Game Events (During Day) ###

Task progress and completion can update via:

    - periodic tick (throttled)
    - inventory change
    - machine finished
    - quest update
    - calendar/day changes
    - player location change

Key requirement:

    - We need an evaluation strategy that prevents overwork.

#### On Returned to Title ####

When the player returns to the title screen, the mod must tear down all
runtime state to prevent stale data from leaking into a subsequent save
load.

**Canonical teardown specification:** This section (§2.5 "On Returned to
Title") is the single authoritative source for the teardown sequence. Section
12 §12.11.1 cross-references this section rather than restating the sequence.
Any future teardown additions must be made here only.

1. Dispose all View Models and unsubscribe from SnapshotChanged
2. Dispose the HUD drawable
3. Clear the State Store
4. Drain and discard the evaluation queue
5. Drain and discard the outbound effect queue
6. Null cached game state references
7. Reset engine lifecycle to pre-initialization state

The mod must be safe to re-initialize on the next `OnSaveLoaded`
without residual state from the previous session.

## 2.6 Localization Boundary (SMAPI I18n) ##

Localization is a UI concern at the architecture boundary.

Hard boundary rules:

    - Backend/domain/state/persistence layers remain locale-neutral.
    - The State Store remains the single source of truth and stores canonical
    semantic task state, not locale-rendered display text.
    - UI surfaces (HUD and menu) resolve localized strings at render/binding
    time using SMAPI `I18n`.
    - Engine outputs intended for display should be represented as stable
    semantic keys (and optional arguments) instead of pre-rendered localized
    strings.

Phase constraint:

    - Any translation-related change that would alter existing Phase 1 or
    Phase 2 runtime behavior is explicitly deferred to Phase 3+
    implementation work.
