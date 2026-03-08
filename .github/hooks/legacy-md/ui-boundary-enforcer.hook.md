---
name: ui-boundary-enforcer
description: >-
  Enforces frontend/backend boundaries. Blocks UI from bypassing snapshot
  consumption. Prevents backend from depending on UI.
trigger: before-edit
applyTo: "{UI/**/*.cs,**/*.sml}"
---

# UI Boundary Enforcer Hook #

    * Trigger: before editing UI C# or StarML files.
    * Purpose: enforce frontend and backend separation rules.

## Frontend to Backend Boundary ##

UI code is allowed to:

    * Consume read-only snapshots.
    * Dispatch commands to express user intent.
    * Subscribe to snapshot-changed events.
    * Maintain UI-local state such as selected tab, sort, or scroll.
    * Call read-only query methods.

UI code is blocked from:

    * Direct canonical state mutation.
    * In-place snapshot mutation.
    * Calling backend mutation shortcuts that bypass command dispatch.
    * Accessing internal State Store collections.
    * Per-frame polling for state refresh.

## Backend to Frontend Boundary ##

Backend code is allowed to:

    * Publish snapshots.
    * Emit events or signals for UI updates.
    * Expose read-only query APIs.
    * Accept command inputs.

Backend code is blocked from:

    * Depending on UI namespaces or UI types.
    * Triggering direct UI refresh calls.
    * Embedding HUD or menu presentation logic.

## Validation Checklist ##

    * [ ] Snapshot consumption remains read-only.
    * [ ] Mutations use command dispatch.
    * [ ] UI edits avoid backend implementation coupling.
    * [ ] Backend edits avoid UI presentation coupling.
    * [ ] Refresh behavior is event-driven, not polling-driven.

## Blocked Patterns ##

    var snapshot = stateStore.GetSnapshot();
    snapshot.Tasks[0].Status = TaskStatus.Completed;

    stateStore.ActiveTasks.Add(newTask);
    taskManager.CompleteTask(taskId);

    void OnUpdate()
    {
        var latest = stateStore.GetSnapshot();
        RefreshDisplay(latest);
    }

## Approved Patterns ##

    var snapshot = stateStore.GetSnapshot();
    Display(snapshot.Tasks);

    var command = new CompleteTaskCommand(taskId);
    commandDispatcher.Dispatch(command);

    stateStore.SnapshotChanged += (_, latest) =>
    {
        RefreshDisplay(latest);
    };

## StarML-Specific Rules ##

    * StarML binds to view-model properties and commands only.
    * StarML event handlers route intent to command paths.
    * StarML files do not contain business logic.

## Violation Output Template ##

    FRONTEND BACKEND BOUNDARY VIOLATION
    File: [path]
    Violation type: [type]
    Why blocked: [reason]
    Required fix: [fix]
    Reference: FRONTEND-ARCHITECTURE-CONTRACT [section]

Do not continue edit generation until boundary violations are resolved.

## Integration ##

Works with:

    * `state-mutation-guard` for mutation-path enforcement.
    * `contract-auto-loader` for frontend and backend contract loading.
    * `design-guide-context-augmenter` for UI data binding context.
