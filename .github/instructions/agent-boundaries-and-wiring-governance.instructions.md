---
name: agent-boundaries-and-wiring-governance
description: "Authoritative no-overlap map and governance rules for keeping agent-domain and instruction/skill wiring aligned."
applyTo: "{.github/agents/*.agent.md,.github/instructions/*.instructions.md,.github/skills/*/SKILL.md,.github/hooks/**/*.sh,.github/hooks/**/hooks.json,.github/prompts/*.prompt.md}"
---

# Agent Boundaries and Wiring Governance

## Purpose ##

This instruction file is the authoritative no-overlap map for JAT agents, instruction routing, and skill usage.

## Agent Domains (Non-Overlapping) ##

| Agent | Exclusive Function | Explicit Exclusions |
| --- | --- | --- |
| Orchestrator | Delegation and workflow routing only | No direct research, planning, implementation, testing, troubleshooting, or review work |
| GodAgent | Agent customization ecosystem files only | No gameplay/UI/business implementation and no general code reviews |
| Researcher | Context discovery and evidence gathering only | No implementation and no final review verdicts |
| Planner | Step-by-step implementation planning only | No direct code edits and no markdown drafting |
| Reviewer | Contract/risk review only | No direct implementation or markdown drafting |
| CSharpMentor | C# mentorship and guidance-only coaching with optional explicit user-requested edits | No autonomous large-scope implementation ownership and no bypass of architecture/style contracts |
| GameAgent | Backend/engine/state/persistence C# implementation | No StarML authoring and no documentation ownership |
| UIAgent | Frontend C# view-model/UI interaction implementation | No .sml authoring and no backend canonical-state ownership |
| StarMLAgent | .sml layout/template/binding authoring only | No backend logic implementation and no non-markup C# ownership |
| Refactorer | Structural refactor execution | No behavior-changing feature work unless explicitly authorized |
| UnitTestAgent | Unit-test creation and test review | No non-test feature implementation ownership |
| Troubleshooter | Diagnosis, root-cause analysis, and documentation-follow-up routing after confirmed cause | No speculative large rewrites by default and no direct ownership of non-agent documentation editing |
| WorkspaceAgent | Non-agent documentation artifacts | No agent customization files (.agent.md/.instructions.md/.prompt.md/SKILL.md/hooks.json) |

## Instruction-to-Agent Wiring ##

Every instruction file below is wired to at least one agent.

| Instruction | Primary Agent | Also Used By |
| --- | --- | --- |
| agent-boundaries-and-wiring-governance.instructions.md | GodAgent | Orchestrator, Reviewer |
| atomic-commit-execution-checklist-creation.instructions.md | Planner | Researcher, Reviewer, WorkspaceAgent, Orchestrator |
| backend-architecture-contract.instructions.md | GameAgent | Planner, Reviewer, Refactorer |
| context-engineering.instructions.md | Researcher | Planner, Orchestrator |
| csharp-style-contract.instructions.md | GameAgent | UIAgent, Refactorer, UnitTestAgent, Reviewer, CSharpMentor |
| external-resources.instructions.md | Researcher | GameAgent, UIAgent, StarMLAgent, Troubleshooter |
| frontend-architecture-contract.instructions.md | UIAgent | StarMLAgent, Reviewer, Planner |
| github-actions-ci-cd-best-practices.instructions.md | Troubleshooter | WorkspaceAgent, GodAgent |
| json-style-contract.instructions.md | GameAgent | Refactorer, Reviewer |
| performance-optimization.instructions.md | Troubleshooter | GameAgent, UIAgent, Reviewer |
| review-and-verification-contract.instructions.md | Reviewer | Orchestrator, UnitTestAgent |
| security-and-owasp.instructions.md | Reviewer | Troubleshooter, GameAgent, UIAgent |
| self-explanatory-code-commenting.instructions.md | Refactorer | GameAgent, UIAgent, UnitTestAgent |
| sml-style-contract.instructions.md | StarMLAgent | UIAgent, Reviewer |
| starml-cheatsheet.instructions.md | StarMLAgent | UIAgent, Researcher |
| ui-component-patterns.instructions.md | UIAgent | StarMLAgent, Planner |
| unit-testing-contract.instructions.md | UnitTestAgent | Reviewer, Planner |
| update-docs-on-code-change.instructions.md | WorkspaceAgent | Reviewer, Troubleshooter |
| visual-design-language.instructions.md | UIAgent | StarMLAgent, WorkspaceAgent |
| workspace-contracts.instructions.md | Orchestrator | GodAgent, Planner, Reviewer, WorkspaceAgent, CSharpMentor |

## Skill-to-Agent Wiring ##

Every skill folder in `.github/skills` is mapped to at least one agent.

| Skill | Primary Agent | Also Used By |
| --- | --- | --- |
| breakdown-feature-implementation | Orchestrator | Planner |
| create-architectural-decision-record | Planner | WorkspaceAgent |
| create-github-issue-feature-from-specification | WorkspaceAgent | Planner |
| create-github-issues-feature-from-implementation-plan | WorkspaceAgent | Planner |
| create-github-pull-request-from-specification | WorkspaceAgent | Reviewer |
| create-implementation-plan | Planner | Orchestrator |
| create-readme | WorkspaceAgent | GodAgent |
| create-specification | Planner | WorkspaceAgent |
| csharp-docs | WorkspaceAgent | Reviewer |
| csharp-mstest | UnitTestAgent | Reviewer |
| csharp-xunit | UnitTestAgent | Reviewer |
| dotnet-best-practices | GameAgent | Refactorer, Reviewer |
| dotnet-upgrade | Refactorer | Troubleshooter |
| ef-core | GameAgent | Refactorer |
| git-commit | Orchestrator | WorkspaceAgent |
| jat-build-debug-and-deployment-workflow | Troubleshooter | GameAgent |
| jat-command-reducer-snapshot-flow | GameAgent | Reviewer |
| jat-dependency-injection-and-composition | GameAgent | Refactorer |
| jat-error-handling-and-validation-patterns | GameAgent | Troubleshooter |
| jat-event-lifecycle-and-game-coupling | GameAgent | Researcher |
| jat-external-resources | Researcher | GameAgent, UIAgent, StarMLAgent |
| jat-identifier-determinism-patterns | GameAgent | UnitTestAgent, Reviewer |
| jat-persistence-migration-and-reconstruction | GameAgent | Reviewer |
| jat-smapi-debugging-and-diagnostics | Troubleshooter | GameAgent |
| jat-snapshot-binding-and-ui-data-flow | UIAgent | StarMLAgent |
| jat-starml-cheatsheet | StarMLAgent | UIAgent |
| jat-task-generation-and-rule-evaluation | GameAgent | Planner |
| jat-testing-patterns-and-fixtures | UnitTestAgent | Reviewer |
| jat-ui-component-patterns | UIAgent | StarMLAgent |
| jat-visual-design-language | UIAgent | StarMLAgent, WorkspaceAgent |
| refactor | Refactorer | Reviewer |
| update-implementation-plan | Planner | WorkspaceAgent |
| update-specification | Planner | WorkspaceAgent |

## Required updates ##

Agents MUST update this instruction file in the same change set when any of the following occurs:

1. A new instruction file is added under `.github/instructions/`.
2. A new skill is added under `.github/skills/*/SKILL.md`.
3. Agent scope changes in any `.github/agents/*.agent.md` file.

## Scope-change rule ##

When agent scope changes, update the **Agent Domains (Non-Overlapping)** table first to prevent overlap drift.

Scope change includes edits to agent responsibilities, exclusions, boundaries, or frontmatter that materially changes an agent's functional domain.

## Cross-Cutting Workflow Patterns ##

### Troubleshooter → GodAgent: Recurring Problem Detection ###

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

### Troubleshooter -> WorkspaceAgent: Architecture-Significant Root-Cause Documentation ###

When Troubleshooter confirms a root cause that reveals a major architecture problem (especially from agent-generated code), it delegates to WorkspaceAgent with:

- Problem summary and trigger conditions
- Confirmed root cause (with emphasis on architectural significance)
- Fix summary and verification evidence
- Suggested target docs and exact updates needed

**Threshold for delegation:** Major architecture problems, design-guide gaps, contract violations — not minor coding errors.

This workflow does **not** give Troubleshooter documentation ownership. Troubleshooter routes the documentation task; WorkspaceAgent owns non-agent Markdown editing.

## Hook enforcement ##

Runtime enforcement is wired through `.github/hooks/ecosystem-maintenance/ecosystem-maintenance.sh` on `sessionEnd`.

If required map updates are missing, the hook MUST fail with a blocking error.
