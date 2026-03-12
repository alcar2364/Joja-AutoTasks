---
name: jat-ui-component-patterns
description: Reusable HUD/menu composition patterns for JAT. Use when: selecting layout shells, implementing list-details flows, organizing task rows, or preventing one-off UI structure drift.
argument-hint: "UI surface + user flow + layout constraints"
---

<!-- markdownlint-disable -->

# JAT UI Component Patterns #

Use this skill when planning or implementing JAT UI structure so surfaces stay consistent.

## When to Use ##

    * Building or refactoring HUD or menu layout structures.
    * Choosing between shell, split-view, and tabbed compositions.
    * Standardizing row, list panel, and details panel patterns.
    * Preventing repeated one-off layout inventions across screens.

## Procedure ##

1. Identify surface scope: HUD or full menu.
2. Select canonical shell from the Pattern Map section.
3. Map interactions to the correct surface complexity budget.
4. Keep task list and task details coordinated in menu flows.
5. Apply row and panel patterns before visual polish.
6. Validate architecture boundary: UI consumes snapshots and dispatches commands.

## Guardrails ##

    * HUD stays lightweight and quick-scannable.
    * Menu handles deeper browsing and management.
    * Prefer split view for list plus details workflows.
    * Keep hierarchy obvious: header, navigation/filters, content, details, actions.

## Pattern Map ##

### Screen Frame Shell ###

Use for major menu pages:

1. Outer frame
2. Vertical lane
3. Header region
4. Body region
5. Optional footer/actions region

### Split View Shell ###

Default for task browsing:

1. Header/navigation
2. Horizontal body lane
3. Left list panel (scrollable)
4. Right details panel

### Tab Shell ###

Use tabs only for major top-level modes, not small sectioning.

Good examples: Tasks, History, Rules, Configuration, Debug.

### HUD Patterns ###

    * Compact header with high-value controls only.
    * Active task list as primary content.
    * Optional completed section separated and collapsible.
    * Bounded interactions only: scroll, select, quick complete toggle, collapse, open menu, drag.

### Row Patterns ###

    * Compact row: icon/status marker, primary text lane, trailing badge/action.
    * Selected row: clear but subtle emphasis.
    * Pinned/urgent states: markers or badges, not a new row structure.

### List Panel Patterns ###

    * Header or section title
    * Filter/sort/day controls
    * Scrollable repeated rows

Keep controls near the list they affect.

## References ##

    * [UI Component Patterns Instruction](../Instructions/ui-component-patterns.instructions.md)
    * [Frontend Architecture Contract](../Contracts/FRONTEND-ARCHITECTURE-CONTRACT.instructions.md)
