# Section 9 — Persistence Model #

## 9.1 Purpose ##

The Persistence Model defines how the Automatic Task Manager stores data
between game sessions.

Persistence ensures that:

    - Player-authored rules are saved.
    - Task Builder baselines remain stable across sessions.
    - Manual tasks are preserved.
    - Task completion state is maintained.
    - The system can recover safely after game reloads.

The persistence layer stores **only the minimal data required to reconstruct
task state**.

All derived data is rebuilt at runtime by the Evaluation Engine and State Store.

## 9.2 Persistence Design Principles ##

The persistence model follows several principles.

### Minimal Storage ###

Only essential state is saved. Derived fields such as progress percentages,
deadline warnings, and task sorting are recomputed.

### Deterministic Reconstruction ###

Tasks generated from rules are recreated through rule evaluation rather than
stored directly.

#### Version Safety ####

All saved data includes version identifiers to support migrations.

#### Separation of Concerns ####

Persistence does not contain UI state or transient engine data.

## 9.3 Saved Data Categories ##

The persistence system stores five primary categories of data.

1. Player Rule Definitions  
2. Manual Tasks  
3. Rule Runtime Data (Baselines)  
4. Store-Level User State  
5. Daily Snapshot Ledger

These are sufficient to rebuild the full task system after loading a save.

## 9.4 SaveData Structure ##

Conceptual structure:

```text
SaveData
    - SaveSchemaVersion
    - TaskBuilderRules
    - ManualTasks
    - RuleRuntimeData
    - StoreUserState
    - DailySnapshotLedger
```

Field descriptions:

SaveSchemaVersion  
Schema version for migration logic.

TaskBuilderRules  
Serialized RuleDefinition objects created by the Task Builder UI.

ManualTasks  
Tasks explicitly created by the player.

RuleRuntimeData  
Runtime values required for rule correctness such as baselines.

StoreUserState  
User-controlled flags such as pinned tasks.

DailySnapshotLedger  
Append-only ledger of daily task snapshots. Storage format is defined in Section 11 §11.7.

## 9.5 Rule Persistence ##

Task Builder rules are stored exactly as serialized in Section 6.

Each rule includes:

    - RuleID
    - Metadata
    - Trigger configuration
    - Condition tree
    - Progress model
    - Output model

Rules are stored independently from generated tasks.

On game load:

1. Rules are loaded.
2. The Evaluation Engine reconstructs runtime rule objects.
3. Rule evaluation generates tasks dynamically.

## 9.6 Manual Task Persistence ##

Manual tasks are fully stored because they are not derived from rules.

Conceptual structure:

ManualTaskRecord

    - TaskID
    - Title
    - Description
    - Category
    - Status
    - ProgressCurrent
    - ProgressTarget
    - Deadline
    - UserFlags

Manual tasks are inserted into the State Store during initialization.

Manual tasks follow the same TaskRecord structure used by generated tasks.

## 9.7 Rule Runtime Data Persistence ##

Certain rule evaluation models require runtime data to remain stable across
sessions.

Examples include:

    - Baselines for progress tracking
    - Initial inventory values
    - Creation timestamps

These values are stored separately from the rule definitions.

Runtime baseline semantics are defined in Section 7.6.

Conceptual structure:

RuleRuntimeRecord

    - TaskID
    - BaselineValues
    - AdditionalRuntimeFields

Example:

RuleRuntimeRecord

TaskID: TaskBuilder_17_ItemWood
BaselineWood: 200

This allows the engine to correctly resume progress tracking after a reload.

## 9.8 Store-Level User State Persistence ##

Some task properties are controlled directly by the player.

Examples include:

    - Task completion toggles
    - Task pinning
    - Future custom ordering

These values are stored in a store-level state structure.

Conceptual structure:

```text
StoreUserState
    - CompletedTasks
    - PinnedTasks
    - OnboardingAcknowledged
```

Fields contain collections of Task IDs unless otherwise noted.

Example:

CompletedTasks

    - TaskBuilder_17_ItemWood

PinnedTasks

    - Manual_3

OnboardingAcknowledged  
Boolean flag. Set to `true` when the player dismisses the first-run experience. Persisted so the onboarding flow is not shown again on subsequent loads.

These flags are applied after tasks are reconstructed during initialization.

## 9.9 Save and Load Lifecycle ##

Persistence occurs during two key lifecycle stages.

### Save Process ###

1. Extract rule definitions from Task Builder.
2. Extract manual tasks from the State Store.
3. Extract runtime baseline data from Rule Runtime Cache.
4. Extract user state flags from the State Store.
5. Write data to mod save storage.

### Load Process ###

1. Load SaveData structure.
2. Rehydrate Task Builder rules.
3. Restore Rule Runtime Cache.
4. Insert manual tasks into State Store.
5. Run rule evaluation cycle.
6. Apply user state flags to tasks.
7. Publish first snapshot.

## 9.10 Rebuilding Generated Tasks ##

Tasks produced by rules are **not persisted directly**.

Instead they are regenerated during the first evaluation pass.

This approach prevents several problems:

    - Outdated task definitions after rule edits
    - Duplicate tasks after schema upgrades
    - Corrupted progress data

Because Task IDs are deterministic, regenerated tasks will map correctly
to stored runtime and user state. See Section 3.3.

## 9.11 Save Schema Versioning ##

All saved data includes a schema version.

Example:

SaveSchemaVersion = 1

If the data structure changes in future versions:

    - Migration logic upgrades old save data
    - Fields may be added or transformed
    - Deprecated fields are removed safely

Migration must always preserve:

    - Player rules
    - Manual tasks
    - Baseline data
    - Completion state

## 9.12 Version 1 Constraints ##

The Version 1 persistence system excludes several features.

    - Multiplayer synchronization
    - Remote data sharing
    - Cloud storage

These systems may be added in future versions.

The initial persistence model focuses only on correctness and stability.

## 9.13 Localization Persistence Boundary ##

Persistence remains canonical and locale-neutral.

Rules:

    - Persist stable semantic identifiers, keys, and arguments required to
    reconstruct task meaning.
    - Do not persist locale-rendered display strings as canonical source of
    truth.
    - If localized display text is ever cached for convenience, it is
    non-canonical and must be recomputable from stable keys and runtime
    locale.

This aligns with minimal persistence and deterministic reconstruction rules.

## 9.14 Daily Snapshot Ledger Persistence ##

The `DailySnapshotLedger` is persisted as part of `SaveData`.

Its JSON storage format is defined in Section 11 §11.7.

The ledger is append-only; historical entries must never be overwritten.

See Section 11 §11.7 for the full storage structure.
