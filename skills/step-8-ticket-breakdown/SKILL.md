---
name: step-8-ticket-breakdown
description: Step 8 of the spec-driven workflow. Decompose validated specs into implementable, story-sized tickets with clear dependencies. Use after Cross-Artifact Validation passes.
argument-hint: [specific area to prioritize, or leave blank for all]
---

# Step 8: Ticket Breakdown

**Reads: All three spec artifacts**
**Produces: `.workflow/artifacts/tickets/TICKET-[N].md` files**

## Role

Technical lead decomposing validated specs into implementable, story-sized tickets with clear dependencies.

## Context to Read

Read `.workflow/artifacts/epic-brief.md`, `.workflow/artifacts/core-flows.md`, and `.workflow/artifacts/tech-plan.md` before starting.

## Process

1. **Review specs** and identify natural units of work.

2. **Apply best judgment** for breakdown:
   - Group by component or layer — not by individual function
   - Group by flow — not by step within a flow
   - Each ticket should be story-sized: meaningful work, not a single function
   - **Anti-pattern: Do NOT over-breakdown.** The minimal set of tickets beats many small ones
   - Identify what can be done in parallel vs. what must be sequential

3. **Draft tickets** using the template in [ticket-template.md](./ticket-template.md). Save each as `.workflow/artifacts/tickets/TICKET-[N].md`.

4. **Present the breakdown** with a Mermaid dependency diagram:
   ```mermaid
   graph TD
     T1[TICKET-1: Title] --> T2[TICKET-2: Title]
     T1 --> T3[TICKET-3: Title]
     T2 --> T4[TICKET-4: Title]
   ```

5. **Offer refinement:**
   - Change granularity (combine related work, or split for parallel execution)
   - Reorganize dependencies or implementation order
   - Different grouping approach (by component, by flow, etc.)

6. **Iterate** until the breakdown is right.

7. **Update state** and tell the user:
   > Step 8 complete. Next: `@orchestrator /step-9-execution`

## State Update on Completion

Edit `.workflow/state/workflow-state.json`:
- `steps["8"].status` → `"complete"`, set `completed_at`
- `steps["9"].status` → `"in_progress"`
- `current_step` → `9`
- `artifacts["tickets"].exists` → `true`, set `count` to number of tickets, set `last_modified`
- `updated_at` → now

## Acceptance Criteria

- Tickets cover all spec scope with no gaps
- Dependencies are clearly mapped
- Each ticket is independently implementable (given its dependencies)
- User confirms the breakdown
- All ticket files saved to `.workflow/artifacts/tickets/`
