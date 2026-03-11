---
name: ui-interaction-patterns
description: "Interaction-focused UI patterns for JAT. Use when: designing modal dialogs, form flows, state transitions, and accessibility behavior in UI surfaces."
applyTo: "{UI/**/*.cs,**/*.sml}"
---

# UI-INTERACTION-PATTERNS.instructions.md #

## Purpose ##

This document defines **interaction-focused UI patterns** for **JAT (Joja AutoTasks)**.

Use this file for modal/dialog behavior, form/input structure, state transition presentation,
and accessibility-related interaction clarity.

**Related Resources:**

- [`ui-component-patterns.instructions.md`](ui-component-patterns.instructions.md) - Layout and composition patterns
- [`visual-design-language.instructions.md`](visual-design-language.instructions.md) - Color, spacing, typography, and accessibility baseline
- [`SML-STYLE-CONTRACT.instructions.md`](../Contracts/SML-STYLE-CONTRACT.instructions.md) - StarML syntax rules

## 1. Modal dialog patterns ##

### 1.1 Dialog shell ###

Modal dialogs should use a centered overlay pattern:

```text
- dark/semi-transparent backdrop
- centered dialog frame
- clear title
- body content
- action buttons (confirm/cancel)
```

### 1.2 Confirmation dialog ###

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

### 1.3 Input dialog ###

Use for simple single-value input:

```text
- prompt label
- text input field
- optional validation hint
- confirm / cancel buttons
```

Example: rename custom task category.

### 1.4 Dialog action placement ###

Place actions at the bottom-right of the dialog:

```text
- confirm (primary) on the right
- cancel on the left
```

Follow Stardew Valley dialog conventions.

### 1.5 Dialog backdrop behavior ###

Clicking the backdrop should be equivalent to cancel/close, not a no-op.

## 2. Form and input patterns ##

### 2.1 Form structure ###

Use vertical form layout:

```text
- field label
- input control
- optional validation message
- next field
```

Keep related fields grouped.

### 2.2 Validation presentation ###

Show validation errors inline below the relevant field:

```text
- red or warning color text
- concise error message
- do not block or hide the invalid field
```

### 2.3 Form submit pattern ###

Place submit/confirm actions at the bottom of the form:

```text
- primary action on the right
- cancel/back on the left
```

Validate before submission when possible.

### 2.4 Multi-field wizard pattern ###

For complex multi-field workflows, use the wizard shell (see section 9 in
`ui-component-patterns.instructions.md`) instead of a giant single form.

### 2.5 Input field types ###

Map JAT input needs to StardewUI controls:

| Input Type | StardewUI Control |
| --- | --- |
| Short text | `textinput` |
| Number | `textinput` or `slider` |
| Boolean | `checkbox` |
| Multiple choice (small) | `dropdown` |
| Multiple choice (large) | list selection panel |
| Date/day | Custom day picker or `dropdown` |

## 3. State transition patterns ##

### 3.1 Empty state ###

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

### 3.2 Loading state ###

If content requires async loading:

```text
- show loading indicator in the content area
- keep shell/header stable
- do not show stale data during load
```

JAT does not currently use async UI loading, but this is the pattern if needed.

### 3.3 Error state ###

If the UI encounters an error rendering content:

```text
- clear error message
- suggest recovery action if known
- log technical details to mod console
```

Do not silently fail or show cryptic placeholder.

### 3.4 Disabled state ###

When a control or action is unavailable:

```text
- use disabled visual style (muted color, reduced opacity)
- optionally show tooltip explaining why
```

Do not hide controls that are sometimes available; disable them instead.

## 4. Accessibility considerations ##

JAT UI should be usable and readable without excessive visual or interaction complexity.

### 4.1 Text readability ###

- Use readable font sizes (reference [`visual-design-language.instructions.md`](visual-design-language.instructions.md) for typography scale)
- Ensure sufficient contrast between text and background
- Avoid relying solely on color to convey status (use icons/text labels)

### 4.2 Keyboard navigation ###

- Ensure all interactive controls are focusable and keyboard-navigable
- Preserve logical tab order
- Use visual focus indicators for keyboard users

### 4.3 Interaction clarity ###

- Clickable elements should have clear hover/focus states
- Use tooltips for icon-only actions
- Avoid tiny click targets

### 4.4 Cognitive load ###

- Keep layout hierarchy clear and predictable
- Avoid overwhelming the player with dense information
- Use progressive disclosure (collapsible sections, tabs) for complexity management

For color and spacing accessibility guidelines, see [`visual-design-language.instructions.md`](visual-design-language.instructions.md) section on Accessibility.
