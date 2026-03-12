# Section 3 — Deterministic Identifier Model #

## 3.1 Purpose ##

The task system relies heavily on deterministic identifiers to ensure that
tasks remain stable across reloads, engine evaluations, and save cycles.

Deterministic identifiers guarantee that:

    - Tasks generated from the same rule or generator always produce the same ID
    - Task completion state persists correctly across sessions
    - Task history records remain accurate
    - Duplicate task creation is prevented
    - UI elements can reliably reference task objects

Identifiers are internal implementation details and are not exposed to players.

## 3.2 Identifier Types ##

The system uses several identifier types.

Primary identifiers include:

    - TaskID
    - RuleId
    - DayKey
    - SubjectId (optional)

Each identifier serves a specific role in maintaining system stability.

## 3.3 TaskID Structure ##

TaskID uniquely identifies a task instance.

TaskIDs are deterministic strings derived from the task source and its
associated subject.

Conceptual structure:

TaskID = `{SourcePrefix}_{RuleId}_{SubjectId?}_{DayKey?}`

Components:

    - `SourceType`
    Identifies task origin.
    Possible values:
        * `BuiltIn`
        * `TaskBuilder`
        * `Manual`
    Note: Both `TaskSourceType` enum and manual `TaskID` source prefix use `Manual`.
    - `RuleOrGeneratorID`
    Identifies the rule or generator responsible for producing the task.
    - `SubjectIdentifier`
    Represents the object or entity associated with the task.

Examples include:

    - `ItemID`
    - `AnimalID`
    - `MachineID`
    - `SkillType`

`ContextIdentifier` - Optional component representing contextual scope such as
location or day.

## 3.4 Task Builder Task IDs ##

Task Builder tasks derive their identity from the Rule ID and subject.

Conceptual structure:

`TaskBuilder_{RuleId}_{SubjectId?}_{DayKey?}`

Examples:

    - Persistent rule: `TaskBuilder_17_ItemWood`
    - Daily rule: `TaskBuilder_42_Daily_Year2-Spring12`

Including the day key ensures that daily recurring tasks generate a new task
instance for each day while still remaining deterministic.

## 3.5 Built-in Generator Task IDs ##

Built-in automatic tasks generate IDs based on their generator and subject.

Example structures:

    - Crop watering task: `BuiltIn_CropWatering_FarmTile_12_14`
    - Animal care task: `BuiltIn_AnimalPet_Cow_3`
    - Machine output task: `BuiltIn_MachineOutput_Keg_15`

Generator implementations are responsible for defining stable subject
identifiers.

## 3.6 Manual Task IDs ##

Manual tasks are created by the player and do not derive from rule systems.

Manual task IDs are generated using a monotonically incrementing counter.

Example:

    - `Manual_3`
    - `Manual_4`
    - `Manual_5`

The counter MUST be persisted as part of the `StoreUserState` structure
(Section 9.8) to ensure it survives across sessions.

The counter MUST only increment, never reset or regress.

Manual task identifiers persist across sessions and are stored in persistence.

## 3.7 RuleId Model ##

`RuleId` is an immutable value object that wraps a normalized rule token.

Current implementation behavior:

    - Outer whitespace is trimmed before validation.
    - Null, empty, and whitespace-only values are rejected.
    - Equality and hash semantics are ordinal and case-sensitive.
    - Sequential RuleId issuance is deferred until rule-generation flow is implemented.

Example values:

    - `Rule-Alpha`
    - `Rule-17`

These invariants keep rule-based identity stable across reconstruction and comparison paths.

## 3.8 DayKey Model ##

Daily tasks and history snapshots rely on a deterministic `DayKey`.

Conceptual structure:

    - `Year{N}-{Season}{D}`

Example:

    - `Year1-Summer15`
    - `Year3-Fall28`

`DayKey` is used in several systems:

    - Daily task identity
    - Daily snapshot history
    - Deadline calculations
    - Statistics aggregation

Current implementation behavior:

    - Canonical shape is exactly `Year{N}-{Season}{D}`.
    - Season token must match one of `Spring`, `Summer`, `Fall`, or `Winter` using invariant case.
    - Year must be a positive integer and day must be in the range 1-28.
    - Outer whitespace is trimmed, but non-canonical casing or structure is rejected.

## 3.9 Subject Identifier Model ##

Subject identifiers represent the in-game entity associated with a task.

Examples include:

    - `ItemID`  
  Used for inventory tracking tasks.

    - `MachineID`  
  Used for machine output tasks.

    - `AnimalID`  
  Used for animal care tasks.

    - `TileCoordinates`  
  Used for crop or location-based tasks.

Subject identifiers must be deterministic and stable across sessions.

## 3.10 Identifier Collision Prevention ##

The identifier system must prevent collisions between task sources.

Rules include:

    - `SourceType` prefixes separate manual, built-in, and rule-generated tasks
    - `RuleId` values remain unique
    - `SubjectIdentifier` values remain stable
    - `ContextIdentifier` values differentiate scoped tasks

If a collision occurs it is considered a development bug.

## 3.11 Identifier Stability Requirements ##

Identifiers must remain stable across:

    - Game reloads
    - Save migrations
    - Rule edits that do not affect task identity
    - Engine evaluation cycles

Breaking identifier stability can cause:

    - Task duplication
    - Loss of completion state
    - Corrupted history records

## 3.12 Version 1 Constraints ##

Version 1 identifier rules remain intentionally simple.

Excluded features include:

    - Hash-based identifiers
    - Multiplayer synchronization IDs
    - Cross-save global identifiers

The initial identifier model focuses on deterministic stability within a
single save file.

## 3.13 Localization and Identity Invariants ##

Localization must never influence deterministic identity behavior.

Hard invariants:

    - Localized strings MUST NOT participate in `TaskID`, `RuleId`, `DayKey`,
    or `SubjectId` generation.
    - Localized strings MUST NOT participate in identifier equality,
    reconciliation, or collision checks.
    - Localized strings MUST NOT participate in deterministic ordering or sort
    fallback chains.
    - `DayKey` remains locale-neutral and uses fixed canonical season tokens.

Display text may vary by locale, but identity semantics remain invariant.
