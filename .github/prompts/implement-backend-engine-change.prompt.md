---
name: "Implement Backend Engine Change"
description: "Use when: implementing backend/state/rule/persistence changes in deterministic, contract-safe ways."
argument-hint: "Backend goal + scope + constraints + expected behavior"
agent: "GameAgent"
---

Implement the backend change below while preserving deterministic behavior and architecture boundaries.

Implementation Inputs
- Goal: <required>
- Scope (files/symbols): <required>
- Constraints: <no behavior change|single file|performance guardrails|other>
- Expected behavior after change: <required>

Backend Guardrails
- State Store remains the canonical state owner.
- Mutations flow through command/reducer path only.
- IDs and ordering remain deterministic.
- Persistence remains minimal and version-safe.

Required Output
1. Summary of implemented changes
2. Contract checks performed
3. Verification steps executed
4. Residual risks or follow-ups
