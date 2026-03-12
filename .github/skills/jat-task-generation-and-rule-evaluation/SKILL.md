---
name: jat-task-generation-and-rule-evaluation
description: "Task generation approaches (generator functions vs Task Builder rules), rule model evaluation, candidate task creation, and determinism guardrails. Use when: implementing task generators, defining rule-based task evaluation, or ensuring deterministic task creation workflows."
argument-hint: "Describe the task generation requirement: are you implementing a new generator, defining evaluation rules, or troubleshooting non-deterministic task behavior?"
---

# Task Generation and Rule Evaluation Skill

Tasks in JAT can be created through two primary paths: custom generator functions or declarative Task Builder rules. Understanding when to use each approach, how generators operate, and how rules are evaluated is critical for implementing deterministic task workflows without unexpected duplicates or missing tasks.

## 1. Generator vs Task Builder Rules Decision ##

**When to Implement a Custom Generator:**
- Complex domain logic that maps game entities to task requirements
- Real-time calculations needed during evaluation (not just configuration)
- Subject-specific filtering or conditional task creation
- Performance optimization where rule-based approach would create excessive candidate overhead

**When to Use Task Builder Rules:**
- Configuration-driven task creation with minimal logic divergence
- Reusable patterns across multiple subjects or rule sets
- Easy editing without code recompilation
- Audit trail and versioning requirements

**Decision Checklist:**
- [ ] Does the task creation logic depend on realtime game state? → Generator
- [ ] Is the pattern reusable across multiple rule configurations? → Task Builder
- [ ] Does non-technical configuration need to be editable? → Task Builder
- [ ] Does the logic require algorithmic complexity (sorting, filtering, scoring)? → Generator

## 2. Generator Pattern and Signature ##

**Core Generator Signature:**
```csharp
// Base pattern - implement ITaskGenerator<TSubject>
public interface ITaskGenerator<in TSubject>
{
    IReadOnlyCollection<GeneratedTask> GenerateCandidateTasks(
        TSubject subject,
        TaskGenerationContext context);
}

// GeneratedTask structure
public record GeneratedTask(
    string RuleId,                    // Linking rule that triggered generation
    SubjectID SubjectId,              // Target subject for the task
    DayKey Day,                       // Task day
    string Title,                     // Display name
    string? Description = null,       // Optional details
    int PriorityScore = 0,           // Numeric priority
    IDictionary<string, object>? Metadata = null  // Extra domain data
);
```

**TaskGenerationContext:**
- `CurrentDay`: DayKey for evaluation context
- `AllPersistedTasks`: Previously saved tasks for reuse/filtering
- `GameState`: IGameStateProvider for live game data
- `Logger`: IModLogger for diagnostic output

## 3. Determinism Requirements for Generators ##

**Critical Rule: Generators must be deterministic — identical inputs must always produce identical outputs.**

**Tests for Determinism:**
- Run the generator twice with same inputs → outputs must match exactly (order, content, identifiers)
- Reload from saved state → task identifiers must match previously generated ones
- No dependency on system time, random numbers, or file system state
- No mutation of shared state (allPersistedTasks, game state captured references)

**Common Failure Modes:**
```csharp
// ❌ Bad: Uses DateTime.Now (non-deterministic)
var task = new GeneratedTask(ruleId, subjectId, DayKey.Today, ...);

// ✓ Good: Uses context day parameter
var task = new GeneratedTask(ruleId, subjectId, context.CurrentDay, ...);

// ❌ Bad: Iterates dictionary without ordering (hash order varies)
foreach (var kvp in gameState.CharacterMap) { ... }

// ✓ Good: Results sorted deterministically
foreach (var kvp in gameState.CharacterMap.OrderBy(kvp => kvp.Key)) { ... }

// ❌ Bad: Modifies allPersistedTasks parameter
allPersistedTasks.Add(newTask);

// ✓ Good: Returns transformed collection without mutation
return allPersistedTasks.Union(generatedTasks).OrderBy(...);
```

**Determinism Checklist:**
- [ ] Generator uses only parameters and captured game state (no static state)
- [ ] All collections returned are ordered (sorted keys, indices deterministic)
- [ ] No DateTime.Now, Random, or async operations
- [ ] Task identifiers (particularly TaskID hashes) derive from stable inputs only
- [ ] Unit tests verify identical results across multiple runs

## 4. Subject Resolution and Task Identification ##

**TaskID Generation Formula (Deterministic Hashing):**
```
TaskID = Hash(RuleID + SubjectID + DayKey + OptionalInternalSeed)
```

This means:
- Same rule + same subject + same day → same TaskID (deterministic)
- Generator must ensure SubjectID resolution is stable and consistent
- DayKey must not include timestamps or variable offsets

**Subject Mapping Stability:**
- Map game entities (NPC, Location, Item, etc.) to stable SubjectID values
- SubjectID should include entity name/key, not object references or indices
- For custom subjects, use consistent formatting (e.g., "npc:Abigail", "location:Farm")
- Never use `GetHashCode()` or object equality for subject identity

## 5. Common Generator Types ##

**Daily Generator (Recurring Tasks Per Subject):**
- Generates a new task instance for each day
- Checks if task already persisted (via allPersistedTasks) to avoid duplicates
- Example: "Feed NPC daily" → creates one task per NPC per day

**Conditional Generator (State-Dependent):**
- Evaluates game state to decide if task should be created
- Example: "Complete Spring crops" → only when in Spring season
- Determinism challenge: season transitions must not cause stuttering

**Recurring with Skip/Reset:**
- Generates series of related tasks (multi-day dependencies)
- Example: "Complete 7-day quest" → generates Day 1–7 task sequence
- Must handle reload: if replaying Day 3–7, identify already-persisted Day 1–2

**Scoring/Priority Generator:**
- Ranks multiple candidate tasks by priority metric
- Example: Character affection → higher priority = more valuable task rewards
- Score calculation must be deterministic (no floating point rounding surprises)

## 6. Rule Model Fundamentals ##

**Rule Definition (Declarative Configuration):**
```csharp
public record TaskRule(
    string Id,                       // RuleID
    string Name,
    string SubjectPattern,           // Identifier pattern: "*" or "npc:*" or "npc:Abigail"
    RuleCondition[] Conditions,      // When rule applies
    RuleAction[] Actions,            // What to do (generate task, set properties)
    int EvaluationOrder = 0          // Execution sequence
);

public record RuleCondition(
    string Type,                     // "season", "day-of-week", "item-exists", custom
    Dictionary<string, object> Parameters
);

public record RuleAction(
    string Type,                     // "create-task", "set-priority", "add-metadata"
    Dictionary<string, object> Parameters
);
```

**Evaluation Order (Determinism):**
- Rules evaluated in ascending EvaluationOrder
- Rules with same order: evaluated alphabetically by RuleID
- First-match-wins vs accumulation must be explicit in rule design
- Example: Rule A (order 10) runs before Rule B (order 20)

## 7. Candidate Task Creation Workflow ##

**Step-by-step generator execution:**
1. **Input**: Subject identifier, generation context (day, persistent tasks)
2. **Filtering**: Determine which rules apply to subject (pattern matching)
3. **Condition Evaluation**: Apply rule conditions (season, availability, etc.)
4. **Candidate Generation**: Create candidate GeneratedTask objects
5. **Deduplication**: Check allPersistedTasks to avoid regenerating existing tasks
6. **Output**: Return deduplicated, ordered collection of new candidates

**Deduplication Safety:**
```csharp
// Pattern: Check existing tasks before regenerating
var existingTaskIds = allPersistedTasks
    .Where(t => t.RuleId == rule.Id && t.SubjectId == subjectId)
    .Select(t => t.TaskId)
    .ToHashSet();

var candidates = new List<GeneratedTask>();
foreach (var generatedTask in GenerateTasksForRule(rule, subjectId, context))
{
    // Simulate what TaskID would be
    var hypotheticalId = TaskID.Create(generatedTask.RuleId, 
                                       generatedTask.SubjectId, 
                                       generatedTask.Day);
    
    // Only include if not already persisted
    if (!existingTaskIds.Contains(hypotheticalId))
    {
        candidates.Add(generatedTask);
    }
}
```

## 8. Testing Determinism ##

**Unit Test Pattern:**
```csharp
[Test]
public void GenerateTasks_WithSameInputs_ProducesIdenticalOutput()
{
    // Arrange
    var generator = new MyTaskGenerator();
    var context = CreateTestContext(day: DayKey.Parse("Spring-1"));
    var subject = new SubjectID("npc:Abigail");
    
    // Act
    var run1 = generator.GenerateCandidateTasks(subject, context);
    var run2 = generator.GenerateCandidateTasks(subject, context);
    
    // Assert
    CollectionAssert.AreEqual(
        run1.OrderBy(t => t.RuleId).ThenBy(t => t.Title),
        run2.OrderBy(t => t.RuleId).ThenBy(t => t.Title),
        new GeneratedTaskComparer()
    );
}

[Test]
public void GenerateTasks_AfterReload_ProducesSameTaskIds()
{
    // Simulate: generate → save → reload → regenerate
    var generator = new MyTaskGenerator();
    var day = DayKey.Parse("Summer-15");
    
    // First generation
    var tasks1 = generator.GenerateCandidateTasks(
        new SubjectID("npc:Sebastian"),
        CreateTestContext(day));
    
    // Simulate reload with previously persisted tasks
    var context2 = CreateTestContext(day, allPersistedTasks: tasks1);
    var tasks2 = generator.GenerateCandidateTasks(
        new SubjectID("npc:Sebastian"),
        context2);
    
    // No new tasks should be generated (duplicates filtered)
    Assert.That(tasks2, Has.Count.Zero);
}
```

## 9. Anti-patterns to Avoid ##

| Anti-pattern | Why It's Bad | Fix |
|--------------|-------------|-----|
| Using `Random` in generator | Non-deterministic across reloads | Use seeded or order-based logic |
| Including `DateTime.Now` | Different hour/minute breaks determinism | Use context day only |
| Unordered iteration (`foreach IEnumerable`) | Hash order varies by .NET version | Use `.OrderBy()` explicitly |
| Returning mutable lists | Caller might modify, affecting hash | Return `IReadOnlyCollection<>` |
| Casting game objects as subject key | Object identity not stable | Use `Name` property or ID string |

## Links ##
- [Backend Architecture Contract](../Contracts/BACKEND-ARCHITECTURE-CONTRACT.instructions.md)
- [Unit Testing Contract](../Contracts/UNIT-TESTING-CONTRACT.instructions.md)
- [C# Style Contract](../Contracts/CSHARP-STYLE-CONTRACT.instructions.md)
