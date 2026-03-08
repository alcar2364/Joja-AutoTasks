---
name: design-guide-contract-sync
description: >-
  Ensures contracts stay aligned when Design Guide sections are updated. Flags
  contract drift when architectural rules change.
trigger: after-edit
applyTo: ".local/Joja AutoTasks Design Guide/**/*.md"
---

# Design Guide Contract Sync Hook #

    * Trigger: after editing Design Guide files.
    * Purpose: detect and report drift between Design Guide intent and contract enforcement.

## Section to Contract Mapping ##

Use this mapping for alignment checks:

    * Section 02 System Architecture: backend and frontend architecture contracts (critical).
    * Section 03 Deterministic Identifiers: backend determinism rules (critical).
    * Section 04 Core Data Model: backend model rules (high).
    * Section 05 and Section 13 Task Generation: backend evaluation rules (high).
    * Section 08 State Store Command Model: backend mutation boundary rules (critical).
    * Section 09 Persistence Model: backend persistence rules (critical).
    * Section 10 and 10A UI Data Binding: frontend snapshot-driven UI rules (critical).
    * Section 12 Engine Update Cycle: backend update-cycle rules (high).
    * Section 18 Versioning and Migration: backend migration rules (high).
    * Section 19 Performance Guardrails: backend and frontend performance guidance (medium).
    * Section 20 UI System Design: frontend surface and boundary rules (critical).

## Change Classification ##

Classify each edit as one of:

    * Rule addition.
    * Rule modification.
    * Rule removal.
    * Example update.
    * Clarification only.

Priority model:

    * Critical: mutation boundaries, determinism, UI/backend separation.
    * High: persistence, identifiers, evaluation cycle.
    * Medium: performance and pattern guidance.
    * Low: wording-only clarifications.

## Drift Detection Procedure ##

1. Identify edited Design Guide section and mapped contracts.
2. Compare updated rule intent to current contract language.
3. Flag missing, stale, or contradictory enforcement statements.
4. Recommend concrete contract updates for each drift finding.

Drift output template:

    DESIGN GUIDE CONTRACT DRIFT DETECTED
    Edited section: [section]
    Mapped contract: [contract]
    Design Guide change: [summary]
    Contract gap: [summary]
    Recommended update: [exact rule text or section target]

## Bidirectional Sync Rule ##

When contracts are edited elsewhere, this hook also checks whether the Design Guide needs updates:

    * If a contract adds an official rule not present in Design Guide, flag for guide update.
    * If a contract removes an enforced rule still in Design Guide, flag mismatch for review.

## Section Reference Validation ##

When sections are renamed or renumbered:

    * Find citations in agents, contracts, and hook docs.
    * Flag stale section numbers and suggest direct replacements.

## New Section Handling ##

When a new Design Guide section is created:

    * Determine whether it introduces enforceable architecture rules.
    * Recommend one of these actions: create a new contract, extend an existing contract,
      or keep it as documentation-only guidance.

## Example Synchronization ##

If Design Guide code examples change, check related contract examples for stale patterns.
Flag mismatches and suggest synchronized updates.

## Validation Checklist ##

    * [ ] Mapped contracts still match edited Design Guide rules.
    * [ ] New rules are enforced where required.
    * [ ] Removed rules are removed or marked deprecated in contracts.
    * [ ] Section references are current.
    * [ ] Example snippets are synchronized where they encode rules.
    * [ ] No contradiction remains between guide and contract text.

## Output Behavior ##

    * Silent for pure clarifications with no rule impact.
    * Emit output for critical or high-priority drift and for stale section references.

## Integration ##

Works with:

    * `agent-ecosystem-sync` for reference propagation after contract updates.
    * `contract-auto-loader` so updated contracts are loaded on future edits.
    * `design-guide-context-augmenter` so updated sections are used in planning.
