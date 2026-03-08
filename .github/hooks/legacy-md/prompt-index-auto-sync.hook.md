---
name: prompt-index-auto-sync
description: >-
  Event-driven automation for prompt maintenance: syncs
  .local/Agents/Prompts/README.md after prompt-file edits.
trigger: after-edit
applyTo: ".local/Agents/Prompts/*.prompt.md"
---

# Prompt Index Auto-Sync Hook #

    * Trigger: after editing any `.prompt.md` file in `.local/Agents/Prompts/`.
    * Purpose: keep the prompt catalog synchronized without manual index maintenance.

## Post-Edit Automation Flow ##

### 1. Detect Scope and Skip Conditions ##

Proceed only when the edited file matches:

    .local/Agents/Prompts/*.prompt.md

Skip when any of the following is true:

    * Only `README.md` changed.
    * No `.prompt.md` files exist.
    * The change is outside `.local/Agents/Prompts/`.

### 2. Build Current Prompt Snapshot ##

From all prompt files in `.local/Agents/Prompts/`:

1. Enumerate `*.prompt.md`.
2. Parse frontmatter fields: `name`, `description`, and `agent`.
3. Record prompt file, prompt name, best-use description, and target agent.

### 3. Synchronize Prompt Index ##

Target index file:

    .local/Agents/Prompts/README.md

Synchronization requirements:

    * Ensure each prompt appears exactly once in the Prompt Catalog section.
    * Add entries for new prompt files.
    * Remove entries for deleted prompt files.
    * Update entries when `name`, `description`, or `agent` changes.
    * Keep entries sorted by prompt filename for deterministic output.
    * Keep Quick Picker and Prompt Catalog aligned.

### 4. Apply Safe Update Policy ##

    * Prefer minimal diffs when updating `README.md`.
    * Preserve sections unrelated to prompt indexing.
    * Do not rewrite prose unless needed for index consistency.
    * Keep links relative inside the prompts folder.

### 5. Validation Checkpoint ##

After synchronization, verify:

    * [ ] Every prompt file appears in the Prompt Catalog.
    * [ ] No stale prompt entries remain.
    * [ ] All prompt links resolve with folder-relative paths.
    * [ ] Agent names match existing `.local/Agents/*.agent.md` names where possible.

If validation fails, emit a warning and suggest this fallback:

    Run refresh-prompt-index.prompt.md in preview mode, then apply mode.

## Output Behavior ##

    * Silent when synchronization succeeds.
    * Emit output only when sync is skipped, frontmatter parsing fails, or validation fails.

## Integration ##

Works with:

    * `refresh-prompt-index.prompt.md` for manual fallback and bulk repair.
    * `choose-workflow-prompt.prompt.md` for prompt-maintenance routing.
