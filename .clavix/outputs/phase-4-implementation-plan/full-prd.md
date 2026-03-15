# Product Requirements Document: Phase 4 Implementation Plan

## Problem & Goal

The existing Phase 4 Atomic Execution Checklist is deficient and does not provide enough confidence that implementation scope, architecture boundaries, and validation criteria are complete for this stage of Joja AutoTasks. The goal is to create a new, comprehensive Phase 4 Implementation Plan by first defining a clear PRD that is complete, architecture-aligned, and directly usable for implementation planning.

Phase 4 introduces the ViewModel layer defined in Design Guide Section 10 and Section 10A. This phase must validate the State Store -> ViewModel -> UI binding pipeline before any StarML layout work begins, and ViewModels must be testable without a running game.

## Requirements

### Must-Have Features

1. Implement a base ViewModel with INPC using PropertyChanged.SourceGenerator.
2. Implement HudViewModel and TaskListViewModel as initial ViewModel consumers.
3. Include ConfigViewModel within the expanded, approved Phase 4 ViewModel scope.
4. Subscribe to SnapshotChanged (Section 8.12) and project snapshot data into bindable properties.
5. Establish the command dispatch pattern from ViewModels back to the State Store.

### Technical Requirements

- Technology baseline: C# on .NET 6 in existing JojaAutoTasks architecture.
- INPC implementation: PropertyChanged.SourceGenerator for bindable property updates.
- Integration alignment: preserve State Store ownership and command/snapshot boundaries while wiring SnapshotChanged to ViewModel projections.
- Architectural compliance: align all implementation decisions with:
  - Project/Planning/Joja AutoTasks Design Guide/JojaAutoTasks Design Guide.md
  - Project/Planning/Joja AutoTasks Design Guide/Section 10 - UI Data Binding Model.md
  - Project/Planning/Joja AutoTasks Design Guide/Section 10A - View Model Architecture.md
- Contract compliance: respect backend and frontend architecture contracts, C# style contract, external resources contract, and applicable UI pattern contracts.
- Carryover issue incorporation (starting in Phase 4 ImplementationIssues):
  - #100 localization/translation runtime behavior (retargeted to Phase 4)
  - #106 terminology clarification
  - #107 snapshot subscription wiring
  - #108 INPC projection behavior
  - #109 UI-local state ownership boundaries
  - #159 config loader exception hardening (with #86 treated as duplicate and merged into #159)

### Architecture & Design

- Folder and namespace convention:
  - ViewModels live in a dedicated folder under UI.
  - Namespace is JojaAutotasks.Ui.ViewModels.
- Boundary rule:
  - No direct game API usage in ViewModels.
- Pattern decisions (locked for this phase):
  - ViewModels act as projection adapters from immutable State Store snapshots to bindable INPC properties.
  - Each ViewModel exposes a single snapshot entrypoint named `ApplySnapshot(...)`.
  - Command actions stay explicit and route back through command dispatch, never through direct state mutation.
  - ViewModels depend only on project abstractions and DTO/snapshot models needed for projection.
- Naming conventions (locked for this phase):
  - Base class name is `ViewModelBase`.
  - ViewModel type names use `<Feature>ViewModel` (for example: `HudViewModel`, `TaskListViewModel`, `ConfigViewModel`).
  - Projection entrypoint is `ApplySnapshot(...)`.
  - Command methods use `Execute<Verb><Target>(...)` naming.
  - Boolean properties use `Is`, `Has`, or `Can` prefixes.
  - Collection properties use plural nouns.
- Testability requirement:
  - ViewModels must remain unit-testable without game runtime.
  - User will write production code; AI assistance is explicitly expected for unit-test code authoring support.
  - Unit test organization for this phase:
    - Tests live under `Tests/UI/ViewModels/`.
    - Test class naming uses `<ViewModelType>Tests`.
    - Required coverage includes snapshot projection correctness, INPC notification behavior, and command dispatch forwarding behavior.
    - Tests must use test doubles/fakes and deterministic snapshot data, with no direct game runtime dependency.

## Out of Scope

1. StarML layout implementation in Phase 4 (binding pipeline must be validated first).
2. New gameplay or task-rule generation features outside the ViewModel pipeline scope.
3. Broad UI visual redesign beyond required initial ViewModel consumers.
4. Production-hardening work beyond the explicitly listed carryover issues.

## Additional Context

No additional context supplied.

## Refinement History

### 2026-03-15

**Changes:**

- [ADDED] Locked Phase 4 ViewModel pattern decisions (`ApplySnapshot(...)`, projection-adapter role, dispatch-only command flow).
- [ADDED] Locked naming conventions for base class, ViewModel types, command methods, boolean/collection properties.
- [ADDED] Locked unit-test structure and minimum coverage targets for ViewModels.
- [UNCHANGED] Core Phase 4 scope, carryover issue list, architectural contract alignment, and out-of-scope boundaries.

**Why:**

- Converted open-ended collaboration items into concrete planning constraints so `/clavix-plan` can produce unambiguous implementation tasks.

---

_Generated with Clavix Planning Mode_
_Generated: 2026-03-15T00:00:00Z_
