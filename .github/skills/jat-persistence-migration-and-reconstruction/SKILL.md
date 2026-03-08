---
name: jat-persistence-migration-and-reconstruction
description: "Minimal persistence principles, versioning requirements, migration patterns, data reconstruction, safe failure modes, round-trip testing, and necessity checklist. Use when: designing what to persist, migrating data formats, saving/loading state, or recovering from corruption."
argument-hint: "Describe what data you need to persist: what state is derived vs. essential, and how does the save format evolve?"
target: vscode
---

# Persistence, Migration, and Reconstruction Skill

JAT minimizes persisted state by reconstructing derived data from essential facts at load time. This reduces complexity, corruption surface area, and inconsistencies. When persistence is necessary, versioning and migration patterns ensure safe evolution. Understanding which data to persist, how to version it, and how to reconstruct safely is critical for save game reliability.

## 1. Minimal Persistence Principle ##

**Core Philosophy:**
Only persist data that **cannot be derived** from other sources. Reconstruct everything else at load time from those essential facts.

**Decision Framework:**

| Data | Persist? | Reason |
|------|----------|--------|
| User-created rules (custom rule definitions) | ✓ Yes | New rules don't exist in base game; essential for user experience |
| Task completion state (did user mark done?) | ✓ Yes | User action data; required to ensure persistence of gameplay decisions |
| Rule completion timestamps | ✓ Yes | Historical record needed for achievement/streak tracking |
| Task ID → NPC mapping (task 1 is for Abigail) | ✗ No | Derived from rule + subject ID; can regenerate deterministically |
| Task priority scores | ✗ No | Derived from rule evaluation each day; recalculate on load |
| NPC name list (who exists in this save) | ✗ No | Derived from game state; use actual game.characters instead |
| Number of tasks generated to date | ✗ No | Derived by rerunning generators; reconstruct from persisted rules |

**Persisted Data Checklist:**
- [ ] Is this data user-created or manually modified? → Persist
- [ ] Will it change in ways mod code can't predict? → Persist
- [ ] Is it purely computed from other persisted data? → Don't persist
- [ ] Is it a property of the current save game (character exists)? → Don't persist (query game state)
- [ ] Could it cause serious bugs if it becomes stale? → Consider reconstruction

## 2. Versioning Requirements ##

**Version Placement (Mandatory):**
```csharp
// Every persisted data structure must include a version
public record PersistedState(
    int DataVersion,              // REQUIRED: tracks format changes
    int SchemaVersion,            // REQUIRED: schema compatibility
    IReadOnlyList<Rule> Rules,
    IReadOnlyList<TaskCompletion> Completions
);

// In JSON manifest or save file:
{
    "dataVersion": 1,
    "schemaVersion": 2,
    "rules": [...],
    "completions": [...]
}
```

**Versioning Strategy:**
- **DataVersion**: Increment on any field addition/removal/reordering/type change
- **SchemaVersion**: Increment on major format evolution (e.g., moving from JSON to binary)
- **Compatibility**: Document which (DataVersion, SchemaVersion) pairs are compatible

**Version Chains (Safe Evolution):**
```csharp
// Pattern: Migrate through version chain
// v1 -> v2 -> v3 (never skip versions)

public static PersistedState MigrateToLatest(dynamic jsonObject)
{
    int version = jsonObject["dataVersion"];

    // Chain migrations
    if (version == 1)
    {
        jsonObject = MigrationV1ToV2(jsonObject);
        version = 2;
    }
    
    if (version == 2)
    {
        jsonObject = MigrationV2ToV3(jsonObject);
        version = 3;
    }

    // version should now equal LATEST_VERSION
    if (version != LATEST_VERSION)
        throw new InvalidOperationException(
            $"Failed to migrate from v{version}. Supported migrations: 1->2->3");

    return DeserializeV3(jsonObject);
}
```

**Avoiding Version Gaps:**
- [ ] Every released version has a migration path to next version
- [ ] No migration jumps (v1 directly to v3); chain through v2
- [ ] Unsupported versions rejected with clear error (not silent corruption)
- [ ] Migration tests verify each step independently

## 3. Migration Patterns ##

**Old-Version Safety (Null-Coalescing Pattern):**
```csharp
// When a field is added in v2, v1 data won't have it
private static PersistedState MigrationV1ToV2(dynamic v1Json)
{
    return new PersistedState(
        DataVersion: 2,
        SchemaVersion: v1Json["schemaVersion"],
        
        Rules: v1Json["rules"] ?? ImmutableList.Create<Rule>(),
        
        Completions: v1Json["completions"] ?? ImmutableList.Create<TaskCompletion>(),
        
        // NEW FIELD in v2: provide sensible default
        LastMigrationDate: DateKey.Today
    );
}
```

**Field Renames (Mapping Pattern):**
```csharp
private static PersistedState MigrationV2ToV3(dynamic v2Json)
{
    // v2 had 'ruleList', v3 renamed to 'rules'
    var ruleList = v2Json["ruleList"] ?? v2Json["rules"] ?? ImmutableList.Create<Rule>();

    // v2 had 'taskStates', v3 renamed to 'completions'
    var completions = v2Json["taskStates"] ?? v2Json["completions"] ?? 
                      ImmutableList.Create<TaskCompletion>();

    return new PersistedState(
        DataVersion: 3,
        SchemaVersion: v2Json["schemaVersion"],
        Rules: ruleList,
        Completions: completions
    );
}
```

**Field Removal (Deprecated Field Ignoring):**
```csharp
// v3 removed 'legacyCompletionFlags' (superseded by 'completions')
private static PersistedState MigrationV2ToV3(dynamic v2Json)
{
    // Simply ignore v2Json["legacyCompletionFlags"]
    // Reconstruct completions from 'completions' field (already in v2)

    return new PersistedState(
        DataVersion: 3,
        Rules: v2Json["rules"],
        Completions: v2Json["completions"]  // legacyCompletionFlags ignored
    );
}
```

**Type Changes (Conversion Pattern):**
```csharp
// v1: ruleId was integer index (0, 1, 2...)
// v3: ruleId is string identifier ("rule:daily-chores", etc.)
private static PersistedState MigrationV1ToV3(dynamic v1Json)
{
    var rules = new List<Rule>();
    var legacyRuleList = v1Json["rules"] as JArray ?? new JArray();

    for (int i = 0; i < legacyRuleList.Count; i++)
    {
        var legacyRule = legacyRuleList[i];
        
        // Convert int index (0) to string identifier ("rule:0")
        var newRuleId = RuleID.Create($"rule:{i}");

        rules.Add(new Rule(
            Id: newRuleId,
            Name: legacyRule["name"],
            // ... other fields
        ));
    }

    return new PersistedState(
        DataVersion: 3,
        Rules: rules.ToImmutableList(),
        Completions: ImmutableList.Create<TaskCompletion>()
    );
}
```

## 4. Data Reconstruction ##

**Reconstruction Pipeline (Load Sequence):**
```csharp
public sealed class StateReconstructor
{
    public static async Task<ApplicationState> ReconstructFromPersistenceAsync(
        IGameStateProvider gameState,
        IPersistenceProvider persistence,
        IModLogger logger)
    {
        // Step 1: Load persisted data
        var savedState = await persistence.LoadAsync();
        logger.Log($"Loaded persisted data: v{savedState.DataVersion}");

        // Step 2: Validate and migrate if needed
        if (savedState.DataVersion != LATEST_VERSION)
        {
            savedState = MigrateToLatest(savedState);
            logger.Log($"Migrated to v{LATEST_VERSION}");
        }

        // Step 3: Reconstruct derived state
        var tasks = await ReconstructTasksAsync(
            savedState.Rules,
            gameState,
            logger);

        var completions = ReconstructCompletionIndexes(
            savedState.Completions,
            tasks);

        // Step 4: Validate consistency
        if (!ValidateReconstructedState(tasks, completions, logger))
        {
            throw new InvalidOperationException(
                "Reconstructed state failed validation checks");
        }

        return new ApplicationState(
            Rules: savedState.Rules,
            GeneratedTasks: tasks,
            CompletionMap: completions
        );
    }

    private static async Task<IReadOnlyCollection<Task>> ReconstructTasksAsync(
        IReadOnlyCollection<Rule> rules,
        IGameStateProvider gameState,
        IModLogger logger)
    {
        var reconstructed = new List<Task>();

        // Regenerate tasks from rules
        foreach (var rule in rules)
        {
            var generator = GetGeneratorForRule(rule);
            var subject = ResolveSubjectFromRule(rule, gameState);
            
            var context = new TaskGenerationContext(
                CurrentDay: gameState.CurrentDay,
                AllPersistedTasks: reconstructed,
                GameState: gameState,
                Logger: logger
            );

            var generatedTasks = generator.GenerateCandidateTasks(subject, context);
            reconstructed.AddRange(generatedTasks);
        }

        return reconstructed.ToImmutableList();
    }

    private static IReadOnlyDictionary<TaskID, Completion> ReconstructCompletionIndexes(
        IReadOnlyCollection<TaskCompletion> persisted,
        IReadOnlyCollection<Task> reconstructedTasks)
    {
        var completionMap = new Dictionary<TaskID, Completion>();

        // Map persisted completions to reconstructed tasks
        foreach (var persistedCompletion in persisted)
        {
            // Find matching task in reconstructed list
            var task = reconstructedTasks.FirstOrDefault(
                t => t.TaskId == persistedCompletion.TaskId);

            if (task != null)
            {
                completionMap[persistedCompletion.TaskId] = 
                    new Completion(persistedCompletion.CompletedDate);
            }
            else
            {
                // Task no longer exists; skip stale completion record
                // (don't corrupt by force-adding invalid reference)
            }
        }

        return completionMap;
    }
}
```

## 5. Safe Failure Modes ##

**Unknown Version (Forward Compatibility):**
```csharp
public static PersistedState LoadWithSafeFallback(dynamic jsonObject)
{
    int version = jsonObject["dataVersion"];

    if (version > LATEST_VERSION)
    {
        // Newer mod version saved this data; we can't migrate
        logger.LogWarning(
            $"Save file created by newer JojaAutoTasks version (v{version}). " +
            $"Ignoring rules and using defaults.");
        
        return new PersistedState(
            DataVersion: LATEST_VERSION,
            Rules: ImmutableList.Create<Rule>(),
            Completions: ImmutableList.Create<TaskCompletion>()
        );
    }

    // Normal migration path
    return MigrateToLatest(jsonObject);
}
```

**Corrupt/Malformed Data:**
```csharp
public static PersistedState LoadWithValidation(string jsonText)
{
    PersistedState result;

    try
    {
        var jsonObject = JObject.Parse(jsonText);
        result = LoadWithSafeFallback(jsonObject);
    }
    catch (JsonException ex)
    {
        logger.LogError($"Corrupted save file: {ex.Message}. Using empty defaults.");
        
        // Return empty state (no rules, no completions)
        return new PersistedState(
            DataVersion: LATEST_VERSION,
            Rules: ImmutableList.Create<Rule>(),
            Completions: ImmutableList.Create<TaskCompletion>()
        );
    }

    // Validate reconstructed data
    if (!ValidatePersistedState(result, logger))
    {
        logger.LogWarning("Reconstructed state failed validation. Using safe defaults.");
        return new PersistedState(
            DataVersion: LATEST_VERSION,
            Rules: result.Rules,  // Keep rules (user data)
            Completions: ImmutableList.Create<TaskCompletion>() // Discard suspicious completions
        );
    }

    return result;
}
```

**Missing File (No Prior Save):**
```csharp
public sealed class PersistenceProvider : IPersistenceProvider
{
    public async Task<PersistedState> LoadAsync()
    {
        string path = GetSaveFilePath();

        if (!File.Exists(path))
        {
            logger.Log("No prior save file found. Using fresh state.");
            
            return new PersistedState(
                DataVersion: LATEST_VERSION,
                Rules: ImmutableList.Create<Rule>(),
                Completions: ImmutableList.Create<TaskCompletion>()
            );
        }

        string jsonText = await File.ReadAllTextAsync(path);
        return LoadWithValidation(jsonText);
    }
}
```

## 6. Round-Trip Testing ##

**Test: Save Then Load (Identity):**
```csharp
[Test]
public async Task SaveAndLoad_WithValidState_ProducesIdenticalState()
{
    // Arrange
    var originalState = new PersistedState(
        DataVersion: LATEST_VERSION,
        Rules: ImmutableList.Create(
            new Rule(RuleID.Parse("rule:1"), "Daily Tasks", ...)
        ),
        Completions: ImmutableList.Create(
            new TaskCompletion(TaskID.Parse("task:1"), DateKey.Parse("Spring-5"))
        )
    );

    // Act: Save
    var persistence = new InMemoryPersistence();
    await persistence.SaveAsync(originalState);

    // Act: Load
    var loadedState = await persistence.LoadAsync();

    // Assert
    Assert.That(loadedState, Is.EqualTo(originalState));
    Assert.That(loadedState.Rules, Is.EqualTo(originalState.Rules));
    Assert.That(loadedState.Completions, Is.EqualTo(originalState.Completions));
}

// Test helper: in-memory storage bypasses JSON serialization bugs
[Test]
public async Task SaveAndLoadViaJson_PreservesAllFields()
{
    // This tests the actual JSON serialization round-trip
    var original = new PersistedState(...);
    
    // Serialize
    var json = JsonConvert.SerializeObject(original);
    
    // Deserialize and migrate
    var jsonObject = JObject.Parse(json);
    var loaded = LoadWithSafeFallback(jsonObject);
    
    // Verify all fields survived
    Assert.That(loaded.DataVersion, Is.EqualTo(original.DataVersion));
    Assert.That(loaded.Rules, Is.EqualTo(original.Rules));
}
```

**Test: Migration Chains:**
```csharp
[Test]
public void MigrationChain_V1ToLatest_PreservesEssentialData()
{
    // Simulate v1 save file
    var v1Json = JObject.Parse(@"
    {
        ""dataVersion"": 1,
        ""rules"": [
            { ""id"": 0, ""name"": ""Daily Chores"" }
        ],
        ""completions"": []
    }");

    // Act
    var migrated = MigrateToLatest(v1Json);

    // Assert
    Assert.That(migrated.DataVersion, Is.EqualTo(LATEST_VERSION));
    Assert.That(migrated.Rules, Has.Count.EqualTo(1));
    Assert.That(migrated.Rules[0].Name, Is.EqualTo("Daily Chores"));
}
```

## 7. Necessity and Scope Checklist ##

Before persisting a piece of data, verify:

- [ ] **Is it user-created?** (User rules → Persist; NPC names → Don't)
- [ ] **Is it derived or computed?** (Task list → Derived; Rule completions → Persist)
- [ ] **Would it cause data loss if not saved?** (User edits → Yes; Task priority → Recalculated)
- [ ] **Does it change unpredictably?** (User actions → Yes; Game state → No)
- [ ] **Can we safely ignore it if corrupt?** (Completions → Maybe; Rules → No)
- [ ] **Will it need migration in the future?** (User rules → Yes, plan versioning)
- [ ] **Is there a size/performance impact?** (Keeping large collections that can be reconstructed)

**Scope Declaration Pattern:**
```csharp
/// <summary>
/// Persisted state contains ONLY:
/// 1. User-defined rules (cannot be derived)
/// 2. Task completion history (user action data)
/// 3. Metadata (schema version, migration markers)
///
/// NOT persisted (reconstructed on load):
/// - Task list (regenerated from rules)
/// - Priority scores (recalculated from rule evaluation)
/// - NPC/location references (derived from game state)
/// </summary>
public record PersistedState(
    int DataVersion,
    IReadOnlyCollection<Rule> Rules,
    IReadOnlyCollection<TaskCompletion> Completions
);
```

## Links ##
- [Backend Architecture Contract](../Contracts/BACKEND-ARCHITECTURE-CONTRACT.instructions.md)
- [Unit Testing Contract](../Contracts/UNIT-TESTING-CONTRACT.instructions.md)
- [Review and Verification Contract](../Contracts/REVIEW-AND-VERIFICATION-CONTRACT.instructions.md)
