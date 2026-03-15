# Section 18 - Versioning and Migration Strategy

## 18.1 Purpose

The versioning and migration system ensures that saved configuration, rules, and persisted task state remain compatible across mod updates.

Because the mod stores persistent data structures such as rules, configuration values, and task state, schema changes must be handled in a controlled and deterministic manner.

The migration system must preserve user data whenever possible while ensuring that incompatible data does not corrupt runtime state.

## 18.2 Versioned data domains

The following data domains require explicit versioning support:

```text
- Configuration files
- Persisted rule definitions
- Task state persistence
- Generated task identity structures
```

Each domain may evolve independently and therefore must include its own schema version identifier.

## 18.3 Schema version identifiers

All persisted data structures must contain a schema version field.

Example configuration structure:

```json
{
  "ConfigVersion": 1,
  "System": {},
  "Hud": {},
  "Menu": {},
  "Generators": {},
  "Debug": {}
}
```

Example rule structure:

```json
{
  "RuleVersion": 1,
  "RuleID": "PlayerRule.CollectWood",
  "Definition": {}
}
```

Version identifiers allow the system to determine whether migration is required during loading.

## 18.4 Migration triggers

Migrations occur during data loading when the stored version does not match the version expected by the current mod build.

Migration checks must occur during:

1. Configuration loading
2. Rule definition loading
3. Task state loading

If the stored version matches the current version, no migration is required.

## 18.5 Migration pipeline

Migrations must follow a deterministic stepwise pipeline.

The system must apply migrations sequentially from the stored version to the current version.

Conceptual flow:

```text
Stored Version → Migration Step → Updated Version → Repeat
```

Example:

```text
Version 1 → Migration 1→2 → Version 2
Version 2 → Migration 2→3 → Version 3
```

Each migration step must operate only on the schema difference between two adjacent versions.

## 18.6 Migration safety rules

Migrations must follow strict safety rules to prevent data corruption.

Migration rules:

1. Migrations must be deterministic.
2. Migrations must not depend on runtime game state.
3. Migrations must preserve user-authored rules whenever possible.
4. Migrations must validate the resulting data structure before use.

If migration produces invalid data, the affected data must be disabled rather than loaded into the system.

## 18.7 Configuration migration

Configuration migrations update the `ModConfig` structure when new fields or categories are introduced.

Typical migration behaviors include:

```text
- Adding new configuration fields with default values
- Renaming deprecated fields
- Removing obsolete configuration fields
```

Configuration migration must preserve existing user preferences whenever possible.

## 18.8 Rule migration

Rule definitions may evolve as the rule engine described in Section 7 gains new features or structural changes.

Rule migrations may include:

```text
- Updating trigger formats
- Converting subject identifiers
- Updating progress model structures
- Updating timing definitions
```

If a rule cannot be migrated safely it must be disabled and flagged for user review rather than automatically removed.

## 18.9 Task state migration

Persisted task state may require migration when the task model evolves.

Possible migration scenarios include:

```text
- New task metadata fields
- Changes to progress model structure
- Changes to completion state representation
```

Migration must preserve task completion history whenever possible.

## 18.10 Forward compatibility

The persistence system should preserve unknown fields when loading future-compatible data.

This prevents older mod versions from destroying fields introduced in newer versions.

Unknown fields must be ignored but retained when possible.

## 18.11 Migration logging

All migrations must produce diagnostic log entries.

Log entries should include:

```text
- data domain
- original version
- target version
- migration steps applied
```

Migration logs assist developers when diagnosing compatibility issues.

## 18.12 Migration failure handling

Migration may fail due to corrupted data or incompatible schema changes.

Failure handling rules:

1. The system must log the failure.
2. The invalid data must be isolated.
3. The rest of the system must continue loading normally.

Affected data may be disabled until corrected or regenerated.

## 18.13 Versioning constraints

Versioning must follow these constraints:

1. Schema versions must increment monotonically.
2. Breaking changes must always introduce a new schema version.
3. Migration steps must remain available for older versions.
4. Migration code must remain deterministic.

These constraints ensure long-term compatibility of user data across mod updates.

## Implementation Plan Traceability

Primary phase owner(s):

- Phase 7 — Persistence System

Also referenced in:

- Phase 10 — Task Builder Wizard

Canonical implementation mapping lives in Section 21.
