---
name: update-docs-on-code-change
description: "Rules for keeping JAT documentation synchronized with code changes. Use when: adding features, changing APIs, modifying build/setup, or making breaking changes."
---

# Update Documentation on Code Change

## Purpose

Keep JAT documentation synchronized with code changes. Documentation updates belong in the same
commit as the code change that requires them.

## When to Update Documentation

Update docs when any of the following change:

- A new feature is added or an existing feature changes behavior
- A public interface, command handler, or config option changes
- Build commands or setup procedures change
- A breaking change is introduced
- A SMAPI or .NET version requirement changes

## What to Update

### README.md at root

- Update the features list when capabilities change
- Update setup/installation steps when the build or dependency chain changes
- Update minimum version requirements (SMAPI API version, .NET, game version) when they change

### Design Guide (`Project/Planning/`)

- Update the relevant design guide section when an architectural decision changes
- Update the Architecture Map when subsystem boundaries or responsibilities change
- Do NOT leave design docs describing an architecture that no longer exists in code

### Breaking changes

JAT does not currently maintain a formal CHANGELOG. Document breaking changes in commit messages
and PR descriptions using the format:

```text
BREAKING: <what changed and why>
```

### Code comments

- Update or remove code comments that reference old behavior
- Do NOT leave comments describing a superseded design when the code has changed

## What NOT to Do

- Do NOT add new documentation files for every change — update existing files
- Do NOT copy-paste content; link to the canonical source instead
- Do NOT document internal implementation details that change frequently in user-facing docs
- Do NOT create migration guides for in-development features that have no released version yet

## Review Check

Before marking a task complete, confirm:

- [ ] README.md reflects the current feature set and setup steps
- [ ] Design guide sections match the current architecture
- [ ] Any old or conflicting documentation has been updated or removed
- [ ] Commit message notes the doc update (e.g., `docs: update README for Phase 2 changes`)
