# JAT Skills Index

This folder contains on-demand skills for the JAT agent ecosystem.

## Structure

- Every skill lives in a folder: `.github/skills/<skill-name>/`
- The main skill file is always: `SKILL.md`
- Folder name must exactly match `name:` in `SKILL.md`
- Optional supporting assets live in `references/`

## Skill Catalog

| Skill                                                 | File                                                                                                                             |
| ----------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------- |
| create-architectural-decision-record                  | [create-architectural-decision-record/SKILL.md](create-architectural-decision-record/SKILL.md)                                   |
| create-github-issue-feature-from-specification        | [create-github-issue-feature-from-specification/SKILL.md](create-github-issue-feature-from-specification/SKILL.md)               |
| create-github-issues-feature-from-implementation-plan | [create-github-issues-feature-from-implementation-plan/SKILL.md](create-github-issues-feature-from-implementation-plan/SKILL.md) |
| create-github-pull-request-from-specification         | [create-github-pull-request-from-specification/SKILL.md](create-github-pull-request-from-specification/SKILL.md)                 |
| create-implementation-plan                            | [create-implementation-plan/SKILL.md](create-implementation-plan/SKILL.md)                                                       |
| create-readme                                         | [create-readme/SKILL.md](create-readme/SKILL.md)                                                                                 |
| atomic-commit-execution-checklist-creation | [atomic-commit-execution-checklist-creation/SKILL.md](atomic-commit-execution-checklist-creation/SKILL.md) |
| create-specification                                  | [create-specification/SKILL.md](create-specification/SKILL.md)                                                                   |
| csharp-docs                                           | [csharp-docs/SKILL.md](csharp-docs/SKILL.md)                                                                                     |
| csharp-mstest                                         | [csharp-mstest/SKILL.md](csharp-mstest/SKILL.md)                                                                                 |
| csharp-xunit                                          | [csharp-xunit/SKILL.md](csharp-xunit/SKILL.md)                                                                                   |
| dotnet-best-practices                                 | [dotnet-best-practices/SKILL.md](dotnet-best-practices/SKILL.md)                                                                 |
| dotnet-upgrade                                        | [dotnet-upgrade/SKILL.md](dotnet-upgrade/SKILL.md)                                                                               |
| ef-core                                               | [ef-core/SKILL.md](ef-core/SKILL.md)                                                                                             |
| git-commit                                            | [git-commit/SKILL.md](git-commit/SKILL.md)                                                                                       |
| jat-build-debug-and-deployment-workflow               | [jat-build-debug-and-deployment-workflow/SKILL.md](jat-build-debug-and-deployment-workflow/SKILL.md)                             |
| jat-command-reducer-snapshot-flow                     | [jat-command-reducer-snapshot-flow/SKILL.md](jat-command-reducer-snapshot-flow/SKILL.md)                                         |
| jat-dependency-injection-and-composition              | [jat-dependency-injection-and-composition/SKILL.md](jat-dependency-injection-and-composition/SKILL.md)                           |
| jat-error-handling-and-validation-patterns            | [jat-error-handling-and-validation-patterns/SKILL.md](jat-error-handling-and-validation-patterns/SKILL.md)                       |
| jat-event-lifecycle-and-game-coupling                 | [jat-event-lifecycle-and-game-coupling/SKILL.md](jat-event-lifecycle-and-game-coupling/SKILL.md)                                 |
| jat-external-resources                                | [jat-external-resources/SKILL.md](jat-external-resources/SKILL.md)                                                               |
| jat-identifier-determinism-patterns                   | [jat-identifier-determinism-patterns/SKILL.md](jat-identifier-determinism-patterns/SKILL.md)                                     |
| jat-persistence-migration-and-reconstruction          | [jat-persistence-migration-and-reconstruction/SKILL.md](jat-persistence-migration-and-reconstruction/SKILL.md)                   |
| jat-smapi-debugging-and-diagnostics                   | [jat-smapi-debugging-and-diagnostics/SKILL.md](jat-smapi-debugging-and-diagnostics/SKILL.md)                                     |
| jat-snapshot-binding-and-ui-data-flow                 | [jat-snapshot-binding-and-ui-data-flow/SKILL.md](jat-snapshot-binding-and-ui-data-flow/SKILL.md)                                 |
| jat-starml-cheatsheet                                 | [jat-starml-cheatsheet/SKILL.md](jat-starml-cheatsheet/SKILL.md)                                                                 |
| jat-task-generation-and-rule-evaluation               | [jat-task-generation-and-rule-evaluation/SKILL.md](jat-task-generation-and-rule-evaluation/SKILL.md)                             |
| jat-testing-patterns-and-fixtures                     | [jat-testing-patterns-and-fixtures/SKILL.md](jat-testing-patterns-and-fixtures/SKILL.md)                                         |
| jat-ui-component-patterns                             | [jat-ui-component-patterns/SKILL.md](jat-ui-component-patterns/SKILL.md)                                                         |
| jat-visual-design-language                            | [jat-visual-design-language/SKILL.md](jat-visual-design-language/SKILL.md)                                                       |
| refactor                                              | [refactor/SKILL.md](refactor/SKILL.md)                                                                                           |
| update-implementation-plan                            | [update-implementation-plan/SKILL.md](update-implementation-plan/SKILL.md)                                                       |
| update-specification                                  | [update-specification/SKILL.md](update-specification/SKILL.md)                                                                   |

## Wiring Requirement

Every skill above must be mapped to at least one agent in `.github/instructions/agent-boundaries-and-wiring-governance.instructions.md`.
