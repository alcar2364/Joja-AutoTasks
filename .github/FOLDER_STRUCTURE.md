<!-- markdownlint-disable -->

# JAT Agent Ecosystem - Folder Structure

This document defines the active Agent customization layout for Joja AutoTasks.

## Directory Layout

```
.github/
├── copilot-instructions.md
├── agents/
│   └── *.agent.md
├── instructions/
│   └── *.instructions.md
├── prompts/
│   └── *.prompt.md
├── hooks/
│   ├── <bundle>/
│   │   ├── hooks.json
│   │   └── *.sh
│   ├── legacy-md/
│   │   └── *.hook.md
│   └── README.md
└── skills/
    ├── <skill-name>/
    │   ├── SKILL.md
    │   └── references/
    └── README.md
```

## Naming Conventions

| File Type | Location | Pattern | Example |
| --- | --- | --- | --- |
| Agent | `.github/agents/` | `{name}.agent.md` | `GodAgent.agent.md` |
| Instruction | `.github/instructions/` | `{name}.instructions.md` | `workspace-contracts.instructions.md` |
| Prompt | `.github/prompts/` | `{name}.prompt.md` | `orchestrate-work-item.prompt.md` |
| Hook bundle config | `.github/hooks/<bundle>/` | `hooks.json` | `.github/hooks/context-preflight/hooks.json` |
| Hook bundle script | `.github/hooks/<bundle>/` | `*.sh` | `.github/hooks/context-preflight/context-preflight.sh` |
| Legacy hook spec | `.github/hooks/legacy-md/` | `{name}.hook.md` | `state-mutation-guard.hook.md` |
| Skill | `.github/skills/<skill-name>/` | `SKILL.md` | `.github/skills/jat-external-resources/SKILL.md` |

## Discovery Rules

- Agents: `.github/agents/*.agent.md`
- Instructions: `.github/instructions/*.instructions.md`
- Prompts: `.github/prompts/*.prompt.md`
- Skills: `.github/skills/*/SKILL.md`
- Runtime hooks: `.github/hooks/*/hooks.json`

## Entry Points

- Repository entry: `.github/copilot-instructions.md`
- Main orchestrator: `.github/agents/Orchestrator.agent.md`
- Meta customization agent: `.github/agents/GodAgent.agent.md`

## Notes

- Runtime hooks execute from hook bundles under `.github/hooks/<bundle>/`; markdown files in `.github/hooks/legacy-md/` are reference-only.
- Skill folder names must exactly match each skill file's YAML `name:` field.
