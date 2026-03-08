# Practical Example: Using Skills in Agents

## Real Example from Your Repo

### Before (Without Skills)

```markdown
# GameAgent

Implement game state and event handling.

## Responsibilities

- Game state management
- Event dispatching
- Rule evaluation
- Task generation

## Implementation Notes

Follow testing best practices. Handle errors properly. Keep deterministic IDs.
Bond views to snapshots carefully.
```

### After (With Skills)

```markdown
# GameAgent

Implement game state and event handling using JAT-specific patterns and skills.

## Responsibilities

- Game state management (see jat-command-reducer-snapshot-flow skill)
- Event dispatching (see jat-event-lifecycle-and-game-coupling skill)
- Rule evaluation (see jat-task-generation-and-rule-evaluation skill)
- Task generation (see jat-task-generation-and-rule-evaluation skill)

## Implementation Guidance

### When Implementing State Management
Use the [jat-command-reducer-snapshot-flow skill](/skill:jat-command-reducer-snapshot-flow) to understand:
- Command pattern structure
- Reducer functions
- Snapshot creation and serialization

### When Handling Events
Follow the [jat-event-lifecycle-and-game-coupling skill](/skill:jat-event-lifecycle-and-game-coupling):
- Game lifecycle phases
- Event subscription best practices
- Coupling risk mitigation

### When Creating Tests
Reference the [jat-testing-patterns-and-fixtures skill](/skill:jat-testing-patterns-and-fixtures) for:
- Fixture setup
- Determinism enforcement
- Boundary testing

### When Building UI Bindings
Use [jat-snapshot-binding-and-ui-data-flow skill](/skill:jat-snapshot-binding-and-ui-data-flow) to ensure:
- Two-way data flow correctness
- View subscription patterns
- Serialization boundaries

### When Identifying Determinism Issues
Check [jat-identifier-determinism-patterns skill](/skill:jat-identifier-determinism-patterns):
- Serializable ID requirements
- Cross-session determinism
- Canonical key construction
```

## Pattern 1: Inline Skill Reference

Use when an agent needs to guide developers to a skill for a specific task:

```markdown
# ReviewerAgent

## Code Review Checklist

When reviewing feature implementations, use [breakdown-feature-implementation skill](/skill:breakdown-feature-implementation) to ensure all steps were completed:
- Specification → clear and accepted
- Plan → detailed and realistic
- Implementation → follows plan and contracts
- Tests → comprehensive coverage
- Documentation → updated
```

**Result in chat:** When a user invokes the Reviewer, they get guidance to follow the skill for completeness.

---

## Pattern 2: Handoff to Another Agent with Skill Guidance

Use in a Planner → Researcher handoff with skill context:

```yaml
---
name: planner
handoffs:
  - label: "Research build system details"
    agent: Researcher
    prompt: "Research using the jat-build-debug-and-deployment-workflow skill. We need to understand MSBuild property flags and SMAPI discovery."
---
```

**Result in workflow:** Planner invokes Researcher with explicit skill guidance.

---

## Pattern 3: Agent Requesting Skill Invocation

An agent can ask the user to load a skill first:

```markdown
# GameAgent

Before you ask me to implement game logic, please use one of these skills first:

- Type `/jat-build-debug-and-deployment-workflow` to understand the build system
- Type `/jat-event-lifecycle-and-game-coupling` to grasp game lifecycle patterns
- Type `/jat-command-reducer-snapshot-flow` to learn state management

This ensures we're aligned on JAT architecture.
```

**Result in chat:** User sees clear skill recommendations.

---

## Pattern 4: Skill Reference with File Patterns

Use when a skill applies to specific file types:

```markdown
# Reviewer

## C# Files (.cs)

- Use [csharp-docs skill](/skill:csharp-docs) for documentation
- Use [csharp-xunit skill](/skill:csharp-xunit) for test reviews
- Use [dotnet-best-practices skill](/skill:dotnet-best-practices) for general patterns

## Config Files (manifest.json, etc.)

See contracts: [JSON-STYLE-CONTRACT.instructions.md](.local/Agents/Contracts/JSON-STYLE-CONTRACT.instructions.md)

## StarML Files (.xml)

- Use [jat-starml-cheatsheet skill](/skill:jat-starml-cheatsheet) for syntax
- Use [jat-ui-component-patterns skill](/skill:jat-ui-component-patterns) for structure
```

**Result in chat:** User gets specific skill guidance per file type.

---

## Pattern 5: Decision Tree with Skills

Use when implementing complex workflows:

```markdown
# PlansAgent

## Implementation Planning

1. **Do you need to break down a feature?**
   → Use [breakdown-feature-implementation skill](/skill:breakdown-feature-implementation)

2. **Do you need to create a detailed plan?**
   → Use [create-implementation-plan skill](/skill:create-implementation-plan)

3. **Need to update an existing plan?**
   → Use [update-implementation-plan skill](/skill:update-implementation-plan)

4. **Reviewing the plan against a spec?**
   → Use [review-and-refactor skill](/skill:review-and-refactor)
```

**Result in chat:** User can navigate complex workflows with skill guidance.

---

## Complete Extended Example: UIAgent

Here's a realistic complete agent that uses multiple skills:

```markdown
---
name: UIAgent
description: "UI implementation for StardewUI views, bindings, and event handlers. Use when: building StarML views, binding snapshots to views, or fixing view logic."
---

# UIAgent

Implement user interfaces using StardewUI framework and JAT patterns.

## Responsibilities

1. **StarML View Creation**
   - Syntax: [jat-starml-cheatsheet skill](/skill:jat-starml-cheatsheet)
   - Structure: [jat-ui-component-patterns skill](/skill:jat-ui-component-patterns)

2. **Snapshot-to-View Binding**
   - Data flow patterns: [jat-snapshot-binding-and-ui-data-flow skill](/skill:jat-snapshot-binding-and-ui-data-flow)
   - Two-way synchronization
   - Mutation boundaries

3. **Event Handling**
   - Game lifecycle: [jat-event-lifecycle-and-game-coupling skill](/skill:jat-event-lifecycle-and-game-coupling)
   - View event patterns: [jat-ui-component-patterns skill](/skill:jat-ui-component-patterns)

4. **Testing UI Logic**
   - Test patterns: [jat-testing-patterns-and-fixtures skill](/skill:jat-testing-patterns-and-fixtures)
   - Mock gamestate
   - Verify binding correctness

## When to Use This Agent

Ask me to:
- Create a new StardewUI view for a form or modal
- Bind a snapshot to a view with proper data flow
- Fix view binding or event handling issues
- Implement event handlers for UI interactions
- Test UI logic and view state

## Example Request

"Create a modal view for configuring task rules. The modal should:
- Display current RuleSnapshot
- Allow editing fields
- Show save/cancel buttons
- Validate before submit"

I will use [jat-starml-cheatsheet skill](/skill:jat-starml-cheatsheet) for syntax and [jat-snapshot-binding-and-ui-data-flow skill](/skill:jat-snapshot-binding-and-ui-data-flow) for binding patterns.

## Contracts I Follow

- [SML-STYLE-CONTRACT.instructions.md](.local/Agents/Contracts/SML-STYLE-CONTRACT.instructions.md)
- [FRONTEND-ARCHITECTURE-CONTRACT.instructions.md](.local/Agents/Contracts/FRONTEND-ARCHITECTURE-CONTRACT.instructions.md)
```

---

## How to Update Your Agents with Skills

For each agent in `.local/Agents/*.agent.md`:

1. **Identify relevant skills** from the list in [SKILLS_EXPLAINED.md](./SKILLS_EXPLAINED.md)
2. **Add skill references** in the agent's body using the patterns above
3. **Test** by checking if the skill appears in the `/` command palette
4. **Document** which skills apply to which responsibilities

Example update:

```diff
# UIAgent

- Implement user interfaces for the mod.
+ Implement user interfaces using StardewUI and JAT patterns.
+ 
+ Follow these skills for implementation:
+  * [jat-starml-cheatsheet skill](/skill:jat-starml-cheatsheet) — StarML syntax
+  * [jat-snapshot-binding-and-ui-data-flow skill](/skill:jat-snapshot-binding-and-ui-data-flow) — Data binding
```

---

## Troubleshooting

### Skill Not Appearing in `/` Command

**Problem:** You type `/jat-build` and don't see `jat-build-debug-and-deployment-workflow`

**Solution:**
1. Check folder name: `.local/Agents/Skills/jat-build-debug-and-deployment-workflow/`
2. Check SKILL.md exists: `.local/Agents/Skills/jat-build-debug-and-deployment-workflow/SKILL.md`
3. Check frontmatter `name:` field in SKILL.md:
   ```yaml
   ---
   name: jat-build-debug-and-deployment-workflow
   ```
4. Restart VS Code
5. Try typing `/jat-` to see partial matches

### Agent References a Skill That Doesn't Exist

**Problem:** Agent says "Use [some-skill skill](/skill:some-skill)" but it's not in the list

**Solution:**
- Verify the folder exists in `.local/Agents/Skills/some-skill/`
- Check the `name:` field in that folder's SKILL.md
- Update the skill reference in the agent if folder/name was wrong

### Skill Provides Wrong Context

**Problem:** Skill is discoverable but doesn't match what the agent needs

**Solution:**
- Check if a more specific JAT skill exists
- Or create a new skill in `.local/Agents/Skills/new-skill/SKILL.md`
- Add it to the agent's references

---

## Summary

Skills are now your primary way of:
1. **Organizing specialized knowledge** (e.g., SMAPI APIs, testing patterns)
2. **Guiding agents** toward consistent workflows
3. **Bundling related resources** (docs, templates, examples)
4. **Enabling on-demand learning** (users invoke `/skill-name`)

Your 35 skills (15 JAT-specific + 20 general development) are fully discoverable and ready to use in agents!
