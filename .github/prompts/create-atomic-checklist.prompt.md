---
description: "Create an atomic phase execution checklist from a design guide phase specification. Use when: planning implementation of a design phase with minimal atomic commits and design-guide compliance."
agent: "Orchestrator"
tools: ["vscode_askQuestions"]
argument-hint: "Design phase or feature to break down into atomic commit steps"
model: ["Claude Opus 4.6 (copilot)", "Claude Sonnet 4.5 (copilot)"]
---

# Create Atomic Phase Execution Checklist

Use this prompt to break down a design phase or feature specification into a detailed, step-by-step atomic commit execution checklist.

## What You Provide

- Reference to the design guide phase or feature specification
- Scope of implementation (what problem/feature is being addressed)
- Any constraints or previous phase context

## What the Orchestrator Will Do

The Orchestrator will use `.github\instructions\atomic-commit-execution-checklist-creation.instructions.md` to guide the creation process and will interactively ask clarifying questions to:

1. **Clarify scope and dependencies** — What does this phase depend on? What are the subsystem boundaries?
2. **Identify atomic commits** — What represents a minimal, testable, self-contained change?
3. **Structure verification steps** — What tests or manual checks must pass after each commit?
4. **Surface constraints** — Are there architecture invariants, determinism requirements, or migration safety concerns?
5. **Confirm coverage** — Are there implementation issues or gaps that need explicit handling?

## Output

A detailed checklist document that:
- Lists step-by-step commits with precise scope
- Includes verification and test coverage for each step
- Explicitly calls out architecture guardrails
- Remains portable and reusable
- Guides both human and AI implementation

## Required Instruction File

This prompt relies on the canonical **atomic-commit-execution-checklist-creation.instructions.md** file, which defines the multi-agent workflow (Researcher → Planner → Reviewer → Implementation) and the detailed checklist structure.

---

**Tip**: After the checklist is created, use it to delegate implementation step-by-step to the appropriate subagents (GameAgent, UIAgent, UnitTestAgent, etc.) with explicit "step N of M" tracking.
