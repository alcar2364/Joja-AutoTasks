---
name: daily-docs-updater
description: "Daily documentation synchronization: detects code changes and identifies design doc updates needed."
on:
  schedule: daily
  workflow_dispatch:
permissions:
  contents: read
  actions: read
  issues: read
  pull-requests: read
strict: true
network:
  allowed: [defaults, github]
engine:
  id: copilot
jobs:
  detect_changes:
    runs-on: ubuntu-latest
    outputs:
      has_changes: ${{ steps.detect.outputs.has_changes }}
      changed_files: ${{ steps.detect.outputs.changed_files }}
      since_utc: ${{ steps.detect.outputs.since_utc }}
    steps:
      - name: Check for code changes since UTC midnight
        id: detect
        env:
          GH_TOKEN: ${{ secrets.GITHUB_TOKEN }}
          REPO: ${{ github.repository }}
          DEFAULT_BRANCH: ${{ github.event.repository.default_branch }}
        run: |
          since="$(date -u '+%Y-%m-%dT00:00:00Z')"
          repo="$REPO"
          default_branch="$DEFAULT_BRANCH"
          
          # Get commits since midnight
          files="$(gh api "repos/${repo}/commits?sha=${default_branch}&since=${since}&per_page=100" --jq '[.[].files[].filename] | unique | @csv' | tr -d '"')"
          
          if [ -n "$files" ]; then
            has_changes=true
          else
            has_changes=false
          fi
          
          {
            echo "has_changes=${has_changes}"
            echo "changed_files=${files}"
            echo "since_utc=${since}"
          } >> "$GITHUB_OUTPUT"
if: ${{ needs.detect_changes.outputs.has_changes == 'true' }}
tools:
  github:
    toolsets: [default]
safe-outputs:
  create-issue:
    title-prefix: "[docs] "
    labels: [agentic-workflow, documentation, needs-review]
    close-older-issues: false
    max: 3
---

# Daily Documentation Updater

Analyze code changes and identify design documentation that needs synchronization.

## Context

- Repository: `${{ github.repository }}`
- Default branch: `${{ github.event.repository.default_branch }}`
- Analysis period: Since `${{ needs.detect_changes.outputs.since_utc }}` (UTC)
- Changed files: `${{ needs.detect_changes.outputs.changed_files }}`

## Documentation Areas to Monitor

### Architecture & Design Docs
- `.github/Project Planning/Architecture Map.md` — Must stay current with Domain, Lifecycle, and StateStore changes
- `.github/Project Planning/Joja AutoTasks Design Guide/` — Update when core domain concepts, identifiers, or command patterns change
- Phase checklists and atomic commit execution plans — Track implementation progress

### Code-Adjacent Docs
- `Tests/README.md` — Test conventions and focused test commands
- Domain folder READMEs — Document identifier formats, task enums, command structures
- Infrastructure docs under `.github/instructions/` — Keep C# style, architecture contracts, and patterns current

### Instruction Files & Contracts
- `.github/instructions/csharp-style-contract.instructions.md` — Sync if naming/style patterns shift
- `.github/instructions/unit-testing-contract.instructions.md` — Update test patterns and determinism rules
- Backend and frontend architecture contracts — Reflect lifecycle, command, and domain object changes
- `AGENTS.md` and `copilot-instructions.md` — Track validated build commands, dependencies, environment changes

## Analysis Process

1. **Categorize changed files:**
   - Source code (Domain, Lifecycle, Configuration, StateStore, Events, Infrastructure)
   - Tests (Tests/ tree)
   - Config (manifest.json, .csproj, JSON configs)
   - Docs (.md files in .github/)
   - Other

2. **Match code changes to relevant docs:**
   - Domain changes → Architecture Map, design guides, identifier docs
   - Lifecycle/Events changes → architecture contracts, lifecycle coordination docs
   - StateStore/Command changes → command pattern docs, backend architecture
   - Test changes → Tests/README.md, unit testing contract
   - Config changes → manifest updates, build instructions, copilot-instructions.md

3. **For each identified doc gap:**
   - Title: `[docs] <short description of what needs updating>`
   - Labels: `documentation`, `needs-review`
   - Specify:
     - Which code changed and why
     - Which doc(s) are affected
     - Concrete sync suggestion (don't implement; propose for review)
     - Risk if docs stay out of sync (e.g., future contributors misunderstand architecture)

4. **Prioritize by impact:**
   - Critical (affects Architecture Map or core design): Create issue immediately
   - High (affects build/test docs, contracts): Create issue same day
   - Medium (affects peripheral docs): Create issue if accumulation reaches 2+ items
   - Low (typos, formatting): Batch into weekly roundup

5. **Close stale documentation issues:**
   - If an issue was created for a specific commit and that doc is now updated, close it (reference the commit/PR that fixed it)
   - Keep issues open if doc guidance still diverges from implementation

## Output Requirements

- Emit exactly one `create-issue` for each distinct documentation gap (max 3 per run)
- If no gaps detected, emit zero issues
- Title format: `[docs] <subsystem>: <specific action needed>`
  - Examples:
    - `[docs] Architecture: Update identifier format examples after RuleId/SubjectId implementation`
    - `[docs] Contracts: Sync C# style guide after domain naming convention shift`
    - `[docs] Tests: Document new UpdateTickedGuard test patterns`
- Body sections:
  - `## Changed Code` — What changed and which files
  - `## Documentation Impact` — Which docs are affected
  - `## Sync Suggestion` — Concrete proposal (not implementation)
  - `## Risk** — Why staying out of sync is problematic
  - `## References` — Links to relevant design docs and code

---

## Notes

- This workflow does NOT make changes; it identifies documentation gaps for human review
- Intentional code-vs-doc mismatches (documented in AGENTS.md approval gates) should be noted in issue comments
- Focus on design docs, architecture, and contracts; typo/formatting fixes are lower priority
- If multiple issues identify the same problem, close duplicates and consolidate
