---
name: deferments-monitor
description: "Weekly monitor for aging deferred work items in the JAT implementation plan deferments index."
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
  create-issue:
    title-prefix: "[deferments] "
    labels: [agentic-workflow, deferments, needs-review]
---

# Deferments Monitor — Aging Deferred Work Item Tracker

Periodically review the deferments index to surface items that have aged beyond their expected
resolution window or that belong to the currently active implementation phase but remain unresolved.

## Context

- Repository: `${{ github.repository }}`
- Deferments index: `.github/Project Tasks/Implementation Plan/Deferments Index.md`
- Deferments archive: `.github/Project Tasks/Implementation Plan/Deferments Archive.md`
- Phase checklists: `.github/Project Tasks/Implementation Plan/Phase N - Atomic Commit Execution Checklist.md`
- Deferment ID format: `DEF-NNN`
- Aging threshold: 28 days (4 weeks) from the deferment creation date

## Analysis Process

### 1. Parse Deferments Index

Read `Deferments Index.md` and extract all active deferments:

- Deferment ID (DEF-NNN)
- Description / title
- Originally deferred in phase
- Scheduled for phase
- Creation date
- Current status

### 2. Determine Currently Active Phase

Cross-reference the phase progress tracker output (or independently parse the checklist files) to
determine which phase is currently in progress.

### 3. Identify Aging Deferments

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

### 4. Compare with Archive

Read `Deferments Archive.md` to verify:

- No deferment IDs appear in both the index and archive (data integrity check)
- All archive entries have a resolution date and phase

## Output

Create a weekly issue titled `[deferments] Weekly Aging Report — <WEEK>`:

```markdown
## Deferments Weekly Aging Report
*Week of YYYY-MM-DD*

### 🚨 Overdue (Scheduled phase already complete)
- DEF-NNN: [description] — Scheduled for Phase N (now complete)

### ⚠️ Stale (> 28 days, open-ended)
- DEF-NNN: [description] — Created YYYY-MM-DD (N days ago)

### 📋 Phase-Active (Scheduled for current phase, unresolved)
- DEF-NNN: [description] — Prioritize in current phase

### ✅ Clean
All other deferments are within their expected window.

### Summary
- Total active deferments: N
- Resolved this week: N (moved to archive)
- Newly created this week: N
```

## Notes

- Issue format keeps this advisory rather than blocking
- If all deferments are clean (no aging or overdue items), skip the issue post (silence is success)
- This workflow does NOT modify the deferments index or archive
- The Deferments lifecycle is managed by the human maintainer during phase completion gates
- Reference: `atomic-commit-execution-checklist-creation.instructions.md` for the full deferment workflow
