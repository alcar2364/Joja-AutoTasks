---
name: "Create Implementation Plan"
description: "Use when: converting requirements into a step-by-step, low-risk implementation plan before edits."
argument-hint: "Goal + scope + constraints + acceptance criteria"
agent: "Planner"
---

Produce a detailed implementation plan without editing files.

Planning Inputs
- Goal: <required>
- Scope: <required>
- Constraints: <single file|no behavior change|performance|other>
- Acceptance criteria: <required>
- Prior research summary (optional): <paste>

Plan Requirements
- Respect JAT architecture boundaries and determinism rules.
- Keep steps ordered for safe, incremental execution.
- Include verification and rollback considerations.
- Flag any required scope expansion before proposing it.

Required Output
1. Assumptions and decisions
2. Step-by-step plan with milestones
3. Verification checklist
4. Risks and fallback strategy
