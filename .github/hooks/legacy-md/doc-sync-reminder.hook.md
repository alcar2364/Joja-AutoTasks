---
name: doc-sync-reminder
description: >-
  Detects code changes that may require documentation updates and prompts agents
  to keep README.md and other docs synchronized with implementation.
trigger: after-edit
applyTo: "**/*.{cs,sml,json,csproj}"
---

# Doc Sync Reminder Hook #

**Trigger:** After editing code or project files.  
**Purpose:** Ensure documentation stays in sync with code changes, preventing stale or misleading docs.

## Scope and Applicability ##

This hook activates when:

- Agent edits `.cs`, `.sml`, `.json`, or `.csproj` files
- Changes affect public APIs, project structure, or functionality
- New features, dependencies, or workflows are introduced
- Existing behaviors are significantly modified

## Post-Edit Documentation Review ##

**RECOMMENDED**: Load [`update-docs-on-code-change.instructions.md`](../Instructions/update-docs-on-code-change.instructions.md) after edits to check:

1. **Public API changes:**
   - New public methods, types, or namespaces
   - Changed method signatures or behaviors
   - New events or hooks introduced
   - Removed or deprecated APIs

2. **Project structure changes:**
   - New folders or subsystems
   - Reorganized modules or responsibilities
   - New build configuration
   - New project dependencies

3. **Feature or workflow changes:**
   - New functionality or commands
   - Changed user-facing behavior
   - New configuration options
   - New deployment steps

4. **Build or tooling changes:**
   - New build steps or dependencies
   - New scripts or automation
   - New version constraints
   - New environment setup requirements

## Documentation Sync Procedure ##

1. **Identify documentation needs:**
   - Check which README, guide, or config files reference the changed code
   - Identify if new documentation is needed
   - List all files that need updates

2. **Update documentation:**
   - README.md: Feature descriptions, setup, usage examples
   - Design guides: Architecture decisions, subsystem explanations
   - Configuration files: New settings, environment variables
   - Contributing guides: Update process for new patterns

3. **Verify accuracy:**
   - Confirm code examples in docs still work
   - Verify links and file references are correct
   - Ensure terminology matches code and design
   - Check for outdated version numbers or references

4. **Prompt agent:**
   - If changes require doc updates, prompt explicitly
   - Suggest which files need updates and why
   - Verify updates are complete before closing work

## Documentation Candidates in JAT ##

High-priority docs to keep current:

- **README.md**: Feature overview, setup, build, run
- **Design guides** (`.local/Joja AutoTasks Design Guide/`): Architecture, state model, data flow
- **AGENTS.md**: Agent ecosystem overview and capabilities
- **Tests/README.md**: Testing strategy and patterns
- **manifest.json**: Mod metadata and version
- **Configuration files**: New config options and behaviors

## Conflict Resolution ##

If documentation updates conflict with other work:

1. **Documentation is a first-class deliverable** — it must be updated.
2. If updates are complex, propose them as separate follow-up tasks.
3. At minimum, flag what docs need updates for manual review.
4. Stale documentation is worse than no documentation.

