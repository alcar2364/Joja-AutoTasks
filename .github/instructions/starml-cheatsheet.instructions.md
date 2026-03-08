---
name: starml-cheatsheet
description: "Quick reference for StarML (.sml) syntax and patterns. Use when: editing StarML files or need quick syntax lookup without reading full SML-STYLE-CONTRACT."
---

# STARML QUICK REFERENCE — JAT #

This is a **fast reference sheet** for editing `.sml` (StarML) files used by **Joja AutoTasks
(JAT)**.

For the complete contract, see [`SML-STYLE-CONTRACT.instructions.md`](../Contracts/SML-STYLE-CONTRACT.instructions.md).

This cheatsheet summarizes the most important rules from the full contract.

Use this when writing or modifying UI without needing to read the full contract.

## 1. Core StarML Model ##

StarML is **view markup** for StardewUI.

Element structure:

```xml
<tag attribute="value">
```

children

```text

</tag>
```

Rules:

• Every tag corresponds to a **StardewUI view**  
• Attributes map to **view properties or events**  
• Children map to **child views**  
• Tags are **semantic**, not arbitrary containers

## 2. Common Tags (Safe to Use) ##

Containers

```text
- frame
- lane
- panel
- grid
- scrollable
```

Display

```text
- label
- banner
- image
- digits
- spacer
```

Interactive

```text
- button
- checkbox
- dropdown
- slider
- textinput
- tab
- expander
```

Advanced

```text
- template
- outlet
- include
```

## 3. Attribute Naming ##

StarML attributes are **kebab-case**.

Examples:

C# property → StarML attribute

```text
HorizontalContentAlignment → horizontal-content-alignment
LeftClick → left-click
PointerEventsEnabled → pointer-events-enabled
```

Never use PascalCase attributes in `.sml`.

## 4. Attribute Types ##

Literal value

```xml
text="Hello"
```

Context binding

```xml
text={Title}
```

Asset binding

```xml
sprite={@Mods/StardewUI/Sprites/MenuBackground}
```

Translation binding

```xml
text={#TaskMenu.Title}
```

Template parameter

```xml
text={&Title}
```

Event binding

```xml
click=|OnClick()|
```

## 5. Structural Attributes ##

Structural attributes control the view tree.

They always start with `*`.

Supported:

```xml
*if
*repeat
*switch
*case
*context
*float
*outlet
```

Examples

Conditional element:

<label text="Done!" *if={TaskCompleted} />

Repeating element:

<label text={Name} *repeat={Tasks} />

Switch / case:

```xml
<frame *switch={Status}>
```

<label text="Ready" *case="Ready"/>
<label text="Running"*case="Running"/>

```text

</frame>
```

Rules:

• Structural attributes appear **first** in attribute order  
• Prefer `*if` over `visibility` when the element should not exist

## 6. Behavior Attributes ##

Behavior attributes start with `+`.

Example:

<button +tooltip text="Build Coop"/>

Rules:

• Behaviors appear **after structural attributes**
• Do not invent behaviors

## 7. Event Binding ##

Events use **pipe syntax**.

`click=|Handler()|`

Common events

```xml
click
left-click
right-click
pointer-enter
pointer-leave
pointer-move
drag
drag-start
drag-end
wheel
button-press
```

Never quote event handlers.

Wrong

`click="OnClick()"`

Correct

`click=|OnClick()|`

## 8. Binding Modifiers ##

Modifiers control binding behavior.

Parent context

`{^Prop}`

Ancestor context

`{~Type.Prop}`

One-time binding

`{:Prop}`

Output binding

`{>Prop}`

Two-way binding

`{<>Prop}`

Order rule:

```txt
direction → context
```

Valid

```xml
{^^<>Value}
{>~ViewModel.Prop}
```

Invalid

<{^^<>Value}>

## 9. Self Closing vs Container Tags ##

Leaf views:

`<label text="Hello" />`

Containers:

```xml
<lane>
<label text="A"/>
<label text="B"/>
</lane>
```

Rules:

• Leaf views should be **self-closing**
• Layout containers should use **open/close tags**

## 10. Attribute Ordering ##

Preferred order:

1. structural (`*if`, `*repeat`, etc)
2. behavior (`+behavior`)
3. layout (`layout`, alignment, margin)
4. content (`text`, `items`, `sprite`)
5. styling (`font`, `background`, `opacity`)
6. events (`click`, `left-click`, etc)

## 11. Layout Containers ##

Use containers intentionally:

```txt
frame      → UI shell/background
lane       → vertical/horizontal flow
panel      → layered overlap
grid       → grid layouts
scrollable → scrolling regions
```

Avoid fake containers like:

`<container>`

Use real StardewUI views instead.

## 12. Includes and Templates ##

Include another StarML file:

`<include name="TaskRow" />`

Template definition (root only):

```xml
<template name="TaskRow">
<lane>
    <label text={Name}/>
</lane>
</template>
```

Rules:

• `template` appears at root level  
• `outlet` only works inside templates

## 13. Formatting Rules ##

Indentation

```txt
4 spaces
```

Use multiline attributes when complex

```xml
<frame layout="stretch content"
       background={@Mods/StardewUI/Sprites/MenuBackground}
       border={@Mods/StardewUI/Sprites/MenuBorder}>
```

Leaf example

```xml
<label text={Title}/>
```

Spacing rules

• no spaces around `=`  
• one space before `/>`

## 14. Common Pitfalls ##

Avoid:

• PascalCase attributes  
• quoted event handlers  
• double brace syntax `{{ }}`  
• random container tags  
• deep ancestor bindings like `{^^^^Value}`  
• unnecessary two-way bindings

## 15. JAT UI Structure Guideline ##

When building menus or HUDs, structure should resemble:

```text

frame
 └ lane (vertical)

```

├ header / date
├ task list (scrollable)
├ selected task details
└ footer / controls

```text

```

Clear hierarchy beats clever markup.

## 16. The Simple Mental Rule ##

StarML is **view composition**.

Think:

```text

Views
 → properties
 → behaviors
 → events
 → child views

```

Not:

```text

XML document formatting

```

Write markup that clearly mirrors the UI hierarchy.

---

## 17. Next Steps ##

For composition patterns and layout best practices, see [`ui-component-patterns.instructions.md`](ui-component-patterns.instructions.md).
