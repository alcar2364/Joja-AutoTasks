---
name: "Implement Frontend UI Change"
description: "Use when: implementing HUD/menu/view-model changes while preserving snapshot-driven UI boundaries."
argument-hint: "UI goal + target surface + scope + constraints"
agent: "UIAgent"
---

Implement the UI change below while preserving frontend architecture contracts.

Implementation Inputs
- Goal: <required>
- Target surface: <HUD|Menu|ViewModel|mixed>
- Scope (files/symbols): <required>
- Constraints: <single file|no behavior change|performance|other>

Frontend Guardrails
- UI consumes snapshots as read-only data.
- UI does not mutate canonical state directly.
- User actions dispatch commands for canonical changes.
- HUD remains lightweight and menu owns complex interactions.

Required Output
1. Summary of implemented UI changes
2. Contract checks and boundary validation
3. Verification steps (interaction and edge cases)
4. Remaining risks or follow-ups
