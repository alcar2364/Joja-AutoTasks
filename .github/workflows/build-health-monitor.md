---
name: build-health-monitor
description: "Monitors build and test pipeline health, identifying failures and regressions."
on:
  schedule: daily
  workflow_dispatch:
  workflow_run:
    workflows: ["CI"]
    types: [completed]
    branches: [main]
permissions:
  contents: read
  actions: read
  issues: read
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
  noop:
    report-as-issue: false
  create-issue:
    title-prefix: "[ci] "
    labels: [agentic-workflow, ci-health, needs-review]
    max: 2
---

# Build & Test Health Monitor

Monitor the health of build and test pipelines, detecting failures and environmental regressions.

## Context

- Repository: `${{ github.repository }}`
- Build command: `dotnet build JojaAutoTasks.csproj -c Debug -p:EnableModDeploy=false -p:EnableModZip=false`
- Test command: `dotnet test Tests/JojaAutoTasks.Tests.csproj`
- Validated .NET versions: 8.0.203, 9.0.311, 10.0.103
- Test baseline: 110 tests passing (from last validated run)

## Monitoring Focus

1. **Build Health:**
   - Compilation failures in recent commits
   - Warning accumulation (treat as potential debt)
   - Platform-specific issues (Windows/Linux/macOS)

2. **Test Health:**
   - Test pass/fail rate changes
   - Flaky tests (intermittent failures)
   - Test execution time regressions

3. **Environment Health:**
   - .NET SDK version compatibility
   - NuGet package resolution issues
   - Tool availability (SMAPI, ModBuildConfig)

## Process

1. **Fetch recent workflow runs** (last 7 days)
2. **Analyze failure patterns** per subsystem
3. **Identify root causes** (code vs. environment)
4. **Create issue** for persistent failures only

## Output

- Create issues only for persistent, actionable failures
- Title: `[ci] <System>: <Failure description>`
- Include: Failure logs, recent changes, reproduction steps
- Link to related test output or build logs

## Notes

- Environment issues (SDK versions) route to docs update
- Code issues route to responsible subsystem agent
- Flaky test issues should trigger isolation and determinism review
