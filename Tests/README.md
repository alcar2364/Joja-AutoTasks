# JojaAutoTasks.Tests

## Purpose

This project contains deterministic unit tests for Joja AutoTasks.

Primary goals: 1. protect behavior and architecture boundaries 1. catch regressions early 1. keep tests stable and non-flaky

## Test Naming Pattern

Use this pattern:

`MethodName_WhenCondition_ExpectedBehavior`

Examples: 1. `Load_WhenReadConfigThrows_ReturnsDefaultConfig` 1. `Load_WhenVersionIsFuture_NormalizesToCurrentVersion`

## Test Structure Pattern

Use standard `Arrange -> Act -> Assert` sections.

Suggested shape: 1. Arrange inputs/mocks and create SUT. 1. Act by calling one public method. 1. Assert only the behavior under test. 1. Verify dependency interactions (`Times.Once`, etc.) when relevant.

## Fact vs Theory

Use `[Fact]` for a unique scenario.

Use `[Theory]` when one behavior is validated by multiple inputs.

Examples for config version logic: 1. older values (`current - 1`, negative, `int.MinValue`) 1. future values (`current + 1`, large future, `int.MaxValue`)

## ConfigLoader Test Matrix

Current coverage in `Configuration/` includes: 1. current version input -> normalized output and preserved flags 1. older version input -> upgraded/normalized to current 1. future version input -> normalized to current 1. exception during read -> default config returned safely 1. null config from helper -> default config returned safely 1. returned config instance is newly created (no mutation of input instance) 1. non-version settings remain stable across old/future normalization paths

## LifecycleCoordinator Test Matrix

Current coverage in `Lifecycle/` includes: 1. `HandleGameLaunched` forwards only `DispatchGameLaunched` 1. `HandleSaveLoaded` forwards only `DispatchSaveLoaded` 1. `HandleSavingInProgress` forwards only `DispatchSavingInProgress` (signal-only `OnSaving` guard) 1. `HandleUpdateTicked(false)` forwards only `DispatchUpdateTicked` 1. sequential lifecycle calls preserve deterministic dispatch order

These tests are intentionally forwarding-focused and do not assert dispatcher internals or
processing logic.

## HUD Lifecycle Test Matrix

Current coverage in `UI/` includes:

1. `ModEntryHudLifecycle` day-start recreation keeps exactly one active
   `HudViewModel` instance after recreation.
1. Day-start recreation uses a fresh `HudViewModel` instance
   (UI-local state reset semantics).
1. `HandleReturnedToTitle` clears the active `HudViewModel` reference.
1. `ModEntry.OnReturnedToTitle` tears down HUD lifecycle state before runtime
   return-to-title dispatch/teardown clears backend state.

## Hooks Test Matrix

Current coverage in `Hooks/` includes: 1. deterministic throttle interval for `ModEntry.ShouldForwardUpdateTick` (`0` true,
`359` false, `360` true, `361` false) 1. `OnUpdateTicked` forwards once, then blocks repeated same-tick calls inside the throttle
window 1. guard-block no-op contract: throttled follow-up call does not require runtime access and
does not dispatch

These tests protect Phase 1 guardrails by locking deterministic tick-throttle behavior,
single-dispatch forwarding semantics, and safe no-op guard-block paths.

## EventDispatcher Test Matrix

Current coverage in `Events/` includes: 1. all dispatch methods are explicit no-ops (no-op contract) 1. dispatcher remains stateless (no instance fields) 1. dispatch call order has no behavioral impact (order-invariant behavior) 1. no-op IL contract is enforced across all public dispatch methods

## State Test Matrix

Current coverage in `State/` includes:

1. `DeadlineEvaluatorTests` covers day/time edges for remaining/overdue/closed.
1. `BootstrapGuardTests` covers policy modes, warning dedupe, and reset.
1. `ToastTransitionTests` covers auto-complete only and toast-before-snapshot.
1. `TimeChangedGatingTests` covers inactive ignore (including no
   projection-context initialization) and active-only forwarding.
1. `CommandHandlerDeterminismTests` covers `CompleteTaskCommand` parity for
   `isPlayerInitiated` true/false.
1. `SnapshotPublishingTests` covers projection for null/populated
   `DeadlineStoredFields`.

## Determinism Rules for Tests

All tests should: 1. avoid wall-clock dependencies 1. avoid random data unless explicitly seeded 1. avoid dependence on unordered collection iteration 1. isolate state per test

## When ConfigVersion Changes

When `ModConfig.CurrentConfigVersion` is incremented: 1. update migration logic in production code first 1. add/adjust migration tests in `ConfigLoaderMigrationSafetyTests` 1. keep boundary cases in place (`int.MinValue`, `int.MaxValue`) unless behavior changes by
design

## Review Checklist for New Tests

Before submitting new tests: 1. test name describes expected behavior clearly 1. assertions validate behavior, not just non-null 1. at least one edge case is covered 1. interaction checks are included where dependency calls are meaningful 1. if production files changed, the mod project still builds without test-only dependencies 1. tests pass locally via:

Required workflow: complete UnitTestAgent handoff after creating or extending
unit tests.

```powershell
dotnet build JojaAutoTasks.csproj -c Debug -p:EnableModDeploy=false -p:EnableModZip=false
```

Run the full test suite:

```powershell
dotnet test "Tests\JojaAutoTasks.Tests.csproj"
```

For focused lifecycle validation:

```powershell
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~LifecycleCoordinatorTests
```

For focused dispatcher validation:

```powershell
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~EventDispatcherTests
```

For focused update-tick guard validation:

```powershell
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~UpdateTickedGuardTests
```

For focused config migration safety validation:

```powershell
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~ConfigLoaderMigrationSafetyTests
```

For focused Section 9 command guard validation:

```powershell
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~CommandValidationTests
```

For focused Section 9 handler determinism validation:

```powershell
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~CommandHandlerDeterminismTests
```

For focused Section 9 field separation validation:

```powershell
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~FieldSeparationTests
```

For focused Section 9 snapshot immutability/publishing validation:

```powershell
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~SnapshotImmutabilityTests|FullyQualifiedName~SnapshotPublishingTests
```

For focused Section 9 day-boundary expiration validation:

```powershell
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~DayBoundaryTests
```

For focused Section 9 manual ID sequencing validation:

```powershell
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~ManualTaskCounterTests
```

For focused Section 9 state boundary enforcement validation:

```powershell
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~StateStoreBoundaryTests
```

For focused Section 9 deadline evaluator validation:

```powershell
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~DeadlineEvaluatorTests
```

Includes an explicit due-date equality guard (`Evaluate_WhenTodayMatchesDueDate_IsOverdueIsFalse`).

For focused Section 9 bootstrap guard validation:

```powershell
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~BootstrapGuardTests
```

For focused Section 9 toast transition validation:

```powershell
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~ToastTransitionTests
```

For focused Section 9 time-changed gating validation:

```powershell
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~TimeChangedGatingTests
```

For focused lifecycle init/teardown integration validation:

```powershell
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~LifecycleCoordinatorIntegrationTests
```

For focused HUD lifecycle ownership validation:

```powershell
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~HudHostLifecycleTests
```
