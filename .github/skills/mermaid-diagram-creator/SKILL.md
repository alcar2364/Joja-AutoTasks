---
name: mermaid-diagram-creator
description: "Create or refresh a polished Mermaid diagram for the Joja AutoTasks codebase, reflecting implemented architecture with clear visual grouping and labeled relationships."
argument-hint: "No arguments needed. This skill generates a Mermaid diagram based on the current codebase structure and relationships."
---
<!-- markdownlint-disable -->

# Mermaid Codebase Diagram Skill

Use this file when a custom agent needs to create or refresh a polished Mermaid diagram for the Joja AutoTasks codebase.

## Outcome

Produce a single Markdown file that:

- uses a standard fenced `mermaid` block compatible with VS Code Markdown Preview and `mjbvz/vscode-markdown-mermaid`
- excludes `Tests/`
- reflects implemented code, not planned-only architecture
- groups the codebase into visually distinct architectural sections
- labels meaningful arrows with short verb phrases
- looks intentional, not auto-generated

Default output path:

- `Project/Planning/Codebase Mermaid Diagram.md`

## Required Workflow

1. Read `AGENTS.md` and `.github/copilot-instructions.md` before editing.
2. Gather the current source layout with targeted search, not broad guessing.
3. Read the implemented C# files that define the real dependency graph:
   - `ModEntry.cs`
   - `Startup/`
   - `Lifecycle/`
   - `Events/`
   - `State/`
   - `Domain/`
   - `UI/`
   - `Configuration/`
   - `Infrastructure/Logging/`
4. Use `Project/Planning/Architecture Map.md` only as a cross-check. If docs and code differ, trust the current code and do not silently "fix" the code.
5. Exclude `Tests/` entirely unless the user explicitly asks for test architecture.
6. Prefer one beautiful, readable diagram over an exhaustive but tangled one.

## Relationship Rules

Show only relationships supported by the source code, such as:

- constructor dependencies
- composition or aggregation
- interface implementation
- dispatch flow
- command routing
- projection flow
- factory or utility usage
- subscription flow

Do not invent relationships just because two types live in the same folder or share similar fields.

## Diagram Structure For This Repo

Default groupings:

1. `Runtime Spine`
   - `ModEntry`
   - `BootstrapContainer`
   - `ModRuntime`
   - `LifecycleCoordinator`
   - `IEventDispatcher`
   - `EventDispatcher`
   - `UiSnapshotSubscriptionManager`
   - `TaskSnapshot`
   - `TaskView`

2. `State Command Engine`
   - `StateStore`
   - `StateContainer`
   - `SnapshotProjector`
   - `ExpirationDetector`
   - `DayTransitionHandler`
   - `ManualTaskCounter`
   - `IStateCommand`
   - all task command types
   - `ICommandHandler<TCommand>`
   - all task command handler types
   - `TaskRecord`

3. `Domain Foundations`
   - identifier utilities and value types
   - task model types and enums
   - config and logging support types

If the repo grows, keep these three visual bands unless the diagram becomes unreadable.

## Visual Style Rules

Aim for a Joja-meets-Stardew look:

- parchment-like background
- Joja blue for orchestration
- warm gold for entry/startup
- teal for state and command flow
- neutral slate for support services

Use:

- `flowchart LR`
- a Mermaid `%%{init: ... }%%` block with custom theme variables
- `classDef` blocks for visual categories
- `subgraph` blocks for architectural sections
- concise node labels in the form `TypeName<br/>purpose`

Keep labels readable:

- 1-2 lines per node
- short verb phrases on arrows
- no paragraph-sized edge labels

Make interfaces visually distinct with a dedicated interface style.

## Arrow Label Rules

Every important edge should include a short verb phrase, for example:

- `builds via`
- `assembles`
- `stores`
- `wires`
- `dispatches to`
- `signals`
- `routes to`
- `handles`
- `mutates`
- `projects`
- `packages`
- `contains`
- `creates`
- `parses`
- `normalizes with`

If an arrow label feels noun-heavy or vague, rewrite it as a verb phrase.

## Scope Rules

Do:

- show implemented types only
- show top-level architectural types first
- include commands and handlers when they materially explain state flow
- simplify low-value edges before removing high-value runtime and state arrows

Do not:

- include `Tests/`
- include speculative future classes from planning docs
- dump every property or method into the diagram
- use unlabeled arrows for important relationships
- create a diagram so dense that labels collide everywhere

## Validation

Before finishing:

1. Re-open the generated Markdown file.
2. Check that the Mermaid fence opens and closes correctly.
3. Scan for invalid HTML or malformed labels.
4. Confirm that every major runtime/state relationship has an arrow label.
5. State whether automated Mermaid render validation was or was not performed.
6. If the change is docs-only, say that `dotnet build` and tests were not run.

## Starter Template

Use this as a starting point, then adapt it to the current code:

````markdown
```mermaid
%%{init: {
  "theme": "base",
  "themeVariables": {
    "background": "#F6F2E8",
    "primaryBorderColor": "#2C6BE0",
    "lineColor": "#58779D"
  },
  "flowchart": {
    "curve": "basis",
    "htmlLabels": true,
    "nodeSpacing": 34,
    "rankSpacing": 48
  }
}}%%
flowchart LR
    classDef entry fill:#FFF3C6,stroke:#C89100,color:#402E00,stroke-width:2px;
    classDef orchestration fill:#DDEBFF,stroke:#2C6BE0,color:#17304F,stroke-width:2px;
    classDef state fill:#D8F1EF,stroke:#188678,color:#103C37,stroke-width:2px;
    classDef command fill:#EDFBF8,stroke:#1F9B8C,color:#103C37,stroke-width:1.5px;
    classDef handler fill:#F4FFFB,stroke:#28A391,color:#103C37,stroke-width:1.5px;
    classDef model fill:#FFF9EE,stroke:#C7A04A,color:#4A3711,stroke-width:1.6px;
    classDef domain fill:#F8F3E8,stroke:#B88A43,color:#4B3415,stroke-width:1.6px;
    classDef support fill:#F4F7FA,stroke:#6F7F92,color:#2F3A47,stroke-width:1.5px;
    classDef iface fill:#FFFFFF,stroke:#2C6BE0,color:#17304F,stroke-width:1.8px,stroke-dasharray: 6 4;
```
````
