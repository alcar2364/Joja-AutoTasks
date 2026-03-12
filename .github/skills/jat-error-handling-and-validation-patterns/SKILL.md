---
name: jat-error-handling-and-validation-patterns
description: "Validation philosophy, null safety, exception types, error messages, safe defaults, identifier validation, collection bounds, error testing, recovery strategies. Use when: implementing input validation, handling errors gracefully, or designing error recovery."
argument-hint: "Describe the validation requirement: what data needs to be validated, what are the error boundaries, and how should failures be communicated?"
---

# Error Handling and Validation Patterns Skill

JAT must validate input data, handle errors gracefully, and recover from corruption without crashing the game. Understanding validation philosophy, null safety patterns, and error recovery strategies ensures robust, player-friendly code.

## 1. Validation Philosophy ##

**Early vs Lazy Validation:**

| Strategy | When | Where | Example |
|----------|------|-------|---------|
| **Early (Preferred)** | Known inputs at creation | Constructor, public method entry | Validate rule before adding |
| **Lazy** | Unknown inputs at creation | When used/accessed | Validate persisted data when loaded |
| **Both** | Mixed | Multiple checks | Create rule (early) + load persisted rule (lazy) |

**Early Validation Pattern:**
```csharp
// ✓ GOOD: Constructor validates immediately
public class Rule
{
    public RuleID Id { get; }
    public string Name { get; }

    public Rule(RuleID id, string name)
    {
        // Validate preconditions
        if (id == null)
            throw new ArgumentNullException(nameof(id));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Rule name required", nameof(name));
        if (name.Length > 200)
            throw new ArgumentException("Rule name exceeds 200 characters", nameof(name));

        Id = id;
        Name = name;
    }
}

// ❌ BAD: No validation; errors appear later when used
public class Rule
{
    public RuleID Id { get; set; }  // No validation
    public string Name { get; set; }  // Could be null or empty
}
```

## 2. Null Safety Patterns ##

**Constructor Protection:**
```csharp
// ✓ GOOD: Guard against null parameters
public class TaskGenerator
{
    private readonly IGameStateProvider _gameState;
    private readonly IModLogger _logger;

    public TaskGenerator(IGameStateProvider gameState, IModLogger logger)
    {
        _gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
        _logger = logger ?? throw new ArgumentException(nameof(logger));
    }
}

// ❌ BAD: Allows null; crashes later with null reference
public class TaskGenerator
{
    public TaskGenerator(IGameStateProvider gameState, IModLogger logger)
    {
        _gameState = gameState;  // Could be null!
        _logger = logger;
    }
}
```

**Property Guard Pattern:**
```csharp
// ✓ GOOD: Null-coalesce with safe default
public string GetTaskTitle(Task task)
{
    return task?.Title ?? "[Untitled Task]";
}

// ✓ GOOD: Explicit null-conditional
public void LogTaskIfValid(Task task)
{
    if (task?.TaskId != null)
    {
        _logger.Log($"Task: {task.TaskId}");
    }
}

// ❌ BAD: Assumes non-null; crashes if null
public void LogTask(Task task)
{
    _logger.Log($"Task: {task.TaskId}");  // NullReferenceException if task is null
}
```

## 3. Exception Types and Usage ##

**Choosing the Right Exception:**

| Exception | When to Use | Example |
|-----------|------------|---------|
| **ArgumentException** | Invalid parameter value (content wrong) | `name.Length > 200` |
| **ArgumentNullException** | Null parameter | `id == null` |
| **ArgumentOutOfRangeException** | Parameter value out of bounds | `day < 1 or > 28` |
| **InvalidOperationException** | Object in wrong state | `Evaluate() called before Initialized` |
| **NotSupportedException** | Operation not valid | Attempting to mutate readonly collection |
| **FormatException** | String parsing failed | `DayKey.Parse("invalid-format")` |

**Exception Usage Examples:**
```csharp
// ArgumentException: content invalid
if (string.IsNullOrWhiteSpace(title))
    throw new ArgumentException("Title cannot be empty", nameof(title));

// ArgumentNullException: null parameter
if (rule == null)
    throw new ArgumentNullException(nameof(rule));

// ArgumentOutOfRangeException: out of bounds
if (day < 1 || day > 28)
    throw new ArgumentOutOfRangeException(nameof(day), day, 
        "Day must be between 1 and 28");

// InvalidOperationException: wrong state
if (_currentPhase != LifecyclePhase.Ready)
    throw new InvalidOperationException(
        $"Cannot evaluate tasks during phase {_currentPhase}");

// FormatException: parsing failed
if (!TryParseTaskId(input, out var id))
    throw new FormatException($"Invalid TaskID format: {input}");
```

## 4. Error Message Clarity ##

**Good Error Messages:**

```csharp
// ✗ Bad: Vague
throw new ArgumentException("Invalid rule");

// ✓ Good: Specific with context
throw new ArgumentException(
    "Rule name cannot exceed 200 characters. Current length: 345", 
    nameof(name));

// ✗ Bad: No recovery guidance
throw new InvalidOperationException("State corrupted");

// ✓ Good: Explains and provides path forward
throw new InvalidOperationException(
    "State reconstruction failed: No rules persisted. " +
    "Create at least one rule before saving. " +
    "See documentation for rule creation guide.");

// ✗ Bad: Technical noise
throw new Exception("NullReferenceException at line 42");

// ✓ Good: User-facing + logged details
throw new InvalidOperationException(
    "Failed to load saved tasks. Using empty task list. " +
    "See mod log for details: SMAPI-latest.txt");
```

## 5. Safe Defaults and Fallbacks ##

**Defensive Reconstruction (Loading Persisted Data):**
```csharp
public async Task<PersistedState> LoadWithDefaults()
{
    try
    {
        var json = await File.ReadAllTextAsync(_savePath);
        var parsed = JsonConvert.DeserializeObject<PersistedState>(json);
        
        // Validate parsed data
        if (parsed == null)
            return CreateEmptyState("Save file contained null object");
        
        return parsed;
    }
    catch (FileNotFoundException)
    {
        _logger.Log("No save file; using empty defaults");
        return CreateEmptyState();
    }
    catch (JsonException ex)
    {
        _logger.LogError($"Corrupted save file: {ex.Message}");
        return CreateEmptyState($"Save file corrupted: {ex.Message}");
    }
}

private PersistedState CreateEmptyState(string reason = null)
{
    if (reason != null)
        _logger.LogWarning($"Using defaults: {reason}");

    return new PersistedState(
        DataVersion: LATEST_VERSION,
        Rules: ImmutableList.Create<Rule>(),
        Completions: ImmutableList.Create<TaskCompletion>()
    );
}
```

## 6. Identifier Validation ##

**SubjectID Validation:**
```csharp
public class SubjectID
{
    public static SubjectID Parse(string value)
    {
        // Check null/empty
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("SubjectID cannot be empty", nameof(value));

        // Check format: "prefix:value"
        var parts = value.Split(':');
        if (parts.Length != 2)
            throw new FormatException($"SubjectID must be 'prefix:value', got '{value}'");

        var (prefix, entity) = (parts[0], parts[1]);

        // Check valid prefix
        if (!IsValidPrefix(prefix))
            throw new FormatException($"Invalid SubjectID prefix: '{prefix}'");

        // Check entity is not empty
        if (string.IsNullOrWhiteSpace(entity))
            throw new FormatException($"SubjectID entity cannot be empty");

        // Check for suspicious characters
        if (entity.Contains("\n") || entity.Contains("\0"))
            throw new FormatException($"SubjectID contains invalid characters");

        return new SubjectID(value);
    }

    private static bool IsValidPrefix(string prefix) =>
        new[] { "npc", "location", "item", "group" }.Contains(prefix);
}
```

**RuleID Validation:**
```csharp
public class RuleID
{
    public static RuleID Create(string value)
    {
        // Check null/empty
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("RuleID cannot be empty", nameof(value));

        // Check length (reasonable limit)
        if (value.Length > 200)
            throw new ArgumentException(
                $"RuleID exceeds 200 characters: length {value.Length}", 
                nameof(value));

        // Warn if unusual format (but don't reject)
        if (!value.StartsWith("rule:"))
            // Could be legacy format; log warning but accept
            _logger.LogWarning($"RuleID '{value}' does not start with 'rule:' prefix");

        return new RuleID(value);
    }
}
```

## 7. Collection Bounds Checking ##

**Safe Iteration and Access:**
```csharp
// ✓ GOOD: Check bounds before accessing
public Task GetTaskByIndex(int index)
{
    if (index < 0 || index >= _tasks.Count)
        throw new ArgumentOutOfRangeException(
            nameof(index), 
            index, 
            $"Index out of range (0–{_tasks.Count - 1})");

    return _tasks[index];
}

// ✓ GOOD: Safe retrieval with TryGet pattern
public bool TryGetTask(TaskID id, out Task task)
{
    task = _tasks.FirstOrDefault(t => t.TaskId == id);
    return task != null;
}

// ❌ BAD: Assumes bounds without checking
public Task GetTaskByIndex(int index)
{
    return _tasks[index];  // IndexOutOfRangeException if index >= Count
}
```

**Collection Mutation Safety:**
```csharp
// ✓ GOOD: Use immutable collections; mutations create new instances
var original = ImmutableList.Create(task1, task2);
var withNewTask = original.Add(task3);  // Returns new list; original unchanged

// ✗ BAD: Mutable list allows accidental corruption
var mutableList = new List<Task> { task1, task2 };
mutableList.Add(task3);  // Mutates in-place; could affect other references
```

## 8. Testing Error Paths ##

**Test: Exception Thrown for Invalid Input**
```csharp
[TestFixture]
public class ValidationTests
{
    [Test]
    public void Rule_NullName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            new Rule(RuleID.Parse("rule:1"), null);
        });
    }

    [Test]
    public void Rule_EmptyName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            new Rule(RuleID.Parse("rule:1"), "   ");
        });
    }

    [Test]
    public void DayKey_InvalidDayNumber_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            new DayKey("Spring", 0);  // Day 0 invalid
        });

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            new DayKey("Spring", 29);  // Day 29 invalid
        });
    }

    [Test]
    public void SubjectID_InvalidFormat_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() =>
        {
            SubjectID.Parse("npc");  // Missing colon and entity
        });

        Assert.Throws<FormatException>(() =>
        {
            SubjectID.Parse("unknown:Abigail");  // Invalid prefix
        });
    }
}
```

**Test: Safe Fallback Behavior**
```csharp
[Test]
public async Task LoadPersistedState_CorruptedFile_ReturnedDefaults()
{
    // Arrange: Write corrupt JSON
    File.WriteAllText(_testPath, "{ invalid json }");

    // Act
    var state = await _persistence.LoadWithDefaults();

    // Assert: Returned defaults, not crashed
    Assert.That(state, Is.Not.Null);
    Assert.That(state.Rules.Count, Is.EqualTo(0));
}

[Test]
public void GetTask_InvalidIndex_ThrowsArgumentOutOfRangeException()
{
    // Arrange
    var tasks = new TaskCollection { task1, task2 };

    // Act & Assert
    Assert.Throws<ArgumentOutOfRangeException>(() => tasks.GetByIndex(999));
}
```

## 9. Recovery Strategies ##

**Corrupted Persistence (Load with Rollback):**
```csharp
public async Task<ApplicationState> LoadWithRecovery()
{
    try
    {
        // Attempt to load
        var persisted = await _persistence.LoadAsync();
        
        // Attempt to reconstruct state
        var reconstructed = await _reconstructor.ReconstructAsync(persisted);
        
        return reconstructed;
    }
    catch (Exception ex)
    {
        _logger.LogError($"State load failed: {ex.Message}");
        
        // Fallback: Create fresh state
        _logger.LogWarning("Rolling back to empty state; user data preserved");
        return CreateEmptyState();
    }
}
```

**Command Validation (Pre-Dispatch):**
```csharp
public void DispatchCommand(ICommand command)
{
    // Validate before applying
    var validation = _validator.Validate(command);
    
    if (!validation.IsValid)
    {
        _logger.LogError($"Invalid command: {validation.Error}");
        
        // Reject; throw or silently ignore based on policy
        throw new InvalidOperationException(
            $"Cannot execute command: {validation.Error}");
    }

    // Command valid; safe to apply
    _stateStore.ApplyCommand(command);
}
```

## Links ##
- [C# Style Contract](../Contracts/CSHARP-STYLE-CONTRACT.instructions.md)
- [Backend Architecture Contract](../Contracts/BACKEND-ARCHITECTURE-CONTRACT.instructions.md)
- [Review and Verification Contract](../Contracts/REVIEW-AND-VERIFICATION-CONTRACT.instructions.md)
