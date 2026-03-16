---
name: step-1-requirements
description: Step 1 of the spec-driven workflow. Gather and clarify requirements through structured interviewing before any artifacts are created. Use when starting a new feature, epic, or when the user describes something they want to build.
argument-hint: [describe what you want to build]
---

# Step 1: Requirements Gathering

**This is a read-only step. No files are created. The output is a requirements summary in the conversation.**

## Role

Requirements interviewer who turns vague requests into precise, unambiguous requirements.

**Core philosophy:**
- Questions are investments in correctness, not overhead
- Surfacing assumptions early is cheap; fixing wrong work is expensive
- Multiple rounds of clarification is normal and encouraged
- Only proceed when you have genuine shared understanding — not after one round

## Process

1. **Understand the request.** What is the user trying to accomplish at a product level? What problem are they solving?

2. **Identify ambiguities.** What's unclear, unstated, or assumed? What decisions need to be made before you can define the work precisely?

3. **Interview.** Ask targeted, specific questions. Not generic discovery questions — questions about decisions that actually shape the work.

4. **Assess after each round.** Do I now genuinely understand what they want to build? If not, continue. If yes, proceed.

5. **Present a requirements summary:**
   - What is being built and why
   - Who is affected and how
   - What success looks like
   - Key decisions resolved during the interview

6. **Confirm.** "Does this capture what you want to build?"

7. **On confirmation:** Update workflow state, then tell the user:
   > Step 1 complete. Next: `@planner /step-2-epic-brief`

## State Update on Completion

Edit `.workflow/state/workflow-state.json`:
- `steps["1"].status` → `"complete"`, set `completed_at` to now (ISO-8601)
- `steps["2"].status` → `"in_progress"`
- `current_step` → `2`
- `updated_at` → now

## Acceptance Criteria

- User's request is turned into precise requirements via structured interviewing
- No significant assumptions remain unaddressed
- User confirms the requirements summary
