# JojaAutoTasks.Tests #

## Purpose ##

This project contains deterministic unit tests for Joja AutoTasks.

Primary goals:
    1. protect behavior and architecture boundaries
    1. catch regressions early
    1. keep tests stable and non-flaky

## Test Naming Pattern ##

Use this pattern:

`MethodName_WhenCondition_ExpectedBehavior`

Examples:
    1. `Load_WhenReadConfigThrows_ReturnsDefaultConfig`
    1. `Load_WhenVersionIsFuture_NormalizesToCurrentVersion`

## Test Structure Pattern ##

Use standard `Arrange -> Act -> Assert` sections.

Suggested shape:
    1. Arrange inputs/mocks and create SUT.
    1. Act by calling one public method.
    1. Assert only the behavior under test.
    1. Verify dependency interactions (`Times.Once`, etc.) when relevant.

## Fact vs Theory ##

Use `[Fact]` for a unique scenario.

Use `[Theory]` when one behavior is validated by multiple inputs.

Examples for config version logic:
    1. older values (`current - 1`, negative, `int.MinValue`)
    1. future values (`current + 1`, large future, `int.MaxValue`)

## ConfigLoader Test Matrix ##

Current coverage in `Configuration/` includes:
    1. current version input -> normalized output and preserved flags
    1. older version input -> upgraded/normalized to current
    1. future version input -> normalized to current
    1. exception during read -> default config returned safely
    1. null config from helper -> default config returned safely
    1. returned config instance is newly created (no mutation of input instance)
    1. non-version settings remain stable across old/future normalization paths

## Determinism Rules for Tests ##

All tests should:
    1. avoid wall-clock dependencies
    1. avoid random data unless explicitly seeded
    1. avoid dependence on unordered collection iteration
    1. isolate state per test

## When ConfigVersion Changes ##

When `ModConfig.CurrentConfigVersion` is incremented:
    1. update migration logic in production code first
    1. add/adjust migration tests in `ConfigLoaderMigrationSafetyTests`
    1. keep boundary cases in place (`int.MinValue`, `int.MaxValue`) unless behavior changes by
       design

## Review Checklist for New Tests ##

Before submitting new tests:
    1. test name describes expected behavior clearly
    1. assertions validate behavior, not just non-null
    1. at least one edge case is covered
    1. interaction checks are included where dependency calls are meaningful
    1. tests pass locally via:

```powershell
dotnet test "JojaAutoTasks.Tests\JojaAutoTasks.Tests.csproj"
```
