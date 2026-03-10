---
name: agent-ecosystem-validator
description: "Validates agent/instruction/skill ecosystem consistency: detects unmapped files and frontmatter issues."
on:
  push:
    paths:
      - ".github/agents/*.agent.md"
      - ".github/instructions/*.instructions.md"
      - ".github/skills/*/SKILL.md"
      - ".github/instructions/agent-boundaries-and-wiring-governance.instructions.md"
  schedule: weekly
  workflow_dispatch:
permissions:
  contents: read
strict: true
network:
  allowed: [defaults, github]
engine:
  id: copilot
tools:
  github:
    toolsets: [default]
safe-outputs:
  create-issue:
    title-prefix: "[ecosystem] "
    labels: [agentic-workflow, agent-ecosystem, governance]
    close-older-issues: true
    max: 1
---

# Agent Ecosystem Validator — Governance Consistency Checker

Validate that the JAT agent ecosystem governance file accurately reflects all instruction files,
skill bundles, and agent definitions present in the repository. Catches governance drift that can
occur when files are added or modified outside of a Copilot session (where the runtime hook runs).

## Context

- Repository: `${{ github.repository }}`
- Governance file: `.github/instructions/agent-boundaries-and-wiring-governance.instructions.md`
- Agent files: `.github/agents/*.agent.md`
- Instruction files: `.github/instructions/*.instructions.md`
- Skill files: `.github/skills/*/SKILL.md`

## Validation Rules

### Rule 1: Every Instruction File Is Mapped

For each `.instructions.md` file found in `.github/instructions/`:

- The file's base name (without `.instructions.md`) must appear in the
  `Instruction-to-Agent Wiring` table in the governance file
- At least one agent must be listed as the `Primary Agent` for it
- At least one agent must be listed in the `Also Used By` column (or `—` if genuinely singleton)

### Rule 2: Every Skill Is Mapped

For each `SKILL.md` found in `.github/skills/*/`:

- The skill folder name must appear in the `Skill-to-Agent Wiring` table in the governance file
- The `name:` field in the SKILL.md frontmatter must match the folder name exactly
- At least one agent must be listed as `Primary Agent`

### Rule 3: Every Agent in the Governance Table Exists

For each agent listed in the `Agent Domains (Non-Overlapping)` table:

- A corresponding `.agent.md` file must exist at `.github/agents/<AgentName>.agent.md`
- The filename (without `.agent.md`) must match the `Agent` column entry

### Rule 4: No Orphaned Agents

For each `.agent.md` file in `.github/agents/`:

- The agent's name must appear in the `Agent Domains (Non-Overlapping)` table
- If a `.agent.md` exists but has no table entry, it is "orphaned"

### Rule 5: YAML Frontmatter Validity

For each file in the ecosystem (agents, instructions, skills):

- The YAML frontmatter block (between `---` delimiters) must be valid YAML
- Required field `name:` must be present and non-empty
- Required field `description:` must be present and non-empty

### Rule 6: Skill Name ↔ Folder Name Consistency

For each SKILL.md file:

- Read the `name:` field from the frontmatter
- Compare it to the parent folder name
- They must be identical (as required by `workspace-contracts.instructions.md` Section 9.2)

## Output

### All Rules Pass

No output (silence is success). Close any older `[ecosystem]` issue.

### Failures Found

Create a single issue listing all failures grouped by rule:

```markdown
## Agent Ecosystem Governance Validation Report
*Triggered by: push / weekly schedule on YYYY-MM-DD*

### Rule 1: Unmapped Instruction Files
- `new-feature.instructions.md` — not listed in governance wiring table

### Rule 2: Unmapped Skills
- `new-skill/` — folder not in Skill-to-Agent Wiring table

### Rule 3: Missing Agent Files
- `NewAgent` — listed in governance table but `.github/agents/NewAgent.agent.md` not found

### Rule 4: Orphaned Agent Files
- `OldAgent.agent.md` — exists but not in governance table

### Rule 5: Frontmatter Validation Failures
- `.github/skills/bad-skill/SKILL.md` — missing `description:` field

### Rule 6: Name/Folder Mismatch
- `.github/skills/my-skill/SKILL.md` — frontmatter `name: mySkill` ≠ folder `my-skill`

---
*Fix these issues and the ecosystem-maintenance runtime hook will also pass.*
*Reference: `.github/instructions/agent-boundaries-and-wiring-governance.instructions.md`*
```

## Notes

- This workflow provides persistent CI enforcement of the ecosystem contract, complementing
  the `ecosystem-maintenance.sh` runtime hook (which only runs during Copilot sessions)
- Runs on push to any of the tracked paths AND weekly to catch changes made via other tools
- Should not replace the runtime hook — both serve different detection windows
- Issue title includes date to disambiguate from previous governance failures
