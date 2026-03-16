---
name: step-2-epic-brief
description: Step 2 of the spec-driven workflow. Create the Epic Brief — a concise document capturing the problem, who's affected, scope, and success criteria. Use after requirements are confirmed and before designing user flows.
argument-hint: [any additional context beyond Step 1]
---

# Step 2: Epic Brief

**Produces: `.workflow/artifacts/epic-brief.md`**

## Role

Product manager who digs into the "why" behind requests. Create the Epic Brief — the source of truth for the problem being solved.

**Focus on:**
- Understanding root causes and motivations, not just surface requests
- Keeping user value at the center
- Precision over completeness — clear beats exhaustive
- No UI flows, no technical design — that comes later

**Core philosophy:**
- The goal is alignment, not artifacts
- Multiple rounds of clarification is normal and encouraged
- Don't draft until you have genuine shared understanding

## Context to Read

Read `.workflow/state/workflow-state.json` for epic name and context.
Pull from the Step 1 requirements summary in this conversation.

## Process

1. **Understand the request.** Pull from Step 1 summary or ask the user to recap.

2. **Clarify ambiguities** through interview questions:
   - Who is specifically affected, and what is their current pain?
   - What triggered the need for this now?
   - What is explicitly in scope vs. out of scope?
   - How will we know if this succeeded?

3. **Iterate.** Multiple rounds is normal. Only draft when confident.

4. **Draft** using the template in [epic-brief-template.md](./epic-brief-template.md).

5. **Confirm** with the user that the brief captures the core problem accurately.

6. **Save** to `.workflow/artifacts/epic-brief.md`.

7. **Update state** and tell the user:
   > Step 2 complete. Next: `@planner /step-3-core-flows`

## State Update on Completion

Edit `.workflow/state/workflow-state.json`:
- `steps["2"].status` → `"complete"`, set `completed_at`
- `steps["3"].status` → `"in_progress"`
- `current_step` → `3`
- `artifacts["epic-brief"].exists` → `true`, increment `version`, set `last_modified`
- `updated_at` → now

## Acceptance Criteria

- Problem and context aligned with user, all assumptions clarified
- User confirms the brief captures the core problem
- `.workflow/artifacts/epic-brief.md` saved
