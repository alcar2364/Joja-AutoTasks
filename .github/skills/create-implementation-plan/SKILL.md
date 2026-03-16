---
name: create-implementation-plan
description: Decompose a JAT feature or refactor into subsystem-scoped implementation phases stored as atomic commit execution checklists. Use when: creating step-by-step checklists to guide implementation with design-guide compliance and minimal atomic commits.
argument-hint: Describe the feature, fix, or refactor to plan; include target subsystem(s), relevant design-guide sections, any constraints (no behavior change, single-subsystem, etc.), and whether documentation updates are needed pre or post implementation.
---

# Create Implementation Plan

## Purpose

This skill defines how to decompose a JAT feature into subsystem-scoped implementation phases stored as atomic commit execution checklists under `Project/Tasks/ImplementationPlan/`. Each phase produces a single checklist file that a GameAgent, UIAgent, or other implementer can execute commit-by-commit without re-deriving scope or ordering. The detailed planning procedure and output template live in `planner-checklist-and-output-format`.

---

## JAT Subsystem Layers

JAT has four canonical layers. Plans must respect their sequencing dependency order:

| Layer | Scope | Must Precede |
|---|---|---|
| Backend / State / Engine | `Domain/`, `State/`, `Events/`, `Lifecycle/` | All other layers |
| UI / View-Model | `UI/`, view-model C# | Requires backend state to exist |
| Persistence / Migration | `Configuration/`, save-data schema | Can run parallel to UI after backend |
| Tests | `Tests/` subsystem folders | Follows the layer it covers |

A phase that spans multiple layers must explicitly declare layer order within the phase. Do not introduce cross-layer steps that jump ahead of the dependency chain.

---

## Phase Structure Conventions

Every phase checklist must match the format used by the existing Phase 1–4 checklists in `Project/Tasks/ImplementationPlan/`. The canonical structure is:

### Phase Header
Phase number, name, and a one-line purpose statement.

### Guardrails Block
Explicit constraints that must remain true for the entire phase. Examples:
- "No task/store mutation logic in this phase"
- "OnSaving is signal-only — no read/write to save data"
- "All new types are internal until the integration substep"

### Steps
Numbered major milestones (e.g., `## 1) Bootstrap SMAPI Skeleton ##`):

- **Step goal:** One-line bullet stating what the step achieves
- **Substeps (A/B/C):** Each substep represents one atomic commit and contains:
  - `Action:` — what changes
  - `Scope:` — exact file(s) and symbol(s) touched
  - `Verify:` — observable confirmation criterion (build passes, test passes, log output, etc.)
  - `Commit message:` — using JAT `phase(step):` convention
  - `Must include:` / `Must exclude:` — explicit boundary guards that define the commit's edges

### Final Completion Gate Checklist
A checkbox list confirming all substeps are complete, no scope drift occurred, and all relevant contracts are satisfied (build green, tests pass, no orphaned symbols, docs updated if required).

---

## Cross-Subsystem Dependency Sequencing

A subsystem layer may not begin until the interfaces and types it depends on exist in the layer below it:

- UI binding cannot begin until the snapshot and view-model types are defined in the backend layer.
- Persistence migration cannot begin until the domain model it migrates is stable.
- Tests for a layer are written after that layer's substeps are complete, not speculatively before.

When a plan spans multiple phases, each phase must confirm its dependency preconditions in its guardrails block.

---

## Atomic Commit Granularity

One substep = one logical change. Correct granularity examples:

- Add one new file (e.g., `IEventDispatcher.cs`)
- Wire one integration point (e.g., connect coordinator to dispatcher in `BootstrapContainer`)
- Add one configuration field and its accessor
- Add one test file covering a specific class or behavior

A substep that expands beyond its declared file scope during implementation must stop, produce the in-scope commit, and open a follow-up substep or issue for the expanded work.

---

## Canonical Examples

`Project/Tasks/ImplementationPlan/` holds the live canonical reference for this format. The Phase 1–4 checklists there are the reference implementation. New plans must match their structure — section names, substep format, guardrails block, and completion gate — exactly.
