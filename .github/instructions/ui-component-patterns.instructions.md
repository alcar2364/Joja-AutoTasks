---
name: ui-component-patterns
description: "Shared UI composition patterns for JAT. Use when: working on task row layouts, local UI state, action mapping, StardewUI view choices, or anti-pattern guidance for both HUD and menu surfaces."
applyTo: "{UI/**/*.cs,**/*.sml}"
---

# UI-COMPONENT-PATTERNS.instructions.md #

## Purpose ##

This document defines **preferred UI component patterns** for **JAT (Joja AutoTasks)**.

Its goal is to stop UI agents from inventing new layout patterns every time they build or refactor a
view. This file provides reusable **composition patterns**, **responsibility boundaries**, and
**StarML structure guidance** for JAT UI work.

**Related Resources:**

- [`SML-STYLE-CONTRACT.instructions.md`](../Contracts/SML-STYLE-CONTRACT.instructions.md) — StarML syntax rules
- [`starml-cheatsheet.instructions.md`](starml-cheatsheet.instructions.md) — Quick StarML reference
- [`visual-design-language.instructions.md`](visual-design-language.instructions.md) — Colors, spacing, visual identity
- [`FRONTEND-ARCHITECTURE-CONTRACT.instructions.md`](../Contracts/FRONTEND-ARCHITECTURE-CONTRACT.instructions.md) — Architecture rules
- [`ui-hud-patterns.instructions.md`](ui-hud-patterns.instructions.md) — HUD-specific composition, interaction budget, and scanability rules
- [`ui-menu-patterns.instructions.md`](ui-menu-patterns.instructions.md) — Menu-specific layout shells, panel patterns, history, statistics, wizard, and debug UI
- [`ui-interaction-patterns.instructions.md`](ui-interaction-patterns.instructions.md) — Modal, form, state transition, and accessibility patterns

This document is a **pattern guide**, not a replacement for the above contracts.

Use this file when deciding **how a UI should be structured**, **what reusable regions should
exist**, and **which StardewUI views should be preferred**.

## 1. Global UI design principles ##

### 1.1 Shared snapshot model ###

All JAT UI surfaces must render from the same published State Store snapshot.

Implications:

```text
- HUD and Menu should display consistent task state.
- UI selection state may exist locally in the UI layer.
- UI must never directly mutate canonical task state.
- User actions must dispatch commands and then wait for a new snapshot.
```

### 1.2 Surface separation ###

JAT has distinct UI surfaces with distinct responsibilities:

```text
- **HUD**
    * lightweight
    * always available during gameplay
    * low interaction complexity
    * quick task interactions only
```

```text
- **Menu**
    * opened on demand
    * supports deeper browsing and management
    * supports history browsing (statistics V2)
    * hosts Task Builder and richer configuration surfaces
```

### 1.3 Stable composition over novelty ###

Prefer a **small set of repeatable component layouts**:

```text
- shell + header + body + footer
- list + details split view
- filter bar + scrollable content
- tabbed sections for major feature groups
- small reusable row/item templates
```

Do not invent a brand-new composition style for each screen.

### 1.4 Readability over visual cleverness ###

A tired future maintainer should be able to identify:

```text
- where the title/header lives
- where filters/navigation live
- where the primary content lives
- where the selected item details live
- where actions live
```

If the hierarchy is not obvious at a glance, the composition is too clever.

## 2. Menu layout shells ##

Menu-specific layout shells have been moved to
[`ui-menu-patterns.instructions.md`](ui-menu-patterns.instructions.md).
This covers screen frames, split-view shells, tab navigation, task list/detail panels,
history browsing, statistics, wizard flows, and debug UI.

When working on any menu surface, load and follow `ui-menu-patterns.instructions.md`
in addition to this file.

## 3. HUD component patterns ##

HUD-specific guidance has been moved to [`ui-hud-patterns.instructions.md`](ui-hud-patterns.instructions.md)
to keep this file focused on shared composition patterns.

When working on HUD shells, row density, interaction limits, or HUD-specific scanability,
load and follow `ui-hud-patterns.instructions.md` in addition to this file.

## 4. Task row patterns ##

### 4.1 Compact row pattern ###

Use this in:

```text
- HUD active list
- HUD completed list
- menu side list
- history lists
```

Structure:

1. row shell
2. optional icon/status marker
3. text lane
4. optional trailing action or status badge

Recommended content priority:

1. title
2. progress/deadline summary
3. category/status badge
4. secondary metadata only if needed

### 4.2 Selected-row pattern ###

When a list has local selection state:

```text
- use clear visual differentiation for selected row
- keep selected styling subtle but obvious
- do not use loud colors or decorative noise to indicate selection
```

### 4.3 Pinned row pattern ###

Pinned tasks should use a small, consistent visual marker.
Do not redesign row structure for pinned items; add a marker/badge instead.

### 4.4 Overdue / urgent row pattern ###

Overdue or near-deadline state should be shown with:

```text
- concise badge, icon, or style accent
- readable deadline text
- no giant alarm siren nonsense
```

The row should still look like a task row, not a crime scene.

## 11. Preferred StardewUI view usage for JAT ##

Use these views deliberately:

```text
- `frame`  
```

  shell, box, panel background, section wrapper

```text
- `lane`  
```

  most vertical/horizontal flow layouts

```text
- `panel`  
```

  layered regions, overlays, floating content

```text
- `grid`  
```

  repeated card/metric layouts, structured stat tiles

```text
- `scrollable`  
```

  lists and any region that can exceed visible height

```text
- `label` / `banner`  
```

  text hierarchy

```text
- `button`, `checkbox`, `dropdown`, `slider`, `textinput`, `tab`, `expander`  
```

  interaction controls

```text
- `include` / `template`  
```

  reuse for repeated row/item layouts

Do not use a more complicated view when a simpler one already fits.

## 12. Reuse patterns ##

### 12.1 Extract repeated row templates ###

When the same row structure appears repeatedly:

```text
- extract into a reusable include/template if repetition harms readability
- keep the repeated view meaningful and local to the feature area
```

### 12.2 Do not over-fragment ###

Do not split every tiny block into separate includes.
That is how readable UI becomes a scavenger hunt.

Good extraction:

```text
- task row
- task detail metadata block
- statistic summary card
- history day header
```

Bad extraction:

```text
- every label + spacer pair
- tiny one-off wrappers that are only used once
```

## 13. Local UI state patterns ##

### 13.1 Allowed local UI state ###

UI-local state may include:

```text
- selected task ID
- selected tab
- selected history day
- scroll positions
- collapsed/expanded panel state
- temporary filter selections
```

### 13.2 Forbidden local canonical state ###

UI must not own:

```text
- authoritative task completion state
- canonical pin state
- persisted task progress
- generator/rule evaluation results
```

Those belong to the State Store / backend.

## 14. UI action mapping pattern ##

Each interactive UI action should map cleanly to one of:

```text
- local UI state change
- command dispatch to backend
- navigation change
```

Examples:

```text
- click task row -> local selected task change
- click complete -> dispatch completion command
- click previous day -> local selected history day change, then query/render historical snapshot
- click open menu -> navigation/open-surface action
```

Do not hide a backend mutation behind a UI-only looking action.

## 15. Performance-aware composition rules ##

### 15.1 Stable repeated content ###

Repeated lists should use stable item templates and avoid rebuilding large trees unnecessarily.

### 15.2 Avoid decorative complexity in high-frequency surfaces ###

High-frequency surfaces should favor compact compositions, stable layout, and predictable bounds.
For HUD-specific performance constraints, follow [`ui-hud-patterns.instructions.md`](ui-hud-patterns.instructions.md).

### 15.3 Refresh only on meaningful changes ###

Patterns should assume refresh on:

```text
- new snapshot
- relevant config change
- relevant debug tuning change
```

Do not design components that expect continuous frame-driven rebuilding.

## 17. JAT anti-patterns ##

Agents should avoid these patterns unless the user explicitly requests them:

```text
- monolithic single-panel screens with no hierarchy
- placing task list and task details on different hidden subpages when they should be visible
together
- burying day navigation at the bottom of history screens
- mixing active and completed tasks without separation
- using HUD like a full configuration surface
- creating one-off custom layouts for every page instead of reusing shells
- scattering task actions across multiple unrelated regions
- overusing tabs where a split panel or grouped section would be clearer
```

## 18. Quick composition cheat sheet ##

Use this when in doubt:

```text
- **Task browsing** -> split view (`list + details`)
- **History** -> split view with day navigation at top
- **Statistics** -> summary cards + grouped sections
- **Wizard** -> step shell with back/next/review
- **HUD** -> compact header + active list + optional completed section
- **Debug** -> grouped utility controls, plain and readable
```

The objective is not to produce novel layout art every time. The objective is to produce UI that
feels like the same coherent mod instead of six different goblins each designing a room in the same
house.

