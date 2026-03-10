---
name: security-scanner
description: "CodeQL static analysis and NuGet vulnerability scanning for security enforcement."
on:
  schedule: weekly
  push:
    branches: [main]
  pull_request:
    branches: [main]
  workflow_dispatch:
permissions:
  contents: read
  security-events: read
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
    title-prefix: "[security] "
    labels: [agentic-workflow, security, needs-review]
    close-older-issues: true
    max: 2
---

# Security Scanner — CodeQL and Dependency Vulnerability Analysis

Continuously enforce security posture through static code analysis and NuGet vulnerability scanning.

## Context

- Repository: `${{ github.repository }}`
- Source language: C# (net6.0 mod, net8.0 tests)
- CodeQL language: `csharp`
- Security instructions: `.github/instructions/security-and-owasp.instructions.md`
- Project files: `JojaAutoTasks.csproj`, `Tests/JojaAutoTasks.Tests.csproj`

## Analysis Scope

### 1. CodeQL Static Analysis (SAST)

Run GitHub CodeQL for C# against all source files:

- **Injection vulnerabilities:** String construction from external input in config parsing
- **Path traversal:** File operations based on user-controlled paths
- **Insecure deserialization:** Unsafe JSON/binary deserialization
- **Sensitive data exposure:** Logging of potentially sensitive player data
- **Null dereference patterns:** Unguarded nullable paths that could crash the game

Use the `security-and-quality` CodeQL query suite for the broadest coverage.

### 2. NuGet Dependency Vulnerability Scan

Run `dotnet list package --vulnerable` on both project files:

- Report high/critical severity advisories immediately
- Report moderate advisories as warnings
- Note which dependency is vulnerable and the advisory CVE/GHSA identifier
- Suggest minimum safe version for each affected package

### 3. Secret Pattern Scan (lightweight)

Search for common secret patterns in source files:

- Hardcoded API keys / tokens matching known patterns
- Connection strings with embedded credentials
- `password`, `secret`, `apiKey` literal assignments in non-test code

## Output Rules

- **Critical/High CodeQL findings:** Create a security issue immediately (even on first occurrence)
- **Moderate CodeQL findings:** Accumulate for weekly discussion
- **Vulnerable NuGet packages:** Create issue with package name, current version, advisory, fix version
- **Secret patterns:** Create issue immediately; do NOT post pattern details in public issue body
- **Clean scan:** No output (silence is success)

## Notes

- CodeQL results appear in the GitHub Security → Code Scanning tab
- Results are also annotated inline on PR diffs when scanning PRs
- CodeQL findings are reported via `safe-outputs.create-issue`; direct security-events upload is not used in strict mode
- This workflow enforces the OWASP guidance in `.github/instructions/security-and-owasp.instructions.md`
- Mod reads/writes SMAPI save data — deserialization paths deserve careful CodeQL review
