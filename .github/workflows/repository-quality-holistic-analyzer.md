---
name: repository-quality-holistic-analyzer
description: "Daily holistic analysis of repository quality from multiple dimensions: architecture, patterns, type safety, test coverage, and conventions."
on:
  schedule: daily
  workflow_dispatch:
permissions:
  contents: read
  issues: read
  pull-requests: read
strict: true
network:
  allowed: [defaults, github]
engine:
  id: copilot
tools:
  github:
    toolsets: [default]
safe-outputs:
  noop:
    report-as-issue: false
  create-issue:
    title-prefix: "[quality] "
    labels: [agentic-workflow, quality, needs-review]
---

# Repository Quality Holistic Analyzer

Rotating daily analysis of repository quality from different architectural and quality dimensions.

This workflow consolidates the following overlapping daily analyses:
- Repository-wide holistic quality (architecture, lifecycle, state, docs, conventions)
- C# type safety improvements (nullable reference types, generics, type consistency)
- Unit test coverage gaps and test quality

## Rotation Schedule

Analyzes one focus area per day, cycling through:

1. **Domain Architecture** — Identifier usage, aggregate design, boundary clarity
2. **Lifecycle Coordination** — Signal propagation, event ordering, determinism
3. **State Management** — Command patterns, persistence contracts, determinism
4. **Testing Infrastructure & Coverage** — Test coverage gaps, determinism verification, architecture boundaries, test quality patterns
5. **Documentation Alignment** — Design guides match implementation, contracts are current
6. **Code Consistency & Type Safety** — Naming conventions, style compliance, pattern adherence, nullable reference types, untyped generics, type consistency
7. **Configuration Safety** — Config migration integrity, version handling, immutability

## Analysis Process

1. **Select focus area** from 7-day rotation (cache-based)
2. **Deep dive analysis:**
   - Scan relevant code subsystem
   - Identify patterns and anomalies
   - Cross-reference with design guides
   - Score improvement opportunities

3. **Compile recommendations:**
   - What's working well (positive note)
   - What needs improvement
   - Concrete suggestions (don't implement)

4. **Create issue** with findings

## Focus Area Details

### Day 4: Testing Infrastructure & Coverage

Analyze the xUnit test suite for coverage gaps, test quality patterns, and determinism verification.

**Context:**
- Test project: `Tests/JojaAutoTasks.Tests.csproj`
- Framework: xUnit + Moq
- Test conventions: `Tests/README.md` — contains focused test commands and naming guidance
- `InternalsVisibleTo` in `JojaAutoTasks.csproj` enables direct tests of internal domain types

**Analysis Goals:**
1. **Test Coverage Gaps:** Public APIs in `Domain/` without corresponding tests; new code paths added in recent commits; internal types used by tests (thanks to `InternalsVisibleTo`)
2. **Test Quality:** Tests following determinism contracts (no random data, fixed seeds); proper use of Moq for isolation; clear AAA (Arrange-Act-Assert) structure
3. **Architecture Boundary Testing:** Tests verifying Lifecycle/Event dispatcher contracts; domain identifier determinism validation; configuration migration safety tests

**Output:** Create issues for concrete coverage gaps with test code examples. Title: `[quality] Testing: Add test coverage for <scenario>`. Include current gap, expected behavior, and suggested assertions.

### Day 6: Code Consistency & Type Safety

Analyze C# code for naming, style compliance, and opportunities to improve type safety.

**Context:**
- Nullable reference types: enabled in `.csproj`
- Target frameworks: `net6.0` (mod), `net8.0` (tests)

**Analysis Goals:**
1. **Nullable Reference Type Improvements:** Variables declared as `object?` where specific types are known; parameters accepting `null` where guards exist; missing `!` suppression operators with justification
2. **Generic Type Constraints:** Unspecialized generics (e.g., `List<object>` where specific type applies); missing `where` constraints on generic parameters; unnecessary `dynamic` usage
3. **Type Consistency:** Inconsistent type naming for the same concept across files; domain identifiers (`TaskId`, `RuleId`, `SubjectId`) using correct types; proper use of value types vs. reference types
4. **Naming & Style:** Adherence to `CSHARP-STYLE-CONTRACT.instructions.md`; identifier naming, acronym casing, forbidden suffixes

**Output:** Create issues grouped by category (nullable refs, generics, consistency, naming). Title: `[quality] Code: <category> — <finding>`.

## Output

- Post one issue per focus area per day (7 issues/week maximum)
- Title: `[quality] <FocusArea>: <Primary Finding>`
- Include:
  - Summary of current state
  - Strengths and areas for improvement
  - Specific examples from codebase
  - Actionable recommendations

## Notes

- Findings posted as issues for tracking and resolution
- Focus on systemic quality, not individual bug fixes
- Reference relevant architecture contracts in findings
- Days 4 and 6 now also cover unit test coverage and type safety analysis (previously handled by separate daily workflows)
