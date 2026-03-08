---
name: jat-smapi-debugging-and-diagnostics
Description: SMAPI logging, ModLogger usage, debug output interpretation, common SMAPI failures, game state inspection, MonitorProxy diagnostics, performance profiling, bug reproduction. Use when: debugging mod issues, interpreting SMAPI logs, capturing game state, or diagnosing performance problems.
argument-hint: "Describe the issue: what error is occurring, when does it happen, and what diagnostic information would help identify the root cause?"
target: vscode
---

# SMAPI Debugging and Diagnostics Skill

JAT runs under SMAPI, which provides logging, hooking, and reflection capabilities. Understanding SMAPI logging, debug techniques, error traces, and diagnostic patterns enables rapid problem identification and resolution.

## 1. SMAPI Logging Overview ##

**Log File Location:**
```
Windows:    C:\Program Files\Steam\steamapps\common\Stardew Valley\ErrorLogs\SMAPI-latest.txt
            C:\Program Files\Steam\steamapps\common\Stardew Valley\ErrorLogs\SMAPI-08-23-21.txt (dated)

macOS:      ~/Library/Application Support/ConcernedApe/Stardew Valley/ErrorLogs/SMAPI-latest.txt

Linux:      ~/.local/share/ConcernedApe/Stardew Valley/ErrorLogs/SMAPI-latest.txt
```

**Console Output:**
- When game runs via SMAPI, a console window appears
- Logs written to console in real-time (visible during gameplay)
- Also written to file above (useful for analysis)

**Log Formats:**
```
[06:43:28 INFO SMAPI] SMAPI 3.13.0 loaded successfully.

[06:43:29 WARN SMAPI] A mod failed to initialize: Author.JojaAutoTasks
    System.NullReferenceException: Object reference not set to an instance of an object.
      at JAT.Domain.TaskID..ctor(String value) in C:\...\TaskID.cs:line 42
      at JAT.ModEntry.Entry(IModHelper helper) in C:\...\ModEntry.cs:line 18

[06:43:30 INFO Author.JojaAutoTasks] Initialized successfully. 5 rules loaded.

[06:45:12 DEBUG Author.JojaAutoTasks] Evaluating tasks: 12 candidates generated.
```

**Log Levels:**
| Level | When to Use | Visibility |
|-------|------------|-----------|
| **TRACE** | Low-level diagnostics (internal function calls) | Console (verbose mode only) |
| **DEBUG** | Feature logic details ("evaluated X tasks") | Console (verbose mode) |
| **INFO** | User-facing milestones ("loaded 5 rules") | Console always |
| **WARN** | Recoverable issues ("unknown rule type") | Console always; flagged in log |
| **ERROR** | Non-recoverable issues (mod load fails) | Console always; flagged prominently |

## 2. ModLogger Usage ##

**Proper Logging Pattern:**
```csharp
public sealed class MyComponent
{
    private readonly IModLogger _logger;

    public MyComponent(IModLogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void DoWork()
    {
        _logger.Log("Starting work...");  // INFO level (implicit)

        try
        {
            // ... work ...
            _logger.Log("Work completed successfully");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning($"Recoverable issue: {ex.Message}");
            // Continue with safe fallback
        }
        catch (Exception ex)
        {
            _logger.LogError($"Critical failure: {ex}");  // Includes stack trace
            throw;  // Let caller handle
        }
    }

    public void ProcessTasks(IReadOnlyList<Task> tasks)
    {
        _logger.Log($"Processing {tasks.Count} tasks");

        foreach (var task in tasks)
        {
            _logger.Log($"  - Task: {task.Title} (ID: {task.TaskId})");
        }

        _logger.Log("Processing complete");
    }
}
```

**Structured Logging for Nested Logic:**
```csharp
public void RegenerateDailyTasks(DayKey day)
{
    _logger.Log($"=== Daily Regeneration (Day: {day}) ===");

    _logger.Log($"  Step 1: Loading persisted tasks...");
    var persisted = _persistence.Load();
    _logger.Log($"  ✓ Loaded {persisted.Count} persisted tasks");

    _logger.Log($"  Step 2: Generating new daily tasks...");
    var generated = GenerateNewTasks(day);
    _logger.Log($"  ✓ Generated {generated.Count} new tasks");

    _logger.Log($"  Step 3: Reconciling...");
    var merged = Reconcile(persisted, generated);
    _logger.Log($"  ✓ Reconciled to {merged.Count} total tasks");

    _logger.Log($"=== Daily Regeneration Complete ===");
}
```

**Avoid Logging Sensitive Data:**
```csharp
// ✓ GOOD: Log relevant data only
_logger.Log($"User {character.Name} affection: {character.Affection}");

// ❌ BAD: Logging entire object (may include private/sensitive data)
_logger.Log($"Character object: {character}");  // ToString() might leak internals

// ✓ GOOD: Log specific fields
_logger.Log($"Rule: ID={rule.Id}, Name={rule.Name}, Priority={rule.EvaluationOrder}");
```

## 3. Debug Output Interpretation ##

**Reading Stack Traces:**
```
System.NullReferenceException: Object reference not set to an instance of an object.
  at JAT.Domain.TaskID..ctor(String value) in C:\Path\TaskID.cs:line 42
  at JAT.ModEntry.Entry(IModHelper helper) in C:\Path\ModEntry.cs:line 18
```

**Interpreting:**
- **Line 1**: Exception type and message ("TID constructor null reference")
- **Line 2**: First error location (TaskID line 42: likely invalid input)
- **Line 3**: Call origin (ModEntry line 18: where TaskID created)

**Working Backward:**
1. Look at ModEntry line 18 → what passed to TaskID?
2. Look at TaskID line 42 → what was null?
3. Fix: validate input before passing to TaskID

**Common Stack Trace Patterns:**

| Pattern | Meaning | Fix |
|---------|---------|-----|
| `NullReferenceException` at property | Something is null | Add null check/validation |
| `IndexOutOfRangeException` in loop | Accessed invalid index | Check bounds before accessing |
| `InvalidOperationException` | State wrong for operation | Check phase/preconditions |
| `ArgumentException` | Parameter invalid | Validate inputs |
| `FileNotFoundException` | Expected file missing | Check file path; handle gracefully |

## 4. Common SMAPI Failures ##

**Failure: "Mod failed to initialize"**
```
[ERROR] A mod failed to initialize: Author.JojaAutoTasks
    System.NullReferenceException...
```

**Diagnosis Steps:**
1. Find the exact exception line in stack trace
2. Add null checks around that line
3. Add logging before the error line
4. Rebuild and run again; check new log

**Example Fix:**
```csharp
// Before (crashes):
public ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        var config = helper.ReadConfig<ModConfig>();  // Could be null!
        _logger.Log($"Version: {config.Version}");  // NRE if config null
    }
}

// After (safe):
public ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        var config = helper.ReadConfig<ModConfig>() 
                    ?? new ModConfig();  // Safe default
        _logger.Log($"Version: {config.Version}");  // Safe
    }
}
```

**Failure: "API not found" (Missing SMAPI API)**
```
[WARN] JojaAutoTasks: API request failed
    Cannot find API: Author.OtherMod
```

**Cause**: Mod depends on another mod that's not installed or not loaded yet

**Fix:**
```csharp
// Add error handling for optional APIs
var apiMod = helper.ModRegistry.GetApi("Author.OtherMod");
if (apiMod == null)
{
    _logger.LogWarning("Optional dependency 'OtherMod' not found. " +
                       "Some features disabled.");
    // Continue without optional API
}
else
{
    // Use API
}
```

**Failure: "Reflection error" (Can't access game field)**
```
[ERROR] Reflection error: Cannot access field 'gameStatus'
    System.FieldAccessException: Field 'StardewValley.Game1.gameStatus' is not accessible
```

**Cause**: Tried to access a private field that SMAPI can't access

**Fix:**
```csharp
// ❌ BAD: Direct private field access
int status = game1.gameStatus;  // Reflection error!

// ✓ GOOD: Use SMAPI reflection helper
var helper = ...;  // IModHelper
var status = helper.Reflection.GetField(game1, "gameStatus").GetValue();

// ✓ EVEN BETTER: Use public APIs when available
int status = game1.gameMode;  // Public property
```

## 5. Inspecting Game State ##

**SMAPI Console Commands:**
```
# List loaded mods
mods list

# Check JojaAutoTasks status
mods list Author.JojaAutoTasks

# Roll back to previous save
help rollback
rollback [name]

# Clear SMAPI cache
clear-cache

# Set log level (verbose)
set_log_level verbose
```

**Debugging via Console Access:**
```csharp
// If mod implements console commands
public override void Entry(IModHelper helper)
{
    helper.ConsoleCommands.Register(
        "jat_dump_state",
        "Dump current JAT state to log",
        DumpState);
}

private void DumpState(string cmd, string[] args)
{
    var snapshot = _stateStore.GetCurrentSnapshot();
    
    _logger.Log("=== JAT State Dump ===");
    _logger.Log($"Current Day: {snapshot.Day}");
    _logger.Log($"Rules: {snapshot.Rules.Count}");
    foreach (var rule in snapshot.Rules)
        _logger.Log($"  - {rule.Id}: {rule.Name}");
    
    _logger.Log($"Tasks: {snapshot.Tasks.Count}");
    foreach (var task in snapshot.Tasks.Take(10))
        _logger.Log($"  - {task.TaskId}: {task.Title}");
    
    if (snapshot.Tasks.Count > 10)
        _logger.Log($"  ... and {snapshot.Tasks.Count - 10} more");
}
```

**Game State Snapshot Capture (In Debugger):**
```csharp
// Breakpoint in OnDayStarted; inspect:
_logger.Log($"Game.Player.Name: {Game1.player.Name}");
_logger.Log($"CurrentLocation: {Game1.currentLocation.Name}");
_logger.Log($"DaysElapsed: {Game1.stats.DaysElapsed}");

// Take snapshot of expected state
var characters = Game1.getAllCharacters();
_logger.Log($"Characters in game: {string.Join(", ", characters.Select(c => c.Name))}");
```

## 6. MonitorProxy for Structured Diagnostics ##

**Using SMAPI Monitor Directly (if IModLogger unavailable):**
```csharp
public class DiagnosticsLogger
{
    private readonly IMonitor _monitor;  // From SMAPI

    public DiagnosticsLogger(IMonitor monitor)
    {
        _monitor = monitor;
    }

    public void LogDiagnostics()
    {
        _monitor.Log("=== Diagnostics ===", LogLevel.Info);
        
        _monitor.Log($"Memory: {GC.GetTotalMemory(false) / 1024 / 1024} MB", 
                     LogLevel.Debug);
        
        _monitor.Log($"Active threads: {Process.GetCurrentProcess().Threads.Count}", 
                     LogLevel.Debug);
        
        // Check game states
        var isSaveLoaded = Game1.hasLoadedGame;
        _monitor.Log($"Save loaded: {isSaveLoaded}", LogLevel.Info);
    }
}
```

## 7. Performance Profiling ##

**Measuring Execution Time (Simple):**
```csharp
public void RegenerateTasksWithTiming()
{
    var stopwatch = Stopwatch.StartNew();

    var tasks = _generator.GenerateTasks();

    stopwatch.Stop();
    _logger.Log($"Task generation took {stopwatch.ElapsedMilliseconds}ms " +
               $"for {tasks.Count} tasks");
}

// Expected output:
// Task generation took 15ms for 137 tasks
```

**Profiling Lifecycle Phases:**
```csharp
public async Task InitializeAsync()
{
    _logger.Log("=== Initialization Timing ===");

    var phase1 = Stopwatch.StartNew();
    var config = await _configLoader.LoadAsync();
    phase1.Stop();
    _logger.Log($"  Config load: {phase1.ElapsedMilliseconds}ms");

    var phase2 = Stopwatch.StartNew();
    var persistence = await _persistence.LoadAsync();
    phase2.Stop();
    _logger.Log($"  Persistence load: {phase2.ElapsedMilliseconds}ms");

    var phase3 = Stopwatch.StartNew();
    var reconstructed = await _reconstructor.ReconstructAsync(persistence);
    phase3.Stop();
    _logger.Log($"  Reconstruction: {phase3.ElapsedMilliseconds}ms");

    var total = phase1.Elapsed + phase2.Elapsed + phase3.Elapsed;
    _logger.Log($"Total initialization: {total.TotalMilliseconds:F1}ms");
}

// Expected output:
// === Initialization Timing ===
//   Config load: 2ms
//   Persistence load: 5ms
//   Reconstruction: 18ms
// Total initialization: 25.0ms
```

**Detecting Performance Regressions:**
- Log timing metrics each run
- If phase consistently > 100ms, investigate
- Common bottlenecks: file I/O, large iterations, reflection

## 8. Reproducing Bugs with Minimal Test Cases ##

**"Crash on Specific Day" Bug:**
```
Player reports: "Game crashes when day 15 starts"

Steps to reproduce:
1. Start new game
2. Skip to Summer (using console: skip_to 1 1)
3. Save and reload
4. Observe crash on day 15

Diagnosis:
- Check DayStarted event in log: is it reached on day 15?
- Add logging to identify which generator fails
- Compare day 14 state vs day 15 (what changed?)
```

**Minimal Test Case (Unit Test Simulation):**
```csharp
[Test]
public void DayStarted_OnDay15_DoesNotCrash_OrThrow()
{
    // Arrange: Simulate loaded game at day 14
    var state = new ApplicationState(
        Day: DayKey.Parse("Summer-14"),
        Rules: LoadedRules
    );
    _stateStore.LoadSnapshot(state);

    // Act: Trigger day transition
    var nextDay = DayKey.Parse("Summer-15");
    
    Assert.DoesNotThrow(() =>
    {
        _lifecycle.OnDayStarted(new GameDayStartedEvent(nextDay));
    });

    // Assert: State updated, no crash
    Assert.That(_stateStore.GetCurrentSnapshot().Day, Is.EqualTo(nextDay));
}
```

**Capture Reproduction Save File:**
- When user reports bug, have them provide save file
- Copy save to local machine; test with exact same state
- Narrows down to specific data pattern issue

## Links ##
- [External Resources Instructions](../Instructions/external-resources.instructions.md)
- [Backend Architecture Contract](../Contracts/BACKEND-ARCHITECTURE-CONTRACT.instructions.md)
