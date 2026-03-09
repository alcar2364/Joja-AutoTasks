---
name: interactive-performance-optimizer
description: "Interactive workflow (via /optimize command) that analyzes performance issues and suggests optimizations."
on:
  issue_comment:
    types: [created]
permissions:
  contents: read
  issues: read
  pull-requests: read
  discussions: read
strict: true
network:
  allowed: [defaults, github]
engine:
  id: copilot
if: |
    contains(github.event.comment.body, '/optimize') &&
    (github.event.issue.pull_request || github.event_name == 'issue_comment')
---

# Interactive Performance Optimizer

On-demand performance analysis triggered by `/optimize` command on issues or PRs.

## Trigger

- `/optimize` — Analyze current PR/issue for performance opportunities
- Response: Investigation + recommendations via comment

## Analysis

1. **Code Performance:**
   - LINQ query efficiency (avoid N+1)
   - Collection allocations in hot paths
   - String concatenation vs. StringBuilder
   - Dictionary/set key selection

2. **Test Performance:**
   - Slow tests (>100ms)
   - Unnecessary setup/teardown
   - Data generation inefficiencies

3. **Build Performance:**
   - Unnecessary project references
   - Compilation bottlenecks

## Output

- Post detailed comment with findings
- Provide code snippets for suggested optimizations
- Link to validation approach
- If complex, create follow-up issue for deeper analysis

## Notes

- Not a complete solution; analysis + recommendations only
- When suggesting changes, include before/after performance expectations
- Reference performance contract from `.github/instructions/` if applicable
