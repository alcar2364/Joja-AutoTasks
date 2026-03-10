---
name: multi-platform-build-matrix
description: "Weekly cross-platform build and test matrix across Windows, Linux, and macOS."
on:
  workflow_dispatch:
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
  dotnet: {}
safe-outputs:
  create-issue:
    title-prefix: "[platform] "
    labels: [agentic-workflow, platform-compatibility, needs-review]
    close-older-issues: true
    max: 3
---

# Multi-Platform Build Matrix — Cross-Platform Compatibility Validator

Proactively validate that the mod project and test suite build and run correctly on Windows, Linux,
and macOS. Catches platform-specific incompatibilities before they affect contributors.

## Context

- Repository: `${{ github.repository }}`
- Platforms: `ubuntu-latest`, `windows-latest`, `macos-latest`
- .NET versions to test: SDK 8 (test target), SDK 6 equivalent (mod target via net6.0 TFM)
- Build command: `dotnet build JojaAutoTasks.csproj -c Debug -p:EnableModDeploy=false -p:EnableModZip=false`
- Test command: `dotnet test Tests/JojaAutoTasks.Tests.csproj`

## Matrix Configuration

```
strategy:
  fail-fast: false
  matrix:
    os: [ubuntu-latest, windows-latest, macos-latest]
```

`fail-fast: false` ensures all three platforms are tested even if one fails, providing complete
compatibility information in a single run.

## Per-Platform Steps

For each OS in the matrix:

1. **Checkout** with `fetch-depth: 1` (shallow clone for speed)
2. **Setup .NET SDK** (latest 8.x available on runner)
3. **Restore** NuGet packages
4. **Build** mod project with deploy/zip disabled
5. **Test** full test suite
6. **Upload** test results as artifact named `test-results-<os>`
7. **Report** pass/fail per platform

## Known Platform Differences to Watch For

### File Path Separators

- Windows uses `\` backslashes; Linux/macOS use `/` forward slashes
- Any `Path.Combine` with hardcoded separators or `string` path construction can silently fail on Linux/macOS

### Case-Sensitive Filesystems

- Linux has a case-sensitive filesystem; macOS has a case-insensitive one by default
- `using Jojautotasks.Foo;` compiles on Windows but fails on Linux if the namespace is `JojaAutoTasks.Foo`

### Environment.SpecialFolder

- `Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)` returns different paths per platform
- Game path resolution (`GamePath` in ModBuildConfig) is platform-specific

### Line Ending Normalization

- `.gitattributes` enforces LF/CRLF normalization, but test fixtures embedded as strings may differ

## Output

### All Platforms Pass

No output (silence is success). Existing `build-health-monitor` covers persistent failures.

### One or More Platforms Fail

For each failing platform, create a separate issue:

- Title: `[platform] <OS>: Build/test failure — <brief description>`
- Body:
  - Platform and .NET version
  - Step that failed (restore / build / test)
  - Relevant error output
  - Suggested investigation approach
- Close older issue for the same platform if it was already open

## Notes

- `EnableModDeploy=false` ensures no Stardew Valley game installation is required
- Platform failures are rare but high-value to catch early; weekly schedule is appropriate
- This workflow is complementary to `pr-ci` (which uses a single platform); it provides
  the cross-platform assurance that the PR gate cannot give
- Windows build is most critical for end-user compatibility; Linux/macOS for contributor workflows
