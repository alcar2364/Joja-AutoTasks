---
name: backend-architecture-contract
description: "Backend architecture rules for JAT: subsystem boundaries, State Store ownership, determinism, event-driven evaluation, persistence. Use when: editing backend/core code."
applyTo: "{Domain,Events,Infrastructure,Integrations,Lifecycle,Startup,Configuration}/**/*.cs"
---

# BACKEND-ARCHITECTURE-CONTRACT.instructions.md #

## Purpose ##

This contract defines **backend architecture rules** for **JAT (Joja AutoTasks)** as specified by
the Joja AutoTasks Design Guide.

Backend = the engine/core logic that owns canonical state, evaluation, persistence, and history.

Agents MUST treat these rules as **hard constraints** when designing, refactoring, or implementing
backend features.

## In scope ##

    - subsystem boundaries and responsibilities (engine, state, persistence, history)
    - canonical data ownership and mutation boundaries
    - determinism requirements (especially identifiers and ordering)
    - engine update cycle rules (event-driven + bounded periodic passes)
    - persistence rules, versioning, and migration behavior
    - reconciliation of generated vs persisted state
    - performance guardrails
    - error handling and safe failure behavior

## Out of scope ##

    - UI layout, rendering, interaction patterns (see [`FRONTEND-ARCHITECTURE-CONTRACT.instructions.md`](FRONTEND-ARCHITECTURE-CONTRACT.instructions.md))
    - coding style and formatting (see [`CSHARP-STYLE-CONTRACT.instructions.md`](CSHARP-STYLE-CONTRACT.instructions.md))
    - code review / verification / testing requirements (see [`REVIEW-AND-VERIFICATION-CONTRACT.instructions.md`](REVIEW-AND-VERIFICATION-CONTRACT.instructions.md), [`UNIT-TESTING-CONTRACT.instructions.md`](UNIT-TESTING-CONTRACT.instructions.md))
    - workspace/user interaction rules (see [`WORKSPACE-CONTRACTS.instructions.md`](WORKSPACE-CONTRACTS.instructions.md))
    - SMAPI API references (see [`../Instructions/external-resources.instructions.md`](../Instructions/external-resources.instructions.md))

## 1. Canonical Backend Subsystems ##

Agents MUST preserve these subsystem boundaries.

## 1.1 Evaluation Engine (task computation) ##

Responsibilities:
    - Evaluate rules and built-in generators to produce **candidate task definitions** and
    evaluation results.
    - Compute derived values needed for decision-making (e.g., requirement satisfaction), but do not
    own canonical task state.

Hard rules:
    - MUST be deterministic for the same inputs.
    - MUST be bounded; avoid unbounded scans.
    - MUST NOT mutate canonical task state directly.

## 1.2 State Store (single source of truth) ##

Responsibilities:
    - Own canonical task state.
    - Accept **commands** (intent) and apply **reducers** (the only legal mutation path).
    - Publish **read-only snapshots** for the frontend.

Hard rules:
    - MUST be the sole owner of canonical state.
    - MUST apply deterministic reducers.
    - MUST publish snapshots only from canonical state.

## 1.3 Persistence ##

Responsibilities:
    - Save/load only the minimal essential data required to reconstruct state.
    - Support explicit versioning and deterministic migrations.

Hard rules:
    - MUST include version identifiers in saved data.
    - MUST NOT persist transient caches or UI state.
    - MUST support safe failure paths for invalid/unknown data versions.

## 1.4 Daily Snapshot Ledger (history/statistics) ##

Responsibilities:
    - Capture daily snapshots (or day-keyed entries) for history browsing and statistics.
    - Maintain deterministic day keys and stable linkage to snapshots.

Hard rules:
    - MUST be deterministic per day key.
    - SHOULD store snapshots in a compact form appropriate for history queries.

## 1.5 Dependency wiring (constructor injection, Design Guide Section 2.4) ##

Core backend subsystems MUST declare dependencies explicitly via constructor parameters.

Backend MUST NOT rely on service locators or ambient global singletons for core subsystem
dependencies.

Lifecycle/composition roots MAY assemble dependency graphs, but runtime subsystem instances MUST
still receive explicit constructor-injected dependencies to preserve testability and dependency
visibility.

## 2. Mutation Boundary (Command → Reducer → State) ##

## 2.1 Single source of truth ##

The State Store is the single source of truth for canonical task state.

## 2.2 Legal mutation path ##

    - UI MUST NOT mutate canonical state.
    - Evaluation Engine MUST NOT mutate canonical state.
    - All canonical state changes MUST occur via commands processed by the State Store.
    - Reducers MUST implement deterministic transformations.

Commands represent intent; reducers are the only legal mutation mechanism.

## 2.3 Snapshot publishing ##

    - The State Store MUST publish read-only snapshots for UI consumption.
    - Snapshots SHOULD be updated when (and only when) canonical state changes.
    - Snapshots MUST be treated as immutable by consumers.

## 3. Determinism Contract ##

Determinism is a first-class requirement.

## 3.1 Deterministic Task IDs ##

Task IDs MUST be deterministic strings derived from stable inputs such as:
    - task source (built-in generator / rule / manual)
    - stable subject identity (item ID, building key, location key, etc.)
    - deterministic time/day key for recurring tasks (when applicable)

Task IDs MUST remain stable across:
    - reloads
    - evaluation passes
    - save/load cycles

Prohibited:
    - random IDs
    - GUID/NewGuid IDs for generated tasks
    - IDs dependent on traversal order of unordered collections

## 3.2 Deterministic ordering ##

Any ordering used for:
    - evaluation
    - reconciliation
    - snapshot output
    - history/ledger entries

MUST be stable and reproducible given the same inputs.

## 4. Rule System Contract (Backend concerns) ##

## 4.1 Rules are data ##

    - Rules MUST be representable as serializable data.
    - Runtime-only fields MUST be recomputed and MUST NOT be persisted unless explicitly required.

## 4.2 Validation ##

    - Rules MUST be validated on create/update and on load.
    - Invalid rules MUST fail safely (no crash, no corrupted state).
    - Validation errors MUST be surfaced through an error channel consumable by the frontend (see
    Interaction section).

## 4.3 Evaluation constraints ##

Rule evaluation MUST be:
    - deterministic
    - bounded in work
    - designed to avoid expensive world scans

## 5. Engine Update Cycle Contract ##

## 5.1 Hybrid update model ##

The backend MUST support:
    - event-triggered evaluation (preferred)
    - bounded periodic passes (only when necessary)
    - snapshot publishing
    - daily snapshot capture

## 5.2 Event/evaluation queue behavior ##

If an evaluation queue exists:
    - duplicates MAY be coalesced
    - queue size MUST remain bounded
    - evaluation SHOULD be batched into a single pass when possible

## 5.3 Lifecycle phases ##

Backend MUST support these conceptual phases:

1. Initialization
2. Runtime updates (event-driven)
3. Day transition
4. Save/load handling

Initialization MUST:
    - run initial evaluation
    - publish initial snapshot

Day transition MUST:
    - capture daily snapshot
    - apply any day-scoped recurring logic deterministically
    - publish refreshed snapshot after reconciliation

## 6. Persistence Contract ##

## 6.1 Minimal storage principle ##

Persistence MUST store only minimal data required to reconstruct canonical state.

Derived values (presentation, sorting, warning flags, progress percentages) MUST be recomputed.

## 6.2 Version safety ##

    - Saved data MUST include version identifiers.
    - Migrations MUST be explicit and deterministic.
    - If migration is unsafe or impossible, backend MUST fail gracefully with a recoverable error
    state.

## 6.3 Separation of concerns ##

Persistence MUST NOT contain:
    - UI state
    - transient engine caches
    - ephemeral runtime-only diagnostics

## 7. Reconciliation Contract (Generated vs Persisted) ##

When reconciling generated task definitions against persisted tasks:
    - reconciliation MUST be deterministic
    - reconciliation SHOULD be linear time relative to task count
    - reconciliation MUST preserve user-driven completion state where applicable

## 8. Performance Guardrails (Backend) ##

## 8.1 Evaluation frequency limits ##

Task generation and rule evaluation MUST NOT run every frame unless explicitly justified and proven
lightweight.

Prefer event-driven triggers such as:
    - day start
    - inventory change
    - building completion
    - skill changes
    - explicit debug command

## 8.2 Generator scan constraints ##

Generators MUST avoid scanning the entire world state on frequent intervals.

Prefer:
    - caching
    - scoping to current location
    - incremental updates based on events

## 9. Error Handling and Safe Failure ##

    - Backend MUST fail safely and avoid crashing the game session.
    - Errors MUST be reportable via channels consumable by the frontend and logs.
    - Invalid rules SHOULD be disabled/marked invalid with remediation guidance.

## 10. Backend ↔ Frontend Interaction Contract (shared boundary) ##

This section describes how backend and frontend MUST interact. (Frontend also repeats this boundary
from its perspective.)

## 10.1 Snapshot boundary ##

Backend MUST publish a read-only snapshot model that the frontend consumes for:
    - HUD display
    - menu dashboards
    - history/statistics views

Frontend MUST NOT modify snapshots; it may request changes only via commands.

## 10.2 Command boundary ##

Frontend actions MUST be expressed as **commands** sent to the backend State Store.

Backend MUST:
    - validate commands
    - apply deterministic reducers
    - publish a new snapshot if canonical state changes

## 10.3 Error and status reporting ##

Backend MUST expose:
    - validation errors (rules/config)
    - migration/version errors
    - runtime engine warnings (bounded, deduplicated)

Frontend MUST render these errors in supported UI surfaces when appropriate.

## 11. Localization-Neutral Backend Contract ##

## 11.1 Canonical locale neutrality ##

Backend canonical state (engine/state/persistence/history) MUST remain locale-neutral.

Translated display strings MUST NOT be treated as canonical source-of-truth data.

## 11.2 Determinism protection ##

Localized text MUST NOT participate in:
    - TaskID/RuleID/DayKey/SubjectID generation
    - identifier equality/collision checks
    - deterministic ordering or reconciliation logic

## 11.3 Engine output for UI text ##

When backend output is intended for UI rendering, backend MUST emit stable semantic keys (and
optional formatting arguments), not locale-rendered strings.

Translation rendering is frontend-owned through SMAPI `I18n`.

## 11.4 Persistence boundary ##

Persistence MUST store stable keys/arguments where required and MUST NOT persist locale-rendered
display text as canonical data.
