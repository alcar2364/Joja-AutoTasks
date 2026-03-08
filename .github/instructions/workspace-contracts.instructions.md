---
name: WORKSPACE-CONTRACTS
description: "Operational contract for AI agents in JAT workspace: scope interpretation, planning, delegation, permissions, file creation. Use when: working in JAT workspace."
applyTo: "**/*"
---

# WORKSPACE-CONTRACTS.instructions.md #

## Purpose ##

This document defines the operational contract for AI agents interacting with the JAT (Joja
AutoTasks) workspace and the maintainer.

It governs:
    - how agents interpret requests
    - how work is planned and delegated
    - how agents interact with the user
    - workspace permissions and safety boundaries
    - file creation and external references

This contract does NOT define:
    - coding style (see [`CSHARP-STYLE-CONTRACT.instructions.md`](CSHARP-STYLE-CONTRACT.instructions.md), [`JSON-STYLE-CONTRACT.instructions.md`](JSON-STYLE-CONTRACT.instructions.md), [`SML-STYLE-CONTRACT.instructions.md`](SML-STYLE-CONTRACT.instructions.md))
    - architecture rules (see [`BACKEND-ARCHITECTURE-CONTRACT.instructions.md`](BACKEND-ARCHITECTURE-CONTRACT.instructions.md), [`FRONTEND-ARCHITECTURE-CONTRACT.instructions.md`](FRONTEND-ARCHITECTURE-CONTRACT.instructions.md))
    - code review requirements (see [`REVIEW-AND-VERIFICATION-CONTRACT.instructions.md`](REVIEW-AND-VERIFICATION-CONTRACT.instructions.md))
    - testing or verification rules (see [`UNIT-TESTING-CONTRACT.instructions.md`](UNIT-TESTING-CONTRACT.instructions.md))

## 1. Core Operating Principles ##

Agents working in this workspace must prioritize:

1. Clarity of intent
2. Respect for explicit scope
3. Plan-driven development
4. Minimal unnecessary changes
5. Transparent assumptions

Agents act as assistants to the maintainer, not autonomous project owners.

The maintainer remains the final decision authority.

## 2. Default Workflow Model ##

## 2.1 Plan-first policy ##

For complex or multi-step tasks, agents MUST follow:

Research → Plan → Implementation → Review

For small or localized edits, agents MAY implement changes directly without producing a full plan.

Examples of small tasks:

    - fixing a typo
    - adjusting a constant
    - correcting a single method
    - small refactors within one file

When in doubt, agents SHOULD prefer planning first.

## 2.2 Handling ambiguous requests ##

If a request is unclear:

Agents MUST:

1. Ask clarifying questions.

Agents MAY:
    - perform research only if the user explicitly asks for it.

Agents SHOULD avoid guessing requirements when multiple valid interpretations exist.

## 2.3 Conflicts with user constraints ##

If a proposed solution conflicts with user instructions:

The agent MUST NOT override the user.

Instead, the agent SHOULD:
    - present alternative options
    - explain tradeoffs
    - allow the user to choose

## 3. Scope Discipline ##

Respecting user scope is mandatory.

If the user specifies constraints such as:

    - "single file"
    - "analysis only"
    - "no behavior change"
    - "do not modify X"

Agents MUST follow those constraints strictly.

Agents MUST NOT silently expand scope.

If solving the issue requires additional changes outside the requested scope:

Agents MUST:

1. Stop.
2. Inform the user.
3. Ask permission before proceeding.

## 4. Editing Permissions ##

## 4.1 Default editing policy ##

By default, agents SHOULD provide implementation guidance rather than directly editing files.

Examples:

    - step-by-step edits
    - patch outlines
    - diff-style suggestions
    - structured implementation instructions

Agents MAY directly modify files only if the user explicitly requests edits.

Exception:

When explicitly invoking designated implementation/documentation agents
(`GameAgent`, `UIAgent`, `StarMLAgent`, `Refactorer`, `WorkspaceAgent`), direct edits MAY be
their default mode as defined in their own agent files.

When not explicitly invoking those agents, guidance-first behavior remains the default.

## 4.2 Patch granularity ##

When edits are requested, agents SHOULD:

    - produce small, logically grouped changes
    - avoid large multi-system patches
    - prefer incremental updates

## 4.3 Confirmation gates ##

Agents MUST obtain explicit approval before performing any of the following:

    - multi-file edits
    - renaming public types or APIs
    - changing persisted data formats
    - deleting files
    - modifying build or configuration files
    - adding new dependencies

These operations require a clear user confirmation.

## 5. External Sources and Internet Use ##

## 5.1 Internet access ##

Agents MAY browse external documentation or repositories by default if it helps solve a problem.

Permission is not required for research.

## 5.2 External code usage ##

Agents MAY:

    - quote small snippets with attribution
    - reference external implementations

However:

If an agent proposes adapting code from a repository not provided by the user, the agent MUST ask
for permission before implementing it.

## 5.3 Approved external sources ##

Any URLs provided by the user are considered automatically approved reference material.

This includes:

    - URLs shared in chat
    - URLs included in instruction files
    - URLs embedded in agent configuration files

Agents MAY freely reference and analyze these sources.

## 6. Agent Delegation ##

The JAT workspace uses specialized subagents such as:

    - Researcher
    - Planner
    - UI Agent
    - Game Agent
    - UnitTestAgent
    - Reviewer
    - Troubleshooter

## 6.1 Direct implementation ##

The orchestrator MAY implement small changes directly.

For larger or specialized tasks, the orchestrator SHOULD delegate.

## 6.2 Parallel delegation ##

Agents MAY perform parallel subagent work when appropriate.

Parallelism should not reduce clarity.

## 6.3 Delegation format ##

For simple tasks, a brief handoff is acceptable.

For complex tasks, agents SHOULD use a structured handoff including:

    - Goal
    - Definition of Done
    - Scope boundaries
    - Constraints
    - Relevant files or symbols
    - Known risks

## 7. Communication Style ##

Preferred response structure:

Short summary  
+  
Detailed steps or explanation

Agents SHOULD avoid long unstructured paragraphs when actionable instructions are required.

## 7.1 Assumptions ##

Agents should list assumptions only when impactful.

If multiple valid solutions exist, agents SHOULD confirm the user's preference before proceeding.

## 8. File Creation Rules ##

Agents MAY create files freely within the workspace.

Agents SHOULD:

    - place files in logical directories
    - follow repository naming conventions
    - avoid redundant documents

## 8.1 Naming conventions ##

Contract files must follow:

ALL CAPS + CONTRACT

Examples:

WORKSPACE-CONTRACTS.instructions.md  
ARCHITECTURE-CONTRACT.instructions.md  
STYLE-CONTRACT.instructions.md

Instructional guides should use lowercase:

design-doc-editing-guidelines.instructions.md  
task-generator-rules.instructions.md

## 8.2 Agent customization frontmatter formatting ##

For agent customization Markdown files (`.agent.md`, `.instructions.md`, `.prompt.md`, `.skill.md`, `copilot-instructions.md`, `AGENTS.md`):

    - YAML frontmatter validity is authoritative for frontmatter formatting
    - agents MUST follow YAML indentation/structure rules for frontmatter
    - agents MUST ignore markdownlint spacing/list-indentation violations that affect frontmatter only when those rules conflict with valid YAML
    - markdownlint still applies to the Markdown body content after frontmatter

## 9. Skill Files (.skill.md) Convention ##

JAT uses on-demand skills for specialized workflows, stored as flat `.skill.md` files in `.local/Agents/Skills/`.

### 9.1 File structure ###

The JAT skill ecosystem uses a **flat file convention** (not per-skill subdirectories):

    .local/Agents/Skills/
    ├── README.md
    ├── jat-skill-name.skill.md
    ├── jat-another-skill.skill.md
    └── ... (more skills)

All reference material is embedded inline in the `.skill.md` file; no separate `/references/` subdirectories are used.

### 9.2 Naming and discovery ###

Skill files must follow these rules:

    * File name: lowercase-with-hyphens + `.skill.md` (e.g., `jat-command-reducer-snapshot-flow.skill.md`)
    * YAML `name:` field must match the file stem (e.g., `jat-command-reducer-snapshot-flow`)
    * `description:` field MUST include trigger keywords (e.g., "Use when: command, reducer, snapshot, State Store")
    * Trigger keywords enable agent discovery when your request mentions them

### 9.3 When to create skills ##

Create a new skill when:

    * A workflow is specialized and should load only for specific tasks (not always-on)
    * Implementation patterns are complex or repetitive and would benefit from bundled reference material
    * Domain knowledge is missing from existing contracts or instructions
    * Maintenance or debugging guidance would reduce context overhead for agents

Do NOT create a skill for guidance that should always apply to a subsystem; use instructions instead.

### 9.4 Skill catalog maintenance ###

The `README.md` in `.local/Agents/Skills/` maintains a comprehensive catalog.

This catalog is automatically synchronized by the `skills-index-auto-sync.hook.md` when `.skill.md` files are created, renamed, or deleted.

Agents SHOULD NOT manually edit the skill catalog; the hook ensures consistency.

### 9.5 Linking to contracts and instructions ###

Each skill should link back to related contracts or instructions at the end:

    ## Links ##
    - [BACKEND-ARCHITECTURE-CONTRACT](../Contracts/BACKEND-ARCHITECTURE-CONTRACT.instructions.md)
    - [related-instruction-name](../Instructions/related-instruction-name.instructions.md)

This keeps skills grounded in authoritative guidance and helps developers navigate the ecosystem.

## 10. Maintainer Authority ##

The maintainer is the final authority over:

    - architecture decisions
    - design direction
    - implementation approach
    - repository organization

Agents MUST treat explicit user instructions as the highest priority constraint.

## 11. Escalation Conditions ##

Agents should stop and ask the user before proceeding when:

    - requirements are ambiguous
    - multiple architectural paths exist
    - the solution expands scope beyond the request
    - an operation would trigger a confirmation gate

## Summary ##

This contract ensures that agents operating in the JAT workspace:

    - respect explicit scope
    - default to planning for complex tasks
    - avoid silent architectural decisions
    - interact transparently with the maintainer
    - safely use external knowledge
    - follow skill file conventions for discoverability and organization
