# Section 11 — Daily Snapshot Ledger #

## 11.1 Purpose ##

The Daily Snapshot Ledger records the state of tasks for each in-game
day.

The ledger enables the following capabilities:

```text
- viewing historical task lists
- browsing tasks from previous days
- displaying task completion history
- supporting future statistics and analytics features
```

The ledger is append-only. Historical snapshots must never be modified
once written.

The ledger is separate from the State Store described in Section 8 and
the persistence model described in Section 9.

## 11.2 Conceptual Model ##

The Daily Snapshot Ledger stores a snapshot of the task list at the end
of each in-game day.

Conceptually:

`DailySnapshotLedger`

```text
`Dictionary<DayKey, DailyTaskSnapshot>`
```

Each `DailyTaskSnapshot` contains the tasks that existed on that day.

Snapshots are read-only historical records.

## 11.3 Snapshot Structure ##

A daily snapshot contains a list of task records captured for that day.

Conceptual structure:

`DailyTaskSnapshot`

```text
`DayKey`
`List<HistoricalTaskRecord>`
```

Each `HistoricalTaskRecord` represents the state of a task on that day.

`HistoricalTaskRecord`

```text
`TaskID`
`Title`
`Status`
`ProgressCurrent`
`ProgressTarget`
`Category`
`DeadlineFields`
```

Note: ProgressCurrent and ProgressTarget are tracking metrics. Status represents
the authoritative completion state as determined by completion conditions, not
solely by progress saturation. See Section 4.4.1 for details.

Only fields required for historical display must be stored.

Fields that are derived for runtime UI display (sorting, grouping,
computed progress percent) should not be persisted in the ledger.

## 11.4 Snapshot Capture Timing ##

A snapshot must be captured during the `OnSaving` handler, when the player goes
to sleep, using the current day's `DayKey`. It is not captured at
`OnDayStarted`.

Capture sequence:

1. The Lifecycle Coordinator receives the `OnSaving` signal.
2. The Daily Snapshot Ledger reads the current task snapshot from the State
   Store (read-only).
3. The snapshot is written to the ledger under today's `DayKey`.
4. The ledger entry for this day is now complete and immutable.

For the full day-transition sequence including new-day task generation, see
Section 02 §2.5 and Section 12 §12.10.

## 11.5 Snapshot Data Source ##

Snapshots must be generated from the published task snapshot described
in Section 8.5.

The ledger must not read directly from the mutable State Store
structures.

Using the published snapshot ensures:

```text
- stable data for the capture event
- consistent task ordering for historical viewing
- deterministic behavior during capture
```

## 11.6 Snapshot Inclusion Rules ##

Snapshots must include tasks that existed during that day.

Included tasks:

```text
- manual tasks
- Task Builder tasks
- built-in generated tasks
```

Excluded tasks:

```text
- tasks created after the day transition
- tasks removed before the snapshot capture
```

Daily tasks are stored as the day-keyed task instance that existed on
that day.

## 11.7 Persistence Model ##

Daily snapshots must be stored using the persistence system described in
Section 9.

Conceptual storage structure:

``` json
{
  "dailySnapshots": {
```

"Year2_Spring12": {
  "dayKey": "Year2_Spring12",
  "tasks": [
    {
      "taskId": "TaskBuilder_42_Daily_Year2_Spring12",
      "title": "Cut down 5 trees",
      "status": "Completed",
      "progressCurrent": 5,
      "progressTarget": 5,
      "category": "Resources",
      "deadlineFields": null
    }
  ]
}

```text

  }
}

```

Snapshots must persist across game reloads.

## 11.8 History Query Model ##

The Task Menu may query the ledger to display historical task lists.

Conceptual query:

`GetSnapshot(DayKey) -> DailyTaskSnapshot`

If a requested day does not exist in the ledger, the UI should display
an empty state rather than attempting to reconstruct history.

Returned snapshots are read-only. The UI must not modify historical
records.

## 11.9 Snapshot Size Management ##

The ledger may grow over time as the save file accumulates daily
snapshots.

Version 1 should store snapshots for all recorded days.

Future implementations may introduce retention policies such as:

```text
- limiting snapshots to a fixed number of days
- archiving older snapshots
- compressing historical data
```

Retention policies must be opt-in and must not destroy data without
explicit user action.

## 11.10 Relationship to Task Identity ##

Snapshots store `TaskID` values as defined in Section 3.

Identity determinism, collision prevention, and stability behavior are canonical
in Section 3.

If identifier rules change in later versions, migrations must preserve existing
historical `TaskID` values or provide a stable mapping layer.

## 11.11 Debug and Development Support ##

The Daily Snapshot Ledger provides debugging and validation support.

Developers may inspect snapshots to:

```text
- verify daily task reset behavior
- confirm rule evaluation stability across day boundaries
- validate task completion state at sleep time
- detect task identity collisions
```

Debug tooling must not modify the ledger.

## 11.12 Version 1 Constraints ##

Version 1 of the ledger supports only basic historical browsing.

Excluded features include:

```text
- statistical aggregation
- streak tracking
- trend dashboards
- multiplayer synchronization
```

These features may be implemented in later versions and should derive
from ledger data rather than duplicating historical state elsewhere.
