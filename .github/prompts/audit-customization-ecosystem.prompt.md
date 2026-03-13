---
name: "Audit Customization Ecosystem"
description: "Use when: performing full .github customization audits for agent overlap, instruction/skill wiring, prompt integrity, and hook runtime validity."
argument-hint: "Scope + strictness level + include legacy coverage check (yes/no)"
agent: "GodAgent"
---

Run a full customization-system audit for `.github/`.

Inputs
- Scope: <agents|instructions|prompts|skills|hooks|all>
- Strictness: <advisory|enforced>
- Include legacy hook coverage validation: <yes|no>

Audit Checklist
1. Validate agent boundaries are non-overlapping using `.github/instructions/agent-boundaries-and-wiring-governance.instructions.md`.
2. Validate every instruction is mapped to at least one agent.
3. Validate every skill is mapped to at least one agent.
4. Validate prompts have frontmatter (`name`, `description`, `agent`) and index coverage in `.github/prompts/README.md`.
5. Validate runtime hook bundles under `.github/hooks/*/hooks.json` reference executable scripts.

Required Output
1. Findings by severity (blocking, major, minor)
2. Missing coverage lists (if any)
3. Exact files to update
4. If strictness=enforced: proposed patch sequence
