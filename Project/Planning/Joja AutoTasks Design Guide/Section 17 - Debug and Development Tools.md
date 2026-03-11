# Section 17 - Debug and Development Tools #

## 17.1 Purpose ##

Debug and development tools provide visibility into the internal state
of the task engine during development and troubleshooting.

These tools allow developers to inspect rule evaluation, generator
behavior, task state transitions, and UI layout behavior without
modifying compiled code.

Debug tools must be optional and disabled during normal gameplay.

## 17.2 Debug system scope ##

The debug system provides development support for the following areas:

    - Rule evaluation diagnostics
    - Generator execution diagnostics
    - Task state inspection
    - HUD layout tuning
    - Performance diagnostics
    - Development commands

All debug features must be controlled through the Debug configuration
defined in Section 15.

## 17.3 Debug configuration menu ##

The system must provide a dedicated debug configuration menu accessible
from the primary configuration menu.

This menu allows developers to modify certain runtime parameters while
the game is running.

The debug menu must be implemented using StardewUI and must operate on
the same `ModConfig.Debug` configuration object described in Section 15.

The debug menu design should be simple and focused on key development parameters,
no artistic design is required.

Changes made in the debug menu must take effect immediately without
requiring a mod reload if possible.

## 17.4 Runtime parameter tuning ##

Certain development parameters must be exposed as adjustable values
through the debug configuration menu.

These parameters allow developers to modify behavior in real time while
the game is running.

Example adjustable parameters include:

    - HUD layout offsets
    - HUD spacing constants
    - Scroll speed values
    - Animation timing constants
    - Debug overlay visibility

These parameters enable iterative UI tuning without recompiling the mod
DLL.

Changes must apply immediately to the relevant subsystem.

## 17.5 HUD and Menu layout tuning ##

HUD and menu layout development requires frequent adjustment of positional and
spacing values.

To support this workflow, the debug system must allow live editing of
selected HUD and menu layout constants.

Example parameters:

    - HUD anchor offsets
    - Task row spacing
    - Scroll bar dimensions
    - Panel padding values
    - Text offset adjustments

The HUD and menu subsystems described in Section 21 must read these values from
runtime configuration rather than static constants when debug tuning is
enabled.

## 17.6 Rule evaluation diagnostics ##

The debug system must support inspection of rule evaluation behavior.

Diagnostics may include:

    - Rule evaluation results
    - Trigger activation events
    - Progress calculation results
    - Disabled rule reasons

Diagnostics should be written to the mod log and may optionally be
displayed through a debug interface.

## 17.7 Generator diagnostics ##

Generators described in Section 13 may expose diagnostic information
during development.

Diagnostics may include:

    - Generator evaluation duration
    - Number of tasks produced
    - Entities scanned during evaluation
    - Generator failure events

These diagnostics assist in identifying inefficient or failing
generators.

## 17.8 Task state inspector ##

The debug system should provide a task state inspector allowing
developers to view the current state of tasks stored in the State Store.

The inspector may display:

    - `TaskID`
    - Generator source
    - Rule source
    - Progress values
    - Completion state
    - Creation timestamp

This tool assists in verifying rule behavior and generator output.

## 17.9 Debug HUD overlay ##

An optional debug overlay may be displayed on screen to visualize
runtime state.

Possible overlay information includes:

    - Active tasks currently visible in the HUD
    - Layout bounding boxes
    - Interaction zones
    - Scroll state values
    - Performance metrics

The overlay must be disabled by default and enabled only through debug
configuration.

## 17.10 Logging controls ##

The debug system must provide configuration flags controlling diagnostic
logging behavior.

Examples include:

    - Enable rule evaluation logging
    - Enable generator diagnostics
    - Enable command tracing
    - Enable persistence diagnostics

Logging verbosity must be adjustable to avoid excessive log output.

## 17.11 Developer commands ##

The debug system may expose developer commands accessible through the
SMAPI console.

Example commands include:

    - Force rule evaluation
    - Regenerate generated tasks
    - Reset task state
    - Dump task state to log
    - Toggle debug overlay

Developer commands must be restricted to debug mode.

## 17.12 Debug system constraints ##

The debug system must follow these constraints:

1. Debug features must not alter deterministic rule evaluation.
2. Debug tools must not corrupt task state.
3. Debug features must be disabled by default in release gameplay.
4. Debug tools must be isolated from core engine logic.

These constraints ensure that development tools do not affect the
correctness of the task engine.
