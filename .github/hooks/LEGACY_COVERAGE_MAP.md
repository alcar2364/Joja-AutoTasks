# Legacy Hook Coverage Map

This map binds each legacy markdown hook spec to an executable runtime bundle.

| Legacy Hook Spec | Runtime Bundle | Runtime Files |
| --- | --- | --- |
| agent-capability-freshness | ecosystem-maintenance | `.github/hooks/ecosystem-maintenance/hooks.json`, `.github/hooks/ecosystem-maintenance/ecosystem-maintenance.sh` |
| agent-ecosystem-sync | ecosystem-maintenance | `.github/hooks/ecosystem-maintenance/hooks.json`, `.github/hooks/ecosystem-maintenance/ecosystem-maintenance.sh` |
| anti-slop-enforcer | safety-guardrails | `.github/hooks/safety-guardrails/hooks.json`, `.github/hooks/safety-guardrails/safety-guardrails.sh` |
| ci-cd-workflow-loader | context-preflight | `.github/hooks/context-preflight/hooks.json`, `.github/hooks/context-preflight/context-preflight.sh` |
| clarity-enforcer | safety-guardrails | `.github/hooks/safety-guardrails/hooks.json`, `.github/hooks/safety-guardrails/safety-guardrails.sh` |
| context-engineering-loader | context-preflight | `.github/hooks/context-preflight/hooks.json`, `.github/hooks/context-preflight/context-preflight.sh` |
| contract-auto-loader | context-preflight | `.github/hooks/context-preflight/hooks.json`, `.github/hooks/context-preflight/context-preflight.sh` |
| design-guide-context-augmenter | context-preflight | `.github/hooks/context-preflight/hooks.json`, `.github/hooks/context-preflight/context-preflight.sh` |
| design-guide-contract-sync | ecosystem-maintenance | `.github/hooks/ecosystem-maintenance/hooks.json`, `.github/hooks/ecosystem-maintenance/ecosystem-maintenance.sh` |
| doc-sync-reminder | validation-postflight | `.github/hooks/validation-postflight/hooks.json`, `.github/hooks/validation-postflight/validation-postflight.sh` |
| handoff-optimizer | context-preflight | `.github/hooks/context-preflight/hooks.json`, `.github/hooks/context-preflight/context-preflight.sh` |
| identifier-validation | validation-postflight | `.github/hooks/validation-postflight/hooks.json`, `.github/hooks/validation-postflight/validation-postflight.sh` |
| performance-advisor | safety-guardrails | `.github/hooks/safety-guardrails/hooks.json`, `.github/hooks/safety-guardrails/safety-guardrails.sh` |
| persistence-safety-validator | validation-postflight | `.github/hooks/validation-postflight/hooks.json`, `.github/hooks/validation-postflight/validation-postflight.sh` |
| prompt-index-auto-sync | ecosystem-maintenance | `.github/hooks/ecosystem-maintenance/hooks.json`, `.github/hooks/ecosystem-maintenance/ecosystem-maintenance.sh` |
| security-validator | safety-guardrails | `.github/hooks/safety-guardrails/hooks.json`, `.github/hooks/safety-guardrails/safety-guardrails.sh` |
| skills-index-auto-sync | ecosystem-maintenance | `.github/hooks/ecosystem-maintenance/hooks.json`, `.github/hooks/ecosystem-maintenance/ecosystem-maintenance.sh` |
| state-mutation-guard | safety-guardrails | `.github/hooks/safety-guardrails/hooks.json`, `.github/hooks/safety-guardrails/safety-guardrails.sh` |
| ui-boundary-enforcer | safety-guardrails | `.github/hooks/safety-guardrails/hooks.json`, `.github/hooks/safety-guardrails/safety-guardrails.sh` |
| unit-test-coverage-enforcer | validation-postflight | `.github/hooks/validation-postflight/hooks.json`, `.github/hooks/validation-postflight/validation-postflight.sh` |

## Enforcement

- Runtime verification is executed by `.github/hooks/legacy-coverage-audit/legacy-coverage-audit.sh`.
- If a new legacy hook spec is added, this map and the audit script must be updated together.
