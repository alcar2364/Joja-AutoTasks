---
name: jat-testing-patterns-and-fixtures
description: "Unit test boundaries, fixture patterns, deterministic test requirements, identifier stability testing, task generation testing, State Store testing, persistence testing, edge case discovery. Use when: writing or debugging JAT unit tests, setting up test fixtures, or validating test determinism."
argument-hint: "Describe what you need to test: a specific component (Domain, Events, Infrastructure), test boundaries, or test fixture needs?"
target: vscode
---

# Testing Patterns and Fixtures Skill

JAT's architecture (tasks, rules, State Store, persistence) demands rigorous testing that isolates components, eliminates non-determinism, and validates invariants. Understanding test boundaries, fixture patterns, and JAT-specific assertions helps catch bugs early and maintain deterministic, repeatable test behavior.

## 1. Test Structure in JAT ##

**Unit Test Scope Definition:**

| Layer | What to Test | Test Isolation |
|-------|--------------|-----------------|
| **Domain** (Identifiers, Tasks, Rules) | TaskID formula, SubjectID mapping, identifier stability | Pure functions; no file I/O or game state |
| **Events** (EventDispatcher) | Event channel subscription, dispatch order, listener callbacks | In-memory event bus; no SMAPI hooks |
| **Infrastructure** (Logging, Persistence) | Serialization/deserialization, version migration, safe fallbacks | File I/O in temp directory; cleanup after test |
| **Lifecycle** (LifecycleCoordinator) | Phase sequencing, game event translation, task reconstruction | Mocked SMAPI events; test container setup |
| **Startup** (BootstrapContainer) | Dependency graph wiring, component initialization order | Verify no circular dependencies; mock external dependencies |

**Test Boundary Pattern:**
```csharp
// ❌ BAD: Test crosses multiple layers
[Test]
public void CreateTask_PersistsToFileAndDispatchesEvent()
{
    // This tests Domain + Infrastructure + Events simultaneously
    var rule = new Rule(...);
    var generator = new MyGenerator(rule);
    var task = generator.CreateTask(...);
    
    _persistence.Save(task);  // Infrastructure
    _eventDispatcher.Dispatch(new TaskCreatedEvent(task));  // Events
    
    Assert.That(File.Exists(savePath), Is.True);  // Multiple assertions across layers
}

// ✓ GOOD: Isolated unit test
[Test]
public void TaskID_Create_ProducesDeterministicHash()
{
    // Pure Domain layer test
    var id1 = TaskID.Create("rule:1", new SubjectID("npc:A"), DayKey.Parse("Spring-1"));
    var id2 = TaskID.Create("rule:1", new SubjectID("npc:A"), DayKey.Parse("Spring-1"));
    
    Assert.That(id1, Is.EqualTo(id2));
}

// ✓ GOOD: Infrastructure test (isolated)
[Test]
public async Task PersistenceProvider_SaveAndLoad_PreservesRuleData()
{
    // Arrange
    var rule = new Rule(RuleID.Parse("test-rule"), "Test Rule", ...);
    var tempFile = Path.GetTempFileName();
    
    // Act
    _persistence.Save(rule, tempFile);
    var loaded = await _persistence.LoadAsync(tempFile);
    
    // Assert
    Assert.That(loaded, Is.EqualTo(rule));
    
    // Cleanup
    File.Delete(tempFile);
}
```

## 2. Fixture Patterns by Layer ##

### Domain Fixtures (Identifiers, Values)

```csharp
[TestFixture]
public class DomainFixtures
{
    protected static readonly RuleID TestRuleId = RuleID.Parse("test-rule:1");
    protected static readonly SubjectID TestSubjectNPC = SubjectID.ForNPC("TestNPC");
    protected static readonly DayKey TestDay = DayKey.Parse("Spring-1");
    
    protected static TaskID CreateTestTaskId()
    {
        return TaskID.Create(TestRuleId.ToString(), TestSubjectNPC, TestDay);
    }

    protected static Rule CreateTestRule(string id = null)
    {
        return new Rule(
            RuleID.Parse(id ?? "rule:test"),
            "Test Rule"
        );
    }

    protected static Task CreateTestTask(
        RuleID ruleId = null,
        SubjectID subjectId = null,
        DayKey day = null)
    {
        return new Task(
            TaskID.Create((ruleId ?? TestRuleId).ToString(), 
                         subjectId ?? TestSubjectNPC, 
                         day ?? TestDay),
            (ruleId ?? TestRuleId).ToString(),
            subjectId ?? TestSubjectNPC,
            day ?? TestDay,
            "Test Task"
        );
    }
}
```

### Events Fixture (Mocked Dispatcher)

```csharp
[TestFixture]
public class EventsPatcherFixtures : DomainFixtures
{
    protected MockEventDispatcher _eventDispatcher = null!;
    protected List<DomainEvent> _capturedEvents = null!;

    [SetUp]
    public void SetUpEvents()
    {
        _eventDispatcher = new MockEventDispatcher();
        _capturedEvents = new List<DomainEvent>();
        
        // Capture all events
        _eventDispatcher.Subscribe<DomainEvent>(
            evt => _capturedEvents.Add(evt));
    }

    // Helper: assert event was dispatched
    protected void AssertEventDispatchedOfType<TEvent>(
        Func<TEvent, bool> predicate = null) where TEvent : DomainEvent
    {
        var matchingEvent = _capturedEvents
            .OfType<TEvent>()
            .FirstOrDefault(predicate ?? (_ => true));

        Assert.That(matchingEvent, Is.Not.Null, 
            $"Expected event of type {typeof(TEvent).Name} but none found. " +
            $"Events: {string.Join(", ", _capturedEvents.Select(e => e.GetType().Name))}");
    }
}

// Mock for testing
public sealed class MockEventDispatcher : IEventDispatcher
{
    private readonly Dictionary<Type, List<Delegate>> _subscriptions = new();

    public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : DomainEvent
    {
        var type = typeof(TEvent);
        if (!_subscriptions.ContainsKey(type))
            _subscriptions[type] = new List<Delegate>();
        
        _subscriptions[type].Add(handler);
    }

    public void Dispatch<TEvent>(TEvent evt) where TEvent : DomainEvent
    {
        var type = typeof(TEvent);
        if (_subscriptions.TryGetValue(type, out var handlers))
        {
            foreach (var handler in handlers.Cast<Action<TEvent>>())
            {
                handler(evt);
            }
        }
    }
}
```

### Infrastructure Fixture (Temporary Files)

```csharp
[TestFixture]
public class InfrastructurePatcherFixtures : DomainFixtures
{
    protected string _testDataDirectory = null!;

    [SetUp]
    public void SetUpInfrastructure()
    {
        // Create isolated temp directory for this test
        _testDataDirectory = Path.Combine(
            Path.GetTempPath(), 
            $"jat-test-{Guid.NewGuid():N}");
        
        Directory.CreateDirectory(_testDataDirectory);
    }

    [TearDown]
    public void CleanUpInfrastructure()
    {
        // Clean up test files
        if (Directory.Exists(_testDataDirectory))
            Directory.Delete(_testDataDirectory, recursive: true);
    }

    protected string GetTestFilePath(string filename)
    {
        return Path.Combine(_testDataDirectory, filename);
    }
}
```

### Lifecycle Fixture (Mocked Game State)

```csharp
[TestFixture]
public class LifecycleFixtures : DomainFixtures
{
    protected MockGameStateProvider _gameState = null!;
    protected MockSmapiEvents _smapiEvents = null!;
    protected LifecycleCoordinator _coordinator = null!;

    [SetUp]
    public void SetUpLifecycle()
    {
        _gameState = new MockGameStateProvider
        {
            CurrentDay = TestDay,
            Characters = new[] { "Abigail", "Sebastian" }
        };

        _smapiEvents = new MockSmapiEvents();
        
        _coordinator = new LifecycleCoordinator(
            _gameState,
            _smapiEvents,
            LoggerFactory.Create());
    }

    protected void TriggerGameSaveLoaded()
    {
        _smapiEvents.RaiseSaveLoaded(new SaveLoadedEventArgs());
    }

    protected void TriggerGameDayStarted()
    {
        _gameState.CurrentDay = _gameState.CurrentDay with
        {
            Day = _gameState.CurrentDay.Day + 1
        };
        _smapiEvents.RaiseDayStarted(new DayStartedEventArgs());
    }
}

// Mocks
public sealed class MockGameStateProvider : IGameStateProvider
{
    public DayKey CurrentDay { get; set; }
    public string[] Characters { get; set; } = Array.Empty<string>();
}

public sealed class MockSmapiEvents : ISmapiEventsProxy
{
    public event EventHandler<SaveLoadedEventArgs>? SaveLoaded;
    public event EventHandler<DayStartedEventArgs>? DayStarted;

    public void RaiseSaveLoaded(SaveLoadedEventArgs args) => SaveLoaded?.Invoke(this, args);
    public void RaiseDayStarted(DayStartedEventArgs args) => DayStarted?.Invoke(this, args);
}
```

## 3. Deterministic Test Requirements ##

**Rule 1: No Random Ordering**
```csharp
// ❌ BAD: Test depends on iteration order
[Test]
public void DictContainsExpectedRules()
{
    var ruleDict = new Dictionary<string, Rule>
    {
        { "daily", new Rule(...) },
        { "weekly", new Rule(...) }
    };

    var ruleList = ruleDict.Keys.ToList();  // Order undefined!
    Assert.That(ruleList[0], Is.EqualTo("daily"));  // Might fail randomly
}

// ✓ GOOD: Sort explicitly
[Test]
public void DictContainsExpectedRules()
{
    var ruleDict = new Dictionary<string, Rule>
    {
        { "daily", new Rule(...) },
        { "weekly", new Rule(...) }
    };

    var ruleList = ruleDict.Keys.OrderBy(k => k).ToList();
    Assert.That(ruleList[0], Is.EqualTo("daily"));  // Stable
}
```

**Rule 2: No Time-Dependent Assertions**
```csharp
// ❌ BAD: Test passes at 3:59, fails at 4:01
[Test]
public void Event_CreatedToday()
{
    var evt = new GameEvent();
    Assert.That(evt.CreatedAt.Date, Is.EqualTo(DateTime.Now.Date));
}

// ✓ GOOD: Use fixture day
[Test]
public void Event_CreatedOnCorrectDay()
{
    var evt = new GameEvent(TestDay);
    Assert.That(evt.Day, Is.EqualTo(TestDay));
}
```

**Rule 3: No Flaky Async Without Await**
```csharp
// ❌ BAD: Test might pass before async completes
[Test]
public void LoadData_SavesCorrectly()
{
    var task = _persistence.LoadAsync();  // Fire and forget
    Assert.That(_data, Is.Not.Null);  // Might fail if not loaded yet
}

// ✓ GOOD: Await completion
[Test]
public async Task LoadData_SavesCorrectly()
{
    await _persistence.LoadAsync();  // Wait for completion
    Assert.That(_data, Is.Not.Null);
}
```

**Determinism Checklist for Tests:**
- [ ] No random number generation in test setup
- [ ] No DateTime.Now; use fixture DayKey/dates
- [ ] All collections sorted before iteration
- [ ] Async operations awaited completely
- [ ] Mocked external dependencies return fixed values
- [ ] Test passes 100 times in a row (run locally)

## 4. Testing Identifier Stability ##

**Test Pattern: JSON Round-Trip Simulation**
```csharp
[TestFixture]
public class IdentifierStabilityTests : DomainFixtures
{
    [Test]
    public void TaskID_JsonRoundTrip_RemainsUnchanged()
    {
        // Arrange
        var original = CreateTestTaskId();

        // Act: Serialize to JSON and back
        var json = JsonConvert.SerializeObject(original.Value);
        var deserialized = JsonConvert.DeserializeObject<string>(json);
        var reloaded = TaskID.Parse(deserialized!);

        // Assert
        Assert.That(reloaded, Is.EqualTo(original));
    }

    [Test]
    [Repeat(100)]
    public void TaskID_MultipleCalls_ProduceSameValue()
    {
        // Run 100 times (test decorator handles repetition)
        var id1 = TaskID.Create(TestRuleId.ToString(), TestSubjectNPC, TestDay);
        var id2 = TaskID.Create(TestRuleId.ToString(), TestSubjectNPC, TestDay);

        Assert.That(id1, Is.EqualTo(id2));
    }
}
```

## 5. Testing Task Generation ##

**Test: Determinism Verification**
```csharp
[TestFixture]
public class TaskGenerationDeterminismTests : DomainFixtures
{
    private ITaskGenerator<NPC> _generator = null!;

    [SetUp]
    public void SetUp()
    {
        _generator = new DailyChoresGenerator();
    }

    [Test]
    public void Generator_WithSameContext_ProducesSameTasksInSameOrder()
    {
        // Arrange
        var context = new TaskGenerationContext(
            CurrentDay: TestDay,
            AllPersistedTasks: ImmutableList.Create<Task>(),
            GameState: new MockGameStateProvider(),
            Logger: new MockLogger()
        );
        var subject = TestSubjectNPC;

        // Act: Generate twice
        var run1 = _generator.GenerateCandidateTasks(subject, context);
        var run2 = _generator.GenerateCandidateTasks(subject, context);

        // Assert: Identical order and content
        Assert.That(run1.Count, Is.EqualTo(run2.Count));
        
        var tasks1 = run1.OrderBy(t => t.Title).ToList();
        var tasks2 = run2.OrderBy(t => t.Title).ToList();
        
        for (int i = 0; i < tasks1.Count; i++)
        {
            Assert.That(
                tasks2[i].TaskId, 
                Is.EqualTo(tasks1[i].TaskId),
                $"Task {i} ID mismatch: run1={tasks1[i].TaskId}, run2={tasks2[i].TaskId}");
        }
    }

    [Test]
    public void Generator_WithPersistedTasks_NeverDuplicates()
    {
        // Arrange: Create some persisted tasks
        var persistedTask = CreateTestTask();
        var context = new TaskGenerationContext(
            CurrentDay: TestDay,
            AllPersistedTasks: ImmutableList.Create(persistedTask),
            GameState: new MockGameStateProvider(),
            Logger: new MockLogger()
        );

        // Act: Generate tasks
        var generated = _generator.GenerateCandidateTasks(TestSubjectNPC, context);

        // Assert: Generated tasks don't include persisted ID
        var generatedIds = generated.Select(t => t.TaskId).ToHashSet();
        Assert.That(generatedIds, Does.Not.Contain(persistedTask.TaskId));
    }
}
```

## 6. Testing State Store (Command → Reducer → Snapshot) ##

**Test: Reducer Purity**
```csharp
[TestFixture]
public class StateStoreReducerTests : DomainFixtures
{
    [Test]
    public void Reducer_WithCommand_DoesNotMutateInputState()
    {
        // Arrange
        var initialState = new ApplicationState(
            Tasks: ImmutableList.Create(CreateTestTask()),
            Rules: ImmutableList.Create(CreateTestRule())
        );
        var command = new CreateTaskCommand("new-rule", TestSubjectNPC, TestDay, "New Task");

        // Act
        var newState = MyReducer.Reduce(initialState, command);

        // Assert: Original state unchanged
        Assert.That(initialState.Tasks.Count, Is.EqualTo(1));
        Assert.That(newState.Tasks.Count, Is.EqualTo(2));
        Assert.That(newState, Is.Not.EqualTo(initialState));
    }

    [Test]
    public void Reducer_SnapshotIsImmutable()
    {
        // Arrange
        var state = new ApplicationState(Tasks: ImmutableList.Create(CreateTestTask()));
        var snapshot = SnapshotFactory.CreateSnapshot(state);

        // Act: Attempt mutation (should not compile or throw)
        Assert.Throws<Exception>(() => 
        {
            // snapshot.Tasks.Add(new Task(...));  // Compile error or runtime error
        });

        Assert.That(snapshot.Tasks, Is.ReadOnly);
    }
}
```

## 7. Testing Persistence (Save/Load Round-Trips) ##

**Test: Migration Chain**
```csharp
[TestFixture]
public class PersistenceMigrationTests : InfrastructurePatcherFixtures
{
    [Test]
    public void MigrateV1ToV2_PreservesRuleData()
    {
        // Arrange: Simulate v1 JSON
        var v1Json = JObject.Parse(@"
        {
            ""dataVersion"": 1,
            ""rules"": [{ ""id"": 0, ""name"": ""Daily Tasks"" }]
        }");

        // Act
        var migrated = PersistenceProvider.MigrateToLatest(v1Json);

        // Assert
        Assert.That(migrated.DataVersion, Is.EqualTo(2));
        Assert.That(migrated.Rules[0].Name, Is.EqualTo("Daily Tasks"));
    }

    [Test]
    public async Task SaveAndLoad_JsonRoundTrip_PreservesAllData()
    {
        // Arrange
        var original = new PersistedState(
            DataVersion: 2,
            Rules: ImmutableList.Create(CreateTestRule()),
            Completions: ImmutableList.Create(
                new TaskCompletion(CreateTestTaskId(), TestDay)
            )
        );

        var provider = new PersistenceProvider(GetTestFilePath("save.json"));

        // Act: Save
        await provider.SaveAsync(original);

        // Act: Load
        var loaded = await provider.LoadAsync();

        // Assert
        Assert.That(loaded.Rules.Count, Is.EqualTo(original.Rules.Count));
        Assert.That(loaded.Completions.Count, Is.EqualTo(original.Completions.Count));
    }
}
```

## 8. Architecture Boundary Testing ##

**Verify Component Isolation:**
```csharp
[TestFixture]
public class ArchitectureBoundaryTests
{
    [Test]
    public void Domain_HasNo_InfrastructureDependencies()
    {
        var domainTypes = typeof(TaskID).Assembly.GetTypes();
        var infrastructureNamespace = "JAT.Infrastructure";

        var violations = domainTypes
            .Where(t => t.Namespace?.StartsWith("JAT.Domain") ?? false)
            .SelectMany(t => t.GetReferencedTypes())
            .Where(t => t.Namespace?.StartsWith(infrastructureNamespace) ?? false)
            .ToList();

        Assert.That(violations, Is.Empty, 
            $"Domain layer has {violations.Count} dependencies on Infrastructure layer");
    }
}
```

## 9. Edge Case Discovery Checklist ##

Before shipping a component, verify these edge cases are tested:

- [ ] **Empty collections**: Generator yields 0 tasks; persistence loads empty rules list
- [ ] **Null/missing fields**: Rule description null; optional task metadata missing
- [ ] **Boundary values**: Day 28 (last day), Day 1 (first day); max string length
- [ ] **Duplicate identifiers**: Two rules with same ID (should fail validation)
- [ ] **Invalid transitions**: Save → Load → Save again (idempotent)
- [ ] **Concurrent access**: Multiple threads calling reducer (should be safe if immutable)
- [ ] **Version mismatches**: Load file marked as future version (v99)
- [ ] **Corrupted data**: Malformed JSON, negative day numbers, unknown seasons
- [ ] **Ordering invariants**: Tasks always sorted consistently, rules evaluated deterministically
- [ ] **Round-trip stability**: Save v1 → Load → Migrate → Save v2 → Load (data preserved)

## Links ##
- [Unit Testing Contract](../Contracts/UNIT-TESTING-CONTRACT.instructions.md)
- [Backend Architecture Contract](../Contracts/BACKEND-ARCHITECTURE-CONTRACT.instructions.md)
- [Review and Verification Contract](../Contracts/REVIEW-AND-VERIFICATION-CONTRACT.instructions.md)
