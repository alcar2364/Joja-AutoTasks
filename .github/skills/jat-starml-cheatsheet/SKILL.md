---
name: jat-starml-cheatsheet
description: "Quick StarML syntax and contract checklist for JAT. Use when: editing .sml tags, bindings, structural attributes, events, or template/include/outlet patterns."
argument-hint: "Target .sml file + intended layout or binding change"
---

<!-- markdownlint-disable -->

# JAT StarML Cheatsheet #

Use this skill for fast StarML correctness checks before and after `.sml` edits.

## When to Use ##

    * Editing layout containers and leaf views.
    * Adding or fixing data bindings.
    * Adding event handlers in StarML markup.
    * Updating templates, includes, outlets, or repeated rows.

## Procedure ##

1. Confirm you are using documented StardewUI tags.
2. Enforce kebab-case attributes and event names.
3. Put structural attributes first (`*if`, `*repeat`, `*switch`, `*case`, `*context`, `*float`, `*outlet`).
4. Use pipe syntax for events (`click=|Handler()|`), not quoted handlers.
5. Keep binding complexity readable; move logic to view models when needed.
6. Validate attribute ordering and container hierarchy readability.

## Quick Validation ##

    * No PascalCase attributes in `.sml`.
    * No quoted event handlers.
    * No invented tags or pseudo-XML containers.
    * Repeated rows are implemented with `*repeat`, not copy-paste blocks.

## Quick Reference ##

### Common Tags ###

    * Containers: `frame`, `lane`, `panel`, `grid`, `scrollable`
    * Display: `label`, `banner`, `image`, `digits`, `spacer`
    * Interactive: `button`, `checkbox`, `dropdown`, `slider`, `textinput`, `tab`, `expander`
    * Advanced: `template`, `outlet`, `include`

### Binding Forms ###

    * Literal: `text="Hello"`
    * Context: `text={Title}`
    * Asset: `sprite={@Mods/StardewUI/Sprites/MenuBackground}`
    * Translation: `text={#TaskMenu.Title}`
    * Template parameter: `text={&Title}`
    * Event handler: `click=|OnClick()|`

### Structural Attributes ###

    * `*if`, `*repeat`, `*switch`, `*case`, `*context`, `*float`, `*outlet`
    * Place structural attributes before behavior and visual attributes.
    * Prefer `*if` over `visibility` when the element should not exist.

### Event Rules ###

    * Event names are kebab-case.
    * Use pipe syntax for handlers.
    * Do not quote handlers.

### Attribute Ordering ###

1. Structural attributes
2. Behavior attributes
3. Layout attributes
4. Content/data attributes
5. Styling attributes
6. Events

## References ##

    * [SML Style Contract](../Contracts/SML-STYLE-CONTRACT.instructions.md)
    * [StarML Cheatsheet Instruction](../Instructions/starml-cheatsheet.instructions.md)
