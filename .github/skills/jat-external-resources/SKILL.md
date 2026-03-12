---
name: jat-external-resources
description: "Official source map for JAT modding APIs and integration libraries. Use when: implementing SMAPI APIs, StardewUI behavior, GMCM integration, or translation/I18n flows."
argument-hint: "Subsystem + question + target API or doc"
---

<!-- markdownlint-disable -->

# JAT External Resources #

Use this skill to ground implementation choices in official or maintained upstream docs.

## When to Use ##

    * You need SMAPI API behavior, event, lifecycle, or save/load documentation.
    * You need StardewUI StarML, binding, view, or menu framework details.
    * You are implementing or validating GMCM bootstrap integration.
    * You are clarifying translation boundaries for locale-neutral backend state.

## Procedure ##

1. Identify subsystem scope: backend, frontend, config, localization.
2. Use the source map section below to select references.
3. Prefer official docs first, then maintained source repositories.
4. Reconcile findings with JAT contracts before implementation.
5. Record the exact URL used when decisions depend on external behavior.

## Output Checklist ##

    * Source URL is explicit.
    * Version-sensitive assumptions are called out.
    * Contract boundary impacts are noted.

## Source Map ##

### Stardew Valley Modding ###

    * [Stardew Valley Wiki Modding Index](https://stardewvalleywiki.com/Modding:Index)
        * Use for API, events, content, and common modding flows.
    * [SMAPI Translation API Guide](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Translation)
        * Use for translation key and I18n usage expectations.
    * [Stardew Modding Wiki](https://stardewmodding.wiki.gg/)
        * Use as community reference when official docs are sparse.

### StardewUI ###

    * [StardewUI Documentation](https://focustense.github.io/StardewUI/)
        * Use for StarML syntax, views, bindings, and framework behavior.
    * [StardewUI Repository](https://github.com/focustense/StardewUI)
        * Use for implementation examples and behavior confirmation.

### SMAPI Source ###

    * [StardewModdingAPI Repository](https://github.com/Pathoschild/SMAPI)
        * Use for framework internals and edge-case behavior checks.

### Generic Mod Config Menu ###

    * [GMCM Repository (reference)](https://github.com/Generic-Mod-Config-Menu-Stardew-Valley)
    * [spacechase0 Stardew mods GMCM source](https://github.com/spacechase0/StardewValleyMods/tree/develop/GenericModConfigMenu)

## JAT Usage Guardrails ##

    * Prefer official docs over inferred behavior.
    * When docs conflict, prioritize official source then latest maintained version.
    * Keep backend canonical state locale-neutral; translate at UI boundary.
    * Treat GMCM as bootstrap surface when full settings live in StardewUI menus.

## References ##

    * [External Resources Instruction](../Instructions/external-resources.instructions.md)
    * [Backend Contract](../Contracts/BACKEND-ARCHITECTURE-CONTRACT.instructions.md)
    * [Frontend Contract](../Contracts/FRONTEND-ARCHITECTURE-CONTRACT.instructions.md)
