# JAT Agent Ecosystem - Folder Structure

This document describes the official folder structure for the Joja AutoTasks (JAT) agent customization ecosystem, following the [awesome-copilot GitHub standard](https://github.com/github/awesome-copilot).

## Directory Layout

```
.local/Agents/
│
├── *.agent.md                           # Individual agent files (flat)
│   ├── GodAgent.agent.md               # Meta-agent for customization
│   ├── Orchestrator.agent.md           # Entry point agent
│   ├── GameAgent.agent.md
│   ├── UIAgent.agent.md
│   ├── StarMLAgent.agent.md
│   ├── WorkspaceAgent.agent.md
│   ├── Planner.agent.md
│   ├── Researcher.agent.md
│   ├── Reviewer.agent.md
│   ├── Refactorer.agent.md
│   ├── Troubleshooter.agent.md
│   └── UnitTestAgent.agent.md
│
├── Contracts/                           # Style and architecture contracts (flat .instructions.md files)
│   ├── BACKEND-ARCHITECTURE-CONTRACT.instructions.md
│   ├── CSHARP-STYLE-CONTRACT.instructions.md
│   ├── FRONTEND-ARCHITECTURE-CONTRACT.instructions.md
│   ├── JSON-STYLE-CONTRACT.instructions.md
│   ├── REVIEW-AND-VERIFICATION-CONTRACT.instructions.md
│   ├── SML-STYLE-CONTRACT.instructions.md
│   ├── UNIT-TESTING-CONTRACT.instructions.md
│   └── WORKSPACE-CONTRACTS.instructions.md
│
├── Instructions/                        # Coding standards & best practices (flat .instructions.md files)
│   └── [future instruction files]
│
├── Hooks/                              # Automated workflow hooks (flat .hook.md files)
│   ├── agent-capability-freshness.hook.md
│   ├── agent-ecosystem-sync.hook.md
│   ├── anti-slop-enforcer.hook.md
│   ├── contract-auto-loader.hook.md
│   ├── design-guide-context-augmenter.hook.md
│   ├── design-guide-contract-sync.hook.md
│   ├── handoff-optimizer.hook.md
│   ├── identifier-validation.hook.md
│   ├── persistence-safety-validator.hook.md
│   ├── prompt-index-auto-sync.hook.md
│   ├── skills-index-auto-sync.hook.md
│   ├── state-mutation-guard.hook.md
│   ├── ui-boundary-enforcer.hook.md
│   ├── unit-test-coverage-enforcer.hook.md
│   └── README.md
│
├── Prompts/                            # Reusable prompt templates (flat .prompt.md files)
│   └── [prompt files as needed]
│
└── Skills/                             # Skill subfolders (awesome-copilot standard)
    ├── jat-build-debug-and-deployment-workflow/
    │   ├── SKILL.md
    │   └── references/                 # Supporting files for skill
    │
    ├── jat-command-reducer-snapshot-flow/
    │   ├── SKILL.md
    │   └── references/
    │
    ├── jat-dependency-injection-and-composition/
    │   ├── SKILL.md
    │   └── references/
    │
    ├── jat-error-handling-and-validation-patterns/
    │   ├── SKILL.md
    │   └── references/
    │
    ├── jat-event-lifecycle-and-game-coupling/
    │   ├── SKILL.md
    │   └── references/
    │
    ├── jat-external-resources/
    │   ├── SKILL.md
    │   └── references/
    │
    ├── jat-identifier-determinism-patterns/
    │   ├── SKILL.md
    │   └── references/
    │
    ├── jat-persistence-migration-and-reconstruction/
    │   ├── SKILL.md
    │   └── references/
    │
    ├── jat-smapi-debugging-and-diagnostics/
    │   ├── SKILL.md
    │   └── references/
    │
    ├── jat-snapshot-binding-and-ui-data-flow/
    │   ├── SKILL.md
    │   └── references/
    │
    ├── jat-starml-cheatsheet/
    │   ├── SKILL.md
    │   └── references/
    │
    ├── jat-task-generation-and-rule-evaluation/
    │   ├── SKILL.md
    │   └── references/
    │
    ├── jat-testing-patterns-and-fixtures/
    │   ├── SKILL.md
    │   └── references/
    │
    ├── jat-ui-component-patterns/
    │   ├── SKILL.md
    │   └── references/
    │
    ├── jat-visual-design-language/
    │   ├── SKILL.md
    │   └── references/
    │
    ├── README.md
    └── [future skill folders]
```

## File Naming Conventions

| File Type | Location | Naming Pattern | Example |
|-----------|----------|----------------|---------|
| Agent | `.local/Agents/` | `{name}.agent.md` | `GodAgent.agent.md` |
| Instruction | `.local/Agents/Instructions/` or `.local/Agents/Contracts/` | `{name}.instructions.md` | `CSHARP-STYLE-CONTRACT.instructions.md` |
| Hook | `.local/Agents/Hooks/` | `{name}.hook.md` | `skills-index-auto-sync.hook.md` |
| Prompt | `.local/Agents/Prompts/` | `{name}.prompt.md` | `code-generation.prompt.md` |
| Skill | `.local/Agents/Skills/{skill-name}/` | `SKILL.md` | `.local/Agents/Skills/jat-external-resources/SKILL.md` |

## Key Rules

### Skills (Awesome-Copilot Standard)
- **Each skill is a folder** under `.local/Agents/Skills/`
- **Folder name matches the `name:` field** in the SKILL.md frontmatter
- **SKILL.md** is the main skill definition file
- **references/** subdirectory contains supporting files (docs, examples, references)
- **Skill discovery** mechanism scans for `.local/Agents/Skills/*/SKILL.md` files

### Agents, Hooks, Instructions, Prompts (Flat Structure)
- Stored as flat `.{type}.md` files in their respective directories
- File name should match the `name:` field in frontmatter when applicable

### YAML Frontmatter
- All customization files require YAML frontmatter at the top
- YAML parser validity is authoritative (even if markdownlint spacing rules conflict)
- Quote string values containing colons: `description: "Use when: specific domain"`

## Bootstrap Discovery

The GodAgent and ecosystem use bootstrap discovery to find customizations:

```powershell
# Agent files (.agent.md)
.local/Agents/*.agent.md

# Instruction files (.instructions.md)
.local/Agents/**/*.instructions.md

# Skill files (.skill.md structure)
.local/Agents/Skills/*/SKILL.md

# Hook files (.hook.md)
.local/Agents/Hooks/*.hook.md

# Prompt files (.prompt.md)
.local/Agents/Prompts/*.prompt.md
```

## Entry Points

- **Repository-level entry point:** `.github/copilot-instructions.md` (minimal, points to Orchestrator)
- **Orchestrator:** `.local/Agents/Orchestrator.agent.md` (discovers all agents)
- **Meta-agent:** `.local/Agents/GodAgent.agent.md` (manages customization ecosystem)

## Further Reading

- [Awesome-Copilot Repository](https://github.com/github/awesome-copilot)
- [VS Code Copilot Customization Docs](https://code.visualstudio.com/docs/copilot/copilot-customization)
- [GodAgent Instruction File](.local/Agents/GodAgent.agent.md) (Section 6.1)
