---
name: agent-boundaries-and-wiring-governance
description: "Authoritative no-overlap map and governance rules for keeping agent-domain and instruction/skill wiring aligned."
applyTo: "{.github/agents/*.agent.md,.github/instructions/*.instructions.md,.github/skills/*/SKILL.md,.github/hooks/**/*.sh,.github/hooks/**/hooks.json,.github/prompts/*.prompt.md}"
---

# Agent Boundaries and Wiring Governance

## Purpose

This instruction file is the authoritative no-overlap map for JAT agents, instruction routing, and skill usage.

## Agent Domains (Non-Overlapping)

| Agent             | Exclusive Function                                                                                                                                                                          | Explicit Exclusions                                                                                 |
| ----------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------- |
| Orchestrator      | Delegation and workflow routing only                                                                                                                                                        | No direct research, planning, implementation, testing, troubleshooting, or review work              |
| GodAgent          | Agent customization ecosystem files only                                                                                                                                                    | No gameplay/UI/business implementation and no general code reviews                                  |
| Researcher        | Context discovery and evidence gathering only                                                                                                                                               | No implementation and no final review verdicts                                                      |
| Planner           | Step-by-step implementation planning only                                                                                                                                                   | No direct code edits and no markdown drafting                                                       |
| Reviewer          | Contract/risk review only                                                                                                                                                                   | No direct implementation or markdown drafting                                                       |
| GitAgent          | Git repository maintenance, commit composition, and git workflow guidance                                                                                                                   | No non-git feature implementation ownership                                                         |
| CSharpMentor      | C# mentorship and guidance-only coaching with optional explicit user-requested edits                                                                                                        | No autonomous large-scope implementation ownership and no bypass of architecture/style contracts    |
| GameAgent         | Backend/engine/state/persistence C# implementation                                                                                                                                          | No StarML authoring and no documentation ownership                                                  |
| UIAgent           | Frontend C# view-model/UI interaction implementation                                                                                                                                        | No .sml authoring and no backend canonical-state ownership                                          |
| StarMLAgent       | .sml layout/template/binding authoring only                                                                                                                                                 | No backend logic implementation and no non-markup C# ownership                                      |
| Refactorer        | Structural refactor execution                                                                                                                                                               | No behavior-changing feature work unless explicitly authorized                                      |
| UnitTestAgent     | Unit-test creation and test review                                                                                                                                                          | No non-test feature implementation ownership                                                        |
| Troubleshooter    | Diagnosis, root-cause analysis, and documentation-follow-up routing after confirmed cause                                                                                                   | No speculative large rewrites by default and no direct ownership of non-agent documentation editing |
| WorkspaceAgent    | Non-agent documentation artifacts                                                                                                                                                           | No agent customization files (.agent.md/.instructions.md/.prompt.md/SKILL.md/hooks.json)            |
| agentic-workflows | **Platform-managed agent (gh-aw framework).** Dispatcher to gh-aw specialized prompts; uses `disable-model-invocation: true` and does not participate in JAT agent delegation or governance | Not a JAT custom agent; no JAT instruction wiring and no JAT skill assignments                      |

## Instruction-to-Agent Wiring

Every instruction file below is wired to at least one agent.

| Instruction                                                | Primary Agent  | Also Used By                                                                                                                        |
| ---------------------------------------------------------- | -------------- | ----------------------------------------------------------------------------------------------------------------------------------- |
| agent-boundaries-and-wiring-governance.instructions.md     | GodAgent       | Orchestrator, Reviewer                                                                                                              |
| atomic-commit-execution-checklist-creation.instructions.md | Planner        | Researcher, Reviewer, WorkspaceAgent, Orchestrator                                                                                  |
| backend-architecture-contract.instructions.md              | GameAgent      | Planner, Reviewer, Refactorer                                                                                                       |
 
| csharp-style-contract.instructions.md                      | GameAgent      | UIAgent, Refactorer, UnitTestAgent, Reviewer, CSharpMentor                                                                          |
| external-resources.instructions.md                         | Researcher     | GameAgent, UIAgent, StarMLAgent, Troubleshooter                                                                                     |
| frontend-architecture-contract.instructions.md             | UIAgent        | StarMLAgent, Reviewer, Planner                                                                                                      |
| grepai-semantic-search.instructions.md                     | Researcher     | GameAgent, UIAgent, Refactorer, Reviewer, Planner, Troubleshooter, CSharpMentor                                                     |
| github-actions-ci-cd-best-practices.instructions.md        | Troubleshooter | WorkspaceAgent, GodAgent                                                                                                            |
| json-style-contract.instructions.md                        | GameAgent      | Refactorer, Reviewer                                                                                                                |
| performance-optimization.instructions.md                   | Troubleshooter | GameAgent, UIAgent, Reviewer                                                                                                        |
| powershell-terminal-best-practices.instructions.md         | Troubleshooter | All agents using run_in_terminal, including GitAgent                                                                                |
| review-and-verification-contract.instructions.md           | Reviewer       | Orchestrator, UnitTestAgent                                                                                                         |
| security-and-owasp.instructions.md                         | Reviewer       | Troubleshooter, GameAgent, UIAgent                                                                                                  |
| self-explanatory-code-commenting.instructions.md           | Refactorer     | GameAgent, UIAgent, UnitTestAgent                                                                                                   |
| sml-style-contract.instructions.md                         | StarMLAgent    | UIAgent, Reviewer                                                                                                                   |
| starml-cheatsheet.instructions.md                          | StarMLAgent    | UIAgent, Researcher                                                                                                                 |
| ui-component-patterns.instructions.md                      | UIAgent        | StarMLAgent, Planner                                                                                                                |
| ui-hud-patterns.instructions.md                            | UIAgent        | StarMLAgent, Planner                                                                                                                |
| ui-interaction-patterns.instructions.md                    | UIAgent        | StarMLAgent, Planner                                                                                                                |
| ui-menu-patterns.instructions.md                           | UIAgent        | StarMLAgent, Planner                                                                                                                |
| unit-testing-contract.instructions.md                      | UnitTestAgent  | Reviewer, Planner                                                                                                                   |
| update-docs-on-code-change.instructions.md                 | WorkspaceAgent | Reviewer, Troubleshooter                                                                                                            |
| visual-design-language.instructions.md                     | UIAgent        | StarMLAgent, WorkspaceAgent                                                                                                         |
| workspace-contracts.instructions.md                        | Orchestrator   | GodAgent, Planner, Reviewer, WorkspaceAgent, CSharpMentor                                                                           |

## Skill-to-Agent Wiring
**Advisory note:** In VS Code, skill triggering is automatic and discovery-based — the IDE discovers skills by scanning `SKILL.md` files directly. The wiring table below is **advisory only**; it documents conventional ownership for governance purposes but is **not required** for skill invocation. Future maintainers must not add speculative bulk wiring rows for skills that have no established agent ownership rationale.

Every skill folder in `.github/skills` is mapped to at least one agent.

| Skill                                                 | Primary Agent  | Also Used By                    |
| ----------------------------------------------------- | -------------- | ------------------------------- |
| create-architectural-decision-record                  | Planner        | WorkspaceAgent                  |
| create-github-issue-feature-from-specification        | WorkspaceAgent | Planner                         |
| create-github-issues-feature-from-implementation-plan | WorkspaceAgent | Planner                         |
| create-github-pull-request-from-specification         | WorkspaceAgent | Reviewer                        |
| create-implementation-plan                            | Planner        | Orchestrator                    |
| create-readme                                         | WorkspaceAgent | GodAgent                        |
| csharp-mentor-response-templates                      | CSharpMentor   | Reviewer                        |
 
| csharp-docs                                           | WorkspaceAgent | Reviewer                        |
| atomic-commit-execution-checklist-creation           | Planner        | WorkspaceAgent                  |
| csharp-mstest                                         | UnitTestAgent  | Reviewer                        |
| csharp-xunit                                          | UnitTestAgent  | Reviewer                        |
| dotnet-best-practices                                 | GameAgent      | Refactorer, Reviewer            |
| dotnet-upgrade                                        | Refactorer     | Troubleshooter                  |
| ef-core                                               | GameAgent      | Refactorer                      |
| git-commit                                            | GitAgent       | Orchestrator, WorkspaceAgent    |
| godagent-workflow-patterns-and-assets                 | GodAgent       | Orchestrator                    |
| jat-build-debug-and-deployment-workflow               | Troubleshooter | GameAgent                       |
| jat-command-reducer-snapshot-flow                     | GameAgent      | Reviewer                        |
| jat-dependency-injection-and-composition              | GameAgent      | Refactorer                      |
| jat-error-handling-and-validation-patterns            | GameAgent      | Troubleshooter                  |
| jat-event-lifecycle-and-game-coupling                 | GameAgent      | Researcher                      |
| jat-external-resources                                | Researcher     | GameAgent, UIAgent, StarMLAgent |
| jat-identifier-determinism-patterns                   | GameAgent      | UnitTestAgent, Reviewer         |
| jat-persistence-migration-and-reconstruction          | GameAgent      | Reviewer                        |
| jat-smapi-debugging-and-diagnostics                   | Troubleshooter | GameAgent                       |
| jat-snapshot-binding-and-ui-data-flow                 | UIAgent        | StarMLAgent                     |
| jat-starml-cheatsheet                                 | StarMLAgent    | UIAgent                         |
| jat-task-generation-and-rule-evaluation               | GameAgent      | Planner                         |
| jat-testing-patterns-and-fixtures                     | UnitTestAgent  | Reviewer                        |
| jat-ui-component-patterns                             | UIAgent        | StarMLAgent                     |
| jat-visual-design-language                            | UIAgent        | StarMLAgent, WorkspaceAgent     |
| planner-checklist-and-output-format                   | Planner        | Reviewer, Orchestrator          |
 
| reviewer-checklist-and-output-format                  | Reviewer       | Orchestrator                    |
| self-splitting-parallel-execution                     | GameAgent      | UIAgent, UnitTestAgent, Refactorer, Researcher, WorkspaceAgent |
| starml-output-format                                  | StarMLAgent    | UIAgent                         |
| troubleshooter-output-format                          | Troubleshooter | Orchestrator                    |
 
| update-specification                                  | Planner        | WorkspaceAgent                  |

## Required updates

Agents MUST update this instruction file in the same change set when any of the following occurs:

1. A new instruction file is added under `.github/instructions/`.
2. A new skill is added under `.github/skills/*/SKILL.md`.
3. Agent scope changes in any `.github/agents/*.agent.md` file.
4. Any agent is created or updated: run an adjacent-artifact audit for **skill**, **prompt**, and **hook** coverage; create/update artifacts when needed or explicitly record `not needed` with rationale.

## Scope-change rule

When agent scope changes, update the **Agent Domains (Non-Overlapping)** table first to prevent overlap drift.

Scope change includes edits to agent responsibilities, exclusions, boundaries, or frontmatter that materially changes an agent's functional domain.

## Cross-Cutting Workflow Patterns

### Troubleshooter → GodAgent: Recurring Problem Detection

When Troubleshooter identifies a recurring problem pattern that indicates agent behavior needs improvement (not code bugs), it delegates to GodAgent with:

- The recurring pattern description
- Which agent(s) need instruction updates
- Specific prevention rules to add
- Concrete examples of the mistake

**Criteria for delegation:**

- Same mistake type occurred multiple times
- Root cause is agent behavior, not code logic
- Fix requires updating agent customization files
- Can articulate specific prevention rule

This workflow does **not** change Troubleshooter's scope (still diagnosis only) or GodAgent's scope (still agent customization only). It establishes a routing pattern for diagnosis results that indicate systemic agent ecosystem issues.

### Troubleshooter -> WorkspaceAgent: Architecture-Significant Root-Cause Documentation

When Troubleshooter confirms a root cause that reveals a major architecture problem (especially from agent-generated code), it delegates to WorkspaceAgent with:

- Problem summary and trigger conditions
- Confirmed root cause (with emphasis on architectural significance)
- Fix summary and verification evidence
- Suggested target docs and exact updates needed
## Required updates
## Hook enforcement

Runtime enforcement is wired through `.github/hooks/ecosystem-maintenance/ecosystem-maintenance.sh` on `sessionEnd`.

If required map updates are missing, the hook MUST fail with a blocking error.
