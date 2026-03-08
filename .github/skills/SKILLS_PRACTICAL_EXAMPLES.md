# Skill Usage Examples

This file shows practical mapping examples aligned with `.github/instructions/agent-boundaries-and-wiring-governance.instructions.md`.

## Backend Example

Request:
- "Implement deterministic task generation for recurring rules"

Primary agent:
- `GameAgent`

Mapped skills:
- `jat-task-generation-and-rule-evaluation`
- `jat-identifier-determinism-patterns`
- `jat-command-reducer-snapshot-flow`

## UI C# Example

Request:
- "Update menu selection behavior and snapshot-driven row states"

Primary agent:
- `UIAgent`

Mapped skills:
- `jat-snapshot-binding-and-ui-data-flow`
- `jat-ui-component-patterns`
- `jat-visual-design-language`

## StarML Example

Request:
- "Refactor .sml templates and event bindings"

Primary agent:
- `StarMLAgent`

Mapped skills:
- `jat-starml-cheatsheet`
- `jat-ui-component-patterns`

## Testing Example

Request:
- "Add deterministic tests for ID stability and reducer behavior"

Primary agent:
- `UnitTestAgent`

Mapped skills:
- `jat-testing-patterns-and-fixtures`
- `csharp-xunit`

## Troubleshooting Example

Request:
- "Debug SMAPI startup failures after refactor"

Primary agent:
- `Troubleshooter`

Mapped skills:
- `jat-smapi-debugging-and-diagnostics`
- `jat-build-debug-and-deployment-workflow`

## Documentation Example

Request:
- "Create a release summary and update docs"

Primary agent:
- `WorkspaceAgent`

Mapped skills:
- `create-readme`
- `csharp-docs`

## Maintenance Rule

When adding a new skill:

1. Add folder + `SKILL.md` under `.github/skills/`.
2. Add catalog entry in `.github/skills/README.md`.
3. Add agent mapping in `.github/instructions/agent-boundaries-and-wiring-governance.instructions.md`.
