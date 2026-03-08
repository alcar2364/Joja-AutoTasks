---
name: ui-component-patterns
description: "Preferred UI composition patterns for JAT. Use when: designing or implementing UI layouts, choosing component structures, or avoiding layout pattern proliferation."
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

## 2. Canonical JAT layout shells ##

### 2.1 Shell pattern: Screen frame ###

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

### 2.2 Split-view shell: List + details ###

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
- any “select item on left, inspect on right” workflow
```

JAT rule:

```text
- default to split view before considering alternate layouts
```

### 2.3 Tab-shell pattern ###

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

## 3. HUD component patterns ##

### 3.1 HUD overall composition ###

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

```text

```

### 3.2 HUD header pattern ###

Header should contain only high-value controls/information:

```text
- current day / date context if shown
- collapse/expand control
- open-menu control
- optional quick summary
```

Do not overload the header with settings or debug-only clutter.

### 3.3 HUD task list pattern ###

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
- any interaction that feels like “editing a record”
```

### 3.4 HUD completed section pattern ###

If completed tasks are shown in HUD:

```text
- place them in a dedicated collapsible section below active tasks
- clearly separate active vs completed
- default behavior may collapse this region when screen space is tight
```

Do not interleave completed tasks among active tasks.

### 3.5 HUD scrolling pattern ###

For large task sets:

```text
- use a scrollable region or equivalent bounded viewport
- keep scroll controls visually anchored to the task list, not floating ambiguously
- preserve scroll state locally in the HUD layer
```

The HUD should support quick scanning, not long-form browsing.

### 3.6 HUD interaction budget ###

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

## 5. Task list panel patterns ##

### 5.1 Menu task list panel ###

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
```

├ TaskListHeader
├ TaskListControls
└ ScrollableTaskRows

```text

```

### 5.2 Filter bar pattern ###

Use a compact horizontal control group above the list for:

```text
- search (if implemented)
- status filter
- category filter
- sort selection
- day navigation/history navigation
```

Do not bury essential filter controls below the list.

### 5.3 Empty-state pattern ###

When no tasks exist for the selected view:

```text
- show a friendly empty-state panel in the content area
- explain what the surface is showing
- optionally suggest a next action (open rules, create manual task, change day, etc.)
```

Do not leave the panel looking broken or blank.

## 6. Task detail panel patterns ##

### 6.1 Standard detail panel ###

The right-side details panel should be the canonical inspection surface for a selected task.

Recommended structure:

1. title row
2. metadata block
3. progress/deadline block
4. description/details body
5. actions row

### 6.2 Metadata block pattern ###

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

### 6.3 Progress block pattern ###

Progress presentation should be consistent across the app:

```text
- current / target
- progress percentage if useful
- deadline or remaining time if relevant
- overdue state if relevant
```

Avoid changing the visual grammar of progress from screen to screen.

### 6.4 Action row pattern ###

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

### 6.5 No-selection pattern ###

When no task is selected:

```text
- show a placeholder detail panel with a brief instruction
- do not leave the detail pane empty with no explanation
```

## 7. History browsing patterns ##

### 7.1 History page shell ###

History should use the same list + details split view as live tasks whenever possible.

Recommended structure:

```text
- header with selected day navigation
- left historical task list
- right historical task detail panel
```

### 7.2 Day navigation pattern ###

Place day navigation controls near the top of the page.
Good controls:

```text
- previous day
- next day
- current selected day label
- optional jump/select control
```

This follows the design requirement that task history be browsable by day.

### 7.3 Historical detail pattern ###

Historical task details may include:

```text
- completion state on that day
- progress snapshot
- deadlines relevant to that day
- task source / type
```

Historical views should feel like “what was true on that day,” not a mutated live task panel.

### 7.4 Empty-history pattern ###

If no snapshot exists:

```text
- show a clear “No history for this day” state
- do not imply data corruption unless there is actual evidence of corruption
```

## 8. Statistics view patterns ##

### 8.1 Statistics shell ###

Statistics should use a dashboard-style composition:

1. top summary band
2. grouped statistic cards/sections
3. optional supporting detail lists or charts

### 8.2 Summary card pattern ###

Use small summary cards for high-level numbers:

```text
- tasks completed
- completion rate
- overdue counts
- streak-like counts if implemented
- category summaries
```

### 8.3 Grouped-section pattern ###

After summary cards, organize supporting stats by theme:

```text
- completion behavior
- task category distribution
- deadline/overdue patterns
- historical trends
```

### 8.4 Statistics readability rule ###

Statistics should be understandable without requiring the player to decode a spreadsheet goblin
ritual.
Prefer simple labels, obvious grouping, and limited visual density.

## 9. Wizard and multi-step flow patterns ##

### 9.1 Wizard shell ###

Use a linear vertical shell with:

```text
- step title
- progress/step indicator
- body content for current step
- back / next / confirm actions
```

### 9.2 One logical decision per step ###

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

### 9.3 Review step pattern ###

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

## 10. Debug and development UI patterns ##

### 10.1 Debug menu pattern ###

Debug surfaces should be plain and utilitarian.
The point is fast tuning, not artistic flourish.

Recommended structure:

```text
- grouped parameter sections
- small labeled controls
- optional diagnostic toggles
- optional read-only state inspection blocks
```

### 10.2 Runtime tuning group pattern ###

Group live-tunable values by subsystem:

```text
- HUD layout
- menu layout
- scrolling
- diagnostics
- overlay visibility
```

### 10.3 Debug inspector pattern ###

For state inspection:

```text
- use clearly labeled rows/blocks
- display raw identifiers where useful
- keep read-only inspection separate from destructive actions
```

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

### 15.2 Avoid decorative complexity in HUD ###

The HUD is performance-sensitive. Use compact compositions, stable layout, and predictable bounds.

### 15.3 Refresh only on meaningful changes ###

Patterns should assume refresh on:

```text
- new snapshot
- relevant config change
- relevant debug tuning change
```

Do not design components that expect continuous frame-driven rebuilding.

## 16. JAT default page compositions ##

When agents need a default, use these:

### 16.1 Current tasks page ###

```text
- header with day / title
- task list panel
- detail panel
```

### 16.2 History page ###

```text
- header with previous/next day selector
- historical list panel
- historical detail panel
```

### 16.3 Statistics page ###

```text
- summary cards
- grouped supporting sections
```

### 16.4 Rules page ###

```text
- rule list panel
- rule detail/summary panel
- create/edit entry points
```

### 16.5 Debug page ###

```text
- grouped tuning controls
- diagnostic toggles
- optional inspection blocks
```

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

## 19. Modal dialog patterns ##

### 19.1 Dialog shell ###

Modal dialogs should use a centered overlay pattern:

```text
- dark/semi-transparent backdrop
- centered dialog frame
- clear title
- body content
- action buttons (confirm/cancel)
```

### 19.2 Confirmation dialog ###

Use for destructive or irreversible actions:

```text
- clear prompt
- concise explanation of what will happen
- confirm / cancel buttons
```

Examples:

```text
- delete manual task
- clear all completed tasks
- reset configuration
```

### 19.3 Input dialog ###

Use for simple single-value input:

```text
- prompt label
- text input field
- optional validation hint
- confirm / cancel buttons
```

Example: rename custom task category.

### 19.4 Dialog action placement ###

Place actions at the bottom-right of the dialog:

```text
- confirm (primary) on the right
- cancel on the left
```

Follow Stardew Valley dialog conventions.

### 19.5 Dialog backdrop behavior ###

Clicking the backdrop should be equivalent to cancel/close, not a no-op.

## 20. Form and input patterns ##

### 20.1 Form structure ###

Use vertical form layout:

```text
- field label
- input control
- optional validation message
- next field
```

Keep related fields grouped.

### 20.2 Validation presentation ###

Show validation errors inline below the relevant field:

```text
- red or warning color text
- concise error message
- do not block or hide the invalid field
```

### 20.3 Form submit pattern ###

Place submit/confirm actions at the bottom of the form:

```text
- primary action on the right
- cancel/back on the left
```

Validate before submission when possible.

### 20.4 Multi-field wizard pattern ###

For complex multi-field workflows, use the wizard shell (see section 9) instead of a giant single form.

### 20.5 Input field types ###

Map JAT input needs to StardewUI controls:

| Input Type | StardewUI Control |
| --- | --- |
| Short text | `textinput` |
| Number | `textinput` or `slider` |
| Boolean | `checkbox` |
| Multiple choice (small) | `dropdown` |
| Multiple choice (large) | list selection panel |
| Date/day | Custom day picker or `dropdown` |

## 21. State transition patterns ##

### 21.1 Empty state ###

When a list or panel has no content:

```text
- centered message (not top-left)
- clear explanation of what's missing
- suggest a next action if appropriate
```

Examples:

```text
- no tasks for selected day
- no rules defined
- no history available
```

### 21.2 Loading state ###

If content requires async loading:

```text
- show loading indicator in the content area
- keep shell/header stable
- do not show stale data during load
```

JAT does not currently use async UI loading, but this is the pattern if needed.

### 21.3 Error state ###

If the UI encounters an error rendering content:

```text
- clear error message
- suggest recovery action if known
- log technical details to mod console
```

Do not silently fail or show cryptic placeholder.

### 21.4 Disabled state ###

When a control or action is unavailable:

```text
- use disabled visual style (muted color, reduced opacity)
- optionally show tooltip explaining why
```

Do not hide controls that are sometimes available; disable them instead.

## 22. Accessibility considerations ##

JAT UI should be usable and readable without excessive visual or interaction complexity.

### 22.1 Text readability ###

- Use readable font sizes (reference [`visual-design-language.instructions.md`](visual-design-language.instructions.md) for typography scale)
- Ensure sufficient contrast between text and background
- Avoid relying solely on color to convey status (use icons/text labels)

### 22.2 Keyboard navigation ###

- Ensure all interactive controls are focusable and keyboard-navigable
- Preserve logical tab order
- Use visual focus indicators for keyboard users

### 22.3 Interaction clarity ###

- Clickable elements should have clear hover/focus states
- Use tooltips for icon-only actions
- Avoid tiny click targets

### 22.4 Cognitive load ###

- Keep layout hierarchy clear and predictable
- Avoid overwhelming the player with dense information
- Use progressive disclosure (collapsible sections, tabs) for complexity management

For color and spacing accessibility guidelines, see [`visual-design-language.instructions.md`](visual-design-language.instructions.md) section on Accessibility.
