---
name: step-7-cross-artifact
description: Step 7 of the spec-driven workflow. Validate consistency across all artifact boundaries — the final quality gate before tickets are created. Use after Architecture Validation passes.
---

# Step 7: Cross-Artifact Validation

**Reads: All artifacts — epic-brief, core-flows, tech-plan, and any existing tickets**
**Produces: Updates to any spec (in place), ticket reconciliation if tickets exist**

## Role

Reviewer who validates consistency across artifact boundaries — the seams where specs connect, and where tickets derive from specs.

**Focus on:**
- Cross-cutting analysis — how specs relate to each other, not internal quality of individual specs
- The joints between specs, not re-reviewing their internals
- Grounding findings in specific references — cite which spec says what
- Calibrating depth to the significance of the finding

**Core philosophy:**
- This answers one question: "Are the artifacts in a state we can confidently act on?"
- Effort is front-loaded in analysis — read deeply, cross-reference thoroughly, form conclusions, then present

## Context to Read

Read all artifacts in `.workflow/artifacts/` before starting, including any existing tickets in `.workflow/artifacts/tickets/`.

## Analysis Dimensions

1. **Conceptual Consistency** — Same concepts, entities, and terms described compatibly across all specs. Watch for terminology drift and contradictory characterizations.

2. **Coverage Traceability** — Bidirectional: requirements in the Brief should have flows and technical support. Tech decisions should trace back to a requirement. Orphans in either direction are findings.

3. **Interface Alignment** — Where specs meet, they should agree on the contract. Data that flows reference should exist in the data model. Interactions in flows should have corresponding components in the Tech Plan.

4. **Specificity** — Identify areas where a downstream implementation agent would be forced to make a design decision because the spec hand-waves.

5. **Assumption Coherence** — Constraints and assumptions in one spec shouldn't contradict decisions in another.

## Process

1. **Internalize all artifacts.** Build a mental model of how the specs connect.

2. **Cross-referential analysis** across all five dimensions. Use existing tickets as additional signal for drift.

3. **Present findings:**
   - Lead with overall assessment — do the specs tell one coherent story?
   - Walk through significant findings, most important first, with specific spec citations
   - Group minor fixes (naming drift, trivial wording) as a batch for user approval

4. **Update specs** based on resolutions. Surgical updates only. After updating one spec, verify it doesn't introduce new inconsistencies elsewhere.

5. **Ticket reconciliation** (if tickets exist in `.workflow/artifacts/tickets/`):
   - Tickets with stale decisions, superseded architecture, or descoped scope
   - Missing tickets for new spec scope
   - If drift is extensive, recommend re-running `@planner /step-8-ticket-breakdown` rather than patching incrementally

6. **Suggest next step** based on outcome.

## Routing on Completion

- **Cross-validation passes → proceed to tickets:** Update state, tell user: `@planner /step-8-ticket-breakdown`
- **Product gaps found → iterate flows:** Tell user: `@planner /step-3-core-flows`
- **Tech gaps found → iterate tech plan:** Tell user: `@planner /step-5-tech-plan`

## State Update on Passing

Edit `.workflow/state/workflow-state.json`:
- `steps["7"].status` → `"complete"`, set `completed_at`
- Increment `steps["7"].iterations` if previously completed
- `steps["8"].status` → `"in_progress"`
- `current_step` → `8`
- `updated_at` → now

## Acceptance Criteria

- Cross-spec consistency evaluated across all analysis dimensions
- Findings requiring user judgment resolved through clarification
- Minor fixes approved and applied
- Specs tell one coherent story
- Existing tickets reconciled against updated specs (if applicable)
