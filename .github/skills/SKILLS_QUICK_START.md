# Skills Quick Start for JAT

You now have **35 skills** ready to use. Here's the simplest way to understand them:

## What Are Skills?

**Skills** = Reusable knowledge packages that appear as slash commands (`/skill-name`).

Think of them as:
- Specialized guides for complex tasks
- Can include documentation, templates, checklists
- Agents can reference them to guide your work

## Your 35 Skills

| Category | Count | What They Do |
|----------|-------|--------------|
| **JAT-Specific** (15 skills) | Custom for your Stardew Valley mod | Build workflow, testing patterns, UI components, state management, SMAPI APIs |
| **General .NET** (20 skills) | From awesome-copilot | C# docs, unit testing, git workflow, planning, refactoring |

## How to Use a Skill Right Now

1. Open VS Code chat (Copilot)
2. Type `/` (forward slash)
3. Start typing a skill name:
   - `/csharp-xunit` ← Find xUnit testing guidance
   - `/jat-build` ← Find build & deployment info
   - `/breakdown-feature` ← Find feature planning

4. Select the skill → it loads with full documentation

## How Agents Will Use Skills

Each agent (GameAgent, UIAgent, Planner, etc.) now guidelines like:

> "When implementing game state, follow the **jat-command-reducer-snapshot-flow skill** for proper structure."

You click that link, and the skill opens with detailed guidance.

## Files to Read

Learn more in these order:

1. **SKILLS_EXPLAINED.md** ← Start here for understanding
2. **SKILLS_PRACTICAL_EXAMPLES.md** ← See how agents use them
3. **FOLDER_STRUCTURE.md** ← Full directory layout
4. **[.local/Agents/GodAgent.agent.md](./GodAgent.agent.md)** Section 6.1 ← Behind-the-scenes details

## The Key Insight

**Before:** Your agents said "follow best practices" (vague)

**After:** Your agents say "Use the jat-command-reducer-snapshot-flow skill" (specific, discoverable, bundled with docs)

---

## Troubleshooting

| Problem | Solution |
|---------|----------|
| Skill doesn't appear in `/` list | Restart VS Code |
| Agent references a skill that doesn't exist | Check folder name matches `name:` in SKILL.md |
| Want to add a new skill? | Create `.local/Agents/Skills/my-skill/SKILL.md` |
| Want to update a skill? | Edit `.local/Agents/Skills/skill-name/SKILL.md` directly |

---

## Next Steps (Optional)

If you want to enhance agents with skill references:

1. Open an agent file (e.g., [GameAgent.agent.md](./GameAgent.agent.md))
2. Add lines like:
   ```
   For state management, use the [jat-command-reducer-snapshot-flow skill](/skill:jat-command-reducer-snapshot-flow).
   ```
3. Save the file

See [SKILLS_PRACTICAL_EXAMPLES.md](./SKILLS_PRACTICAL_EXAMPLES.md) for more patterns.

---

**That's it!** Your 35 skills are discoverable and ready to guide your work.
