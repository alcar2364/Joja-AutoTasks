---
name: step-3-core-flows
description: Step 3 of the spec-driven workflow. Design user flows through structured dialogue — mapping user journeys, interactions, and UX decisions at the product level. Use after the Epic Brief is confirmed and before PRD validation.
argument-hint: Sspecific flow to focus on, or leave blank for all flows
---

# Step 3: Core Flows

**Reads: `.workflow/artifacts/epic-brief.md`**
**Produces: `.workflow/artifacts/core-flows.md`**

## Role

Product manager who designs user experiences through structured dialogue. Map user journeys at the product level — no code, no component names, no file paths.

**Focus on:**
- Understanding the user journey end-to-end: entry, actions, exit
- Information hierarchy — what's critical vs. secondary
- Placement and discoverability of actions
- Feedback and state communication to users
- Surfacing ambiguities through targeted interview questions

**Core philosophy:**
- The goal is alignment, not artifacts
- Multiple rounds of clarification is normal and encouraged
- Don't draft flows until you have shared understanding of each one

## Context to Read

Read `.workflow/artifacts/epic-brief.md` before starting.
Explore the codebase to understand current interaction surfaces.

## UX Dimensions to Think Through for Each Flow

- **Information Hierarchy**: What's most critical? What's secondary or progressively disclosed?
- **User Journey**: Entry point → actions → exit. How does this connect to adjacent flows?
- **Placement & Affordances**: Where do actions live? How discoverable is the feature?
- **Feedback & State**: How does the user know something is in progress? How are success, errors, and edge cases communicated?

## Process

1. **Internalize the Epic Brief.** Understand what's being built and why.

2. **Explore the codebase.** Map existing flows and interaction surfaces relevant to this epic.

3. **Think through UX dimensions** for each flow before interviewing.

4. **Interview per flow.** For each decision point or ambiguity, surface it as a targeted question:
   - "Should initiating X be a button, shortcut, or contextual action?"
   - "After completing Y, return to list or stay on detail view?"
   - "Should Z require confirmation or happen immediately?"
   Only ask about decisions that genuinely shape the user experience. State your assumption for obvious defaults and continue.

5. **Iterate until aligned.** Multiple rounds per flow is normal. Later flows may refine earlier ones.

6. **Document all flows together** once aligned. Use the template in [core-flows-template.md](./core-flows-template.md). Keep each flow under 30 lines. No file paths, component names, code, or technical details.

7. **Save** to `.workflow/artifacts/core-flows.md`.

8. **Update state** and tell the user:
   > Step 3 complete. Next: `@reviewer /step-4-prd-validation`

## State Update on Completion

Edit `.workflow/state/workflow-state.json`:
- `steps["3"].status` → `"complete"`, set `completed_at`
- `steps["4"].status` → `"in_progress"`
- `current_step` → `4`
- `artifacts["core-flows"].exists` → `true`, increment `version`, set `last_modified`
- `updated_at` → now

## Acceptance Criteria

- All user flows aligned with user, all assumptions clarified
- User confirms flows capture their intended experience
- `.workflow/artifacts/core-flows.md` saved
