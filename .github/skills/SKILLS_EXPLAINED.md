<!-- markdownlint-disable -->

# How Skills Work in JAT

Skills are on-demand workflow bundles used by agents and slash-command invocations.

## Skill Structure

Each skill must follow:

- Folder: `.github/skills/<skill-name>/`
- Main file: `.github/skills/<skill-name>/SKILL.md`
- Optional support files: `.github/skills/<skill-name>/references/`

## Discovery Rules

Discovery depends on these rules:

- Skill folder name must be lowercase-with-hyphens.
- `name:` in frontmatter must exactly match the folder name.
- `description:` should include trigger terms such as `Use when:`.

If folder name and `name:` do not match, discovery can fail silently.

## Where Skill Ownership Lives

- Catalog: `.github/skills/README.md`
- Agent usage wiring: `.github/instructions/agent-boundaries-and-wiring-governance.instructions.md`

Every skill must be mapped to at least one agent.

## Typical Usage

- Direct invocation in chat using `/skill-name`
- Agent workflow usage via explicit wiring to an owner agent

## Quick Validation Checklist

- [ ] Skill folder exists under `.github/skills/`
- [ ] `SKILL.md` exists in that folder
- [ ] Frontmatter has `name:` and `description:`
- [ ] Folder name equals `name:`
- [ ] Skill is listed in `.github/skills/README.md`
- [ ] Skill is mapped in `.github/instructions/agent-boundaries-and-wiring-governance.instructions.md`
