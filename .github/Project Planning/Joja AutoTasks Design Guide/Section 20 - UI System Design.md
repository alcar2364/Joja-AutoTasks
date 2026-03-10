# Section 20 - UI System Design #

## 20.1 Purpose ##

The UI system provides the primary interaction surface between the
player and the task engine.

The UI system must support two distinct experiences:

```text
- A lightweight in-game HUD for daily play
- A full menu experience for browsing tasks, viewing details, managing
rules, modifying configuration, and reviewing history
```

The HUD and menu are separate because they have different performance,
layout, and interaction requirements.

## 20.2 UI surfaces ##

The mod provides the following UI surfaces:

```text
- HUD surface
    * Always available during gameplay
    * Optimized for minimal draw cost and low interaction complexity
    * Provides quick interactions (scroll, select, collapse/expand,
    open menu)
- Menu surface
    * Opened on demand
    * Supports complex interaction flows including task browsing, task
    details, the Task Builder Wizard, and staged history browsing
- Configuration entry surface
    * A minimal access layer exposed via GMCM
    * Opens the full configuration menu implemented using StardewUI
```

Version 1 delivery rule:

```text
- Both HUD and Menu surfaces are required in Version 1.
- Neither surface may be dropped; sequencing only affects polish depth.
- Both surfaces must remain parity-aligned through shared snapshot data.
- Task Builder in Now remains rule-definition only and follows
    command/snapshot boundaries.
```

## 20.3 Shared UI data model ##

Both HUD and menus must display information derived from the same State
Store snapshot described in Section 8.

UI code must never directly mutate task state. UI actions must dispatch
commands to the State Store.

Conceptual flow:

``` text
UI Interaction → Command Dispatch → State Store → UI Refresh
```

This ensures deterministic state updates and prevents UI logic from
corrupting task state.

## 20.4 UI update and refresh model ##

UI refresh must be driven by state changes rather than frame-by-frame
recomputation.

The UI system must refresh when one of the following occurs:

```text
- A new State Store snapshot is published
- UI configuration values change (Section 15)
- Debug tuning values change (Section 17)
```

The UI system must avoid rebuilding view structures when the underlying
data has not changed.

## 20.5 StardewUI usage model ##

StardewUI is used across both UI surfaces.

```text
- HUD uses StardewUI for layout and rendering only.
- Menus use StardewUI for layout, rendering, and interaction.
```

StardewUI view composition uses StarML and provides standard views such
as scrollable regions, expanders, and tabs.

## 20.6 HUD design goals ##

The HUD provides a compact, real-time display of tasks during gameplay.

The HUD must prioritize:

```text
- user configurable screen space usage and positioning with constraints
    * User can move HUD to any position within the screen bounds
    * HUD size and text size can be adjusted by users
- consistent readability
- low draw cost
- predictable interactions
```

### 20.6.1 HUD implementation model (StardewUI IViewDrawable) ###

The HUD is implemented using StardewUI's `IViewDrawable` API.

The mod creates the HUD drawable using
`viewEngine.CreateDrawableFromAsset()`, which returns an `IViewDrawable`
bound to a StarML asset and a view model context.

The HUD drawable is drawn each frame by calling its `Draw()` method
during the game's render cycle. StardewUI handles layout, rendering,
and input routing through the drawable.

The HUD does not use StardewUI's menu pipeline (`IClickableMenu`).
Instead it operates as a non-modal drawable overlay rendered alongside
normal gameplay.

The view model bound to the HUD drawable follows the patterns defined
in Section 10A.

#### 20.6.2 HUD interactions ####

The HUD must support the following interactions:

```text
- Dragging the HUD to reposition it within the viewport
- Collapsing and expanding the HUD as a whole
- Collapsing and expanding completed tasks within the HUD
- Scrolling the task list using:
    * mouse wheel
    * scroll bar track/thumb interaction
    * scroll up/down buttons
    * click-and-hold behavior on scroll buttons
- Opening the full mod menu from the HUD
- Tooltip display for buttons with unclear functions
- User configurable tooltip to display detailed task information
```

HUD interactions must be translated into semantic UI actions that
either:

```text
- dispatch commands to the State Store, or
- update HUD-local UI state (selection, scroll position, collapsed
state, drag position)
```

HUD-local UI state must not be persisted as task state.

#### 20.6.3 HUD view composition ####

The HUD visual tree is defined using StardewUI view composition.

The HUD must construct a view tree representing:

```text
- HUD frame and panels
- Scrollable task list region
- Task row visuals (including completion affordances)
- HUD controls (scroll arrows, open menu button, collapse controls)
```

The HUD view tree must be rebuilt only when one of the following
changes:

```text
- UI configuration values affecting layout change
- Debug tuning values affecting layout change
- The set of visible tasks changes
```

Otherwise the HUD must reuse the existing view tree and cached layout.

#### 20.6.4 HUD interaction handling ####

StardewUI's `IViewDrawable` routes input events (hover, click, scroll)
through the view tree automatically. The mod does not need to implement
custom hit testing or maintain an interaction registry.

HUD interaction handling is limited to:

```text
- Binding event handlers in StarML (pipe-syntax `|`) to view model
methods
- Defining drag behavior for HUD repositioning (may require custom
handling outside the view tree if StardewUI does not natively
support drag-to-reposition on drawables)
```

If drag repositioning requires manual coordinate tracking, that logic
lives in the HUD host code (the class that owns the `IViewDrawable`),
not in the view model or StarML.

#### 20.6.5 HUD rendering constraints ####

HUD rendering occurs during the game's draw cycle.

The HUD renderer must:

```text
- avoid per-frame allocations
- avoid rebuilding the view tree every frame
- reuse cached layout results when possible
- restrict expensive layout recomputation to state/config changes
```

These constraints are required to satisfy the performance guardrails
defined in Section 19.

#### 20.6.6 HUD configuration and debug tuning ####

HUD layout and behavior parameters must be configurable through the
configuration system (Section 15).

When debug mode is enabled (Section 17), selected HUD layout parameters
must support live tuning through the debug configuration menu.

When live tuning is enabled, the HUD must read layout parameters from
runtime configuration values rather than static constants.

## 20.7 Menu system design (StardewUI) ##

The full menu system is implemented using StardewUI for:

```text
- view composition and layout
- menu rendering
- menu interaction handling
- control widgets (lists, buttons, text, toggles, tabs, scrollables)
```

Menus provide comprehensive task and rule management including:

```text
- browsing tasks and viewing task details in the same section
- creating custom rules via the Task Builder Wizard (Section 14)
- modifying configuration (Section 15)
- accessing debug tools (Section 17)
- reviewing task history
```

Task Builder boundary in the Menu surface:

```text
- Menu-hosted Task Builder interactions are limited to rule-definition authoring,
  validation, and persistence intents.
- Task Builder interactions must not directly mutate runtime task entities.
```

### 20.7.1 Tasks section (combined list + details) ###

The Tasks section must display the task list and task details together.

The Tasks section must use a split layout:

```text
- Left panel: scrollable task list
- Right panel: details for the currently selected task
```

Selection changes update the detail panel without leaving the Tasks
section.

The Tasks section must support filtering and sorting controls, but these
controls must remain menu-local state unless explicitly defined as
configuration.

#### 20.7.2 History section ####

The menu must provide a section dedicated to historical viewing.

History section requirements:

```text
- browse previous days
- view incomplete and completed tasks for a selected day
- support date navigation as the Version 1 (Now) baseline
- stage quick-jump depth to a specific day as Next hardening scope
```

History must not alter deterministic task identity. This view is a
read-only surface over persisted history described in Section 11.

Statistics section (V2): A dedicated statistics section showing
aggregate counts, completion rates, streaks, and breakdowns by
generator/category/task type is deferred to Version 2. The Daily
Snapshot Ledger provides the data foundation for future statistics.

##### 20.7.3 Menu navigation structure #####

The menu system must provide a structured navigation model that keeps
features organized without excessive depth.

The navigation model may be implemented using a tab system or other
clear navigation controls.

Example structure:

``` text
Task Manager Menu
 ├─ Tasks (list + details)
 ├─ Task Builder Wizard
 ├─ History
 ├─ Configuration
 └─ Debug
```

Version 2 may add a Statistics tab.

This structure may evolve, but the core requirement is that tasks,
history, task builder wizard, configuration, and debug tooling are
reachable through clear navigation.

##### 20.7.4 Menu interaction and command dispatch #####

Menu interactions must follow the same command dispatch model as the
HUD.

Menu interactions must not directly mutate task state. Instead, the menu
must dispatch commands to the State Store.

Task Builder-specific command constraint:

```text
- Task Builder UI emits rule-definition commands only.
- Runtime task state changes happen after rule evaluation through the State Store.
```

Menu-local state (selection, filters, sorting state, current history
date) must remain within the menu subsystem and must not be persisted as
task state unless explicitly defined as configuration.

## 20.8 Notification and toast system ##

The HUD may display brief notification toasts for important events.

Toasts are non-interactive, auto-dismissing messages that appear near
the HUD. They are driven by state changes, not by UI code directly.

### 20.8.1 Toast triggers ###

Toasts may be shown when:

```text
- A task completes automatically
- A Task Builder rule first activates
- A manual task reminder fires
- A daily summary is captured
```

Toasts must not appear for routine state refreshes or minor progress
updates.

#### 20.8.2 Toast behavior ####

```text
- Toasts auto-dismiss after a short duration (approximately 3-5
seconds)
- Multiple toasts queue and display sequentially, not stacked
- Toasts must not block gameplay input
- The player may configure toast visibility and duration through the
configuration system (Section 15)
```

#### 20.8.3 Toast rendering ####

Toasts are rendered as part of the HUD drawable's view tree. Toast
visibility is driven by the `HudViewModel` (Section 10A) which manages
a toast queue and exposes the current toast for binding.

The visual design of toasts follows the notification system defined in
`visual-design-language.instructions.md` (Section 11).

## 20.9 Gamepad and controller support ##

StardewUI provides built-in controller and gamepad support for both
menus and drawables. The mod does not need to implement custom gamepad
input handling.

All interactive elements defined in StarML are automatically navigable
via gamepad. Custom focus order can be specified using StardewUI's
focus-navigation attributes if the default tab order is insufficient.

## 20.10 First-run onboarding ##

The mod should provide a lightweight onboarding experience on first use.

Onboarding goals:

```text
- Welcome the player and briefly explain what the mod does
- Point out the HUD and how to interact with it
- Offer a link to create a first Task Builder rule or browse sample
rules
- Provide a sample set of pre-configured rules the player can enable
```

Onboarding must not block gameplay. It should be dismissible and must
not reappear once acknowledged. The onboarding state is persisted as
part of `StoreUserState` (Section 9.8).

## 20.11 UI consistency rules ##

The UI system must follow these consistency rules:

1. HUD and menus must reflect the same underlying task snapshot model.
2. UI interactions must dispatch commands rather than mutating task

```text
state directly.
```

1. Deterministic task identity must never depend on UI configuration or

```text
UI-local state.
```

1. UI must refresh based on state/config changes rather than frame

```text
updates.
```

These rules ensure predictable behavior across all UI surfaces.

## 20.12 Localization Model (SMAPI I18n) ##

HUD and menu localization must follow a shared model.

Rules:

```text
- Both UI surfaces resolve translated text through SMAPI `I18n` using stable localization keys and arguments.
- Translation resolution occurs in UI/view-model binding, not in backend engine/state/persistence layers.
- Missing translation keys must follow a consistent fallback path across HUD and menu (explicit fallback text first, unresolved key token second).
- Locale changes should trigger UI refresh through normal snapshot/view update paths without mutating canonical task state.
- Deterministic IDs, equality, and ordering remain locale-neutral and must never depend on translated text.
```

## 20.13 No-drop UI delivery staging ##

UI sequencing follows the no-drop Now/Next/Later model in Section 21.

Capability-level mapping is canonical in Section 21.3.1.

```text
- Now: deliver required Version 1 HUD + Menu capabilities, including Task Builder
    rule-definition flow and baseline history/debug entry points.
- Next: deliver remaining Phase 11 and Phase 12 UX depth (history filtering,
    quick jump, debug ergonomics) without removing capabilities.
- Later: deliver post-Version-1 UI expansion such as statistics dashboards.
```

Promotion from Now to Next requires passing boundary, parity, performance,
and documentation-sync gates defined in Section 21.3.2.
