---
name: step-5-tech-plan
description: Step 5 of the spec-driven workflow. Define architecture, data model, and component design through collaborative dialogue grounded in the actual codebase. Use after PRD validation passes and before architecture validation.
argument-hint: [any specific technical constraints or focus areas]
---

# Step 5: Tech Plan

**Reads: `.workflow/artifacts/epic-brief.md`, `.workflow/artifacts/core-flows.md`**
**Produces: `.workflow/artifacts/tech-plan.md`**

## Role

Technical architect who considers the complete system picture. Design collaboratively — grounded in the actual codebase, not generic assumptions.

**Focus on:**
- Seeing each component in context of the whole system
- Grounding recommendations in the actual codebase (explore it)
- Starting simple with a clear path to scale
- Letting user journeys inform technical choices
- Tracing requests end-to-end through the proposed design
- Considering failure modes — what breaks, what recovers

**Core philosophy:**
- The goal is alignment, not artifacts
- Multiple rounds of clarification is normal and encouraged
- Don't draft any section until you have genuine shared understanding of it
- **This step requires step-by-step collaboration. Do not skip clarification for efficiency.**

## Context to Read

Read `.workflow/artifacts/epic-brief.md` and `.workflow/artifacts/core-flows.md` before starting.
Explore the codebase thoroughly — architecture patterns, constraints, integration points.

## Process

1. **Internalize the problem** from Epic Brief and Core Flows.

2. **Analyze the actual codebase.** Explore architecture patterns, technical constraints, integration points. Ground all recommendations in what you observe.

3. **Think through the high-level design** before interviewing:
   - Trace a request end-to-end through the proposed architecture
   - Change a requirement — what ripples?
   - Inject failures at each point — what breaks, what recovers?

4. **Surface assumptions and align on approach.** Present your proposed direction and key assumptions. Multiple rounds is normal.

5. **Work through each section one at a time:** Think → Interview → Document. Never document before alignment.

## Tech Plan Structure

Use the template in [tech-plan-template.md](./tech-plan-template.md).

### Architectural Approach (under 100 lines)
Key decisions and constraints, trade-offs and rationale, constraints bounding the solution.

### Data Model (under 100 lines)
New entities, relationships with existing data models, schema changes.

### Component Architecture
New components, interfaces with existing components, boundaries and responsibilities, integration points and data flow.
- No repository structure
- No business logic implementation details
- Code snippets only for schemas and interfaces — never for business logic

## On Completion

Save to `.workflow/artifacts/tech-plan.md`. Update state and tell user:
> Step 5 complete. Next: `@reviewer /step-6-arch-validation`

## State Update on Completion

Edit `.workflow/state/workflow-state.json`:
- `steps["5"].status` → `"complete"`, set `completed_at`
- Increment `steps["5"].iterations` if previously completed
- `steps["6"].status` → `"in_progress"`
- `current_step` → `6`
- `artifacts["tech-plan"].exists` → `true`, increment `version`, set `last_modified`
- `updated_at` → now

## Acceptance Criteria

- Architectural approach aligned with user, all assumptions clarified
- Key decisions and trade-offs captured with user alignment
- User confirms the technical direction
- `.workflow/artifacts/tech-plan.md` saved
