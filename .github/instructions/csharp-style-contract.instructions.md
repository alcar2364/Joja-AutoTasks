---
name: CSHARP-STYLE-CONTRACT
description: "C# coding style, naming, formatting, and file structure rules for JAT. Use when: editing .cs files."
---

# CSHARP-STYLE-CONTRACT.instructions.md #

## Purpose ##

This contract defines **C# coding style + file/folder conventions** for **JAT (Joja AutoTasks)**.

It also defines **edit behavior rules** agents MUST follow when changing C# files.

This contract applies to:
    - `*.cs`

This contract does **not** define:
    - architecture rules (see [`BACKEND-ARCHITECTURE-CONTRACT.instructions.md`](BACKEND-ARCHITECTURE-CONTRACT.instructions.md), [`FRONTEND-ARCHITECTURE-CONTRACT.instructions.md`](FRONTEND-ARCHITECTURE-CONTRACT.instructions.md))
    - verification/testing rules (see [`REVIEW-AND-VERIFICATION-CONTRACT.instructions.md`](REVIEW-AND-VERIFICATION-CONTRACT.instructions.md), [`UNIT-TESTING-CONTRACT.instructions.md`](UNIT-TESTING-CONTRACT.instructions.md))
    - workspace/user interaction rules (see [`WORKSPACE-CONTRACTS.instructions.md`](WORKSPACE-CONTRACTS.instructions.md))

## 1. Canonical reference ##

For identifier naming, agents MUST follow the official .NET C# identifier naming guidance
**strictly**:
    - <https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/identifier-names>

Where this contract adds specificity (e.g., acronym handling), it should remain consistent with the
above guidance.

## 2. Agent edit behavior (C# files) ##

### 2.1 Scope hygiene ###

    - Agents MUST avoid unrelated drive-by edits.
    - Agents MUST keep changes **minimal and localized** to the requested scope.
    - Agents MUST NOT reformat entire files unless explicitly requested or required by an enforced
    convention in this contract.

### 2.2 Rename/normalization policy ###

    - If an agent is already editing a file, it SHOULD **normalize naming aggressively within that
    file** to match this contract.
    - If normalization would touch many files, the agent MUST propose the broader rename but MUST
    NOT apply it without explicit permission (see [`WORKSPACE-CONTRACTS.instructions.md`](WORKSPACE-CONTRACTS.instructions.md) confirmation gates).

### 2.3 “Style-only” changes ###

    - Style-only refactors MUST be avoided unless:
        * they are necessary to keep the edited code consistent, or
        * the user explicitly requested styling cleanup.

### 2.4 Output expectations when proposing edits ###

When not directly editing code, agents SHOULD provide:
    - a short summary
    - a concrete edit checklist in safe edit order
    - a list of files/symbols impacted

## 3. File & folder structure rules (C#) ##

### 3.1 One top-level type per file ###

    - Default: **one top-level type per file**.
    - Limited exceptions allowed (keep them small and cohesive):
        * tiny, tightly-coupled immutable `record`/`record struct` types that exist only to support
          the primary type
        * enums used exclusively by the primary type
        * delegates used exclusively by the primary type

### 3.2 File naming ###

    - File names MUST match the primary type name exactly:
        * `HudLayout.cs` contains `HudLayout`
        * `TaskState.cs` contains `TaskState`
    - Interfaces follow the same rule:
        * `ITaskStore.cs` contains `ITaskStore`

### 3.3 Folder naming and intent ###

    - Folders MUST be named for responsibilities/subsystems (not vague buckets).
    - Avoid “junk drawers” like `Misc`, `Stuff`, `Random`, `New`, `Old`.

## 4. Identifier naming conventions (C#) ##

### 4.1 Standard naming ###

Agents MUST follow the official conventions (reference link above), including:
    - **PascalCase** for namespaces, types, methods, properties, events
    - **camelCase** for parameters and local variables
    - interface prefix `I` (e.g., `ITaskStore`)
    - clear, descriptive names over abbreviations

### 4.2 Acronym / abbreviation conversion rules (JAT-specific) ###

**All abbreviations follow standard PascalCase/camelCase rules with no special treatment.**

Treat abbreviations and acronyms as normal word chunks for case conversion purposes.

#### 4.2.1 PascalCase acronyms (types/methods/properties) ####

Abbreviations in PascalCase identifiers follow **normal PascalCase capitalization**:

    - ✅ `TaskId` (not `TaskID`)
    - ✅ `ModApi` (not `ModAPI`)
    - ✅ `UiState` (not `UIState`)
    - ✅ `HudLayout` (not `HUDLayout`)
    - ✅ `JsonConfig` (not `JSONConfig`)
    - ✅ `SmartContract` (not `smartcontract`)
    - ✅ `SmapiConfig` (not `SMAPIConfig`)

Each abbreviation is capitalized as the **first letter only**, treating multi-letter abbreviations
as a single word chunk.

#### 4.2.2 camelCase acronyms (locals/parameters/fields) ####

Abbreviations in camelCase identifiers follow **normal camelCase capitalization**:

    - ✅ `taskId` (not `taskID`)
    - ✅ `modApi` (not `modAPI`)
    - ✅ `uiState` (not `UIState`)
    - ✅ `hudLayout` (not `HUDLayout`)
    - ✅ `exampleModApi` (not `exampleModAPI`)
    - ✅ `smapiConfig` (not `SMAPIConfig`)

The abbreviation is treated as a **single word chunk**, with only the leading letter lowercase
when it starts the identifier, or only the first letter capitalized when it follows another word
chunk.

## 5. Modifiers and accessibility ##

### 5.1 Be restrictive by default ###

    - Agents MUST choose the most restrictive access level that satisfies usage.
    - Prefer `internal` unless public API is intended.

### 5.2 Prefer sealed ###

    - Concrete classes SHOULD be `sealed` unless inheritance is explicitly intended.

### 5.3 Prefer readonly where correct ###

    - Fields SHOULD be `readonly` when not reassigned.
    - Structs SHOULD be `readonly` when they are immutable.

### 5.4 `var` usage ###

    - `var` is preferred when the type is obvious from the right-hand side.
    - Use explicit types when it improves clarity.

### 5.5 Nullability ###

    - Agents MUST write explicit null-handling when null is possible.
    - Avoid null-forgiving (`!`) unless justified and localized.

## 6. Formatting rules ##

### 6.1 Braces ###

    - Allman braces are required.
    - Single-line conditionals **without braces** are forbidden.

### 6.2 Expression-bodied members ###

    - Allowed for trivial properties/methods only.

### 6.3 Modern C# syntax ###

    - Pattern matching and modern C# syntax SHOULD be used when it improves clarity and reduces
    bugs.

### 6.4 LINQ ###

    - LINQ is allowed when it is clear and allocation cost is acceptable.
    - Prefer loops in hot paths or performance-sensitive contexts.

### 6.5 Control flow ###

    - Prefer early returns and guard clauses over deep nesting.
    - Guard blocks MUST be grouped together before executing the things they guard.
    - Ternaries are fine for simple assignments; nested ternaries are forbidden.

## 7. Comments and documentation ##

All commenting rules are defined in [`self-explanatory-code-commenting.instructions.md`](self-explanatory-code-commenting.instructions.md).

During code review, comments MUST be validated against those guidelines.

## 8. Member ordering (required) ##

Within a type, members MUST be ordered as follows:

1. Nested types
2. Constants
3. Static fields (`static readonly`)
4. Instance fields (`readonly` then mutable)
5. Properties
6. Constructor(s)
7. Public/internal methods
8. Event handlers (near lifecycle/public methods)
9. Private methods

Fields MUST appear before properties.

## 9. Naming vocabulary restrictions (anti-entropy rules) ##

The following vague suffixes are **banned** in type names unless explicitly justified:

    - `Manager`
    - `Helper`
    - `Utils`
    - `Data` (as a vague suffix)

`Controller` is allowed **only** when it truly controls something.

The term `Reducer` is banned. Use a clearer name for the responsibility (examples):
    - `StateController`
    - `StateApplier`
    - `StateMutator`
    - `CommandHandler`
    - `TransitionApplier`

Use `Service` and `Coordinator` only when they actually provide a service or coordinate other
components.
