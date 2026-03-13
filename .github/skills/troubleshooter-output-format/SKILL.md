---
name: troubleshooter-output-format
description: "Troubleshooter diagnostic response template for root-cause analysis reports. Use when: reporting troubleshooting findings for build/runtime/UI/state/persistence/performance issues."
argument-hint: "Problem symptoms and diagnostic findings"
---

# Troubleshooter Output Format #

Use this skill when drafting Troubleshooter responses.

## Required Sections ##

1. Problem Summary
2. Most Likely Cause
3. Other Plausible Causes
4. Evidence Reviewed
5. Boundary / Contract Checks
6. Recommended Next Step
7. Optional Fix Direction
8. Documentation Update

## Template ##

## Problem Summary ##

- What is failing and when.
- Classify failure type (compile-time, runtime, UI behavior, state-flow, persistence, determinism, performance).

## Most Likely Cause ##

- Strongest root-cause hypothesis.
- Why it is the leading explanation.

## Other Plausible Causes ##

- Ranked alternatives.
- Evidence needed to separate each alternative.

## Evidence Reviewed ##

- Logs, errors, stack traces, files, and reproduction data used.

## Boundary / Contract Checks ##

- Relevant architecture or contract checks.

## Recommended Next Step ##

Choose one:
- safe to fix locally
- needs Planner before fix
- needs Researcher context first
- needs Reviewer after patch
- needs more evidence

## Optional Fix Direction ##

- Exact inspection/edit area.
- What not to change.
- What to verify after fix.

## Documentation Update ##

- docs update needed: yes or no
- if yes: target owner (WorkspaceAgent for workspace docs, GodAgent for agent customization)
- if no: concise rationale

## Delegation Decision Criteria ##

Use these criteria when deciding whether to route troubleshooting outcomes.

## Delegate to GodAgent ##

Delegate when:

- The same mistake type has occurred multiple times (for example repeated contract violations, skipped validations, or boundary misses).
- Root cause is agent behavior, not code logic.
- Fix requires updating agent customization artifacts (`.agent.md`, `.instructions.md`, `SKILL.md`, hooks, or governance rules).
- You can articulate a specific prevention rule.

Do not delegate when:

- Issue is a one-off code bug with no systemic pattern.
- Problem is user error or environment configuration rather than agent behavior.
- Fix is code-level implementation or architecture work rather than agent-guidance work.

When delegating, provide:

1. Recurring pattern description (what repeats and frequency).
2. Affected agent(s).
3. Specific prevention rule to add.
4. Concrete examples illustrating the pattern.

## Delegate to WorkspaceAgent ##

Delegate when:

- Root cause confirms a major architecture issue, especially from agent-generated code.
- Issue exposes a gap in design guides, architecture contracts, or contributor docs.
- Fix changes documented behavior, setup, workflow, or known limitations.
- Troubleshooting outcome yields a reusable prevention pattern with architecture implications.

Do not delegate when:

- Issue is a minor local coding error (typo, off-by-one, null check, local logic bug) with no architecture significance.
- Issue is transient/local with no reusable guidance.
- Proposed docs would only duplicate existing guidance.

When delegating, provide:

1. Problem summary and trigger conditions.
2. Confirmed root cause.
3. Fix summary and verification evidence.
4. Suggested target docs and exact required updates.
