# Phase 4 Implementation Plan - Quick PRD

Phase 4 requires replacing a deficient existing Atomic Execution Checklist with a comprehensive, implementation-ready plan grounded in an explicit PRD. The core deliverable is a validated ViewModel pipeline that introduces a base INPC-enabled ViewModel (PropertyChanged.SourceGenerator), implements HudViewModel, TaskListViewModel, and ConfigViewModel, and projects SnapshotChanged data into bindable properties while preserving deterministic State Store ownership and command/snapshot boundaries.

Implementation must align with the Joja AutoTasks Design Guide, especially Section 10 and Section 10A, and follow architecture and style contracts. ViewModels are required to live under a dedicated UI ViewModels folder with namespace JojaAutotasks.Ui.ViewModels, must avoid direct game API dependencies, and must support command dispatch back to the State Store. Phase 4 scope also explicitly incorporates carryover issue coverage for #100, #106, #107, #108, #109, and #159 (with #86 merged into #159).

Locked refinement decisions for this PRD are: `ViewModelBase` as the base type, `ApplySnapshot(...)` as the projection entrypoint, `Execute<Verb><Target>(...)` command-method naming, and standard bool/collection property naming (`Is/Has/Can`, plural collections). Unit tests are structured under `Tests/UI/ViewModels/` with `<ViewModelType>Tests` classes and required coverage for snapshot projection correctness, INPC notifications, and command dispatch forwarding, all without game runtime dependencies. Out of scope for this phase includes StarML layout work, gameplay/rule-generation features outside the ViewModel pipeline, broad visual redesign, and production-hardening work beyond the listed carryover issues; the user will author production code and use AI support to generate unit test code.

---

_Generated with Clavix Planning Mode_
_Generated: 2026-03-15T00:00:00Z_
