---
name: starml-output-format
description: "StarMLAgent response template for .sml implementation summaries and verification notes. Use when: reporting completed StarML or StardewUI markup changes."
argument-hint: "Summary of .sml edits and verification results"
---

# StarML Output Format #

Use this skill when drafting StarMLAgent responses after editing `.sml` files.

## Required Sections ##

1. Implementation Summary
2. Files Changed
3. Key Notes
4. Verification Notes
5. Risks / Follow-Ups

## Template ##

## Implementation Summary ##

- What changed in markup.
- Whether the change was additive, corrective, or refactor-only.
- Whether scope remained narrow.

## Files Changed ##

- List each `.sml` file changed.
- Add one-line purpose per file.

## Key Notes ##

- StarML or StardewUI considerations.
- Template/include decisions.
- Boundary notes (snapshot/local-state/command flow).
- Any plan deviations and why.

## Verification Notes ##

- Markup syntax validity.
- Layout hierarchy sanity.
- Binding correctness.
- Event syntax correctness.
- No direct canonical-state ownership in markup.

## Risks / Follow-Ups ##

- List only genuine concerns.
- Note clearly when follow-up belongs to UIAgent or GameAgent.
