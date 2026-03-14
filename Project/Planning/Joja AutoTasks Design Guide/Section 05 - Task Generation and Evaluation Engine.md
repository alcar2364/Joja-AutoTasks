# Section 5 — Task Generation and Evaluation Engine #

## 5.1 Purpose ##

The Task Generation and Evaluation Engine is responsible for creating and
updating tasks from all task sources.

The engine must:

    - Generate tasks deterministically
    - Evaluate progress and completion state
    - Update tasks at appropriate times without causing performance issues
    - Merge tasks from all sources into a unified task list for the State Store
    - Support a highly extensible Task Builder rule system

The engine operates on a unified output model:

    - Automatic Task Generators output tasks
    - Task Builder Rules output tasks
    - Manual Tasks output tasks

All tasks then flow through a shared pipeline and into the State Store.

## 5.2 Task Sources and Engine Inputs ##

The engine consumes three sources of task definitions.

1. Built-in Automatic Task Generators

    * Hardcoded generators implemented as independent modules.
    * Each generator is responsible for a defined gameplay domain
    * Generators create tasks based on game state detection

2. Task Builder Rules

    * Player-authored rules created through the wizard interface.
    * Rules are stored in a stable serialized format
    * Rules are evaluated by the rule engine to create tasks

3. Manual Tasks

    * Player-authored tasks created manually.
    * Manual tasks do not require evaluation for progress unless optionally
        supported later
    * Manual tasks persist until manually completed

The engine produces tasks using a single unified context input:

    - Generation and Evaluation Context

## 5.3 Generation and Evaluation Context ##

The engine constructs a Generation/Evaluation Context which is a snapshot of
relevant game state needed for evaluation.

The context exists to:

    - Reduce repeated expensive lookups
    - Provide deterministic evaluation inputs
    - Allow caching and throttling
    - Decouple generators and rules from direct event dependencies
    - Enable unit testing without a running game

The context MUST be structured as a collection of **domain-specific slices**
that are lazily materialized. This ensures that a rule needing only inventory
data never pays the cost of scanning farm state.

Conceptual structure:

    public sealed class EvaluationContext
    {
        public TimeContext Time => _time ??= BuildTimeContext();
        public PlayerContext Player => _player ??= BuildPlayerContext();
        public InventoryContext Inventory => _inventory ??= BuildInventoryContext();
        public WorldContext World => _world ??= BuildWorldContext();
        public FarmContext Farm => _farm ??= BuildFarmContext();
    }

Each domain slice is built on first access and cached for the duration of
the evaluation cycle.

### 5.3.1 Game State Abstraction ###

Generators and rules MUST NOT call game APIs directly (e.g.,
`Game1.player.Items`, `Game1.getFarm()`). All game state access MUST go
through abstraction interfaces that the EvaluationContext consumes.

Conceptual interface:

    public interface IGameStateProvider
    {
        IReadOnlyList<Item> GetPlayerItems();
        IReadOnlyList<FarmAnimal> GetFarmAnimals();
        IReadOnlyList<GameLocation> GetLocations();
        int GetSkillLevel(SkillType skill);
        // ... domain-specific accessors
    }

| Method | V1 Required | Purpose |
|--------|-------------|---------|
| `GetPlayerItems()` | ✅ V1 | Inventory-based generators and rules |
| `GetFarmAnimals()` | ✅ V1 | Animal care generator |
| `GetLocations()` | ✅ V1 | Machine output generator (scans locations for machines) |
| `GetSkillLevel(SkillType)` | ✅ V1 | Skill-level conditions in Task Builder rules |
| `GetCurrentDayKey()` | ✅ V1 | All generators (day-keyed task identity) |
| `GetCurrentTime()` | ✅ V1 | Time-window evaluation (`ExpiresAtTime`) |
| `GetCalendarEvents()` | ✅ V1 | Calendar generator (birthdays, festivals) |
| `GetActiveQuests()` | ✅ V1 | Quest progress generator |
| `GetCropsNeedingWater()` | ✅ V1 | Crop watering generator |
| `GetCropsReadyToHarvest()` | ✅ V1 | Crop harvest generator |
| `GetNPCBirthdays()` | ✅ V1 | Birthday generator |
| `GetHouseUpgradeLevel()` | V2+ | House upgrade condition support |
| `GetToolUpgradeState()` | V2+ | Tool upgrade condition support |

Methods marked V2+ are not required for the V1 generator set defined in
§13.10. They may be added in future phases without breaking the V1 interface
contract.

This abstraction enables:

    - Unit testing rule evaluation with mock game state
    - Deterministic evaluation isolated from the game environment
    - Clean separation between engine logic and SMAPI/game coupling

The concrete implementation of `IGameStateProvider` wraps actual game
API calls and is injected during mod initialization.

The context includes only the data needed for supported task types and rules.

Example context domains:

    - Time domain
        * Current day key
        * Current season/year
        * Day-of-month
        * Day-of-week
        * Current in-game time
        * Festival and special day indicators
    - Player domain
        * Skill levels
        * Current location
        * Relationships
        * Tool upgrade state (if accessible)
        * House upgrade state (if accessible)
    - Inventory domain
        * Item counts by ID
        * Currency
    - World domain
        * Weather
        * Calendar events (birthdays, shop schedules)
        * Quest state
        * Location-scoped machine readiness index (machines and ready outputs)
    - Farm domain
        * Animals and animal products
        * Crops needing water or harvest
        * Fruit Trees or other harvestable trees needing harvest

The context is built lazily where possible and cached per evaluation cycle.

## 5.4 Built-in Automatic Task Generators ##

Built-in tasks are implemented as independent generators.

Each generator:

    - Is responsible for one gameplay domain
    - Produces a set of deterministic tasks
    - Does not store mutable state inside the generator
    - Uses the shared context instead of direct SMAPI queries where possible

V1 built-in generators:

    - Crop care generator
        * Watering
        * Harvest readiness
    - Animal care generator
        * Petting
        * Milking / shearing readiness
        * Collecting animal products
    - Machine output generator
        * Machines finished processing
        * Items ready to collect
    - Calendar generator
        * Birthdays
        * Festivals
        * Shop closure reminders
    - Quest generator
        * Active quest progress tasks

Generators output tasks into the shared pipeline.

The normative V1 generator set is defined in Section 13 §13.10. The generators
listed here are illustrative of the domain structure; §13.10 is the
authoritative V1 commitment.

## 5.5 Task Builder Rule Engine ##

Task Builder rules are evaluated using a composable logic tree.

The rule engine must support:

    - AND / OR / NOT logic
    - Persistent goals and daily recurring goals
    - Progress tracking goals
    - Deadline-based goals and reminders
    - Deterministic task identity generation per rule

Task Builder evaluation is split into two conceptual steps:

1. Rule evaluation determines whether a task exists and what its progress is.
2. Task completion determination decides whether it is Completed.

The rule engine produces the same Task Object model as built-in generators.

## 5.6 Rule Model Overview ##

A Task Builder Rule is represented internally as:

    - Rule identity
    - Rule metadata
    - Trigger model
    - Condition tree
    - Progress model
    - Output model

Rule identity:

    - Each rule has a stable identifier used to create deterministic task
    identifiers
    - Deterministic Task ID structure and stability requirements are defined in
    Section 3.3 and Section 3.11.
    - Task Builder-specific identity composition is defined in Section 3.4.

Rule metadata includes:

    - User-facing task title
    - Task category (fixed enum in V1)
    - Optional description
    - Optional icon selection

Trigger model:

    - A trigger defines when a rule is evaluated and when it can produce tasks
    - Triggers include:
        * Day start triggers
        * Time-of-day triggers
        * Event triggers (inventory changes, location changes, machine ready, quest
        updates)
        * Periodic evaluation triggers (throttled) (per tick, on time change, etc.)
    - Triggers are used to avoid unnecessary evaluation
    - Runtime trigger filtering and dirty-flag scheduling semantics are defined in
    Section 7.8.

Condition tree:

    - Conditions define whether the task should exist and what completion means
    - Condition tree supports:
        * AND
        * OR
        * NOT
    - Leaf conditions support at least the following categories in V1:
        * Item count conditions
            * Own at least N of item X
            * Collect N more of item X
        * Skill level conditions
            * Skill >= N
        * Progress conditions
            * Reach goal by a deadline
            * Reach goal by a day key
        * Location/time conditions (for reminders)
            * Day-of-week
            * Day-of-month
            * In-game time window
            * Festival day / shop closure day

Progress model:

    - Rules can define progress tracking
    - Progress tracking includes:
        * Baseline-based progress
        * Current progress computed from context
        * Target progress stored in the task
    - Rules may choose whether baseline is required
    - Baseline examples:
        * Collect 300 wood more than current inventory
        * Gain 2 farming levels from current value
    - Runtime baseline capture semantics are defined in Section 7.6.

Output model:

    - Rule outputs define what task is produced
    - Output types include:
        * Progress task output
            * A task with progress and completion logic
        * Reminder task output
            * A task whose main purpose is notifying about a time-based event
    - Reminder outputs support:
        * Day-based reminders (tomorrow, by date)
        * Time-based reminders (at 5:00pm)

## 5.7 Deadline and "By Date" Tasks ##

The Task Builder must support goals such as:

    - Silos full of hay by Fall 28
    - Reach floor 70 of the mines by Spring 25
    - Reach farming skill level 8 by Summer 20
    - Own 8 iridium sprinklers by Fall 15
    - Upgrade house by Fall 16

Deadline tasks include a Due Day Key.

In Version 1 the Task Status model remains:

    - Incomplete
    - Completed

Therefore deadline behavior is represented as derived properties rather than new
statuses.

Deadline derived properties include:

    - Due Day Key
    - IsOverdue (computed)
    - DaysRemaining (computed)

Tasks remain Incomplete when overdue unless completed.

This avoids introducing Failed/Expired statuses in Version 1 while still
enabling meaningful UI warnings and statistics.

## 5.8 Daily Recurring Tasks ##

The Task Builder must support daily recurring tasks such as:

    - Cut down 5 trees every day

Daily recurring tasks:

    - Create a daily instance keyed to the current day key
    - Record baseline values at the start of the day when required
    - Reset progress daily through daily task generation

Detailed daily baseline behavior is defined in Section 7.6.2.

Daily tasks are stored in the daily snapshot system so that players can review:

    - whether the task was completed that day
    - the progress achieved that day

## 5.9 Evaluation Scheduling and Performance ##

The engine uses a hybrid scheduling model to balance responsiveness and
performance.

Canonical rule scheduling behavior is defined in Section 7.8.

Engine lifecycle timing and phase orchestration are defined in Section 12.

## 5.10 Task Generation Pipeline ##

All task sources flow into a single pipeline.

Pipeline stages:

1. Generate candidate tasks

    * Built-in generators
    * Task Builder rules
    * Manual tasks

2. Normalize tasks

    * Ensure deterministic IDs
    * Ensure required fields exist
    * Apply category and presentation defaults

3. Merge and reconcile

    * Resolve collisions using deterministic IDs
    * Prefer a single authoritative task instance per ID

4. Apply deterministic task-type ordering

    * Use a derived deterministic task-type ordering map
    * Apply fixed fallback chain (Task Creation Day, then canonical Task ID)
    * Apply pinned tasks at top from the State Store

5. Evaluate completion and progress

    * Automatic completion rules
    * Task Builder completion rules
    * Manual tasks remain incomplete until manually completed

6. Publish to State Store

    * Store new task list
    * Notify UI

## 5.11 Examples of Task Builder Rules ##

The following example tasks represent required Task Builder capabilities.

1. Silos full of hay by Fall 28

    * Deadline goal with a farm state condition
    * Derived overdue state when past due date

2. Collect 300 wood to build Coop

    * Inventory baseline rule
    * Progress = current wood - baseline
    * Completed when baseline + 300 reached

3. Reach floor 70 of the mines by Spring 25

    * Deadline goal with a progression state condition
    * Progress = deepest floor reached

4. Cut down 5 trees every day

    * Daily recurring goal
    * Baseline taken at day start
    * Completed when baseline + 5 reached

5. Reach farming skill level 8 by Summer 20

    * Deadline + skill condition

6. Own 8 iridium sprinklers by Fall 15

    * Deadline + item count condition

7. Upgrade house by Fall 16

    * Deadline + house upgrade state condition

8. Upgrade watering can

    * Tool upgrade state condition
    * Persistent until completed

## 5.12 Version 1 Constraints ##

Version 1 engine constraints:

    - Task status remains Incomplete/Completed only
    - Task dismissal is excluded from V1
    - Deadline failure is represented via derived properties rather than Failed
    status
    - Multiplayer support is excluded from V1

These constraints intentionally reduce complexity while still supporting the
core value proposition of powerful user-authored rules.

## 5.13 Localization-Neutral Engine Output ##

The engine output model must remain semantic and locale-neutral.

Rules:

    - Generators and rule evaluation produce stable task semantics,
    identifiers, and machine-usable output fields.
    - Engine output intended for UI text should use stable localization keys
    with optional formatting arguments where needed.
    - Engine and evaluators MUST NOT emit locale-rendered strings as canonical
    output.
    - Any user-authored free text (for example manual task titles) is treated
    as user data, not as an identity input.

This preserves deterministic behavior and keeps translation resolution at the
UI boundary.
