# Section 8 — State Store Command Model #

## 8.1 Purpose ##

The State Store is the **authoritative in-memory representation of all active
tasks**.

It is responsible for maintaining the canonical task list used by the HUD,
task menu, and all other UI systems.

The State Store ensures that:

```text
- Task state mutations occur in a controlled and predictable way
- The UI receives stable read-only task snapshots
- User-driven task changes are preserved
- The evaluation engine cannot directly mutate task state
```

All task mutations must occur through **commands processed by the State Store**.

No other system may modify tasks directly.

## 8.2 State Store Responsibilities ##

The State Store performs several critical functions.

1. Maintain canonical task state.

2. Process commands that modify task state.

3. Publish read-only snapshots for the UI.

4. Preserve user-authored task changes.

5. Ensure consistency between evaluation results and UI state.

The State Store **does not perform rule evaluation**.

Rule evaluation is handled exclusively by the **Evaluation Engine**.

## 8.3 State Store Architecture ##

The State Store follows a **Command → Reducer → State** architecture.

Conceptual flow:

Command  
→ Reducer  
→ State Mutation  
→ Snapshot Published

Commands represent **intent**.

Reducers apply **deterministic transformations** to state.

Snapshots expose **read-only task views** to consumers.

This model ensures:

```text
- Predictable state transitions
- Controlled mutation boundaries
- Clear debugging behavior
```

## 8.4 Task State Structure ##

The State Store maintains a dictionary keyed by **Task ID**.

Conceptual structure:

TaskStore  

```text
Dictionary<TaskID, TaskRecord>
```

Each TaskRecord represents the current state of a task.

Conceptual fields:

TaskRecord

```text
- TaskID
- SourceType
- Status
- ProgressCurrent
- ProgressTarget
- Metadata
- DeadlineFields
- UserFlags
```

Field descriptions:

TaskID  
Unique deterministic identifier for the task.

SourceType  
Identifies the task origin.

Possible values:

```text
- Manual
- BuiltIn
- TaskBuilder
```

Status

```text
- Incomplete
- Completed
```

ProgressCurrent  
Current progress value if the task tracks progress.

Used for display, HUD rendering, and rule evaluation support.

ProgressTarget  
Target progress value.

Used for display, HUD rendering, and rule evaluation support.

Note: Progress fields are tracking metrics only. Completion is determined by the
task's completion condition, not solely by progress saturation (see Section 4.4.1).

Metadata  
Title, description, category, icon, and display information.

DeadlineFields  
Derived deadline information such as due date and remaining days.

UserFlags  
User-controlled properties such as pinning.

## 8.5 Immutable Snapshot Model ##

External systems never read the mutable task store directly.

Instead, they receive **snapshots**.

Conceptual structure:

TaskSnapshot

```text
- `IReadOnlyList<TaskView>`
```

Each TaskView represents a read-only projection of task data.

Snapshots ensure:

```text
- UI cannot accidentally mutate tasks
- Rendering uses stable task data during a frame
- Systems cannot interfere with each other during updates
```

Snapshots are regenerated whenever commands modify task state.

## 8.6 Command System ##

Commands represent **requests to change task state**.

Each command describes a single mutation.

Commands are processed sequentially by the State Store.

Example commands include:

```text
- AddOrUpdateTaskCommand
- CompleteTaskCommand
- UncompleteTaskCommand
- RemoveTaskCommand
- PinTaskCommand
- UnpinTaskCommand
```

Commands may originate from:

```text
- Evaluation Engine
- UI interactions
- Persistence rehydration
```

Commands must contain all information required for the reducer to apply
the change.

## 8.7 Reducer Behavior ##

Reducers apply commands to the current state.

Conceptual rule:

NewState = Reduce(CurrentState, Command)

Reducers must be:

```text
- Deterministic
- Side-effect free
- Limited to modifying store state
```

Reducers must not:

```text
- Query game state
- Perform rule evaluation
- Trigger persistence
- Interact with UI
```

Reducers only transform state.

## 8.8 Engine → Store Interaction ##

The Evaluation Engine never modifies tasks directly.

Instead, it emits **AddOrUpdateTaskCommand** objects.

Example command fields:

AddOrUpdateTaskCommand

```text
- TaskID
- Title
- Description
- Category
- ProgressCurrent
- ProgressTarget
- Status
- DeadlineFields
```

Reducer behavior:

If the task does not exist  
→ Create new task record.

If the task exists  
→ Update engine-controlled fields only.

User-controlled fields must never be overwritten.

## 8.9 Preserving User-Controlled State ##

Certain fields belong exclusively to the player.

Examples include:

```text
- Manual completion toggles
- Pinned tasks
- Custom ordering (future feature)
```

Reducers must enforce separation between:

Engine-controlled fields  
User-controlled fields

Conceptual rule:

Engine fields overwrite engine fields.  
User fields overwrite user fields.  

Neither system may overwrite the other.

Example:

An engine update must **not remove a pin set by the player**.

## 8.10 Task Removal Rules ##

Tasks may be removed for several reasons.

Rule no longer produces the task  
→ Remove task.

Built-in generator disabled  
→ Remove task.

Manual deletion by the player  
→ Remove task.

Daily task expiration  
→ Remove task.

Task removal occurs through:

RemoveTaskCommand

Reducers delete the record from the store.

## 8.11 Day Boundary Behavior ##

The start of a new in-game day triggers several actions.

1. Expired daily tasks are removed.

2. Daily rules generate new task instances.

3. The task snapshot is rebuilt.

4. Daily task history may be persisted.

Daily tasks are identified by **day-keyed Task IDs**.

Persistent tasks remain unchanged.

## 8.12 Snapshot Publishing ##

Whenever commands modify the store:

1. Reducers apply the changes.

2. Store version increments.

3. A new snapshot is generated.

4. Subscribers are notified via a C# event.

The State Store exposes a snapshot-changed event:

``` cs
public event Action<TaskSnapshot>? SnapshotChanged;
```

Subscribers include:

```text
- View Models (Section 10A) — the primary consumers
- Debug tools (when enabled)
```

View Models subscribe to this event and update their INPC properties
when a new snapshot arrives, causing StardewUI to re-render affected
elements. See Section 10A.6 for the full subscription model.

UI rendering should always use the latest snapshot.

## 8.13 Debug and Development Benefits ##

The command model enables powerful debugging.

Command streams can be logged.

Example:

[Engine] AddOrUpdateTask TaskBuilder_17  
[UI] CompleteTask TaskBuilder_17  
[Engine] UpdateProgress TaskBuilder_17

This allows developers to observe task lifecycle events and easily
trace state transitions.

The model also allows potential future support for **time-travel debugging**.

## 8.14 Version 1 Constraints ##

The Version 1 State Store intentionally remains minimal.

Limitations include:

```text
- No batch command transactions
- No undo history
- No dismissed task tracking
- No multiplayer synchronization
```

The command architecture is designed to remain stable while allowing
future features to expand on the system safely.
