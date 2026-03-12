---
name: security-and-owasp
description: "Security rules for JAT: safe config loading, dependency hygiene, no hardcoded secrets, safe persistence. Use when: editing config/persistence code or adding dependencies."
---

# Secure Coding Guidelines — JAT

## Purpose

JAT is a local offline game mod. Most OWASP Top 10 web concerns (SQL injection, XSS, CSRF,
cookies) do not apply. These rules cover the security concerns that ARE relevant to a SMAPI mod.

## 1. No Hardcoded Secrets

- Do NOT hardcode API keys, tokens, or credentials in source files.
- If future features require credentials, load them from config or environment — never from
  committed code.

## 2. Safe Config Deserialization

- Always validate config files after loading (version field, expected types, value ranges).
- On version mismatch or unknown version, fall back to defaults — do NOT throw unhandled
  exceptions.
- Use SMAPI's `ReadConfig<T>` / `WriteConfig<T>` exclusively for config I/O.
- Do NOT deserialize arbitrary JSON from user-provided external paths without validation.

## 3. Safe Persistence Loading

- Persisted save data MUST include a version field (see `BACKEND-ARCHITECTURE-CONTRACT`).
- Validate version before applying data to state.
- On unknown or future version, degrade gracefully — skip, reset, or prompt. Do NOT corrupt
  existing state.
- Do NOT reconstruct state from externally-modified save files without defensive validation.

## 4. Dependency Hygiene

- Keep NuGet packages (Pathoschild.Stardew.ModBuildConfig, xUnit, Moq, etc.) reasonably up
  to date.
- Review release notes for security advisories when upgrading dependencies.
- Do NOT add dependencies that pull in unmaintained or vulnerable transitive packages without
  justification.

## 5. Path Safety

- When loading or saving files, use absolute paths resolved through SMAPI's helper
  (`Helper.DirectoryPath`), not user-provided relative paths.
- Do NOT accept file paths from user input without sanitization.

## 6. Logging Safety

- Do NOT log sensitive user data or internal game state at Info/Warning level.
- Debug-level logging of internal state is acceptable behind `IsDebugMode` guards.
