---
name: ui-hud-patterns
description: "HUD-focused UI patterns for JAT. Use when: implementing or refactoring HUD layout, row density, scroll behavior, or HUD interaction limits."
applyTo: "{UI/**/*.cs,**/*.sml}"
---

# UI-HUD-PATTERNS.instructions.md #

## Purpose ##

This document defines **HUD-specific UI patterns** for **JAT (Joja AutoTasks)**.

Use this file when editing the always-on gameplay HUD surface. These rules intentionally
optimize for fast scanning, low interaction complexity, and stable runtime behavior.

**Related Resources:**

- [`ui-component-patterns.instructions.md`](ui-component-patterns.instructions.md) — Shared composition patterns
- [`ui-interaction-patterns.instructions.md`](ui-interaction-patterns.instructions.md) — Modal/form/state-transition interaction patterns
- [`visual-design-language.instructions.md`](visual-design-language.instructions.md) — Visual language and spacing rules
- [`FRONTEND-ARCHITECTURE-CONTRACT.instructions.md`](../Contracts/FRONTEND-ARCHITECTURE-CONTRACT.instructions.md) — Snapshot and command-dispatch boundaries

## 1. HUD overall composition ##

The HUD should remain structurally simple and visually scannable.

Preferred composition:

1. draggable root shell
2. compact header
3. active task list
4. optional completed task section
5. compact footer/summary row (optional)

Recommended conceptual hierarchy:

```text
HudRoot
  └ HudFrame
      └ VerticalLane
          ├ HudHeader
          ├ ActiveTaskList
          ├ CompletedTaskSection (optional / collapsible)
          └ HudSummaryRow (optional)
```

## 2. HUD header pattern ##

Header should contain only high-value controls/information:

```text
- current day / date context if shown
- collapse/expand control
- open-menu control
- optional quick summary
```

Do not overload the header with settings or debug-only clutter.

## 3. HUD task list pattern ##

Use:

```text
- a compact repeated row template
- predictable row height
- minimal text wrapping
- compact status/progress presentation
```

Each row should usually contain:

```text
- icon (optional)
- title
- compact progress/deadline/status
- quick completion toggle if supported
```

Avoid:

```text
- large multiline descriptions in the HUD
- dense metadata blocks
- any interaction that feels like "editing a record"
```

## 4. HUD completed section pattern ##

If completed tasks are shown in HUD:

```text
- place them in a dedicated collapsible section below active tasks
- clearly separate active vs completed
- default behavior may collapse this region when screen space is tight
```

Do not interleave completed tasks among active tasks.

## 5. HUD scrolling pattern ##

For large task sets:

```text
- use a scrollable region or equivalent bounded viewport
- keep scroll controls visually anchored to the task list, not floating ambiguously
- preserve scroll state locally in the HUD layer
```

The HUD should support quick scanning, not long-form browsing.

## 6. HUD interaction budget ##

Allowed HUD interactions:

```text
- scroll
- select
- quick complete/uncomplete if supported
- collapse/expand
- open menu
- drag within viewport bounds
```

Avoid:

```text
- multi-step flows
- nested configuration dialogs
- task editing forms
- complex history browsing inside HUD
```

## 7. HUD performance emphasis ##

The HUD is performance-sensitive. Keep compositions compact, stable, and predictable.

```text
- avoid decorative layout complexity
- avoid large tree rebuilds on non-structural changes
- prefer stable repeated templates for rows
```

## 8. HUD anti-pattern reminder ##

Do not use HUD as a full configuration surface.

```text
- keep HUD interactions quick and shallow
- route advanced management to menu surfaces
```
