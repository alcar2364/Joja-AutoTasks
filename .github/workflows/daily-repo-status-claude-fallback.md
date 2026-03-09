---
name: daily-repo-status-claude-fallback
description: "Fallback daily report workflow when Codex is unavailable or fails."
on:
  workflow_run:
    workflows: [daily-repo-status-codex]
    types: [completed]
    branches: [main]
  workflow_dispatch:
permissions:
  contents: read
  actions: read
  issues: read
  pull-requests: read
strict: true
network:
  allowed: [defaults, github]
if: ${{ github.event_name == 'workflow_dispatch' || github.event.workflow_run.conclusion == 'failure' }}
engine:
  id: claude
tools:
  github:
    toolsets: [default]
safe-outputs:
  create-issue:
    title-prefix: "[daily repo report] "
    labels: [agentic-workflow, daily-report, fallback-claude]
    close-older-issues: true
    max: 1
---

# Daily Repo Status Report (Claude Fallback)

Run this report when the Codex workflow fails or when manually triggered.

Context:
- Repository: `${{ github.repository }}`
- Upstream workflow conclusion: `${{ github.event.workflow_run.conclusion }}`

Tasks:
1. Summarize the last 24 hours of repository activity:
   - Notable commits and branch activity
   - Pull request and issue movement
   - Workflow failures and operational risks
2. Identify blockers and recommend the top 3 next actions.

Output requirements:
- Emit exactly one `create_issue` safe output.
- Title format: `Daily Repo Report - YYYY-MM-DD (UTC) - Claude Fallback`.
- Body format:
  - `## Snapshot`
  - `## Notable Changes`
  - `## Risks`
  - `## Recommended Actions`
- If required data is missing, emit `missing_data` instead of guessing.
