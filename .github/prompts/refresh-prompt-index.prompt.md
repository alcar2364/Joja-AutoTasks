---
name: "Refresh Prompt Index"
description: "Use when: syncing the prompt catalog README after adding, renaming, or removing prompt files."
argument-hint: "Prompt folder + README path + apply or preview mode"
agent: "Orchestrator"
---

Synchronize the prompt index README with the current prompt files.

Maintenance Inputs
- Prompt directory: <default .github/prompts>
- README file: <default .github/prompts/README.md>
- Sort mode: <filename|prompt-name|agent>
- Update mode: <preview changes|apply changes>

Sync Rules
1. Discover all `*.prompt.md` files in the prompt directory.
2. Parse frontmatter fields: `name`, `description`, and `agent`.
3. Ensure each prompt appears exactly once in the Prompt Catalog table.
4. Add new prompts and remove stale entries for deleted files.
5. Keep the Quick Picker and Prompt Catalog aligned.
6. Preserve any sections outside index content unless explicitly asked to rewrite.

Required Output
1. Sync summary (added, updated, removed entries)
2. Prompt Catalog updates with rationale
3. Validation checks performed
4. If preview mode: proposed patch only
