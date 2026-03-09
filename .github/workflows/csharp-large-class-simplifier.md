---
name: csharp-large-class-simplifier
description: "Monitors C# class file sizes and proposes splitting oversized classes into focused domain modules."
on:
  schedule: weekly
  workflow_dispatch:
permissions:
  issues: read
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
  create-issue:
    title-prefix: "[refactor] "
    labels: [agentic-workflow, refactoring, csharp, needs-review]
    close-older-issues: true
    max: 1
---

# C# Large Class Simplifier

Analyze C# class file sizes and identify opportunities to split oversized classes into focused implementations.

## Context

- Repository: `${{ github.repository }}`
- Default branch: `${{ github.event.repository.default_branch }}`
- Analysis focus: `.cs` files in Domain/, Lifecycle/, StateStore/, Configuration/ directories
- Healthy class size threshold: ~300 lines per class

## Analysis Process

1. **Discover all C# classes:**
   - Scan all `.cs` files in source directories (exclude Tests/)
   - Extract class names and file locations
   - Calculate lines per class (count { to } blocks)

2. **Identify size outliers:**
   - Flag classes exceeding 300 lines as candidates
   - Determine if class has mixed responsibilities (multiple public methods serving different concerns)
   - Use semantic analysis to identify logical groupings

3. **Propose refactoring:**
   - Identify natural module boundaries based on domain concepts
   - Suggest splitting large classes into focused implementations
   - Prioritize the largest offender (one issue per run)

4. **Issue creation:**
   - Title: `[refactor] <FileName>: Split into multiple focused classes`
   - Include: Current size, proposed structure, responsibility groupings
   - Reference relevant Domain/ architecture patterns

## Output Requirements

- Create at most 1 issue per run (targeting the largest class)
- Skip creation if open `[refactor]` issue already exists
- Close existing `[refactor]` issues if the class they target has been split
- Title format: `[refactor] <ClassName>: Split <N> responsibilities into separate classes`
- Body sections:
  - `## Current Structure` — Class name, file, current size in lines
  - `## Responsibility Analysis` — Identified concerns/responsibilities
  - `## Proposed Structure` — How to split into N focused classes
  - `## Rationale` — Why splitting improves the codebase
  - `## References` — Links to Domain architecture guidelines
