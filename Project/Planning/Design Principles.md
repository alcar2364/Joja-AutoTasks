# Joja AutoTasks — Design Principles

## Purpose

This document defines the core design principles for **Joja AutoTasks (JAT)**, a SMAPI mod for Stardew Valley. These principles guide all architectural decisions, implementation work, and code reviews across the project lifecycle.

Principles are organized into two categories:

1. **Adapted Game Development Principles** — foundational software engineering practices adapted specifically for JAT's context as a deterministic task management mod
2. **JAT-Specific Architectural Principles** — principles derived from JAT's unique technical contracts and design constraints

**Target audience:** Developers, AI agents, code reviewers, and contributors working on JAT.

**Last Updated:** 2026-03-08

---

## Adapted Game Development Principles

### Principle 1: Predictable Naming Conventions

**Original Principle:** Pick a naming convention and stick to it. Names should be predictable; if you can't type a name from memory, rename it.

**JAT Adaptation:**

Identifiers must follow strict C# naming conventions as defined in [CSHARP-STYLE-CONTRACT.instructions.md](../../.github/instructions/csharp-style-contract.instructions.md).

**Hard Rules:**

    - **PascalCase** for types, methods, properties, events
        * `TaskStore`, `EvaluateRules()`, `ActiveTasks`, `SnapshotPublished`
    - **camelCase** for parameters and local variables
        * `taskId`, `ruleEvaluator`, `completedTasks`
    - **Interface prefix `I`** for all interfaces
        * `ITaskStore`, `IEventDispatcher`, `IStateSnapshot`
    - **One type per file**, file name matches type name exactly
        * `TaskStore.cs` contains `TaskStore`
        * `ITaskStore.cs` contains `ITaskStore`

**Why This Matters in JAT:**

JAT's architecture relies on deterministic identifiers (`TaskId`, `RuleId`, `DayKey`, `SubjectId`) that must remain stable across save/load cycles. Predictable naming ensures:

    - Task identifiers are derived from stable, recognizable sources
    - UI binding property paths are type-safe and discoverable
    - Dependency injection constructor parameters clearly communicate intent
    - Code review can verify correct identifier usage at a glance

**Examples:**

```csharp
// ✅ GOOD: Predictable, follows conventions
public class TaskStore : ITaskStore
{
    public IStateSnapshot CurrentSnapshot { get; private set; }
    public void ApplyCommand(ITaskCommand command) { ... }
}

// ❌ BAD: Unpredictable, violates conventions
public class task_store : ITaskStore
{
    public IStateSnapshot cur_snap { get; private set; }
    public void apply_cmd(ITaskCommand cmd) { ... }
}
```

**Related Contracts:**

    - [CSHARP-STYLE-CONTRACT.instructions.md](../../.github/instructions/csharp-style-contract.instructions.md) (Section 4)
    - [Design Guide Section 03 - Deterministic Identifier Model](Joja%20AutoTasks%20Design%20Guide/Section%2003%20-%20Deterministic%20Identifier%20Model.md)

---

### Principle 2: Subsystem Boundaries and Modularity

**Original Principle:** Compartmentalize your game. Make code modular, avoid tight coupling, use hierarchies.

**JAT Adaptation:**

JAT architecture is organized into **canonical subsystems** with explicit boundaries and responsibilities. All subsystem dependencies are wired via **constructor injection**.

**Hard Rules:**

JAT defines five core backend subsystems (per [BACKEND-ARCHITECTURE-CONTRACT](../../.github/instructions/backend-architecture-contract.instructions.md)):

1. **Evaluation Engine** — Evaluates rules and generators to produce candidate tasks
2. **State Store** — Single source of truth; owns canonical task state
3. **Persistence Layer** — Saves/loads minimal essential data with versioning
4. **Daily Snapshot Ledger** — Maintains task history by day key
5. **Dependency Wiring** — Constructor injection only; no service locators or globals

**Subsystem Contract:**

    - Each subsystem declares dependencies via **constructor parameters** only
    - Subsystems must **not** mutate state outside their responsibility
    - Cross-subsystem communication uses **commands** (State Store) or **events** (EventDispatcher)
    - **No circular dependencies** between subsystems

**Why This Matters in JAT:**

Modular boundaries enable:

    - **Testing:** Each subsystem can be unit tested in isolation with mocks
    - **Determinism:** Clear ownership prevents accidental state mutation
    - **Visibility:** Constructor injection makes dependencies explicit and auditable
    - **Maintainability:** Changes to one subsystem don't cascade unexpectedly

**Examples:**

```csharp
// ✅ GOOD: Explicit constructor injection
public class TaskStore : ITaskStore
{
    private readonly IEventDispatcher _eventDispatcher;
    private readonly ITaskEvaluator _evaluator;

    public TaskStore(IEventDispatcher eventDispatcher, ITaskEvaluator evaluator)
    {
        _eventDispatcher = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
        _evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
    }
}

// ❌ BAD: Hidden dependencies via service locator
public class TaskStore : ITaskStore
{
    public void ApplyCommand(ITaskCommand command)
    {
        var dispatcher = ServiceLocator.Get<IEventDispatcher>(); // Hidden dependency
        dispatcher.Dispatch(new TaskUpdatedEvent());
    }
}
```

**Related Contracts:**

    - [BACKEND-ARCHITECTURE-CONTRACT.instructions.md](../../.github/instructions/backend-architecture-contract.instructions.md) (Sections 1, 2)
    - [Design Guide Section 02 - System Architecture](Joja%20AutoTasks%20Design%20Guide/Section%2002%20-%20System%20Architecture.md) (Section 2.4)

---

### Principle 3: Centralized Control Flow

**Original Principle:** Don't spread code across multiple objects. Keep logic centralized in controllers.

**JAT Adaptation:**

All canonical state mutations flow through a **single path**: Commands → Reducers → State Store. The State Store is the **sole owner** of canonical task state.

**Hard Rules:**

    - **UI must not mutate** canonical state
    - **Evaluation Engine must not mutate** canonical state
    - **All state changes** occur via commands processed by the State Store
    - **Reducers** implement deterministic transformations (pure functions)
    - **Snapshots** published by State Store are **read-only** and immutable

**Command Flow:**

```text
UI Interaction → Command Dispatch → State Store → Reducer → New State → Snapshot Published → UI Refresh
```

**Why This Matters in JAT:**

Centralized control ensures:

    - **Determinism:** Same inputs always produce same outputs
    - **Auditability:** All mutations traceable to commands
    - **Testability:** Reducers are pure functions easily tested in isolation
    - **History tracking:** Command log can support undo/redo or replay (future)

**Examples:**

```csharp
// ✅ GOOD: UI dispatches command
public void OnCompleteTaskButtonClicked(string taskId)
{
    var command = new CompleteTaskCommand(taskId);
    _taskStore.ApplyCommand(command);
}

// ❌ BAD: UI mutates state directly
public void OnCompleteTaskButtonClicked(string taskId)
{
    var task = _taskStore.CurrentSnapshot.Tasks.First(t => t.Id == taskId);
    task.Status = TaskStatus.Completed; // Direct mutation!
}
```

**Related Contracts:**

    - [BACKEND-ARCHITECTURE-CONTRACT.instructions.md](../../.github/instructions/backend-architecture-contract.instructions.md) (Section 2)
    - [FRONTEND-ARCHITECTURE-CONTRACT.instructions.md](../../.github/instructions/frontend-architecture-contract.instructions.md) (Section 2)

---

### Principle 4: Organized Planning and Backlog Management

**Original Principle:** Keep a todo list for all ideas. Organize by time commitment, with session notes.

**JAT Adaptation:**

JAT uses **Implementation Plans** stored in `Project/Tasks/Implementation Plan/` as flat Markdown files. All planning artifacts follow a structured format with phases, atomic commits, and verification criteria.

**Hard Rules:**

    - Implementation plans stored as **flat `.md` files** (no nested subfolders)
    - Phase checklists use `Phase N - Description.md` naming for natural sort order
    - Each phase is broken into **atomic commits** with clear verification steps
    - Session progress tracked in comments or dedicated session log files

**Why This Matters in JAT:**

JAT is a complex mod with determinism requirements, versioned persistence, and SMAPI integration constraints. Organized planning ensures:

    - **Incremental progress:** Atomic commits allow safe iteration
    - **Verification gates:** Each commit can be tested before proceeding
    - **Scope containment:** Clear phase boundaries prevent feature creep
    - **Shared context:** Plans serve as documentation for future work

**Examples:**

    - [Phase 1 - Atomic Commit Execution Checklist.md](Phase%201%20-%20Atomic%20Commit%20Execution%20Checklist.md)
    - [Phase 2 - Atomic Commit Execution Checklist.md](Phase%202%20-%20Atomic%20Commit%20Execution%20Checklist.md)

**Related Contracts:**

    - [WORKSPACE-CONTRACTS.instructions.md](../../.github/instructions/workspace-contracts.instructions.md) (Section 2)
    - [Implementation Plan README.md](README.md)

---

### Principle 5: Scope Discipline and Feature Containment

**Original Principle:** Do NOT give in to feature slip. Decide scope early, stick to it, 3x time estimates.

**JAT Adaptation:**

JAT enforces **strict scope discipline** through multi-phase implementation plans and explicit confirmation gates for scope-expanding changes.

**Hard Rules:**

    - **Plan-first policy:** Complex tasks require Research → Plan → Implementation → Review
    - **Explicit approval required** for:
        * Multi-file edits
        * Renaming public types or APIs
        * Changing persisted data formats
        * Deleting files
        * Modifying build/config files
        * Adding new dependencies
    - **No silent scope expansion:** If solving an issue requires additional changes outside requested

scope, **stop and ask permission**

**Why This Matters in JAT:**

JAT's determinism contract and versioned persistence make scope creep dangerous:

    - **Breaking changes** to identifiers or persistence formats require migrations
    - **Cascading edits** across subsystems risk introducing non-deterministic behavior
    - **Unplanned dependencies** can violate SMAPI compatibility or licensing constraints

**Examples:**

```text
✅ GOOD workflow:
User: "Fix task completion bug in HUD"
Agent: [Reviews code, identifies root cause in State Store reducer]
Agent: "The bug is in TaskStore.ApplyCommand, but fixing it requires changing
        the reducer signature in ITaskStore. This affects 3 files. Proceed?"
User: "Yes, proceed."
Agent: [Makes changes]

❌ BAD workflow:
User: "Fix task completion bug in HUD"
Agent: [Silently refactors State Store, Persistence, and Evaluation Engine]
Agent: "Done! Also improved the architecture while I was there."
```

**Related Contracts:**

    - [WORKSPACE-CONTRACTS.instructions.md](../../.github/instructions/workspace-contracts.instructions.md) (Sections 2-4)
    - [REVIEW-AND-VERIFICATION-CONTRACT.instructions.md](../../.github/instructions/review-and-verification-contract.instructions.md)

---

### Principle 6: Visual Documentation of System Design

**Original Principle:** Create physical documents about what objects do. Visual representations of variables and functions.

**JAT Adaptation:**

JAT maintains a comprehensive **Design Guide** with section-based organization. Each section covers a specific subsystem or design concern with clear diagrams, data structures, and flow descriptions.

**Hard Rules:**

    - Design Guide sections stored in `Project/Planning/Joja AutoTasks Design Guide/`
    - Sections numbered sequentially: `Section NN - Title.md`
    - Each section includes:
        * **Purpose statement**
        * **Data structures** with field descriptions
        * **Flow diagrams** (text-based or Mermaid)
        * **Examples** showing correct usage
        * **Anti-patterns** showing what to avoid
    - Main index: `JojaAutoTasks Design Guide.md`

**Why This Matters in JAT:**

JAT's architecture is non-trivial:

    - **State flow** crosses multiple subsystems (Engine → State Store → Snapshot → UI)
    - **Identifier derivation** requires understanding determinism contracts
    - **Persistence versioning** needs explicit migration strategy documentation

Visual documentation ensures contributors and reviewers can verify correctness without deep code archaeology.

**Examples:**

    - [Section 02 - System Architecture.md](Joja%20AutoTasks%20Design%20Guide/Section%2002%20-%20System%20Architecture.md)
    - [Section 03 - Deterministic Identifier Model.md](Joja%20AutoTasks%20Design%20Guide/Section%2003%20-%20Deterministic%20Identifier%20Model.md)
    - [Section 04 - Core Data Model.md](Joja%20AutoTasks%20Design%20Guide/Section%2004%20-%20Core%20Data%20Model.md)

**Related Contracts:**

    - [Design Guide EditingInstructions.md](Joja%20AutoTasks%20Design%20Guide/EditingInstructions.md)

---

### Principle 7: Concise, Semantic Identifiers

**Original Principle:** Give everything a name. Short (6 letters max), consistent, noun-only identifiers.

**JAT Adaptation:**

JAT **balances brevity with clarity**. While the original principle favors 6-letter max identifiers, JAT prioritizes **self-documenting names** that communicate intent without requiring comments.

**Hard Rules:**

    - **Prefer clarity over brevity** — `TaskEvaluator` is better than `TskEvl`
    - **Nouns for types** — `TaskStore`, `RuleEvaluator`, `StateSnapshot`
    - **Verbs for methods** — `EvaluateRules()`, `ApplyCommand()`, `PublishSnapshot()`
    - **Avoid abbreviations** unless widely recognized in domain:
        * ✅ `HUD` (game UI term), `ID` (standard)
        * ❌ `TskBldr`, `EvlEng`, `SnapPub`
    - **Domain-specific terms** preferred over generic:
        * ✅ `TaskId` (JAT domain, C# identifier form)
        * ❌ `Identifier`, `Key`, `UniqueString`

**Why This Matters in JAT:**

JAT code serves as **living documentation**. Self-explanatory names reduce cognitive load and allow reviewers to verify correctness without constant context switching to documentation.

**Examples:**

```csharp
// ✅ GOOD: Self-documenting names
public class TaskEvaluator : ITaskEvaluator
{
    public IEnumerable<TaskCandidate> EvaluateRules(IEnumerable<TaskRule> rules, GameState gameState)
    {
        // Intent is immediately clear
    }
}

// ❌ BAD: Overly abbreviated
public class TskEvl : ITskEvl
{
    public IEnumerable<TskCand> EvlRls(IEnumerable<TskRl> rls, GmSt gs)
    {
        // Requires mental translation
    }
}
```

**Related Contracts:**

    - [CSHARP-STYLE-CONTRACT.instructions.md](../../.github/instructions/csharp-style-contract.instructions.md) (Section 4)
    - [SELF-EXPLANATORY-CODE-COMMENTING.instructions.md](../../.github/instructions/self-explanatory-code-commenting.instructions.md)

---

### Principle 8: Design Before Implementation

**Original Principle:** Know in advance the best way to build your code. Experiment first, avoid mid-implementation rewrites.

**JAT Adaptation:**

JAT requires **upfront architectural planning** due to determinism constraints, SMAPI integration boundaries, and versioned persistence. Mid-implementation pivots risk breaking saves or introducing non-deterministic behavior.

**Hard Rules:**

    - **Plan-first for complex features:** New subsystems, persistence format changes, and identifier

changes require Design Guide sections before implementation - **Prototype risky integrations** before committing to architecture: _ SMAPI event lifecycle interactions _ StardewUI binding patterns \* Game state query performance - **Design Guide updates before breaking changes:** If architecture must change, update Design Guide first, then implement - **Avoid mid-flight architecture pivots:** If a fundamental flaw is discovered, pause implementation, update plan, get approval, then resume

**Why This Matters in JAT:**

JAT's constraints make course corrections expensive:

    - **Identifier changes** break task history and persistence
    - **State Store refactors** cascade to UI, persistence, and evaluation engine
    - **Persistence format changes** require explicit migrations with version bumps
    - **SMAPI lifecycle changes** affect when evaluation runs and snapshot publishing

**Examples:**

**✅ GOOD workflow:**

1. Write Design Guide section for new "Task Tagging" feature
2. Define tag persistence format with version bump strategy
3. Design tag-based filtering in Evaluation Engine
4. Prototype UI for tag assignment and filtering
5. Review design with stakeholders
6. Implement per atomic commit plan

**❌ BAD workflow:**

1. Start implementing "Task Tagging" directly in UI
2. Realize tags need persistence halfway through
3. Add tags to persistence without version bump
4. Realize deterministic tag IDs needed
5. Refactor mid-implementation
6. Break existing saves

**Related Contracts:**

    - [Design Guide Sections 01-21](Joja%20AutoTasks%20Design%20Guide/JojaAutoTasks%20Design%20Guide.md)
    - [WORKSPACE-CONTRACTS.instructions.md](../../.github/instructions/workspace-contracts.instructions.md) (Section 2.1)

---

### Principle 9: Finish Before Starting New Work

**Original Principle:** Don't abandon your project midway. Maintain discipline, finish before starting new projects.

**JAT Adaptation:**

JAT enforces **phase-based completion discipline**. Each phase must reach a **verified working state** before proceeding to the next phase.

**Hard Rules:**

    - **Atomic commits must be verifiable:** Each commit includes verification steps (build passes,

tests pass, determinism checks) - **No half-implemented features in main branch:** Use feature branches for work-in-progress - **Phase completion gates:** _ All atomic commits in phase completed _ All verification steps passed _ No known blockers or regressions _ Phase retrospective documented (optional but recommended) - **No scope expansion mid-phase:** New feature ideas captured in backlog, not inserted into active phase

**Why This Matters in JAT:**

JAT's determinism contract means **partially implemented features can corrupt saves** or introduce subtle bugs that only manifest across save/load cycles. Incremental, verified progress is mandatory.

**Examples:**

**✅ GOOD progression:**

    - Phase 1: Foundation (Domain models, identifiers) → **Verified working, tests pass**
    - Phase 2: State Store + Commands → **Verified working, determinism checks pass**
    - Phase 3: Persistence + Versioning → **Verified working, migration tests pass**

**❌ BAD progression:**

    - Phase 1: Half-implemented State Store
    - Phase 2: Started working on UI while State Store has bugs
    - Phase 3: Realized identifier model was wrong, refactored everything

**Related Contracts:**

    - [Phase 1 - Atomic Commit Execution Checklist.md](Phase%201%20-%20Atomic%20Commit%20Execution%20Checklist.md)
    - [Phase 2 - Atomic Commit Execution Checklist.md](Phase%202%20-%20Atomic%20Commit%20Execution%20Checklist.md)
    - [UNIT-TESTING-CONTRACT.instructions.md](../../.github/instructions/unit-testing-contract.instructions.md)

---

## JAT-Specific Architectural Principles

The following principles are derived from JAT's unique technical contracts and design constraints.

---

### Principle 10: Determinism is Non-Negotiable

**Definition:**

All identifier generation, task evaluation, and state transformations must be **deterministic** — producing identical outputs for identical inputs, regardless of evaluation order, timing, or session.

**Hard Rules:**

    - **TaskId must be deterministic:**
        * Derived from stable inputs: source type, rule ID, subject ID, day key
        * **Never** use `Guid.NewGuid()`, random numbers, or time-based UUIDs for generated tasks
        * TaskIds must remain stable across reloads and evaluation passes
    - **Reducer functions must be pure:**
        * Same command + same state → same new state
        * No side effects (logging is permissible, state mutation is not)
    - **Evaluation order must not affect results:**
        * Rule evaluation must not depend on traversal order of unordered collections
        * If ordering is semantically relevant, use **explicit sorting** with stable keys
    - **Persistence reconstruction must be deterministic:**
        * Loading saved data must produce identical state given identical file contents

**Why This Matters:**

Non-deterministic behavior causes:

    - **Duplicate tasks** on reload (different IDs for same logical task)
    - **Lost progress** (completed task not recognized after reload)
    - **Flaky tests** (identifier mismatch in unit tests)
    - **User frustration** (tasks appear/disappear unpredictably)

**Examples:**

```csharp
// ✅ GOOD: Deterministic ID from stable inputs
public static TaskId ForBuiltInTask(string generatorName, SubjectID subjectId)
{
    return new TaskId($"BuiltIn:{generatorName}:{subjectId}");
}

// ❌ BAD: Non-deterministic GUID
public static TaskId ForBuiltInTask(string generatorName, SubjectID subjectId)
{
    return new TaskId(Guid.NewGuid().ToString()); // Different every time!
}
```

**Verification:**

    - Unit tests must verify **identifier stability** across multiple calls
    - Round-trip persistence tests must verify **exact state reconstruction**

**Related Contracts:**

    - [BACKEND-ARCHITECTURE-CONTRACT.instructions.md](../../.github/instructions/backend-architecture-contract.instructions.md) (Section 3)
    - [Design Guide Section 03 - Deterministic Identifier Model](Joja%20AutoTasks%20Design%20Guide/Section%2003%20-%20Deterministic%20Identifier%20Model.md)
    - [UNIT-TESTING-CONTRACT.instructions.md](../../.github/instructions/unit-testing-contract.instructions.md)

---

### Principle 11: Immutability Boundaries are Inviolable

**Definition:**

**Snapshots** published by the State Store are **read-only**. UI and other consumers must **never** mutate snapshot data.

**Hard Rules:**

    - State Store publishes **immutable snapshots** to UI
    - UI **reads** from snapshots but **never writes**
    - All state changes flow through **Command → Reducer → State Store** path
    - Snapshot properties should use **read-only collections** where possible:
        * `IReadOnlyList<T>`, `IReadOnlyDictionary<K,V>`, `ImmutableList<T>` preferred over mutable
    collections

**Why This Matters:**

Violating immutability boundaries causes:

    - **Invisible state mutations** (UI changes state without going through reducers)
    - **Lost determinism** (mutations bypass command log)
    - **Concurrency bugs** (snapshot shared across threads)
    - **Untestable behavior** (mutations not captured in reducer tests)

**Examples:**

```csharp
// ✅ GOOD: UI reads snapshot, dispatches command for changes
public void OnCompleteTaskClicked(string taskId)
{
    _taskStore.ApplyCommand(new CompleteTaskCommand(taskId));
}

public void RenderTaskList()
{
    var snapshot = _taskStore.CurrentSnapshot;
    foreach (var task in snapshot.ActiveTasks) // Read-only iteration
    {
        DrawTaskRow(task);
    }
}

// ❌ BAD: UI mutates snapshot directly
public void OnCompleteTaskClicked(string taskId)
{
    var snapshot = _taskStore.CurrentSnapshot;
    var task = snapshot.ActiveTasks.First(t => t.Id == taskId);
    task.Status = TaskStatus.Completed; // Direct mutation!
}
```

**Related Contracts:**

    - [FRONTEND-ARCHITECTURE-CONTRACT.instructions.md](../../.github/instructions/frontend-architecture-contract.instructions.md) (Section 2)
    - [BACKEND-ARCHITECTURE-CONTRACT.instructions.md](../../.github/instructions/backend-architecture-contract.instructions.md) (Section 2.2)

---

### Principle 12: Performance Through Design, Not Restrictions

**Definition:**

JAT achieves performance through **architectural design** (caching, bounded evaluation, event- driven updates) rather than feature restrictions.

**Hard Rules:**

    - **Avoid per-frame work:** Expensive computations (rule evaluation, game state queries) run on

**events** or **throttled ticks**, not every frame - **Cache expensive lookups:** Game state snapshots cached for evaluation window - **Bounded scans:** Avoid unbounded loops; use explicit limits or early exits - **UI rebuild throttling:** UI refreshes only on snapshot changes, not continuously - **Lazy evaluation:** Defer expensive computations until actually needed

**Why This Matters:**

SMAPI mods share the game's main thread. Heavy per-frame work causes:

    - **Frame drops** and stuttering gameplay
    - **Slow UI responsiveness**
    - **Player frustration** and mod uninstalls

JAT's "unlimited tasks" philosophy requires performance discipline at the architectural level.

**Examples:**

```csharp
// ✅ GOOD: Rule evaluation on day start event, cached game state snapshot
public void OnDayStarted(object sender, DayStartedEventArgs e)
{
    var gameStateSnapshot = CaptureGameState(); // One-time snapshot
    var candidateTasks = _ruleEvaluator.EvaluateRules(_rules, gameStateSnapshot);
    _taskStore.ApplyCommand(new ReconcileTasksCommand(candidateTasks));
}

// ❌ BAD: Rule evaluation every frame
public void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
{
    var candidateTasks = _ruleEvaluator.EvaluateRules(_rules, CaptureGameState());
    _taskStore.ApplyCommand(new ReconcileTasksCommand(candidateTasks));
    // This will destroy performance!
}
```

**Performance Guardrails:**

    - **HUD rendering:** Must remain lightweight; cache layout calculations
    - **Menu initialization:** Allowed to be slower; runs once on menu open
    - **Task evaluation:** Throttled to sensible intervals (day start, periodic tick with cooldown)

**Related Contracts:**

    - [FRONTEND-ARCHITECTURE-CONTRACT.instructions.md](../../.github/instructions/frontend-architecture-contract.instructions.md) (Section 5)
    - [PERFORMANCE-OPTIMIZATION.instructions.md](../../.github/instructions/performance-optimization.instructions.md)
    - [Design Guide Section 19 - Performance Guardrails](Joja%20AutoTasks%20Design%20Guide/Section%2019%20-%20Performance%20Guardrails.md)

---

### Principle 13: Minimal Persistence, Explicit Versioning

**Definition:**

Persist **only the minimal essential data** required to reconstruct state. All persisted data must include **explicit version identifiers** to support safe migrations.

**Hard Rules:**

    - **Do not persist transient caches** or UI state (scroll positions, selection state)
    - **Do not persist derived data** that can be recomputed (task counts, completion percentages)
    - **Do persist:**
        * Task completion state and user actions
        * Task Builder rules and definitions
        * Manual task content
        * Daily snapshot ledger entries
        * Monotonic counters (manual task ID counter)
    - **Version all persisted structures:**
        * Include `Version` field in root of saved data
        * Bump version on schema changes
        * Implement explicit migration paths for version upgrades
    - **Safe failure modes:**
        * Unknown versions should fail loudly with clear error messages
        * Corrupted data should not crash the mod; fall back to safe defaults

**Why This Matters:**

Over-persistence causes:

    - **Save bloat** (large files, slow load times)
    - **Migration complexity** (more fields = more migration paths)
    - **Fragility** (transient state persisted incorrectly breaks reload)

Under-versioning causes:

    - **Silent data corruption** when schema changes
    - **Lost user data** when migration fails
    - **Impossible rollbacks** (can't detect version mismatches)

**Examples:**

```csharp
// ✅ GOOD: Minimal persistence with versioning
public class SaveData
{
    public int Version { get; set; } = 1;
    public Dictionary<string, TaskCompletionState> CompletedTasks { get; set; }
    public List<TaskBuilderRule> UserRules { get; set; }
    public int ManualTaskCounter { get; set; }
}

// ❌ BAD: Over-persistence, no versioning
public class SaveData
{
    // No version field!
    public List<TaskObject> AllTasks { get; set; } // Includes generated tasks!
    public int CurrentScrollPosition { get; set; } // UI state!
    public Dictionary<string, int> TaskCountCache { get; set; } // Derived data!
}
```

**Related Contracts:**

    - [BACKEND-ARCHITECTURE-CONTRACT.instructions.md](../../.github/instructions/backend-architecture-contract.instructions.md) (Section 1.3)
    - [Design Guide Section 09 - Persistence Model](Joja%20AutoTasks%20Design%20Guide/Section%2009%20-%20Persistence%20Model.md)
    - [Design Guide Section 18 - Versioning and Migration Strategy](Joja%20AutoTasks%20Design%20Guide/Section%2018%20-%20Versioning%20and%20Migration%20Strategy.md)

---

### Principle 14: Comment Why, Not What

**Definition:**

Code should be **self-explanatory** through clear naming and structure. Comments explain **why** decisions were made, not **what** the code does.

**Hard Rules:**

    - **Do not comment obvious code:**

```csharp
// ❌ BAD
counter++; // Increment counter
```

    - **Do comment non-obvious decisions:**

```csharp
// ✅ GOOD
// SMAPI event order: DayStarted fires before TimeChanged,
// so we must evaluate rules here to catch new daily tasks
private void OnDayStarted(object sender, DayStartedEventArgs e)
{
    EvaluateAllRules();
}
```

    - **Do comment complex algorithms:**

```csharp
// ✅ GOOD
  // TaskId must include DayKey for daily recurring tasks to ensure
// a new task instance is created each day while remaining deterministic
  var taskId = $"TaskBuilder:{ruleId}:{subjectId}:{dayKey}";
```

    - **Do comment performance-critical code:**

```csharp
// ✅ GOOD
// Cache game state snapshot to avoid per-rule queries
// (reduces N rule evaluations from N*M queries to 1 snapshot + N evaluations)
var gameStateSnapshot = CaptureGameState();
foreach (var rule in rules)
{
    EvaluateRule(rule, gameStateSnapshot);
}
```

**Why This Matters:**

Good comments explain **intent and context** that isn't obvious from code alone:

    - **SMAPI lifecycle constraints**
    - **Performance optimizations**
    - **Determinism requirements**
    - **Edge case handling**

Bad comments clutter code and become outdated when code changes.

**Related Contracts:**

    - [SELF-EXPLANATORY-CODE-COMMENTING.instructions.md](../../.github/instructions/self-explanatory-code-commenting.instructions.md)

---

### Principle 15: Explicit Dependencies via Constructor Injection

**Definition:**

All subsystems declare dependencies **explicitly** via constructor parameters. No service locators, no ambient globals.

**Hard Rules:**

    - **Constructor injection only** for subsystem dependencies
    - **Validate parameters** with `ArgumentNullException` checks
    - **Composition root** (BootstrapContainer) wires dependencies at startup
    - **No hidden dependencies** via static singletons or service locators

**Why This Matters:**

Explicit dependencies enable:

    - **Testability:** Mock dependencies in unit tests
    - **Visibility:** Dependencies clear from constructor signature
    - **Compile-time safety:** Missing dependencies caught at composition, not runtime

**Examples:**

```csharp
// ✅ GOOD: Explicit constructor injection
public class TaskStore : ITaskStore
{
    private readonly IEventDispatcher _eventDispatcher;
    private readonly ITaskEvaluator _evaluator;

    public TaskStore(IEventDispatcher eventDispatcher, ITaskEvaluator evaluator)
    {
        _eventDispatcher = eventDispatcher ?? throw new ArgumentNullException(nameof(eventDispatcher));
        _evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
    }
}

// ❌ BAD: Hidden service locator dependency
public class TaskStore : ITaskStore
{
    public void PublishSnapshot()
    {
        var dispatcher = ServiceLocator.Get<IEventDispatcher>(); // Hidden!
        dispatcher.Publish(new SnapshotUpdatedEvent());
    }
}
```

**Related Contracts:**

    - [BACKEND-ARCHITECTURE-CONTRACT.instructions.md](../../.github/instructions/backend-architecture-contract.instructions.md) (Section 1.5)
    - [Design Guide Section 02 - System Architecture](Joja%20AutoTasks%20Design%20Guide/Section%2002%20-%20System%20Architecture.md) (Section 2.4)

---

## Principle Application Workflow

When implementing a new feature or reviewing code, apply these principles in order:

1. **Check naming** (Principle 1) — Are identifiers predictable and follow conventions?
2. **Verify boundaries** (Principle 2) — Does code respect subsystem boundaries?
3. **Trace control flow** (Principle 3) — Do mutations go through Command → Reducer → State?
4. **Review scope** (Principle 5) — Is this the minimal change for the requested scope?
5. **Validate determinism** (Principle 10) — Are identifiers and evaluation deterministic?
6. **Check immutability** (Principle 11) — Does UI respect snapshot read-only contract?
7. **Assess performance** (Principle 12) — Is expensive work event-driven, not per-frame?
8. **Verify persistence** (Principle 13) — Is minimal data persisted with explicit versioning?
9. **Audit dependencies** (Principle 15) — Are dependencies explicit via constructor injection?

If any principle is violated, **stop and fix** before proceeding.

---

## Risks and Concerns Identified

During adaptation of these principles to JAT context, the following risks were identified:

1. **Tension between determinism and flexibility:**
   - JAT's determinism requirements can conflict with rapid prototyping
   - **Mitigation:** Design Guide sections and upfront planning reduce mid-flight pivots

2. **Performance vs. feature richness:**
   - "Unlimited tasks" philosophy requires careful performance discipline
   - **Mitigation:** Event-driven architecture and caching prevent per-frame work

3. **Persistence migration complexity:**
   - Explicit versioning is critical but adds upfront design overhead
   - **Mitigation:** Version from day 1; incremental migrations easier than retroactive fixes

4. **Learning curve for contributors:**
   - JAT's principles are more stringent than typical SMAPI mods
   - **Mitigation:** This document + Design Guide + contracts provide clear onboarding path

---

## Document Maintenance

This document should be updated when:

    - New architectural patterns emerge that warrant principle-level guidance
    - Existing principles prove insufficient or incorrect in practice
    - Contract files are updated with new hard rules
    - Design Guide introduces new subsystems with unique constraints

**Review Frequency:** After each major phase completion.

---

## References

### Architecture Contracts

    - [BACKEND-ARCHITECTURE-CONTRACT.instructions.md](../../.github/instructions/backend-architecture-contract.instructions.md)
    - [FRONTEND-ARCHITECTURE-CONTRACT.instructions.md](../../.github/instructions/frontend-architecture-contract.instructions.md)
    - [WORKSPACE-CONTRACTS.instructions.md](../../.github/instructions/workspace-contracts.instructions.md)

### Style Contracts

    - [CSHARP-STYLE-CONTRACT.instructions.md](../../.github/instructions/csharp-style-contract.instructions.md)
    - [SELF-EXPLANATORY-CODE-COMMENTING.instructions.md](../../.github/instructions/self-explanatory-code-commenting.instructions.md)

### Design Guide Sections

    - [JojaAutoTasks Design Guide.md](Joja%20AutoTasks%20Design%20Guide/JojaAutoTasks%20Design%20Guide.md)
    - [Section 02 - System Architecture](Joja%20AutoTasks%20Design%20Guide/Section%2002%20-%20System%20Architecture.md)
    - [Section 03 - Deterministic Identifier Model](Joja%20AutoTasks%20Design%20Guide/Section%2003%20-%20Deterministic%20Identifier%20Model.md)
    - [Section 09 - Persistence Model](Joja%20AutoTasks%20Design%20Guide/Section%2009%20-%20Persistence%20Model.md)

### Testing Contracts

    - [UNIT-TESTING-CONTRACT.instructions.md](../../.github/instructions/unit-testing-contract.instructions.md)

---

**End of Document**
