---
name: unit-test-coverage-analyzer
description: "Daily analyzer for unit test coverage gaps and test quality improvements."
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
  dotnet: {}
safe-outputs:
  create-issue:
    title-prefix: "[tests] "
    labels: [agentic-workflow, testing, needs-review]
    max: 3
---

# Unit Test Coverage Analyzer

Analyze xUnit test suite for coverage gaps, test quality patterns, and determinism verification.

## Context

- Repository: `${{ github.repository }}`
- Test project: Tests/JojaAutoTasks.Tests.csproj
- Framework: xUnit + Moq
- Current test count: Baseline established from latest run

## Analysis Goals

1. **Test Coverage Gaps:**
   - Public APIs in Domain/ without corresponding tests
   - New code paths added in recent commits
   - Internal types used by tests (thanks to InternalsVisibleTo)

2. **Test Quality:**
   - Tests following determinism contracts (no random data, fixed seeds)
   - Proper use of Moq for isolation
   - Clear AAA (Arrange-Act-Assert) structure

3. **Architecture Boundary Testing:**
   - Tests verifying Lifecycle/Event dispatcher contracts
   - Domain identifier determinism validation
   - Configuration migration safety tests

## Process

1. **Analyze test files** for coverage and quality
2. **Run `dotnet test`** to establish baseline
3. **Identify gaps** in coverage per subsystem
4. **Create issues** for highest-priority gaps (max 3 per run)

## Output Requirements

- Create issues for concrete coverage gaps with test code examples
- Title: `[tests] <Component>: Add test coverage for <scenario>`
- Include: Current gap, expected behavior, suggested assertions
- Reference Tests/README.md for testing conventions

## Notes

- Do not implement tests; propose for review
- Focus on determinism and architecture boundaries
- Reference identity types (TaskId, RuleId, etc.) formats in identity tests
