---
name: skills-index-auto-sync
description: >-
  Event-driven automation for skill maintenance: syncs
  .local/Agents/Skills/README.md after skill-file edits.
trigger: after-edit
applyTo: ".local/Agents/Skills/*.skill.md"
---

# Skills Index Auto-Sync Hook #

    * Trigger: after editing any `.skill.md` file in `.local/Agents/Skills/`.
    * Purpose: keep the skill catalog synchronized without manual index maintenance.

## Post-Edit Automation Flow ##

### 1. Detect Scope and Skip Conditions ##

Proceed only when the edited file matches:

    .local/Agents/Skills/*.skill.md

Skip when any of the following is true:

    * Only `README.md` changed.
    * No `.skill.md` files exist.
    * The change is outside `.local/Agents/Skills/`.

### 2. Build Current Skill Snapshot ##

From all skill files in `.local/Agents/Skills/`:

1. Enumerate `*.skill.md`.
2. Parse frontmatter fields: `name`, `description`.
3. Record skill file, skill name, and best-use description (first sentence of description).

### 3. Categorize Skills ##

Organize skills into logical categories based on their purpose:
Foundation, UI and StarML, Architecture, Implementation, Lifecycle, and Maintenance.

    * Foundation Skills: External resources, visual design (universal reference)
    * UI & StarML: UI patterns, component composition, snapshot binding
    * Architecture & State: Command/reducer, identifiers, persistence, task generation
    * Implementation & Testing: Testing patterns, error handling, dependency injection
    * Lifecycle & Integration: Event dispatch, game coupling
    * Maintenance & Debugging: Build workflow, SMAPI debugging

### 4. Synchronize Skill Index ##

Target index file:

    .local/Agents/Skills/README.md

Synchronization requirements:

    * Ensure each skill appears exactly once in the appropriate category section.
    * Add entries for new skill files.
    * Remove entries for deleted skill files.
    * Update entries when `name` or `description` changes (extract first sentence as best-use).
    * Keep entries sorted by skill filename within each category for deterministic output.
    * Create new category sections if skills are added to new domains.

### 5. Apply Safe Update Policy ##

    * Prefer minimal diffs when updating `README.md`.
    * Preserve sections unrelated to skill indexing (Skills vs Instructions, Notes).
    * Do not rewrite prose unless needed for index consistency.
    * Keep links relative inside the skills folder.
    * Maintain the two-column Skill Catalog table format.

### 6. Validation Checkpoint ##

After synchronizing:

    * Verify each `.skill.md` file row contains: File, Skill Name, Best Use.
    * Confirm no duplicate skill entries exist.
    * Check that deleted skills are removed from the index.
    * Ensure all `.skill.md` files are represented in some category.

If validation fails, do not save; report the inconsistency for manual review.

## Implementation Notes ##

    * Use file listing and YAML frontmatter parsing to extract skill metadata.
    * Sort within each category by filename to ensure deterministic output.
    * Extract the first sentence from the `description` field as best-use guidance.
    * Preserve the exact formatting of category headers and table structure.
    * When a skill is renamed (file stem changes) or deleted, handle gracefully by removing old entries.
