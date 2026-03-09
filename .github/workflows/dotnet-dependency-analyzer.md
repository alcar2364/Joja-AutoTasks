---
name: dotnet-dependency-analyzer
description: "Weekly analyzer for NuGet package usage, version currency, and dependency health."
on:
  schedule: weekly
  workflow_dispatch:
permissions:
  contents: read
strict: true
network:
  allowed: [defaults, github]
engine:
  id: codex
tools:
  github:
    toolsets: [default]
  dotnet: {}
safe-outputs:
  create-discussion:
    category: general
    title-prefix: "[dependencies] "
---

# .NET Dependency Analyzer

Weekly analysis of NuGet packages for usage patterns, version currency, and dependency health.

## Context

- Repository: `${{ github.repository }}`
- Project files: JojaAutoTasks.csproj, Tests/JojaAutoTasks.Tests.csproj
- Core dependency: Pathoschild.Stardew.ModBuildConfig 4.4.0
- Min API version: 4.4.0

## Analysis Focus

1. **Dependency Freshness:**
   - Direct dependencies and their versions
   - Available updates (breaking vs. minor vs. patch)
   - Last update time per dependency

2. **Usage Patterns:**
   - Required vs. optional dependencies
   - Transitive dependency bloat
   - Unused dependencies (if detectable)

3. **Health Indicators:**
   - Package maintenance status (active vs. dormant)
   - License compatibility
   - Security advisories

## Process

1. **Extract dependencies** from .csproj files
2. **Fetch package metadata** from NuGet.org
3. **Analyze usage patterns** in code
4. **Compile recommendations** for review
5. **Create discussion** with findings and suggestions

## Output

- Weekly discussion with dependency analysis
- Group by: Mod project, Test project, Transitive
- Include: Current version, available updates, recommendations
- Note any security advisories or maintenance concerns

## Notes

- Focus on direct dependencies; transitive as secondary
- ModBuildConfig is strategic dependency; note version alignment with SMAPI
- Test project dependencies can differ from mod project
- Suggest updates but don't implement automatically
