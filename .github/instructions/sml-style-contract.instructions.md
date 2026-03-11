---
name: sml-style-contract
description: "StarML/SML coding style and authoring rules for StardewUI markup in JAT. Use when: editing .sml files."
applyTo: "**/*.sml"
---

# SML-STYLE-CONTRACT.instructions.md #

## Purpose ##

This contract defines **StarML / SML coding style and authoring rules** for **JAT (Joja
AutoTasks)**.

This file is intentionally **specific to StardewUI StarML** so agents do **not** need to search
external docs for ordinary StarML conventions while editing JAT `.sml` files.

Applies to:

```text
- `*.sml`
```

StarML is the StardewUI markup language. It is **HTML-like**, built from a tree of **elements**,
where each element corresponds to a StardewUI **view**. Attributes map to view properties or events,
except for structural attributes, and child elements map to child views. Tags are **not arbitrary**;
except for `<include>`, the tag determines the view type, which determines valid attributes and
child limits. StardewUI also uses **kebab-case** for StarML attribute and event names.

This contract does **not** define:

```text
- C# style (see CSHARP-STYLE-CONTRACT.instructions.md)
- JSON style (see JSON-STYLE-CONTRACT.instructions.md)
- workspace behavior (see WORKSPACE-CONTRACTS.instructions.md)
- backend/frontend architecture boundaries (see BACKEND-ARCHITECTURE-CONTRACT.instructions.md, FRONTEND-ARCHITECTURE-CONTRACT.instructions.md)
- UI component patterns (see ../Instructions/UI-COMPONENT-PATTERNS.instructions.md)
```

## 1. StarML source-of-truth rules ##

Agents MUST follow this precedence order:

1. This contract
2. Existing consistent conventions already present in the touched file
3. StardewUI / StarML documented syntax and semantics
4. Standard XML conventions as fallback only when StarML-specific guidance is absent

Agents MUST treat `.sml` as **StarML first**, not as generic XML with funny hats.

## 2. Canonical StarML mental model ##

### 2.1 Elements ###

A StarML element consists of:

```text
- a tag
- zero or more attributes
- zero or more children
```

### 2.2 View mapping ###

Every StarML element corresponds to a StardewUI **view**.

### 2.3 Tag validity ###

Tags are semantic and type-defining, not arbitrary container labels. Except for `<include>`, the tag
selects the specific view type, which controls:

```text
- what attributes are legal
- whether children are legal
- how many children are legal
```

### 2.4 Open/close vs self-closing ###

StarML supports both:

```text
- opening + closing tags
- self-closing tags
```

Any element may use either form, but self-closing tags cannot have children. In practice, simple
leaf views tend to be self-closing and layout/container views tend to use opening/closing tags.
îˆ€citeîˆ‚turn0view0îˆ‚turn2view3îˆ

### 2.5 JAT rule for tag form ###

Use:

```text
- **self-closing tags** for leaf/simple views with no children
- **opening/closing tags** for container/layout views and any element with children
```

Do not convert forms unless the edit requires it or the current form harms readability.

## 3. Supported baseline StarML tags agents may use in JAT ##

These standard tags are explicitly recognized by this contract, based on StardewUI's documented
built-in tags:
`banner`, `button`, `checkbox`, `digits`, `dropdown`, `expander`, `frame`, `grid`, `image`,
`include`, `keybind`, `keybind-editor`, `label`, `lane`, `marquee`, `outlet`, `panel`, `scrollable`,
`slider`, `spacer`, `tab`, `template`, `textinput`. îˆ€citeîˆ‚turn2view3îˆ

### 3.1 Preferred JAT usage by intent ###

Use these tags with the following intent:

```text
- `frame` for bordered/background shell containers
- `lane` for one-axis layout flows
- `panel` for layered composition / z-indexed overlap
- `grid` for repeated tile-like or tabular layouts
- `scrollable` for content that may exceed available space
- `label` for ordinary text
- `banner` for title/header text with banner treatment
- `image` for sprite or asset display
- `button`, `checkbox`, `dropdown`, `slider`, `textinput`, `tab`, `expander` for interaction
- `spacer` for layout push/separation, not dummy visual content
- `include` for deliberate reuse
- `template` and `outlet` only when templating is actually needed
```

### 3.2 JAT anti-slop rule ###

Agents MUST NOT invent pseudo-tags or generic XML-style semantic wrappers if a documented StardewUI
view tag already fits the job.

Bad:

```xml
<container>
```

Better:

```xml
<frame>
<lane>
<panel>
<scrollable>
```

## 4. Attribute naming rules ##

### 4.1 Kebab-case is mandatory ###

In StarML, attribute names are the **kebab-case** form of the underlying property or event name.

Examples:

```text
- `HorizontalContentAlignment` -> `horizontal-content-alignment`
- `LeftClick` -> `left-click`
```

Agents MUST use kebab-case for:

```text
- ordinary properties
- common attributes
- event attributes
- view-specific attributes
```

Agents MUST NOT write C#-style PascalCase attribute names in `.sml`.
îˆ€citeîˆ‚turn1view1îˆ‚turn2view3îˆ

### 4.2 JAT attribute ordering ###

Preferred attribute order in JAT:

1. structural attributes (`*if`, `*context`, `*repeat`, `*switch`, `*case`, `*float`, `*outlet`)
2. behavior attributes (`+...`)
3. core layout attributes (`layout`, orientation, alignment, margin, padding)
4. content/data attributes (`text`, `items`, `name`, `source`, `sprite`, etc.)
5. styling/display attributes (`font`, `background`, `border`, `opacity`, `visibility`, `z-index`)
6. interaction/event attributes
7. tooltip/miscellaneous attributes

When a file already uses a clear, consistent local order, preserve it unless there is a strong
reason to normalize.

## 5. Common StarML attributes agents should know without looking them up ##

The following common attributes are documented as applying broadly across views:  
`actual-bounds`, `border-size`, `content-bounds`, `content-size`, `draggable`, `focusable`,
`inner-size`, `layout`, `margin`, `name`, `opacity`, `outer-size`, `padding`,
`pointer-events-enabled`, `scroll-with-children`, `tooltip`, `transform`, `transform-origin`,
`visibility`, `z-index`. îˆ€citeîˆ‚turn0view0îˆ

### 5.1 JAT conventions for common attributes ###

```text
- Prefer `layout` explicitly on important containers when it materially affects readability or
behavior.
- Use `margin` and `padding` deliberately; do not stack random spacing like cursed lasagna.
- Use `name` only for meaningful diagnostics/troubleshooting or where required by include semantics.
- Use `visibility` only when the view should remain in layout while hidden; use `*if` when the
element should not exist at all. StardewUI explicitly distinguishes these. îˆ€citeîˆ‚turn0view0îˆ
- Use `z-index` only when layering matters, typically inside `panel`.
- Use `pointer-events-enabled="false"` when drawing non-interactive overlays above interactive
content.
```

### 5.2 Out-only properties ###

Some common attributes are output-oriented and write data back from the view to the model. Out
properties cannot be assigned literal values or bound without the `>` direction modifier.
îˆ€citeîˆ‚turn2view0îˆ

JAT rule:

```text
- Never assign literal values to out-only properties.
- If binding an out-only property, use the correct output-style binding form.
```

## 6. Structural attributes: exact JAT rules ##

Structural attributes begin with `*` and control how the view tree is constructed rather than
setting a view property. StardewUI documents these structural attributes: `*case`, `*context`,
`*if`, `*float`, `*outlet`, `*repeat`, `*switch`. îˆ€citeîˆ‚turn2view0îˆ

### 6.1 `*if` ###

Use `*if` when the element should be removed from the tree unless a condition is met.

JAT rule:

```text
- Prefer `*if` over `visibility` when absence from layout is the real intent.
```

### 6.2 `*!if` ###

Negation is valid for certain conditional structural attributes. Example: `*!if={Condition}`.
îˆ€citeîˆ‚turn2view0îˆ

JAT rule:

```text
- `*!if` is allowed, but do not nest double-negative nonsense. If readability suffers, move the
logic into the view model/context.
```

### 6.3 `*switch` and `*case` ###

Use `*switch` on the controlling branch context and `*case` on subsequent matching elements. `*case`
compares against the most recent `*switch`. îˆ€citeîˆ‚turn2view0îˆ

JAT rules:

```text
- Keep related `*switch` / `*case` blocks visually adjacent.
- Use this for mutually exclusive UI branches rather than stacking many sibling `*if`s when only one
branch should show.
- `*!case` is allowed where negation improves clarity, but use sparingly.
```

### 6.4 `*repeat` ###

`*repeat` repeats the element over a collection, creates a new view for each item, and sets the
context to that item. StardewUI explicitly notes that if `*repeat` and `*if` are both present,
`*repeat` applies first. îˆ€citeîˆ‚turn2view0îˆ‚turn1view5îˆ

JAT rules:

```text
- Put `*repeat` first in the structural attribute group.
- Do not mix `*repeat` with messy deeply nested conditionals unless the structure is still readable.
- For repeated task rows, history rows, or stats rows, prefer one clean repeated row element over
copy-pasted sibling markup.
```

### 6.5 `*context` ###

Use `*context` to intentionally change the binding context for child nodes.

JAT rules:

```text
- Use `*context` only when it clearly improves readability or avoids repeated ancestor bindings.
- Do not stack many nested `*context` levels without section comments in complex files.
```

### 6.6 `*float` ###

Use `*float` for floating-position behavior where the view is intended to float relative to
layout/container semantics.

JAT rule:

```text
- Reserve `*float` for actual floating UI behavior, not as a random fix for bad container structure.
```

### 6.7 `*outlet` ###

`*outlet` routes a node into one of the parent node's outlets, and its value must be a quoted
string; it does not support bindings. îˆ€citeîˆ‚turn2view0îˆ

JAT rules:

```text
- Never bind `*outlet`.
- Keep outlet-targeted children grouped together when possible.
- Use explicit outlet names only when the template/container actually defines meaningful outlets.
```

## 7. Behavior attributes: exact JAT rules ##

Behavior attributes begin with `+` and refer to StardewUI behaviors, which are independent entities
acting on a view. StardewUI notes that the behavior system is extensible and may include arguments
after a `:` in the attribute name. îˆ€citeîˆ‚turn2view0îˆ

JAT rules:

```text
- Group behavior attributes immediately after structural attributes.
- Use behaviors only when the behavior model is the correct StardewUI solution, not as a substitute
for sensible tree structure.
- Keep behavior usage explicit and readable. If many behavior attributes crowd a line, switch to
one-attribute-per-line formatting.
- Do not invent fake behavior names.
```

## 8. Event attributes: exact JAT rules ##

Event attributes use kebab-case names and bind handlers using pipe syntax:

```xml
<button click=|OnClick()| />
```

Documented common events include:  
`button-press`, `click`, `drag`, `drag-end`, `drag-start`, `left-click`, `pointer-enter`,
`pointer-leave`, `pointer-move`, `right-click`, `wheel`. îˆ€citeîˆ‚turn1view1îˆ

### 8.1 JAT event rules ###

```text
- Use event bindings only on interactive UI.
- Prefer the most specific event that matches intent.
- Do not bind both `click` and `left-click` to overlapping logic unless there is a real reason.
- Keep event handlers short and obvious at the markup level.
```

### 8.2 Event syntax ###

Event bindings use pipe-delimited handler syntax, not quoted strings. Example:

```xml
<image click=|PlantCrops("corn", ^Quantity, $Button)| />
```

îˆ€citeîˆ‚turn1view1îˆ

JAT rule:

```text
- Never write event handlers as quoted text.
- Never replace event bindings with pseudo-XML conventions from other UI frameworks.
```

## 9. Attribute flavors: exact JAT rules ##

StardewUI documents these primary attribute flavors:  

```text
- `attr="value"` for literal converted values  
- `attr={PropertyName}` for context-property binding  
- `attr={@AssetName}` for asset binding  
- `attr={#TranslationKey}` for translation binding  
- `attr={&templateParam}` for template-attribute replacement inside `<template>`  
- `attr=|Handler(...)|` for event bindings îˆ€citeîˆ‚turn2view1îˆ‚turn2view2îˆ
```

### 9.1 Literal values ###

Use quoted literals for actual literal values.

### 9.2 Context bindings ###

Use `{PropertyName}` for normal context binding.

### 9.3 Asset bindings ###

Use `{@AssetName}` for assets/sprites.

JAT rule:

```text
- Prefer explicit asset references for menu backgrounds, borders, and sprites instead of hiding them
in mysterious indirection unless the file already follows an abstraction pattern.
```

### 9.4 Translation bindings ###

Use `{#TranslationKey}` for localized text.

JAT rule:

```text
- Prefer translation bindings for user-facing reusable text instead of hard-coded literals when the
text belongs in i18n resources.
```

### 9.5 Template attribute replacement ###

Use `{&templateParam}` only inside `<template>`.

### 9.6 Double braces ###

StardewUI allows `{{` and `}}` in place of single braces for people migrating from Content Patcher
habits, but explicitly says they are not recommended due to inconsistency and reduced readability.
îˆ€citeîˆ‚turn2view1îˆ

JAT rule:

```text
- Use **single-brace** StarML forms by default.
- Do not introduce double-brace forms in JAT unless editing a file that already consistently uses
them and the user asked to preserve local style.
```

### 9.7 No fake token syntax ###

StardewUI explicitly warns that StarML bindings are not string-replacement tokens, and patterns like
`attr="A {{value}} B"` are not the right mental model. îˆ€citeîˆ‚turn2view1îˆ

JAT rule:

```text
- Do not write Content Patcher-style string interpolation inside quoted literals.
```

## 10. Binding modifiers: exact JAT rules ##

StardewUI documents these binding modifiers for context bindings:  

```text
- `^` parent context  
- `~` typed ancestor  
- `<` input  
- `:` or `<:` one-time input  
- `>` output  
- `<>` in/out two-way  
```

Direction modifiers must come **before** context modifiers, e.g. `{<>^^Prop}` and `{>~Foo.Prop}` are
valid, while `{^^<>Prop}` and `{~>Foo.Prop}` are invalid. îˆ€citeîˆ‚turn2view2îˆ

### 10.1 Parent context `^` ###

Use `^` when binding to a parent context property. Multiple `^` may be chained.

JAT rule:

```text
- One `^` or two is fine when it improves clarity.
- If bindings start looking like ladder-climbing spelunking, prefer `*context` or better local
structure.
```

### 10.2 Typed ancestor `~` ###

Use `~` when binding to a typed ancestor context.

JAT rule:

```text
- Use this deliberately; do not reach through half the tree because the local context model is
messy.
```

### 10.3 Direction modifiers ###

```text
- `<` input-only
- `:` or `<:` one-time input
- `>` output-only
- `<>` two-way
```

JAT rules:

```text
- Omit `<` when ordinary one-way input is obvious; StardewUI says that is the default.
îˆ€citeîˆ‚turn2view2îˆ
- Use `:` or `<:` for values intended to initialize once and then stay disconnected.
- Use `>` only when the view should write back but not read.
- Use `<>` only when true two-way binding is actually intended. Do not scatter two-way bindings like
confetti.
```

### 10.4 Modifier order ###

Always put direction before context:

```text
- valid: `{<>^^Prop}`, `{>~Foo.Prop}`
- invalid: `{^^<>Prop}`, `{~>Foo.Prop}` îˆ€citeîˆ‚turn2view2îˆ
```

## 11. Templates, outlets, and includes ##

### 11.1 `<template>` ###

`<template>` defines a custom tag for replacement and must be at the document root; it is not valid
as a child of ordinary elements. `<outlet>` is only valid within a `<template>`.
îˆ€citeîˆ‚turn2view3îˆ

JAT rules:

```text
- Keep templates near the top/root where expected by the file structure.
- Do not bury templates inside ordinary view trees.
- Use template names and parameters that are stable and descriptive.
```

### 11.2 `<outlet>` ###

Use only inside templates.

### 11.3 `<include>` ###

`<include>` inserts another StarML view using its asset `name`. îˆ€citeîˆ‚turn2view3îˆ

JAT rules:

```text
- Use includes for meaningful reuse, not to fracture a readable file into tiny scraps.
- Include names must be stable, descriptive, and project-meaningful.
- Prefer includes for repeated large sections such as reusable task row templates, settings
sections, or statistics panels when repetition would otherwise become sludge.
```

## 12. Child and container rules ##

### 12.1 Respect child limits ###

Because StarML tags map directly to view types, child legality is determined by the underlying view.
StardewUI explicitly says the framework will not allow impossible view relationships, such as adding
children to a non-layout view. îˆ€citeîˆ‚turn0view0îˆ

JAT rule:

```text
- Do not add children to obvious leaf views like `label` or `image`.
- Use layout/container tags for structural composition.
```

### 12.2 Container intent ###

Use:

```text
- `lane` for row/column flow
- `panel` for overlap/layering
- `frame` for shelling a child or region
- `scrollable` when overflow is expected
- `grid` when repeated content needs regular grid layout
```

### 12.3 Tree readability ###

The markup should make the UI hierarchy obvious at a glance.

For JAT menu and HUD work, keep major sections visually distinct:

```text
- header/date/navigation
- task list
- selected task details
- history/statistics areas
- footer/actions/debug only when applicable
```

## 13. Formatting rules for JAT `.sml` ##

### 13.1 Indentation ###

```text
- Use **4 spaces** per indentation level.
- Do not use tabs.
```

### 13.2 Single-line vs multi-line elements ###

Single-line is fine for short leaf elements:

```xml
<label text={Title} />
```

Use multi-line formatting when:

```text
- the element has many attributes
- any attribute uses a long binding
- the element uses behaviors or multiple structural attributes
- readability clearly improves
```

Preferred multi-line form:

```xml
<frame layout="stretch content"
       background={@Mods/StardewUI/Sprites/MenuBackground}
       border={@Mods/StardewUI/Sprites/MenuBorder}>
<lane orientation="vertical">
    <label text={Title} />
</lane>
</frame>
```

### 13.3 Attribute-per-line threshold ###

If an element has:

```text
- more than 3 short attributes, or
- any combination of structural + behavior + event bindings, or
- a long asset/translation/binding expression
```

then agents SHOULD switch to one attribute per line after the tag name.

### 13.4 Quotes ###

```text
- Use double quotes for literal quoted attributes.
- Do not quote binding expressions.
```

### 13.5 Spacing ###

```text
- No spaces around `=`
- One space before `/>`
- No decorative alignment games beyond consistent multiline indentation
```

## 14. Comment rules ##

StarML supports HTML-style block comments. They are visible in the source file to users who open the
`.sml`, though they are ignored after parsing. îˆ€citeîˆ‚turn1view4îˆ

JAT comment rules:

```text
- Use comments sparingly.
- Use comments for major sections, non-obvious layout hacks, outlet/template wiring, or unusual
binding assumptions.
- Do not narrate obvious markup.
- Do not leave dead commented-out UI blocks.
```

Use:

```xml
<!-- Section: Task History -->
```

Do not use malformed comment syntax. The docs show HTML-style block comments as the correct form.
îˆ€citeîˆ‚turn1view4îˆ

## 15. Minimal-diff edit behavior ##

### 15.1 Preserve local consistency ###

If a file already has a strong, readable local convention that does not conflict with this contract,
preserve it.

### 15.2 Normalize touched regions ###

If already editing a region, agents SHOULD normalize that region toward this contract:

```text
- kebab-case attributes
- cleaner structural/behavior/content ordering
- consistent self-closing vs open/close usage
- clearer multiline formatting
```

### 15.3 Avoid drive-by cleanup ###

Do not reformat entire files just because the markup offends aesthetic sensibilities.

### 15.4 Stable names only ###

Do not invent random IDs, random include names, or vague names like `Panel1`, `Thing`, `ContainerA`.

## 16. XML fallback rules ##

When this contract and StarML-specific semantics do not address a formatting question, fall back to
ordinary XML discipline:

```text
- well-formed nesting
- matching open/close tags
- proper escaping where required
- consistent indentation
- double-quoted literal attributes
```

But XML is fallback only. Agents MUST NOT:

```text
- rewrite StarML bindings as quoted XML strings
- replace StarML event syntax with another UI framework's syntax
- import XAML/HTML habits that contradict StarML semantics
```

## 17. JAT quick reference summary ##

Agents should be able to work from this cheat sheet without looking elsewhere:

```text
- Tags map to StardewUI views.
- Tags are semantic, not arbitrary.
- Attributes and events are **kebab-case**.
- Structural attributes start with `*`.
- Behavior attributes start with `+`.
- Event handlers use pipe syntax: `click=|Handler()|`.
- Binding flavors:
    * literal: `"value"`
    * context: `{Prop}`
    * asset: `{@Asset}`
    * translation: `{#Key}`
    * template attr: `{&Param}`
- Binding modifiers:
    * `^` parent
    * `~` typed ancestor
    * `<` input
    * `:` / `<:` one-time
    * `>` output
    * `<>` two-way
- Direction modifiers come before context modifiers.
- Use `*if` to remove from layout/tree; use `visibility` to hide but keep layout.
- `*outlet` must be a quoted string, not a binding.
- `<template>` is root-level; `<outlet>` is template-only.
- Use single braces, not double braces, in new JAT code.
- Use XML rules only as fallback.
```

The goal is valid, readable StarML that matches StardewUI semantics closely enough that the agent
does not have to go rummaging through docs like a raccoon in a syntax dumpster.
