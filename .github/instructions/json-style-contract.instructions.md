---
name: json-style-contract
description: "JSON formatting, key naming, and versioning conventions for JAT. Use when: editing .json files."
applyTo: "**/*.json"
---

# JSON-STYLE-CONTRACT.instructions.md #

## Purpose ##

This contract defines JSON conventions for JAT.

Applies to:
    - `*.json`

Where JSON conventions conflict with C# style, JSON conventions win.

## 1. Agent edit behavior (JSON files) ##

    - Agents MUST avoid reformatting unrelated JSON files.
    - Agents MUST preserve ordering unless this contract requires a specific ordering.
    - Changes MUST be minimal and scoped.

## 2. Formatting ##

    - Indentation: 2 spaces (preferred) unless the repository already uses 4 spaces consistently in
    JSON.
    - No trailing commas.
    - Always include a trailing newline at end of file.

## 3. Key naming ##

    - JSON property names MUST match the corresponding C# property names exactly when they represent
    config/serialized models.
    - Use PascalCase keys if the matching C# property is PascalCase.
    - Do not invent alternate casing systems.

## 4. Key ordering (recommended, not strict) ##

When creating or significantly editing a JSON schema/config object, prefer this ordering:

1. `Version` / `SchemaVersion` (if present)
2. High-level metadata fields (e.g., `Enabled`)
3. Primary configuration sections
4. Advanced/optional sections
5. Debug sections last

## 5. Versioning ##

If a JSON file represents persisted or user-facing config/state:
    - it SHOULD include a version field
    - version migrations MUST be handled by backend migration logic (see [`BACKEND-ARCHITECTURE-CONTRACT.instructions.md`](BACKEND-ARCHITECTURE-CONTRACT.instructions.md))

## 6. Localization JSON Carveout (I18n assets) ##

This contract distinguishes model-shaped JSON from translation JSON.

For model-shaped JSON (config/save/schema), Section 3 naming rules apply.

For localization JSON assets consumed by SMAPI `I18n`:
    - keys MAY use localization-oriented naming conventions (for example dotted or scoped tokens)
    that do not mirror C# property names
    - values are localized display strings and are not canonical model fields
    - placeholder tokens SHOULD remain stable across locales for consistent formatting

Do not force C# model-key casing rules onto translation files.
