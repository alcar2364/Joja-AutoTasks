# Phase 2 - Atomic Commit Execution Checklist #

Purpose: execute Phase 2 (Core Domain Model) in strict, linear, atomic commits with explicit
file/symbol scope.

Note: If a sub-step expands beyond the stated file scope, stop and open a follow-up commit.

## Guardrails (Must Stay True) ##

    * [x] Phase 2 only.
    * [x] Preserve all Phase 1 invariants (`OnSaving` signal-only, throttled `OnUpdateTicked`, bounded logging).
    * [x] No State Store command/reducer/snapshot logic in this phase.
    * [x] No UI/HUD/Menu/ViewModel/StarML work in this phase.
    * [x] No persistence schema/migration work in this phase.
    * [x] Localization changes that alter runtime behavior are deferred to Phase 3+; Phase 2 may
    only add documentation/contract guardrails.
    * [x] Manual task ID issuance/counter ownership is deferred to Phase 3 (State Store path).
    * [x] Completion-marking runtime behavior is deferred (domain structure exists in Phase 2).
    * [x] RuleId sequential-generation enforcement is deferred until RuleId generation exists.
    * [x] Deterministic IDs only; no random/GUID/time-seeded identity generation.
    * [x] Canonical TaskId forms stay underscore-delimited (`BuiltIn_*`, `TaskBuilder_*`,
    `Manual_{Counter}`).
    * [x] `TaskStatus` is V1-minimal (`Incomplete`, `Completed`) only.
    * [x] Sorting/comparison is deterministic and cannot depend on unordered traversal or insertion order.
    * [x] DayKey uses canonical `Year{N}-{Season}{D}` style (for example,
    `Year1-Summer15`) with fixed non-localized season tokens and invariant casing.
    * [x] UpdateTicked guard seam fix remains test-only and passing.
    * [x] All new C# files/types satisfy `CSHARP-STYLE-CONTRACT.instructions.md`.

## 1) Establish Core Identifier Value Types ##

Step goal:

    * [x] Add strongly-typed deterministic identity primitives for domain modeling.

### 1A - Add `TaskId` value type ###

    * [x] Action: add immutable `TaskId` value type with explicit canonical string normalization/validation entry points.
    * [x] Scope: `Domain/Identifiers/TaskId.cs` (`TaskId`).
    * [x] Verify: build succeeds; equality semantics are deterministic.
    * [x] Commit message: `phase2(step1A): add immutable TaskId value type`
    * [x] Must include: `TaskId` only.
    * [x] Must exclude: factories/parsers/day-key logic.

### 1B - Add `DayKey` value type ###

    * [x] Action: add immutable `DayKey` value type for canonical day identity.
    * [x] Scope: `Domain/Identifiers/DayKey.cs` (`DayKey`).
    * [x] Verify: build succeeds; value comparison is deterministic.
    * [x] Commit message: `phase2(step1B): add immutable DayKey value type`
    * [x] Must include: `DayKey` type only.
    * [x] Must exclude: factory helpers and task sorting logic.

### 1C - Add `RuleId` and `SubjectId` value types ###

    * [x] Action: add immutable `RuleId` and `SubjectId` wrappers for stable identity composition.
    * [x] Scope: `Domain/Identifiers/RuleId.cs`, `Domain/Identifiers/SubjectId.cs` (`RuleId`, `SubjectId`).
    * [x] Verify: build succeeds; wrappers expose explicit canonical token forms.
    * [x] Commit message: `phase2(step1C): add RuleId and SubjectId value wrappers`
    * [x] Must include: value wrappers only.
    * [x] Must exclude: task object/enums/factory behavior.

## 2) Add Core Task Vocabulary Enums ##

Step goal:

    * [x] Define V1 domain vocabulary as explicit enums with bounded scope.

### 2A - Add `TaskStatus` ###

    * [x] Action: add V1-minimal status enum with `Incomplete` and `Completed` only.
    * [x] Scope: `Domain/Tasks/TaskStatus.cs` (`TaskStatus`).
    * [x] Verify: no extra V2 statuses are introduced.
    * [x] Commit message: `phase2(step2A): add v1 TaskStatus enum`
    * [x] Must include: status enum only.
    * [x] Must exclude: status transition logic.

### 2B - Add `TaskCategory` ###

    * [x] Action: add fixed V1 categories as internal enum.
    * [x] Scope: `Domain/Tasks/TaskCategory.cs` (`TaskCategory`).
    * [x] Verify: categories compile and are stable for downstream sorting/filtering.
    * [x] Commit message: `phase2(step2B): add fixed v1 TaskCategory enum`
    * [x] Must include: category enum only.
    * [x] Must exclude: UI grouping/render logic.

### 2C - Add `TaskSourceType` ###

    * [x] Action: add source enum used by core task identity and source partitioning.
    * [x] Scope: `Domain/Tasks/TaskSourceType.cs` (`TaskSourceType`).
    * [x] Verify: source values support deterministic prefix partitioning.
    * [x] Commit message: `phase2(step2C): add TaskSourceType enum`
    * [x] Must include: enum definition only.
    * [x] Must exclude: task-type ordering map/comparer implementations.

## 3) Implement Immutable `TaskObject` Domain Model ##

Step goal:

    * [x] Introduce the canonical task data container from Section 4 without engine/store logic.

### 3A - Add `TaskObject` shell with conceptual fields ###

    * [x] Action: create immutable `TaskObject` type with explicit fields for `TaskId`, `TaskCategory`, `TaskSourceType`, title/description, `TaskStatus`, progress/target, creation day, completion day, and source identifier.
    * [x] Scope: `Domain/Tasks/TaskObject.cs` (`TaskObject`).
    * [x] Verify: type compiles and reflects Section 4.2 field model without introducing State Store concerns.
    * [x] Commit message: `phase2(step3A): add immutable TaskObject core fields`
    * [x] Must include: domain fields and constructor signature only.
    * [x] Must exclude: evaluator/store/persistence wiring.

### 3B - Add constructor invariants and domain guards ###

    * [x] Action: enforce invariants for null/empty strings, negative progress, target/progress consistency, and completion-state compatibility.
    * [x] Scope: `Domain/Tasks/TaskObject.cs` (`TaskObject`).
    * [x] Verify: invalid inputs fail fast without side effects.
    * [x] Commit message: `phase2(step3B): enforce TaskObject constructor invariants`
    * [x] Must include: guard clauses only.
    * [x] Must exclude: status transition workflow logic.

## 4) Add Deterministic TaskId Construction and Format Utilities ##

Step goal:

    * [x] Centralize deterministic TaskId composition for built-in/task-builder forms and canonical parsing.

### 4A - Add `TaskIdFactory` for built-in and task-builder IDs ###

    * [x] Action: add explicit deterministic constructors for built-in and task-builder task ID composition.
    * [x] Scope: `Domain/Identifiers/TaskIdFactory.cs` (`TaskIdFactory`).
    * [x] Verify: repeated inputs produce identical IDs; source prefixes remain collision-safe.
    * [x] Commit message: `phase2(step4A): add deterministic built-in and task-builder TaskId constructors`
    * [x] Must include: construction API for built-in/task-builder only.
    * [x] Must exclude: manual counter issuance/storage behavior (Phase 3).

### 4B - Add canonical `TaskId` parser/formatter ###

    * [x] Action: add centralized formatting and `TryParse` helper for canonical TaskId token handling, including manual ID shape validation without issuing manual IDs.
    * [x] Scope: `Domain/Identifiers/TaskIdFormat.cs` (`TaskIdFormat`).
    * [x] Verify: format/parse round-trip is stable for canonical forms.
    * [x] Commit message: `phase2(step4B): add canonical TaskId format parser`
    * [x] Must include: parser/formatter only.
    * [x] Must exclude: engine reconciliation or migration handling.

### 4C - Document Phase 3 defer for manual ID issuance ###

    * [x] Action: add a clear defer note in checklist/audit notes that manual task ID issuance is owned by Phase 3 State Store command flow.
    * [x] Scope: this checklist file and optional existing defer-note file.
    * [x] Verify: no Phase 2 commit introduces manual counter state ownership.
    * [x] Commit message: `phase2(step4C): document manual id issuance defer to phase3`
    * [x] Must include: defer-note updates only.
    * [x] Must exclude: production code changes.

### 4D - Document Phase 3 defer for translation-impacting implementation ###

    * [x] Action: add an explicit defer note that translation handling changes impacting runtime
    behavior are owned by Phase 3+.
    * [x] Scope: this checklist file and optional existing defer-note file.
    * [x] Verify: Phase 2 commits do not introduce translation plumbing that changes behavior.
    * [x] Commit message: `phase2(step4D): defer translation-impacting implementation to phase3+`
    * [x] Must include: defer-note updates only.
    * [x] Must exclude: production code changes.

## 5) Add DayKey Construction Utilities ##

Step goal:

    * [x] Provide canonical day-key builder utilities reused by later phases.

### 5A - Add `DayKeyFactory` ###

    * [x] Action: add deterministic day-key constructor for canonical `Year{N}-{Season}{D}` format using fixed non-localized season tokens and invariant casing.
    * [x] Scope: `Domain/Identifiers/DayKeyFactory.cs` (`DayKeyFactory`).
    * [x] Verify: outputs match canonical format across repeat runs.
    * [x] Commit message: `phase2(step5A): add deterministic DayKey factory`
    * [x] Must include: construction/validation logic only.
    * [x] Must exclude: history ledger capture logic.

## 6) Defer Deterministic Task-Type Sorting/Comparison Helper ##

Step goal:

    * [x] Document explicit defer of task-type sorting/comparer implementation to later phase.

### 6A - Document defer for deterministic task-type sorting comparer ###

    * [x] Action: document that deterministic task-type ordering (derived map + fallback chain: `TaskCreationDay`, then canonical `TaskId`) is deferred to Phase 5+ when generator/task-type coverage is stable.
    * [x] Scope: this checklist file and optional defer-note artifact.
    * [x] Verify: no Phase 2 commit adds task sorting comparer implementation.
    * [x] Commit message: `phase2(step6A): defer task-type ordering comparer to phase5+`
    * [x] Must include: defer-note updates only.
    * [x] Must exclude: comparer implementation, ordering map code, or snapshot ordering logic.

## 7) Add Phase 2 Verification Tests ##

Step goal:

    * [x] Lock deterministic identity and domain-model invariants before Phase 3.

### 7A - Add identifier value-type tests ###

    * [x] Action: add tests for equality, normalization, and invalid input handling in `TaskId`, `DayKey`, `RuleId`, `SubjectId`.
    * [x] Scope: `Tests/Domain/Identifiers/TaskIdTests.cs`, `Tests/Domain/Identifiers/DayKeyTests.cs`, `Tests/Domain/Identifiers/RuleIdTests.cs`, `Tests/Domain/Identifiers/SubjectIdTests.cs`.
    * [x] Verify: tests fail when deterministic equality/validation regresses, including `DayKey` canonical shape `Year{N}-{Season}{D}`, strict season token casing, and `RuleId`/`SubjectId` ordinal case-sensitive equality.
    * [x] Commit message: `phase2(step7A): add deterministic identifier value-type tests`
    * [x] Must include: tests and minimal fixtures.
    * [x] Must exclude: production behavior refactors outside identifiers.

### 7B - Add TaskObject invariant tests ###

    * [x] Action: add tests for constructor guards and legal state combinations.
    * [x] Scope: `Tests/Domain/Tasks/TaskObjectTests.cs`.
    * [x] Verify: invalid combinations fail fast and legal combinations pass.
    * [x] Commit message: `phase2(step7B): add TaskObject invariant tests`
    * [x] Must include: domain tests only.
    * [x] Must exclude: store/evaluator logic tests.

### 7C - Add deterministic TaskId factory/parser tests ###

    * [x] Action: add repeatability/collision and round-trip tests for canonical built-in/task-builder/manual ID string forms.
    * [x] Scope: `Tests/Domain/Identifiers/TaskIdFactoryTests.cs`, `Tests/Domain/Identifiers/TaskIdFormatTests.cs`.
    * [x] Verify: identical inputs produce identical outputs and parse/format remains stable.
    * [x] Commit message: `phase2(step7C): add deterministic TaskId factory and parser tests`
    * [x] Must include: deterministic ID tests only.
    * [x] Must exclude: persistence counter storage tests.

### 7D - Add reconstruction-stability seam tests ###

    * [x] Action: add tests that reconstruct IDs/day-keys from canonical serialized forms and assert stable equality/order semantics across reconstruction paths.
    * [x] Scope: `Tests/Domain/Identifiers/IdentifierReconstructionStabilityTests.cs`.
    * [x] Verify: reconstruction cannot change canonical identity.
    * [x] Commit message: `phase2(step7D): add identifier reconstruction stability tests`
    * [x] Must include: seam stability tests only.
    * [x] Must exclude: full save/load integration tests (Phase 7).

### 7E - Add DayKey factory tests ###

    * [x] Action: add tests for canonical format, token casing, and invalid date-part handling.
    * [x] Scope: `Tests/Domain/Identifiers/DayKeyFactoryTests.cs`.
    * [x] Verify: canonical and invalid cases are both covered.
    * [x] Commit message: `phase2(step7E): add DayKey factory determinism tests`
    * [x] Must include: day-key tests only.
    * [x] Must exclude: history ledger integration.

### 7F - Document deferred task-type ordering comparer tests ###

    * [x] Action: document that deterministic task-type ordering comparer tests are deferred with the comparer implementation to Phase 5+; Step 7F remains docs-only in this phase.
    * [x] Scope: this checklist file and optional test planning note.
    * [x] Verify: Phase 2 test scope excludes task-type ordering comparer tests, and no comparer tests are implemented in Step 7.
    * [x] Commit message: `phase2(step7F): defer task-type ordering comparer tests to phase5+`
    * [x] Must include: defer-note updates only.
    * [x] Must exclude: comparer test implementation.

### 7G - Add phase-boundary tests/audit checks ###

    * [x] Action: add lightweight boundary assertions that domain layer stays independent of UI/store/persistence concerns.
    * [x] Scope: `Tests/Domain/Phase2BoundaryTests.cs`.
    * [x] Verify: boundary tests fail when forbidden namespaces appear in type metadata/signatures.
    * [x] Commit message: `phase2(step7G): add phase2 boundary guard tests`
    * [x] Must include: boundary tests only.
    * [x] Must exclude: broad architecture rewrites.

## 8) Phase 2 Completion Gate ##

Step goal:

    * [x] Confirm Phase 2 is complete, atomic, deterministic, and contract-aligned.

### 8A - Run clean build and full tests ###

    * [x] Action: run clean build and full test suite, including existing Phase 1 regressions.
    * [x] Scope: no source edits expected.
    * [x] Verify: build succeeds; all tests pass.
    * [x] Commit message: `phase2(step8A): record phase2 build and test evidence` (marked
    complete per user-audited commit/message waiver).
    * [x] Must include: durable evidence/docs only if maintained in repo.
    * [x] Must exclude: opportunistic production edits.
    * [x] Command outcomes (2026-03-08):
        * [x] `dotnet clean JojaAutoTasks.csproj -c Debug -p:EnableModDeploy=false -p:EnableModZip=false` succeeded.
        * [x] `dotnet build JojaAutoTasks.csproj -c Debug -p:EnableModDeploy=false -p:EnableModZip=false` succeeded.
        * [x] `dotnet test "Tests\\JojaAutoTasks.Tests.csproj" -c Debug` succeeded (110 passed,
        0 failed, 0 skipped).
        * [x] `dotnet test "Tests\\JojaAutoTasks.Tests.csproj" -c Debug --filter "FullyQualifiedName~UpdateTickedGuardTests"` succeeded (3 passed,
        0 failed, 0 skipped).

### 8B - Complete checklist and contract audit ###

    * [x] Action: audit against this checklist and confirm Section 3/4/21 + workspace/backend/testing/style contract compliance.
    * [x] Scope: this checklist file and optional audit notes.
    * [x] Verify: no UI/store/persistence drift; determinism guarantees remain explicit.
    * [x] Evidence map (Section/contract -> proof points):
        * [x] Section 3 deterministic identifier model: Steps `1A-1C`, `4A-4B`, `5A`; tests
        `7A`, `7C`, `7D`, `7E` (`Domain/Identifiers/*.cs`,
        `Tests/Domain/Identifiers/*Tests.cs`).
        * [x] Section 4 task domain model: Steps `3A-3B`; test `7B`
        (`Domain/Tasks/TaskObject.cs`, `Tests/Domain/Tasks/TaskObjectTests.cs`).
        * [x] Section 21 (including 21.5 core domain types): Steps `1A-5A` and tests `7A-7G`;
        corroborated by the Final Completion Gate item for Section 21.5 coverage.
        * [x] Workspace contract proof: guardrails and phase-scope clauses remain enforced in
        this checklist; atomic commit-boundary clauses remain completed/waived per explicit user
        direction in Step `8C`.
        * [x] Backend architecture contract proof: Step `8B` no UI/store/persistence drift
        verification and Step `7G` boundary guard tests
        (`Tests/Domain/Phase2BoundaryTests.cs`).
        * [x] Testing contract proof: Steps `7A-7G` deterministic/invariant/boundary tests and
        Step `8A` full-suite evidence (`dotnet test ...` succeeded, 110 passed).
        * [x] Style contract proof: Phase 2 touched C# artifacts audited in Step `8B` against
        `CSHARP-STYLE-CONTRACT.instructions.md`.
        * [x] Phase 1 safeguard guard-test proof:
        `Tests/Lifecycle/LifecycleCoordinatorTests.cs` (`OnSaving` signal-only forwarding) and
        `Tests/Hooks/UpdateTickedGuardTests.cs` (throttled `OnUpdateTicked` guard), plus Step
        `8A` targeted `UpdateTickedGuardTests` execution (3 passed).
    * [x] Commit message: `phase2(step8B): complete phase2 contract and scope audit` (marked
    complete per user-audited commit/message waiver).
    * [x] Must include: checklist/audit updates only.
    * [x] Must exclude: Phase 3 feature work.

### 8C - Validate atomic boundaries and create Phase 3 defer list ###

    * [x] Action: verify each completed sub-step maps to one atomic commit and list explicitly
    deferred Phase 3 work (commit-boundary verification treated as completed/waived per explicit
    user direction).
    * [x] Scope: checklist updates and optional status note files.
    * [x] Verify: no commit crosses unrelated file/symbol scopes (treated as completed/waived per
    explicit user direction).
    * [x] Commit message: `phase2(step8C): finalize phase2 atomic boundaries and phase3 defer list`
    (marked complete per user-audited commit/message waiver).
    * [x] Must include: final checklist status and defer notes.
    * [x] Must exclude: state-store implementation.

## Final Completion Gate Checklist ##

Closure note: Guardrails above are continuous constraints audited throughout Phase 2 and
re-checked in Step `8B`; completion is determined by Step `8A-8C` evidence plus this final gate,
not by converting every guardrail checkbox to a completion marker.

    * [x] All sub-steps `1A` through `8C` are completed or explicitly deferred.
    * [x] Every completed sub-step has a matching atomic commit (treated as completed/waived per
    explicit user direction: commit messages/history audited by user).
    * [x] No commit crosses unrelated file/symbol scopes (treated as completed/waived per explicit
    user direction: commit messages/history audited by user).
    * [x] Core domain types from Section 21.5 are implemented and test-covered.
    * [x] Deterministic TaskId/DayKey rules are implemented and regression-tested.
    * [x] No random/GUID/non-deterministic identity paths exist.
    * [x] `TaskStatus` remains V1-minimal (`Incomplete`, `Completed`).
    * [x] Deterministic task-type ordering/comparer implementation and tests are explicitly deferred to Phase 5+.
    * [x] No UI, State Store, or persistence logic was introduced.
    * [x] Manual task ID issuance remains deferred to Phase 3 State Store command flow.
    * [x] Completion-marking runtime behavior remains deferred until State Store command flow exists.
    * [x] RuleId sequential-generation enforcement remains deferred until RuleId generation exists.
    * [x] Translation-impacting implementation remains deferred to Phase 3+.
    * [x] UpdateTicked guard seam fix remains test-only and passing.
    * [x] Build/tests pass and Phase 1 safeguards remain intact (proof:
    `Tests/Lifecycle/LifecycleCoordinatorTests.cs`,
    `Tests/Hooks/UpdateTickedGuardTests.cs`, and Step `8A` targeted guard-test run).
