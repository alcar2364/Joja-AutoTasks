# Context Handoff: Design-Guide-to-Implementation-Plan Alignment

Date: 2026-03-14 Scope: Documentation planning/execution context for keeping design-guide changes synchronized with implementation plan artifacts (Section 21, ImplementationIssues, Phase 4 checklist, and future checklist generation guidance).

## 1) Governing Sources and Precedence

1. Design guide is canonical source of truth.
   - `Project/Planning/Joja AutoTasks Design Guide/JojaAutoTasks Design Guide.md:30`
   - `Project/Planning/Joja AutoTasks Design Guide/JojaAutoTasks Design Guide.md:31`
2. Section 21 defines planning precedence and conflict handling.
   - `Project/Planning/Joja AutoTasks Design Guide/Section 21 - Implementation Plan.md:47`
   - `Project/Planning/Joja AutoTasks Design Guide/Section 21 - Implementation Plan.md:51`
   - `Project/Planning/Joja AutoTasks Design Guide/Section 21 - Implementation Plan.md:54`
3. Workflow contract is plan-first and scope-disciplined.
   - `Project/Planning/Workflows/initial-workflow.md:1`
   - `.github/instructions/WORKSPACE-CONTRACTS.instructions.md:34`
   - `.github/instructions/WORKSPACE-CONTRACTS.instructions.md:64`

## 2) Workflow File Mapping (Requested A-I Sequence)

Requested names vs workspace files:

- `initial-workflow` -> `Project/Planning/Workflows/initial-workflow.md`
- `workflow-brief` -> `Project/Planning/Workflows/workflow-brief.md`
- `core-flows` -> `Project/Planning/Workflows/core-flows.md`
- `pre-validation` -> `Project/Planning/Workflows/prd-validation.md` (workspace naming)
- `tech-plan` -> `Project/Planning/Workflows/tech-plan.md`
- `architecture-validation` -> `Project/Planning/Workflows/architecture-validation.md`
- `cross-artifact-validation` -> `Project/Planning/Workflows/cross-artifact-validation.md`
- `ticket-breakdown` -> `Project/Planning/Workflows/ticket-breakdown.md`
- `execute.md` -> `Project/Planning/Workflows/execution.md` (workspace naming)
- `implementation-validation` -> `Project/Planning/Workflows/implementation-validation.md`

Evidence for workspace naming references:

- `Project/Planning/Workflows/cross-artifact-validation.md:8`
- `Project/Planning/Workflows/execution.md:155`

## 3) Section 21 Context Relevant to Phase 4 and Future Checklists

Phase 4 carryover explicitly includes:

- #100 retargeted to Phase 4
- #106, #107, #108, #109
- #159 with #86 duplicate/merged treatment

Citations:

- `Project/Planning/Joja AutoTasks Design Guide/Section 21 - Implementation Plan.md:255`
- `Project/Planning/Joja AutoTasks Design Guide/Section 21 - Implementation Plan.md:260`

Cross-cutting ownership mapping that constrains checklist scope:

- Section 20.8 toast routing -> Phase 9
- Section 20.10 onboarding -> Phase 8 with Phase 7 dependency

Citations:

- `Project/Planning/Joja AutoTasks Design Guide/Section 21 - Implementation Plan.md:304`
- `Project/Planning/Joja AutoTasks Design Guide/Section 21 - Implementation Plan.md:311`
- `Project/Planning/Joja AutoTasks Design Guide/Section 21 - Implementation Plan.md:312`

## 4) ImplementationIssues Pattern and Current Scheduling References

Index indicates:

- #159 is active, scheduled Phase 4, canonical active tracker for merged #86 scope.
- #100 is active, scheduled Phase 4, note aligned to Section 21 carryover.

Citations:

- `Project/Tasks/ImplementationPlan/ImplementationIssues/ImplementationIssuesIndex.md:21`
- `Project/Tasks/ImplementationPlan/ImplementationIssues/ImplementationIssuesIndex.md:23`

Record-level merged/scheduling metadata:

- #86 historical merged record, scheduled target points to merged status.
- #100 scheduled target Phase 4.
- #159 open active tracker for merged #86 scope.

Citations:

- `Project/Tasks/ImplementationPlan/ImplementationIssues/Records/issue-86.md:9`
- `Project/Tasks/ImplementationPlan/ImplementationIssues/Records/issue-86.md:10`
- `Project/Tasks/ImplementationPlan/ImplementationIssues/Records/issue-86.md:18`
- `Project/Tasks/ImplementationPlan/ImplementationIssues/Records/issue-100.md:9`
- `Project/Tasks/ImplementationPlan/ImplementationIssues/Records/issue-159.md:9`
- `Project/Tasks/ImplementationPlan/ImplementationIssues/Records/issue-159.md:10`

## 5) Phase 4 Checklist Pattern and Scope Boundaries

Checklist currently encodes:

- Target issue set (#159 incl. merged #86, #100, #106, #107, #108, #109)
- Explicit exclusions for toast/onboarding/gamepad verification from Phase 4
- Phase goal and architecture components tied to view-model infrastructure and snapshot flow

Citations:

- `Project/Tasks/ImplementationPlan/Phase 4 - Atomic Commit Execution Checklist.md:7`
- `Project/Tasks/ImplementationPlan/Phase 4 - Atomic Commit Execution Checklist.md:33`
- `Project/Tasks/ImplementationPlan/Phase 4 - Atomic Commit Execution Checklist.md:34`
- `Project/Tasks/ImplementationPlan/Phase 4 - Atomic Commit Execution Checklist.md:44`
- `Project/Tasks/ImplementationPlan/Phase 4 - Atomic Commit Execution Checklist.md:46`
- `Project/Tasks/ImplementationPlan/Phase 4 - Atomic Commit Execution Checklist.md:50`

## 6) Future Checklist Generation Guardrails (Instruction + Skill)

Instruction-level required checks include:

- Section 21 mapping consistency
- ImplementationIssues index/record scheduling consistency
- merged-duplicate single active tracker consistency

Citations:

- `.github/instructions/atomic-commit-execution-checklist-creation.instructions.md:413`
- `.github/instructions/atomic-commit-execution-checklist-creation.instructions.md:414`

Skill-level required checks mirror instruction:

- `.github/skills/atomic-commit-execution-checklist-creation/SKILL.md:27`
- `.github/skills/atomic-commit-execution-checklist-creation/SKILL.md:28`
- `.github/skills/atomic-commit-execution-checklist-creation/SKILL.md:29`
- `.github/skills/atomic-commit-execution-checklist-creation/SKILL.md:30`

## 7) Runtime Symbol Patterns That Back Planning Constraints

These symbols represent existing architecture boundaries referenced by planning docs:

1. StateStore is command/snapshot authority.
   - Symbol: `StateStore`
   - `State/StateStore.cs:11`
   - `State/StateStore.cs:35` (`Dispatch`)
   - `State/StateStore.cs:32` (`SnapshotChanged`)
2. UI subscription manager encapsulates snapshot subscription lifecycle.
   - Symbol: `UiSnapshotSubscriptionManager`
   - `UI/UiSnapshotSubscriptionManager.cs:6`
   - `UI/UiSnapshotSubscriptionManager.cs:42` (`Subscribe`)
3. Config loader catch-path target for issue #159.
   - Symbol: `ConfigLoader.Load`
   - `Configuration/ConfigLoader.cs:6`
   - `Configuration/ConfigLoader.cs:18`
   - `Configuration/ConfigLoader.cs:26` (`catch`)
4. Lifecycle forwarding includes day-start pass-through to StateStore.
   - Symbol: `LifecycleCoordinator.HandleDayStarted`
   - `Lifecycle/LifecycleCoordinator.cs:9`
   - `Lifecycle/LifecycleCoordinator.cs:42`
   - `Lifecycle/LifecycleCoordinator.cs:47`

## 8) High-Value Consistency Checks for Any Follow-On Work

Use this order before writing/updating any phase checklist:

1. Confirm Section 21 phase ownership and stage notes for target capability.
2. Confirm ImplementationIssues index row + record metadata agree on scheduled target and active status.
3. Confirm merged duplicates have one active tracker in index, with duplicate retained only as historical record.
4. Confirm checklist target issues and explicit exclusions match steps 1-3.
5. Confirm checklist instruction/skill still require these checks (for future checklist generation).

## 9) Open Risk Notes

- `issue-159.md` still contains large imported HTML content in narrative sections; this can cause review noise and inconsistency drift in future doc updates.
  - `Project/Tasks/ImplementationPlan/ImplementationIssues/Records/issue-159.md:31`
