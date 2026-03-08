---
name: handoff-optimizer
description: >-
  Optimizes agent handoffs by pre-loading context, suggesting parallel work,
  and preventing circular delegation. Improves routing efficiency.
trigger: before-handoff
applyTo: all
---

# Handoff Optimizer Hook #

    * Trigger: before delegating to a subagent.
    * Purpose: improve handoff quality, reduce redundant work, and avoid routing loops.

## 1. Context Pre-Loading ##

Before handoff, attach the minimum context that enables execution.

By target type:

    * Researcher: likely file paths, symbols, and known context to avoid duplicate discovery.
    * Planner: research findings, relevant guide sections, and architecture constraints.
    * Implementation agents: approved plan, target files and symbols, and scope limits.
    * Reviewer: relevant contracts, risk focus, and verification priorities.

## 2. Parallel Work Detection ##

Suggest parallel handoffs only when dependencies are independent.

Usually parallel-safe:

    * Independent research tracks.
    * Loosely coupled UI and StarML edits.
    * Separate test files with no shared ordering dependency.
    * Documentation updates that do not depend on pending code decisions.

Usually sequential:

    * Research -> planning -> implementation.
    * Implementation -> review.
    * Backend API changes -> dependent UI integration.

Parallel suggestion output pattern:

    HANDOFF OPTIMIZATION OPPORTUNITY
    Parallel candidates: [list]
    Expected benefit: reduced overall handoff overhead
    Action: execute in parallel or keep sequential

## 3. Circular Handoff Prevention ##

Before handoff, validate:

    * Target agent is not already in the active delegation stack.
    * Same handoff has not repeated without new context.
    * A direct action by current agent is not a better next step.

If a loop is detected, block handoff and suggest a concrete non-loop action.

## 4. Scope Sharpening ##

Handoffs must be specific and actionable.

Required handoff fields:

    * Target subsystem or concern.
    * Explicit action verb such as implement, review, or refactor.
    * File or symbol scope, or explicit research-discovery request.
    * Constraints such as single-file, no behavior change, or touched-region only.
    * Expected output format.

Sharpening example:

    Implement HUD collapse toggle in UI/HUD/TaskListView.cs.
    Track local collapse state, wire button event, and preserve snapshot read-only usage.

## 5. Success Criteria ##

Define completion criteria per handoff type:

    * Researcher: return file paths, symbols, and citations.
    * Planner: return ordered plan with assumptions and decision points.
    * Implementation agent: return edited files and verification status.
    * Reviewer: return severity-ranked findings with fix guidance.

## 6. Handoff Caching ##

Avoid redundant handoffs when prior results already cover the same request.

    * Reuse prior result if context did not change.
    * If context changed, send only the delta and required refresh scope.

## Output Behavior ##

    * Silent for normal optimization.
    * Emit output only when a loop is blocked, parallelization needs confirmation,
      or prompt sharpening is required.

## Integration ##

Works with:

    * `contract-auto-loader` for contract-aware handoff context.
    * `design-guide-context-augmenter` for guide citations in delegation.
    * `state-mutation-guard` and `ui-boundary-enforcer` for boundary-safe delegation.
