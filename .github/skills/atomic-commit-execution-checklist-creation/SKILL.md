---
name: atomic-commit-execution-checklist-creation
description: "Create an atomic commit execution checklist from a design guide phase specification. Produces a step-by-step, reviewer-validated checklist suitable for WorkspaceAgent drafting."
argument-hint: "Provide a design guide phase reference or feature specification and optional scope constraints."
---

# Atomic Commit Execution Checklist Creation

Use this skill to produce a detailed, implementation-ready atomic commit execution checklist from a design guide phase or feature specification. The checklist breaks a phase into minimal, testable commits, includes guardrails, verification steps, and a final review gate.

Inputs
- `phaseReference` (string): path or citation to the design guide phase or feature spec.
- `constraints` (optional string): implementation constraints or scope notes.

Outcome
- A structured plan suitable for Reviewer validation and final drafting by WorkspaceAgent. Targets: Guardrails, Phase Overview, Steps+Substeps, Unit Tests, Final Review Gate.

When to use
- The design phase is specified and ready for implementation decomposition.

How it works (high level)
1. Researcher gathers design context and implementation-issue signals.
2. Planner produces a structured step/substep breakdown with verification criteria.
3. Reviewer validates the plan.
4. WorkspaceAgent drafts the final Markdown checklist file.

Mandatory pre-publish checks
- Section 21 section-to-phase mapping check against checklist scope.
- `ImplementationIssues` scheduling consistency check across index and referenced issue records.
- merged-duplicate check to ensure one canonical active tracker remains for merged scopes.

Notes
- This skill encapsulates the multi-agent flow into a discoverable action; it does not itself perform code edits.
