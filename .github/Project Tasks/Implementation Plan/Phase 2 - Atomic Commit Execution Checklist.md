# Phase 2 - Atomic Commit Execution Checklist #

Purpose: execute Phase 2 (Core Domain Model) in strict, linear, atomic commits with explicit
file/symbol scope.

Note: If a sub-step expands beyond the stated file scope, stop and open a follow-up commit.

## Guardrails (Must Stay True) ##

    * [ ] Phase 2 only.
    * [ ] Preserve all Phase 1 invariants (`OnSaving` signal-only, throttled `OnUpdateTicked`, bounded logging).
    * [ ] No State Store command/reducer/snapshot logic in this phase.
    * [ ] No UI/HUD/Menu/ViewModel/StarML work in this phase.
    * [ ] No persistence schema/migration work in this phase.
    * [ ] Localization changes that alter runtime behavior are deferred to Phase 3+; Phase 2 may
    only add documentation/contract guardrails.
    * [ ] Manual task ID issuance/counter ownership is deferred to Phase 3 (State Store path).
    * [ ] Deterministic IDs only; no random/GUID/time-seeded identity generation.
    * [ ] `TaskStatus` is V1-minimal (`Incomplete`, `Completed`) only.
    * [ ] Sorting/comparison is deterministic and cannot depend on unordered traversal or insertion order.
    * [ ] DayKey uses fixed non-localized season tokens with invariant casing.
    * [ ] All new C# files/types satisfy `CSHARP-STYLE-CONTRACT.instructions.md`.

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

### 1C - Add `RuleID` and `SubjectID` value types ###

    * [x] Action: add immutable `RuleID` and `SubjectID` wrappers for stable identity composition.
    * [x] Scope: `Domain/Identifiers/RuleID.cs`, `Domain/Identifiers/SubjectID.cs` (`RuleID`, `SubjectID`).
    * [x] Verify: build succeeds; wrappers expose explicit canonical token forms.
    * [x] Commit message: `phase2(step1C): add RuleID and SubjectID value wrappers`
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

    * [ ] Introduce the canonical task data container from Section 4 without engine/store logic.

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

    * [ ] Centralize deterministic TaskId composition for built-in/task-builder forms and canonical parsing.

### 4A - Add `TaskIdFactory` for built-in and task-builder IDs ###

    * [x] Action: add explicit deterministic constructors for built-in and task-builder task ID composition.
    * [x] Scope: `Domain/Identifiers/TaskIdFactory.cs` (`TaskIdFactory`).
    * [x] Verify: repeated inputs produce identical IDs; source prefixes remain collision-safe.
    * [x] Commit message: `phase2(step4A): add deterministic built-in and task-builder TaskId constructors`
    * [x] Must include: construction API for built-in/task-builder only.
    * [x] Must exclude: manual counter issuance/storage behavior (Phase 3).

### 4B - Add canonical `TaskId` parser/formatter ###

    * [ ] Action: add centralized formatting and `TryParse` helper for canonical TaskId token handling, including manual ID shape validation without issuing manual IDs.
    * [ ] Scope: `Domain/Identifiers/TaskIdFormat.cs` (`TaskIdFormat`).
    * [ ] Verify: format/parse round-trip is stable for canonical forms.
    * [ ] Commit message: `phase2(step4B): add canonical TaskId format parser`
    * [ ] Must include: parser/formatter only.
    * [ ] Must exclude: engine reconciliation or migration handling.

### 4C - Document Phase 3 defer for manual ID issuance ###

    * [ ] Action: add a clear defer note in checklist/audit notes that manual task ID issuance is owned by Phase 3 State Store command flow.
    * [ ] Scope: this checklist file and optional existing defer-note file.
    * [ ] Verify: no Phase 2 commit introduces manual counter state ownership.
    * [ ] Commit message: `phase2(step4C): document manual id issuance defer to phase3`
    * [ ] Must include: defer-note updates only.
    * [ ] Must exclude: production code changes.

### 4D - Document Phase 3 defer for translation-impacting implementation ###

    * [ ] Action: add an explicit defer note that translation handling changes impacting runtime
    behavior are owned by Phase 3+.
    * [ ] Scope: this checklist file and optional existing defer-note file.
    * [ ] Verify: Phase 2 commits do not introduce translation plumbing that changes behavior.
    * [ ] Commit message: `phase2(step4D): defer translation-impacting implementation to phase3+`
    * [ ] Must include: defer-note updates only.
    * [ ] Must exclude: production code changes.

## 5) Add DayKey Construction Utilities ##

Step goal:

    * [ ] Provide canonical day-key builder utilities reused by later phases.

### 5A - Add `DayKeyFactory` ###

    * [ ] Action: add deterministic day-key constructor for canonical `Year{N}_{Season}{D}` format using fixed non-localized season tokens and invariant casing.
    * [ ] Scope: `Domain/Identifiers/DayKeyFactory.cs` (`DayKeyFactory`).
    * [ ] Verify: outputs match canonical format across repeat runs.
    * [ ] Commit message: `phase2(step5A): add deterministic DayKey factory`
    * [ ] Must include: construction/validation logic only.
    * [ ] Must exclude: history ledger capture logic.

## 6) Defer Deterministic Task-Type Sorting/Comparison Helper ##

Step goal:

    * [ ] Document explicit defer of task-type sorting/comparer implementation to later phase.

### 6A - Document defer for deterministic task-type sorting comparer ###

    * [ ] Action: document that deterministic task-type ordering (derived map + fallback chain: `TaskCreationDay`, then canonical `TaskId`) is deferred to Phase 5+ when generator/task-type coverage is stable.
    * [ ] Scope: this checklist file and optional defer-note artifact.
    * [ ] Verify: no Phase 2 commit adds task sorting comparer implementation.
    * [ ] Commit message: `phase2(step6A): defer task-type ordering comparer to phase5+`
    * [ ] Must include: defer-note updates only.
    * [ ] Must exclude: comparer implementation, ordering map code, or snapshot ordering logic.

## 7) Add Phase 2 Verification Tests ##

Step goal:

    * [ ] Lock deterministic identity and domain-model invariants before Phase 3.

### 7A - Add identifier value-type tests ###

    * [ ] Action: add tests for equality, normalization, and invalid input handling in `TaskId`, `DayKey`, `RuleID`, `SubjectID`.
    * [ ] Scope: `Tests/Domain/Identifiers/TaskIdTests.cs`, `Tests/Domain/Identifiers/DayKeyTests.cs`, `Tests/Domain/Identifiers/RuleIDTests.cs`, `Tests/Domain/Identifiers/SubjectIDTests.cs`.
    * [ ] Verify: tests fail when deterministic equality/validation regresses.
    * [ ] Commit message: `phase2(step7A): add deterministic identifier value-type tests`
    * [ ] Must include: tests and minimal fixtures.
    * [ ] Must exclude: production behavior refactors outside identifiers.

### 7B - Add TaskObject invariant tests ###

    * [ ] Action: add tests for constructor guards and legal state combinations.
    * [ ] Scope: `Tests/Domain/Tasks/TaskObjectTests.cs`.
    * [ ] Verify: invalid combinations fail fast and legal combinations pass.
    * [ ] Commit message: `phase2(step7B): add TaskObject invariant tests`
    * [ ] Must include: domain tests only.
    * [ ] Must exclude: store/evaluator logic tests.

### 7C - Add deterministic TaskId factory/parser tests ###

    * [ ] Action: add repeatability/collision and round-trip tests for canonical built-in/task-builder/manual ID string forms.
    * [ ] Scope: `Tests/Domain/Identifiers/TaskIdFactoryTests.cs`, `Tests/Domain/Identifiers/TaskIdFormatTests.cs`.
    * [ ] Verify: identical inputs produce identical outputs and parse/format remains stable.
    * [ ] Commit message: `phase2(step7C): add deterministic TaskId factory and parser tests`
    * [ ] Must include: deterministic ID tests only.
    * [ ] Must exclude: persistence counter storage tests.

### 7D - Add reconstruction-stability seam tests ###

    * [ ] Action: add tests that reconstruct IDs/day-keys from canonical serialized forms and assert stable equality/order semantics across reconstruction paths.
    * [ ] Scope: `Tests/Domain/Identifiers/IdentifierReconstructionStabilityTests.cs`.
    * [ ] Verify: reconstruction cannot change canonical identity.
    * [ ] Commit message: `phase2(step7D): add identifier reconstruction stability tests`
    * [ ] Must include: seam stability tests only.
    * [ ] Must exclude: full save/load integration tests (Phase 7).

### 7E - Add DayKey factory tests ###

    * [ ] Action: add tests for canonical format, token casing, and invalid date-part handling.
    * [ ] Scope: `Tests/Domain/Identifiers/DayKeyFactoryTests.cs`.
    * [ ] Verify: canonical and invalid cases are both covered.
    * [ ] Commit message: `phase2(step7E): add DayKey factory determinism tests`
    * [ ] Must include: day-key tests only.
    * [ ] Must exclude: history ledger integration.

### 7F - Document deferred task-type ordering comparer tests ###

    * [ ] Action: document that deterministic task-type ordering comparer tests are deferred with the comparer implementation to Phase 5+.
    * [ ] Scope: this checklist file and optional test planning note.
    * [ ] Verify: Phase 2 test scope excludes task-type ordering comparer tests.
    * [ ] Commit message: `phase2(step7F): defer task-type ordering comparer tests to phase5+`
    * [ ] Must include: defer-note updates only.
    * [ ] Must exclude: comparer test implementation.

### 7G - Add phase-boundary tests/audit checks ###

    * [ ] Action: add lightweight boundary assertions that domain layer stays independent of UI/store/persistence concerns.
    * [ ] Scope: `Tests/Domain/Phase2BoundaryTests.cs`.
    * [ ] Verify: tests fail if forbidden dependencies are introduced.
    * [ ] Commit message: `phase2(step7G): add phase2 boundary guard tests`
    * [ ] Must include: boundary tests only.
    * [ ] Must exclude: broad architecture rewrites.

## 8) Phase 2 Completion Gate ##

Step goal:

    * [ ] Confirm Phase 2 is complete, atomic, deterministic, and contract-aligned.

### 8A - Run clean build and full tests ###

    * [ ] Action: run clean build and full test suite, including existing Phase 1 regressions.
    * [ ] Scope: no source edits expected.
    * [ ] Verify: build succeeds; all tests pass.
    * [ ] Commit message: `phase2(step8A): record phase2 build and test evidence`
    * [ ] Must include: durable evidence/docs only if maintained in repo.
    * [ ] Must exclude: opportunistic production edits.

### 8B - Complete checklist and contract audit ###

    * [ ] Action: audit against this checklist and confirm Section 3/4/21 + workspace/backend/testing/style contract compliance.
    * [ ] Scope: this checklist file and optional audit notes.
    * [ ] Verify: no UI/store/persistence drift; determinism guarantees remain explicit.
    * [ ] Commit message: `phase2(step8B): complete phase2 contract and scope audit`
    * [ ] Must include: checklist/audit updates only.
    * [ ] Must exclude: Phase 3 feature work.

### 8C - Validate atomic boundaries and create Phase 3 defer list ###

    * [ ] Action: verify each completed sub-step maps to one atomic commit and list explicitly deferred Phase 3 work.
    * [ ] Scope: checklist updates and optional status note files.
    * [ ] Verify: no commit crosses unrelated file/symbol scopes.
    * [ ] Commit message: `phase2(step8C): finalize phase2 atomic boundaries and phase3 defer list`
    * [ ] Must include: final checklist status and defer notes.
    * [ ] Must exclude: state-store implementation.

## Final Completion Gate Checklist ##

    * [ ] All sub-steps `1A` through `8C` are completed or explicitly deferred.
    * [ ] Every completed sub-step has a matching atomic commit.
    * [ ] No commit crosses unrelated file/symbol scopes.
    * [ ] Core domain types from Section 21.5 are implemented and test-covered.
    * [ ] Deterministic TaskId/DayKey rules are implemented and regression-tested.
    * [ ] No random/GUID/non-deterministic identity paths exist.
    * [ ] `TaskStatus` remains V1-minimal (`Incomplete`, `Completed`).
    * [ ] Deterministic task-type ordering/comparer implementation and tests are explicitly deferred to Phase 5+.
    * [ ] No UI, State Store, or persistence logic was introduced.
    * [ ] Manual task ID issuance remains deferred to Phase 3 State Store command flow.
    * [ ] Translation-impacting implementation remains deferred to Phase 3+.
    * [ ] Build/tests pass and Phase 1 safeguards remain intact.
