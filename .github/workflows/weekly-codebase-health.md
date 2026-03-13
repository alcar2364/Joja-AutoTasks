---
name: weekly-codebase-health
description: "Weekly analyzer for C# class structure, NuGet dependency health, and deterministic pattern safety."
on:
  schedule: weekly
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
    title-prefix: "[codebase-health] "
    labels: [agentic-workflow, implementation-issue, issue-type: architecture-concern, priority: medium, codebase-health]
    close-older-issues: true
    max: 3
---

# Weekly Codebase Health — Class Structure, Dependencies, and Determinism

Weekly analysis of three closely related codebase health dimensions: class decomposition, NuGet
dependency freshness, and non-deterministic pattern safety.

This workflow consolidates the responsibilities of the former:
- `csharp-large-class-simplifier` (weekly) — class size and structure
- `dotnet-dependency-analyzer` (weekly) — NuGet package health
- `determinism-regression-detector` (weekly schedule) — forbidden non-deterministic pattern scan

The PR-triggered determinism scan has been merged into `architecture-contract-compliance`.

## Context

- Repository: `${{ github.repository }}`
- Default branch: `${{ github.event.repository.default_branch }}`
- Project files: `JojaAutoTasks.csproj`, `Tests/JojaAutoTasks.Tests.csproj`
- Source directories: `Domain/`, `StateStore/`, `Lifecycle/`, `Configuration/`, `Startup/`, `Events/`
- Excluded from determinism scan: `Tests/` (test code may use random/GUID legitimately)
- Class size threshold: ~300 lines per class
- Core dependency: `Pathoschild.Stardew.ModBuildConfig 4.4.0`

## Analysis 1: Class Structure

### Process

1. **Discover all C# classes:** Scan all `.cs` files in source directories (exclude `Tests/`); extract class names, file locations, line counts.
2. **Identify size outliers:** Flag classes exceeding 300 lines; determine if class has mixed responsibilities.
3. **Propose refactoring:** Identify natural module boundaries; suggest splitting large classes into focused implementations; prioritize the largest offender.

### Output

Create at most 1 implementation issue per run (targeting the largest class). Close or archive older codebase-health implementation issues if the previously flagged class has been split or the concern is no longer valid.

- Title: `[codebase-health] Refactor: <ClassName> — Split <N> responsibilities into separate classes`
- Body sections:
  - `## Current Structure` — Class name, file, current size in lines
  - `## Responsibility Analysis` — Identified concerns/responsibilities
  - `## Proposed Structure` — How to split into N focused classes
  - `## Rationale` — Why splitting improves the codebase
  - `## References` — Links to Domain architecture guidelines

## Analysis 2: NuGet Dependency Health

### Process

1. **Extract dependencies** from `.csproj` files
2. **Analyze versions** for available updates (breaking vs. minor vs. patch)
3. **Check health indicators:** maintenance status, license compatibility, security advisories
4. **Compile recommendations** for review

### Output

- Weekly implementation issue with dependency analysis (grouped by mod project / test project / transitive)
- Include: current version, available updates, recommendations
- Note any security advisories or maintenance concerns
- Title: `[codebase-health] Dependencies: <primary finding>`
- ModBuildConfig is a strategic dependency; note alignment with SMAPI version

## Analysis 3: Determinism Pattern Scan (Full Weekly)

Protect JAT's first-class determinism requirement by scanning all production source files for
forbidden patterns.

**Reference:** `Project/Planning/Joja AutoTasks Design Guide/Section 03 - Deterministic Identifier Model.md`

### Forbidden Patterns (Production Code Only)

**Pattern 1 — Random GUID:** `Guid.NewGuid()` — Task IDs must be derived from stable inputs.

**Pattern 2 — Unseeded Randomness:** `new Random()` or `new Random(Environment.TickCount)` — non-deterministic seed corrupts task state stability.

**Pattern 3 — Wall-Clock Time in ID/Ordering:** `DateTime.Now`, `DateTime.UtcNow`, `DateTimeOffset.Now`, `Environment.TickCount`, `Stopwatch.GetTimestamp()` — acceptable only in logging/measurements; flag all for manual review.

**Pattern 4 — Unordered Collection Traversal:** `foreach.*Dictionary` or `foreach.*HashSet` followed immediately by a sort or projection without an explicit stable key.

**Pattern 5 — Environment-Dependent Identifiers:** `Environment.MachineName`, `Environment.UserName`, `Environment.GetEnvironmentVariable(` — differ between machines, breaking the deterministic ID contract.

### Severity Classification

- Patterns 1 and 2 in `Domain/` or `StateStore/`: **Blocker**
- Patterns 1 and 2 in other directories: **Major**
- Pattern 3: **Major** (manual review required)
- Patterns 4 and 5: **Minor** (advisory)

### Output

Create an implementation issue if any patterns are found in the full scan:

- Title: `[codebase-health] Determinism: Non-deterministic patterns found`
- Summary of patterns found per directory
- If no patterns found: skip issue (silence is success)

## Summary Output Format

Each analysis produces at most one implementation issue. Total maximum: 3 issues per weekly run.

If none of the three analyses find issues: no output for that analysis category.

## Implementation Issues Mapping

- Large-class structural findings: `Architecture concern`
- Dependency freshness / maintainability findings: `Open issue` unless they indicate architectural risk
- Determinism violations: `Architecture concern` or `Review follow-up` depending on severity and certainty
- Prefer filling the Implementation Issues body sections so the issue can sync cleanly into local records
