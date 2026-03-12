---
name: self-explanatory-code-commenting
description: "Guidelines for writing self-explanatory comments in JAT, with C#-first examples and language-specific carveouts for .sml and .json files."
applyTo: "**/*.cs"
---

# Self-explanatory Code Commenting Instructions

## Core Principle
Write code that speaks for itself. Comment only when necessary to explain WHY, not WHAT.
Most code should be readable without extra comments.

## JAT Language Scope and Precedence

This instruction defines all commenting rules for JAT across all languages.

1. For C# files (`.cs`), this file defines all required comment structures and styling.
2. For StarML files (`.sml`), follow [`sml-style-contract.instructions.md`](sml-style-contract.instructions.md).
3. For JSON files (`.json`), comments are not supported; document intent via schema keys and docs files.

## Commenting Guidelines

### AVOID These Comment Types

**Obvious Comments**
```csharp
int retryCount = 0; // Set retry count to zero.
retryCount++; // Increment retry count.
```

**Redundant Comments**
```csharp
string GetUserName(User user)
{
    return user.Name; // Return the user's name.
}
```

**Outdated Comments**
```csharp
// Apply a 5% tax rate.
var tax = price * 0.08m;
```

### WRITE These Comment Types

**Complex Business Logic**
```csharp
// Recurrence rules are evaluated against the target in-game day, not DateTime.Now,
// so save/load and replay remain deterministic.
var isActive = recurrenceEvaluator.IsActive(rule, dayKey);
```

**Non-obvious Algorithms**
```csharp
// Sort by canonical TaskId before snapshot projection so ordering remains stable
// even if upstream collections come from hash-based containers.
var orderedTasks = tasks.OrderBy(static task => task.TaskId.Value, StringComparer.Ordinal);
```

**Regex Patterns**
```csharp
// Matches "Year1-Summer15" style day keys.
var dayKeyPattern = new Regex(@"^Year\d+\-[A-Za-z]+\d+$", RegexOptions.Compiled);
```

**API Constraints or Gotchas**
```csharp
// SMAPI update-tick handlers fire frequently; throttle expensive work to avoid
// per-tick evaluation overhead.
if (!updateTickGuard.ShouldRun(e.IsMultipleOf))
{
    return;
}
```

## Decision Framework

Before writing a comment, ask:

1. Is the code self-explanatory? If yes, do not add a comment.
2. Would a better type/member/variable name remove the need for a comment? Refactor first.
3. Does the comment explain intent, invariants, or constraints instead of narrating syntax?
4. Will this comment still be true after likely refactors?

## Special Cases for Comments

### Public and Internal C# Types

Use XML docs for public and internal C# types (see C# Structural Comment Requirements section above).

```csharp
/// <summary>
/// Coordinates lifecycle signal forwarding from SMAPI hooks to the event dispatcher.
/// </summary>
internal sealed class LifecycleCoordinator
{
}
```

### Configuration and Constants
```csharp
// Leave 1 second of buffer under the upstream timeout to avoid partial writes.
private const int ApiTimeoutMilliseconds = 14_000;
```

### Annotations
```csharp
// TODO: Replace hard-coded default with configuration once GMCM binding is complete.
// FIXME: Preserve completion state when rule subject aliases are migrated.
// HACK: Temporary shim for legacy TaskId parsing in v0 save files.
// NOTE: This branch intentionally treats missing snapshots as "no history".
// WARNING: This mutates the provided list for allocation-free reconciliation.
// PERF: Avoid LINQ here; this method runs in a hot update path.
// SECURITY: Validate external identifier input before persistence.
// BUG: Empty subject key can still bypass this guard in legacy config.
// REFACTOR: Split parser and validator once migration window closes.
// DEPRECATED: Use ParseCanonicalDayKey instead; remove after v2 migration support ends.
```

## C# Structural Comment Requirements

### XML docs (required scope)

    - XML doc comments (`///`) are required for **all public + internal types**.
    - For members, use XML docs when tooling/consumers benefit; otherwise prefer normal comments.

### XML `<summary>` style

    - `<summary>` MUST describe the type's purpose/role or member's responsibility.
    - `<summary>` text SHOULD be one concise sentence in present tense ending with a period.
    - Short summaries SHOULD use single-line form:
        * `/// <summary>Coordinates lifecycle signal forwarding.</summary>`
    - Multi-line summaries MUST use canonical three-line form:
        * `/// <summary>`
        * `/// Coordinates lifecycle signal forwarding.`
        * `/// </summary>`
    - Avoid boilerplate like "This class..." when it adds no useful context.

### Private method comments

Private methods MUST include a comment when:
    - behavior is non-obvious, OR
    - the method has many parameters.

When required, private-method comments SHOULD explain intent/constraints, not restate syntax.

### Guard clause comments

    - Guard clauses SHOULD be marked with `// -- Guards -- //` section header.
    - Guard blocks MUST be grouped together before executing the things they guard.

Example:
```csharp
// -- Guards -- //
if (task == null)
{
    throw new ArgumentNullException(nameof(task));
}

if (string.IsNullOrEmpty(task.Name))
{
    return;
}

// Main logic starts here
var result = ProcessTask(task);
```

### Section headers inside types

Section headers are required only when the section contains code.

Section headers MUST be:
    - formatted as `// -- [Title] -- //`
    - title case phrases
    - placed immediately before the first member in that section

Use these headers (when each section exists), in member-order sequence:
    - `// -- Dependencies -- //`
    - `// -- Constants -- //`
    - `// -- State -- //`
    - `// -- Constructor -- //`
    - `// -- Public API -- //`
    - `// -- Event Handlers -- //`
    - `// -- Private Helpers -- //`

(Event handlers should be grouped near lifecycle/public methods.)

## Anti-Patterns to Avoid

### Dead Code Comments
```csharp
// private void LegacyMethod() { ... }
private void CurrentMethod()
{
}
```

### Changelog Comments
```csharp
// Modified by Jane on 2025-01-02.
internal void Process()
{
}
```

### Divider Comments
```csharp
// ===============================
// UTILITY METHODS
// ===============================
```

Do not use decorative divider comments. Use section headers with the required format instead (see C# Structural Comment Requirements section).

## Quality Checklist

Before finalizing comments, ensure they:

- Explain WHY, constraints, invariants, or non-obvious behavior.
- Stay accurate as code evolves.
- Are short, clear, and specific.
- Are placed immediately above the code they clarify.
- Follow language-specific syntax and contract rules.
- Do not duplicate names, signatures, or obvious statements.

## Summary

The best comment is the one you do not need. When comments are required, make them intentional, durable, and constraint-focused.
