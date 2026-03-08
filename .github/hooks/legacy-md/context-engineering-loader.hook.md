---
name: context-engineering-loader
description: >-
  Loads codebase structure principles before code generation to ensure agents understand
  file organization, naming conventions, and colocated file patterns.
trigger: before-generation
applyTo: "**/*.{cs,sml,json,md}"
---

# Context Engineering Loader Hook #

**Trigger:** Before code generation or refactoring.  
**Purpose:** Ensure agents understand and respect the JAT codebase structure and organization principles.

## Scope and Applicability ##

This hook activates when:

- Agent is generating new code (`**/*.cs`, `**/*.sml`, `**/*.json`)
- Agent is refactoring existing code
- Agent is creating architectural guidance
- Intent indicates file/folder reorganization

## Pre-Generation Context Load ##

**MANDATORY**: Load [`context-engineering.instructions.md`](../Instructions/context-engineering.instructions.md) when:

1. **Creating new files or modules:**
   - Agent should understand folder structure conventions
   - Agent should know colocations (tests, types, implementations)
   - Agent should understand file naming conventions for discoverability

2. **Refactoring or reorganizing code:**
   - Agent should preserve structure principles
   - Agent should not create "junk drawer" folders (`Misc`, `Utils`, `Helpers`)
   - Agent should colocate related files

3. **Generating API or module guidance:**
   - Agent should export public contracts from index files
   - Agent should explain folder boundaries
   - Agent should guide on semantic naming over abbreviations

## Loading Procedure ##

1. When edit/generation intent is detected, read [`context-engineering.instructions.md`](../Instructions/context-engineering.instructions.md).
2. Apply structure principles to proposed code layout **before** generating files.
3. If proposing reorganization, reference folder intent and colocations.
4. Verify proposed file paths follow naming and structure conventions.

## Conflict Resolution ##

If context engineering principles conflict with architecture contracts:

1. Check [`BACKEND-ARCHITECTURE-CONTRACT`](../Instructions/backend-architecture-contract.instructions.md) for subsystem boundaries (takes precedence).
2. Check [`CSHARP-STYLE-CONTRACT`](../Instructions/csharp-style-contract.instructions.md) for file/folder rules (takes precedence).
3. Context engineering principles apply when no direct conflict exists.

