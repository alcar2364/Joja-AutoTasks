---
name: atomic-commit-execution-checklist-creation
description: "Create an atomic commit execution checklist from a design guide phase specification. Use when: creating a step-by-step checklist to guide implementation of a specific phase or feature with minimal atomic commits and design-guide compliance."
applyTo: "**/*Atomic Commit Execution Checklist*.md"
---

# Atomic Commit Execution Checklist Creation

## Purpose

An **atomic commit execution checklist** is a detailed, implementation-ready plan that breaks down a design phase into small, individually-testable commits. Its goal is to:

- Guide developers (human or AI) through a phase incrementally
- Ensure each substep represents a minimal, scope-respecting change
- Catch architectural drift early via explicit verification steps and test coverage
- Produce a working checkpoint at the end of the phase
- Remain portable and reusable across phases and projects

This instruction file describes how to create such a checklist.

## When to Create

Create an atomic commit execution checklist when:

1. A design phase or feature is fully specified in a design guide or user request
2. You need a step-by-step breakdown to guide implementation (human or AI)
3. You want to ensure commits are minimal and scope-respecting
4. You want explicit test coverage and verification steps before moving to the next phase

Do NOT create a checklist for:

- Exploratory work or research tasks
- Ad-hoc bug fixes (use a smaller task list instead)
- Phases already executed and documented

## Planning Phase Workflow

To create a checklist, follow this multi-agent workflow:

### 1. Orchestrator (Intake)

The user requests a checklist for a specific design phase or feature.

**Input from user:**

- Reference to the design guide phase or feature specification
- Scope of implementation (what problem/feature is being addressed)
- Any constraints or previous phase context

**Orchestrator delegates to Researcher.**

### 2. Researcher (Context Discovery)

The Researcher gathers:

- The full design guide phase specification (architecture, responsibilities, dependencies)
- Any relevant previous checklists for earlier phases (to avoid overlap and ensure continuity)
- Design contracts and constraints relevant to the phase
- Any existing code/structure that impacts scope
- External references (e.g., SMAPI API docs, third-party libraries)
- **Active implementation issues from `Project/Tasks/ImplementationPlan/ImplementationIssues/ImplementationIssuesIndex.md`** that are scheduled for or relevant to the target phase

**Output:** Research summary with citations, identifying:

- Key subsystems and their responsibilities
- Atomic commit granularity (what fits in one commit)
- Dependencies between steps
- Known constraints and guardrails
- Gaps or ambiguities requiring clarification
- **Relevant active implementation issues** (especially `Deferment`, `Open issue`, `Ambiguity / question`, `Architecture concern`, and `Review follow-up`) scheduled for the target phase or marked open with applicable scope

**Researcher delegates to Planner.**

### 3. Planner (Structure Design)

The Planner creates a **step-by-step breakdown** of the phase:

**Output structure:**

- **Guardrails section** — principles/constraints that must remain true throughout the phase (not checkboxes during planning)
- **Steps (N)** — major milestones (e.g., "Bootstrap SMAPI Skeleton", "Add Logging Foundation")
- **Phase Overview section** — architectural context for the phase (goal, components, prerequisites, relationships, and design guide citations)
- **Substeps (NA, NB, NC, ...)** — granular, atomic commits (one per substep)
- **Unit Tests section** — tests that lock constraints and catch drift
- **Final Review section** — verification checkpoints before phase completion
- **Implementation-issue-related substeps or explicit re-deferral** — for active implementation issues scheduled to this phase

Planner does NOT draft the final Markdown format. Planner structures the plan so Reviewer can validate it and WorkspaceAgent can draft it.

**Key planning decisions:**

- What is the commit granularity? (One file? One feature? One boundary?)
- What are the dependencies between substeps?
- How many explicit test substeps are needed?
- What guardrails must be preserved?
- **Which active implementation issues (from Researcher findings) are resolved in-scope vs. explicitly re-deferred or left open with rationale?**

**Reviewer validates the plan before WorkspaceAgent drafts.**

### 4. Reviewer (Plan Validation)

The Reviewer checks the Planner's structured plan against:

**First priority: Design guide alignment**

- Does the plan cover all responsibilities specified in the design phase?
- Is the plan a faithful decomposition of the design guide phase?
- Are there sub-phases or dependencies missing?

**Second priority: Workspace contracts and standards**

- Do guardrails match the design phase constraints?
- Is the step structure clear and followable?
- Are unit test substeps comprehensive?
- Is the final review section sufficient for verification?
- Can both humans and AI agents follow the instructions?

**Reviewer output:**

- Approval or specific fixes needed in the plan structure
- Any guardrails or test coverage gaps to add
- Clarity issues (ambiguous action descriptions, missing verification criteria)

**After approval, Reviewer delegates to WorkspaceAgent.**

### 5. WorkspaceAgent (Checklist Drafting)

The WorkspaceAgent **creates the final Markdown checklist file directly** from the Reviewer-approved plan.

**Critical:** WorkspaceAgent must **use the createFile tool** to write the checklist immediately. Do NOT draft content and hand back to Orchestrator. WorkspaceAgent's default mode is direct editing for documentation artifacts.

**Output:** A complete checklist file written to the target location (e.g., `Project/Tasks/Implementation Plan/Phase N - Atomic Commit Execution Checklist.md`).

## Checklist Structure Requirements

### Guardrails Section

**Purpose:** Define principles/constraints that must remain true throughout implementation.

**Format:** Checkbox list (unchecked during planning, checked during final review if agent/human reviews)

**Example:**

```
## Guardrails (Must Stay True) ##

* [ ] Phase 1 only (no Phase 2 features).
* [ ] No task/store mutation logic.
* [ ] OnSaving is signal-only with no writes.
```

**Key point:** Guardrails are NOT execution checkboxes. They are design principles to preserve. They are checked OFF only during final review after coding is complete.

### Phase Overview

**Purpose:** Provide architectural context for this implementation phase.

**Required content:**

- [ ] **Phase Goal**: One-paragraph summary of what this phase delivers and why
- [ ] **Architecture Components**: List of major systems/classes/boundaries being implemented
- [ ] **Prerequisites**: What prior phases must be complete and what they provide
- [ ] **Architecture Relationships**: How this phase's components relate to:
  - Prior phases (what they build on)
  - Future phases (what foundation they provide)
  - Overall system architecture (where they fit)
- [ ] **Design Guide References**: Citations to relevant design guide sections

### Steps (Major Milestones)

**Format:** `## N) [Step Title] ##`

**Each step must include:**

1. **Step goal** — One-sentence purpose of the step

   ```
   * [ ] [Goal description]
   ```

2. **Substeps** — Implementation checkpoints (see below)

3. **Step completion checkpoint** — After all substeps (NA, NB, NC, etc.) are done
   ```
   * [ ] All substeps in Step N complete
   ```

**Example:**

```
## 1) Bootstrap SMAPI Skeleton ##

Step goal:

* [ ] Create a minimal compile-safe SMAPI entry shell.

### 1A - Add baseline entry shell ###
[substep details]

### 1B - Align project and manifest baseline ###
[substep details]

### 1C - Register minimal lifecycle hook stubs ###
[substep details]

## Step 1 Completion ##

* [ ] All substeps in Step 1 complete (1A, 1B, 1C).
```

### Substeps (Implementation Checkpoints)

**Format:** `### NA - [Substep title] ###`

**Each substep must include:**

1. **Action** — What to do (clear, concise)

   ```
   * [ ] Action: [description of change]
   ```

2. **Scope** — Files and symbols affected (explicit boundaries)

   ```
   * [ ] Scope: [file paths and/or symbol names]
   ```

3. **Verify** — How to test that the change worked

   ```
   * [ ] Verify: [testable criterion]
   ```

4. **Suggested commit** — Workflow reminder (optional, not enforced)

   ```
   * [ ] Suggested commit: `[suggested message]`
   ```

   **Important:** This is a workflow reminder, not a verification requirement. Committing after each substep is recommended for progress tracking and easier rollback, but is not checked during phase completion. Developers may use different commit messages as long as work is traceable.

5. **Must include** — What the change must add/change

   ```
   * [ ] Must include: [list of requirements]
   ```

6. **Must exclude** — What to avoid or defer
   ```
   * [ ] Must exclude: [list of exclusions]
   ```

**Example:**

```
### 1A - Add baseline entry shell ###

* [ ] Action: add `ModEntry : Mod` and minimal `Entry(IModHelper helper)` startup wiring.
* [ ] Scope: `src/JojaAutoTasks/ModEntry.cs` (`ModEntry`, `Entry`).
* [ ] Verify: build succeeds and one startup log appears.
* [ ] Suggested commit: `phase1(step1A): add minimal ModEntry shell`
* [ ] Must include: baseline `ModEntry` and minimal startup code.
* [ ] Must exclude: config/lifecycle/dispatcher internals.
```

### Unit Tests Section

**Purpose:** Define tests that lock phase constraints and catch drift.

**Format:** Steps N-X (embedded as normal steps) with unit test substeps

**Key design rule:** Unit test substeps must be **executable by both human and AI agents**.

**Guidance for clarity:**

- Use precise test names and target files (e.g., `ConfigLoaderTests.cs`)
- State what assertions or checks are needed (methods, coverage areas)
- Keep language implementation-agnostic (e.g., "add tests for X" not "use xUnit with Moq")
- Allow flexibility in test framework/style as long as constraints are met

**Example:**

```
## 7) Add Phase 1 Verification Tests ##

Step goal:

* [ ] Add tests that lock Phase 1 constraints and catch drift.

### 7A - Add config tests ###

* [ ] Action: add tests for defaulting/validation and ConfigVersion handling.
* [ ] Scope: `tests/JojaAutoTasks.Tests/Configuration/ConfigLoaderTests.cs`.
* [ ] Verify: tests compile and fail if ConfigVersion behavior regresses.
* [ ] Suggested commit: `phase1(step7A): add ConfigLoader tests with ConfigVersion coverage`
* [ ] Must include: default value tests, validation edge cases, ConfigVersion round-trip tests.
* [ ] Must exclude: production logic changes.

### 7B - Add lifecycle tests ###

* [ ] Action: add signal flow tests including assertions that OnSaving writes no state.
* [ ] Scope: `tests/JojaAutoTasks.Tests/Lifecycle/LifecycleCoordinatorTests.cs`.
* [ ] Verify: tests fail if OnSaving writes or checkpoints are introduced.
* [ ] Suggested commit: `phase1(step7B): add lifecycle tests for signal-only OnSaving`
* [ ] Must include: signal sequencing tests, OnSaving read-only assertions, required test doubles/mocks.
* [ ] Must exclude: dispatcher-specific tests and production refactors.
```

### Final Review Section

**Purpose:** Verification checkpoints once implementation and all tests are complete.

**Format:** Final step (`## N) Phase Completion Gate ##`) with review substeps

**Key design rule:** Review substeps must be **executable by both human and AI agents (Reviewer)**.

**Guidance for clarity:**

- State what to check (e.g., "verify build succeeds")
- State what artifact to produce or update (checklist, audit notes)
- Specify acceptance criteria (all tests pass, no scope drift)
- Allow flexibility in how the check is performed (automated vs. manual audit)
- If scope/architecture issues are found during completion review, require a
  post-phase implementation review report and a user-owned post-phase
  implementation atomic execution checklist instead of inline implementation fixes

**Example:**

```
## 8) Phase 1 Completion Gate ##

Step goal:

* [ ] Confirm Phase 1 is complete, atomic, and contract-aligned.

### 8A - Run build and full tests ###

* [ ] Action: run clean build and full test suite.
* [ ] Scope: no source edits expected.
* [ ] Verify: build succeeds without warnings and all Phase 1 tests pass.
* [ ] Suggested commit: `phase1(step8A): record build and test completion evidence`
* [ ] Must include: build log evidence (if persisting) or test output confirmation.
* [ ] Must exclude: opportunistic code edits.

### 8B - Audit guardrails ###

* [ ] Action: review implementation against each guardrail from the start of this checklist.
* [ ] Scope: this checklist file and the implemented code.
* [ ] Verify: each guardrail is preserved (e.g., no task mutation, OnSaving is signal-only).
* [ ] Suggested commit: `phase1(step8B): confirm guardrails preserved in Phase 1`
* [ ] Must include: explicit check against each guardrail.
* [ ] Must exclude: design guide rewrites or Phase 2 work.

### 8C - Validate implementation scope ###

* [ ] Action: review implementation to ensure no unintended scope expansion beyond phase requirements.
* [ ] Scope: review changed files and symbols from phase start to completion.
* [ ] Verify: no changes outside documented phase scope; architecture boundaries preserved.
* [ ] Suggested commit: `phase1(step8C): finalize scope validation audit`
* [ ] Must include: scope validation notes.
* [ ] Must exclude: rewriting code unrelated to phase requirements.

### 8D - Reconcile implementation issues ###

* [ ] Action: review checklist for newly identified implementation issues and reconcile with `Project/Tasks/ImplementationPlan/ImplementationIssues/ImplementationIssuesIndex.md` and the corresponding issue records under `ImplementationIssues/Records/`.
* [ ] Scope: this checklist file, `ImplementationIssuesIndex.md`, `ImplementationIssuesArchive.md`, and affected record files under `ImplementationIssues/Records/`.
* [ ] Verify: newly identified items are captured as implementation issue records with correct type/priority/source/scheduled target; resolved items are updated so the archive view reflects completion evidence and date.
* [ ] Suggested commit: `phase1(step8D): reconcile implementation issues after Phase 1 completion`
* [ ] Must include: any new implementation issue records needed for review findings or deferments; status/history updates for resolved items; phase/date/resolution notes.
* [ ] Must exclude: retroactive edits to unrelated historical issues without explicit justification.

## Final Completion Gate Checklist ##

At the end, add a completion summary:

```

## Final Completion Gate Checklist

- [ ] All substeps 1A through NC are complete.
- [ ] Implementation scope matches phase requirements (no unintended expansion).
- [ ] All guardrails are preserved.
- [ ] Unit tests pass and cover all Phase constraints.
- [ ] Final review checklist is complete and signed off.
- [ ] Build and test suite succeed.
- [ ] Implementation issues reconciled: newly identified issues captured in records/indexes; resolved issues reflected in the archive view with completion evidence.
- [ ] Phase is ready for the next phase to begin.

```

## Implementation Issues Workflow ##

### Purpose ###

Implementation issues are tracked centrally so work discovered during planning, review, or phase completion is not lost and can be scheduled, resolved, or archived later.

### Source of Truth ###

- **`Project/Tasks/ImplementationPlan/ImplementationIssues/ImplementationIssuesIndex.md`** is the canonical active summary of unresolved implementation issues across phases.
- **`Project/Tasks/ImplementationPlan/ImplementationIssues/ImplementationIssuesArchive.md`** is the archive summary of resolved or archived implementation issues.
- **`Project/Tasks/ImplementationPlan/ImplementationIssues/Records/`** contains the canonical per-issue record files.
- **Checklist inline notes** are phase-local observations until reconciled into the Implementation Issues system at phase completion.

### Workflow Integration ###

**During Checklist Creation (Researcher):**
  - Read active implementation issues from `Project/Tasks/ImplementationPlan/ImplementationIssues/ImplementationIssuesIndex.md`
- Report issues scheduled for the target phase or still open with applicable scope
- Include issue numbers, types, and any legacy deferment IDs when present
- Run mandatory pre-publish consistency checks before handoff to Planner:
   - Section 21 section-to-phase mapping check against the candidate checklist phase scope
   - `ImplementationIssues` scheduling conflict check between the index and referenced issue records
   - merged-duplicate check to ensure only one active tracker remains per merged scope

**During Checklist Planning (Planner):**
- Incorporate scheduled implementation issues into checklist steps/substeps if resolving them in-scope
- Explicitly re-defer or keep open with rationale if not addressing them in this phase
- Add implementation issue reconciliation substep in Final Completion Gate section (typically last substep before final summary)

**During Phase Completion:**
- Add newly identified findings as implementation issue records with the appropriate issue type, priority, source, and scheduled target
- Update resolved implementation issue records with:
  - status/history notes showing what phase resolved the issue
  - updated date and any resolution PR or completion evidence
  - archived/resolved state so the archive summary reflects completion
- For scope/architecture findings discovered in completion review:
   - document findings and remediation guidance in a post-phase implementation review report
   - create/queue a user-owned post-phase implementation atomic execution checklist
   - do not implement those fixes inside the completion gate checklist step

### Example Implementation Issue Reconciliation Substep ###

```

### 8D - Reconcile implementation issues

- [ ] Action: review checklist for newly identified implementation issues and reconcile with `Project/Tasks/ImplementationPlan/ImplementationIssues/ImplementationIssuesIndex.md` and `ImplementationIssues/Records/`.
- [ ] Scope: this checklist file, `ImplementationIssuesIndex.md`, `ImplementationIssuesArchive.md`, and affected record files.
- [ ] Verify: new issue records exist for newly identified findings; resolved issue records have updated status/history so archive output reflects completion.
- [ ] Suggested commit: `phase2(step8D): reconcile implementation issues after Phase 2 completion`
- [ ] Must include: any new implementation issue records; status/history updates for resolved items; phase/date/resolution notes.
- [ ] Must exclude: retroactive edits to unrelated historical issues without explicit justification.

```

### Legacy Deferment IDs ###

- Preserve `legacy_id` values such as `DEF-NNN` when an implementation issue originated from the old deferments system
- New implementation issues use the GitHub issue number as the canonical identifier
- Only assign a new `legacy_id` when intentionally backfilling a legacy deferment mapping; otherwise leave it blank

## Key Principles ##

### Implementation Checkpoints per Substep ###

Each substep should represent ONE logical change:
- One new file or class
- One integration point
- One configuration update
- One test file or test suite

If a substep description says "and also refactor X", split it into two substeps.

This scope discipline applies to substeps as planning units. Commits may be structured differently based on developer preference, but substeps define the recommended implementation granularity.

### No Scope Creep ###

A substep must not expand beyond its stated scope. If implementation reveals a missing piece outside the stated scope:
- Stop and note the gap
- Open a follow-up substep or commit
- Do not silently expand into neighboring subsystems

### Clear Verification Criteria ###

Every "Verify" line should be testable:
- "build succeeds" ✓ (compile check)
- "tests pass" ✓ (automated)
- "implementation follows design guide" ✗ (too vague; be specific about what to check)

### Commits as Workflow Reminders, Not Requirements ###

Suggested commits are workflow reminders to help track progress:
- Format guidance: `phase(step): brief description`
- Example: `phase1(step1A): add minimal ModEntry shell`
- **Not verified or enforced**: Commit history structure is not checked during phase completion gates
- **Acceptance criteria focus**: Phase completion validated by scope boundaries, tests passing, and guardrails preserved — not by commit history
- **Recommended workflow**: Commit after completing each substep for progress tracking and easier rollback; use descriptive messages that reference substep IDs (e.g., `phase3(step1A): ...`)

### Guardrails Checked After Coding ###

Guardrails are NOT execution checkboxes during the phase. They are checked off in the Final Review section once all substeps are done and code is ready for build/test.

### Both Human and AI Execution ###

Unit testing and final review instructions must be clear enough for:
- A human developer to read and execute
- An AI agent (e.g., UnitTestAgent, Reviewer) to read and execute

Avoid implementation-specific language (e.g., "use xUnit mocks") unless the phase/project requires it.

## Researcher Input Checklist ##

When the Researcher gathers context, they should answer:

- [ ] What is the phase responsibility from the design guide?
- [ ] What subsystems or components are involved?
- [ ] What are the edge cases or constraints mentioned in the design guide?
- [ ] What previous phases/checklists exist (to check for overlap)?
- [ ] What external dependencies exist (libraries, APIs)?
- [ ] What implementation scope boundaries make sense (one file? one responsibility boundary)?
- [ ] Are there listed guardrails/constraints in the design guide to preserve?
- [ ] What test coverage is recommended or implied?
- [ ] **What active implementation issues (from `Project/Tasks/ImplementationPlan/ImplementationIssues/ImplementationIssuesIndex.md`) are scheduled for this phase or still open with applicable scope?**

## Planner Output Checklist ##

The Planner should produce:

- [ ] Step titles and goals (major milestones)
- [ ] Substep list with action, scope, verify, and must-include/exclude rules
- [ ] Implementation checkpoint granularity decisions
- [ ] Dependencies or ordering constraints between substeps
- [ ] Unit test substeps (which constraints to test)
- [ ] **Implementation issue incorporation plan: which active issues (from Researcher findings) are resolved in-scope, and which are explicitly re-deferred/left open with rationale**
- [ ] **Implementation issue reconciliation substep in Final Completion Gate section**
- [ ] Final review substeps (guardrail audit, atomic boundary check)
- [ ] Any clarifications or ambiguities from the design guide

## Reviewer Validation Checklist ##

The Reviewer should verify:

**Design Guide Alignment:**
- [ ] Plan covers all phase responsibilities from the design guide
- [ ] Plan respects dependencies specified in the design guide
- [ ] No critical subsystems are missing
- [ ] Phase constraints/guardrails are identified

**Workspace Contracts:**
- [ ] Guardrails are clear and preservable
- [ ] Step structure is logical and followable
- [ ] Substep actions are precise and scope-limited
- [ ] Verification criteria are testable
- [ ] Unit test substeps are comprehensive
- [ ] Final review section checks guardrails and scope boundaries
- [ ] Both human and AI agents can follow the instructions
- [ ] Mandatory pre-publish checks were executed and passed:
   - [ ] Section 21 section-to-phase mapping consistency
   - [ ] `ImplementationIssues` index/record scheduling consistency
   - [ ] merged-duplicate active-tracker consistency

**Plan Quality:**
- [ ] No ambiguities in step/substep descriptions
- [ ] Commit granularity is realistic and atomic
- [ ] No scope creep detected in must-include/exclude rules
- [ ] Test coverage is sufficient to catch drift

## WorkspaceAgent Drafting Checklist ##

The WorkspaceAgent should:

- [ ] **CREATE THE FILE DIRECTLY** using createFile tool (do NOT hand content back to Orchestrator)
- [ ] Use correct file path (e.g., `Project/Tasks/Implementation Plan/Phase N - Atomic Commit Execution Checklist.md`)
- [ ] Produce complete Markdown file with all sections
- [ ] Use correct naming (`Phase N - Atomic Commit Execution Checklist.md`)
- [ ] Use proper checkbox formatting (unchecked `[ ]` for checkboxes)
- [ ] Use clear, readable language suited for both human and AI execution
- [ ] Include explicit scope/file references in each substep
- [ ] Include verification criteria that can be verified without guesswork
- [ ] Ensure unit test section is clear enough for human or UnitTestAgent
- [ ] Ensure final review section is clear enough for human or Reviewer
- [ ] Include final completion gate checklist summarizing all prerequisites
- [ ] Re-run and confirm the same three mandatory pre-publish checks before final file write:
   - [ ] Section 21 section-to-phase mapping consistency
   - [ ] `ImplementationIssues` index/record scheduling consistency
   - [ ] merged-duplicate active-tracker consistency

## Important Notes for Project Portability ##

This instruction file is designed to be portable across projects and phases. When adapting it:

1. **Research phase:** Always research the design guide phase specification (location varies by project)
2. **Scope granularity:** Adjust atomic commit granularity based on the project's architecture (monorepo vs. modular, language/framework specifics)
3. **Test frameworks:** Refer to project-specific testing contracts (e.g., xUnit, Moq for C#)
4. **Guardrails:** Extract from design guide phase specs; they are project-specific
5. **Reviewer criteria:** Always prioritize design guide alignment first, then workspace contracts

If a project has its own atomic commit guidelines or design-phase templates, the Researcher should identify and cite them.

```
