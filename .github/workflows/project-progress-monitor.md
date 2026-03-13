---
name: project-progress-monitor
description: "Weekly monitor for implementation phase progress and implementation issue backlog hygiene."
on:
  schedule: weekly
  workflow_dispatch:
permissions:
  contents: read
  issues: read
  pull-requests: read
strict: true
network:
  allowed: [defaults, github]
engine:
  id: copilot
tools:
  github:
    toolsets: [default]
safe-outputs:
  noop:
    report-as-issue: false
  create-discussion:
    category: general
    title-prefix: "[progress] "
    max: 1
  create-issue:
    title-prefix: "[implementation-issues] "
    labels: [agentic-workflow, implementation-issue, issue-type: review-follow-up, priority: medium]
---

# Project Progress Monitor — Phase Dashboard & Implementation Issues Backlog Tracker

Weekly combined view of implementation phase progress and deferred work item health. This workflow
merges the responsibilities of the former `phase-progress-tracker` (daily) and
`deferments-monitor` (weekly) into a single weekly run, using the Implementation Issues system as the backlog source of truth.

## Context

- Repository: `${{ github.repository }}`
- Checklist directory: `Project/Tasks/Implementation Plan/`
- Checklist pattern: `Phase N - Atomic Commit Execution Checklist.md`
- Implementation Issues index: `Project/Tasks/ImplementationPlan/ImplementationIssues/ImplementationIssuesIndex.md`
- Implementation Issues archive: `Project/Tasks/ImplementationPlan/ImplementationIssues/ImplementationIssuesArchive.md`
- Implementation issue records: `Project/Tasks/ImplementationPlan/ImplementationIssues/Records/`
- Checkbox syntax: `- [x]` (complete), `- [ ]` (incomplete)
- Legacy deferment ID format (when present): `DEF-NNN`
- Backlog aging threshold: 28 days (4 weeks) from record creation or last meaningful update

## Part 1: Phase Progress Analysis

### Discover Checklist Files

Find all files matching `Phase * - Atomic Commit Execution Checklist.md` in the implementation
plan directory, ordered by phase number ascending.

### Parse Each Checklist

For each checklist file:

- Count total checkboxes (`- [ ]` + `- [x]`)
- Count completed checkboxes (`- [x]`)
- Calculate completion percentage: `completed / total * 100`
- Identify the Final Completion Gate section (last section)
- Determine phase status:
  - **Not Started:** 0% complete
  - **In Progress:** 1%–99% complete
  - **Complete:** 100% complete AND Final Completion Gate all checked

### Identify Active Phase

The active phase is the highest-numbered phase with status "In Progress."
If no phase is In Progress, the active phase is the first "Not Started" phase.

### Detect Stalled Steps

For each In Progress phase, identify Steps where:

- The step header exists (e.g., `## 2) Bootstrap Logging Foundation ##`)
- No substep under that step has been completed in the last 14 days (based on commit history)
- This indicates a potentially stalled step

> **Note:** The 14-day stall window (vs. the 7-day window used by the former daily
> `phase-progress-tracker`) is intentional: this workflow runs weekly, so a 7-day window
> would trivially flag every step between Monday runs. 14 days provides a meaningful signal
> that a step has genuinely stalled across two consecutive weekly checks.

## Part 2: Implementation Issues Backlog Analysis

### Parse Implementation Issues Index

Read `ImplementationIssuesIndex.md` and supporting record files and extract all active implementation issues:

- GitHub issue number
- Legacy deferment ID (if present)
- Issue type
- Description / title
- Created phase
- Scheduled for phase
- Creation date / updated date
- Current status
- Priority

### Determine Currently Active Phase

Use the phase analysis from Part 1 to determine which phase is currently active.

### Identify Aging Implementation Issues

Flag implementation issues that are:

**Category A — Overdue (Phase Mismatch):**
- Issue `scheduled_target` points to a phase that has already been completed
- Issue status remains `Open`, `Scheduled`, `In progress`, or `Blocked`
- These items were supposed to be addressed but were not resolved, archived, or explicitly re-targeted

**Category B — Stale (Age Threshold):**
- Issue has been open for more than 28 days with no meaningful status/target movement
- The issue has not moved to the archive
- Especially important for `Critical`, `High`, `Ambiguity / question`, or `Architecture concern` items

**Category C — Phase-Active But Unresolved:**
- Issue `scheduled_target` matches the currently active phase
- Not yet resolved
- These should be prioritized in the current implementation cycle

### Validate Backlog Integrity

Read `ImplementationIssuesArchive.md` and the issue record set to verify:

- No issue numbers appear in both the active index and archive summary
- Legacy deferment IDs, when present, do not map to multiple active issues
- Archived/resolved entries have resolution notes or status history

## Output

### Progress Dashboard Discussion

Create a discussion titled `[progress] Implementation Phase Dashboard — <DATE>` in the `general`
category:

```markdown
## Implementation Phase Progress Dashboard
*Updated: YYYY-MM-DD UTC*

| Phase | Status | Progress | Completion |
|-------|--------|----------|-----------|
| Phase 1 | ✅ Complete | 100% | Gate cleared |
| Phase 2 | 🔄 In Progress | 67% (45/67) | Active |
| Phase 3 | ⏳ Not Started | 0% | Waiting |

## Active Phase: Phase 2

**Current Step:** Step 4 — [step title]
**Blocking items:** [list of unchecked items in current step]

## Stalled Steps (no activity in 14+ days)
- Phase 2, Step 3: [step title] — last activity: YYYY-MM-DD
```

### Implementation Issues Backlog Report (only when aging items found)

If aging or overdue implementation issues are found, create an implementation issue titled
`[implementation-issues] Weekly Backlog Hygiene Report — <WEEK>`:

```markdown
## Implementation Issues Weekly Backlog Hygiene Report
*Week of YYYY-MM-DD*

### 🚨 Overdue (Scheduled phase already complete)
- #NNN: [description] — Scheduled for Phase N (now complete)

### ⚠️ Stale (> 28 days, not progressing)
- #NNN: [description] — Last updated YYYY-MM-DD (N days ago)

### 📋 Phase-Active (Scheduled for current phase, unresolved)
- #NNN: [description] — Prioritize in current phase

### Summary
- Total active implementation issues: N
- Resolved since last report: N
- Newly created since last report: N
```

If all implementation issues are within their expected window, skip the backlog issue (silence is success).

## Behavior Notes

- Runs weekly; no longer runs daily (previous `phase-progress-tracker` ran daily)
- The discussion is created in the `general` category; restrict this category to admins for
  admin-only visibility
- This workflow does NOT modify checklist files or implementation issue records
- This workflow should reference the Implementation Issues system as the canonical backlog source of truth
- When a hygiene issue is emitted, it should be treated as a `review-follow-up` implementation issue for triage rather than a standalone reporting artifact
- If all phases are 100% complete: post a celebratory notice in the discussion
- If no phases exist: skip (no output)
- Reference: `atomic-commit-execution-checklist-creation.instructions.md` for the full implementation issue reconciliation workflow
