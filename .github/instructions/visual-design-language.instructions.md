---
name: visual-design-language
description: "JAT visual design language, color palette, spacing, typography, and UX foundation. Use when: implementing UI visuals, choosing colors/spacing values, or designing new UI components."
applyTo: "{**/*.sml,UI/**/*.cs}"
---

# Visual Design Language — JAT #

This document defines the **visual design language** for the Joja AutoTasks (JAT) user interface.

**Related Resources:**

- [`ui-component-patterns.instructions.md`](ui-component-patterns.instructions.md) — UI composition patterns
- [`SML-STYLE-CONTRACT.instructions.md`](../Contracts/SML-STYLE-CONTRACT.instructions.md) — StarML syntax rules

The goal is to establish a **consistent visual identity and UX foundation** before implementing the
HUD and menu systems.

This design language will evolve during development. Values defined here may be tuned using debug UI
layout tools.

The design language should remain consistent with Stardew Valley's visual style while incorporating
a subtle **Joja corporate aesthetic** as a thematic overlay.

## 1. Visual Design Philosophy ##

The visual design should balance two ideas:

### 1. Stardew-native UI familiarity ###

```text
- Pixel art
- Warm earthy tones
- Rounded menu frames
- Simple readable layouts
```

### 2. Joja corporate productivity theme ###

```text
- Clean layout
- Structured panels
- Blue / white accents
- Organized information hierarchy
```

Tone:

"Joja productivity software accidentally shipped inside Stardew Valley."

## 2. Color Palette ##

### Primary Accent (Joja) ###

| Purpose | Color | Hex |
| --- | --- | --- |
| Primary accent | Joja Blue | #2C6BE0 |
| Light accent | Joja Light Blue | #6EA6FF |
| Highlight | Soft Blue | #A8C8FF |

### Neutral UI Colors ###

| Purpose | Description |
| --- | --- |
| Panel background | Stardew parchment |
| Panel border | Stardew menu border |
| Text primary | Dark brown |
| Text secondary | Grey-brown |
| Disabled text | Muted grey |

### Status Colors ###

| Status | Color |
| --- | --- |
| Completed task | Green |
| In-progress task | Blue |
| Warning / deadline soon | Orange |
| Overdue | Red |

## 3. Typography ##

Primary font: **Stardew Valley dialogue/menu font**

| Usage | Size |
| --- | --- |
| HUD task text | Small |
| Menu task list | Medium |
| Section headings | Large |
| Wizard titles | Large |
| Metadata text | Small |

## 4. Spacing System ##

Base spacing unit: **4px**

| Unit | Size |
| --- | --- |
| XS | 4px |
| SM | 8px |
| MD | 12px |
| LG | 16px |
| XL | 24px |

Example usage:

```text
- Task row padding: SM (8px)
- Panel padding: MD–LG (12–16px)
- Section spacing: XL (24px)
```

**Mapping to StardewUI attributes:**

Spacing values map to StarML layout attributes:

| Design Token | StarML Attribute | Example |
| --- | --- | --- |
| SM (8px) | `margin="8"` | `<label text="Task" margin="8" />` |
| MD (12px) | `padding="12"` | `<frame padding="12">` |
| LG (16px) | `margin="16, 0"` | Horizontal margin only |
| XL (24px) | `padding="0, 24"` | Vertical padding only |

Use design tokens consistently; avoid arbitrary spacing values.

## 5. Animation and Transitions ##

Animations should be short and lightweight.

### Completion Sound ###

```text
- Checkmark appears
- Text fades to completed style
- Optional sparkle
```

Duration: **200–300ms**

### HUD Expand / Collapse ###

```text
- Vertical slide
- Subtle fade
```

Duration: **150–200ms**

### Wizard Step Transition ###

```text
- Horizontal slide
- Light fade
```

Duration: **200ms**

## 6. Iconography ##

Icons should follow **Stardew pixel art style**.

Sources:

```text
- Stardew item sprites
- Stardew UI icons
- Custom pixel icons
```

### Task Type Icons ###

| Task Type | Icon Example |
| --- | --- |
| Farming | Crop |
| Animals | Cow |
| Machines | Keg |
| Social | Heart |
| Resources | Wood / Stone |

### Status Icons ###

| Status | Icon |
| --- | --- |
| Completed | Checkmark |
| Incomplete | Empty circle |
| Progress | Progress bar |
| Reminder | Clock |

## 7. Sound Design ##

### Task Completion ###

```text
- Short confirmation sound
- Similar to quest completion
```

### Notification ###

```text
- Soft bell notification
```

Must avoid excessive repetition.

## 8. HUD Layout Concept ##

Purpose: **quick gameplay reference**.

Example layout:

```text
+--------------------------------+
| Year 2 - Spring 12 |
| -------------------------------- |
| [ ] Water crops |
| [ ] Pet animals |
| [ ] Collect machine output |
| [x] Check mail |
| -------------------------------- |
| [^] [v] |
| Open Menu |
+--------------------------------+
```

Features:

```text
- Scrollable task list
- Expand/collapse
- Drag reposition
- Optional completed tasks
```

## 9. Menu Layout Concept ##

Split panel design:

```text
+----------------------------------------------------+
| Tasks                                              |
|----------------------------------------------------|
| Task List              | Task Details              |
|------------------------|---------------------------|
| [ ] Water crops        | Title                     |
| [ ] Pet animals        | Description               |
| [ ] Collect machine    | Progress                  |
| [x] Check mail         | Deadline                  |
|                        | Category                  |
|                        | Source                    |
+----------------------------------------------------+
```

Left panel: task list

Right panel: details and metadata

## 10. Wizard Layout Concept ##

Step-based workflow:

```text
[ Step 1 ] -> [ Step 2 ] -> [ Step 3 ] -> [ Review ]
```

Each step includes:

```text
- Title
- Input fields
- Preview
- Next / Back navigation
```

## 11. Notification / Toast Concept ##

Lightweight notification overlay for task events:

```text
+-----------------------------+
| [checkmark] Task completed  |
| Water crops                 |
+-----------------------------+
```

Behavior:

```text
- Appears near the HUD or screen edge
- Auto-dismisses after 2–3 seconds
- Stacks if multiple notifications arrive
- Does not interrupt gameplay
- Optional sound cue
```

Trigger conditions:

```text
- Task completed (automatic or manual)
- Deadline approaching (configurable threshold)
- New task created by rule
- Rule validation warning
```

## 12. Design Consistency Rules ##

1. Follow Stardew UI conventions.
2. Use Joja colors only as accent.
3. Keep HUD readable at small scale.
4. Keep animations subtle.
5. Avoid clutter.

## 13. Accessibility ##

JAT UI should be usable and readable for all players.

### 13.1 Color Contrast ###

Ensure sufficient contrast between text and background:

| Element | Minimum Contrast Ratio |
| --- | --- |
| Primary text | 4.5:1 |
| Large headings | 3:1 |
| UI controls | 3:1 |

Test critical UI elements for readability.

**Status indicators:** Do not rely solely on color to convey status (e.g., completed, overdue). Use icons, text labels, or visual patterns in addition to color.

### 13.2 Focus Indicators ###

All interactive elements must have clear focus indicators for keyboard navigation:

```text
- border highlight
- background color change
- subtle glow or outline
```

Focus indicators should follow Stardew Valley's existing focus style where possible.

### 13.3 Keyboard Navigation ###

All functionality accessible by mouse must also be accessible by keyboard:

```text
- tab order follows logical reading order
- enter/space activates buttons
- arrow keys navigate lists and dropdowns
- escape closes dialogs and menus
```

### 13.4 Text Size and Readability ###

- Use font sizes defined in Section 3 (Typography)
- Avoid text smaller than "Small" size for critical information
- Ensure adequate line spacing for readability

### 13.5 Interaction Targets ###

- Minimum clickable area: 32×32 pixels for touch-like controllers
- Adequate spacing between adjacent interactive elements
- Clear hover states for all clickable elements

### 13.6 Cognitive Load ###

- Use progressive disclosure (collapsible sections, tabs) for complex information
- Keep visual hierarchy clear and predictable
- Avoid overwhelming the player with dense information grids
- Provide clear empty states and helpful error messages

**Color/Spacing mapping to StardewUI:** See Section 4 for spacing token mapping. For implementation patterns, consult [`ui-component-patterns.instructions.md`](ui-component-patterns.instructions.md).

## 14. Future Extensions ##

Possible later additions:

```text
- Alternate themes
- Custom icons
- Accessibility settings
- Enhanced notifications
```
