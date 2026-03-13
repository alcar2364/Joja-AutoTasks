<!-- markdownlint-disable -->

# Skills Quick Start for JAT

This workspace currently has 32 skills under `.github/skills/`.

## What a Skill Is

- A reusable workflow package that can be invoked by name.
- Stored as `.github/skills/<skill-name>/SKILL.md`.
- Optionally includes supporting assets in `references/`.

## Use a Skill in Chat

1. Open Copilot Chat.
2. Type `/`.
3. Search for a skill name such as:
   - `/csharp-xunit`
   - `/jat-build-debug-and-deployment-workflow`
   - `/jat-command-reducer-snapshot-flow`
4. Select the skill.

## Use a Skill in Agent Workflow

- Agent ownership for each skill is defined in `.github/instructions/agent-boundaries-and-wiring-governance.instructions.md`.
- Every skill is mapped to at least one agent.

## Add a New Skill

1. Follow the official authoring reference: https://code.visualstudio.com/docs/copilot/customization/agent-skills
2. Create `.github/skills/<new-skill>/SKILL.md`.
3. Ensure `name:` in frontmatter exactly matches `<new-skill>`.
4. Add optional support docs to `.github/skills/<new-skill>/references/`.
5. Add the skill to:
   - `.github/skills/README.md`
   - `.github/instructions/agent-boundaries-and-wiring-governance.instructions.md`

## Troubleshooting

| Problem                     | Check                                                                                |
| --------------------------- | ------------------------------------------------------------------------------------ |
| Skill not discoverable      | Folder name matches `name:` in `SKILL.md`                                            |
| Skill opens but seems wrong | Verify the correct skill was mapped in the wiring file                               |
| Agent does not use a skill  | Update `.github/instructions/agent-boundaries-and-wiring-governance.instructions.md` |
