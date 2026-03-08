<!-- markdownlint-disable -->

# JAT Skills Index #

This folder contains on-demand skills for the JAT agent ecosystem.

## Skills vs Instructions (Quick Rule) ##

	* Use instruction files when guidance should be always-on for a file type or subsystem.
	* Use skills when guidance is specialized and should load only for specific tasks.

## Skill Catalog ##

### Foundation Skills (External Resources & Visual Design)

| File | Skill Name | Best Use |
| --- | --- | --- |
| [jat-external-resources.skill.md](jat-external-resources.skill.md) | jat-external-resources | SMAPI, StardewUI, GMCM, and translation resource lookup |
| [jat-visual-design-language.skill.md](jat-visual-design-language.skill.md) | jat-visual-design-language | Applying JAT visual tokens and accessibility constraints |

### UI & StarML Skills

| File | Skill Name | Best Use |
| --- | --- | --- |
| [jat-starml-cheatsheet.skill.md](jat-starml-cheatsheet.skill.md) | jat-starml-cheatsheet | Fast StarML syntax and binding checks while editing `.sml` |
| [jat-ui-component-patterns.skill.md](jat-ui-component-patterns.skill.md) | jat-ui-component-patterns | Choosing HUD/menu composition shells and reusable structures |
| [jat-snapshot-binding-and-ui-data-flow.skill.md](jat-snapshot-binding-and-ui-data-flow.skill.md) | jat-snapshot-binding-and-ui-data-flow | Snapshot binding patterns and read-only UI data flow |

### Architecture & State Management Skills

| File | Skill Name | Best Use |
| --- | --- | --- |
| [jat-command-reducer-snapshot-flow.skill.md](jat-command-reducer-snapshot-flow.skill.md) | jat-command-reducer-snapshot-flow | State mutation boundary and command/reducer/snapshot flow patterns |
| [jat-identifier-determinism-patterns.skill.md](jat-identifier-determinism-patterns.skill.md) | jat-identifier-determinism-patterns | Deterministic identifier generation (TaskID, RuleID, SubjectID, DayKey) |
| [jat-persistence-migration-and-reconstruction.skill.md](jat-persistence-migration-and-reconstruction.skill.md) | jat-persistence-migration-and-reconstruction | Persistence, versioning, migration, and state reconstruction patterns |
| [jat-task-generation-and-rule-evaluation.skill.md](jat-task-generation-and-rule-evaluation.skill.md) | jat-task-generation-and-rule-evaluation | Deterministic task generation and rule evaluation patterns |

### Implementation & Testing Skills

| File | Skill Name | Best Use |
| --- | --- | --- |
| [jat-testing-patterns-and-fixtures.skill.md](jat-testing-patterns-and-fixtures.skill.md) | jat-testing-patterns-and-fixtures | JAT-specific unit test patterns, fixtures, and determinism verification |
| [jat-error-handling-and-validation-patterns.skill.md](jat-error-handling-and-validation-patterns.skill.md) | jat-error-handling-and-validation-patterns | Validation, null safety, error handling, and recovery patterns |
| [jat-dependency-injection-and-composition.skill.md](jat-dependency-injection-and-composition.skill.md) | jat-dependency-injection-and-composition | Constructor injection, dependency graphs, and composition root wiring |

### Lifecycle & Integration Skills

| File | Skill Name | Best Use |
| --- | --- | --- |
| [jat-event-lifecycle-and-game-coupling.skill.md](jat-event-lifecycle-and-game-coupling.skill.md) | jat-event-lifecycle-and-game-coupling | Game lifecycle integration and event dispatch patterns |

### Maintenance & Debugging Skills

| File | Skill Name | Best Use |
| --- | --- | --- |
| [jat-build-debug-and-deployment-workflow.skill.md](jat-build-debug-and-deployment-workflow.skill.md) | jat-build-debug-and-deployment-workflow | Build variants, SMAPI integration, debugging, and mod deployment |
| [jat-smapi-debugging-and-diagnostics.skill.md](jat-smapi-debugging-and-diagnostics.skill.md) | jat-smapi-debugging-and-diagnostics | SMAPI logging, diagnostic techniques, and game state inspection |

## Notes ##

	* Keep skill file names lowercase with hyphens and `.skill.md` suffix.
	* Ensure each `name` field matches the skill file stem.
	* Keep each skill focused on one repeatable workflow.
