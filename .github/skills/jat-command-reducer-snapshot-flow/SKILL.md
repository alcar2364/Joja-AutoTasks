---
name: jat-command-reducer-snapshot-flow
description: Command object patterns, reducer pure functions, snapshot creation/publishing, command dispatch, multi-slice reducers, and mutation boundary auditing. Use when: implementing state mutations, designing command flows, testing reducer chains, or ensuring State Store immutability contracts.
argument-hint: "Describe the state change: what command are you implementing, which state slices does it affect, and how should the UI be notified?"
target: vscode
---

# Command-Reducer-Snapshot Flow Skill

JAT's State Store manages all mutable application state through a strict command → reducer → snapshot flow. Commands express intent to mutate state, reducers apply those mutations purely and deterministically, and snapshots provide immutable views for UI binding. Understanding this architecture prevents state inconsistencies, ensures UI synchronization, and maintains testability.

## 1. Command Object Structure ##

**Command Pattern (Intent Expression):**
```csharp
// Base interface
public interface ICommand
{
    string CommandType { get; }
    // Commands should be data-only, no methods
}

// Concrete example
public record CreateTaskCommand(
    string RuleId,
    SubjectID SubjectId,
    DayKey Day,
    string Title,
    string? Description = null
) : ICommand
{
    public string CommandType => nameof(CreateTaskCommand);
}
```

**Command Design Principles:**
- **Pure Data:** Commands carry only the data needed for the state change (no logic/methods)
- **Immutable**: Use `record` types for value semantics and immutability
- **Deterministic**: Identical commands must always produce identical state changes
- **Domain-Meaningful**: Command name should express user/system intent, not low-level operations
- **Serializable**: Commands may be logged/persisted, so avoid non-serializable types

**Command Naming Convention:**
- Use verb-noun style: `CreateTaskCommand`, `UpdateRuleCommand`, `DeletePersistenceCommand`
- Avoid generic names like `UpdateCommand` or `ModifyCommand`
- Reflect domain terminology: "Task" not "Item", "Rule" not "Config"

## 2. Reducer Function Patterns ##

**Core Reducer Signature:**
```csharp
// Pure function: (State, Command) -> NewState
// Reducers must be synchronous, pure, and deterministic

public delegate State Reducer<State>(State previousState, ICommand command);

// Concrete example
public sealed class TaskStateReducer
{
    public static TaskState Reduce(TaskState state, ICommand command)
    {
        return command switch
        {
            CreateTaskCommand cmd => ReduceCreateTask(state, cmd),
            UpdateTaskCommand cmd => ReduceUpdateTask(state, cmd),
            DeleteTaskCommand cmd => ReduceDeleteTask(state, cmd),
            _ => state
        };
    }

    private static TaskState ReduceCreateTask(TaskState state, CreateTaskCommand cmd)
    {
        // Create new immutable state, never mutate input
        var newTask = new Task(
            TaskID.Create(cmd.RuleId, cmd.SubjectId, cmd.Day),
            cmd.RuleId,
            cmd.SubjectId,
            cmd.Day,
            cmd.Title,
            cmd.Description
        );

        // Return new state with appended task
        var updatedTasks = state.Tasks.Add(newTask);
        return state with { Tasks = updatedTasks };
    }
}
```

**Pure Function Requirements:**
- **No Side Effects**: Reducers must not log, access external services, or mutate globals
- **Immutable Operations**: Use `with` expressions, `ImmutableList.Add()`, not list mutation
- **Deterministic Output**: Same (state, command) → always same result
- **No Time Dependencies**: Don't call DateTime.Now or check elapsed time
- **Complete**: Handle all known command types explicitly; use default for unknown types

**Reducer Chain Pattern (Multi-Slice Dependencies):**
```csharp
// When a command affects multiple state slices
public sealed class MultiSliceReducer
{
    public static ApplicationState Reduce(ApplicationState state, ICommand command)
    {
        // Step 1: Reduce task slice
        var newTaskState = TaskStateReducer.Reduce(state.Tasks, command);
        
        // Step 2: Reduce rule slice (may depend on new task state)
        var newRuleState = RuleStateReducer.Reduce(state.Rules, command, newTaskState);
        
        // Step 3: Return entirely new application state
        return state with
        {
            Tasks = newTaskState,
            Rules = newRuleState
        };
    }
}

// Anti-pattern: passing mutable state between slices
// ❌ var newRuleState = RuleStateReducer.Reduce(state.Rules, command, newTaskState);
// ✓  Create new state, only read from previous slices
```

## 3. Validation and Immutability Enforcement ##

**Pre-Reducer Validation:**
```csharp
// Validate commands BEFORE they reach the reducer
public sealed class CommandValidator
{
    public static ValidationResult Validate(ICommand command)
    {
        return command switch
        {
            CreateTaskCommand cmd => ValidateCreateTask(cmd),
            UpdateTaskCommand cmd => ValidateUpdateTask(cmd),
            _ => ValidationResult.Valid()
        };
    }

    private static ValidationResult ValidateCreateTask(CreateTaskCommand cmd)
    {
        // Check preconditions
        if (string.IsNullOrWhiteSpace(cmd.Title))
            return ValidationResult.Error("Task title required");
        if (cmd.Title.Length > 200)
            return ValidationResult.Error("Task title exceeds 200 characters");
        
        return ValidationResult.Valid();
    }
}

// Usage in dispatch
public void DispatchCommand(ICommand command)
{
    var validation = CommandValidator.Validate(command);
    if (!validation.IsValid)
        throw new InvalidOperationException($"Invalid command: {validation.Error}");
    
    var newState = _reducer.Reduce(_currentState, command);
    PublishSnapshot(newState);
}
```

**Immutability Audit Checklist:**
- [ ] All reducer parameters are used read-only (no `.Clear()`, `.Add()`, mutations)
- [ ] Return type is new state object, never the input state
- [ ] All collections use immutable types (`ImmutableList`, `ImmutableDictionary`)
- [ ] Unit tests prove original state unchanged after reducer call
- [ ] No `ref` or `out` parameters in reducer signature

## 4. Snapshot Creation and Publishing ##

**Snapshot Generation (Read-Only View):**
```csharp
// Snapshots are immutable projections of the current state
public record TaskSnapshot(
    IReadOnlyCollection<Task> Tasks,
    IReadOnlyCollection<Rule> Rules,
    Metadata Metadata // e.g., LastUpdated, Version
)
{
    // Prevent mutation via sealed type
    // No setters, no mutable properties
}

public sealed class SnapshotFactory
{
    public static TaskSnapshot CreateSnapshot(ApplicationState state)
    {
        return new TaskSnapshot(
            Tasks: state.Tasks.ToReadOnlyCollection(),
            Rules: state.Rules.ToReadOnlyCollection(),
            Metadata: new Metadata(
                LastUpdated: state.LastUpdated,
                Version: state.Version
            )
        );
    }
}
```

**Snapshot Publishing Lifecycle:**
```csharp
public sealed class StateStore
{
    private ApplicationState _currentState;
    private TaskSnapshot _currentSnapshot;
    
    // Event: UI subscribes to snapshot changes
    public event Action<TaskSnapshot>? SnapshotChanged;

    public void DispatchCommand(ICommand command)
    {
        // 1. Validate
        var validation = CommandValidator.Validate(command);
        if (!validation.IsValid)
            throw new InvalidOperationException(validation.Error);

        // 2. Reduce
        var previousState = _currentState;
        _currentState = Reducer.Reduce(_currentState, command);

        // 3. Create snapshot
        _currentSnapshot = SnapshotFactory.CreateSnapshot(_currentState);

        // 4. Publish (notify UI)
        SnapshotChanged?.Invoke(_currentSnapshot);

        // 5. Persist (if needed)
        _persistence.SaveSnapshot(_currentSnapshot);
    }

    public TaskSnapshot GetCurrentSnapshot() => _currentSnapshot;
}
```

**Change Notification Pattern:**
- Snapshot published **only after** new state fully constructed
- UI receives notification asynchronously (deferred via event loop if needed)
- No snapshot changes mid-way through state construction
- Snapshot immutability prevents accidental UI-side mutations

## 5. Command Dispatch Entry Points ##

**Typical Dispatch Flow:**
```csharp
// Entry 1: User action (UI button click)
public void OnSaveTaskButtonClicked(string title, string rule)
{
    var command = new CreateTaskCommand(
        RuleId: rule,
        SubjectId: _currentSubjectId,
        Day: _gameState.CurrentDay,
        Title: title
    );
    _stateStore.DispatchCommand(command);
}

// Entry 2: Game event (SMAPI hook)
public void OnDayStarted(object? sender, DayStartedEventArgs e)
{
    var command = new DailyResetCommand(
        Day: DayKey.FromSmapiDate(e.DaysElapsed)
    );
    _stateStore.DispatchCommand(command);
}

// Entry 3: Persistence/Reconstruction
public void LoadPersistedSnapshot(TaskSnapshot snapshot)
{
    var command = new LoadSnapshotCommand(snapshot);
    _stateStore.DispatchCommand(command);
}
```

**Dispatch Safety Guardrails:**
- [ ] Dispatch called on main thread only (no race conditions)
- [ ] Commands queued if dispatch called during snapshot processing
- [ ] Invalid commands rejected with clear error messages
- [ ] Command history logged for replay/debugging

## 6. Testing Reducer Chains ##

**Unit Test: Single Reducer with Command Flow:**
```csharp
[TestFixture]
public class TaskStateReducerTests
{
    [Test]
    public void Reduce_CreateTaskCommand_AppendsNewTask()
    {
        // Arrange
        var initialState = new TaskState(
            Tasks: ImmutableList.Create(
                new Task(TaskID.Parse("task-1"), "rule-1", new SubjectID("npc:Abigail"), ...)
            )
        );

        var command = new CreateTaskCommand(
            RuleId: "rule-2",
            SubjectId: new SubjectID("npc:Sebastian"),
            Day: DayKey.Parse("Spring-5"),
            Title: "New Task"
        );

        // Act
        var newState = TaskStateReducer.Reduce(initialState, command);

        // Assert
        Assert.That(newState.Tasks, Has.Count.EqualTo(2));
        Assert.That(initialState.Tasks, Has.Count.EqualTo(1)); // Original unchanged
    }

    [Test]
    public void Reduce_WithInvalidCommand_ReturnsUnchangedState()
    {
        // Arrange
        var state = new TaskState(Tasks: ImmutableList.Create<Task>());
        var unknownCommand = new SomeUnknownCommand();

        // Act
        var newState = TaskStateReducer.Reduce(state, unknownCommand);

        // Assert
        Assert.That(newState, Is.EqualTo(state)); // No change
    }
}
```

**Integration Test: Multi-Slice Reducer:**
```csharp
[Test]
public void Reduce_CreateTaskCommand_UpdatesRuleAndTaskSlices()
{
    // Arrange
    var initialState = new ApplicationState(
        Tasks: ImmutableList.Create<Task>(),
        Rules: ImmutableList.Create(
            new Rule(RuleID.Parse("rule-1"), ...)
        )
    );

    var command = new CreateTaskCommand(...);

    // Act
    var newState = MultiSliceReducer.Reduce(initialState, command);

    // Assert
    Assert.That(newState.Tasks, Has.Count.EqualTo(1)); // Task added
    Assert.That(newState.Rules[0].TaskCount, Is.EqualTo(1)); // Rule bookkeeping updated
}
```

## 7. Anti-patterns and Common Mistakes ##

| Mistake | Why It's Bad | Fix |
|---------|-------------|-----|
| Mutating input state directly | Breaks immutability contract; causes stale snapshots | Use `with` or immutable collections |
| Calling external APIs in reducer | Non-deterministic; breaks replay/testing | Dispatch async command after reducer completes |
| Reducer with side effects (logging, saving) | State and snapshots become unsynchronized | Pure reducer; side effects in dispatch flow |
| Unordered multi-slice updates | Dependent slices see inconsistent state | Update slices in dependency order |
| Snapshots containing mutable objects | UI accidental mutations affect next state | Use sealed records, read-only collections |
| Command validation inside reducer | Validation failures not atomic; unclear intent | Validate before dispatch |

## Links ##
- [Backend Architecture Contract](../Contracts/BACKEND-ARCHITECTURE-CONTRACT.instructions.md)
- [Unit Testing Contract](../Contracts/UNIT-TESTING-CONTRACT.instructions.md)
- [Review and Verification Contract](../Contracts/REVIEW-AND-VERIFICATION-CONTRACT.instructions.md)
- [C# Style Contract](../Contracts/CSHARP-STYLE-CONTRACT.instructions.md)
