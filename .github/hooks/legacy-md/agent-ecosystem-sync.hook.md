---
name: agent-ecosystem-sync
description: >-
  Maintains cross-references when agents, contracts, instructions, or hooks are
  renamed, moved, or have scope changes. Prevents broken references.
trigger: after-edit
applyTo: ".local/Agents/**/*.{md,agent.md,instructions.md,hook.md}"
---

# Agent Ecosystem Sync Hook #

    * Trigger: after editing agent customization files.
    * Purpose: keep cross-file references and naming consistent across the ecosystem.

## Change Types to Detect ##

    * Agent rename or move.
    * Contract or instruction rename.
    * Hook rename or `applyTo` scope change.
    * Handoff target changes.
    * Tool grant changes.
    * Source-of-truth list changes.
    * Citation path changes.

## 1. Agent Rename or Move ##

When an agent name or path changes, verify references in:

    * `.github/copilot-instructions.md`.
    * Other agents `handoffs` sections.
    * Other agents `agents` lists.
    * `AGENTS.md` if present.
    * Any customization docs that mention the old name.

Flag stale references and propose exact replacements.

## 2. Contract or Instruction Rename ##

When a contract or instruction file changes name, verify updates in:

    * Agent source-of-truth lists.
    * `.github/copilot-instructions.md` instruction references.
    * Cross-references inside other contract files.
    * Related `applyTo` guidance where naming is embedded in prose.

## 3. Hook Rename or Scope Change ##

Validate:

    * Hook filename and frontmatter `name` are still aligned.
    * `applyTo` still covers intended files.
    * Hook is still discoverable from Hooks README where applicable.
    * Overlap with nearby hooks is intentional, not accidental duplication.

## 4. Handoff Reference Validation ##

For every edited handoff:

    * Confirm the referenced agent exists.
    * Confirm `agent` value matches real name exactly, including case.
    * Confirm prompt is specific enough to produce actionable output.
    * Detect circular routes without progress criteria.

Broken reference output pattern:

    BROKEN HANDOFF REFERENCE
    File: [path]
    Handoff label: [label]
    Target: [agent]
    Issue: Target agent not found
    Fix: Replace with existing agent name [candidate]

## 5. Tool Restriction Changes ##

When `tools` changes, verify:

    * All tool names are valid.
    * Tool grant level matches the description and responsibilities.
    * Read-only agents do not gain write tools without explicit rationale.
    * No Swiss-army bloat appears in focused agents.

## 6. Source-of-Truth List Freshness ##

When contracts are added, removed, or renamed:

    * Add new required references where the agent depends on those rules.
    * Remove stale references that no longer exist.
    * Keep precedence order consistent with workspace contract hierarchy.

## 7. Citation Path Validation ##

Check all edited markdown links and file citations for:

    * Correct relative path.
    * Existing target file.
    * Accurate section references when section names are cited.

## 8. Description Keyword Sync ##

If responsibilities change, description must reflect them.
Flag missing new capability keywords and suggest concise additions.

## 9. Batch Update Suggestions ##

When multiple files need updates, provide one batch option:

    ECOSYSTEM SYNC RECOMMENDATIONS
    Affected files: [list]
    Action: apply all updates or review one by one

## Validation Report Format ##

    AGENT ECOSYSTEM SYNC VALIDATION
    Edited file: [path]
    Change type: [type]
    References checked: [count]
    Critical: [count]
    Major: [count]
    Minor: [count]
    Status: [PASS | NEEDS UPDATES | CRITICAL ISSUES]

## Output Behavior ##

    * Silent when no cross-file impact exists.
    * Emit output when stale references, broken handoffs, or inconsistent tool grants are found.

## Severity Levels ##

    * Critical: broken handoffs, missing required files, invalid tool aliases.
    * Major: stale references, description-capability mismatch, unclear tool intent.
    * Minor: optional cleanup opportunities and documentation polish notes.

## Integration ##

Works with:

    * `agent-capability-freshness` for description and tool alignment.
    * `design-guide-contract-sync` for contract reference updates.
    * `contract-auto-loader` to keep updated contracts loaded during edits.
