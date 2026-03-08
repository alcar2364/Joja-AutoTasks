---
name: "Agent Customization Task"
description: "Use when: creating, tuning, or debugging agent customization files and ecosystem coherence."
argument-hint: "Customization goal + primitive type + scope + constraints"
agent: "GodAgent"
---

Handle this customization request for the JAT agent ecosystem.

Customization Inputs
- Goal: <create|analyze|tune|debug>
- Primitive type: <.agent.md|.instructions.md|.skill.md|.prompt.md|hooks|workspace config>
- Scope: <single file|cross-file|workspace-level>
- Constraints: <required>

Customization Expectations
- Use minimal-change edits with valid YAML frontmatter.
- Ensure discoverability via specific description keywords.
- Maintain cross-file consistency for renamed or referenced artifacts.
- Flag scope expansion and ask before broader edits.

Required Output
1. Changes made and rationale
2. Ecosystem consistency checks
3. Validation results
4. Optional next improvements
