---
name: smapi-manifest-validator
description: "Validates manifest.json integrity and version consistency against JojaAutoTasks.csproj on every relevant change."
on:
  pull_request:
    paths:
      - "manifest.json"
      - "JojaAutoTasks.csproj"
    types: [opened, synchronize, reopened]
  push:
    branches: [development]
    paths:
      - "manifest.json"
      - "JojaAutoTasks.csproj"
  workflow_dispatch:
permissions:
  contents: read
  pull-requests: read
  checks: read
  issues: read
strict: true
network:
  allowed: [defaults, github]
engine:
  id: copilot
tools:
  github:
    toolsets: [default]
safe-outputs:
  noop:
    report-as-issue: false
  create-issue:
    title-prefix: "[manifest] "
    labels: [agentic-workflow, smapi, needs-review]
    max: 1
---

# SMAPI Manifest Validator — Mod Metadata Integrity Checker

Validate `manifest.json` for correctness and consistency with the `.csproj` version declaration
every time either file changes. Prevents mod boot failures caused by malformed manifest metadata.

## Context

- Repository: `${{ github.repository }}`
- Manifest file: `manifest.json`
- Project file: `JojaAutoTasks.csproj`
- SMAPI minimum supported API: `4.4.0`
- SMAPI documentation: https://stardewvalleywiki.com/Modding:Modder_Guide/APIs

## Validation Rules

### Rule 1: Required Fields Present

All of the following fields must exist and be non-empty in `manifest.json`:

- `Name` — Non-empty string
- `Author` — Non-empty string
- `Version` — Valid semantic version string (e.g., `0.1.0`, `1.0.0-beta.1`)
- `Description` — Non-empty string
- `UniqueID` — Follows `Author.ModName` convention (no spaces, uppercase first letter per segment)
- `EntryDll` — Matches the expected output DLL name (`JojaAutoTasks.dll`)
- `MinimumApiVersion` — Valid semantic version string

### Rule 2: No Unresolved Placeholder

**Policy:** `%ProjectVersion%` is the **official SMAPI `ModBuildConfig` substitution token**.
This repository intentionally uses it in `manifest.json`. The token is replaced at build time by
the `<Version>` value in `JojaAutoTasks.csproj` via the `Pathoschild.Stardew.ModBuildConfig`
NuGet package. **Do not flag `%ProjectVersion%` as an error.**

Only flag placeholder values that are **not** the recognized `%ProjectVersion%` token
(e.g., `%UNSET%`, `TODO`, an empty string, or any other unexpected `%..%`-style token).

### Rule 3: MinimumApiVersion Constraint

`MinimumApiVersion` must be:

- A valid semantic version parseable by semver rules
- Greater than or equal to `4.4.0` (the declared minimum for this mod)
- Not higher than the latest released SMAPI version (advisory warning, not block)

### Rule 4: Version Consistency with .csproj

If `manifest.json` `Version` is `%ProjectVersion%`, this rule **passes automatically** — the
token is the official SMAPI `ModBuildConfig` build-time substitution pattern, and the
`JojaAutoTasks.csproj` `<Version>` value is the single authoritative version source. No mismatch
is reported when the placeholder is in use.

If `manifest.json` contains a **literal** version string (i.e., not `%ProjectVersion%`), it must
match the `<Version>` property in `JojaAutoTasks.csproj`. If they differ, a release build would
embed a mismatched version.

### Rule 5: UniqueID Format

`UniqueID` must:

- Contain exactly one `.` separator
- Not contain spaces, special characters, or path separators
- Follow `Author.ModName` pattern (e.g., `Alcar.JojaAutoTasks`)

### Rule 6: JSON Validity

The file must be valid JSON (no trailing commas, no comments, correct bracket matching).

## Output

### On PR (path trigger)

Post a PR check with:

- ✅ All rules pass
- ❌ List of failing rules with exact field names and values
- Annotate the manifest.json diff inline for any failures

### On Push to Main

Post a check run. If failures detected, create an issue.

### On Failure

Create issue with:

- Which rules failed
- The actual field values that failed validation
- Instructions for fixing each failure
- Reference to SMAPI manifest documentation

## Notes

- This validator runs only when `manifest.json` or `JojaAutoTasks.csproj` changes (path filter)
- Does not require .NET SDK or build environment — pure JSON and XML parsing
- Completes in under 10 seconds
- Version consistency check is especially important before tagging a release
- If `release-packaging` workflow is also configured, manifest validation runs first in that pipeline
- **`%ProjectVersion%` policy:** This repository intentionally commits `%ProjectVersion%` as the
  `Version` value in `manifest.json`. This is the official SMAPI `ModBuildConfig` build-time
  substitution pattern documented at https://stardewvalleywiki.com/Modding:Modder_Guide/APIs.
  Rules 2 and 4 treat `%ProjectVersion%` as a valid, expected token and do **not** report it as
  an error or a version mismatch. Only unknown or non-standard placeholder tokens are flagged.
