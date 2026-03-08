# Section 19 - Performance Guardrails #

## 19.1 Purpose ##

The performance guardrail system defines constraints and design rules
that ensure the mod operates efficiently during gameplay.

Because the task engine evaluates generators, rules, and UI updates
during active gameplay, poorly constrained logic could introduce
performance degradation.

Performance guardrails establish limits and best practices that prevent
subsystems from performing excessive computation during game updates.

## 19.2 Performance-sensitive subsystems ##

The following subsystems have the greatest potential impact on
performance:

    - Task generation engine
    - Rule evaluation engine
    - State Store reconciliation
    - HUD rendering and layout updates
    - Debug diagnostics when enabled

Each subsystem must operate within the guardrails defined in this
section.

## 19.3 Evaluation frequency limits ##

Task generation and rule evaluation must not occur every frame unless necessary.

Evaluation must instead be triggered by discrete events such as:

    - Day start
    - Player inventory change
    - Machine output completion
    - Skill level change
    - Explicit developer command

Evaluation frequency must be minimized to avoid unnecessary repeated
computation.

## 19.4 Generator scanning limits ##

Generators described in Section 13 may scan portions of the game state.

Generators must avoid scanning the entire world state whenever possible.

Preferred strategies include:

    - scanning only relevant entity collections
    - caching references to frequently accessed objects
    - limiting evaluation to the current game location when applicable

Generators must not perform expensive world scans every update tick.

## 19.5 Rule evaluation constraints ##

Rule evaluation must remain lightweight and deterministic.

Rules must avoid:

    - complex nested iteration
    - expensive game-state queries
    - dependence on large entity scans

Rule conditions should rely primarily on the `GenerationContext`
snapshot provided by the evaluation engine.

## 19.6 State store reconciliation limits ##

The State Store reconciliation process compares generated task
definitions against persisted tasks.

Reconciliation must operate in linear time relative to the number of
tasks involved.

Design constraints:

    - use deterministic `TaskID` lookup tables
    - avoid repeated list scans
    - avoid allocating large temporary collections

Efficient reconciliation ensures that task generation remains scalable
as the number of tasks grows.

## 19.7 HUD rendering constraints ##

The HUD described in Section 21 is rendered during the game's draw
cycle.

HUD rendering must remain lightweight and avoid unnecessary
recomputation.

Recommended practices:

    - cache layout calculations when possible
    - avoid rebuilding UI elements every frame
    - minimize dynamic allocations during rendering
    - restrict expensive layout recalculations to state changes

HUD updates should occur only when relevant task state or configuration
changes occur.

## 19.8 Debug feature impact ##

Debug and development tools described in Section 17 may introduce
additional computational overhead.

Debug diagnostics must be disabled by default during normal gameplay.

When enabled, debug systems should:

    - limit diagnostic update frequency
    - avoid scanning large data structures every frame
    - allow selective enabling of specific debug tools

## 19.9 Memory allocation guardrails ##

Subsystems must avoid excessive runtime memory allocation.

Frequent allocations may introduce garbage collection pauses that affect
gameplay smoothness.

Preferred practices:

    - reuse collections when possible
    - avoid allocating objects inside tight loops
    - cache frequently used objects

## 19.10 Performance monitoring ##

Optional performance diagnostics may be provided through the debug
system.

Diagnostics may include:

    - generator execution time
    - rule evaluation duration
    - reconciliation duration
    - HUD rendering cost

These diagnostics assist developers in identifying performance
bottlenecks during development.

## 19.11 Performance constraints ##

The system must follow these performance constraints:

1. Evaluation must be event-driven rather than frame-driven.
2. Generators must avoid full-world scans.
3. Rule evaluation must operate on pre-collected context data.
4. HUD rendering must minimize per-frame computation.
5. Debug diagnostics must remain optional.

These constraints ensure the mod operates smoothly even in complex save
files with large numbers of entities.
