# Section 13 — Built-in Task Generators #

## 13.1 Purpose ##

Built-in task generators produce tasks derived automatically from the
current game state. Generators allow the system to surface useful tasks
without requiring the player to manually define rules.

Generators must operate deterministically so that task identities remain
stable across evaluations. See Section 3.3 and Section 3.11 for deterministic
identifier rules.

Generators are evaluated by the Task Generation Engine described in
Section 5.1.

## 13.2 Generator responsibilities ##

A task generator is responsible for:

```text
- Inspecting a specific domain of the game state.
- Determining whether tasks should exist for entities in that domain.
- Producing deterministic task definitions for the engine.
```

A generator must not:

```text
- Mutate the State Store directly.
- Dispatch commands to the store.
- Persist any state outside of the engine evaluation cycle.
```

Generators only produce task definitions. The engine reconciles those
definitions against the State Store. See Section 5.10.

## 13.3 Generator domains ##

Each generator operates on a specific domain of the game state.

Examples include:

```text
- Crops
- Animals
- Machines
- Buildings
- Calendar events
- Player progression
- Inventory state
```

A generator must only inspect its own domain. Cross-domain logic belongs
in rule evaluation composition (Section 7.4), not in generator coupling.

## 13.4 Generator identity ##

Each generator must declare a unique `GeneratorID`.

The `GeneratorID` must remain stable across versions to preserve task
identity stability.

Example format:

``` text
GeneratorID = "BuiltIn.Crops.Water"
GeneratorID = "BuiltIn.Animals.Pet"
GeneratorID = "BuiltIn.Machines.Harvest"
```

The `GeneratorID` participates in deterministic `TaskID` generation. See
Section 3.3.

## 13.5 Generator interface ##

Generators must implement a common interface.

Conceptual interface:

``` cs
public interface ITaskGenerator
{
string GeneratorID { get; }

IEnumerable<GeneratedTask> GenerateTasks(
    GenerationContext context
);
}

```

`GenerationContext` contains a read-only snapshot of the current game
state required for evaluation.

Generators must treat the context as immutable.

## 13.6 Generated task structure ##

Generators return `GeneratedTask` structures describing the task that
should exist.

Conceptual structure:

``` cs
public struct GeneratedTask
{
string GeneratorID;
string SubjectID;
TaskType Type;
TaskProgressModel ProgressModel;
TaskMetadata Metadata;
}

```

`GeneratedTask` must contain only identity-affecting fields and metadata
necessary for task creation.

Runtime task state is managed by the State Store. See Section 8.

## 13.7 Subject identifiers ##

`SubjectID` uniquely identifies the in-game entity associated with the
task.

Examples:

```text
- `Crop.Tile(64,15)`
- `Animal.UUID(3fa9...)`
- `Machine.Tile(22,9)`
```

`SubjectID` must remain stable for the lifetime of the entity.

## 13.8 Evaluation triggers ##

Generators are evaluated when the engine determines that their domain
may have changed.

Typical triggers include:

```text
* Crops: day start, crop harvest, watering state change
* Animals: day start, animal interaction
* Machines: machine finished processing
* Calendar: day start
* Player progression: skill level change
```

Trigger routing is handled by the evaluation scheduler described in
Section 7.8 and Section 12.5.

## 13.9 Generator evaluation rules ##

Generators must follow these rules:

1. Evaluation must be deterministic given the same context.
2. Generators must not depend on evaluation order.
3. Generators must not maintain internal mutable state.
4. Generators should avoid expensive scans where possible.

Generators should only examine the subset of state required for their
domain.

## 13.10 Built-in generator set ##

Version 1 includes the following built-in generators. The generators listed
below are the minimum V1 commitment. The set may expand, but none of the listed
generators may be dropped from V1.

| GeneratorID | Domain | Example Tasks | V1 Required |
|---|---|---|---|
| `BuiltIn.Crops.Water` | Crops | Water crops | ✅ |
| `BuiltIn.Crops.Harvest` | Crops | Harvest ready crops | ✅ |
| `BuiltIn.Animals.Pet` | Animals | Pet animals | ✅ |
| `BuiltIn.Animals.Milk` | Animals | Milk cows / goats | ✅ |
| `BuiltIn.Machines.Harvest` | Machines | Collect machine output | ✅ |
| `BuiltIn.Calendar.Festival` | Calendar | Attend festival | ✅ |
| `BuiltIn.Calendar.Birthday` | Calendar | Gift NPC birthday | ✅ |
| `BuiltIn.Quest.Progress` | Quests | Active quest progress | ✅ |

All generators marked V1 Required must ship in Version 1. Additional generators
may be added in later phases or future versions.

## 13.11 Generator configuration ##

Generators may expose configuration flags allowing players to enable or
disable them.

Configuration is managed by the configuration system described in
Section 15.3.

## 13.12 Custom rule interaction ##

Built-in generators operate independently of player-defined rules.

Player-defined rules may reference the same domains but should not
modify the behavior of built-in generators.

This separation ensures predictable behavior and stable task identity.
