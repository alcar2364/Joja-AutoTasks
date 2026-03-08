---
name: FRONTEND-ARCHITECTURE-CONTRACT
description: "Frontend architecture rules for JAT: HUD vs menu, snapshot-driven UI, command dispatch, performance guardrails. Use when: editing UI code or .sml files."
applyTo: "{UI/**/*.cs,**/*.sml}"
---

# FRONTEND-ARCHITECTURE-CONTRACT.instructions.md #

## Purpose ##

This contract defines **frontend architecture rules** for **JAT (Joja AutoTasks)** as specified by
the Joja AutoTasks Design Guide.

Frontend = HUD + menus + UI data binding and interaction, consuming backend snapshots and
dispatching commands.

Agents MUST treat these rules as **hard constraints** when designing, refactoring, or implementing
UI features.

## In scope ##

    - UI surfaces and their responsibilities (HUD vs menu)
    - UI data binding model (snapshot-driven)
    - interaction patterns and input boundaries
    - how UI dispatches commands to backend
    - UI performance guardrails (rendering, layout, rebuild avoidance)
    - UX-critical rules (task list/detail coupling, history entry points)
    - frontend error presentation requirements (surfacing backend errors)

## Out of scope ##

    - backend engine/state/persistence internals (see [`BACKEND-ARCHITECTURE-CONTRACT.instructions.md`](BACKEND-ARCHITECTURE-CONTRACT.instructions.md))
    - coding style and formatting (see [`CSHARP-STYLE-CONTRACT.instructions.md`](CSHARP-STYLE-CONTRACT.instructions.md), [`SML-STYLE-CONTRACT.instructions.md`](SML-STYLE-CONTRACT.instructions.md))
    - code review / verification / testing requirements (see [`REVIEW-AND-VERIFICATION-CONTRACT.instructions.md`](REVIEW-AND-VERIFICATION-CONTRACT.instructions.md), [`UNIT-TESTING-CONTRACT.instructions.md`](UNIT-TESTING-CONTRACT.instructions.md))
    - workspace/user interaction rules (see [`WORKSPACE-CONTRACTS.instructions.md`](WORKSPACE-CONTRACTS.instructions.md))
    - UI component patterns (see [`../Instructions/UI-COMPONENT-PATTERNS.instructions.md`](../Instructions/UI-COMPONENT-PATTERNS.instructions.md))
    - StarML authoring rules (see [`SML-STYLE-CONTRACT.instructions.md`](SML-STYLE-CONTRACT.instructions.md))

## 1. Canonical UI Surfaces ##

## 1.1 HUD (lightweight surface) ##

Responsibilities:
    - Always available during gameplay.
    - Display active tasks (and optionally completed tasks).
    - Provide bounded interaction: scrolling, expand/collapse, open menu, drag position (if
    implemented).
    - Must remain fast and low-allocation.

Hard rules:
    - HUD MUST be snapshot-driven (read-only).
    - HUD MUST NOT be responsible for complex filtering, analytics, or rule/task editing.
    - HUD MUST NOT mutate canonical task state.

## 1.2 Main Menu (dashboard surface) ##

Responsibilities:
    - Primary management UI for tasks and their details.
    - Provide access to history browsing (statistics deferred to V2).
    - Support task list + task details visible from the same section (coordinated panes/sections).
    - Provide configuration surfaces appropriate for the mod design (including debug surfaces when
    enabled).

Hard rules:
    - Menu MUST be snapshot-driven (read-only data binding).
    - Menu actions MUST dispatch commands to backend; it MUST NOT mutate canonical task state.

## 2. UI Data Binding Contract (Snapshot-driven UI) ##

## 2.1 Single UI truth source ##

HUD and menus MUST display information derived from the **same backend State Store snapshot** (or
projections derived from that snapshot).

The backend State Store is the sole owner of canonical task state; frontend models are projections
only.

## 2.2 Refresh triggers ##

UI MUST refresh when:
    - backend publishes a new snapshot
    - snapshot-changed events emitted by backend State Store (preferred over polling)
    - relevant UI configuration changes
    - debug tuning values change (when debug mode is enabled)

UI SHOULD avoid refresh/rebuild when none of the above changed.

UI MUST NOT poll backend state on a per-frame cadence to discover changes.

## 2.3 No direct mutation ##

UI MUST NEVER directly mutate canonical task state.

The only legal path is:
UI interaction → command dispatch → backend reducer → new snapshot → UI refresh

## 3. Interaction and Input Boundaries ##

## 3.1 HUD interaction budget ##

HUD interaction MUST remain bounded. Acceptable interactions include:
    - scroll list (mouse wheel and/or arrows)
    - expand/collapse HUD
    - expand/collapse completed section
    - drag HUD within viewport bounds
    - open main menu

HUD SHOULD avoid complex modal flows.

## 3.2 Menu interaction scope ##

Menu may support:
    - browsing tasks and details
    - selecting days for history browsing
    - viewing statistics (V2)
    - editing rules/configuration (as designed)
    - viewing errors and validation messages

All state changes are command-based.

## 4. UI System Design Constraints ##

## 4.1 StardewUI usage model ##

    - HUD uses StardewUI's `IViewDrawable` API for layout, rendering, and input routing.
    - Menus use StardewUI's menu API for layout, rendering, and interaction.

## 4.2 Stable view models ##

UI SHOULD prefer stable view models derived from snapshots:
    - avoid re-allocating large collections every frame
    - avoid rebuilding UI trees unless inputs changed

## 4.3 Task list + details coordination ##

Menu SHOULD support:
    - task list and task details visible concurrently (e.g., left list + right details panel)
    - consistent selection model driven from snapshot + UI selection state
    - history browsing entry point (date/day selector) integrated near the top of the menu surface

## 5. UI Performance Guardrails ##

## 5.1 Render loop constraints ##

HUD rendering MUST be lightweight:
    - cache layout calculations when possible
    - avoid rebuilding UI elements every frame
    - minimize dynamic allocations during rendering
    - recalc layout on snapshot/config/debug changes, not continuously

Menu rendering SHOULD also avoid unnecessary churn.

## 5.2 Avoid per-frame data work ##

UI MUST NOT:
    - run expensive filtering/sorting every frame
    - repeatedly traverse large task sets on draw/update
    - request backend evaluation on a per-frame cadence

## 6. Error Presentation and User Feedback ##

Frontend MUST be able to display backend-reported:
    - rule validation errors
    - migration/version errors
    - runtime engine warnings (deduplicated)

HUD MAY display minimal warnings (e.g., icon/badge + “open menu”), while Menu MUST provide detailed
messages and remediation guidance.

## 7. Frontend ↔ Backend Interaction Contract (shared boundary) ##

This section describes how frontend and backend MUST interact. (Backend also repeats this boundary
from its perspective.)

## 7.1 Snapshot boundary ##

Frontend consumes backend snapshots as read-only data.

Frontend MUST NOT modify snapshot objects in-place.

## 7.2 Command boundary ##

Frontend MUST express user intent via commands to the backend State Store.

Backend MUST validate and apply reducers deterministically, then publish an updated snapshot if
canonical state changes.

## 7.3 Error and status channel ##

Frontend MUST consume backend error/status messages and surface them in UI:
    - minimal/summary in HUD (optional)
    - detailed in Menu (required)

## 8. Localization Boundary Contract (SMAPI I18n) ##

## 8.1 UI-owned translation resolution ##

Frontend MUST resolve translated display text using SMAPI `I18n`.

Frontend MUST treat backend snapshots as locale-neutral semantic data.

## 8.2 Identity and mutation safety ##

Frontend MUST NOT use translated text as input to:
    - TaskID/RuleID/DayKey/SubjectID logic
    - command identity payloads
    - deterministic ordering/equality behavior

Frontend MUST treat TaskID/RuleID/DayKey/SubjectID as opaque canonical values from backend.

Frontend MUST NOT generate, mutate, or normalize canonical identifiers.

Canonical state remains backend-owned and locale-neutral.

## 8.3 Missing-key fallback behavior ##

Frontend MUST implement deterministic fallback behavior when translation keys are missing:
    - prefer explicit fallback text if available
    - otherwise display the unresolved key token

Frontend SHOULD emit bounded/deduplicated warning logs for missing keys.

## 9. Dependency Wiring Contract (Design Guide Section 2.4) ##

Frontend subsystems and view models MUST declare core dependencies via constructor parameters.

Frontend MUST NOT rely on service locators or ambient global singletons for core runtime
dependencies.

Composition roots/factories MAY assemble dependencies, but constructed instances MUST still receive
explicit constructor arguments for testability and dependency visibility.
