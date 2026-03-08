# Section 4 — Core Data Model #

## 4.1 Task Identity ##

Every task in the system must have a deterministic identifier.

Canonical Task ID structure, composition rules, and stability requirements are
defined in Section 3.

This section relies on those rules rather than redefining identifier mechanics.

## 4.2 Task Object Model ##

Each task instance in the system is represented by a Task Object containing all
data required for UI display, evaluation, and persistence.

A task object contains the following conceptual fields:

    - Task ID
    - Task Type
    - Task Category
    - Task Title
    - Task Description
    - Task Status
    - Task Progress
    - Task Progress Target
    - Task Creation Day
    - Task Completion Day
    - Task Source Identifier

These fields allow the task engine, UI systems, and persistence layer to operate
independently while referencing the same task data.

## 4.3 Task Status ##

Version 1 of the system supports two task states:

    - Incomplete
    - Completed

Tasks transition from Incomplete → Completed when their completion condition is
satisfied.

Completion behavior varies depending on task type.

    - Automatic Tasks complete when their associated game condition becomes true.
    - Task Builder Tasks complete when their defined rule condition is satisfied.
    - Manual Tasks are completed manually by the player.

Future versions may introduce additional status types such as:

    - Dismissed
    - Hidden
    - Failed
    - Snoozed

However these are intentionally excluded from Version 1 to reduce system complexity.

## 4.4 Task Progress Model ##

Some tasks require progress tracking rather than simple completion checks.

Examples include:

    - Collecting resources
    - Crafting items
    - Reaching skill levels
    - Completing multi-step goals

Progress-based tasks store two values:

    - Current Progress
    - Target Progress

Example:

    Collect Wood
    Progress: 120 / 300

Baseline-based progress behavior is defined in Section 7.6.

This section defines the data model fields only.

### 4.4.1 Progress Fields as Tracking Metrics ###

ProgressCurrent and ProgressTarget are **tracking metrics only**.

These fields serve three purposes:

    - Progress tracking for display purposes
    - HUD and UI progress bar rendering
    - Support for rule evaluation logic

**Progress saturation does not imply automatic task completion.**

TaskStatus completion is determined by the task's completion condition, not
solely by whether ProgressCurrent >= ProgressTarget.

Some tasks complete when progress reaches the target value.

Other tasks may reach or exceed the target while remaining incomplete until
additional conditions are satisfied.

Example:

    Task: Gather 300 wood to build a Coop
    ProgressCurrent: 305
    ProgressTarget: 300
    Status: Incomplete

    Reason: The player must still build the Coop. Progress saturation alone does
    not satisfy the task's completion condition.

TaskStatus represents the authoritative completion state as determined by the
task's rule or condition system.

## 4.5 Task Categories ##

Version 1 uses fixed category definitions implemented as an internal enumeration.

Example categories include:

    - Farming
    - Animals
    - Machines
    - Social
    - Exploration
    - Resources

Categories are used for:

    - Menu organization
    - HUD grouping (optional)
    - Statistical analysis

Future versions may introduce user-defined categories.

## 4.6 Deterministic Task-Type Ordering ##

Default ordering behavior in the HUD and task menu is derived from task type,
plus a fixed deterministic fallback chain.

Version 1 ordering chain:

1. Pinned tasks first (pin state comes from the State Store).
2. Task-type rank from an in-code deterministic ordering map.
3. Task Creation Day.
4. Canonical Task ID string.

Example default task-type ordering map:

    - Attend festival
    - Spouse interaction
    - Pet interaction
    - Petting/Milking/Shearing Farm Animals
    - Collect animal products
    - Water crops
    - Harvest crops
    - Harvest trees
    - Collect from machines
    - Collect needed resources
    - Social tasks
    - etc.

The ordering chain is derived at runtime and does not require a persisted
per-task priority field in the Task Object.

Pinned tasks are stored in the State Store and persisted between sessions.

In Version 1 players cannot modify ordering directly.

Future versions may allow user-configurable ordering profiles/mechanisms that
still resolve to deterministic task-type ordering with the same fallback
behavior.

## 4.7 Task Source Types ##

Tasks originate from three sources.

1. Automatic Task Generators

    Built-in task detection systems that monitor gameplay conditions.

2. Task Builder Rules

    User-defined rule systems that generate custom automated tasks.

3. Manual Tasks

    Tasks created manually by the player.

The task source type is stored with each task instance to allow the engine to determine:

    - evaluation logic
    - persistence behavior
    - completion rules

## 4.8 Task Snapshot Model ##

At the end of each in-game day a Daily Task Snapshot is recorded.

A snapshot contains:

    - Task ID
    - Task Title
    - Task Category
    - Task Status
    - Task Progress
    - Task Progress Target

Snapshots allow the system to reconstruct:

    - historical task lists
    - completion statistics
    - productivity metrics

Snapshots are read-only records.

They can be exported for viewing outside the game but cannot be modified and re-imported.

## 4.9 State Store Model ##

The mod uses a centralized State Store that acts as the authoritative runtime
data source.

The State Store maintains:

    - Active tasks for the current day
    - Progress values
    - Task completion states
    - Pinned task information
    - Daily snapshot history

Key design principles:

    - UI systems never mutate task objects directly
    - All task changes occur through defined commands
    - The store exposes immutable snapshots for UI rendering

This architecture ensures that UI systems, the task engine, and persistence
logic remain cleanly separated
