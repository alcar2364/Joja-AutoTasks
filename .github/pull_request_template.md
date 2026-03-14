# Pull Request

## Summary

Describe what changed and why.

## Scope

    * [ ] Scope is intentional and limited to the requested behavior/change.
    * [ ] Any out-of-scope follow-ups are explicitly listed below.

## Testing

> **⚠️ Testing is manual and must be performed by an admin before merge.**
> Automated tests are NOT run in CI. If this PR contains code changes, an admin must run the
> test suite locally and confirm all tests pass before approving.

If this PR contains code changes, an admin must verify the following before approving:

    * [ ] Admin has run `dotnet build JojaAutoTasks.csproj -c Debug -p:EnableModDeploy=false -p:EnableModZip=false` locally and the build succeeds.
    * [ ] Admin has run `dotnet test "Tests/JojaAutoTasks.Tests.csproj"` locally and all tests pass.
    * [ ] If tests were created or extended, UnitTestAgent handoff is included.
    * [ ] If `OnUpdateTicked` flow changed, admin verified tests cover deterministic
    	tick throttling and the guard-block no-op path (no runtime access, no
    	second dispatch in-window).
    * [ ] If lifecycle signal routing changed, admin verified lifecycle tests cover
    	signal forwarding and signal-only `OnSaving` expectations.
    * [ ] If dispatcher routing changed, admin verified dispatcher tests still
    	assert determinism, statelessness, and no-op dispatch contracts.
    * [ ] If `ModConfig.CurrentConfigVersion` or config migration logic changed,
    	admin ran `dotnet test "Tests/JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~ConfigLoaderMigrationSafetyTests`.
    * [ ] If Section 9 command validation changed, admin verified command guard
    	validation (`CommandValidationTests`).
    * [ ] If Section 9 handler behavior changed, admin verified handler
    	determinism (`CommandHandlerDeterminismTests`).
    * [ ] If Section 9 handler field writes changed, admin verified field
    	separation (`FieldSeparationTests`).
    * [ ] If snapshot creation/publishing changed, admin verified snapshot
    	immutability and publishing (`SnapshotImmutabilityTests`,
    	`SnapshotPublishingTests`).
    * [ ] If deadline evaluation or projection behavior changed, admin verified
    	deadline edge matrix and projection coverage (`DeadlineEvaluatorTests`,
    	`SnapshotPublishingTests`).
    * [ ] If day transition/expiration logic changed, admin verified day-boundary
    	expiration (`DayBoundaryTests`).
    * [ ] If bootstrap initialization/guard behavior changed, admin verified
    	bootstrap guard policy, warning dedupe, and reset behavior
    	(`BootstrapGuardTests`).
    * [ ] If completion toast behavior changed, admin verified auto-complete-only
    	toast emission, no duplicate re-complete toasts, and toast-before-snapshot
    	ordering (`ToastTransitionTests`).
    * [ ] If time-change lifecycle forwarding changed, admin verified inactive
        ignore behavior, no projection-context initialization while inactive,
        active forwarding, and no snapshot publication from time changes alone
        (`TimeChangedGatingTests`).
    * [ ] If manual task ID sequencing changed, admin verified manual ID
        sequencing (`ManualTaskCounterTests`).
    * [ ] If StateStore boundaries changed, admin verified state boundary
        enforcement (`StateStoreBoundaryTests`).
    * [ ] If lifecycle init/teardown wiring changed, admin verified lifecycle
        integration (`LifecycleCoordinatorIntegrationTests`).
    * [ ] If ModEntry HUD lifecycle ownership wiring changed (`SaveLoaded`,
        `DayStarted`, `ReturnedToTitle`), admin verified
        `HudHostLifecycleTests` cover exactly-one active snapshot/toast
        subscriptions after day-start recreation, disposal-before-replacement
        host subscription ordering, and fresh `HudViewModel` recreation.

If this PR contains **only documentation changes**, testing is not required.

## Risks / Notes

List risks, tradeoffs, and migration concerns (if any).

## Follow-ups

List optional follow-up tasks.
