# Section 16 - Error Handling and Rule Validation #

## 16.1 Purpose ##

The error handling and validation system ensures that rule definitions,
task generation, and engine evaluation operate safely and predictably
even when invalid inputs or runtime failures occur.

The system must prevent corrupted rules, invalid task states, and
generator failures from affecting the stability of the task engine.

Failures must be isolated, logged, and safely recovered without
interrupting normal gameplay.

## 16.2 Failure domains ##

Failures may occur in several subsystems of the task engine.

Primary failure domains include:

    - Rule definition validation
    - Generator execution
    - Rule evaluation
    - State store command application
    - Persistence and serialization

Each domain must isolate failures so that one failing component does not
prevent the rest of the system from functioning.

## 16.3 Rule definition validation ##

Rule definitions created by the Task Builder Wizard must be validated
before persistence.

Validation must ensure:

    - All required fields are defined.
    - Trigger definitions are valid.
    - Subject identifiers are deterministically defined.
    - Progress models contain valid parameters.
    - Timing constraints are logically consistent.

Invalid rules must not be persisted to storage. See Section 9.

## 16.4 Runtime rule validation ##

Persisted rules must also be validated during rule loading.

This protects the system from:

    - corrupted rule data
    - incompatible rule versions
    - manual modification of save files

If a rule fails runtime validation it must be disabled and excluded from
evaluation until corrected.

## 16.5 Generator error handling ##

Task generators described in Section 13 may fail due to unexpected game
state conditions or runtime exceptions.

Generator execution must be wrapped in a safe evaluation boundary.

If a generator throws an exception:

1. The exception must be caught.
2. The generator must be marked as failed for the current evaluation.
3. A diagnostic log entry must be recorded.
4. Other generators must continue execution.

Generator failures must not terminate the evaluation cycle.

## 16.6 Rule evaluation failures ##

Rule evaluation failures may occur during condition evaluation or
progress calculation.

Evaluation failures must follow these rules:

    - The failing rule must be skipped for the current evaluation cycle.
    - The failure must be logged for diagnostics.
    - Other rules must continue evaluating normally.

A rule that repeatedly fails evaluation should be automatically disabled
after a defined failure threshold.

## 16.7 State store command safety ##

Commands applied to the State Store must be validated before execution.
See Section 8.

Command validation must ensure:

    - Referenced `TaskID` values exist.
    - Task identity fields are immutable.
    - Commands do not violate store invariants.

Invalid commands must be rejected and logged.

## 16.8 Task state consistency ##

The system must prevent inconsistent task states from being persisted.

Examples of invalid states include:

    - Negative progress values
    - Progress exceeding defined completion targets
    - Completion timestamps earlier than task creation

Consistency checks must be applied during command processing within the
State Store.

## 16.9 Persistence failures ##

Persistence failures may occur during save or load operations described
in Section 9.

Failure scenarios include:

    - corrupted save data
    - serialization errors
    - schema mismatches

The persistence system must attempt safe recovery when possible.

Recovery strategies may include:

    - fallback to last valid snapshot
    - migration of compatible fields
    - disabling invalid rules

## 16.10 Logging and diagnostics ##

All failures must produce diagnostic log entries.

Diagnostic logs should include:

    - subsystem name
    - rule or generator identifier
    - failure description
    - timestamp

Logging must be rate-limited when repeated failures occur to prevent log
flooding.

## 16.11 User-facing error behavior ##

Most runtime errors must remain invisible to the player.

Errors that affect player-created rules may optionally produce
non-intrusive notifications indicating that a rule was disabled or
requires correction.

User notifications must not interrupt gameplay.

## 16.12 Failure recovery principles ##

The error handling system must follow these principles:

1. Failures must be isolated to the smallest possible scope.
2. The system must continue operating whenever possible.
3. Invalid inputs must never corrupt runtime state.
4. Diagnostic information must be available for debugging.

These principles ensure that the task engine remains stable during
unexpected runtime conditions.
