---
name: anti-slop-enforcer
description: >-
  Blocks vague naming (Manager, Helper, Utils), generic comments,
  over-engineering, and scope creep. Enforces crisp, focused implementations.
trigger: before-edit
applyTo: "**/*.cs"
---

# Anti-Slop Enforcer Hook #

    * Trigger: before editing C# files.
    * Purpose: block vague naming, over-engineering, and scope drift before edits are generated.

## Pre-Edit Checks ##

Run these checks before proposing any C# edit.

### 1. Naming Slop Detection ###

Block vague type names that do not describe responsibility.

Blocked examples:

    public class TaskManager { }
    public class TaskHelper { }
    public class TaskUtils { }
    public class TaskService { }

Approved examples:

    public class StateStore { }
    public class TaskGenerator { }
    public class CommandDispatcher { }
    public class PersistenceWriter { }

Decision rule:

    * If the name clearly states role, allow.
    * If the name is generic suffix-only vocabulary, block and require a specific name.

### 2. Over-Engineering Detection ###

Block abstraction layers without current need.

Common block conditions:

    * Added interfaces with one implementation and no near-term variation need.
    * Pattern-heavy indirection without a concrete requirement.
    * Hypothetical extensibility with no active user story.

Guidance:

    * Prefer concrete implementation first.
    * Add abstractions only when multiple implementations or hard test seams require it.

### 3. Scope Creep Detection ###

Validate edit scope against user request.

Block when:

    * Single-file request becomes multi-file change.
    * No-behavior-change request introduces features.
    * Bug-fix request expands into unrelated refactors.

Scope violation output pattern:

    SCOPE CREEP DETECTED
    Requested scope: [scope]
    Proposed change: [change]
    Out-of-scope edits: [list]
    Action: reduce to requested scope or ask permission

### 4. Comment Slop Detection ###

Block low-value comments.

Blocked examples:

    // Gets the task
    public Task GetTask() { }

    // TODO: Fix this later

Allowed examples:

    // Snapshot is cloned to keep UI-facing data immutable.
    public Snapshot GetSnapshot() => canonicalSnapshot.Clone();

    // TODO(alcar, 2026-03-15): Extract when second generator type is added.

Rule:

    * Comments must explain why, constraints, or justified exceptions.
    * Remove comments that only restate obvious code behavior.

### 5. Performance Slop Detection ###

In hot paths, block avoidable allocation and unbounded work patterns.

Block examples:

    void OnUpdateTicked()
    {
        var active = allTasks.Where(t => t.IsActive).ToList();
    }

Checks:

    * Avoid per-frame LINQ allocations where loops are clearer and cheaper.
    * Avoid unbounded scans in frequent update paths.
    * Avoid repeated string concatenation in loops.

### 6. Magic Number Detection ###

Block unexplained numeric literals in domain logic.

Blocked:

    if (tasks.Count > 42) { }
    Task.Delay(5000);

Preferred:

    private const int MaxConcurrentTasks = 42;
    private const int PollingIntervalMs = 5000;

## Final Pre-Edit Checklist ##

    * [ ] No vague naming suffixes without explicit role meaning.
    * [ ] No unnecessary abstraction layers.
    * [ ] No out-of-scope edits.
    * [ ] No low-value comments.
    * [ ] No hot-path allocation slop.
    * [ ] No unexplained magic numbers.

## Output Behavior ##

    * Silent when all checks pass.
    * Emit blocking output when any slop rule fails.

Blocking output template:

    ANTI-SLOP VIOLATION DETECTED
    Rule: [rule]
    Pattern: [snippet]
    Reason: [why]
    Required fix: [action]

Do not continue edit generation until violations are resolved.

## Integration ##

Works with:

    * `contract-auto-loader` for C# style contract context.
    * `handoff-optimizer` to keep delegated scope precise.
    * `state-mutation-guard` and `ui-boundary-enforcer` for boundary-critical checks.
