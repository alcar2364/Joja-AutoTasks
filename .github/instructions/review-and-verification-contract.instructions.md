---
name: review-and-verification-contract
description: "Review, verification, and acceptance rules: contract-based review, risk assessment, verification paths, pre-merge checklists. Use when: reviewing code or implementations."
---

# REVIEW-AND-VERIFICATION-CONTRACT.instructions.md #

## Purpose ##

This contract defines **review, verification, and acceptance rules** for AI agents working in the
**JAT (Joja AutoTasks)** workspace.

Its purpose is to ensure that:
    - changes are reviewed against **contracts**, not vibes
    - reviewers check the **right risks** for JAT
    - implementation guidance includes a clear **verification path**
    - refactors do not quietly introduce architectural drift, determinism bugs, or UI regressions

This contract applies to:
    - code review
    - design review
    - implementation review
    - refactor review
    - patch acceptance guidance
    - pre-merge verification checklists produced by agents

This contract does **not** define:
    - code style rules (see [`CSHARP-STYLE-CONTRACT.instructions.md`](CSHARP-STYLE-CONTRACT.instructions.md), [`JSON-STYLE-CONTRACT.instructions.md`](JSON-STYLE-CONTRACT.instructions.md), [`SML-STYLE-CONTRACT.instructions.md`](SML-STYLE-CONTRACT.instructions.md))
    - backend/frontend architecture rules (see [`BACKEND-ARCHITECTURE-CONTRACT.instructions.md`](BACKEND-ARCHITECTURE-CONTRACT.instructions.md), [`FRONTEND-ARCHITECTURE-CONTRACT.instructions.md`](FRONTEND-ARCHITECTURE-CONTRACT.instructions.md))
    - workspace interaction rules (see [`WORKSPACE-CONTRACTS.instructions.md`](WORKSPACE-CONTRACTS.instructions.md))

## 1. Review source-of-truth order ##

Agents performing review or verification MUST use this precedence order:

1. Explicit user instructions in the current task
2. [`WORKSPACE-CONTRACTS.instructions.md`](WORKSPACE-CONTRACTS.instructions.md)
3. [`BACKEND-ARCHITECTURE-CONTRACT.instructions.md`](BACKEND-ARCHITECTURE-CONTRACT.instructions.md)
4. [`FRONTEND-ARCHITECTURE-CONTRACT.instructions.md`](FRONTEND-ARCHITECTURE-CONTRACT.instructions.md)
5. [`CSHARP-STYLE-CONTRACT.instructions.md`](CSHARP-STYLE-CONTRACT.instructions.md)
6. [`JSON-STYLE-CONTRACT.instructions.md`](JSON-STYLE-CONTRACT.instructions.md)
7. [`SML-STYLE-CONTRACT.instructions.md`](SML-STYLE-CONTRACT.instructions.md)
8. [`../Instructions/UI-COMPONENT-PATTERNS.instructions.md`](../Instructions/UI-COMPONENT-PATTERNS.instructions.md)
9. Existing stable patterns already present in the touched subsystem

If sources conflict, the reviewer MUST state the conflict explicitly and explain which
higher-priority rule controls.

## 2. Review goals ##

Every review MUST attempt to answer these questions:

1. Did the change satisfy the user's requested scope?
2. Did the change violate any workspace or architecture contract?
3. Did the change preserve determinism and state ownership rules?
4. Did the change preserve UI/backend boundaries?
5. Did the change introduce performance risk?
6. Did the change introduce naming/style drift in the touched area?
7. Is there a concrete way to verify the change safely?

A reviewer is not there to admire the patch like it is hanging in a museum. A reviewer is there to
find failure modes before the user does.

## 3. Required review output format ##

When performing review, agents SHOULD return results using this structure unless the user requests
otherwise:

### 3.1 Scope assessment ###

    - what the change appears to do
    - whether it matches requested scope
    - whether scope was expanded

### 3.2 Findings by severity ###

Use these severity levels:

    - **Blocker**  
  Must be fixed before acceptance. Breaks contract, correctness, determinism, or user scope.

    - **Major**  
  Strongly recommended fix. High chance of bug, maintenance issue, or architectural drift.

    - **Minor**  
  Worth fixing for clarity, consistency, or maintainability, but not necessarily
  acceptance-blocking.

    - **Note**  
  Observation, tradeoff, or follow-up suggestion.

### 3.3 Verification checklist ###

Include:
    - what to compile
    - what behavior to exercise
    - what edge cases to test
    - what contract boundaries to re-check

### 3.4 Acceptance statement ###

Conclude with one of:
    - **Accept**
    - **Accept with minor follow-ups**
    - **Needs changes**
    - **Blocked by contract violation**

## 4. Scope review rules ##

### 4.1 Respect explicit scope ###

Reviewers MUST verify that the implementation stayed within the requested scope.

Examples:
    - if the user asked for single-file refactor, the reviewer must call out any multi-file
    spillover
    - if the user asked for no behavior change, the reviewer must flag any behavioral change
    - if the user asked for analysis only, the reviewer must flag direct edits

### 4.2 No silent scope forgiveness ###

Reviewers MUST NOT excuse scope expansion just because it seems “cleaner” or “more correct.”

If broader changes were truly necessary, the reviewer should say so explicitly.

## 5. Backend verification rules ##

Use these rules when reviewing backend or mixed changes.

### 5.1 Canonical state ownership ###

Reviewers MUST verify that:
    - canonical task state remains owned by the State Store
    - no UI layer directly mutates canonical task state
    - evaluation/generation logic does not mutate canonical task state directly
    - state mutation still happens only through the designated command/state-control path

### 5.2 Determinism ###

Reviewers MUST flag:
    - random or GUID-based task IDs
    - ordering that depends on unordered traversal
    - save/load behavior that could produce unstable IDs
    - time-based mutation logic that is not explicitly designed and controlled

### 5.3 Persistence discipline ###

Reviewers MUST verify:
    - persisted data remains minimal
    - derived/transient values are not being unnecessarily saved
    - version changes are explicit
    - migration logic is present when persisted format changes

### 5.4 Reconciliation correctness ###

When task generation/reconciliation is touched, reviewers MUST check:
    - generated and persisted tasks are reconciled deterministically
    - completion state preservation still works where applicable
    - day-scoped recurring behavior remains stable

### 5.5 Engine update cycle ###

Reviewers MUST flag:
    - accidental per-frame evaluation
    - unbounded periodic scans
    - evaluation work that should be event-driven but is not
    - queue growth or duplicate work risk if an event/evaluation queue is used

## 6. Frontend verification rules ##

Use these rules when reviewing frontend or mixed changes.

### 6.1 Snapshot-driven UI ###

Reviewers MUST verify that:
    - UI is consuming snapshots or UI-local state derived from snapshots
    - UI is not directly mutating canonical state
    - UI actions dispatch commands or local UI changes appropriately

### 6.2 Surface responsibility ###

Reviewers MUST confirm that:
    - HUD remains lightweight
    - complex browsing/management remains in the full menu
    - history browsing is not being awkwardly shoved into the HUD
    - the UI surface chosen matches the intended level of interaction

### 6.3 Component pattern consistency ###

Reviewers SHOULD compare new or changed UI against [`../Instructions/UI-COMPONENT-PATTERNS.instructions.md`](../Instructions/UI-COMPONENT-PATTERNS.instructions.md) and flag:
    - unnecessary one-off layout inventions
    - list/details flows that should use split view but do not
    - buried navigation controls
    - scattered actions
    - unclear hierarchy

### 6.4 StarML/SML correctness ###

When `.sml` is involved, reviewers MUST check (see [`SML-STYLE-CONTRACT.instructions.md`](SML-STYLE-CONTRACT.instructions.md)):
    - StardewUI/StarML tags are valid and semantically appropriate
    - attributes and events use kebab-case
    - structural attributes are used correctly
    - event bindings use pipe syntax
    - templates/includes/outlets are used in valid places
    - bindings are readable and not over-complicated

### 6.5 Local UI state discipline ###

Reviewers MUST ensure that local UI state is limited to:
    - selection
    - collapsed/expanded state
    - scroll position
    - selected day/tab/filter
    - similar view-local state

Reviewers MUST flag any attempt to let UI-local state become canonical task state.

## 7. Style verification rules ##

### 7.1 Language-specific contracts ###

Reviewers MUST use the appropriate style contract for each file type:
    - C# → [`CSHARP-STYLE-CONTRACT.instructions.md`](CSHARP-STYLE-CONTRACT.instructions.md)
    - JSON → [`JSON-STYLE-CONTRACT.instructions.md`](JSON-STYLE-CONTRACT.instructions.md)
    - SML → [`SML-STYLE-CONTRACT.instructions.md`](SML-STYLE-CONTRACT.instructions.md)

### 7.2 Touched-region rule ###

Reviewers SHOULD focus style enforcement primarily on the touched region unless:
    - the user requested broader cleanup
    - the file is being actively normalized as part of the change
    - an unchanged adjacent region directly conflicts with the edited code

### 7.3 Naming verification ###

Reviewers MUST check:
    - type/file naming matches JAT rules
    - acronym conversion matches JAT rules
    - banned vague names are not introduced
    - newly introduced names fit architectural vocabulary

## 8. Performance verification rules ##

Performance matters for JAT, especially in HUD/UI and evaluation logic.

Reviewers MUST flag:
    - per-frame allocations that do not need to exist
    - rebuilding UI trees on every draw/update without justification
    - frequent whole-world scans
    - repeated filtering/sorting in hot paths
    - event handlers or bindings that imply excessive repeated work
    - excessive LINQ in clearly performance-sensitive paths

Performance findings should be categorized:
    - **Blocker** if clearly dangerous
    - **Major** if high-risk or likely to regress gameplay responsiveness
    - **Minor** if cleanup would help but risk is modest

## 9. Refactor review rules ##

When reviewing a refactor, reviewers MUST separately evaluate:

### 9.1 Architectural preservation ###

Did the refactor move responsibilities across subsystem boundaries incorrectly?

### 9.2 Behavioral preservation ###

Did behavior change despite “refactor only” scope?

### 9.3 Cleanup quality ###

Did the refactor actually reduce duplication, improve structure, or clarify ownership?

### 9.4 Diff safety ###

Did the refactor introduce unnecessary rename churn or broad formatting noise?

A refactor that produces lots of motion but little clarity should be called out. Fancy turbulence is
not the same thing as improvement.

## 10. Verification checklist templates ##

### 10.1 Backend change checklist ###

Use when reviewing engine/state/persistence changes:

    - Build/compile target affected code
    - Confirm canonical state ownership unchanged
    - Confirm mutation still flows through command/state-control path
    - Confirm task IDs remain deterministic
    - Confirm ordering remains stable
    - Confirm persisted format/version handling is correct
    - Confirm migrations exist if format changed
    - Confirm evaluation/update cycle did not become per-frame or unbounded
    - Exercise day transition if relevant
    - Exercise save/load if relevant

### 10.2 Frontend change checklist ###

Use when reviewing HUD/menu/StarML changes:

    - Build/compile affected code and markup
    - Open affected UI surface
    - Verify rendering/layout at intended screen sizes
    - Verify selection, scroll, tab/day navigation, and actions
    - Confirm no direct state mutation from UI
    - Confirm snapshot refresh behavior is correct
    - Confirm HUD still feels lightweight
    - Confirm menu hierarchy is readable
    - Confirm empty-state / no-selection state behaves correctly if relevant

### 10.3 Style-only cleanup checklist ###

Use when reviewing style cleanups:

    - Confirm no behavior change
    - Confirm naming/style rules were applied consistently in touched region
    - Confirm no unrelated churn
    - Confirm file/type ordering remains compliant
    - Confirm comments/docs remain accurate after cleanup

## 11. Edge-case review rules ##

Reviewers SHOULD explicitly consider edge cases when relevant, including:

    - empty task list
    - no historical snapshot for selected day
    - repeated tasks across day boundaries
    - invalid rule/config data
    - missing or null optional fields
    - very long task names or descriptions
    - small or crowded HUD layouts
    - no task selected in details panel
    - config/debug toggles changed at runtime

If an edge case is relevant and not checked, the reviewer should say so.

## 12. Reviewer behavior rules ##

### 12.1 Be concrete ###

Review comments MUST be actionable.
Bad:
    - “This seems off.”
Good:
    - “This introduces UI-driven mutation of canonical task state by setting completion directly in
    the menu layer; route this through the command/state-control path instead.”

### 12.2 Do not invent issues ###

If something is uncertain, say it is uncertain.
Do not fabricate certainty.

### 12.3 Distinguish contract issues from preferences ###

Reviewers MUST clearly separate:
    - contract violations
    - probable bugs
    - maintainability concerns
    - personal/style preferences

### 12.4 Honor user intent ###

If the user explicitly chose a tradeoff, reviewers may note the tradeoff but should not pretend it
is an accidental mistake.

### 12.5 Phase completion learning workflow ###

When reviewing a phase completion gate/checklist, reviewers MUST:

    - finalize the checklist review artifacts (build/test evidence, guardrail audit, scope audit, deferment reconciliation)
    - document scope/architecture issues, ambiguity, and remediation in a **post-phase implementation review report**
    - avoid implementing those scope/architecture fixes during the review pass
    - route fixes into a **post-phase implementation atomic execution checklist** owned by the user

For phase completion reviews, the reviewer role is verification and teaching, not direct implementation.

## 13. Acceptance rules ##

A change SHOULD be considered **Blocked by contract violation** if it:
    - breaks state ownership boundaries
    - introduces nondeterministic IDs or ordering
    - violates explicit user scope
    - adds clearly unsafe persistence changes without versioning/migration
    - makes UI directly mutate canonical state
    - misuses StarML syntax in a way likely to break or misrepresent UI behavior

A change SHOULD be considered **Needs changes** if it:
    - has major correctness or maintainability issues
    - has unverified risky behavior
    - has significant performance concerns
    - introduces naming/style drift in important touched areas

A change MAY be considered **Accept with minor follow-ups** if:
    - core contracts are satisfied
    - correctness looks sound
    - only small clarity/style polish remains

A change MAY be considered **Accept** if:
    - requested scope is satisfied
    - contracts are respected
    - verification path is clear
    - no material issues remain

## 14. Quick reviewer cheat sheet ##

When in doubt, check these first:

    - Did scope creep happen?
    - Did UI mutate canonical state?
    - Did backend bypass the State Store / command path?
    - Did anything make task IDs or ordering nondeterministic?
    - Did persistence format change without versioning/migration?
    - Did HUD become heavier or menu composition become muddled?
    - Did StarML syntax or bindings drift from contract?
    - Is there a concrete test path for the change?

If the answer to any of those is “yes” or “maybe,” the reviewer should dig there first. That is
where the gremlins live.
