---
name: daily-repo-status-codex
description: "Daily repository report with Codex primary and fallback chain."
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
  id: codex
jobs:
  detect_changes:
    runs-on: ubuntu-latest
    outputs:
      has_changes: ${{ steps.detect.outputs.has_changes }}
      commit_count: ${{ steps.detect.outputs.commit_count }}
      since_utc: ${{ steps.detect.outputs.since_utc }}
    steps:
      - name: Check default branch commits since UTC midnight
        id: detect
        env:
          GH_TOKEN: ${{ secrets.GITHUB_TOKEN }}
          REPO: ${{ github.repository }}
          DEFAULT_BRANCH: ${{ github.event.repository.default_branch }}
        run: |
          since="$(date -u '+%Y-%m-%dT00:00:00Z')"
          repo="$REPO"
          default_branch="$DEFAULT_BRANCH"
          commit_count="$(gh api "repos/${repo}/commits?sha=${default_branch}&since=${since}&per_page=1" --jq 'length')"

          if [ "${commit_count}" -gt 0 ]; then
            has_changes=true
          else
            has_changes=false
          fi

          {
            echo "has_changes=${has_changes}"
            echo "commit_count=${commit_count}"
            echo "since_utc=${since}"
          } >> "$GITHUB_OUTPUT"
if: ${{ needs.detect_changes.outputs.has_changes == 'true' }}
tools:
  github:
    toolsets: [default]
safe-outputs:
  create-issue:
    title-prefix: "[daily repo report] "
    labels: [agentic-workflow, daily-report]
    close-older-issues: true
    max: 1
---

# Daily Repo Status Report (Codex Primary)

Create a single daily report for `${{ github.repository }}`.

Context:
- Default branch: `${{ github.event.repository.default_branch }}`
- Commit scan start (UTC): `${{ needs.detect_changes.outputs.since_utc }}`
- Commit count since UTC midnight: `${{ needs.detect_changes.outputs.commit_count }}`

Tasks:
1. Summarize repository activity in the last 24 hours:
   - Code changes and commits on default branch
   - Opened, closed, and merged pull requests
   - Issue movement and discussion highlights
2. Identify top risks or blockers.
3. Recommend the next 3 maintainer actions.

Output requirements:
- Emit exactly one `create_issue` safe output.
- Title format: `Daily Repo Report - YYYY-MM-DD (UTC) - Codex`.
- Body format:
  - `## Snapshot`
  - `## Notable Changes`
  - `## Risks`
  - `## Recommended Actions`
- Keep it concise and concrete. If key data is unavailable, emit `missing_data` instead of guessing.
