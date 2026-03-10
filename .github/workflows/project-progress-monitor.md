---
name: project-progress-monitor
description: "Weekly monitor for implementation phase progress and aging deferred work items."
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
    title-prefix: "[deferments] "
    labels: [agentic-workflow, deferments, needs-review]
---

# Project Progress Monitor — Phase Dashboard & Deferments Aging Tracker

Weekly combined view of implementation phase progress and deferred work item health. This workflow
merges the responsibilities of the former `phase-progress-tracker` (daily) and
`deferments-monitor` (weekly) into a single weekly run.

## Context

- Repository: `${{ github.repository }}`
- Checklist directory: `.github/Project Tasks/Implementation Plan/`
- Checklist pattern: `Phase N - Atomic Commit Execution Checklist.md`
- Deferments index: `.github/Project Tasks/Implementation Plan/Deferments Index.md`
- Deferments archive: `.github/Project Tasks/Implementation Plan/Deferments Archive.md`
- Checkbox syntax: `- [x]` (complete), `- [ ]` (incomplete)
- Deferment ID format: `DEF-NNN`
- Deferment aging threshold: 28 days (4 weeks) from creation date

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

## Part 2: Deferments Aging Analysis

### Parse Deferments Index

Read `Deferments Index.md` and extract all active deferments:

- Deferment ID (DEF-NNN)
- Description / title
- Originally deferred in phase
- Scheduled for phase
- Creation date
- Current status

### Determine Currently Active Phase

Use the phase analysis from Part 1 to determine which phase is currently active.

### Identify Aging Deferments

Flag deferments that are:

**Category A — Overdue (Phase Mismatch):**
- Deferment's "Scheduled For" phase is a phase that has already been completed
- These items were supposed to be addressed but were not resolved or re-deferred

**Category B — Stale (Age Threshold):**
- Creation date is more than 28 days ago
- The deferment has not moved to the archive
- Not scheduled for a specific future phase (open-ended deferments)

**Category C — Phase-Active But Unresolved:**
- Deferment's "Scheduled For" phase matches the currently active phase
- Not yet resolved
- These should be prioritized in the current implementation cycle

### Validate Archive Integrity

Read `Deferments Archive.md` to verify:

- No deferment IDs appear in both the index and archive (data integrity check)
- All archive entries have a resolution date and phase

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

### Deferments Report (only when aging items found)

If aging or overdue deferments are found, create an issue titled
`[deferments] Weekly Aging Report — <WEEK>`:

```markdown
## Deferments Weekly Aging Report
*Week of YYYY-MM-DD*

### 🚨 Overdue (Scheduled phase already complete)
- DEF-NNN: [description] — Scheduled for Phase N (now complete)

### ⚠️ Stale (> 28 days, open-ended)
- DEF-NNN: [description] — Created YYYY-MM-DD (N days ago)

### 📋 Phase-Active (Scheduled for current phase, unresolved)
- DEF-NNN: [description] — Prioritize in current phase

### Summary
- Total active deferments: N
- Resolved since last report: N
- Newly created since last report: N
```

If all deferments are within their expected window, skip the deferments issue (silence is success).

## Behavior Notes

- Runs weekly; no longer runs daily (previous `phase-progress-tracker` ran daily)
- The discussion is created in the `general` category; restrict this category to admins for
  admin-only visibility
- This workflow does NOT modify checklist files or the deferments index/archive
- The deferments lifecycle is managed by the human maintainer during phase completion gates
- If all phases are 100% complete: post a celebratory notice in the discussion
- If no phases exist: skip (no output)
- Reference: `atomic-commit-execution-checklist-creation.instructions.md` for the full deferment workflow
