# Joja AutoTasks — Repository Instructions

This repository is the **Joja AutoTasks** Stardew Valley mod.

These instructions apply to all Copilot interactions in this workspace, including inline completions, chat, and agent mode.

---

## Project overview

Joja AutoTasks is a deterministic task management engine for Stardew Valley. It automatically detects, tracks, and organizes player goals using built-in generators, user-defined rules (Task Builder), and manual tasks. The mod provides a lightweight HUD and a full menu dashboard, both driven by read-only snapshots published by a central State Store.

---

## Architecture rules

1. **Single source of truth**: The State Store owns all canonical task state. All mutations flow through commands → reducers → new snapshot.
2. **UI never mutates canonical state**: HUD and menus consume snapshots and dispatch commands. No direct state mutation from UI code.
3. **Deterministic identifiers**: TaskID, RuleID, DayKey, and SubjectID must remain stable across reloads, evaluation passes, and save/load cycles. No random IDs, no GUID-based generation for tasks.
4. **Minimal persistence**: Persist only what is needed to reconstruct canonical state. Derived values must be recomputed. Versioned saves with explicit migrations.
5. **Event-driven evaluation**: Prefer event-triggered evaluation over per-frame work. Bounded periodic passes only when necessary.

---

## Coding conventions

- **C#**: Follow `CSHARP-STYLE-CONTRACT.instructions.md` in `.local/Agents/Contracts/`. PascalCase types, camelCase locals, Allman braces, `sealed` by default, no vague naming sludge (Manager/Helper/Utils).
- **StarML (.sml)**: Follow `SML-STYLE-CONTRACT.instructions.md`. Kebab-case attributes, pipe-syntax events, structural attributes first, StarML conventions before XML fallback.
- **JSON**: Follow `JSON-STYLE-CONTRACT.instructions.md`. Match C# property casing, 2-space indent preferred.

---

## Editing behavior

By default, provide implementation guidance (step-by-step edits, patch outlines, diff-style suggestions) rather than directly editing files. Edit files directly only when the user explicitly requests edits.

Respect scope constraints strictly. If a request says "single file," "no behavior change," or "analysis only," treat that as absolute.

---

## Design reference

The full technical design guide is located at:
`.local/Joja AutoTasks Design Guide/JojaAutoTasks Design Guide.md`

Use it as the primary navigation entry for product and architecture intent. Individual sections cover specific subsystems (architecture, data model, persistence, UI, etc.).

---

## Contract and instruction files

Authoritative contract and instruction files are in `.local/Agents/Contracts/` and `.local/Agents/Instructions/`. When working in a subsystem, consult the relevant contracts for hard rules.

---

## Agent system

This workspace uses specialized agents defined in `.local/Agents/`. When working in agent mode, follow the agent-specific instructions and source-of-truth ordering defined in each agent file.
