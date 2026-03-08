---
name: clarity-enforcer
description: >-
  Enforces self-explanatory code practices during code generation and editing,
  reducing unnecessary comments and ensuring code clarity through naming and structure.
trigger: on-generation
applyTo: "**/*.{cs,sml,json}"
---

# Clarity Enforcer Hook #

**Trigger:** During code generation (all file types).  
**Purpose:** Ensure generated code is self-explanatory with minimal, purpose-focused comments.

## Scope and Applicability ##

This hook applies to all code generation, especially:

- New method or class implementations
- Complex algorithms or business logic
- Refactored code with improved structure
- Test naming and organization

## Self-Explanatory Code Enforcement ##

**APPLY BEFORE GENERATION**: Load [`self-explanatory-code-commenting.instructions.md`](../Instructions/self-explanatory-code-commenting.instructions.md).

### 1. Naming-First Approach ##

Before adding comments, ensure:

- **Method names describe what they do**: `CalculateCompoundInterest()` not `Calc()`
- **Variable names are semantic**: `activeAdultUsers` not `x`, `items`
- **Class names describe responsibility**: `TaskEvaluator` not `Processor`
- **Constants have descriptive names**: `MAX_RETRY_ATTEMPTS` not magic number `3`
- **Function parameters have clear types and names**: `CalculateDiscount(decimal orderTotal, decimal itemPrice)`

### 2. Code Structure Before Comments ##

Use clear structure to reduce comment needs:

- **Guard clauses** instead of nested conditionals
- **Early returns** to exit error cases
- **Single responsibility** per method
- **Small methods** (< 20-30 lines) that do one thing well
- **Pattern matching** and clear conditionals in C#

### 3. Comments for Intent Only ##

Add comments ONLY when:

- **Complex business logic** explaining WHY (not WHAT)
- **Non-obvious algorithms** explaining the approach
- **Regular expressions** explaining the pattern
- **External constraints** (API limits, format rules)
- **Tradeoffs** explaining design decisions
- **Configuration values** explaining their source or reasoning

### 4. Comments to AVOID ##

Never commit these:

- Obvious comments (`// Increment counter`)
- Redundant comments (`int x; // assign x`)
- Outdated comments (code changed, comment didn't)
- Commented-out code (use version control)
- Decoration/dividers (`// ===== METHODS =====`)
- Dead code markers without tickets

## Generation Procedure ##

1. **Plan the code** with clarity in mind:
   - Use self-documenting names
   - Simplify structure
   - Remove unnecessary complexity

2. **Generate code** following naming conventions from contracts:
   - [`CSHARP-STYLE-CONTRACT`](../Instructions/csharp-style-contract.instructions.md) for C# files
   - [`SML-STYLE-CONTRACT`](../Instructions/sml-style-contract.instructions.md) for SML files
   - [`JSON-STYLE-CONTRACT`](../Instructions/json-style-contract.instructions.md) for JSON files

3. **Add comments** only for complex intent:
   - Explain business rules or constraints
   - Explain non-obvious algorithms
   - Explain integration points

4. **Minimize new comments**:
   - Let code clarity speak for itself
   - Reserve comments for WHY, not WHAT

## JAT-Specific Clarity Guidelines ##

High-value areas to clarify through naming:

- **State Store operations**: `GetTasksByStateAndCategory()` not `Query()`
- **Event evaluation**: `EvaluateRuleAgainstEvent()` not `Check()`
- **Task transformations**: `ApplyCommandToSnapshot()` not `Process()`
- **Persistence**: `MigrateFromV1ToV2()` not `Upgrade()`
- **Identifiers**: Use full names in types (`DayKey`, `TaskId`, `RuleID`)

## Conflict Resolution ##

If clarity requirements conflict with other instructions:

1. **Code clarity is non-negotiable** for maintainability and correctness.
2. Names and structure take precedence over minimizing comment count.
3. If a comment is necessary for safety, add it (don't sacrifice clarity for dogma).

