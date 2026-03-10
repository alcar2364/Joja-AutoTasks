---
name: pr-ci
description: "PR-gated build pipeline: blocks merge on build failure. Testing must be performed manually by an admin before merge."
on:
  pull_request:
    paths:
      - "**.cs"
      - "**.csproj"
      - "**.sln"
      - "manifest.json"
    types: [opened, synchronize, reopened]
  push:
    branches: [main]
    paths:
      - "**.cs"
      - "**.csproj"
      - "**.sln"
      - "manifest.json"
  workflow_dispatch:
permissions:
  contents: read
  checks: read
  pull-requests: read
strict: true
network:
  allowed: [defaults, github]
engine:
  id: copilot
tools:
  github:
    toolsets: [default]
safe-outputs:
  create-issue:
    title-prefix: "[ci] "
    labels: [agentic-workflow, ci-health, needs-review]
    max: 1
---

# PR CI — Build Gate

Validate every pull request affecting code files with a full build before allowing merge. Blocks
merge on build failure.

> **⚠️ Manual Testing Required:** Automated tests are NOT run in this pipeline. An admin must run
> `dotnet test "Tests/JojaAutoTasks.Tests.csproj"` locally and confirm all tests pass before
> approving and merging any pull request that changes code.

## Context

- Repository: `${{ github.repository }}`
- PR branch: `${{ github.head_ref }}`
- Base branch: `${{ github.base_ref }}`
- Build command: `dotnet build JojaAutoTasks.csproj -c Debug -p:EnableModDeploy=false -p:EnableModZip=false`
- Target frameworks: mod net6.0, tests net8.0
- Trigger: code changes only (`.cs`, `.csproj`, `.sln`, `manifest.json`)

## Pipeline Steps

1. **Restore** NuGet packages (cache if possible)
2. **Build** mod project in Debug configuration with deploy/zip disabled
3. **Report** pass/fail as a PR status check

## Pass Criteria

- Build exits with code 0 (no compiler errors or warnings-as-errors)
- Triggered only on code file changes (documentation-only PRs are skipped)

## Fail Behavior

- Post a PR check failure with a link to the failing log
- Do NOT create a GitHub Issue for transient failures; only for persistent failures (3+ consecutive)

## Cache Strategy

- Cache NuGet global packages directory (`~/.nuget/packages`)
- Key: `nuget-${{ runner.os }}-${{ hashFiles('**/*.csproj') }}`
- Restore key: `nuget-${{ runner.os }}-`

## Notes

- This workflow is a required status check; configure as branch protection rule for `main`
- `EnableModDeploy=false` ensures no game installation is required in CI
- `EnableModZip=false` speeds up build (packaging not needed for validation)
- **Testing is manual**: Admin must run the full test suite locally before approving any code PR
- This workflow runs only on code changes; documentation-only PRs skip this check
