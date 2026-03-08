# How Skills Work in VS Code Copilot

Skills are **on-demand, bundled workflows** that enhance AI capabilities for specialized tasks. Think of them as reusable knowledge packages that agents can invoke when needed.

## Quick Analogy

| Without Skill | With Skill |
|---------------|-----------|
| "Write a test for this C# function" | `/csharp-xunit` → "Write a test following xUnit best practices" |
| "Create documentation" | `/csharp-docs` → "Create documentation with C# conventions" |
| "Implement a feature" | `/breakdown-feature-implementation` → "Implement feature following the complete workflow" |

## Key Differences: Skills vs Other Customizations

| Customization | When to Use | How It's Invoked | Bundled Assets |
|---------------|------------|-----------------|-----------------|
| **Instructions** (`.instructions.md`) | General coding standards | Auto-applied based on file pattern | ❌ No |
| **Prompts** (`.prompt.md`) | Single focused task | `/prompt-name` slash command | Some (templates) |
| **Skills** (`.SKILL.md`) | Multi-step specialized workflow | `/skill-name` slash command or agent reference | ✓ Yes (docs, scripts, templates) |
| **Agents** (`.agent.md`) | Complex multi-stage work with tool restrictions | Agent selection + tool permissions | ❌ No |
| **Hooks** (`.hook.md`) | Deterministic shell commands at lifecycle events | Auto-triggered | ❌ No |

## Skill Folder Structure (Awesome-Copilot Standard)

```
.local/Agents/Skills/
└── skill-name/                          # Folder name must match SKILL.md's name: field
    ├── SKILL.md                         # Main skill definition with frontmatter
    └── references/                      # Optional supporting files
        ├── template.md
        ├── example.json
        ├── checklist.md
        └── [any other docs]
```

### The name: Field is Critical

```yaml
---
name: my-skill                          # Must exactly match folder name
description: "..."
---
```

If `name: my-skill` but folder is `my_skill/` → skill won't be discovered.

## How Agents Use Skills

### Method 1: Reference by Name (In Agent Body)

If an agent needs a skill, it references it by the `name:` field:

```markdown
# MyAgent

When implementing features, use the [breakdown-feature-implementation skill](/skill:breakdown-feature-implementation).

This skill guides you through:
1. Breaking down the feature
2. Creating a plan
3. Implementing step-by-step
```

### Method 2: Skill Invoke Command

Users can invoke a skill directly via the `/` command in chat:

```
/breakdown-feature-implementation
```

This opens the skill's SKILL.md and makes it available for the agent to follow.

### Method 3: Agent Configuration (Handoff)

An agent can include a skill in its handoff to another agent's subagent:

```yaml
handoff:
  - label: "Break down complex features"
    agent: Planner
    prompt: "Use the breakdown-feature-implementation skill to..."
```

## Skill Discovery Mechanism

**How Copilot finds skills:**

1. Scans these standard locations:
   - `.github/skills/*/SKILL.md`
   - `.local/agents/skills/*/SKILL.md`
   - `.local/Agents/Skills/*/SKILL.md` ← **Your JAT location**
   - `.agents/skills/*/SKILL.md`
   - `.claude/skills/*/SKILL.md`

2. For each folder found:
   - Reads the `SKILL.md` file
   - Parses YAML frontmatter (`name:`, `description:`, etc.)
   - Indexes the skill by its `name:` field

3. If the folder name ≠ `name:` field → skill fails silently (not indexed)

## Your Current Skills

You now have **35 skills** in `.local/Agents/Skills/`:

### JAT Domain-Specific Skills (15)
These are customized for Joja AutoTasks and Stardew Valley modding:

- `jat-build-debug-and-deployment-workflow` — Build variants, SMAPI deployment, debug workflow
- `jat-command-reducer-snapshot-flow` — Command pattern, reducers, snapshot creation
- `jat-dependency-injection-and-composition` — DI container, bootstrapping, composition root
- `jat-error-handling-and-validation-patterns` — Exception handling, validation, error codes
- `jat-event-lifecycle-and-game-coupling` — Game lifecycle, SMAPI events, coupling risks
- `jat-external-resources` — SMAPI docs, StardewUI, GMCM, i18n resources
- `jat-identifier-determinism-patterns` — Deterministic IDs across sessions
- `jat-persistence-migration-and-reconstruction` — Save/load, migrations, backward compatibility
- `jat-smapi-debugging-and-diagnostics` — SMAPI logs, breakpoints, troubleshooting
- `jat-snapshot-binding-and-ui-data-flow` — View binding, two-way data flow
- `jat-starml-cheatsheet` — StarML syntax, views, event handlers
- `jat-task-generation-and-rule-evaluation` — Rule system, task creation
- `jat-testing-patterns-and-fixtures` — Test structure, mocking, fixtures
- `jat-ui-component-patterns` — Component lifecycle, state management
- `jat-visual-design-language` — Design system, colors, spacing, typography

### General Development Skills (20)
From awesome-copilot, applicable to general C# and .NET work:

- `breakdown-feature-implementation` — Multi-step feature implementation workflow
- `conventional-commit` — Conventional commit message standard
- `create-architectural-decision-record` — ADR (Architecture Decision Record) template
- `create-github-issue-feature-from-specification` — Issues from specs
- `create-github-issues-feature-from-implementation-plan` — Issues from plans
- `create-github-pull-request-from-specification` — PR creation workflow
- `create-implementation-plan` — Implementation planning
- `create-readme` — README documentation
- `create-specification` — Feature specification
- `csharp-docs` — C# documentation best practices
- `csharp-mstest` — MSTest unit testing
- `csharp-xunit` — xUnit testing framework
- `dotnet-best-practices` — .NET general best practices
- `dotnet-upgrade` — Upgrading .NET versions
- `ef-core` — Entity Framework Core patterns
- `git-commit` — Git commit workflow
- `refactor` — Refactoring workflow
- `review-and-refactor` — Code review + refactoring combined
- `update-implementation-plan` — Updating existing plans
- `update-specification` — Updating specs

## How Your Agents Can Use These Skills

### Example: Planner Agent

The Planner agent might invoke `create-implementation-plan` skill:

```
# Planner

Create detailed implementation plans following the create-implementation-plan skill.

This ensures consistent structure and completeness.
```

### Example: GameAgent

When implementing a feature, GameAgent could reference `breakdown-feature-implementation`:

```
# GameAgent

1. Break down the feature using breakdown-feature-implementation skill
2. Implement each component
3. Test with jat-testing-patterns-and-fixtures skill
```

### Example: Reviewer

Code review could leverage `review-and-refactor` skill:

```
# Reviewer

Review code changes using review-and-refactor skill for comprehensive feedback.
```

## Best Practices for Skill Use

### ✓ Do

- **Reference skills by name** in agent docs when the workflow is relevant
- **Organize supporting files** in the `references/` subfolder (docs, templates, checklists)
- **Keep skill names kebab-case** and consistent with folder name
- **Include a meaningful `description:`** field that starts with trigger keywords ("Use when:")
- **Test skill discovery** by checking if the skill appears in the `/` command palette

### ✗ Don't

- Create flat `.skill.md` files in the root Skills folder (must be in subfolders)
- Use spaces or underscores in folder names (use kebab-case: `my-skill`, not `my skill` or `my_skill`)
- Forget to match the `name:` field with the folder name
- Over-document within SKILL.md when supporting docs can go in `references/`

## Testing Skills Are Discoverable

Open VS Code and:

1. Start a Copilot chat
2. Type `/` to see the slash command palette
3. Search for a skill by name:
   - `/breakdown-feature` should show `breakdown-feature-implementation`
   - `/jat-build` should show `jat-build-debug-and-deployment-workflow`
   - `/csharp-xunit` should show `csharp-xunit`

If a skill doesn't appear:
- Check folder name matches `name:` field (case-sensitive)
- Verify `SKILL.md` exists in the folder
- Ensure YAML frontmatter is valid (no unescaped colons, proper quoting)
- Restart VS Code if needed

## Glossary

| Term | Meaning |
|------|---------|
| **name:** | Identifier for the skill; must match folder name |
| **description:** | Discovery text shown in `/` command; should start with "Use when:" |
| **SKILL.md** | The main skill file (always named exactly this) |
| **references/** | Optional subfolder for supporting docs, templates, examples |
| **Skill discovery** | Copilot's automatic scanning for SKILL.md files in standard locations |
| **Slash command** | Type `/name` in chat to invoke a skill or prompt |
| **Handoff** | Agent routing to another agent, optionally with skill guidance |

## Further Reading

- [VS Code Copilot Customization Docs](https://code.visualstudio.com/docs/copilot/copilot-customization)
- [FOLDER_STRUCTURE.md](./.local/Agents/FOLDER_STRUCTURE.md) — Full agent ecosystem layout
- [GodAgent.agent.md](./.local/Agents/GodAgent.agent.md) — Meta-agent instructions
- [awesome-copilot Repository](https://github.com/github/awesome-copilot) — Reference implementation
