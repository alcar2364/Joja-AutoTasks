# Joja AutoTasks

Joja AutoTasks is an in-development [SMAPI](https://smapi.io/) mod for Stardew Valley focused on automatic in-game task tracking.

The long-term plan is a unified task system that combines built-in automatic tasks, manually created automatic tasks, and manual tasks into an all-in one system!

This repository is currently source-first: it is meant for modders and contributors who want to follow implementation progress and work directly with the code.

## Contents

- [Current Status](#current-status)
- [What You Get Today](#what-you-get-today)
- [Install (Development Build)](#install-development-build)
- [Build and Test](#build-and-test)
- [Configuration](#configuration)
- [Source Code Tour](#source-code-tour)
- [Design Docs and Roadmap](#design-docs-and-roadmap)
- [Contributing](#contributing)
- [License](#license)

## Current Status

Joja AutoTasks is actively being built in phases.

Implemented now:

- SMAPI entrypoint and lifecycle hook forwarding.
- Deterministic config loading and version normalization.
- Deterministic identifier primitives (`TaskId`, `RuleId`, `SubjectId`, `DayKey`).
- Core task domain model with constructor invariants.
- State Store foundation: command contracts, command handlers, internal state container, and read-only snapshot projection models.
- Deterministic unit tests for lifecycle, config migration safety, dispatcher guardrails, identifiers, domain invariants, and state-store foundations.

Still in progress:

- Wiring lifecycle dispatch into full task processing.
- Persistence/save-data integration.
- In-game HUD/menu surfaces.
- Built-in generator engine and task-builder rule evaluation pipeline.

## What You Get Today

If you install the current development build, the mod loads and runs safely, but it is still foundational.

You should expect:

- Stable startup and lifecycle wiring.
- Config file generation/loading.
- No full player-facing task UI or complete gameplay automation loop yet.

In short: this is a real, working codebase under active construction, but nowhere close to being released.

## Install (Development Build)

1. Install the latest version of [SMAPI](https://smapi.io/).
2. Clone this repository.
3. From the repository root, build with deploy enabled:

```powershell
dotnet build JojaAutoTasks.csproj -c Debug -p:EnableModDeploy=true -p:EnableModZip=true
```

4. Launch Stardew Valley through SMAPI.

Notes:

- The project uses `Pathoschild.Stardew.ModBuildConfig` for deploy/packaging behavior.
- You can verify the resolved game path with:

```powershell
dotnet msbuild JojaAutoTasks.csproj -nologo -getProperty:GamePath
```

## Build and Test

Build (check only, no deploy/zip):

```powershell
dotnet build JojaAutoTasks.csproj -c Debug -p:EnableModDeploy=false -p:EnableModZip=false
```

Run full tests:

```powershell
dotnet test "Tests\JojaAutoTasks.Tests.csproj"
```

Focused test examples:

```powershell
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~LifecycleCoordinatorTests
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~EventDispatcherTests
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~UpdateTickedGuardTests
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~ConfigLoaderMigrationSafetyTests
```

## Configuration

`config.json` is created/managed by SMAPI and currently supports:

| Setting | Default | Purpose |
| --- | --- | --- |
| `ConfigVersion` | `1` | Internal schema version marker for safe normalization/migration paths. |
| `EnableMod` | `true` | Master enable flag reserved for runtime gating as features are wired in. |
| `EnableDebugMode` | `false` | Enables debug-level lifecycle/tick-forwarding diagnostics. |

## Source Code Tour

- `ModEntry.cs`: SMAPI entrypoint and lifecycle hook forwarding.
- `Startup/`: composition root/runtime container.
- `Configuration/`: config schema and loader normalization logic.
- `Domain/Identifiers/`: deterministic identifier value types and format/factory utilities.
- `Domain/Tasks/`: immutable task domain objects and enums.
- `Lifecycle/`: lifecycle coordination logic.
- `Events/`: dispatcher contract and implementation.
- `StateStore/`: command/handler state mutation pipeline plus snapshot projection models.
- `Tests/`: deterministic xUnit test suites.

## Design Docs and Roadmap

If you want the architectural intent and staged implementation plan, start here:

- [Joja AutoTasks Design Guide](Project/Planning/Joja%20AutoTasks%20Design%20Guide/JojaAutoTasks%20Design%20Guide.md)
- [Architecture Map](Project/Planning/Architecture%20Map.md)
- [Implementation Plan Folder](Project/Tasks/Implementation%20Plan)
- [Test Project Notes](Tests/README.md)

## Contributing

Issues and PRs are welcome.

If you want to help, the most useful contributions right now are:

- deterministic behavior fixes,
- state-store and lifecycle wiring improvements,
- tests that lock invariants and prevent regressions,
- docs updates that keep implementation and design notes in sync.

Before opening a PR, please run:

```powershell
dotnet build JojaAutoTasks.csproj -c Debug -p:EnableModDeploy=false -p:EnableModZip=false
dotnet test "Tests\JojaAutoTasks.Tests.csproj"
```

## License

MIT. See [LICENSE.txt](LICENSE.txt).
