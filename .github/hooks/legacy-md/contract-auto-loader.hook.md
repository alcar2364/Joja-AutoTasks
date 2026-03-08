---
name: contract-auto-loader
description: >-
  Intelligently loads relevant style and architecture contracts based on file
  context before code generation. Prevents contract violations.
trigger: before-edit
applyTo: "**/*.{cs,sml,json}"
---

# Contract Auto-Loader Hook #

    * Trigger: before editing `.cs`, `.sml`, or `.json` files.
    * Purpose: load the right contracts into context before edits are generated.

## File Path Contract Mapping ##

Load these contracts by default for matching paths:

1. `{Domain,Events,Infrastructure,Integrations,Lifecycle,Startup,Configuration}/**/*.cs`:
   `BACKEND-ARCHITECTURE-CONTRACT` and `CSHARP-STYLE-CONTRACT` (critical).
2. `UI/**/*.cs`: `FRONTEND-ARCHITECTURE-CONTRACT`, `BACKEND-ARCHITECTURE-CONTRACT`
   for boundaries, and `CSHARP-STYLE-CONTRACT` (critical).
3. `**/*.sml`: `SML-STYLE-CONTRACT`, `FRONTEND-ARCHITECTURE-CONTRACT`,
   and `UI-COMPONENT-PATTERNS` (critical).
4. `Tests/**/*.cs`: `UNIT-TESTING-CONTRACT`, `BACKEND-ARCHITECTURE-CONTRACT`,
   and `CSHARP-STYLE-CONTRACT` (critical).
5. `**/*.json` excluding dependency folders: `JSON-STYLE-CONTRACT` (recommended).

## Intent-Based Extra Loading ##

Add focused references when intent keywords appear:

    * persistence, save, load, migration: backend persistence section.
    * State Store, command, reducer, snapshot: backend mutation-boundary section.
    * HUD, menu, UI, view model: frontend architecture contract.
    * identifier, TaskID, RuleID, SubjectID, DayKey: Design Guide Section 03.
    * test, unit test, spec: unit-testing contract.

## Loading Procedure ##

1. Detect target files and inferred edit intent.
2. Load critical contracts with `read_file` before generating edits.
3. Load recommended contracts when edit complexity or risk warrants it.
4. Cache loaded contracts for the active conversation turn.
5. Reuse cached contracts for multi-file edits to avoid redundant reads.

## Conflict Resolution Order ##

If guidance conflicts, follow this order:

1. Explicit user instruction in the current task.
2. File-specific architecture contracts.
3. Language-specific style contracts.
4. General workspace contracts.

If conflict remains, state it explicitly and follow higher-priority guidance.

## Verification Checkpoint ##

Before presenting edits, verify:

    * [ ] Correct contracts were loaded for all touched files.
    * [ ] Contract rules were applied in generated edits.
    * [ ] No architecture boundary violations were introduced.
    * [ ] Style conventions were preserved in touched regions.

If verification fails, revise the edit before output.

## Output Behavior ##

    * Silent by default.
    * Emit output only when contract conflicts or blocking violations require user attention.
