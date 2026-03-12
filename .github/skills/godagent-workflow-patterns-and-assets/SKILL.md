---
name: godagent-workflow-patterns-and-assets
description: "GodAgent workflow procedures and reusable templates for agent customization tasks. Use when: creating, analyzing, auditing, or debugging agent ecosystem files."
argument-hint: "Customization task type and scope"
---

# GodAgent Workflow Patterns and Assets #

Use this skill for detailed procedures and reusable assets referenced by GodAgent.

## Workflow Patterns ##

### New Agent Creation ###

1. Clarify scope (what the agent does and does not do).
2. Choose minimal tool set required for the role.
3. Write discovery-friendly description with trigger keywords.
4. Check local registry and define valid handoffs.
5. Write concise body with clear boundaries and anti-slop rules.
6. Validate YAML frontmatter.
7. Create in the correct path and verify discoverability.

### Agent Effectiveness Analysis ###

1. Read the target agent file.
2. Check description keyword coverage.
3. Check tool-set fit (no bloat, no gaps).
4. Validate handoff coherence and circular risk.
5. Validate body clarity and role alignment.
6. Validate YAML syntax.
7. Produce prioritized recommendations.

### Ecosystem Consistency Audit ###

1. Discover all agents, instructions, and skills.
2. Validate references and handoff targets.
3. Identify circular handoffs or overlap drift.
4. Assess tool coherence.
5. Identify description coverage gaps.
6. Report findings by impact.

### Invocation Debugging ###

1. Capture the request pattern that failed.
2. Compare expected trigger keywords to actual description.
3. Propose concrete description fixes.
4. Validate YAML and frontmatter integrity.
5. Verify invocation behavior after change.

### Self-Analysis Protocol ###

1. Read `GodAgent.agent.md`.
2. Apply meta-agent quality criteria.
3. Verify bootstrap readiness and tool minimality.
4. Produce recommendations.
5. Apply edits only when explicitly requested.

## Reusable Assets ##

### Agent Template (.agent.md) ###

```markdown
---
name: [AgentName]
description: "[Single-sentence description with trigger keywords. Use when: specific domain]"
argument-hint: "[What user should provide as input]"
target: vscode
tools: [list of needed tools]
agents: []
handoffs: []
---

# [AgentName] #

## 1. Responsibilities ##

[What this agent does]

## 2. Exclusions ##

[What this agent does not do]

## 3. Operating Model ##

[How this agent works]

## 4. Anti-Slop Rules ##

[Domain-specific prohibitions]

## 5. Output Format ##

[Expected output structure]
```

### YAML Validation Checklist ###

- Description has trigger keywords.
- Description is quoted when needed.
- Skill `name` matches skill folder name.
- Tool aliases are valid.
- Handoff entries contain label/agent/prompt.
- No tabs in frontmatter.
- Frontmatter is YAML-valid.

### Ecosystem Health Checklist ###

1. Discovery
- All agent files found.
- All skill files found.
- All instruction files found.

2. Reference Validation
- Handoffs target existing agents.
- No circular handoffs without progress criteria.

3. Description Coverage
- Distinct, discoverable descriptions.
- No duplicate responsibilities.

4. Tool Coherence
- Tools match role.
- No excess permissions.

5. Bootstrap Readiness
- Registry complete and current.
- Handoff adaptation works.
