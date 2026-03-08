---
name: jat-dependency-injection-and-composition
description: "Constructor injection, dependency graphs, BootstrapContainer pattern, adding dependencies, testing dependency graphs, circular dependencies, optional dependencies. Use when: designing component dependencies, wiring the composition root, adding new services, or debugging injection issues."
argument-hint: "Describe the dependency structure: what components need what dependencies, and where should the composition root wire them?"
target: vscode
---

# Dependency Injection and Composition Skill

JAT uses constructor injection to declare dependencies explicitly, enabling testability and clear component relationships. BootstrapContainer orchestrates the composition root where all dependencies are wired. Understanding dependency graphs, injection patterns, and circular dependency resolution prevents tight coupling and runtime failures.

## 1. Constructor Injection (Required Parameters) ##

**Why Constructor Injection:**
- **Explicit Dependencies**: Required parameters make dependencies visible
- **Immutability**: Dependencies assigned once; can't be swapped later (safer)
- **Testability**: Easy to provide mocks for testing
- **Fail Fast**: Missing dependencies detected at construction, not at use
- **No Service Locator**: Avoids hidden dependencies and ambient state

**Declaring Dependencies:**
```csharp
// ✓ GOOD: Dependencies as required constructor parameters
public sealed class TaskGenerator
{
    private readonly IGameStateProvider _gameState;
    private readonly IModLogger _logger;
    private readonly ITaskRepository _repository;

    public TaskGenerator(
        IGameStateProvider gameState,
        IModLogger logger,
        ITaskRepository repository)
    {
        _gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public IReadOnlyList<Task> GenerateDailyTasks(DayKey day)
    {
        // Use injected dependencies
        var npcCount = _gameState.Characters.Count;
        _logger.Log($"Generating tasks for {npcCount} characters");
        
        var tasks = new List<Task>();
        // ... generation logic
        return tasks;
    }
}

// ❌ BAD: Hidden dependencies (dependency unclear, hard to test)
public sealed class TaskGenerator
{
    public IReadOnlyList<Task> GenerateDailyTasks(DayKey day)
    {
        var logger = ModEntry.Logger;  // Service locator (bad!)
        var gameState = ModEntry.GameState;  // Ambient state (bad!)
        
        // ... usage
    }
}
```

**Testing with Injected Dependencies:**
```csharp
[Test]
public void GenerateDailyTasks_WithMockGameState_ProducesTasksForAllCharacters()
{
    // Arrange: Create mocks
    var mockGameState = new MockGameStateProvider
    {
        Characters = new[] { "Abigail", "Sebastian", "Leah" }
    };
    var mockLogger = new MockLogger();
    var mockRepository = new InMemoryTaskRepository();

    // Inject mocks
    var generator = new TaskGenerator(mockGameState, mockLogger, mockRepository);

    // Act
    var tasks = generator.GenerateDailyTasks(DayKey.Parse("Spring-1"));

    // Assert
    Assert.That(tasks.Count, Is.EqualTo(3));  // One task per character
}
```

## 2. Dependency Graph Visualization ##

**Understanding Component Relationships:**

```
ModEntry (entry point)
  ↓
BootstrapContainer (composition root)
  ├─→ IGameStateProvider
  │    └─ GameStateProvider
  │       └─ [reads from game]
  │
  ├─→ IModLogger
  │    └─ ModLogger
  │       (depends on SMAPI IMonitor)
  │
  ├─→ IEventDispatcher
  │    └─ EventDispatcher (stateless)
  │
  ├─→ ITaskGenerator
  │    └─ TaskGenerator
  │       ├─ IGameStateProvider
  │       ├─ IModLogger
  │       └─ ITaskRepository
  │
  ├─→ IStateStore
  │    └─ StateStore
  │       ├─ MyReducer (stateless; injected as behavior)
  │       ├─ IPersistenceProvider
  │       └─ IEventDispatcher
  │
  └─→ LifecycleCoordinator
       ├─ IStateStore
       ├─ IPersistenceProvider
       ├─ IEventDispatcher
       ├─ IGameStateProvider
       └─ ITaskGenerator
```

**Dependency Graph Analysis:**
- **Leaf nodes** (no dependencies): IGameStateProvider, IModLogger, EventDispatcher
- **Mid-level** (depend on leaves): TaskGenerator, StateStore
- **Root** (depends on most): LifecycleCoordinator

**Circular Dependency Detection:**
```csharp
// ❌ CIRCULAR (BAD): A → B → A
public class ComponentA
{
    public ComponentA(ComponentB b) { }
}

public class ComponentB
{
    public ComponentB(ComponentA a) { }
}

// Result: Cannot construct either!
var a = new ComponentA(new ComponentB(new ComponentA(...)));  // Infinite recursion

// ✓ SOLUTION: Inject interface; resolve at root
public interface IComponentB { }

public class ComponentA
{
    public ComponentA(IComponentB b) { }
}

public class ComponentB : IComponentB
{
    public ComponentB(ComponentA a) { }  // Now breaking the cycle
}

// Root wires carefully:
var a = new ComponentA(new ComponentB(a));  // Root controls order
```

## 3. BootstrapContainer Pattern ##

**Container Responsibility:**
- Construct all components
- Inject dependencies in correct order
- Handle singletons (one instance shared)
- Return root service for use in ModEntry

**Typical BootstrapContainer Implementation:**
```csharp
public sealed class BootstrapContainer
{
    private readonly IMonitor _smapiMonitor;
    private readonly IModHelper _smapiHelper;

    public BootstrapContainer(IMonitor monitor, IModHelper helper)
    {
        _smapiMonitor = monitor;
        _smapiHelper = helper;
    }

    /// <summary>
    /// Builds the application container. Called once during mod initialization.
    /// </summary>
    public ModRuntime BuildContainer()
    {
        // Layer 1: Infrastructure (no JAT dependencies)
        var logger = new ModLogger(_smapiMonitor);
        var gameState = new GameStateProvider();
        var eventDispatcher = new EventDispatcher();
        var persistence = new PersistenceProvider(
            Path.Combine(_smapiHelper.DirectoryPath, "data.json"));

        // Layer 2: Domain services (depend on Layer 1)
        var taskRepository = new TaskRepository();
        var taskGenerator = new TaskGenerator(gameState, logger, taskRepository);

        // Layer 3: State management (depend on layers 1–2)
        var stateStore = new StateStore(
            reducer: MyReducer.Reduce,
            persistence: persistence,
            logger: logger);

        // Layer 4: Coordination (depends on most layers)
        var lifecycle = new LifecycleCoordinator(
            stateStore: stateStore,
            persistence: persistence,
            eventDispatcher: eventDispatcher,
            gameState: gameState,
            taskGenerator: taskGenerator,
            logger: logger);

        // Return root service
        return new ModRuntime(
            StateStore: stateStore,
            EvenDispatcher: eventDispatcher,
            LifecycleCoordinator: lifecycle,
            Logger: logger);
    }
}

// Usage in ModEntry
public class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        var container = new BootstrapContainer(this.Monitor, helper);
        var runtime = container.BuildContainer();

        // Hook into game events
        runtime.LifecycleCoordinator.Initialize();
    }
}
```

## 4. Adding New Dependencies (Checklist) ##

**Steps to Add a New Service:**

1. **Define Interface:**
   ```csharp
   public interface INewService
   {
       void DoSomething();
   }
   ```

2. **Implement Class:**
   ```csharp
   public sealed class NewService : INewService
   {
       private readonly IDependency1 _dep1;
       private readonly IDependency2 _dep2;

       public NewService(IDependency1 dep1, IDependency2 dep2)
       {
           _dep1 = dep1 ?? throw new ArgumentNullException(nameof(dep1));
           _dep2 = dep2 ?? throw new ArgumentNullException(nameof(dep2));
       }

       public void DoSomething()
       {
           // Implement using injected dependencies
       }
   }
   ```

3. **Wire in BootstrapContainer:**
   ```csharp
   public ModRuntime BuildContainer()
   {
       // ... existing wiring ...

       // Wire new service (after its dependencies)
       var newService = new NewService(
           dep1: existingService1,
           dep2: existingService2);

       // If other services depend on INewService, inject it:
       var consumer = new ServiceThatUsesNew(newService, ...);

       return new ModRuntime(
           // ... exposing services used by ModEntry ...
           NewService: newService);
   }
   ```

4. **Test the Dependency Graph:**
   ```csharp
   [Test]
   public void Container_BuildsSuccessfully()
   {
       var container = new BootstrapContainer(...);
       var runtime = container.BuildContainer();  // Should not throw

       Assert.That(runtime, Is.Not.Null);
       Assert.That(runtime.NewService, Is.Not.Null);
   }
   ```

**Dependency Addition Checklist:**
- [ ] Interface defined (contracts what service provides)
- [ ] Implementation complete (all methods implemented)
- [ ] Constructor declares all dependencies explicitly
- [ ] Wired in BootstrapContainer (after its dependencies)
- [ ] All consumers of INewService updated to receive it
- [ ] No circular dependencies introduced
- [ ] Unit test verifies dependency graph builds

## 5. Testing Dependency Graphs ##

**Test: Graph Construction Succeeds**
```csharp
[TestFixture]
public class DependencyGraphTests
{
    [Test]
    public void BootstrapContainer_BuildContainer_Succeeds()
    {
        // Arrange
        var mockMonitor = new MockMonitor();
        var mockHelper = new MockModHelper();
        var container = new BootstrapContainer(mockMonitor, mockHelper);

        // Act
        var runtime = container.BuildContainer();

        // Assert
        Assert.That(runtime, Is.Not.Null);
        Assert.That(runtime.StateStore, Is.Not.Null);
        Assert.That(runtime.LifecycleCoordinator, Is.Not.Null);
    }

    [Test]
    public void DependenciesAreWiredCorrectly()
    {
        // Arrange
        var container = new BootstrapContainer(...);
        var runtime = container.BuildContainer();

        // Act & Assert: Verify key dependencies are injected
        Assert.That(runtime.StateStore, Is.Not.Null);
        
        // Can invoke methods; no NullReferenceException
        var snapshot = runtime.StateStore.GetCurrentSnapshot();
        Assert.That(snapshot, Is.Not.Null);
    }
}
```

**Test: Missing Dependency is Caught**
```csharp
[Test]
public void NewComponent_WithoutDependency_FailsConstruction()
{
    // Arrange
    var missingDep = null as IRequiredService;

    // Act & Assert
    Assert.Throws<ArgumentNullException>(() =>
    {
        new ComponentThatNeedsService(missingDep);
    });
}
```

## 6. Circular Dependency Resolution ##

**Pattern 1: Inject Interface Instead of Concrete Class**
```csharp
// ❌ CIRCULAR: ComponentA → ComponentB → ComponentA
public class ComponentA
{
    public ComponentA(ComponentB b) { }
}

public class ComponentB
{
    public ComponentB(ComponentA a) { }
}

// ✓ SOLUTION: Use interface; lazy-wire at root
public interface IComponentA { }

public class ComponentA : IComponentA
{
    public ComponentA(IComponentB b) { }
}

public class ComponentB : IComponentB
{
    public ComponentB(IComponentA a) { }
}

// Root can resolve:
IComponentA a = new ComponentA(
    new ComponentB(a)  // 'a' available by reference at root
);
```

**Pattern 2: Extract Shared Interface (Avoid Bidirectional Dependency)**
```csharp
// ❌ CIRCULAR: TaskGenerator ↔ StateStore both need each other
public class TaskGenerator
{
    public TaskGenerator(StateStore store) { /* store.GenerateTasks(this); */ }
}

public class StateStore
{
    public StateStore(TaskGenerator generator) { }
}

// ✓ SOLUTION: Use an event/callback instead of bidirectional reference
public interface ITaskGenerationListener
{
    void OnTaskGenerated(Task task);
}

public class TaskGenerator
{
    public TaskGenerator(ITaskGenerationListener listener)
    {
        // Call listener.OnTaskGenerated instead of StateStore.AddTask
    }
}

public class StateStore : ITaskGenerationListener
{
    public StateStore(TaskGenerator generator)
    {
        // generator will call OnTaskGenerated on this
    }

    public void OnTaskGenerated(Task task)
    {
        _tasks.Add(task);
    }
}
```

## 7. Optional Dependencies and Null-Coalescing ##

**Optional Dependency Pattern:**
```csharp
// Component may or may not have logging
public sealed class OptionalDependencyComponent
{
    private readonly IModLogger _logger;

    public OptionalDependencyComponent(IModLogger logger = null)
    {
        _logger = logger;  // Can be null; coalesce on use
    }

    public void DoWork()
    {
        _logger?.Log("Starting work");  // Safe null-conditional
        
        try
        {
            // ... work ...
            _logger?.Log("Work completed");
        }
        catch (Exception ex)
        {
            _logger?.LogError($"Work failed: {ex.Message}");
        }
    }
}

// Wiring (logger optional)
var service = new OptionalDependencyComponent(logger: null);  // Allowed
var service2 = new OptionalDependencyComponent(logger: _logger);  // Provided if available
```

## Links ##
- [C# Style Contract](../Contracts/CSHARP-STYLE-CONTRACT.instructions.md)
- [Backend Architecture Contract](../Contracts/BACKEND-ARCHITECTURE-CONTRACT.instructions.md)
