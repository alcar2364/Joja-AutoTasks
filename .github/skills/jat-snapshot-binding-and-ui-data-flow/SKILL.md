---
name: jat-snapshot-binding-and-ui-data-flow
description: "Snapshot role in UI binding, immutability contracts, StardewUI binding patterns, change notification, anti-patterns, and performance guardrails. Use when: connecting State Store snapshots to UI, implementing UI data binding, or debugging UI synchronization issues."
argument-hint: "Describe the UI requirement: what snapshot data needs to display, how should user actions update state, and what triggers UI refresh?"
---

# Snapshot Binding and UI Data Flow Skill

Snapshots provide immutable, read-only views of application state for UI consumption. Binding snapshots to StardewUI components ensures UI always reflects current state while preventing accidental mutations. Understanding the snapshot-driven data flow, immutability contracts, and binding patterns is critical for building responsive, consistent user interfaces.

## 1. Snapshot Role in UI Architecture ##

**Snapshot Design Principles:**
- **Read-Only View**: Snapshot represents state at a moment in time; UI components consume it without modification
- **Immutable Contract**: All properties are sealed; no setters; no mutable collections
- **Independent from Internal State**: UI never accesses internal state directly; only through snapshots
- **Change Notification**: When state changes, new snapshot is published; UI receives notification and refreshes

**Snapshot vs Internal State:**
```csharp
// Internal state (mutable, UI-invisible)
public sealed class ApplicationState
{
    public ImmutableList<Task> Tasks { get; set; }  // Mutable holder
    public Dictionary<TaskID, Completion> Completions { get; set; }  // Mutable
}

// Snapshot (immutable, UI-visible)
public record ApplicationSnapshot(
    IReadOnlyCollection<Task> Tasks,              // Read-only view
    IReadOnlyDictionary<TaskID, Completion> Completions  // Read-only view
)
{
    // No setters; sealed record ensures immutability
}

// StateStore exposes snapshots for UI
public sealed class StateStore
{
    private ApplicationState _internalState;

    public ApplicationSnapshot GetCurrentSnapshot()
    {
        return new ApplicationSnapshot(
            Tasks: _internalState.Tasks.ToReadOnlyCollection(),
            Completions: _internalState.Completions.AsReadOnly()
        );
    }
}
```

## 2. Snapshot Creation and Publishing Lifecycle ##

**Publication Flow:**
```csharp
public sealed class StateStore : IStateStore
{
    private ApplicationState _currentState = new();
    private ApplicationSnapshot _currentSnapshot = null!;
    
    // Event: UI subscribes to changes
    public event Action<ApplicationSnapshot>? SnapshotChanged;

    public void DispatchCommand(ICommand command)
    {
        // Step 1: Validate
        ValidateCommand(command);

        // Step 2: Apply command to state (pure reducer)
        var previousState = _currentState;
        _currentState = Reducer.Reduce(_currentState, command);

        // Step 3: Create new immutable snapshot
        _currentSnapshot = CreateSnapshot(_currentState);

        // Step 4: Publish snapshot to UI
        RaiseSnapshotChanged();

        // Step 5: Persist (if needed)
        PersistSnapshot(_currentSnapshot);
    }

    private void RaiseSnapshotChanged()
    {
        var snapshotAtTimeOfEvent = _currentSnapshot;  // Capture reference
        
        // Invoke subscribers (typically async via event loop)
        try
        {
            SnapshotChanged?.Invoke(snapshotAtTimeOfEvent);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error in snapshot change handler: {ex}");
        }
    }

    public ApplicationSnapshot GetCurrentSnapshot() => _currentSnapshot;
}
```

**Lifecycle Guarantees:**
- [ ] Snapshot created **only after** state fully constructed
- [ ] Snapshot published **exactly once** per command
- [ ] Old snapshot references remain valid (immutable); don't refresh old references
- [ ] UI receives notification asynchronously (no blocking operations)
- [ ] Persistence happens after snapshot published (UI not blocked by save)

## 3. StardewUI Snapshot Binding Pattern ##

**Basic Binding Pattern:**
```csharp
public sealed class TaskListMenu : IMenu
{
    private readonly IStateStore _stateStore;
    private readonly StardewUIFactory _uiFactory;
    
    private ApplicationSnapshot _currentSnapshot = null!;
    private View _rootView = null!;

    public TaskListMenu(IStateStore stateStore, StardewUIFactory uiFactory)
    {
        _stateStore = stateStore;
        _uiFactory = uiFactory;

        // Subscribe to snapshot changes
        _stateStore.SnapshotChanged += HandleSnapshotChanged;

        // Initialize with current snapshot
        _currentSnapshot = _stateStore.GetCurrentSnapshot();
        _rootView = BuildUIFromSnapshot(_currentSnapshot);
    }

    private void HandleSnapshotChanged(ApplicationSnapshot newSnapshot)
    {
        // Called when state changes
        _currentSnapshot = newSnapshot;
        
        // Rebuild UI with new snapshot
        _rootView = BuildUIFromSnapshot(_currentSnapshot);
    }

    private View BuildUIFromSnapshot(ApplicationSnapshot snapshot)
    {
        return _uiFactory.Column(
            snapshot.Tasks
                .OrderBy(t => t.Priority)
                .Select(t => BuildTaskRow(t))
                .ToList()
        );
    }

    private View BuildTaskRow(Task task)
    {
        return _uiFactory.Row(
            _uiFactory.Text(task.Title),
            _uiFactory.Button("Complete", () => OnCompleteTask(task))
        );
    }

    private void OnCompleteTask(Task task)
    {
        // User action → dispatch command
        var command = new CompleteTaskCommand(task.TaskId);
        _stateStore.DispatchCommand(command);
        // Snapshot change will trigger HandleSnapshotChanged → UI rebuild
    }

    public void Dispose()
    {
        _stateStore.SnapshotChanged -= HandleSnapshotChanged;
    }
}
```

**Key Binding Principles:**
1. **Subscribe to SnapshotChanged Event**: Receive notifications when state updates
2. **Rebuild UI from Snapshot**: Create fresh UI when snapshot changes
3. **User Actions → Dispatch Commands**: Map UI interactions to state mutations
4. **Closure over Snapshot**: UI methods capture current snapshot; old references become stale

## 4. Immutability Contract Enforcement ##

**Sealed Record Prevents Mutation:**
```csharp
// ✓ GOOD: Sealed record, no setters, read-only properties
public record ApplicationSnapshot(
    IReadOnlyCollection<Task> Tasks,
    IReadOnlyDictionary<TaskID, Completion> Completions
);

// Usage: UI can read but not write
var snapshot = _stateStore.GetCurrentSnapshot();
var tasks = snapshot.Tasks;  // ✓ Can read

// snapshot.Tasks.Add(newTask);  // ❌ Compile error: IReadOnlyCollection has no Add
// snapshot.Tasks = newList;     // ❌ Compile error: no setter
// snapshot = null;              // ❌ (reference can be reassigned, but snapshot itself is immutable)
```

**Enforcing Read-Only Collections:**
```csharp
// ❌ BAD: Public mutable collection
public record BadSnapshot(List<Task> Tasks);

// Caller can mutate
var snapshot = store.GetCurrentSnapshot();
snapshot.Tasks.Add(newTask);  // Corrupts snapshot!

// ✓ GOOD: Read-only wrapper
public record GoodSnapshot(
    IReadOnlyCollection<Task> Tasks
);

// Internally backed by immutable list
private ApplicationSnapshot CreateSnapshot(ApplicationState state)
{
    return new ApplicationSnapshot(
        Tasks: state.Tasks.ToReadOnlyCollection(),  // IReadOnlyCollection
        Completions: state.Completions.AsReadOnly()  // Read-only dict view
    );
}
```

**Testing Immutability:**
```csharp
[Test]
public void Snapshot_ImmutableCollections_PreventWrite()
{
    // Arrange
    var snapshot = new ApplicationSnapshot(
        Tasks: ImmutableList.Create(CreateTestTask()).AsReadOnly()
    );

    // Act & Assert: Attempt to write fails
    Assert.Throws<NotSupportedException>(() => 
    {
        snapshot.Tasks.Add(CreateTestTask());
    });

    Assert.Throws<NotSupportedException>(() => 
    {
        ((IList)snapshot.Tasks).Clear();
    });
}

[Test]
public void Snapshot_RecordSealed_PreventsMutation()
{
    // Arrange
    var originalSnapshot = new ApplicationSnapshot(
        Tasks: ImmutableList.Create(CreateTestTask())
    );

    // Act: Create "modified" snapshot (creates new instance)
    var newSnapshot = originalSnapshot with 
    { 
        Tasks = ImmutableList.Create(CreateTestTask(), CreateTestTask()) 
    };

    // Assert: Original unchanged
    Assert.That(originalSnapshot.Tasks.Count, Is.EqualTo(1));
    Assert.That(newSnapshot.Tasks.Count, Is.EqualTo(2));
}
```

## 5. Snapshot Change Notification ##

**Change Event Pattern (Reliable Notifications):**
```csharp
public sealed class StateStore : IStateStore
{
    // ✓ Use Action<T> for simple notification
    public event Action<ApplicationSnapshot>? SnapshotChanged;

    // Alternative: Custom event for more control
    public event EventHandler<SnapshotChangedEventArgs>? SnapshotChangedExt;

    private void PublishSnapshot(ApplicationSnapshot snapshot)
    {
        // Notify UI
        SnapshotChanged?.Invoke(snapshot);

        // Or with extended event
        SnapshotChangedExt?.Invoke(this, new SnapshotChangedEventArgs(snapshot));
    }
}

// Usage in UI
public TaskListMenu(IStateStore stateStore)
{
    _stateStore = stateStore;

    // Subscribe
    _stateStore.SnapshotChanged += OnSnapshotChanged;
}

private void OnSnapshotChanged(ApplicationSnapshot newSnapshot)
{
    logger.Log($"Snapshot updated: {newSnapshot.Tasks.Count} tasks");
    
    // Trigger UI rebuild
    RefreshUI(newSnapshot);
}
```

**Avoiding Missed Notifications:**
```csharp
// ❌ BAD: Snapshot might change during subscription
var snapshot = store.GetCurrentSnapshot();
store.SnapshotChanged += HandleChange;  // Change happens here!
RefreshUI(snapshot);  // UI uses stale snapshot

// ✓ GOOD: Subscribe first, then get initial snapshot
store.SnapshotChanged += HandleChange;
RefreshUI(store.GetCurrentSnapshot());  // Always fresh

// ✓ GOOD: Single subscription point
public TaskListMenu(IStateStore stateStore)
{
    _stateStore = stateStore;
    _stateStore.SnapshotChanged += HandleSnapshotChanged;
    
    // Initialize
    HandleSnapshotChanged(_stateStore.GetCurrentSnapshot());
}

private void HandleSnapshotChanged(ApplicationSnapshot snapshot)
{
    _currentSnapshot = snapshot;
    RefreshUI();
}
```

## 6. Anti-patterns and Common Mistakes ##

| Anti-Pattern | Why It's Bad | Fix |
|--------------|-------------|-----|
| UI mutates snapshot | Breaks immutability; corrupts state | Use sealed records + read-only collections |
| Snapshot contains mutable objects | Snapshot "immutable" but inner objects mutable | Use immutable value types; serialize to create copy |
| Capturing old snapshot in closures | UI callback uses stale data | Capture current snapshot in handler; refresh on each change |
| Direct access to _state (bypassing snapshot) | UI sees uncommitted state; race conditions | Always route UI through snapshots only |
| Snapshot caching without subscription | UI misses updates | Always subscribe to SnapshotChanged event |
| Multiple snapshots for same state | Inconsistent views; difficult reasoning | Single source of truth; one snapshot per state |
| Blocking operations in snapshot handlers | UI thread blocks; freezes interface | Keep handlers fast; defer expensive work to background |
| Unsubscribing on menu close | Memory leak if menu reopened | Ensure Dispose() unsubscribes listeners |

## 7. Performance Guardrails ##

**Snapshot Creation Efficiency:**
```csharp
// ❌ BAD: Deep clone of entire state
private ApplicationSnapshot CreateSnapshot(ApplicationState state)
{
    var cloned = JsonConvert.DeserializeObject(
        JsonConvert.SerializeObject(state)
    );
    return cloned;  // Expensive!
}

// ✓ GOOD: Lightweight read-only wrapper
private ApplicationSnapshot CreateSnapshot(ApplicationState state)
{
    return new ApplicationSnapshot(
        Tasks: state.Tasks.ToReadOnlyCollection(),
        Completions: state.Completions.AsReadOnly()
    );
}
```

**Snapshot Change Frequency:**
```csharp
// Monitor how often snapshots are published
public sealed class StateStore : IStateStore
{
    private int _snapshotPublishCount = 0;
    private DateTime _lastSnapshotTime = DateTime.UtcNow;

    private void PublishSnapshot(ApplicationSnapshot snapshot)
    {
        _snapshotPublishCount++;

        // Log if publishing too frequently (e.g., every frame)
        if (_snapshotPublishCount % 100 == 0)
        {
            var elapsed = DateTime.UtcNow - _lastSnapshotTime;
            logger.Log($"Published {100} snapshots in {elapsed.TotalMilliseconds:F1}ms");
        }

        _lastSnapshotTime = DateTime.UtcNow;
        SnapshotChanged?.Invoke(snapshot);
    }
}

// Expected: ~1 snapshot per second or less, not per frame (60+ per second)
```

**UI Refresh Optimization:**
```csharp
// ❌ BAD: Rebuild entire UI on every snapshot change
private void OnSnapshotChanged(ApplicationSnapshot snapshot)
{
    _rootView = BuildUIFromSnapshot(snapshot);  // Recreates everything
}

// ✓ GOOD: Selective updates based on changed data
private void OnSnapshotChanged(ApplicationSnapshot snapshot)
{
    if (!SnapshotTasksChanged(snapshot))
        return;  // No task changes; UI rebuild not needed

    _rootView = BuildUIFromSnapshot(snapshot);
}

private bool SnapshotTasksChanged(ApplicationSnapshot newSnapshot)
{
    if (_currentSnapshot == null)
        return true;

    return !_currentSnapshot.Tasks.SequenceEqual(newSnapshot.Tasks);
}
```

## 8. Testing Snapshot Binding ##

**Test: Snapshot Immutability Enforced**
```csharp
[TestFixture]
public class SnapshotBindingTests
{
    [Test]
    public void Snapshot_ReadOnlyCollections_PreventWrite()
    {
        // Arrange
        var snapshot = new ApplicationSnapshot(
            Tasks: ImmutableList.Create(CreateTestTask()).AsReadOnly()
        );

        // Act & Assert
        Assert.Throws<NotSupportedException>(() => 
        {
            snapshot.Tasks.Add(CreateTestTask());
        });
    }

    [Test]
    public void StateStore_SnapshotChanged_NotifiesUI()
    {
        // Arrange
        var store = new StateStore();
        var receivedSnapshots = new List<ApplicationSnapshot>();
        
        store.SnapshotChanged += snapshot => receivedSnapshots.Add(snapshot);

        // Act
        store.DispatchCommand(new CreateTaskCommand(...));

        // Assert
        Assert.That(receivedSnapshots, Has.Count.EqualTo(1));
        Assert.That(receivedSnapshots[0].Tasks.Count, Is.GreaterThan(0));
    }

    [Test]
    public void Menu_RefreshUI_UsesCurrentSnapshot()
    {
        // Arrange
        var store = new StateStore();
        var menu = new TaskListMenu(store);

        // Act: Dispatch command
        store.DispatchCommand(new CreateTaskCommand(...));

        // Assert: Menu received new snapshot
        Assert.That(menu.CurrentSnapshot.Tasks.Count, Is.GreaterThan(0));
    }
}
```

## Links ##
- [Frontend Architecture Contract](../Contracts/FRONTEND-ARCHITECTURE-CONTRACT.instructions.md)
- [Backend Architecture Contract](../Contracts/BACKEND-ARCHITECTURE-CONTRACT.instructions.md)
- [UI Component Patterns](../Instructions/ui-component-patterns.instructions.md)
