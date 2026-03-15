# Section 7 — Rule Evaluation Model

## 7.1 Purpose

The Rule Evaluation Model defines how Task Builder rules are:

- evaluated against game state
- converted into Task Objects
- updated over time (progress and completion)
- reconciled with the State Store without duplication or task shuffle
- evaluated efficiently without overwork

This section focuses only on Task Builder rules. Built-in generators follow a similar pipeline, but have separate generator logic.

## 7.2 Core Requirements

The evaluation model must guarantee:

- Determinism The same rule evaluated against the same context produces the same `TaskId` and the same task output fields.
- Stability across sessions Rules persist and rehydrate without changing the produced task identities.
- Correct progress tracking Baseline-based progress must not reset incorrectly, except for daily rules that intentionally reset each day.
- Bounded performance Rules must not evaluate every tick unless needed. Evaluation should be event-driven where possible and corrected by throttled periodic passes.
- Reconciliation-safe behavior Rule output must merge cleanly with tasks from built-in generators and manual tasks using deterministic `TaskId` values.

## 7.3 Evaluation Inputs and Outputs

Rule evaluation consumes:

- `RuleDefinition` serialized data loaded into the runtime model
- Generation and Evaluation Context as a snapshot of relevant game state
- Rule Runtime Cache as engine-maintained per-rule runtime values, such as baselines

Rule evaluation produces a Rule Evaluation Result which is then normalized into a Task Object.

Conceptual output fields:

- Deterministic `TaskId`
- `Title`, `Description`, `Category`, and `Icon` metadata
- `Status` as `Incomplete` or `Completed`
- Optional progress and target values
- Derived properties such as deadline warnings and overdue flags
- Scheduling metadata if needed later

## 7.4 Two-Phase Rule Evaluation

Rule evaluation is split into two conceptual phases.

### 7.4.1 Phase A — Existence and Progress

This phase determines:

- Should a task exist right now?
- If it exists, what is its current progress state?

This phase computes:

- `ShouldExist`
- Optional `ProgressCurrent`
- Optional `ProgressTarget`
- Optional `DerivedDeadlineState`

### 7.4.2 Phase B — Completion Determination

This phase determines whether the task should be marked completed.

It applies:

- condition tree completion rules
- progress threshold completion rules
- deadline-based derived properties without adding new statuses in Version 1

This yields:

- `Status = Completed | Incomplete`

This split ensures the engine can compute progress even when a task is incomplete and handle derived UI warnings consistently.

## 7.5 Task Identity Derivation

Task identity must be stable and deterministic.

Task Builder `TaskId` values are derived from:

- stable `RuleID`
- stable `SubjectID` values for the target entity, item, or goal
- `DayKey`, only for daily recurring rules

Conceptual format:

```text
TaskBuilder_{RuleID}_{SubjectID?}_{DayKey?}
```

Rules must declare whether they generate:

- persistent tasks with no day key
- daily tasks that include a day key

Examples:

- Persistent: `TaskBuilder_17_ItemWood`
- Daily: `TaskBuilder_42_Daily_Year2-Spring12`

**Note:** `_` is the `TaskId` component separator. The hyphen inside the embedded `DayKey` token is preserved as part of the canonical `DayKey` format (e.g., `Year2-Spring12`). Do not replace the hyphen with an underscore.

Key constraint:

A single rule must never generate two different task identities for the same conceptual task within the same day or context.

## 7.6 Baseline Capture Rules

Some progress models require a baseline value, for example collect 300 more wood from now.

Baseline capture is controlled by `BaselineMode`:

- `None`
- `CaptureAtCreation`
- `CaptureDaily`

### 7.6.1 CaptureAtCreation

Baseline is recorded the first time the engine creates the task instance.

Baseline is stored in the engine's runtime cache and persisted through the save system.

Baseline remains constant for the lifetime of the persistent task.

Baseline must be keyed by deterministic `TaskId`.

Example:

- Player wood = 200 when task first created
- Baseline = 200
- Target = 300
- Completion when wood >= 500

### 7.6.2 CaptureDaily

Baseline is recorded at day start for each daily instance.

Baseline resets every day because the task identity is day-keyed.

Baselines are stored per-day instance and included in daily snapshot results.

Example:

- Cut down 5 trees every day
- Baseline captured at start of day
- Progress computed against that baseline only for the day's `TaskId`

**Normative (V1):** Daily baselines are stored in the daily snapshot ledger entry for that day, not in `RuleRuntimeData`. On mid-day reload (e.g., after a crash), the daily baseline is re-captured at the start of the next evaluation pass. Progress for the day resets to zero. This is accepted V1 behavior and is not a bug.

## 7.7 Rule Runtime Cache

The engine maintains a Rule Runtime Cache for any rule that requires:

- baselines
- last evaluation time
- derived persistent internal values needed for correctness and performance

Conceptual cache record:

```text
RuleRuntimeRecord
    - TaskId
    - BaselineValues (optional)
    - LastEvaluatedAt (optional)
    - LastKnownProgress (optional)
```

Persistence requirements:

- Persistent tasks must persist baseline values across sessions.
- Daily tasks may persist only within the daily snapshot system.

## 7.8 Evaluation Scheduling

Task Builder rule evaluation uses a hybrid scheduling strategy:

- event-driven updates to respond quickly
- periodic throttled passes to correct missed events and prevent drift

### 7.8.1 Domain Dirty Flags

Game events mark domains dirty:

- Inventory changed -> Inventory domain dirty
- Quest updated -> Quest domain dirty
- Time changed -> Time domain dirty
- Location changed -> Location domain dirty
- Machine state changed -> Machine domain dirty, if detectable

Dirty flags indicate that at least one rule might be affected.

### 7.8.2 Rule Trigger Filtering

Each rule declares triggers, and triggers map to domains.

Example mapping:

- `InventoryChange` -> Inventory domain
- `TimeChange` -> Time domain
- `LocationChange` -> Location domain
- `DayStart` -> Full re-evaluation boundary

When a domain is dirty:

- only rules subscribed to that domain's triggers are queued for evaluation
- other rules are skipped

### 7.8.3 Periodic Throttle Pass

A bounded periodic pass runs to:

- evaluate queued rules
- enforce a maximum work budget per tick
- correct missed edge cases

Conceptually:

- queue grows via events
- periodic pass drains queue within budget

## 7.9 Evaluation Cycle and Reconciliation

Each evaluation cycle follows:

1. Build or refresh the Evaluation Context using snapshot and cached lookups.
2. Determine affected rules using dirty flags and triggers.
3. Evaluate rules using two-phase evaluation.
4. Convert results into normalized Task Objects.
5. Merge results into the unified pipeline with built-in and manual tasks.
6. Publish to the State Store via commands only.

Key constraint:

UI never mutates tasks directly. Only the engine and manual UI commands emit mutations into the store.

## 7.10 Rule Result Normalization

Rule outputs must be normalized into a consistent Task Object.

Normalization includes:

- enforcing deterministic `TaskId`
- applying default category and priority if missing
- ensuring consistent title, description, and icon formatting
- ensuring progress fields are present only when required
- ensuring deadline-derived fields are computed consistently

Deadline fields are derived in Version 1:

- `DueDayKey`
- `DaysRemaining`
- `IsOverdue`

No new statuses are introduced in Version 1. Overdue tasks remain `Incomplete`.

## 7.11 Collision and Merge Rules

Multiple sources may attempt to create tasks with the same `TaskId`.

Resolution rule:

- `TaskId` is the primary key.
- Exactly one authoritative task instance exists per ID in the unified task list.

If a collision occurs, the engine uses deterministic rules to select the winner, typically source precedence.

Collisions should be treated as a bug during development, since `TaskId` values should be designed to avoid overlaps between subsystems.

Recommended precedence in Version 1:

- Manual Tasks as user-authored explicit tasks
- Task Builder Tasks as user rules
- Built-in Tasks as automatic generators

This ensures player-authored tasks are never unexpectedly replaced by automatic outputs.

## 7.12 Completion Reconciliation with Store State

The engine must reconcile with store state to preserve:

- manual completions for manual tasks
- pinned tasks
- task history continuity
- stable baselines

Rules:

- If a task is marked completed in the store for the current day, the engine must not re-open it unless the task is daily and the day key changes.
- For persistent tasks, completed tasks remain completed and should not reappear as incomplete.
- For daily tasks, completion applies only to that day-keyed instance.

## 7.13 Examples

### 7.13.1 Example A — Persistent Baseline Progress

Rule: Collect 300 wood to build coop

- `TaskId`: `TaskBuilder_17_ItemWood`
- `BaselineMode`: `CaptureAtCreation`
- Baseline captured first time task is created
- `Progress = CurrentWood - BaselineWood`
- Completed when `CurrentWood >= Baseline + 300`

### 7.13.2 Example B — Daily Recurring Baseline Progress

Rule: Cut down 5 trees every day

- `TaskId` includes day key: `TaskBuilder_42_Daily_Year2-Spring12`
- `BaselineMode`: `CaptureDaily`
- Baseline captured at day start for that instance
- Progress tracked for the day only
- Next day produces a new task ID with a new baseline

### 7.13.3 Example C — Deadline-Derived Properties

Rule: Reach floor 70 by Spring 25

- `DueDayKey = YearX-Spring25`
- Derived: `DaysRemaining`
- Derived: `IsOverdue`
- Status remains `Incomplete` until goal met and does not become `Failed` in Version 1

## 7.14 Version 1 Constraints

Version 1 evaluation constraints:

- only two statuses: `Incomplete` and `Completed`
- no dismissal or snooze system
- deadline failure represented only via derived properties
- multiplayer excluded

## Implementation Plan Traceability

Primary phase owner(s):

- Phase 6 — Rule Evaluation Engine

Also referenced in:

- Phase 7 — Persistence System
- Phase 12 — Debug and Development Tools

Canonical implementation mapping lives in Section 21.
