# Phase 1 - Atomic Commit Execution Checklist #

Purpose: execute Phase 1 in strict, linear, atomic commits with explicit file/symbol scope.

Note: If a sub-step expands beyond the stated file scope, stop and open a follow-up commit.

## Guardrails (Must Stay True) ##

    * [x] Phase 1 only.
    * [x] No task/store mutation logic.
    * [x] `OnSaving` is signal-only with no writes/checkpoints.
    * [x] `OnUpdateTicked` is throttled no-op guard only.
    * [x] `ConfigVersion` handling is explicit and safe.
    * [x] Config does not alter identity semantics or determinism boundaries.

## 1) Bootstrap SMAPI Skeleton ##

Step goal:

    * [x] Create a minimal compile-safe SMAPI entry shell.

### 1A - Add baseline entry shell ###

    * [x] Action: add `ModEntry : Mod` and minimal `Entry(IModHelper helper)` startup wiring.
    * [x] Scope: `src/JojaAutoTasks/ModEntry.cs` (`ModEntry`, `Entry`).
    * [x] Verify: build succeeds and one startup log appears.
    * [x] Commit message: `phase1(step1A): add minimal ModEntry shell`
    * [x] Must include: baseline `ModEntry` and minimal startup code.
    * [x] Must exclude: config/lifecycle/dispatcher internals.

### 1B - Align project and manifest baseline ###

    * [x] Action: align project metadata and manifest to load cleanly.
    * [x] Scope: `src/JojaAutoTasks/JojaAutoTasks.csproj`, `src/JojaAutoTasks/manifest.json`.
    * [x] Verify: project builds and mod loads without exceptions.
    * [x] Commit message: `phase1(step1B): align csproj and manifest baseline`
    * [x] Must include: metadata and manifest updates only.
    * [x] Must exclude: runtime behavior changes.

### 1C - Register minimal lifecycle hook stubs ###

    * [x] Action: add minimum required hook subscriptions with safe empty handlers.
    * [x] Scope: `src/JojaAutoTasks/ModEntry.cs` hook subscription lines and stub handlers.
    * [x] Verify: hooks compile and remain behavior-free.
    * [x] Commit message: `phase1(step1C): register minimal lifecycle hook stubs`
    * [x] Must include: subscriptions and stubs only.
    * [x] Must exclude: dispatcher/coordinator logic and tests.

## 2) Add Logging Foundation ##

Step goal:

    * [x] Add deterministic and bounded lifecycle logging.

### 2A - Define log event constants ###

    * [x] Action: add stable event names for startup/config/lifecycle paths.
    * [x] Scope: `src/JojaAutoTasks/Infrastructure/Logging/LogEvents.cs`.
    * [x] Verify: constants are deterministic and reusable.
    * [x] Commit message: `phase1(step2A): define stable log event constants`
    * [x] Must include: constants file only.
    * [x] Must exclude: wrapper wiring and behavior changes.

### 2B - Add ModLogger wrapper ###

    * [x] Action: wrap `IMonitor` with conventions for level/context formatting.
    * [x] Scope: `src/JojaAutoTasks/Infrastructure/Logging/ModLogger.cs`.
    * [x] Verify: wrapper compiles and does not mutate runtime state.
    * [x] Commit message: `phase1(step2B): add ModLogger conventions wrapper`
    * [x] Must include: logger wrapper only.
    * [x] Must exclude: integration wiring and tests.

### 2C - Integrate bounded logging ###

    * [x] Action: wire logging into startup/lifecycle entry points with frequency guards.
    * [x] Scope: `src/JojaAutoTasks/ModEntry.cs` and optional coordinator call sites.
    * [x] Verify: logs are concise and there is no tick spam.
    * [x] Commit message: `phase1(step2C): wire bounded lifecycle logging`
    * [x] Must include: integration call sites only.
    * [x] Must exclude: config/dispatcher behavior changes.

## 3) Implement Configuration Load Pipeline ##

Step goal:

    * [x] Add explicit `ConfigVersion` load/default/validate flow.

### 3A - Add config model ###

    * [x] Action: create `ModConfig` with `ConfigVersion` and approved Phase 1 fields.
    * [x] Scope: `src/JojaAutoTasks/Configuration/ModConfig.cs`.
    * [x] Verify: model compiles and defaults are deterministic.
    * [x] Commit message: `phase1(step3A): add ModConfig with ConfigVersion`
    * [x] Must include: config model only.
    * [x] Must exclude: loader/startup wiring.

### 3B - Add loader validation/defaulting ###

    * [x] Action: implement read, validate, and safe default/clamp behavior.
    * [x] Scope: `src/JojaAutoTasks/Configuration/ConfigLoader.cs`.
    * [x] Verify: missing/invalid config is handled safely without crash.
    * [x] Commit message: `phase1(step3B): implement safe ConfigLoader validation path`
    * [x] Must include: loader logic only.
    * [x] Must exclude: coordinator/dispatcher/persistence behavior.

### 3C - Wire config into startup ###

    * [x] Action: wire loader into startup composition before runtime activation.
    * [x] Scope: `src/JojaAutoTasks/ModEntry.cs`.
    * [x] Verify: startup order is deterministic and side-effect free.
    * [x] Commit message: `phase1(step3C): wire config load into startup`
    * [x] Must include: minimal startup integration.
    * [x] Must exclude: non-config runtime refactors.

## 4) Add Lifecycle Coordinator ##

Step goal:

    * [x] Centralize lifecycle signal sequencing in a thin coordinator.

### 4A - Add coordinator shell ###

    * [x] Action: add coordinator class with explicit responsibilities and boundaries.
    * [x] Scope: `src/JojaAutoTasks/Lifecycle/LifecycleCoordinator.cs`.
    * [x] Verify: class compiles and remains orchestration-only.
    * [x] Commit message: `phase1(step4A): add LifecycleCoordinator shell`
    * [x] Must include: class shell and docs/comments.
    * [x] Must exclude: full signal wiring and tests.

### 4B - Route core signals ###

    * [x] Action: route `OnGameLaunched` and `OnSaveLoaded` through coordinator.
    * [x] Scope: `src/JojaAutoTasks/ModEntry.cs`,
      `src/JojaAutoTasks/Lifecycle/LifecycleCoordinator.cs`.
    * [x] Verify: signal order logs are stable and deterministic.
    * [x] Commit message: `phase1(step4B): route core lifecycle signals via coordinator`
    * [x] Must include: forwarding wiring for non-saving hooks.
    * [x] Must exclude: dispatcher processing logic.

### 4C - Enforce signal-only OnSaving ###

    * [x] Action: route `OnSaving` as signal-only with explicit no-write rule.
    * [x] Scope: `src/JojaAutoTasks/ModEntry.cs`,
      `src/JojaAutoTasks/Lifecycle/LifecycleCoordinator.cs`.
    * [x] Verify: no file writes/checkpoints on `OnSaving`.
    * [x] Commit message: `phase1(step4C): enforce signal-only OnSaving flow`
    * [x] Must include: `OnSaving` signal route and guard assertions/comments.
    * [x] Must exclude: persistence layer/checkpoint code.

## 5) Wire Event Dispatcher ##

Step goal:

    * [x] Add deterministic no-op signal forwarding through dispatcher.

### 5A - Add dispatcher interface ###

    * [x] Action: define minimal lifecycle signal dispatch contract.
    * [x] Scope: `src/JojaAutoTasks/Events/IEventDispatcher.cs`.
    * [x] Verify: contract stays Phase 1-appropriate and non-mutating.
    * [x] Commit message: `phase1(step5A): add event dispatcher contract`
    * [x] Must include: interface only.
    * [x] Must exclude: concrete implementation and wiring.

### 5B - Add concrete no-op dispatcher ###

    * [x] Action: implement deterministic `EventDispatcher` forwarding/recording only.
    * [x] Scope: `src/JojaAutoTasks/Events/EventDispatcher.cs`.
    * [x] Verify: ordering is deterministic and processing is no-op.
    * [x] Commit message: `phase1(step5B): implement no-op deterministic EventDispatcher`
    * [x] Must include: dispatcher implementation only.
    * [x] Must exclude: startup/composition wiring and tests.

### 5C - Wire coordinator to dispatcher ###

    * [x] Action: connect coordinator outputs to dispatcher in startup composition.
    * [x] Scope: `src/JojaAutoTasks/ModEntry.cs`,
      `src/JojaAutoTasks/Lifecycle/LifecycleCoordinator.cs`.
    * [x] Verify: expected signals are observed without spam.
    * [x] Commit message: `phase1(step5C): wire coordinator outputs to dispatcher`
    * [x] Must include: composition and forwarding hookups only.
    * [x] Must exclude: task/store mutation paths.

## 6) Implement Safe No-Op Hook Handlers with Tick Guardrails ##

Step goal:

    * [x] Keep handlers forwarding-only and enforce tick no-op throttling.

### 6A - Enforce forwarding-only handlers ###

    * [x] Action: ensure hook handlers only forward signals and return.
    * [x] Scope: `src/JojaAutoTasks/ModEntry.cs` hook handlers.
    * [x] Verify: no heavy processing blocks in hook handlers.
    * [x] Commit message: `phase1(step6A): enforce hook handlers as forwarding no-ops`
    * [x] Must include: handler restrictions and forwarding calls.
    * [x] Must exclude: new runtime workload behavior.

### 6B - Add explicit tick throttle guard ###

    * [x] Action: add `TickThrottleGuard` (or equivalent) for `OnUpdateTicked`.
    * [x] Scope: `src/JojaAutoTasks/ModEntry.cs` and/or
      `src/JojaAutoTasks/Lifecycle/LifecycleCoordinator.cs`.
    * [x] Verify: `OnUpdateTicked` remains throttled no-op guard only.
    * [x] Commit message: `phase1(step6B): add explicit OnUpdateTicked throttle guard`
    * [x] Must include: throttle guard and minimal integration.
    * [x] Must exclude: periodic workloads and persistence behavior.

### 6C - Gate tick diagnostics ###

    * [x] Action: keep tick diagnostics optional, sparse, and config-gated.
    * [x] Scope: `src/JojaAutoTasks/ModEntry.cs`,
      `src/JojaAutoTasks/Infrastructure/Logging/ModLogger.cs`,
      `src/JojaAutoTasks/Configuration/ModConfig.cs` (if field already approved).
    * [x] Verify: idle gameplay has no tick spam.
    * [x] Commit message: `phase1(step6C): gate and bound tick diagnostics`
    * [x] Must include: minimal gating checks.
    * [x] Must exclude: schema expansion beyond approved Phase 1 fields.

## 7) Add Phase 1 Verification Tests ##

Step goal:

    * [x] Add tests that lock Phase 1 constraints and catch drift.

### 7A - Add config tests ###

    * [x] Action: add tests for defaulting/validation and `ConfigVersion`.
    * [x] Scope: `tests/JojaAutoTasks.Tests/Configuration/ConfigLoaderTests.cs`.
    * [x] Verify: tests fail when `ConfigVersion` behavior regresses.
    * [x] Commit message: `phase1(step7A): add ConfigLoader tests with ConfigVersion coverage`
    * [x] Must include: config tests and minimal fixtures.
    * [x] Must exclude: production logic changes.

### 7B - Add lifecycle tests ###

    * [x] Action: add signal flow tests including signal-only `OnSaving` assertions.
    * [x] Scope: `tests/JojaAutoTasks.Tests/Lifecycle/LifecycleCoordinatorTests.cs`.
    * [x] Verify: tests fail if `OnSaving` writes/checkpoints are introduced.
    * [x] Commit message: `phase1(step7B): add lifecycle tests for signal-only OnSaving`
    * [x] Must include: lifecycle tests and required doubles.
    * [x] Must exclude: dispatcher-specific test files and production refactors.

### 7C - Add dispatcher determinism tests ###

    * [x] Action: add deterministic ordering and no-op processing tests.
    * [x] Scope: `tests/JojaAutoTasks.Tests/Events/EventDispatcherTests.cs`.
    * [x] Verify: tests fail if ordering or no-op constraints regress.
    * [x] Commit message: `phase1(step7C): add EventDispatcher determinism tests`
    * [x] Must include: dispatcher tests only.
    * [x] Must exclude: lifecycle/tick test edits and production changes.

### 7D - Add tick guard tests ###

    * [x] Action: add tests for `OnUpdateTicked` throttle and no-op guarantees.
    * [x] Scope: `tests/JojaAutoTasks.Tests/Hooks/UpdateTickedGuardTests.cs`.
    * [x] Verify: tests fail if heavy tick work or unthrottled behavior appears.
    * [x] Commit message: `phase1(step7D): add OnUpdateTicked guardrail tests`
    * [x] Must include: tick guard tests and minimal test utilities.
    * [x] Must exclude: non-test feature changes.

## 8) Phase 1 Completion Gate ##

Step goal:

    * [ ] Confirm Phase 1 is complete, atomic, and contract-aligned.

### 8A - Run build and full tests ###

    * [x] Action: run clean build and full test suite.
    * [x] Scope: no source edits expected.
    * [x] Verify: build succeeds and all Phase 1 tests pass.
    * [x] Commit message: `phase1(step8A): record build and test completion evidence`
    * [x] Must include: durable evidence/docs only if maintained in repo.
    * [x] Must exclude: opportunistic code edits.

### 8B - Complete checklist and scope audit ###

    * [x] Action: audit implementation against checklist and guardrails.
    * [x] Scope: this checklist file and optional existing audit notes.
    * [x] Verify: no task/store mutation paths were introduced.
    * [x] Commit message: `phase1(step8B): complete scope audit against phase1 guardrails`
    * [x] Must include: checklist/audit status updates only.
    * [x] Must exclude: runtime behavior changes.

### 8C - Validate atomic boundaries and defer list ###

    * [x] Action: validate commit boundaries and list deferred non-Phase-1 work.
    * [x] Scope: checklist updates and optional status note files.
    * [x] Verify: each sub-step maps to one clean scope-respecting commit.
    * [x] Commit message: `phase1(step8C): finalize atomic boundary review and defer list`
    * [x] Must include: final checklist state and defer notes.
    * [x] Must exclude: Phase 2 code and broad documentation rewrites.

## Final Completion Gate Checklist ##

    * [ ] All sub-steps `1A` through `8C` are completed or explicitly deferred.
    * [ ] Every completed sub-step has a matching atomic commit.
    * [ ] No commit crosses unrelated file/symbol scopes.
    * [ ] No task/store mutation logic exists in Phase 1.
    * [ ] `OnSaving` remains signal-only with zero writes/checkpoints.
    * [ ] `OnUpdateTicked` remains throttled no-op guard only.
    * [ ] `ConfigVersion` handling is explicit and test-covered.
    * [ ] Build/tests pass with deterministic, bounded logging.
    * [ ] Phase 1 is complete without architectural drift.
