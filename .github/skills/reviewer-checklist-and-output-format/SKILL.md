---
name: reviewer-checklist-and-output-format
description: "Reviewer checklist plus response template for contract, architecture, and risk verification. Use when: reviewing plans, diffs, or full implementations."
argument-hint: "Review scope, changed files, and findings"
---

# Reviewer Checklist and Output Format #

Use this skill for Reviewer work so checklist coverage and report format stay consistent.

## Review Checklist ##

Cover as many relevant checks as possible:

1. Scope compliance
- Requested scope followed.
- No unrelated opportunistic edits.
- No-behavior-change claims validated.

2. Architecture placement
- Logic lives in the correct layer.
- No UI mutation of canonical backend state.
- No persistence or migration work in presentation layers.

3. Contract compliance
- Applicable contracts checked (workspace, review, backend/frontend, style, StarML, JSON).

4. Behavioral integrity
- No unintended behavior changes.
- No hidden coupling or state-flow regressions.

5. Determinism and persistence safety
- Deterministic ID/order behavior preserved.
- Persistence/migration compatibility risks assessed.

6. Performance safety
- No obvious hot-path regressions.

## Output Template ##

## Review Scope ##

- What was reviewed.
- Review mode used.
- Scope limits or assumptions.

## Context Gathered (if applicable) ##

- File searches, read locations, and evidence collected.

## Governing Sources ##

- Contracts, design sections, plan references, and constraints used.

## Verdict ##

Choose one:
- approved
- approved with follow-ups
- changes required
- cannot verify fully

## Blocking Issues ##

- List true blockers only, or state None.

## Major Issues ##

- List important non-blocking concerns, or state None.

## Minor Issues ##

- List minor concerns, or state None.

## What Looks Correct ##

- Call out validated parts to avoid accidental rework.

## Verification Notes ##

- Architecture fit
- Contract compliance
- Behavior preservation
- Determinism
- Persistence/migration
- Performance/hot-path safety

## Recommended Next Step ##

Choose one:
- safe to merge
- fix blockers and re-review
- fix majors and proceed
- return to Planner
- needs deeper code/context review

## Optional Fix List ##

- Short ordered fix list limited to real findings.

## Phase Completion Addendum ##

When the review target is a phase completion gate/checklist, use this protocol:

1. Finalize checklist verification artifacts and completion notes.
2. Reconcile implementation issue findings with the Implementation Issues system (records/index/archive view).
3. Record scope/architecture issues, ambiguity, and remediation guidance in a standalone post-phase implementation review report.
4. Do not implement scope/architecture fixes during the review pass.
5. Route implementation remediation to a user-owned post-phase implementation atomic execution checklist.

For this workflow, the Reviewer is a verifier and teacher, not a fixer.

Also create two separate artifacts:

1. A standalone post-phase implementation review report.
2. A separate post-phase implementation atomic execution checklist.

Do not combine both artifacts into one section or one file.

You must not:

- Approve work you cannot actually verify.
- Label speculation as fact.
- Inflate minor style concerns into blockers without contract support.
- Ignore user scope in order to perform a vanity audit.
- Miss hidden behavior drift in cleanup patches.
- Ignore determinism or persistence risks when they are in play.
- Recommend broad rewrites when the review target is local.
- Handwave with best practices without naming the exact contract or boundary involved.

## Post-Phase Review Report ##

- Scope/architecture issues and ambiguity discovered during completion review.
- Evidence for each finding (files/symbols/contracts).
- Why each issue is deferred from in-phase implementation.

## Post-Phase Atomic Execution Checklist ##

- Ordered remediation items the user should execute in a post-phase implementation atomic execution checklist.
- Acceptance criteria per item.
- Explicit note that reviewer did not implement scope/architecture fixes during the review pass.
