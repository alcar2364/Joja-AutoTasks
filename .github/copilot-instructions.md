# Joja AutoTasks - Copilot Coding Agent Onboarding

## Read This First

1. Trust this document as the primary operational guide for this repository.
2. Only search the repo when information here is missing or proven wrong.
3. Keep edits scoped and deterministic; this project enforces strong architecture boundaries.

## Repository Summary

Joja AutoTasks is a Stardew Valley SMAPI mod that provides deterministic in-game task tracking.
It is designed around a command/snapshot architecture with strict separation between lifecycle
signal forwarding, canonical state ownership, persistence, and UI consumption.

Current implemented core is a Phase 1 foundation:

* SMAPI entrypoint and lifecycle hook forwarding
* config loading/normalization with version handling
* deterministic identifier/domain primitives
* dispatcher/lifecycle guardrails and deterministic unit tests

## High-Level Repository Facts

* Repository type: .NET/C# SMAPI mod + xUnit test project
* Primary language: C#
* Tracked repo size: about 180 tracked files
* Tracked extension mix: mostly Markdown docs/instructions, plus C# source and JSON config
* Target frameworks:
		* Mod project: net6.0
		* Tests project: net8.0
* Core build dependency: Pathoschild.Stardew.ModBuildConfig 4.4.0
* Test stack: xUnit, Moq, Microsoft.NET.Test.Sdk, coverlet.collector
* CI/workflows: no .github/workflows YAML files are currently present

## Bootstrap and Environment Setup (Validated)

Always run from repository root.

Required tooling and environment:

1. .NET SDKs (validated locally):
		* 8.0.203
		* 9.0.311
		* 10.0.103
2. Stardew Valley + SMAPI installation is required for run/deploy tasks.
3. Game path must resolve through ModBuildConfig:

```powershell
dotnet msbuild JojaAutoTasks.csproj -nologo -getProperty:GamePath
```

Validated result in this environment:

* C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley

Recommended bootstrap sequence:

```powershell
dotnet clean JojaAutoTasks.sln -c Debug
dotnet restore JojaAutoTasks.sln
```

## Build, Test, Run, and Packaging Commands (Validated)

### Build (check-only)

```powershell
dotnet build JojaAutoTasks.csproj -c Debug -p:EnableModDeploy=false -p:EnableModZip=false
```

Observed time: about 0.6-1.7 seconds.
Postcondition: `bin/Debug/net6.0/JojaAutoTasks.dll` exists.

### Build (debug deploy + zip)

```powershell
dotnet build JojaAutoTasks.csproj -c Debug -p:EnableModDeploy=true -p:EnableModZip=true
```

Observed time: about 0.85 seconds.
Postconditions:

* deployed mod folder exists at `<GamePath>\Mods\JojaAutoTasks`
* zip created in `bin/Debug/net6.0/` (example: `JojaAutoTasks 0.1.0.zip`)

### Build (release package)

```powershell
dotnet build JojaAutoTasks.csproj -c Release -p:EnableModDeploy=false -p:EnableModZip=true
```

Observed time: about 0.83 seconds.
Postcondition: release zip created in `bin/Release/net6.0/`.

### Test

```powershell
dotnet test "Tests\JojaAutoTasks.Tests.csproj"
```

Observed time: about 2.9-3.9 seconds.
Observed result: 110 passed, 0 failed.

Focused test examples (from Tests/README.md):

```powershell
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~LifecycleCoordinatorTests
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~EventDispatcherTests
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~UpdateTickedGuardTests
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~ConfigLoaderMigrationSafetyTests
```

### Run (SMAPI)

```powershell
$gamePath = (dotnet msbuild JojaAutoTasks.csproj -nologo -getProperty:GamePath).Trim()
$smapiExe = Join-Path $gamePath 'StardewModdingAPI.exe'
Start-Process -FilePath $smapiExe -WorkingDirectory $gamePath
```

Observed launch time: about 0.04 seconds to spawn process.

### Close running game/SMAPI (safe cleanup)

```powershell
$processNames = @('Stardew Valley', 'StardewValley', 'StardewModdingAPI')
Get-Process -ErrorAction SilentlyContinue |
Where-Object { $processNames -contains $_.ProcessName } |
Stop-Process -Force
```

## Lint/Formatting/Static Analysis

No dedicated lint command or CI linter workflow is currently defined in repository-tracked files.

Operational validation gate for this repo is:

1. successful build of mod project (check-only)
2. successful unit test run
3. targeted tests when touching specific areas (see PR template and Tests/README.md)

## Command Order, Failures, and Workarounds

Known-good sequences:

### Fast local verify

```powershell
dotnet build JojaAutoTasks.csproj -c Debug -p:EnableModDeploy=false -p:EnableModZip=false
dotnet test "Tests\JojaAutoTasks.Tests.csproj"
```

### Clean rebuild verify

```powershell
dotnet clean JojaAutoTasks.sln -c Debug
dotnet restore JojaAutoTasks.sln
dotnet build JojaAutoTasks.csproj -c Debug -p:EnableModDeploy=false -p:EnableModZip=false
dotnet test "Tests\JojaAutoTasks.Tests.csproj"
```

### In-game debug cycle

```powershell
dotnet build JojaAutoTasks.csproj -c Debug -p:EnableModDeploy=true -p:EnableModZip=true
# then launch SMAPI
```

Observed failure mode:

* Running `dotnet test ... --no-build` immediately after `dotnet clean Tests\JojaAutoTasks.Tests.csproj -c Debug`
	fails because the test DLL is missing (`Tests\bin\Debug\net8.0\JojaAutoTasks.Tests.dll` not found).
* Workaround: run normal `dotnet test` (without `--no-build`) or build tests first.

Timeout notes:

* No command failures due to timeout were observed during validation.

VS Code tasks notes:

* `.vscode/tasks.json` defines helpful commands (build variants, SMAPI run, close game), but `.vscode`
	is ignored by `.gitignore`; do not assume task definitions exist in every clone.
* When task terminals are reused, output may include prior command history. Verify the final execution block.

## Pre-PR Validation (Replicate Locally)

There are no checked-in GitHub Actions workflows to mirror, so use this local pre-PR checklist:

1. `dotnet build JojaAutoTasks.csproj -c Debug -p:EnableModDeploy=false -p:EnableModZip=false`
2. `dotnet test "Tests\JojaAutoTasks.Tests.csproj"`
3. If touched area requires it, run focused tests listed in `Tests/README.md`
4. Align with `.github/pull_request_template.md` testing checkboxes

## Architecture and Project Layout Map

### Root inventory

`.git/`, `.github/`, `.vscode/`, `Configuration/`, `Domain/`, `Events/`, `Infrastructure/`,
`Lifecycle/`, `Startup/`, `StateStore/`, `Tests/`, `ModEntry.cs`, `JojaAutoTasks.csproj`,
`JojaAutoTasks.sln`, `manifest.json`, `README.md`, `LICENSE.txt`.

### Major architectural elements

* `ModEntry.cs`: SMAPI entrypoint; subscribes lifecycle events and throttles UpdateTicked forwarding.
* `Startup/BootstrapContainer.cs`: composition root wiring logger, config loader, dispatcher,
	lifecycle coordinator.
* `Startup/ModRuntime.cs`: runtime dependency container.
* `Configuration/ModConfig.cs`: persisted config schema and `CurrentConfigVersion`.
* `Configuration/ConfigLoader.cs`: config read, fallback, and version normalization.
* `Lifecycle/LifecycleCoordinator.cs`: lifecycle signal forwarding and debug-aware tick logging.
* `Events/IEventDispatcher.cs` and `Events/EventDispatcher.cs`: lifecycle dispatch contract and current
	deterministic no-op implementation.
* `Domain/Identifiers/`: canonical value types (`TaskId`, `RuleId`, `SubjectId`, `DayKey`) and format/factory helpers.
* `Domain/Tasks/`: immutable task domain object and enums.
* `StateStore/Commands/`: command contracts (for state mutation boundary evolution).
* `.github/agents/BrainAgent.agent.md`: memory-system agent that owns indexed store operations under `.github/memory/`.
* `Tests/`: unit tests grouped by subsystem (`Configuration`, `Lifecycle`, `Events`, `Hooks`, `Domain`).

### Documentation and contracts location

* Primary technical docs: `.github/Project Planning/Joja AutoTasks Design Guide/`
* Architecture reference map: `.github/Project Planning/Architecture Map.md`
* Core contracts/instructions: `.github/instructions/*.instructions.md`
* PR validation checklist: `.github/pull_request_template.md`
* Test conventions and focused command list: `Tests/README.md`

### README status

`README.md` is currently minimal (`Project repository initialized.`).
Use the design guide and architecture map in `.github/Project Planning/` as the operational source
of truth for implementation context.

## Non-Obvious Dependencies and Behaviors

1. `Pathoschild.Stardew.ModBuildConfig` drives GamePath resolution, deploy-copy behavior, and zip packaging.
2. Test project references the mod project; test runs also build mod output.
3. Manifest `MinimumApiVersion` is `4.4.0`, aligned with ModBuildConfig package baseline.
4. Copilot hooks exist under `.github/hooks/` for prompt/session guardrails, but these are not a replacement
	 for build/test validation.

## Final Operating Rule for Agents

Trust this onboarding file first. Do not start broad repo searches unless:

1. required information is missing here, or
2. a command/file/path documented here no longer behaves as described.
