---
name: "Create Atomic Commit Execution Checklist"
description: "Use when: converting a design guide phase specification into a step-by-step checklist with minimal atomic commits."
argument-hint: "Phase spec + design guide section + target scope"
agent: "Orchestrator"
---

Create an atomic commit execution checklist from a design guide phase specification.

Checklist Inputs
- Phase specification: <required>
- Design guide section(s): <required>
- Target scope: <required>
- Constraints (optional): <list>

Delegation Chain
1. Researcher: gather design guide context, existing implementation patterns, and relevant active implementation issues for the target phase
2. Planner: structure the checklist, define substeps with atomic commit boundaries, and decide which implementation issues are resolved in-scope vs. explicitly re-deferred
3. Reviewer: validate compliance with design guide and contract requirements
4. WorkspaceAgent: create the final checklist file directly (use #createFile tool)

Checklist Requirements
- Each substep must have a completion checkbox.
- Step completion checkboxes appear after all substeps in each step.
- Provide suggested commit messages (not mandatory).
- Do not require a final audit proving every step was atomically committed.
- Guardrails are checked in final review before build/test verification.
- Unit testing must be executable by human or UnitTestAgent.
- Final review must be executable by human or Reviewer agent.
- Checklist creation must include active implementation issue discovery and a final implementation issue reconciliation step.

Required Output
1. Checklist file written to `Project/Tasks/Implementation Plan/Phase N - Atomic Commit Execution Checklist.md`
2. Substeps with completion checkboxes and suggested commit messages
3. Final review and verification steps
