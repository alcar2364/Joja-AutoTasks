# Section 3 — Deterministic Identifier Model

## 3.1 Purpose

The task system relies heavily on deterministic identifiers to ensure that tasks remain stable across reloads, engine evaluations, and save cycles.

Deterministic identifiers guarantee that:

- Tasks generated from the same rule or generator always produce the same ID
- Task completion state persists correctly across sessions
- Task history records remain accurate
- Duplicate task creation is prevented
- UI elements can reliably reference task objects

Identifiers are internal implementation details and are not exposed to players.

## 3.2 Identifier Types

The system uses several identifier types.

Primary identifiers include:

- `TaskId`
- `WizardKey`
- `DayKey`
- `SubjectId` (optional)

Each identifier serves a specific role in maintaining system stability.

Naming note:

- `TaskId` is the canonical type and documentation name.
- Legacy uppercase `TaskID` wording may still appear in older artifacts when referring to conceptual string formats, but new code and documentation should use `TaskId`.

## 3.3 TaskId Structure

TaskId uniquely identifies a task instance.

TaskIds are deterministic strings derived from the task source and its associated subject.

Each category, key, or identifier component follows specific rules to ensure stability and collision prevention. The combination of each should result in unique TaskIds for each task instance.

Conceptual structure:

TaskId = `"{SourcePrefix}{GeneratorKey/WizardKey/ManualKey}_{DayKey}_{SubjectId?}_{ContextId?}"`

Components:

- `SourcePrefix` consists of a `TaskSourceType` which identifies task origin by category. Possible values:
  - `Auto`
  - `Wizard`
  - `Manual`
- `DayKey` corresponds to the in-game day the task was created
- `AutoKey`, `WizardKey` or `ManualKey` is a unique number originating from the task source. In V1, the number is derived from an auto-incrmenting counter. This is a deterministic safeguard to ensure all TaskIds are unique even if other components (like SubjectId) are not sufficient to guarantee uniqueness on their own.
  - `Auto` sourced tasks that are built-in and automatically tracked use an `AutoKey`.
  - `Wizard` sourced tasks from the task builder use a `WizardKey`
  - `Manual` sourced tasks created by the user use a `ManualKey`
- `SubjectId` Represents the object or entity associated with the task. It is optional depending on task source.
  Examples include:
  - `ItemId`
  - `AnimalId`
  - `MachineId`
  - `SkillType`
- `ContextId` - Optional component representing contextual scope such as location

## 3.4 Built-in Generator Task IDs

Built-in automatic tasks generate IDs based on their generator and subject.

Conceptual structure:

`$"Auto{AutoKey}_{DayKey}_{SubjectId}_{ContextId?}"`

Example structures:

- Crop watering task: `Auto28_Year2_Fall12_CropWatering_FarmTile_12_14`
- Animal care task: `Auto15_Year1_Winter5_AnimalPet_Bessie`
- Machine output task: `Auto_Year3_Spring_27_Machine_Keg_BeachTile_5_10`

Generator implementations are responsible for defining stable subject identifiers. Each generator will use its own counter to produce the `AutoKey`. Most generators will also provide context identifiers unique to their domain (e.g., farm tile coordinates for crop tasks, machine type and location for machine tasks).

## 3.5 Task Builder Task IDs

Conceptual structure:

`$"Wizard_{WizardKey}_{DayKey}_{SubjectId}_{ContextId?}"`

Examples:

- No context: `Wizard_15_Year2_Summer8_ItemWood`
- Context: `Wizard_28_Year1_Spring17_TreeCut_Daily`

TaskIds can be built with or without context. Context is optional and depends on the task being generated. For example, a task to cut a certain amount of trees every day may include `Daily` context, while a more general task like "Collect 300 wood" may not require context. The presence of context should be determined by the generator logic based on whether it is needed to ensure uniqueness or provide necessary information for the task.

## 3.6 Manual Task IDs

Manual tasks are created by the player and do not derive from rule systems.

Manual task IDs are generated using their creation date and a monotonically incrementing counter.

Conceptual structure:

`$"Manual_{ManualKey}_{DayKey}"`

Example:

- `Manual_3_Year2_Spring5`
- `Manual_3_Year2_Spring6`
- `Manual_4_Year2_Spring6`

**Normative rule:** `Manual_{ManualKey}_{DayKey}` is the canonical prefix for manual task identifiers. The prefix `ManualTask_N` is a non-canonical legacy variant and must not be used in new code or documentation. The canonical form aligns with the `SourceType` prefix pattern used in Section 3.3 and Section 3.5.

## 3.7 `AutoKey`, `WizardKey`, and `ManualKey` Models

Normative definition:

These keys are immutable value objects that follows an auto-incrementing integer counter pattern for tasks generated in their specific source.  

- `AutoKey` is used for built-in automatic tasks and is managed by each generator. So each generator will have its own `AutoKey` counter that increments with each task it generates.
- `WizardKey` is used for tasks generated by the Task Builder Wizard and follows a similar auto-incrementing pattern, except there is a single global `WizardKey` counter that increments with each new task generated by the wizard regardless of type.
- `ManualKey` is used for manual tasks created by the player. There is a single global `ManualKey` counter that increments with each new manual task created by the player.
- The value object wraps the counter value as a numeric string internally (e.g., `"17"`).
- Each key is serialized as an integer in JSON (e.g., `"WizardKey": 17`).
- Each counter resets every in game day `OnSaving`, so the same key values may be reused across different days but will never collide within the same day.

## 3.8 DayKey Model

Daily tasks and history snapshots rely on a deterministic `DayKey`. A `DayKey` represnts a in-game day in Stardew Valley and can be used for different purposes.  
`CreationDay` and `CompletionDay` are examples of `DayKey` usage for task lifecycle tracking

Conceptual structure:

- `Year{N}_{Season}{D}`

Example:

- `Year1_Summer15`
- `Year3_Fall28`

`DayKey` is used in several systems:

- Task identity
- Logging the creation day or completion day of a task.
- Daily snapshot history
- Deadline calculations
- Statistics aggregation (out of scope for V1)

Current implementation behavior:

- Canonical shape is exactly `Year{N}_{Season}{D}`.
- Season token must match one of `Spring`, `Summer`, `Fall`, or `Winter` using invariant case.
- Year must be a positive integer and day must be in the range 1-28.
- Outer whitespace is trimmed, but non-canonical casing or structure is rejected.

## 3.9 Subject Identifier Model

Subject identifiers represent the in-game entity associated with a task.

Examples include:

- `ItemId` - Used for inventory tracking tasks and should correspond to the real qualified item id in the game data + DisplayName (see [Modding:Items](https://stardewvalleywiki.com/Modding:Items))

- `MachineId` - This is essentially equivalent to an `ItemId` but specifically for machine. It corresponds to the machine's qualified item id + display name. (see [Modding:Machines](https://stardewvalleywiki.com/Modding:Machines))

- `AnimalId`

Used for animal care tasks.

- `TileCoordinates`

Used for crop or location-based tasks.

Subject identifiers must be deterministic and stable across sessions.

## 3.10 Context Identifier Model



## 3.10 Identifier Collision Prevention

The identifier system must prevent collisions between task sources.

Rules include:

- `SourceType` prefixes separate manual, built-in, and rule-generated tasks
- `RuleId` and manual task counter values remain unique
- `SubjectIdentifier` values remain stable
- `ContextIdentifier` values differentiate scoped tasks

If a collision occurs it is considered a development bug.

## 3.11 Identifier Stability Requirements

Identifiers must remain stable across:

- Game reloads
- Save migrations
- Rule edits that do not affect task identity
- Engine evaluation cycles

Breaking identifier stability can cause:

- Task duplication
- Loss of completion state
- Corrupted history records

## 3.12 Version 1 Constraints

Version 1 identifier rules remain intentionally simple.

Excluded features include:

- Hash-based identifiers
- Multiplayer synchronization IDs
- Cross-save global identifiers

The initial identifier model focuses on deterministic stability within a single save file.

## 3.13 Localization and Identity Invariants

Localization must never influence deterministic identity behavior.

Hard invariants:

- Localized strings MUST NOT participate in `TaskId`, `RuleId`, `DayKey`,
or `SubjectId` generation.
- Localized strings MUST NOT participate in identifier equality,
reconciliation, or collision checks.
- Localized strings MUST NOT participate in deterministic ordering or sort
fallback chains.
- `DayKey` remains locale-neutral and uses fixed canonical season tokens.

Display text may vary by locale, but identity semantics remain invariant.

## Implementation Plan Traceability

Primary phase owner(s):

- Phase 2 — Core Domain Model

Also referenced in:

- Phase 5 — Built-in Task Generators
- Phase 6 — Rule Evaluation Engine
- Phase 7 — Persistence System
- Phase 11 — History Browsing UI

Canonical implementation mapping lives in Section 21.
