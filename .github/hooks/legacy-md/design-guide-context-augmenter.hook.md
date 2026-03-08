---
name: design-guide-context-augmenter
description: >-
  Auto-loads relevant Design Guide sections based on task keywords. Provides deep
  architecture context before implementation.
trigger: before-planning
applyTo: all
---

# Design Guide Context Augmenter Hook #

    * Trigger: before planning or implementation.
    * Purpose: auto-inject Design Guide context relevant to the task.

## Keyword to Section Mapping ##

Load these sections when matching cues appear in task text or target paths:

    * task generation, generator, built-in: Section 05 and Section 13.
    * rule evaluation, Task Builder, rule model: Section 06 and Section 07.
    * State Store, command, reducer, snapshot: Section 08.
    * persistence, save, load, migration: Section 09 and Section 18.
    * UI binding, view model, snapshot-driven: Section 10 and Section 10A.
    * HUD, menu, StarML, StardewUI: Section 20 and Section 10.
    * history, daily snapshot, statistics: Section 11.
    * identifier, TaskID, RuleID, determinism: Section 03.
    * update cycle, event-driven, periodic: Section 12.
    * Task Builder wizard, UX flow: Section 14.
    * configuration, ModConfig: Section 15.
    * error handling, validation: Section 16.
    * debug, diagnostic, dev tools: Section 17.
    * performance, guardrails, optimization: Section 19.

## Loading Procedure ##

1. Detect keywords in user request, handoff text, and likely file paths.
2. Map cues to sections and include all relevant sections for multi-subsystem work.
3. Load sections with `read_file` and prioritize sections closest to planned changes.
4. For broad tasks, load Section 02 first, then targeted sections.
5. Apply loaded rules as architectural constraints during planning and implementation.

## Automatic Loading Rules ##

Always load without asking when these areas are in scope:

    * Section 02 for cross-subsystem tasks.
    * Section 03 for deterministic identifier work.
    * Section 08 for canonical state mutation work.
    * Section 10 for UI and backend interaction work.

Load other sections only when directly mentioned or clearly implied.

## Handoff Context Expectations ##

When Planner or Researcher hands off implementation work, include concise citations.

Example handoff payload:

    Context loaded:
    * Design Guide Section 08: State Store uses command/reducer flow.
    * Design Guide Section 03: Identifier formulas must be deterministic.

    Key constraints:
    * Canonical state changes must flow through command dispatch.
    * Identifier generation must avoid random and wall-clock sources.

## Caching and Reuse ##

    * Cache loaded sections within the same conversation.
    * Reuse cached sections instead of re-reading them.
    * Load only missing sections needed for the current task.

## Output Behavior ##

    * Silent by default.
    * Emit output only for conflicts, hard-rule clarifications, or approach changes.

When reporting a conflict, cite the exact Design Guide section and provide a compliant alternative.

## Integration ##

Works with:

    * `contract-auto-loader` for file-type contract loading.
    * `state-mutation-guard` for command-based mutation enforcement.
    * `identifier-validation` for deterministic identifier checks.
