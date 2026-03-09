---
name: csharp-type-safety-analyzer
description: "Daily analyzer for improving C# type safety: nullable reference types, untyped generics, and type consistency."
on:
  schedule: daily
  workflow_dispatch:
permissions:
  contents: read
strict: true
network:
  allowed: [defaults, github]
engine:
  id: copilot
tools:
  github:
    toolsets: [default]
safe-outputs:
  create-discussion:
    category: general
    title-prefix: "[type-safety] "
---

# C# Type Safety Analyzer

Analyze C# code for opportunities to improve type safety through nullable reference types, generic constraints, and type consistency.

## Context

- Repository: `${{ github.repository }}`
- Default branch: `${{ github.event.repository.default_branch }}`
- Target frameworks: net6.0 (mod), net8.0 (tests)
- Nullable reference types: enabled in `.csproj`

## Analysis Focus

1. **Nullable Reference Type Improvements:**
   - Variables declared as `object?` where specific types are known
   - Parameters accepting `null` where guards exist
   - Missing `!` suppression operators with justification

2. **Generic Type Constraints:**
   - Unspecialized generics (e.g., `List<object>` where specific type applies)
   - Missing `where` constraints on generic parameters
   - Unnecessary `dynamic` usage

3. **Type Consistency:**
   - Inconsistent type naming for the same concept across files
   - Domain identifiers (TaskId, RuleId, SubjectId) using correct types
   - Proper use of value types vs. reference types

## Process

1. **Scan all .cs files** for nullable and type patterns
2. **Analyze recent changes** to domain and core modules
3. **Identify improvement opportunities** without breaking changes
4. **Create discussion** with recommendations (not implementations)
5. **Post weekly summary** of patterns found

## Output

- Create 1 discussion per week with type-safety findings
- Group findings by category (nullable refs, generics, consistency)
- Include code examples and suggested improvements
- Reference nullable reference type best practices from C# docs
