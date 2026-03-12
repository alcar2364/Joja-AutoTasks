---
name: jat-event-lifecycle-and-game-coupling
description: "SMAPI event lifecycle, JAT lifecycle phases, event dispatch translation, task evaluation triggers, reconciliation after reload, update tick frequency, new feature integration patterns. Use when: hooking into game events, implementing lifecycle phases, debugging lifecycle coordination, or understanding game-JAT coupling points."
argument-hint: "Describe the game event requirement: which SMAPI event should trigger what JAT behavior, and how should it integrate with the existing lifecycle?"
target: vscode
---

# Event Lifecycle and Game Coupling Skill

JAT's operation is coordinated by SMAPI game events (SaveLoaded, DayStarted, UpdateTicked) that trigger state mutations, task generation, and persistence. Understanding the event lifecycle, JAT's internal phases, and reconciliation patterns ensures features integrate correctly without duplicating work or missing updates.

## 1. SMAPI Event Lifecycle Overview ##

**Primary SMAPI Events JAT Depends On:**

| Event | When Fired | JAT Usage | Frequency |
|-------|-----------|-----------|-----------|
| **SaveLoaded** | After save file opened; game ready | Reconstruct persisted state, initialize components | Once per game load |
| **DayStarted** | New in-game day begins (past 6 AM) | Reset daily state, regenerate tasks for new day | Once per day (in-game) |
| **UpdateTicked** | Every game frame | Evaluate tasks, check completion, process recurring work | 60+ times per second |
| **SaveCompleted** | Save file written to disk | Persist current state (optional; can also save on-demand) | Once per game save |

**Event Order (Typical Game Session):**
```
[Game Launch] 
  → SaveLoaded (state restored) 
  → DayStarted (6 AM, new day)
  → UpdateTicked (repeating, every frame while game running)
  → [Player saves]
  → SaveCompleted
  → [Next in-game day]
  → DayStarted
  → UpdateTicked (repeating)
```

## 2. JAT Lifecycle Phases ##

**5 Main Phases (Coordinated by LifecycleCoordinator):**

```csharp
public enum LifecyclePhase
{
    Uninitialized,      // Before SaveLoaded
    Initializing,       // During SaveLoaded; restoring persisted state
    Ready,              // Initialization complete; normal operation
    Evaluating,         // DayStarted or per-frame task evaluation
    Persisting          // Saving state to disk
}
```

**Phase Progression:**
```
Uninitialized 
  → [SaveLoaded event]
  → Initializing (load persisted data, reconstruct tasks)
  → Ready (initialization complete)
  
  → [DayStarted event]
  → Evaluating (regenerate tasks for new day)
  → Ready (tasks ready for display)
  
  → [UpdateTicked event]
  → Evaluating (check task completions, evaluate recurring rules)
  → Ready
  
  → [Manual save command or SaveCompleted event]
  → Persisting (write state to disk)
  → Ready
```

**Guarding Phase Transitions:**
```csharp
public sealed class LifecycleCoordinator : ILifecycleCoordinator
{
    private LifecyclePhase _currentPhase = LifecyclePhase.Uninitialized;

    public async Task InitializeAsync()
    {
        if (_currentPhase != LifecyclePhase.Uninitialized)
            throw new InvalidOperationException(
                $"Cannot initialize from phase {_currentPhase}");

        _currentPhase = LifecyclePhase.Initializing;

        try
        {
            // Load persisted state
            var persistedState = await _persistence.LoadAsync();
            
            // Reconstruct tasks
            var reconstructed = await _reconstructor.ReconstructAsync(persistedState);
            
            _stateStore.LoadSnapshot(reconstructed);
            
            _currentPhase = LifecyclePhase.Ready;
        }
        catch (Exception ex)
        {
            _currentPhase = LifecyclePhase.Uninitialized;  // Revert on failure
            throw;
        }
    }

    public void OnDayStarted()
    {
        if (_currentPhase != LifecyclePhase.Ready)
        {
            logger.LogWarning(
                $"DayStarted received during phase {_currentPhase}; ignoring");
            return;
        }

        _currentPhase = LifecyclePhase.Evaluating;

        try
        {
            // Regenerate tasks for new day
            _taskGenerator.RegenerateDailyTasks(_stateStore.GetCurrentSnapshot().Day);
            
            _currentPhase = LifecyclePhase.Ready;
        }
        catch
        {
            _logger.LogError("Error during day transition");
            _currentPhase = LifecyclePhase.Ready;  // Recover to allow gameplay
        }
    }
}
```

## 3. Event Dispatch Pattern (SMAPI → JAT Domain) ##

**Translation from SMAPI Events to JAT Domain Events:**

```csharp
public sealed class SmapiEventBridge
{
    private readonly IEventDispatcher _jatDispatcher;
    private readonly ISmapiEvents _smapiEvents;
    private readonly IModLogger _logger;

    public void HookIntoSmapiEvents()
    {
        // SMAPI event → JAT domain event translation
        _smapiEvents.SaveLoaded += (sender, args) =>
        {
            _logger.Log("SaveLoaded event received");
            _jatDispatcher.Dispatch(new GameSaveLoadedEvent(
                SavePath: args.SaveFolderPath
            ));
        };

        _smapiEvents.DayStarted += (sender, args) =>
        {
            _logger.Log($"DayStarted event received (day {args.DaysElapsed})");
            _jatDispatcher.Dispatch(new GameDayStartedEvent(
                Day: DayKey.FromSmapiDaysElapsed(args.DaysElapsed)
            ));
        };

        _smapiEvents.UpdateTicked += (sender, args) =>
        {
            // Only dispatch every N frames to avoid too-frequent polling
            if (args.TicksElapsed % 10 == 0)  // Every ~6 frames at 60fps
            {
                _jatDispatcher.Dispatch(new GameUpdateTickEvent(
                    TicksSinceLastUpdate: args.TicksElapsed
                ));
            }
        };

        _smapiEvents.SaveCompleted += (sender, args) =>
        {
            _logger.Log("SaveCompleted event received");
            _jatDispatcher.Dispatch(new GameSaveCompletedEvent());
        };
    }
}

// Domain events (decoupled from SMAPI)
public record GameSaveLoadedEvent(string SavePath) : IDomainEvent;
public record GameDayStartedEvent(DayKey Day) : IDomainEvent;
public record GameUpdateTickEvent(int TicksSinceLastUpdate) : IDomainEvent;
public record GameSaveCompletedEvent : IDomainEvent;
```

**Listener Pattern (Reacting to Domain Events):**
```csharp
public sealed class TaskEvaluationListener
{
    private readonly IEventDispatcher _dispatcher;
    private readonly ITaskGenerator _generator;
    private readonly IStateStore _stateStore;

    public void Subscribe()
    {
        _dispatcher.Subscribe<GameDayStartedEvent>(OnDayStarted);
        _dispatcher.Subscribe<GameUpdateTickEvent>(OnUpdateTick);
    }

    private void OnDayStarted(GameDayStartedEvent evt)
    {
        logger.Log($"Day started: {evt.Day}");

        // Regenerate daily tasks
        var tasks = _generator.GenerateForDay(evt.Day);
        _stateStore.DispatchCommand(new ResetDailyTasksCommand(tasks));
    }

    private void OnUpdateTick(GameUpdateTickEvent evt)
    {
        // Evaluate task conditions every N ticks (not every frame)
        EvaluateTaskCompletions();
    }
}
```

## 4. Task Evaluation Trigger Points ##

**When Tasks Are Generated/Evaluated:**

| Trigger | Frequency | Reason |
|---------|-----------|--------|
| **SaveLoaded** | Once per load | Reconstruct all tasks from persisted rules |
| **DayStarted** | Once per in-game day | Generate fresh daily task set for new day |
| **UpdateTicked** | Every 10-60 frames | Check task completion conditions, recurring tasks |
| **Rule modified** | On-demand | Regenerate affected tasks if rule changed |
| **User command** | On-demand | Generate or refresh tasks manually |

**Update Tick Frequency Bounds:**
```csharp
public sealed class TaskEvaluator
{
    private DateTime _lastEvaluationTime = DateTime.UtcNow;
    private const int MIN_EVALUATION_INTERVAL_MS = 100;  // At least 100ms between evals

    public void OnUpdateTick(GameUpdateTickEvent evt)
    {
        var elapsed = DateTime.UtcNow - _lastEvaluationTime;

        if (elapsed.TotalMilliseconds < MIN_EVALUATION_INTERVAL_MS)
            return;  // Skip; too soon

        // Evaluate task completions
        EvaluateCompletions();

        _lastEvaluationTime = DateTime.UtcNow;
    }

    private void EvaluateCompletions()
    {
        var snapshot = _stateStore.GetCurrentSnapshot();

        // Check each task's completion condition
        foreach (var task in snapshot.Tasks)
        {
            if (IsTaskCompleted(task))
            {
                _stateStore.DispatchCommand(
                    new CompleteTaskCommand(task.TaskId, DateTime.Now));
            }
        }
    }
}
```

**Avoiding Update Tick Thrashing:**
```csharp
// ❌ BAD: Evaluates every frame (60+ times per second)
_smapiEvents.UpdateTicked += (_, args) =>
{
    if (args.TicksElapsed % 1 == 0)  // Every frame
        EvaluateTasks();  // Too frequent!
};

// ✓ GOOD: Evaluates ~10 times per second
_smapiEvents.UpdateTicked += (_, args) =>
{
    if (args.TicksElapsed % 6 == 0)  // Every ~6 frames at 60fps
        EvaluateTasks();  // Reasonable frequency
};

// ✓ GOOD: Evaluates only on specific events
_dispatcher.Subscribe<GameUpdateTickEvent>(evt =>
{
    if (DateTime.UtcNow - _lastEval > TimeSpan.FromMilliseconds(100))
    {
        EvaluateTasks();
        _lastEval = DateTime.UtcNow;
    }
});
```

## 5. Reconciliation After Reload ##

**Problem: Persisted Tasks vs Regenerated Tasks**
```
Scenario:
1. Day 1: Game generates Task A (from rule: daily chores)
2. User saves
3. Next session: Game loads → must regenerate Task A
4. Without reconciliation: Thinks Task A is new → duplicates it
```

**Reconciliation Pattern (Merge Logic):**
```csharp
public sealed class TaskReconciler
{
    public IReadOnlyList<Task> ReconcileAfterReload(
        IReadOnlyList<Task> persistedTasks,
        IReadOnlyList<Task> regeneratedTasks)
    {
        // Create merged set: keep persisted + add only new regenerated
        var merged = new List<Task>(persistedTasks);

        // Find newly generated tasks (not in persisted set)
        var persistedIds = persistedTasks.Select(t => t.TaskId).ToHashSet();
        var newTasks = regeneratedTasks
            .Where(t => !persistedIds.Contains(t.TaskId))
            .ToList();

        merged.AddRange(newTasks);

        logger.Log(
            $"Reconciliation: {persistedTasks.Count} persisted + " +
            $"{newTasks.Count} new = {merged.Count} total");

        return merged;
    }
}

// Usage in LifecycleCoordinator.Initialize
public async Task InitializeAsync()
{
    // Load persisted tasks
    var persistedTasks = await _persistence.LoadAsync();

    // Regenerate from rules
    var regeneratedTasks = await _generator.RegenerateAllAsync(_currentDay);

    // Merge without duplicates
    var reconciled = _reconciler.ReconcileAfterReload(
        persistedTasks,
        regeneratedTasks);

    _stateStore.LoadSnapshot(new ApplicationSnapshot(Tasks: reconciled));
}
```

## 6. Lifecycle Initialization (SaveLoaded) ##

**Complete SaveLoaded Flow:**
```csharp
public async Task InitializeModAsync()
{
    _smapiEvents.SaveLoaded += async (sender, args) =>
    {
        try
        {
            logger.Log("=== JAT Initialization Started ===");

            // Phase 1: Load persisted configuration
            var config = await _configLoader.LoadAsync();
            logger.Log($"Configuration loaded: {config.EnabledFeatures.Count} features");

            // Phase 2: Load persisted state (tasks, completions, rules)
            var persistedState = await _persistence.LoadAsync();
            logger.Log($"Persisted state loaded: {persistedState.Rules.Count} rules");

            // Phase 3: Reconstruct tasks from rules
            var reconstructed = await _reconstructor.ReconstructAsync(
                persistedState,
                _gameState);
            logger.Log($"Tasks reconstructed: {reconstructed.Tasks.Count} tasks");

            // Phase 4: Load into State Store
            _stateStore.LoadSnapshot(reconstructed);

            // Phase 5: Subscribe to game events
            _eventBridge.HookIntoSmapiEvents();

            logger.Log("=== JAT Initialization Complete ===");
            _coordinator.SetPhase(LifecyclePhase.Ready);
        }
        catch (Exception ex)
        {
            logger.LogError($"Initialization failed: {ex}");
            logger.LogError("JAT will be unavailable this session");
            // Game continues safely; JAT just not functional
        }
    };
}
```

**Initialization Safety:**
- [ ] All databases/files loaded before setting Ready phase
- [ ] Errors logged with context (file paths, version numbers)
- [ ] Game continues if JAT initialization fails (graceful degradation)
- [ ] Async operations completed before event handlers return

## 7. Lifecycle Phases for DayStarted ##

**Handling Day Transitions:**
```csharp
public void OnDayStarted(GameDayStartedEvent evt)
{
    if (!_coordinator.CanTransitionToEvaluating())
    {
        logger.LogWarning("Ignoring DayStarted; coordinator not ready");
        return;
    }

    _coordinator.SetPhase(LifecyclePhase.Evaluating);

    try
    {
        var previousDay = _stateStore.GetCurrentSnapshot().Day;
        var newDay = evt.Day;

        logger.Log($"Day transition: {previousDay} → {newDay}");

        // Step 1: Reset daily-completion tracking (preserve across-day completions)
        _stateStore.DispatchCommand(new ResetDailyMarksCommand());

        // Step 2: Regenerate daily tasks for new day
        var dailyTasks = _generator.GenerateDailyTasks(newDay);
        _stateStore.DispatchCommand(new SetDailyTasksCommand(dailyTasks));

        // Step 3: Persist (save day transition point)
        await _persistence.SaveAsync(_stateStore.GetCurrentSnapshot());

        logger.Log($"Day started successfully: {newDay}, " +
                  $"{dailyTasks.Count} tasks generated");
    }
    finally
    {
        _coordinator.SetPhase(LifecyclePhase.Ready);
    }
}
```

## 8. Hooking New Features into Lifecycle ##

**Checklist for Integrating New Feature:**

- [ ] **Identify trigger event**: Which SMAPI event(s) should launch feature? (SaveLoaded, DayStarted, UpdateTicked, SaveCompleted)
- [ ] **Map to domain event**: Create JAT domain event; add to event bus translation
- [ ] **Implement listener**: Subscribe to event; handle in listener class
- [ ] **Phase safety**: Ensure listener only runs in Ready phase
- [ ] **Error recovery**: Feature failure doesn't break lifecycle
- [ ] **Logging**: Log key steps for debugging
- [ ] **Testing**: Test with mocked SMAPI events; no real game dependency

**Pattern: Adding Scoring Feature**
```csharp
// Domain event (decoupled from SMAPI)
public record DailyTasksScoredEvent(
    Dictionary<TaskID, int> ScoreMap
) : IDomainEvent;

// Event translation (in SmapiEventBridge)
_dispatcher.Dispatch(new GameDayStartedEvent(newDay));

// Listener (in TaskScoringEngine)
_dispatcher.Subscribe<GameDayStartedEvent>(evt =>
{
    if (!_coordinator.IsReady()) return;
    
    var scores = ComputeScoresForDay(evt.Day);
    
    _stateStore.DispatchCommand(new SetTaskScoresCommand(scores));
    _dispatcher.Dispatch(new DailyTasksScoredEvent(scores));
});

// Tests (no real game required)
[Test]
public void OnDayStarted_ComputesTaskScores()
{
    var bridge = new EventBridge();
    var engine = new TaskScoringEngine(_stateStore);
    engine.Subscribe();

    // Mock event
    bridge.Dispatch(new GameDayStartedEvent(DayKey.Parse("Spring-1")));

    // Verify scores computed
    Assert.That(_capturedEvents, Has.Some.TypeOf<DailyTasksScoredEvent>());
}
```

## 9. Testing Without Game Instance ##

**Mock SMAPI Events for Unit Testing:**
```csharp
[TestFixture]
public class LifecycleTests
{
    private MockSmapiEvents _mockEvents = null!;
    private LifecycleCoordinator _coordinator = null!;
    private InMemoryPersistence _persistence = null!;

    [SetUp]
    public void SetUp()
    {
        _mockEvents = new MockSmapiEvents();
        _persistence = new InMemoryPersistence();
        
        _coordinator = new LifecycleCoordinator(
            _mockEvents,
            _persistence,
            new MockGameState());
    }

    [Test]
    public async Task OnSaveLoaded_InitializesState()
    {
        // Arrange: Pre-populate persistence
        await _persistence.SaveAsync(new PersistedState(
            Rules: ImmutableList.Create(CreateTestRule())
        ));

        var initialized = false;
        _coordinator.InitializationComplete += () => initialized = true;

        // Act
        _mockEvents.RaiseSaveLoaded(new SaveLoadedEventArgs());

        // Assert
        Assert.That(initialized, Is.True);
        Assert.That(_stateStore.GetCurrentSnapshot().Rules.Count, Is.EqualTo(1));
    }

    [Test]
    public void OnDayStarted_RegeneratesDailyTasks()
    {
        // Arrange
        _coordinator.Initialize();
        var previousSnapshot = _stateStore.GetCurrentSnapshot();

        // Act
        _mockEvents.RaiseDayStarted(new DayStartedEventArgs());

        // Assert
        var newSnapshot = _stateStore.GetCurrentSnapshot();
        // Verify tasks regenerated (or verify day field updated)
        Assert.That(newSnapshot.Day, Is.Not.EqualTo(previousSnapshot.Day));
    }
}
```

## Links ##
- [Backend Architecture Contract](../Contracts/BACKEND-ARCHITECTURE-CONTRACT.instructions.md)
- [External Resources Instructions](../Instructions/external-resources.instructions.md)
- [Unit Testing Contract](../Contracts/UNIT-TESTING-CONTRACT.instructions.md)
