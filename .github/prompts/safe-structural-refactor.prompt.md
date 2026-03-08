---
name: "Safe Structural Refactor"
description: "Use when: performing rename/extract/move refactors while preserving behavior and contract boundaries."
argument-hint: "Refactor goal + affected symbols/files + constraints"
agent: "Refactorer"
---

Execute the refactor below with behavior preservation and low-risk sequencing.

Refactor Inputs
- Refactor goal: <required>
- Affected symbols/files: <required>
- Scope boundaries: <required>
- Constraints: <no behavior change|required naming rules|other>

Refactor Guardrails
- Preserve existing behavior unless explicitly requested.
- Keep edits minimal, grouped, and reversible.
- Avoid broad churn outside requested scope.
- Respect architecture and style contracts.

Required Output
1. Refactor summary
2. Behavior-preservation checks
3. Verification steps executed
4. Follow-up recommendations
