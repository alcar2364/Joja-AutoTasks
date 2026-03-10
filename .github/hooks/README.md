# JAT Agent Hooks System #

Hooks are now implemented as executable bundles under `.github/hooks/*/`.
Each bundle contains:

    * `hooks.json` (runtime trigger registration)
    * at least one `.sh` script (runtime command)

This matches the runnable hook format and keeps hooks local to the JAT workspace.

## Active Runtime Hook Bundles ##

### 1) context-preflight ###

Path: `.github/hooks/context-preflight/`

Runtime trigger:

    * `userPromptSubmitted`

Domain:

    * context loading and planning preflight
    * memory-operation routing reminder (route memory tasks to BrainAgent)
    * combines former intent from:
        - contract-auto-loader
        - context-engineering-loader
        - design-guide-context-augmenter
        - ci-cd-workflow-loader
        - handoff-optimizer

### 2) safety-guardrails ###

Path: `.github/hooks/safety-guardrails/`

Runtime trigger:

    * `userPromptSubmitted`

Domain:

    * quality and safety guardrails before implementation
    * combines former intent from:
        - anti-slop-enforcer
        - security-validator
        - performance-advisor
        - clarity-enforcer
        - state-mutation-guard
        - ui-boundary-enforcer

### 3) validation-postflight ###

Path: `.github/hooks/validation-postflight/`

Runtime trigger:

    * `sessionEnd`

Domain:

    * post-edit validation and quality checks
    * memory sync enforcement: block on session end when customization changes occur without `.github/memory/` updates
    * combines former intent from:
        - identifier-validation
        - persistence-safety-validator
        - unit-test-coverage-enforcer
        - doc-sync-reminder

### 4) ecosystem-maintenance ###

Path: `.github/hooks/ecosystem-maintenance/`

Runtime trigger:

    * `sessionEnd`

Domain:

    * customization ecosystem integrity and index sync reminders
    * combines former intent from:
        - agent-capability-freshness
        - agent-boundaries-wiring-sync
        - agent-ecosystem-sync
        - design-guide-contract-sync
        - prompt-index-auto-sync
        - skills-index-auto-sync

### 5) legacy-coverage-audit ###

Path: `.github/hooks/legacy-coverage-audit/`

Runtime trigger:

    * `sessionEnd`

Domain:

    * validates that every legacy markdown hook scenario maps to an executable runtime bundle
    * enforces `.github/hooks/LEGACY_COVERAGE_MAP.md`

## Legacy Hook Specs ##

Previous markdown-only hook specs were archived (not deleted) at:

    * `.github/hooks/legacy-md/`

Legacy coverage mapping is maintained in:

    * `.github/hooks/LEGACY_COVERAGE_MAP.md`

These files are reference material and do not execute directly.

## Trigger Model ##

Active runtime events used:

    * `userPromptSubmitted` (preflight checks)
    * `sessionEnd` (postflight validation and sync)

## Maintenance ##

When adding or modifying hooks:

1. Keep one clear domain per hook bundle.
2. Avoid overlap between bundles.
3. Keep `hooks.json` valid JSON with correct script paths.
4. Keep scripts deterministic, fast, and non-destructive.

Created by: GodAgent
Version: 3.2
Last Updated: 2026-03-10
