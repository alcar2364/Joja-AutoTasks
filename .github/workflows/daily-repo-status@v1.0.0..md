---
name: daily-repo-status
description: "Provides a daily status report for the repo"
on:
  schedule: daily
  workflow_dispatch:
  workflow_run:
    workflows: [build-health-monitor, daily-docs-updater]
    types: [completed]
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

# Daily Repo Status Report (Copilot Final Fallback)

Run this report daily

Context:
- Repository: `${{ github.repository }}`
- Trigger event: `${{ github.event_name }}`
- Upstream workflow conclusion: `${{ github.event.workflow_run.conclusion }}`

Tasks:
1. Summarize the last 24 hours of repository activity:
   - Notable commits and branch activity
   - Pull request and issue movement
   - Workflow failures and operational risks
2. Identify blockers and recommend the top 3 next actions.

Output requirements:
- Emit exactly one `create_issue` safe output.
- Title format: `Daily Repo Report - YYYY-MM-DD (UTC) - Copilot Final Fallback`.
- Body format:
  - `## Snapshot`
  - `## Notable Changes`
  - `## Risks`
  - `## Recommended Actions`
- If required data is missing, emit `missing_data` instead of guessing.
