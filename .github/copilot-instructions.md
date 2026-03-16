# Joja AutoTasks: AI Operating Contract

Use this file as the default execution contract for work in this repository.
Prefer it over broad repository discovery. Search code or docs only when this file is missing
needed detail or appears stale.

## Primary Goal

Work in ways that preserve deterministic behavior, strong subsystem boundaries, and low-churn
edits.

Repository context that matters:

- Joja AutoTasks is a Stardew Valley SMAPI mod.
- The architecture is command/snapshot oriented.
- Canonical state ownership, lifecycle forwarding, persistence, and UI consumption are separate
  responsibilities.

## Decision Order

When instructions conflict, use this order:

1. Explicit user request
2. [`AGENTS.md`](../AGENTS.md)
3. This file
4. Relevant files under [`.github/instructions/`](./instructions/)
5. Design docs under [`Project/Planning/`](../Project/Planning/)

## Behavior Model

- Keep changes scoped, deterministic, and minimal.
- Do not widen scope silently.
- State assumptions when they affect behavior or architecture.
- Ask before any destructive, high-impact, or contract-changing work.
- Prefer small patches over broad rewrites.
- Do not treat absence of documentation as permission to redesign.

## Architecture Invariants

Do not violate these unless the user explicitly requests a contract change:

- Do not introduce direct state mutation paths that bypass command or state-store boundaries.
- Preserve deterministic identifier behavior, ordering guarantees, and snapshot semantics.
- Keep lifecycle signal forwarding separate from state ownership and persistence.
- Keep backend/game logic, UI C# interaction logic, and StarML responsibilities separate.
- Preserve the composition-root role of startup wiring.

If code appears to conflict with docs, stop and surface the mismatch instead of choosing a side
unilaterally.

## Edit Strategy

Default workflow for non-trivial tasks:

1. Read only the instructions relevant to the task.
2. Inspect the smallest code surface that can answer the question.
3. Plan briefly if the change is multi-step or risky.
4. Implement the smallest correct patch.
5. Run the narrowest validation that proves the change.

For small, localized tasks, implement directly.

## Validation

Run commands from repository root.

Default code validation:

```powershell
dotnet build JojaAutoTasks.csproj -c Debug -p:EnableModDeploy=false -p:EnableModZip=false
dotnet test "Tests\JojaAutoTasks.Tests.csproj"
```

Focused test examples:

```powershell
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~LifecycleCoordinatorTests
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~EventDispatcherTests
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~UpdateTickedGuardTests
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~ConfigLoaderMigrationSafetyTests
```

Clean bootstrap when needed:

```powershell
dotnet clean JojaAutoTasks.sln -c Debug
dotnet restore JojaAutoTasks.sln
```

Known pitfall:

- `dotnet test --no-build` can fail immediately after cleaning test outputs because the test DLL
  may not exist yet. Use normal `dotnet test` or build first.

## Documentation Policy

When code changes alter behavior, contracts, build/setup, or architecture intent, update the
relevant existing docs in the same change. Do not create redundant documentation or restate code
structure that the repository already expresses clearly.

## Communication Style

Optimize for agent usefulness over prose quality:

- Be concise, direct, and explicit.
- Separate facts, assumptions, and open questions.
- Prefer operational guidance over explanation.
- Preserve a calm, collaborative, no-drama tone.
- When multiple valid paths exist, present the tradeoff that matters most.

## Routing Hints

Load only the contract files needed for the task:

- Backend/gameplay C#: [`backend-architecture-contract.instructions.md`](./instructions/backend-architecture-contract.instructions.md)
- UI/view-model C#: [`frontend-architecture-contract.instructions.md`](./instructions/frontend-architecture-contract.instructions.md)
- C# style: [`csharp-style-contract.instructions.md`](./instructions/csharp-style-contract.instructions.md)
- Tests: [`unit-testing-contract.instructions.md`](./instructions/unit-testing-contract.instructions.md)
- Review/risk checks: [`review-and-verification-contract.instructions.md`](./instructions/review-and-verification-contract.instructions.md)
- Docs updates: [`update-docs-on-code-change.instructions.md`](./instructions/update-docs-on-code-change.instructions.md)


# Spec-Driven Workflow

This repo uses a 10-step spec-driven development workflow with three custom agents. Always be aware of workflow position before starting feature work.

## Agent Routing

Each step is owned by a specific agent. Select the correct agent from the dropdown first:

| Agent | Steps | Model |
|-------|-------|-------|
| `SpecOrchestrator` | 1 (Requirements), 9 (Execution), Status, Revise | GPT-5.4 xhigh |
| `SpecPlanner` | 2 (Epic Brief), 3 (Core Flows), 5 (Tech Plan), 8 (Tickets) | Claude Sonnet 4.6 |
| `SpecReviewer` | 4 (PRD Validation), 6 (Arch Validation), 7 (Cross-Artifact), 10 (Impl Validation) | GPT-5.4 xhigh |

## Invoking Steps

Switch to the correct agent, then type `/` to select the skill:

```
[SpecOrchestrator] /step-1-requirements
[SpecPlanner]      /step-2-epic-brief
[SpecPlanner]      /step-3-core-flows
[SpecReviewer]     /step-4-prd-validation
[SpecPlanner]      /step-5-tech-plan
[SpecReviewer]     /step-6-arch-validation
[SpecReviewer]     /step-7-cross-artifact
[SpecPlanner]      /step-8-ticket-breakdown
[SpecOrchestrator] /step-9-execution
[SpecReviewer]     /step-10-impl-validation
[SpecOrchestrator] /workflow-status
[SpecOrchestrator] /revise-requirement
```

After each step, the active agent will surface handoff buttons to the next — use them to advance with full context already loaded.

## Workflow State

Current position is in `.workflow/state/workflow-state.json`. Artifacts live in `.workflow/artifacts/`.

## Transition Rules

```
1 → 2 → 3 → 4 → 5 → 6 → 7 → 8 → 9 → 10
              ↑       ↑   ↑           ↑
             4→3     6→5 7→3/5      10→9   (validation failures loop back)
Any step → revise-requirement (on scope change)
```

## Core Values

- Questions are investments in correctness, not overhead
- Multiple rounds of clarification is normal and encouraged
- Specs record decisions made together, not deliverables to rush toward
- Surfacing assumptions early is cheap; fixing wrong work is expensive


## Final Rule

Trust this file for operational guidance, but verify against code when behavior is the source of
truth.
