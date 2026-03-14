# Section 1 — Product Definition #

## 1.1 Mod Concept ##

This mod provides an **automatic task management system integrated directly into**
**Stardew Valley gameplay.**

The system allows players to:

- Automatically track common gameplay tasks
- Build custom automatic tracking rules through a Task Builder
- Create manual tasks when automation is not possible

The mod functions as an in-game productivity and task tracking system, designed
to reduce mental overhead and help players keep track of goals, responsibilities,
and reminders.

The mod includes:

Version 1 features:

- Automatic task tracking (built-in generators)
- Manual task creation (Task Builder)
- Manual tasks
- Daily task tracking
- Historical task snapshots
- Completion tracking
- HUD display and full task management menu

Version 2 (planned):

    - Statistics and analytics

## 1.2 Task Types ##

The mod supports three core task types.

1. Automatic Tasks (Built-in)

    These tasks are generated automatically by the mod based on game state detection.

    Examples include:

    * Crops needing watering
    * Crops ready for harvest
    * Animals needing petting
    * Machines finished processing
    * NPC birthdays
    * Shops being closed
    * Quest progress tracking

    Characteristics:

    * Generated automatically
    * Completed automatically when the condition is met
    * Reset or update daily when appropriate

    These tasks represent common gameplay activities the mod tracks out of the
    box.

2. Task Builder Tasks (User-defined Automatic Tasks)

    Players can create their own automatic tracking rules using the Task Builder
    system.

    The Task Builder lets players define custom automatic goals, such as
    tracking progress toward collecting 300 wood for a building. The system
    detects the player’s current wood count and marks the task complete when
    the requirement is met.

    Additional examples include:

    * Collect X resources
    * Complete a quest objective
    * Reach a skill level
    * Craft a specific item
    * Machine output monitoring
    * Calendar reminders

    Characteristics:

    * Automatically tracked
    * Persist until completed
    * Highly configurable
    * Designed to be extensible over time

    The Task Builder system is expected to be the most complex part of the mod
    and will evolve across future versions.

3. Manual Tasks

    Manual tasks allow players to add reminders that cannot be automatically
    detected or are not yet in release features.

    Examples:

    * “Decorate house”
    * “Upgrade pickaxe on Spring 7”

    Characteristics:

    * Created manually by the player
    * Completed manually by the player
    * Persist until completed
    * Manual tasks serve as a fallback system when automation is not possible yet.

## 1.3 Task Completion ##

Task completion behavior depends on task type.

| Task Type | Completion Method |
| ------------------ | ------------------------------------ |
| Automatic Tasks | Automatically detected by game state |
| Task Builder Tasks | Automatically detected by rule logic |
| Manual Tasks | Manually completed by player |

Automatic completion ensures the system **reflects real gameplay progress**
**without requiring manual interaction.**

## 1.4 Task Lifecycle ##

Tasks may behave differently depending on their nature.

### Daily Tasks ###

Some tasks reset each day because they represent daily activities.

Examples:

    - Water crops
    - Pet animals
    - Harvest crops

These tasks are regenerated or updated each day.

#### Persistent Tasks ####

Other tasks persist until completed.

Examples:

    - Quest tracking
    - Resource collection goals
    - Task Builder resource goals
    - Manual tasks

These tasks remain active across days until completed

## 1.5 Daily Snapshot System ##

At the end of each day (when the player sleeps), a **snapshot of all tasks** is
stored containing:

    - Completed tasks
    - Incomplete tasks
    - Task progress

This snapshot becomes part of the **task history system**.

This enables:

    - Viewing previous days’ tasks
    - Historical tracking
    - Statistics generation (V2)

Snapshot export (V2): daily snapshots may be exported and viewed outside
of the game in a future version, but cannot be modified and re-imported
into the game.

## 1.6 Completed Task Handling ##

When tasks are completed:

    - They move to a completed task list

The HUD may optionally display completed tasks depending on user configuration.

## 1.7 Task Organization ##

Tasks are categorized in the menu interface.

Possible categories include:

    - Farming
    - Animals
    - Machines
    - Social
    - Exploration
    - Resources
    - Custom

The HUD may optionally group tasks by category depending on configuration.

## 1.8 Deterministic Task-Type Ordering ##

Tasks use a deterministic ordering derived from task type rather than a
stored per-task field.

Deterministic ordering determines:

    - Sorting order
    - HUD display order
    - Visual emphasis

Future versions may allow user-configurable ordering profiles/mechanisms
that still resolve to deterministic ordering behavior.

## 1.9 History and Statistics ##

The mod stores historical task snapshots.

Version 1 features:

    - Viewing previous days' task lists
    - Browsing completed and incomplete tasks for a selected day

Statistics features (V2):

    - Completion statistics
    - Task frequency
    - Productivity trends

Statistics and analytics are deferred to Version 2. The Daily Snapshot
Ledger (Section 11) provides the raw data needed to support these
features in a future version.

## 1.10 Multiplayer ##

Multiplayer functionality will not be included in the initial release.

Multiplayer support may be considered in future versions.

## 1.11 Design Philosophy ##

The mod is designed as an:

Automatic Task Management System inside Stardew Valley

Goals:

    - Reduce mental overhead for players
    - Track goals and responsibilities automatically
    - Provide powerful customization through the Task Builder
    - Maintain a lightweight HUD with deeper functionality in menus
