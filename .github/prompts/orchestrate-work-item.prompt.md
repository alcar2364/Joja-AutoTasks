---
name: "Orchestrate Work Item"
description: "Use when: routing feature, bug, refactor, or mixed requests through JAT specialist agents."
argument-hint: "Goal + scope + constraints + definition of done"
agent: "Orchestrator"
---

Route this request through the JAT agent ecosystem and execute the safest workflow.

Task Intake
- Goal: <required>
- Work type: <feature|bug|refactor|review|mixed>
- Target subsystem(s): <required>
- Scope limits: <single file|no behavior change|analysis only|other>
- Definition of done: <required>

Workflow Controls (customizable)
- Research first: <yes/no>
- Plan before edits: <yes/no>
- Reviewer pass before final output: <yes/no>
- Unit test step: <none|new tests|test review>

Required Output
1. Classification and selected route
2. Delegation chain with rationale
3. Actions taken and files touched
4. Risks, open questions, and next step options
