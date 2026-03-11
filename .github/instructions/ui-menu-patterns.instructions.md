---
name: ui-menu-patterns
description: "Menu-specific UI patterns for JAT. Use when: designing or implementing menu surfaces, layout shells, task list/detail panels, history browsing, statistics views, wizard flows, or debug UI."
applyTo: "{UI/**/*.cs,**/*.sml}"
---

# UI-MENU-PATTERNS.instructions.md #

## Purpose ##

This document defines **preferred UI patterns specific to JAT menu surfaces**.

It covers layout shells, task list and detail panels, history browsing, statistics, wizard flows,
and debug UI. Load this file when working on any menu surface in JAT.

**Related Resources:**

- [`ui-component-patterns.instructions.md`](ui-component-patterns.instructions.md) — Shared patterns (task rows, local state, action mapping, anti-patterns)
- [`ui-hud-patterns.instructions.md`](ui-hud-patterns.instructions.md) — HUD-specific patterns
- [`ui-interaction-patterns.instructions.md`](ui-interaction-patterns.instructions.md) — Modal, form, state transition, and accessibility patterns
- [`visual-design-language.instructions.md`](visual-design-language.instructions.md) — Colors, spacing, visual identity
- [`FRONTEND-ARCHITECTURE-CONTRACT.instructions.md`](../Contracts/FRONTEND-ARCHITECTURE-CONTRACT.instructions.md) — Architecture rules
- [`SML-STYLE-CONTRACT.instructions.md`](../Contracts/SML-STYLE-CONTRACT.instructions.md) — StarML syntax rules

## 1. Canonical menu layout shells ##

### 1.1 Shell pattern: Screen frame ###

Use this for most menus and major panels.

Structure:

1. outer `frame`
2. vertical `lane`
3. header region
4. body region
5. optional footer/actions region

Recommended StarML skeleton:

```xml
<frame layout="stretch content">
<lane orientation="vertical">
    <!-- Header -->
    <frame>
        <lane orientation="horizontal">
            <banner text={Title} />
        </lane>
    </frame>
  <!-- Body -->
            <panel>
                <!-- main content -->
            </panel>

  <!-- Footer -->
  <frame *if={ShowFooter}>
    <lane orientation="horizontal">
      <!-- actions -->
    </lane>
  </frame>

 </lane>
</frame>
```

Use when:

```text
- building menu pages
- building settings sections
- building statistics/history panels
- building wizard steps
```

### 1.2 Split-view shell: List + details ###

This is the **default JAT menu pattern** for task browsing.

Structure:

1. page shell
2. top navigation/header
3. horizontal body lane
4. left scrollable list panel
5. right details panel

Use this for:

```text
- current task browsing
- history task browsing
- rule browsing
- any "select item on left, inspect on right" workflow
```

JAT rule:

```text
- default to split view before considering alternate layouts
```

### 1.3 Tab-shell pattern ###

Use tabs only for **major top-level mode switching**, not as a substitute for normal grouping.

Good uses:

```text
- Tasks
- History
- Statistics
- Rules
- Configuration
- Debug
```

Bad uses:

```text
- replacing a simple section header
- splitting tiny related settings into many tabs
- burying essential actions several clicks deep
```

## 2. Task list panel patterns ##

### 2.1 Menu task list panel ###

The left panel in the split view should usually contain:

```text
- section title or mode label
- date/history selector when relevant
- optional filter/sort controls
- scrollable repeated list of task rows
```

Recommended hierarchy:

```text
TaskListPanel
 └ VerticalLane
    ├ TaskListHeader
    ├ TaskListControls
    └ ScrollableTaskRows
```

### 2.2 Filter bar pattern ###

Use a compact horizontal control group above the list for:

```text
- search (if implemented)
- status filter
- category filter
- sort selection
- day navigation/history navigation
```

Do not bury essential filter controls below the list.

### 2.3 Empty-state pattern ###

When no tasks exist for the selected view:

```text
- show a friendly empty-state panel in the content area
- explain what the surface is showing
- optionally suggest a next action (open rules, create manual task, change day, etc.)
```

Do not leave the panel looking broken or blank.

## 3. Task detail panel patterns ##

### 3.1 Standard detail panel ###

The right-side details panel should be the canonical inspection surface for a selected task.

Recommended structure:

1. title row
2. metadata block
3. progress/deadline block
4. description/details body
5. actions row

### 3.2 Metadata block pattern ###

Metadata may include:

```text
- category
- status
- pin state
- source
- deadline summary
- generated/manual/rule origin where relevant
```

Keep metadata compact and grouped.
Do not scatter metadata in random unrelated corners.

### 3.3 Progress block pattern ###

Progress presentation should be consistent across the app:

```text
- current / target
- progress percentage if useful
- deadline or remaining time if relevant
- overdue state if relevant
```

Avoid changing the visual grammar of progress from screen to screen.

### 3.4 Action row pattern ###

Actions belong near the bottom or bottom-right area of the detail panel.

Typical actions:

```text
- complete / uncomplete
- pin / unpin
- edit manual task
- remove manual task
- open related rule (if applicable)
```

Do not spread task actions across multiple distant regions of the panel.

### 3.5 No-selection pattern ###

When no task is selected:

```text
- show a placeholder detail panel with a brief instruction
- do not leave the detail pane empty with no explanation
```

## 4. History browsing patterns ##

### 4.1 History page shell ###

History should use the same list + details split view as live tasks whenever possible.

Recommended structure:

```text
- header with selected day navigation
- left historical task list
- right historical task detail panel
```

### 4.2 Day navigation pattern ###

Place day navigation controls near the top of the page.
Good controls:

```text
- previous day
- next day
- current selected day label
- optional jump/select control
```

This follows the design requirement that task history be browsable by day.

### 4.3 Historical detail pattern ###

Historical task details may include:

```text
- completion state on that day
- progress snapshot
- deadlines relevant to that day
- task source / type
```

Historical views should feel like "what was true on that day," not a mutated live task panel.

### 4.4 Empty-history pattern ###

If no snapshot exists:

```text
- show a clear "No history for this day" state
- do not imply data corruption unless there is actual evidence of corruption
```

## 5. Statistics view patterns ##

### 5.1 Statistics shell ###

Statistics should use a dashboard-style composition:

1. top summary band
2. grouped statistic cards/sections
3. optional supporting detail lists or charts

### 5.2 Summary card pattern ###

Use small summary cards for high-level numbers:

```text
- tasks completed
- completion rate
- overdue counts
- streak-like counts if implemented
- category summaries
```

### 5.3 Grouped-section pattern ###

After summary cards, organize supporting stats by theme:

```text
- completion behavior
- task category distribution
- deadline/overdue patterns
- historical trends
```

### 5.4 Statistics readability rule ###

Statistics should be understandable without requiring the player to decode a spreadsheet goblin
ritual.
Prefer simple labels, obvious grouping, and limited visual density.

## 6. Wizard and multi-step flow patterns ##

### 6.1 Wizard shell ###

Use a linear vertical shell with:

```text
- step title
- progress/step indicator
- body content for current step
- back / next / confirm actions
```

### 6.2 One logical decision per step ###

A wizard step should collect one coherent piece of information:

```text
- rule type
- trigger
- subject
- progress model
- timing
- metadata
- review
```

Do not cram multiple unrelated decisions into one giant step.

### 6.3 Review step pattern ###

The final review screen should summarize:

```text
- rule type
- trigger
- subject
- progress target
- timing
- metadata/presentation options
```

Validation problems should be visible before confirmation.

## 7. Debug and development UI patterns ##

### 7.1 Debug menu pattern ###

Debug surfaces should be plain and utilitarian.
The point is fast tuning, not artistic flourish.

Recommended structure:

```text
- grouped parameter sections
- small labeled controls
- optional diagnostic toggles
- optional read-only state inspection blocks
```

### 7.2 Runtime tuning group pattern ###

Group live-tunable values by subsystem:

```text
- HUD layout
- menu layout
- scrolling
- diagnostics
- overlay visibility
```

### 7.3 Debug inspector pattern ###

For state inspection:

```text
- use clearly labeled rows/blocks
- display raw identifiers where useful
- keep read-only inspection separate from destructive actions
```

## 8. JAT default page compositions ##

When designing a new menu page, use these as the default starting point:

### 8.1 Current tasks page ###

```text
- header with day / title
- task list panel
- detail panel
```

### 8.2 History page ###

```text
- header with previous/next day selector
- historical list panel
- historical detail panel
```

### 8.3 Statistics page ###

```text
- summary cards
- grouped supporting sections
```

### 8.4 Rules page ###

```text
- rule list panel
- rule detail/summary panel
- create/edit entry points
```

### 8.5 Debug page ###

```text
- grouped tuning controls
- diagnostic toggles
- optional inspection blocks
```
