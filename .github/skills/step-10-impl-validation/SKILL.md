---
name: step-10-impl-validation
description: Step 10 of the spec-driven workflow. Verify implementation matches specs and works correctly — the final quality gate. Use after execution is complete to validate the full implementation or specific tickets.
argument-hint: Ticket number(s) to validate, or leave blank for full implementation
---

# Step 10: Implementation Validation

**Reads: All specs + all tickets + code changes**
**Produces: Validation report, ticket status updates, bug tickets if needed**

## Role

Careful reviewer who checks if what was built matches what was planned, and if it works correctly.

**Focus on:**
- Evidence over assumption — cite specific code and spec references
- Advisory, not authoritative — present findings, let user decide actions
- Severity matters — distinguish blockers from minor observations
- Practical focus — catch real issues, not pedantic nitpicks

This is **not** a generic code review. It's a focused check against planned work.

**Core philosophy:**
Two questions only:
1. **Alignment** — Does the code match what was planned in the specs?
2. **Correctness** — Does the code actually work? Are there bugs or gaps?

## Context to Read

Read all spec artifacts and all tickets in `.workflow/artifacts/tickets/`. Use `git diff` or review ticket-referenced files to understand what changed.

## Process

### 1. Identify Scope
Specific ticket(s) or full implementation.

### 2. Alignment Analysis
Compare implementation against specs:
- Requirements from tickets implemented?
- Architecture follows Tech Plan?
- Acceptance criteria met?
- Any deviations? (Note: deviations may be justified)

### 3. Correctness Analysis
Review for:
- 🚫 **Blockers**: Broken functionality, major spec deviations, security concerns, data corruption risks — must fix before completion
- 🐛 **Bugs**: Logic errors, incorrect behavior, broken flows — should fix
- ⚠️ **Edge Cases**: Unhandled scenarios, missing validations, boundary conditions — clarify and decide
- 💡 **Observations**: Minor concerns, potential improvements — note for awareness
- ✅ **Validated**: Explicitly confirm what's working correctly

### 4. Present Findings and Ask for Direction
In a single response:
- Present findings organized by importance (blockers first)
- Summarize what's working correctly
- **Update passing tickets** to done status (no confirmation needed for clear passes)
- Ask for direction on issues: Which become bug tickets? Which are intentional deviations to document? Which can be deferred?

### 5. Execute Based on Direction
- Create bug tickets in `.workflow/artifacts/tickets/` for issues needing tracking
- Add notes to existing tickets for observations
- Document accepted deviations in the relevant spec artifact

### 6. Confirm Completion
Summarize: validated, complete, needing follow-up, and any accepted trade-offs.

## Routing on Completion

- **Validation passes → Epic complete!** Update state, congratulate
- **Issues found → re-execute:** Tell user: `@orchestrator /step-9-execution` — specify which tickets need fixes

## State Update on Passing

Edit `.workflow/state/workflow-state.json`:
- `steps["10"].status` → `"complete"`, set `completed_at`
- Increment `steps["10"].iterations` if previously completed
- `updated_at` → now

## State Update on Re-executing

- `steps["10"].iterations` → increment
- `steps["9"].status` → `"in_progress"`
- `current_step` → `9`
- `updated_at` → now

## Acceptance Criteria

- All implementation validated against specs and for correctness
- Findings are specific and actionable with code references
- User has guided how to handle each issue category
- All tickets either marked Done or have follow-up tracked
