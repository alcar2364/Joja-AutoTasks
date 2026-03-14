# Section 12 — Engine Update Cycle #

## 12.1 Purpose ##

The Engine Update Cycle defines how the Automatic Task Manager updates
its internal state during gameplay.

The update cycle coordinates the following systems:

```text
- rule evaluation
- event trigger handling
- state store mutation
- snapshot publishing
- daily snapshot capture
```

The engine must update task state in response to game events while
maintaining deterministic and bounded behavior.

## 12.2 Update Model Overview ##

The engine operates using a hybrid model combining:

```text
- event-driven rule evaluation
- scheduled evaluation passes
```

Event triggers enqueue rule evaluations when relevant game state
changes. Periodic passes ensure the system remains consistent even if
events are missed.

Conceptual flow:

``` text
Game Event
```

→ Engine Trigger Handler
→ Rule Evaluation Queue
→ Evaluation Pass
→ State Store Commands
→ Snapshot Published

```text

```

## 12.3 Engine Lifecycle Phases ##

The engine operates through several lifecycle phases.

1. Initialization
2. Runtime update
3. Day transition
4. Save event handling
5. Teardown (return to title)

Each phase has distinct responsibilities.

## 12.4 Initialization Phase ##

Initialization occurs when the save file is loaded.

Initialization sequence:

1. Load persisted data (Section 9).
2. Restore rule definitions.
3. Restore runtime baseline data.
4. Restore manual tasks.
5. Initialize the State Store.
6. Perform an initial rule evaluation pass.
7. Publish the first task snapshot.

After initialization, the engine enters the runtime update phase.

## 12.5 Event Trigger Handling ##

The engine must listen for relevant game events and convert them into
rule evaluation triggers.

Examples of event categories include:

```text
- inventory changes
- item collection
- machine output events
- location transitions
- time changes
- skill level changes
```

Each event maps to one or more rule triggers.

Event handlers must enqueue rule evaluations rather than executing them
immediately.

## 12.6 Rule Evaluation Queue ##

Triggered rule evaluations are placed into an evaluation queue.

Conceptual structure:

```text
EvaluationQueue
Queue<RuleID>
```

Queue behavior rules:

```text
- duplicate entries may be coalesced
- rules may be evaluated once per update pass
- queue size must remain bounded
```

## 12.7 Evaluation Pass ##

Evaluation passes process queued rules.

During an evaluation pass:

1. Rules are dequeued.
2. Each rule is evaluated against the current evaluation context.
3. Rule outputs are normalized into task results.
4. Commands are emitted to the State Store.

Example command types include:

```text
- `AddOrUpdateTaskCommand`
- `RemoveTaskCommand`
```

The State Store reducer then applies these commands.

## 12.8 Evaluation Context Construction ##

Rules require access to game state during evaluation.

The engine constructs an evaluation context representing relevant game
data.

Conceptual structure:

``` text
EvaluationContext
```

InventorySnapshot
PlayerStats
FarmState
MachineState
CurrentLocation
CurrentDayKey

```text

```

The context should represent a stable snapshot of game state during the
evaluation pass.

Rules must read from the context rather than directly querying the game
environment.

## 12.9 Snapshot Publishing ##

After rule evaluation and state mutations occur, the State Store
publishes a new snapshot if task state has changed.

Snapshot publication sequence:

1. State Store reducers apply commands.
2. Task state version increments.
3. A new immutable snapshot is created.
4. UI subscribers are notified.

If no task state changed, a new snapshot should not be published.

## 12.10 Day Transition Handling ##

Day transitions trigger several system actions.

The normative day-start ownership table — mapping each step to its owning layer
— is defined in **Section 02 §2.5** (On Day Start / On Player Sleep). Section
2.5 is the canonical source. The sequence is not restated here to prevent
drift.

This ensures daily tasks are recreated deterministically.

## 12.11 Save Event Handling ##

When the game is saved, the engine must persist the current system
state.

Save sequence:

1. Persist rule definitions.
2. Persist manual tasks.
3. Persist rule runtime data (baselines).
4. Persist store-level user state.
5. Persist daily snapshot ledger updates.

Persistence behavior must follow the rules defined in Section 9.

## 12.11.1 Teardown Phase (Return to Title) ##

When the player returns to the title screen, the engine must tear down
all runtime state.

The canonical teardown sequence is defined in **Section 02 §2.5** ("On
Returned to Title"). Section 2.5 is the authoritative specification. This
section cross-references it rather than restating the sequence to prevent drift
between sections.

This ensures the mod is safe to re-initialize on the next `OnSaveLoaded`
without residual state from the previous session.

## 12.12 Performance Constraints ##

The engine must avoid excessive computation during gameplay.

Implementation guidelines:

```text
- evaluation passes should be short and bounded
- rule evaluation should be filtered using triggers
- evaluation queues should coalesce duplicate entries
- expensive state queries should be cached in the evaluation context
```

These rules ensure the system does not impact gameplay performance.

## 12.13 Determinism Requirements ##

Engine behavior must remain deterministic for a given game state.

Evaluation results must not depend on:

```text
- evaluation ordering differences
- frame timing
- transient UI state
```

Determinism ensures stable task identities and consistent history
records.

## 12.14 Version 1 Constraints ##

Version 1 of the engine update cycle includes the following limitations:

```text
- no multiplayer synchronization
- no background thread evaluation
- no distributed rule execution
- no asynchronous persistence operations
```

All rule evaluation occurs on the main game thread to maintain
compatibility with the game state APIs.
