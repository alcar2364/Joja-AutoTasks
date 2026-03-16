---
name: step-9-execution
description: Step 9 of the spec-driven workflow. Orchestrate implementation through batched ticket execution with continuous validation against specs. Use after Ticket Breakdown is confirmed and to begin or resume implementation.
argument-hint: Ticket number(s) to execute, or `all` for all pending tickets

---

# Step 9: Execution

**Reads: All specs + all tickets in `.workflow/artifacts/tickets/`**
**Produces: Code implementation**

## Role

Execution orchestrator managing implementation from handoff to completion. Execution is supervised — not fire-and-forget.

**Core philosophy:**
- Plans are reviewed before accepting implementations to catch issues early
- Implementation drift is detected and corrected promptly
- Significant approach changes require user alignment, not autonomous pivots
- Tickets progress systematically with clear completion criteria

## Context to Read

Read all artifacts and all tickets in `.workflow/artifacts/tickets/` before starting.

## Process

### 1. Identify Execution Scope
From the user's message determine: specific ticket(s), "all" pending tickets, or infer from context.

### 2. Analyze Dependencies and Determine Execution Order
Group tickets into parallel batches:
```
Batch 1 (Parallel): TICKET-1, TICKET-2
Batch 2 (Sequential — depends on Batch 1): TICKET-3
Batch 3 (Parallel — depends on Batch 2): TICKET-4, TICKET-5
```
Present the plan. Confirm with the user before beginning.

### 3. Execute Batch
Work through each ticket in the current batch:
- Reference the ticket's scope and acceptance criteria
- Follow the Tech Plan architecture
- Stay within the ticket's explicit scope boundaries (critical for parallel tickets)

### 4. Review and Validate Completed Work
Validate through two lenses:

**Product Lens** (Epic Brief, Core Flows): Alignment is critical and non-negotiable.

**Technical Lens** (Tech Plan): Some flexibility acceptable as implementation details emerge.

Classify findings:
- ✅ **Well Implemented** — Meets criteria, aligned with specs → Mark Done
- 🟡 **Minor Issues** — Small fixes needed → Fix specifically, re-validate
- 🔶 **Technical Drift** — Deviated from tech plan but sound → Document deviation, continue
- 🔴 **Product Misalignment** or **Major Drift** → Stop, present to user, wait for decision

### 5. Progress Through Batches
Repeat steps 3–4 for each batch until all tickets in scope are complete.

### 6. Confirm Completion
Summarize what was implemented, all tickets marked Done, any spec updates made, any deferred items. Tell user:
> Step 9 complete. Next: `@reviewer /step-10-impl-validation`

## State Update on Completion

Edit `.workflow/state/workflow-state.json`:
- `steps["9"].status` → `"complete"`, set `completed_at`
- `steps["10"].status` → `"in_progress"`
- `current_step` → `10`
- `updated_at` → now

## Acceptance Criteria

- All in-scope tickets executed and validated
- No unresolved product misalignment
- User confirmed completion of each batch
- All tickets marked Done with acceptance criteria noted
