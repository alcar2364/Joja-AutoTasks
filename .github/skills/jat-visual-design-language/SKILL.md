---
name: jat-visual-design-language
Description: JAT visual language tokens and UX guardrails. Use when: choosing color accents, spacing scale, typography emphasis, animation style, or accessibility constraints for HUD/menu UI.
argument-hint: "Surface + visual objective + constraints"
---

<!-- markdownlint-disable -->

# JAT Visual Design Language #

Use this skill to keep UI visuals cohesive with Stardew-native readability plus JAT's Joja accent.

## When to Use ##

    * Defining or applying color accents and status indicators.
    * Applying spacing and hierarchy tokens.
    * Choosing animation style and transition limits.
    * Verifying accessibility and input target constraints.

## Procedure ##

1. Start from the tokens and rules section below.
2. Select tokens before implementing ad-hoc values.
3. Keep Joja color usage as accent, not full UI saturation.
4. Validate readability and contrast for key text and controls.
5. Confirm status communication uses text/icons, not color alone.

## Guardrails ##

    * Keep animations short and functional.
    * Keep HUD readable at small scale.
    * Preserve consistent spacing rhythm using token increments.
    * Avoid clutter and decorative noise that obscures task state.

## Tokens and Rules ##

### Visual Philosophy ###

    * Stardew-native familiarity: pixel readability, clear hierarchy, simple framing.
    * Joja productivity accent: restrained blue/white emphasis for structure and status.

### Accent Palette ###

    * Primary accent: Joja Blue `#2C6BE0`
    * Light accent: Joja Light Blue `#6EA6FF`
    * Highlight accent: Soft Blue `#A8C8FF`

Use accent colors intentionally for focus and status, not everywhere.

### Spacing Scale ###

    * XS: 4px
    * SM: 8px
    * MD: 12px
    * LG: 16px
    * XL: 24px

Use 4px-step spacing for consistent rhythm.

### Typography Intent ###

    * HUD task text: small and scannable
    * Menu task list: medium
    * Section headings and wizard titles: large
    * Metadata: small but readable

### Motion Limits ###

    * Completion feedback: 200-300ms
    * HUD expand/collapse: 150-200ms
    * Step transitions: around 200ms

Animations should clarify state changes, not distract.

### Accessibility Baselines ###

    * Primary text contrast target: 4.5:1
    * Large heading contrast target: 3:1
    * UI control contrast target: 3:1
    * Do not encode status by color alone.
    * Keep keyboard focus indicators obvious.
    * Minimum interaction target size: 32x32 where practical.

## References ##

    * [Visual Design Language Instruction](../Instructions/visual-design-language.instructions.md)
    * [UI Component Patterns Instruction](../Instructions/ui-component-patterns.instructions.md)
