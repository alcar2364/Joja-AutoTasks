---
name: state-mutation-guard
description: >-
  Prevents agents from violating State Store mutation boundaries. Blocks direct
  state mutation; enforces command -> reducer -> snapshot flow.
trigger: before-edit
applyTo: "{Domain,Events,Infrastructure,UI,Integrations,Lifecycle}/**/*.cs"
---

# State Mutation Guard Hook #

    * Trigger: before C# edits in core and UI-related folders.
    * Purpose: enforce canonical state mutation boundaries.

## Pre-Edit Guard ##

Before generating edits in matching paths, detect whether proposed changes attempt direct mutation
of canonical state.

Block immediately when edits include:

    * Direct task state property mutation in non-store paths.
    * Direct modification of State Store owned collections.
    * UI code mutating canonical state or snapshots in place.
    * Bypassing command dispatch and reducer flow.

## Legal Mutation Path ##

Only this mutation flow is allowed:

    Intent or interaction
    -> command object
    -> dispatch to State Store
    -> reducer applies deterministic state transition
    -> new snapshot published
    -> UI refresh from snapshot

## Block Response Template ##

    STATE MUTATION BOUNDARY VIOLATION DETECTED
    Subsystem: [subsystem]
    Blocked operation: [operation]
    Required path: command -> dispatch -> reducer -> snapshot
    Suggested command: [command name]

Do not proceed with edit generation until a command-based approach is accepted.

## Allowed Exceptions ##

Direct mutation is allowed only for non-canonical local state:

    * UI-local presentation state such as sorting, expansion, or scroll.
    * Explicitly non-canonical transient caches.
    * Local debug or diagnostic variables.
    * Test fixture setup inside test files.

## Escalation Rule ##

If canonical versus local status is unclear:

    * Treat as potentially canonical.
    * Flag uncertainty.
    * Request explicit clarification before proceeding.
