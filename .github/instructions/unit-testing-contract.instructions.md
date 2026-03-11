---
name: unit-testing-contract
description: "Unit testing rules: determinism, architecture boundaries, identifier/ordering verification, persistence safety, review output. Use when: creating or editing unit tests."
applyTo: "Tests/**/*.cs"
---

# UNIT-TESTING-CONTRACT.instructions.md #

## Purpose ##

This contract defines **unit testing rules** for AI agents working in the
**JAT (Joja AutoTasks)** workspace.

Its purpose is to ensure unit tests:
    - are deterministic and non-flaky
    - protect architecture boundaries from regression
    - verify deterministic identifiers and ordering behavior
    - cover persistence reconstruction and migration safety
    - enforce event-driven and performance-sensitive expectations
    - produce consistent, severity-based review output

This contract applies to:
    - new C# unit test creation
    - edits to existing unit tests
    - review of drafted or existing unit tests
    - test adequacy assessment for backend, state, persistence, and boundary behavior

This contract does **not** define:
    - general coding style rules for production code (see [`CSHARP-STYLE-CONTRACT.instructions.md`](CSHARP-STYLE-CONTRACT.instructions.md))
    - frontend visual design rules (see [`FRONTEND-ARCHITECTURE-CONTRACT.instructions.md`](FRONTEND-ARCHITECTURE-CONTRACT.instructions.md), [`../Instructions/visual-design-language.instructions.md`](../Instructions/visual-design-language.instructions.md))
    - workspace interaction and delegation policy (see [`WORKSPACE-CONTRACTS.instructions.md`](WORKSPACE-CONTRACTS.instructions.md))

## 1. Source-of-truth order for test work ##

Agents creating or reviewing unit tests MUST use this precedence order:

1. explicit user instructions in the current task
2. approved task plan for the current work, if one exists
3. [`WORKSPACE-CONTRACTS.instructions.md`](WORKSPACE-CONTRACTS.instructions.md)
4. [`BACKEND-ARCHITECTURE-CONTRACT.instructions.md`](BACKEND-ARCHITECTURE-CONTRACT.instructions.md)
5. [`FRONTEND-ARCHITECTURE-CONTRACT.instructions.md`](FRONTEND-ARCHITECTURE-CONTRACT.instructions.md)
6. [`REVIEW-AND-VERIFICATION-CONTRACT.instructions.md`](REVIEW-AND-VERIFICATION-CONTRACT.instructions.md)
7. [`CSHARP-STYLE-CONTRACT.instructions.md`](CSHARP-STYLE-CONTRACT.instructions.md)
8. `JojaAutoTasks Design Guide.md` and referenced design sections
9. established test patterns already present in the touched subsystem

If sources conflict, the agent MUST state the conflict and follow the higher-priority rule.

## 2. Deterministic test requirements ##

Unit tests MUST be deterministic and repeatable across runs.

Tests MUST NOT rely on:
    - ambient wall-clock time unless time is explicitly controlled
    - random values without deterministic seeding/control
    - ordering from unordered collections when order is asserted
    - shared mutable global state across tests
    - external process/environment state that is not isolated by the test

If deterministic seams are missing, the agent MUST report the testability gap and propose the
minimal corrective seam.

## 3. State Store boundary verification ##

Tests for state mutation behavior MUST enforce the command/reducer/snapshot boundary.

Required checks where relevant:
    - commands represent intent, not direct mutation
    - reducers are the only legal canonical mutation path
    - snapshots are treated as read-only outputs
    - non-State-Store components do not mutate canonical state directly

When reviewing existing tests, agents MUST flag missing boundary assertions as at least **Major**.

## 3.1 UI snapshot-consumption verification ##

When UI/view-model tests are in scope, tests MUST verify:
    - UI consumes snapshots as read-only data
    - UI does not mutate snapshot objects in place
    - UI refresh is triggered by snapshot-changed events/signals rather than per-frame polling
    - UI does not generate or mutate TaskID/RuleID/DayKey/SubjectID values

When reviewing existing UI tests, missing snapshot immutability or refresh-trigger assertions MUST
be flagged as at least **Major**.

## 3.2 Dependency wiring and constructor-injection verification ##

As defined by the Design Guide (Section 2.4), tests SHOULD verify that touched subsystems/view
models receive core dependencies via constructors rather than service-locator lookups.

When service-locator or ambient-global usage blocks deterministic testing in touched scope, agents
MUST flag it and propose the minimal constructor-injection seam required.

## 4. Deterministic IDs and ordering checks ##

Where IDs, reconciliation, recurrence, or snapshot ordering are involved, tests MUST verify:
    - deterministic ID composition from stable inputs
    - stability across repeated evaluation passes
    - stability across save/load reconstruction
    - stable ordering for evaluated/reconciled/snapshotted collections

Tests MUST include at least one regression-oriented assertion that would fail if traversal-order
dependence is introduced.

## 5. Persistence reconstruction and migration coverage ##

When persistence is in scope, tests MUST cover:
    - reconstruction of canonical state from minimal persisted data
    - omission of derived/transient values from persisted payloads
    - explicit version handling behavior
    - deterministic migration behavior between supported versions
    - safe failure behavior for unknown/invalid versions

If migration logic is changed without corresponding tests, agents MUST flag a **Blocker**.

## 6. Event-driven and performance-sensitive testing guidance ##

Tests SHOULD prefer event-triggered evaluation pathways over frame-driven assumptions.

Tests for evaluation scheduling MUST verify, where applicable:
    - work is triggered by explicit events
    - queue/coalescing behavior is bounded
    - duplicate events do not create unbounded repeated work
    - no accidental per-frame heavy processing path is required for correctness

Performance-sensitive tests MAY use lightweight instrumentation/assertions to validate bounded work,
but MUST avoid brittle timing-based thresholds.

## 7. Test review requirements ##

When reviewing drafted or existing tests, agents MUST produce findings in severity order:
    - **Blocker**
    - **Major**
    - **Minor**
    - **Note**

Review findings MUST include:
    - file/symbol under review
    - missing or weak scenario description
    - impact statement tied to architecture/determinism/persistence/performance risk
    - concrete fix direction

## 8. Required unit-test review output format ##

Unless the user requests another format, test review output MUST use:

### 8.1 Scope ###

    - what tests were reviewed or created
    - relevant scope limits and assumptions

### 8.2 Findings by severity ###

    - Blocker
    - Major
    - Minor
    - Note

Each finding must include the affected file/symbol and the missing or weak assertion.

### 8.3 Missing-case matrix ###

    - scenario not covered
    - expected behavior
    - recommended test shape

### 8.4 Confidence statement ###

    - High / Medium / Low confidence
    - short justification

## 9. Production-code shortcomings surfaced by tests ##

When test design reveals production-code shortcomings (for example, no deterministic seam,
hard-coupled dependencies, or mixed responsibilities), agents MUST:
    - call them out explicitly
    - classify severity based on impact to correctness/determinism/safety
    - propose minimal changes needed to make behavior testable

Agents MUST NOT silently broaden implementation scope without user approval.

## Summary ##

This contract ensures unit-test work in JAT is deterministic, architecture-aware, persistence-safe,
and reviewed with consistent severity-based output.