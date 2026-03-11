# --- Joja AutoTasks Design Guide --- #

## --- [Front Matter] --- ##

Title: Joja AutoTasks Design Guide

Mod Author: Alcar

Repository: <https://github.com/alcar2364/Joja-AutoTasks>

License: MIT

Last Updated:   2026 Mar 10

Delivery Model: No-drop Now/Next/Later sequencing with mandatory stage gates
                (see Section 21).

Sync Policy:    Design guide sections are canonical and updated first;
                Architecture Map is reconciled second (see Section 21.3.3).

Variance Register: Section 21.3.3 (template + seeded VAR-001 baseline entry).

Instructions:

    - Use this file as the primary navigation entry for both readers and LLM chunking.
    - Use [EditingInstructions.md](./EditingInstructions.md) for detailed editing guidelines.

Description:    Comprehensive technical design guide for the Joja AutoTasks mod, detailing
                architecture, data models, task generation, and more. This document
                is the ultimate source of truth for the mod's design and implementation.
                Any conflicts between this guide and other documentation should be resolved in favor
                of this guide.

## --- [Introduction] --- ##

Joja AutoTasks is an automated task management system designed to
integrate directly into Stardew Valley gameplay. The mod provides a
structured framework for detecting, tracking, and organizing player goals
within the game world, allowing players to manage both short-term activities
and long-term objectives without relying on external notes or memory.

Rather than functioning as a simple checklist, Joja AutoTasks operates as a
deterministic task engine that evaluates gameplay state, generates tasks from
multiple sources, tracks progress automatically, and records historical task
data over time. The system is designed to surface relevant information during
gameplay while maintaining a lightweight runtime footprint.

The mod supports three primary task sources:

Built-in automatic tasks derived from game state

Player-defined rule-driven tasks created through the Task Builder

Manual tasks created directly by the player

Tasks generated from these sources are unified into a single task system that
tracks progress, completion state, deadlines, and historical snapshots.

Joja AutoTasks is designed as a deterministic system in which task identity,
evaluation results, and persistence behavior remain stable across save loads
and engine evaluations. The architecture emphasizes separation of concerns
between the task engine, state store, persistence layer, and UI systems in
order to maintain predictable behavior and long-term maintainability.

The mod provides two primary user interface surfaces:

A lightweight in-game HUD designed for moment-to-moment task tracking

A full task management menu providing task browsing, rule creation,
configuration, and historical analysis

The document that follows describes the complete technical design of the
system, including the core data model, rule evaluation engine, persistence
model, UI architecture, and implementation roadmap.

Each section focuses on a specific subsystem or design concern, allowing the
document to serve as both an implementation guide and a long-term reference
for future development.

## --- [Table of Contents] --- ##

## Sections ##

1. [Section 01 - Product Definition]
    * Location: (./Section 01 - Product Definition.md)

2. [Section 02 - System Architecture]
    * Location: (./Section 02 - System Architecture.md)

3. [Section 03 - Deterministic Identifier Model]
    * Location: (./Section 03 - Deterministic Identifier Model.md)

4. [Section 04 - Core Data Model]
    * Location: (./Section 04 - Core Data Model.md)

5. [Section 05 - Task Generation and Evaluation Engine]
    * Location: (./Section 05 - Task Generation and Evaluation Engine.md)

6. [Section 06 - Task Builder Rule Serialization]
    * Location: (./Section 06 - Task Builder Rule Serialization.md)

7. [Section 07 - Rule Evaluation Model]
    * Location: (./Section 07 - Rule Evaluation Model.md)

8. [Section 08 - State Store Command Model]
    * Location: (./Section 08 - State Store Command Model.md)

9. [Section 09 - Persistence Model]
    * Location: (./Section 09 - Persistence Model.md)

10. [Section 10 - UI Data Binding Model]
    * Location: (./Section 10 - UI Data Binding Model.md)

10A. [Section 10A - View Model Architecture]
    - Location: (./Section 10A - View Model Architecture.md)

1. [Section 11 - Daily Snapshot Ledger]
    * Location: (./Section 11 - Daily Snapshot Ledger.md)

2. [Section 12 - Engine Update Cycle]
    * Location: (./Section 12 - Engine Update Cycle.md)

3. [Section 13 - Built-in Task Generators]
    * Location: (./Section 13 - Built-in Task Generators.md)

4. [Section 14 - Task Builder Wizard UX]
    * Location: (./Section 14 - Task Builder Wizard UX.md)

5. [Section 15 - Configuration System]
    * Location: (./Section 15 - Configuration System.md)

6. [Section 16 - Error Handling and Rule Validation]
    * Location: (./Section 16 - Error Handling and Rule Validation.md)

7. [Section 17 - Debug and Development Tools]
    * Location: (./Section 17 - Debug and Development Tools.md)

8. [Section 18 - Versioning and Migration Strategy]
    * Location: (./Section 18 - Versioning and Migration Strategy.md)

9. [Section 19 - Performance Guardrails]
    * Location: (./Section 19 - Performance Guardrails.md)

10. [Section 20 - UI System Design]
    * Location: (./Section 20 - UI System Design.md)

11. [Section 21 - Implementation Plan]
    * Location: (./Section 21 - Implementation Plan.md)

## Instruction Files ##

The following instruction files supplement the design guide with focused reference material.

    - [Visual Design Language]
        * Location: (../../instructions/visual-design-language.instructions.md)
        * Visual identity, color palette, typography, spacing, animation, iconography,
          HUD/menu/wizard layout concepts.

    - [External Resources]
        * Location: (../../instructions/external-resources.instructions.md)
        * Links to Stardew Valley modding documentation, StardewUI, and GMCM.

## Related Planning References ##

    - [Architecture Map]
        * Location: (../Architecture Map.md)
        * Code-level architecture map reconciled from canonical design sections
          after Section 21 updates.

    - [Section 21 - Implementation Plan]
        * Location: (./Section 21 - Implementation Plan.md)
        * Canonical no-drop staged delivery model, capability mapping table
          (Section 21.3.1), and variance policy/register (Section 21.3.3).
