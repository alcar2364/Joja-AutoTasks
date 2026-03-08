---
name: identifier-validation
description: >-
  Validates determinism of TaskID, RuleID, SubjectID, DayKey. Ensures stable
  identifiers across reloads and evaluation passes.
trigger: after-edit
applyTo: "{Domain/Identifiers/**/*.cs,Domain/Tasks/**/*.cs,**/*Generator*.cs,**/*Rule*.cs}"
---

# Identifier Validation Hook #

    * Trigger: after editing identifier, task generation, or rule evaluation code.
    * Purpose: keep TaskID, RuleID, SubjectID, and DayKey deterministic and stable.

## Determinism Rules ##

### TaskID ###

    * Must be deterministic for the same source and inputs.
    * Must stay stable across evaluation passes and save/load cycles.
    * Must be derived from stable inputs such as rule or generator identity, subject identity,
  and day key when applicable.
    * Must not use random values, wall-clock time, or frame counters.

### RuleID ###

    * Must be deterministic for user-authored rules.
    * Should remain stable across ordinary edits unless identity-defining fields change.
    * Must not use GUID or timestamp generation.

### SubjectID ###

    * Must map to stable game entity identity.
    * Must be unique for target subject scope.
    * Must remain stable across sessions.

### DayKey ###

    * Must map one-to-one with game date.
    * Must be stable across sessions for the same game date.
    * Must support deterministic ordering for history operations.

## Validation Checklist ##

    * [ ] No `Guid.NewGuid()` in deterministic identifier code paths.
    * [ ] No wall-clock time usage in identifier formulas.
    * [ ] No unseeded randomness in identifier formulas.
    * [ ] Rule-based TaskID formulas include RuleID context.
    * [ ] Generator-based TaskID formulas include generator identity.
    * [ ] DayKey uses game date, not real-world date.
    * [ ] SubjectID uses stable entity keys, not transient object references.

## Required Test Expectations ##

When identifier logic changes, verify tests cover:

    * Same input produces same identifier across repeated calls.
    * Identifier stability across simulated reload.
    * Identifier independence from call order.
    * Expected day-transition behavior for day-scoped identifiers.

If tests are missing, report a test-gap warning and suggest target test locations.

## Blocked Patterns ##

    var taskId = new TaskID(Guid.NewGuid().ToString());
    var taskId = new TaskID($"task-{DateTime.Now.Ticks}");
    var taskId = new TaskID($"task-{new Random().Next()}");

## Approved Patterns ##

    var taskId = TaskID.FromRule(ruleId, subjectId, dayKey);
    var taskId = TaskID.FromGenerator("MailTaskGenerator", npcName);
    var dayKey = DayKey.FromGameDate(Game1.Date);

## Violation Output Template ##

    IDENTIFIER DETERMINISM VIOLATION
    File: [path]
    Issue: [issue]
    Impact: [impact]
    Required fix: [fix]

Do not accept edit output until determinism violations are resolved.

## Integration ##

Works with:

    * `contract-auto-loader` for backend determinism and architecture rules.
    * `design-guide-context-augmenter` for Design Guide identifier guidance.
    * `unit-test-coverage-enforcer` for stability and regression test coverage.
