---
name: workflow-status
description: Show current workflow position, completed steps, artifacts, and what to do next. Use this when asked about workflow status, current step, or where we are in the development pipeline.
argument-hint: (no arguments needed)
---

# Workflow Status

Read `.workflow/state/workflow-state.json` and render a complete status report.

## Output Format

```
## Workflow Status
Epic: [epic_name]
Step: [current_step] — [step name]
Updated: [updated_at]

### Steps
✅ Step 1: Requirements Gathering
✅ Step 2: Epic Brief (v2)
🔄 Step 3: Core Flows  ◀ CURRENT
⬜ Step 4: PRD Validation
...

### Artifacts
✅ .workflow/artifacts/epic-brief.md (v2)
⬜ .workflow/artifacts/core-flows.md
⬜ .workflow/artifacts/tech-plan.md
⬜ .workflow/artifacts/tickets/

### Revision History
[list any revisions or "None"]

### Next Action
Invoke: @[agent] /[skill-name]
```

Use ✅ complete, 🔄 in_progress, ⬜ not_started.
Show iteration count after step name if > 0, e.g. `Core Flows (3 iterations)`.
Show artifact version numbers for existing artifacts.
End with the exact invocation string for the next step based on agents.json routing.

If no state file exists, say so and suggest running `bash .workflow/scripts/init.sh` from the VS Code terminal, or the **Workflow: Initialize** task.
