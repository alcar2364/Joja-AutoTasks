---
name: revise-requirement
description: Trace and propagate a requirement change across all existing specs and tickets. Use at any point in the workflow when scope changes, requirements shift, or the user signals a pivot. Triggered by phrases like "actually let's change", "new requirement", "scope change", "we need to rethink".
argument-hint: [describe what changed and why]
---

# Revise Requirement

**Reads: All artifacts + all tickets**
**Produces: Targeted updates to affected specs and tickets**

## Role

Strategic planner who traces the ripple effects of change across an established plan.

**Focus on:**
- Understanding the full picture before touching anything
- Tracing how changes cascade through interconnected specs
- Making targeted, surgical updates — not rewriting from scratch
- Surfacing non-obvious downstream effects the user might not have considered

**Core philosophy:**
- Requirements change — the goal is to propagate them deliberately and completely
- Comprehensive impact analysis prevents half-updated specs that contradict each other
- Targeted updates preserve work already done
- Multiple rounds of clarification per spec is normal and encouraged

## Context to Read

Read all artifacts in `.workflow/artifacts/` and all tickets in `.workflow/artifacts/tickets/` before starting.

## Process

### 1. Internalize Current State
Read all existing specs and tickets. Build a mental model of how the pieces connect.

### 2. Understand the Change
Use interview questions to crystallize your understanding before touching anything:
- What specifically changed and why?
- What's the broader intention behind this change?
- What does the user think is affected?

Probe for the motivation — understanding "why" helps assess impact accurately. Don't proceed to impact analysis until the change is precisely understood.

### 3. Impact Analysis
Systematically trace the change's effects through each spec. For each spec, assess:
- Is this spec affected?
- Which specific sections need revision?
- Severity: minor tweak vs. significant rework?
- Preliminary thinking on how it should change?

Think through second-order implications:
- If a flow changes, does the tech plan's component architecture still support it?
- If a data model changes, do the flows that display that data still make sense?
- If scope shifts, are flows or technical decisions now unnecessary?

### 4. Present Impact Analysis
Concrete map for each affected spec:
- What's affected and why
- Severity of changes needed
- Preliminary proposal for how it should change

**Get user agreement on scope of changes before making any updates.**

### 5. Update Specs — Top-Down, One at a Time

Work through: **Epic Brief → Core Flows → Tech Plan**

For each spec:
1. **Think** — What specifically needs to change and what can stay?
2. **Interview** — Surface proposed changes as questions appropriate to the spec type. Iterate until shared understanding.
3. **Update** — Targeted changes only. Preserve what still holds.
4. **Verify consistency** — Check updated spec against already-updated specs before moving on.

**Spec-specific interview lenses:**

*Epic Brief lens:* Has the core problem shifted? Has scope expanded or contracted? New constraints to capture?

*Core Flows lens:* Do journeys remain coherent end-to-end? New flows needed or existing ones now unnecessary? Have interaction patterns changed? New error scenarios?

*Tech Plan lens:* Do key architectural choices still hold? Trace a request end-to-end through the revised architecture — does it hold? Data model additions/removals? Changed component interfaces?

### 6. Update Tickets
Once specs are updated, check all tickets for drift:
- Tickets referencing outdated decisions or superseded architecture
- Tickets for descoped work (mark obsolete)
- Missing tickets for new scope (create stubs)

### 7. Wrap Up
Confirm the updated specs reflect the intended changes. Summarize what changed across all specs and tickets. Tell the user:
> Revision complete. Earliest affected step is [N]. Resume with: `@[agent] /[skill]`

## State Update on Completion

Edit `.workflow/state/workflow-state.json`:
- Add entry to `revision_history`:
  ```json
  {
    "timestamp": "[ISO-8601]",
    "description": "[brief description of the change]",
    "affected_steps": ["3", "5"]
  }
  ```
- Set `current_step` to the earliest affected step number
- Set that step's `status` → `"in_progress"`
- Reset subsequent dependent steps to `"not_started"` where specs changed invalidate prior work
- `updated_at` → now

## Acceptance Criteria

- Requirement change crystallized through interview before any edits
- Impact analysis presented and agreed before updates begin
- All affected specs updated with targeted, consistent changes
- Updated specs don't contradict each other
- Tickets updated for drift
- Downstream re-planning path clearly communicated
