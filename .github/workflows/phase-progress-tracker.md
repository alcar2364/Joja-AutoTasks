---
name: phase-progress-tracker
description: "Tracks atomic commit checklist completion across all implementation phases and posts a progress dashboard."
on:
  schedule: daily
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
---

# Phase Progress Tracker — Implementation Checklist Dashboard

Parse all phase atomic commit execution checklists and publish a progress dashboard issue showing
completion percentage per phase, the current active phase, and any stalled or blocked steps.

## Context

- Repository: `${{ github.repository }}`
- Checklist directory: `.github/Project Tasks/Implementation Plan/`
- Checklist pattern: `Phase N - Atomic Commit Execution Checklist.md`
- Checkbox syntax: `- [x]` (complete), `- [ ]` (incomplete)

## Pre-Flight: Check for Recent Changes

Before running the full analysis, check whether any checklist files have changed since the last
workflow run:

1. Use the GitHub commits API to list commits on the default branch that touch
   `.github/Project Tasks/Implementation Plan/` in the last 24 hours.
2. If **no commits** have modified those paths in the last 24 hours, call `noop` and stop. Do
   not create or update any issue.
3. If at least one commit has modified those paths, continue with the Analysis Process below.

## Analysis Process

### 1. Discover Checklist Files

Find all files matching `Phase * - Atomic Commit Execution Checklist.md` in the implementation
plan directory, ordered by phase number ascending.

### 2. Parse Each Checklist

For each checklist file:

- Count total checkboxes (`- [ ]` + `- [x]`)
- Count completed checkboxes (`- [x]`)
- Calculate completion percentage: `completed / total * 100`
- Identify the Final Completion Gate section (last section)
- Determine phase status:
  - **Not Started:** 0% complete
  - **In Progress:** 1%–99% complete
  - **Complete:** 100% complete AND Final Completion Gate all checked

### 3. Identify Active Phase

The active phase is the highest-numbered phase with status "In Progress."
If no phase is In Progress, the active phase is the first "Not Started" phase.

### 4. Detect Stalled Steps

For each In Progress phase, identify Steps where:

- The step header exists (e.g., `## 2) Bootstrap Logging Foundation ##`)
- No substep under that step has been completed in the last 7 days (based on commit history)
- This indicates a potentially stalled step

## Output: Progress Dashboard Discussion

Create a discussion titled `[progress] Implementation Phase Dashboard — <DATE>` in the `general`
category (configure this category as admin-only in repository discussion settings to restrict
visibility to admins):

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

## Stalled Steps (no activity in 7+ days)
- Phase 2, Step 3: [step title] — last activity: YYYY-MM-DD

## Deferments Snapshot
- Active deferments: N (from Deferments Index.md)
- Resolved this week: N
```

## Update Behavior

- Creates a new discussion each time; older `[progress]` discussions expire naturally
- If all phases are 100% complete: post a celebratory notice
- If no phases exist: skip (no output)

## Notes

- This workflow is purely observational — it does NOT modify checklist files
- The discussion is created in the `general` category; restrict this category to admins in
  repository discussion settings to achieve admin-only visibility
- Commit-history analysis for stalled steps requires `contents: read` on git API
- Phase ordering is determined by the numeric value in the phase filename, not alphabetical
