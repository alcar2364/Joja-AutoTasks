# Section 6 — Task Builder Rule Serialization #

## 6.1 Purpose ##

Task Builder rules must be stored in a stable serialized format so that:

```text
- Player-authored rules persist across game sessions
- Rules remain compatible across mod updates
- Rules can be evaluated deterministically by the engine
- Future rule system expansions can be introduced without breaking saves
```

The serialization model represents rules as structured data, not executable
logic.

This ensures:

```text
- Stability
- Forward compatibility
- Safe validation before evaluation
```

Rules are therefore data-driven definitions interpreted by the rule engine.

## 6.2 Rule Object Structure ##

A Task Builder rule is stored as a serialized Rule Definition.

Conceptual structure:

```text
RuleDefinition
```

RuleID
Metadata
Trigger
ConditionTree
ProgressModel
OutputModel

```text

```

Each component has a dedicated purpose.

|   Component   |                          Purpose                         |
| ------------- | -------------------------------------------------------- |
| RuleID        | Stable identifier used for deterministic task generation |
| Metadata      | UI presentation information                              |
| Trigger       | Defines when rule evaluation occurs                      |
| ConditionTree | Logical evaluation structure                             |
| ProgressModel | Defines how progress is measured                         |
| OutputModel   | Defines the task produced                                |

## 6.3 Rule Identity ##

Each rule is assigned a stable integer Rule ID at creation time.

Example:

```text
RuleID = 17
```

Rule IDs must never change after creation.

Rule IDs are auto-incrementing integers issued by the `RuleIdCounter`,
parallel to `ManualTaskCounter`. See §3.7.

Rule IDs are used when generating deterministic task identifiers. Canonical
TaskID structure is defined in Section 3.3, and Task Builder-specific TaskID
composition is defined in Section 3.4.

This ensures that the same rule always generates the same task identity.

## 6.4 Rule Metadata ##

Metadata contains presentation information used by the UI.

Fields include:

```text
Metadata
```

Title
Description
Category
Icon

```text

```

Example:

```text
Title: Collect 300 Wood
Description: Gather enough wood to build a coop
Category: Resources
Icon: Wood
```

Metadata does not affect rule logic.

## 6.5 Trigger Model ##

Triggers determine when rule evaluation should occur.

A rule may define one or more triggers.

Example structure:

```text
Trigger
```

Type
Parameters

```text

```

Trigger types supported in Version 1:

|   Trigger Type  |              Description             |
| --------------- | ------------------------------------ |
| DayStart        | Evaluate rule when a new day begins  |
| InventoryChange | Evaluate when inventory changes      |
| TimeChange      | Evaluate when in-game time advances  |
| LocationChange  | Evaluate when player moves locations |
| Periodic        | Evaluate on throttled tick           |

Example:

```text
Trigger:
```

Type: InventoryChange

```text

```

Rules can define multiple triggers if required.

Triggers are used to reduce unnecessary evaluation work.

## 6.6 Condition Tree ##

The condition tree defines logical evaluation rules.

Conditions form a composable boolean structure.

Supported operators:

```text
- AND
- OR
- NOT
```

Example:

```text
AND
```

Condition: ItemCount(Wood >= 300)
Condition: DayOfSeason <= 28

```text

```

Leaf condition types supported in Version 1:

|   Condition  |            Description           |
| ------------ | -------------------------------- |
| ItemCount    | Inventory contains item >= value |
| SkillLevel   | Skill level >= value             |
| QuestState   | Quest active or completed        |
| Location     | Player location                  |
| DayOfMonth   | Specific day                     |
| TimeWindow   | Time of day range                |
| MachineState | Machine ready                    |

The condition tree determines whether a task exists and contributes to
completion evaluation.

## 6.7 Progress Model ##

Rules may optionally define progress tracking.

Structure:

```text
ProgressModel
```

Type
BaselineMode
TargetValue

```text

```

Supported types:

| Progress Type | Description |
| ------------------ | ----------------- |
| None | Binary condition |
| Counter | Count of actions |
| ResourceCollection | Resource progress |
| SkillProgress | Skill progress |

Baseline modes (determine whether progress is measured from a baseline value or
as an absolute value):

| Baseline Mode | Description |
| ----------------- | ----------------------------------- |
| None | No baseline needed |
| CaptureAtCreation | Baseline captured when task created |
| CaptureDaily | Baseline captured at day start |

Runtime baseline behavior for these modes is defined in Section 7.6.

Example:

```text
ProgressModel
```

Type: ResourceCollection
BaselineMode: CaptureAtCreation
TargetValue: 300

```text

```

## 6.8 Output Model ##

The Output Model defines what task is produced.

Structure:

```text
OutputModel
```

TaskType
Persistence
Deadline

```text

```

Fields:

| Field | Description |
| ----------- | -------------------- |
| TaskType | Progress or Reminder |
| Persistence | Persistent or Daily |
| Deadline | Optional due date |

Example:

```text
OutputModel
```

TaskType: Progress
Persistence: Persistent

```text

```

Example with deadline:

```text
OutputModel
```

TaskType: Progress
Persistence: Persistent
Deadline: Fall 28

```text

```

## 6.9 Example Serialized Rule ##

Example rule: Collect 300 wood.

```text
RuleID: 17

Metadata
```

Title: Collect 300 Wood
Category: Resources

```text

Trigger

```

Type: InventoryChange

```text

ConditionTree

```

ItemCount(Wood)

```text

ProgressModel

```

Type: ResourceCollection
BaselineMode: CaptureAtCreation
TargetValue: 300

```text

OutputModel

```

TaskType: Progress
Persistence: Persistent

```text

```

## 6.10 Serialization Format ##

Rules are stored in the mod save data as structured JSON.

Example conceptual JSON:

```json
{
  "ruleId": 17,
  "metadata": {
```

"title": "Collect 300 Wood",
"category": "Resources"

```text

  },
  "trigger": {

```

"type": "InventoryChange"

```text

  },
  "progress": {

```

"type": "ResourceCollection",
"baselineMode": "CaptureAtCreation",
"target": 300

```text

  }
}

```

JSON allows:

```text
- Version migration
- Debug inspection
- Easy extension
```

## 6.11 Versioning Strategy ##

Rules must include a schema version.

Example:

```text
RuleSchemaVersion = 1
```

If rule formats change in future versions:

```text
- Migration functions transform old rules into the new format
- Saves remain compatible
```

This prevents rule corruption when expanding the system.
