---
name: step-4-prd-validation
description: Step 4 of the spec-driven workflow. Validate that requirements are clear, complete, and actionable before technical planning begins. Quality gate between product definition and technical architecture. Use after Core Flows are confirmed.
argument-hint: (no arguments needed — reads epic-brief and core-flows automatically)
---

# Step 4: PRD Validation

**Reads: `.workflow/artifacts/epic-brief.md`, `.workflow/artifacts/core-flows.md`**
**Produces: Updates to epic-brief and core-flows (in place)**

## Role

Product quality advocate. Ensure requirements are clear, complete, and actionable before technical work begins.

**Focus on:**
- Evidence-based validation — cite specific sections when identifying issues
- Ensuring every requirement ties back to user value
- Clarity over completeness — clear requirements beat exhaustive ones
- Finding gaps together and fixing them through collaboration

**Core philosophy:**
- Finding ambiguity now is cheap; discovering it during implementation is expensive
- Gaps should be filled in the original specs, not documented separately
- Multiple rounds of clarification is normal and encouraged

## Context to Read

Read `.workflow/artifacts/epic-brief.md` and `.workflow/artifacts/core-flows.md` before starting.

## Validation Dimensions

### 1. Problem Definition & Context
- Is the problem clearly articulated?
- Is it clear who experiences this problem and why it matters?
- Is the scope appropriate — solving a real problem without over-reaching?
- Are success criteria defined?

### 2. User Experience Requirements
- Are primary user flows documented with clear entry and exit points?
- Are decision points and branches in flows identified?
- Are critical edge cases considered?
- Are error scenarios and recovery approaches outlined?
- Is the user journey coherent end-to-end?

### 3. Functional Requirements Quality
- Are requirements specific and unambiguous?
- Do requirements focus on WHAT (behavior), not HOW (implementation)?
- Is terminology consistent throughout?
- Can each requirement be tested/verified?

## Process

1. **Read and internalize** both artifacts.

2. **Evaluate each dimension** qualitatively — not "is this documented?" but "is this clear and actionable?"

3. **Interview for resolution.** Present findings as interview questions, most important first. Explain the area and why it matters, then ask focused questions.

4. **Update specs in place** as issues are resolved. Keep changes targeted.

5. **Confirm readiness** with the user.

## Routing on Completion

- **Gaps found and fixed → proceed:** Update state, tell user: `@planner /step-5-tech-plan`
- **Significant gaps requiring flow redesign → iterate:** Tell user: `@planner /step-3-core-flows` — explain what needs to be redesigned

## State Update on Passing

Edit `.workflow/state/workflow-state.json`:
- `steps["4"].status` → `"complete"`, set `completed_at`
- Increment `steps["4"].iterations` if previously completed
- `steps["5"].status` → `"in_progress"`
- `current_step` → `5`
- `updated_at` → now

## State Update on Iterating Back

- `steps["4"].iterations` → increment
- `steps["3"].status` → `"in_progress"`
- `current_step` → `3`
- `updated_at` → now

## Acceptance Criteria

- All focus areas evaluated against existing specs
- Gaps and ambiguities resolved through clarification
- Original documents updated with agreed changes
- User confirms updated specs are complete and accurate
