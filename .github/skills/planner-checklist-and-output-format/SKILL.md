---
name: planner-checklist-and-output-format
description: "Planner checklist plus response template for scoped implementation planning. Use when: converting research and constraints into executable plans."
argument-hint: "Planning scope, constraints, and target subsystem"
---

# Planner Checklist and Output Format #

Use this skill for Planner outputs to keep planning structure consistent and deterministic.

## Planning Checklist ##

Cover as many relevant checks as possible:

1. Goal classification
- Feature, bug fix, refactor, UI composition, state flow, persistence, tooling, or review preparation.

2. Affected subsystems
- Identify touched layers (HUD/Menu/State Store/Engine/Persistence/Snapshot/History/Config/shared IDs).

3. Change type
- Additive, corrective, refactor, extraction, rename, schema/version, markup, or test-only.

4. Constraint class
- No behavior change, no new dependencies, file-scope limits, determinism, save compatibility, no direct UI mutation, no per-frame heavy work.

5. Deferment consideration
- For atomic checklist creation tasks, integrate scheduled deferments or explicitly re-defer with rationale.

## Output Template ##

## Plan Summary ##

- Target outcome.
- Additive/corrective/refactor-only classification.
- Scope clarity.

## Governing Constraints ##

- Contracts, design sections, and user limits controlling the plan.

## Architecture Decision ##

- Correct layer ownership and boundary protections.

## In Scope ##

- Concrete items to change now.

## Out of Scope ##

- Related items intentionally deferred.

## Deferment Incorporation (when applicable) ##

- Scheduled deferments integrated now.
- Re-deferred items with rationale.

## Files / Symbols Likely Affected ##

- Specific files, folders, and symbols expected to change.

## Ordered Implementation Plan ##

- Numbered execution steps, concrete and actionable.

## Verification Checklist ##

- Compile/runtime checks.
- Contract checks.
- Behavior checks.
- Determinism/persistence checks where relevant.

## Risks / Review Notes ##

- Failure points, review concerns, migration/performance concerns.

## Done Definition ##

- Concrete completion criteria.

## Handoff Block ##

Include:
- Goal
- Scope
- Constraints
- Edit order
- Verify before merge

For WorkspaceAgent handoffs, include Review Gate fields from Planner guidance.
