---
name: external-resources
description: "External documentation and reference sources for JAT development. Use when: implementing SMAPI features, UI framework integration, or consulting official modding/library documentation."
---

# External Resources — JAT

This file lists the **external documentation and reference sources** relevant to developing Joja
AutoTasks (JAT).

Agents SHOULD consult these resources when working on features that involve Stardew Valley modding
APIs, UI frameworks, or integration libraries.

## 1. Stardew Valley Modding

### Stardew Valley Wiki — Modding Index

Primary modding reference maintained by the community. Covers SMAPI APIs, content packs, game state
queries, events, and more.

    - <https://stardewvalleywiki.com/Modding:Index>

📘 **Referenced by:** [`BACKEND-ARCHITECTURE-CONTRACT.instructions.md`](../Contracts/BACKEND-ARCHITECTURE-CONTRACT.instructions.md) for event handling and game state access patterns.

### SMAPI Translation API

Primary reference for translation handling and `I18n` usage.

    - <https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Translation>

### Stardew Modding Wiki

Community wiki focused on modding tools, techniques, and best practices.

    - <https://stardewmodding.wiki.gg/>

## 2. StardewUI (UI Framework)

JAT uses StardewUI as its primary UI framework for both HUD and menu surfaces.

### StardewUI Documentation

Official documentation covering StarML, binding context, view models, standard views, overlays,
focus and interaction, and framework extensions.

    - <https://focustense.github.io/StardewUI/>

📘 **Referenced by:** [`FRONTEND-ARCHITECTURE-CONTRACT.instructions.md`](../Contracts/FRONTEND-ARCHITECTURE-CONTRACT.instructions.md), [`SML-STYLE-CONTRACT.instructions.md`](../Contracts/SML-STYLE-CONTRACT.instructions.md), and UI agents for view composition, binding patterns, and StarML syntax.

### StardewUI Repository

Source code, examples, and test mod for reference implementations.

    - <https://github.com/focustense/StardewUI>

### StardewModdingAPI Repository

SMAPI source code, API documentation, and framework internals.

    - <https://github.com/Pathoschild/SMAPI>

## 3. Generic Mod Config Menu (GMCM)

JAT uses GMCM as a minimal bootstrap configuration surface. Full configuration is handled by the
StardewUI configuration menu.

### GMCM Nexus / Documentation

    - <https://github.com/Generic-Mod-Config-Menu-Stardew-Valley>

### GMCM Source (spacechase0)

    - <https://github.com/spacechase0/StardewValleyMods/tree/develop/GenericModConfigMenu>

## 4. Usage Guidelines

### General Principles

    - Prefer official documentation over inferred behavior.
    - If documentation is unclear, note the ambiguity explicitly.
    - When cross-referencing resources, link to specific pages/sections where possible.
    - When documentation conflicts, prioritize: (1) official source, (2) latest version,
      (3) community consensus.

### Per-Agent Guidance

**GameAgent / Backend Implementation:** - Consult Stardew Valley Wiki Modding Index for lifecycle, event handling, and game-state APIs. - Consult SMAPI GitHub for framework internals and edge-case behavior. - Follow [`BACKEND-ARCHITECTURE-CONTRACT.instructions.md`](../Contracts/BACKEND-ARCHITECTURE-CONTRACT.instructions.md)
for JAT-specific constraints.

**UIAgent / Frontend Implementation:** - Consult StardewUI docs for bindings, view composition, and StarML syntax. - Consult StardewUI GitHub examples for implementation patterns. - Follow [`FRONTEND-ARCHITECTURE-CONTRACT.instructions.md`](../Contracts/FRONTEND-ARCHITECTURE-CONTRACT.instructions.md)
and [`SML-STYLE-CONTRACT.instructions.md`](../Contracts/SML-STYLE-CONTRACT.instructions.md).

**Localization / Translation Work:** - Consult SMAPI Translation API guide for `I18n` usage. - Keep translation resolution at UI boundaries/view models. - Preserve locale-neutral canonical backend state.

**GMCM Integration:** - Use GMCM as bootstrap config surface only. - Keep advanced configuration in the StardewUI configuration menu.

**StarMLAgent / Markup Editing:** - Consult StardewUI docs for valid view types, attributes, and binding/event syntax. - Use [`starml-cheatsheet.instructions.md`](starml-cheatsheet.instructions.md) for quick syntax lookup. - Use [`ui-component-patterns.instructions.md`](ui-component-patterns.instructions.md) for composition patterns.
