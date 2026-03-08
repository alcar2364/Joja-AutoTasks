---
name: agent-capability-freshness
description: >-
  Validates agent descriptions match actual responsibilities, tools, and body
  content. Prevents description drift and improves discoverability.
trigger: after-edit
applyTo: ".local/Agents/**/*.agent.md"
---

# Agent Capability Freshness Hook #

    * Trigger: after editing `.agent.md` files.
    * Purpose: keep description, tools, body, and handoffs aligned.

## Validation Scope ##

Run these checks after each agent-file edit:

1. Description keyword coverage.
2. Tool grant and responsibility alignment.
3. Responsibility coverage in body sections.
4. Exclusion and handoff consistency.
5. `argument-hint` completeness.
6. Source-of-truth reference validity.
7. Handoff prompt specificity.
8. Name and filename consistency.
9. Trigger phrase coverage for likely user wording.

## 1. Description Keyword Coverage ##

    * Extract core responsibilities from the body and exclusions.
    * Ensure the description includes high-signal trigger terms for those responsibilities.
    * Flag missing responsibility keywords as a major drift issue.

Example recommendation:

    Body includes task generation and migrations, but description omits both.
    Add "task generation" and "migrations" to improve discovery.

## 2. Tool and Description Alignment ##

    * Read-only descriptions should not carry write tools unless explicitly justified.
    * Implementation descriptions must include required write tools.
    * Flag mismatches and provide one clear correction path.

Mismatch example:

    description: "Gather context and research codebase patterns"
    tools: [read, search, edit]

## 3. Responsibility and Body Alignment ##

    * Each responsibility listed in section 1 must be explained in operating guidance.
    * If a responsibility is declared but not operationalized, flag a coverage gap.
    * Suggest adding focused guidance where the gap exists.

## 4. Exclusions and Handoffs ##

    * If the agent excludes work, verify there is a handoff to the owning agent.
    * Flag exclusion-without-handoff as a major routing issue.

## 5. Argument Hint Completeness ##

    * Ensure `argument-hint` reflects what the workflow actually expects.
    * If body expects plan, scope constraints, or target files, hint should mention them.

## 6. Source-of-Truth Reference Validity ##

    * Validate each cited contract or instruction path exists.
    * Flag missing references as critical.
    * Suggest likely intended replacement paths when possible.

## 7. Handoff Prompt Specificity ##

    * Handoff prompts must include action, scope, and expected output.
    * Flag vague prompts and provide a sharpened prompt template.

Sharpened pattern:

    Resolve architecture uncertainty for backend persistence changes.
    Return a concrete next-step plan with constraints and target files.

## 8. Name and Filename Consistency ##

    * `name` in frontmatter should match filename stem.
    * Flag mismatches as critical because they break discovery and handoff routing.

## 9. Trigger Phrase Coverage Test ##

    * Compare description text against common user request phrases.
    * If common phrases have weak match coverage, suggest minimal keyword additions.

## Validation Report Format ##

When output is needed, use this structure:

    AGENT CAPABILITY FRESHNESS VALIDATION
    Agent: [Name]
    Checks: [list]
    Critical: [count]
    Major: [count]
    Minor: [count]
    Overall: [FRESH | NEEDS UPDATES | CRITICAL ISSUES]
    Actions: [specific fixes]

## Output Behavior ##

    * Silent when all checks pass or changes are purely internal wording tweaks.
    * Emit output for description drift, missing tools, broken references, or critical misalignment.

## Integration ##

Works with:

    * `agent-ecosystem-sync` for cross-file reference updates.
    * `design-guide-contract-sync` for source-of-truth updates after contract changes.
    * `handoff-optimizer` for handoff prompt quality.
