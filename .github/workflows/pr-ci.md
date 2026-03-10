---
name: pr-ci
description: "PR-gated build and test pipeline: blocks merge on build failure or test regression."
on:
  pull_request:
    branches: [main]
    types: [opened, synchronize, reopened]
  push:
    branches: [main]
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
  dotnet: {}
safe-outputs:
  create-issue:
    title-prefix: "[ci] "
    labels: [agentic-workflow, ci-health, needs-review]
    max: 1
---

# PR CI — Build and Test Gate

Validate every pull request with a full build and test run before allowing merge. Blocks merge on
build failure or test regression.

## Context

- Repository: `${{ github.repository }}`
- Actor: `${{ github.actor }}`
- Run ID: `${{ github.run_id }}`
- Build command: `dotnet build JojaAutoTasks.csproj -c Debug -p:EnableModDeploy=false -p:EnableModZip=false`
- Test command: `dotnet test Tests/JojaAutoTasks.Tests.csproj --logger trx --results-directory ./TestResults`
- Test baseline: 110 tests passing
- Target frameworks: mod net6.0, tests net8.0

## Pipeline Steps

1. **Restore** NuGet packages (cache if possible)
2. **Build** mod project in Debug configuration with deploy/zip disabled
3. **Test** full test suite and collect TRX results
4. **Report** pass/fail as a PR status check
5. **Annotate** any failing tests directly on the PR diff

## Pass Criteria

- Build exits with code 0 (no compiler errors or warnings-as-errors)
- All tests pass (`dotnet test` exit code 0)
- Test count ≥ baseline (no test deletions without justification)

## Fail Behavior

- Post a PR check failure with a link to the failing log
- Annotate the PR with the specific test names that failed
- If test count dropped: flag as a warning (not automatic block)
- Do NOT create a GitHub Issue for transient failures; only for persistent failures (3+ consecutive)

## Cache Strategy

- Cache NuGet global packages directory (`~/.nuget/packages`)
- Key: `nuget-ubuntu-latest-<hash of *.csproj files>`
- Restore key: `nuget-ubuntu-latest-`

## Notes

- This workflow is a required status check; configure as branch protection rule for `main`
- `EnableModDeploy=false` ensures no game installation is required in CI
- `EnableModZip=false` speeds up build (packaging not needed for validation)
- Publish TRX test results as an artifact named `test-results` for the build-health-monitor to consume
