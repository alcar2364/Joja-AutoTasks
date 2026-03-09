<!-- markdownlint-disable -->

# JAT Prompt Index

This folder contains reusable prompt templates for the current JAT custom-agent architecture.

## Quick Picker

If unsure where to start, use [choose-workflow-prompt.prompt.md](choose-workflow-prompt.prompt.md).

- Need orchestration across multiple steps/agents: [orchestrate-work-item.prompt.md](orchestrate-work-item.prompt.md)
- Need context first: [research-codebase-context.prompt.md](research-codebase-context.prompt.md)
- Need plan only: [create-implementation-plan.prompt.md](create-implementation-plan.prompt.md)
- Need implementation work:
  - Backend engine/state/persistence: [implement-backend-engine-change.prompt.md](implement-backend-engine-change.prompt.md)
  - Frontend HUD/menu/view-model: [implement-frontend-ui-change.prompt.md](implement-frontend-ui-change.prompt.md)
  - StarML markup: [implement-starml-markup-change.prompt.md](implement-starml-markup-change.prompt.md)
- Need tests or review:
  - Unit tests: [create-or-review-unit-tests.prompt.md](create-or-review-unit-tests.prompt.md)
  - Contract review: [review-for-contract-compliance.prompt.md](review-for-contract-compliance.prompt.md)
- Need special workflows:
  - Refactor: [safe-structural-refactor.prompt.md](safe-structural-refactor.prompt.md)
  - Troubleshoot: [troubleshoot-build-or-runtime.prompt.md](troubleshoot-build-or-runtime.prompt.md)
  - Agent customization: [agent-customization-task.prompt.md](agent-customization-task.prompt.md)
  - Ecosystem audit: [audit-customization-ecosystem.prompt.md](audit-customization-ecosystem.prompt.md)
  - Legacy hook coverage: [verify-hook-legacy-coverage.prompt.md](verify-hook-legacy-coverage.prompt.md)
  - Atomic commit checklist: [create-atomic-commit-execution-checklist.prompt.md](create-atomic-commit-execution-checklist.prompt.md)
  - Prompt index sync: [refresh-prompt-index.prompt.md](refresh-prompt-index.prompt.md)
  - Workspace docs/plans: [workspace-documentation-task.prompt.md](workspace-documentation-task.prompt.md)

## Prompt Catalog

| Prompt File | Prompt Name | Best Use | Agent |
| --- | --- | --- | --- |
| [choose-workflow-prompt.prompt.md](choose-workflow-prompt.prompt.md) | Choose Workflow Prompt | Select or execute the best workflow prompt by task type and constraints | Orchestrator |
| [orchestrate-work-item.prompt.md](orchestrate-work-item.prompt.md) | Orchestrate Work Item | Route mixed or cross-agent work safely end-to-end | Orchestrator |
| [research-codebase-context.prompt.md](research-codebase-context.prompt.md) | Research Codebase Context | Gather file/symbol context and patterns before planning/coding | Researcher |
| [create-implementation-plan.prompt.md](create-implementation-plan.prompt.md) | Create Implementation Plan | Produce a low-risk, step-by-step plan without edits | Planner |
| [implement-backend-engine-change.prompt.md](implement-backend-engine-change.prompt.md) | Implement Backend Engine Change | Backend/state/rule/persistence implementation | GameAgent |
| [implement-frontend-ui-change.prompt.md](implement-frontend-ui-change.prompt.md) | Implement Frontend UI Change | HUD/menu/view-model implementation | UIAgent |
| [implement-starml-markup-change.prompt.md](implement-starml-markup-change.prompt.md) | Implement StarML Markup Change | .sml layout/binding/template/event edits | StarMLAgent |
| [create-or-review-unit-tests.prompt.md](create-or-review-unit-tests.prompt.md) | Create Or Review Unit Tests | Create tests, expand coverage, or review test adequacy | UnitTestAgent |
| [review-for-contract-compliance.prompt.md](review-for-contract-compliance.prompt.md) | Review For Contract Compliance | Contract-focused code/plan review and risk findings | Reviewer |
| [safe-structural-refactor.prompt.md](safe-structural-refactor.prompt.md) | Safe Structural Refactor | Behavior-safe structural refactors | Refactorer |
| [troubleshoot-build-or-runtime.prompt.md](troubleshoot-build-or-runtime.prompt.md) | Troubleshoot Build Or Runtime Issue | Root-cause analysis for build/runtime/tooling issues | Troubleshooter |
| [agent-customization-task.prompt.md](agent-customization-task.prompt.md) | Agent Customization Task | Create/tune/debug customization assets and ecosystem coherence | GodAgent |
| [audit-customization-ecosystem.prompt.md](audit-customization-ecosystem.prompt.md) | Audit Customization Ecosystem | Full .github customization-system audit with overlap and coverage checks | GodAgent |
| [verify-hook-legacy-coverage.prompt.md](verify-hook-legacy-coverage.prompt.md) | Verify Hook Legacy Coverage | Validate that every legacy hook scenario maps to executable runtime hooks | GodAgent |
| [create-atomic-commit-execution-checklist.prompt.md](create-atomic-commit-execution-checklist.prompt.md) | Create Atomic Commit Execution Checklist | Generate step-by-step atomic commit checklist from design guide phase | Orchestrator |
| [refresh-prompt-index.prompt.md](refresh-prompt-index.prompt.md) | Refresh Prompt Index | Sync prompt README catalog after prompt add/rename/remove changes | Orchestrator |
| [workspace-documentation-task.prompt.md](workspace-documentation-task.prompt.md) | Workspace Documentation Task | Plans, design docs, and user-facing workspace artifacts | WorkspaceAgent |

## Customization Notes

- Keep each prompt focused on one task.
- Update `description` and `argument-hint` when your preferred intake changes.
- Keep `agent` aligned to an existing file in [..](..) under `.agent.md`.
- Add new prompts to this index so future sessions can discover them quickly.
- Event-driven sync reminders are emitted by [../hooks/ecosystem-maintenance/ecosystem-maintenance.sh](../hooks/ecosystem-maintenance/ecosystem-maintenance.sh).
- Use [refresh-prompt-index.prompt.md](refresh-prompt-index.prompt.md) as manual fallback for preview/apply repair.
