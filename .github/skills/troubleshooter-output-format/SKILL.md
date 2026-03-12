---
name: troubleshooter-output-format
description: Troubleshooter diagnostic response template for root-cause analysis reports. Use when: reporting troubleshooting findings for build/runtime/UI/state/persistence/performance issues.
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
