---
name: git-commit
description: 'Compose and execute JAT-standard commits using phase(step): imperative description format aligned to Atomic Commit Execution Checklists. Use when user asks to commit changes, group staged work, or draft commit messages in this repository.'
license: MIT
---

# JAT Git Commit Standard

## Canonical Message Format

Use this repository-specific format for the first line of every commit:

```
phaseN(stepXY): imperative description
```

Where:

- `phaseN` maps to the active implementation checklist phase (for example `phase1`, `phase2`, `phase3`, `phase4`).
- `stepXY` maps to exactly one checklist step identifier (for example `step1A`, `step7D`, `step10C`).
- Description is imperative, concise, and scoped to that single step.

This format is tied to files under `Project/Tasks/ImplementationPlan/Phase * - Atomic Commit Execution Checklist.md`.

## Scope Rules

- One logical change per commit.
- One checklist step per commit.
- If work spans multiple checklist steps, split into multiple commits.
- Do not stage unrelated files to satisfy a single step message.

## Commit Body Policy

Add a commit body when any of these apply:

- validation evidence should be preserved with the commit
- behavior or contract changes need explicit callouts
- the step touches multiple subsystems and needs cross-file impact notes

Recommended body structure:

```
<optional short rationale bullets>

Validation: <command1> ; <command2>
```

Validation line examples:

- `Validation: dotnet build JojaAutoTasks.csproj -c Debug -p:EnableModDeploy=false -p:EnableModZip=false ; dotnet test Tests/JojaAutoTasks.Tests.csproj`
- `Validation: not run (docs-only change)`

## Forbidden Patterns

Do not use generic Conventional Commits prefixes in this repository:

- `feat:`
- `fix:`
- `docs:`
- `chore:`
- any other `<type>:` SaaS or badge-style prefixing pattern

Do not use vague subjects like "misc updates" or "cleanup" without a phase/step binding.

## JAT Examples (from ImplementationPlan)

- `phase1(step1A): add minimal ModEntry shell`
- `phase1(step5C): wire coordinator outputs to dispatcher`
- `phase2(step4A): add deterministic built-in and task-builder TaskId constructors`
- `phase3(step5C): wire snapshot generation and SnapshotChanged event`
- `phase4(step3D): add IDisposable with deterministic unsubscription`

## Commit Workflow

1. Identify the exact checklist step being completed.
2. Stage only files required for that step.
3. Confirm staged diff matches the step scope.
4. Compose `phase(step): description` subject.
5. Add body and `Validation:` line when needed.
6. Commit once scope and evidence are coherent.

## Safety Guardrails

- Never run destructive git operations without explicit user approval.
- Never include secrets in commits.
- Prefer creating a follow-up commit over amending unless the user explicitly asks to amend.
