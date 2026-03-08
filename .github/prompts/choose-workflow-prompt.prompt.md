---
name: "Choose Workflow Prompt"
description: "Use when: selecting the best JAT prompt by task type, scope, and constraints before execution."
argument-hint: "Goal + task type + subsystem + constraints"
agent: "Orchestrator"
---

Select the best prompt from the JAT prompt catalog and either recommend it or execute it through the correct workflow.

Task Intake
- Goal: <required>
- Task type: <feature|bug|refactor|review|tests|troubleshooting|agent-customization|documentation|prompt-maintenance|unknown>
- Target subsystem(s): <backend|frontend|starml|tests|mixed|unknown>
- Scope limits: <single file|no behavior change|analysis only|other>
- Execution mode: <recommend only|recommend and execute>

Prompt Catalog Mapping (customizable)
- orchestrate-work-item.prompt.md -> mixed scope, uncertain scope, or cross-agent orchestration
- research-codebase-context.prompt.md -> context discovery before planning or coding
- create-implementation-plan.prompt.md -> step-by-step implementation plan only
- implement-backend-engine-change.prompt.md -> backend engine/state/rules/persistence implementation
- implement-frontend-ui-change.prompt.md -> HUD/menu/view-model implementation
- implement-starml-markup-change.prompt.md -> .sml layout/binding/template/event changes
- create-or-review-unit-tests.prompt.md -> new tests, coverage expansion, or test review
- review-for-contract-compliance.prompt.md -> contract/correctness/risk review
- safe-structural-refactor.prompt.md -> rename/extract/move refactor work
- troubleshoot-build-or-runtime.prompt.md -> build/runtime/tooling diagnostics
- agent-customization-task.prompt.md -> .agent/.instructions/.prompt/SKILL/hooks customization
- workspace-documentation-task.prompt.md -> plans, docs, and task-list artifacts
- refresh-prompt-index.prompt.md -> sync prompt README catalog with current prompt files

Selection Rules
1. If task type is unknown or request spans multiple domains, choose orchestrate-work-item.prompt.md.
2. If user explicitly asks for planning only, choose create-implementation-plan.prompt.md.
3. If scope constraint says analysis only, prefer research-codebase-context.prompt.md or review-for-contract-compliance.prompt.md based on intent.
4. If task type is prompt-maintenance, choose refresh-prompt-index.prompt.md.
5. If execution mode is recommend and execute, run the selected workflow end-to-end.
6. If execution mode is recommend only, do not edit files unless explicitly asked.

Required Output
1. Primary prompt choice with brief rationale
2. Optional secondary prompt choices
3. Pre-filled prompt body for immediate use
4. If executing: actions taken, touched files, and residual risks

Customization Area
- Additional prompt mappings:
  - <prompt-file-name> -> <when to use>
