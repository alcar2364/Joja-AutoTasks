---
name: agentic-workflow-health-monitor
description: "Meta-workflow that monitors the health of all agentic workflows themselves."
on:
  schedule: daily
  workflow_run:
    workflows: ["*"]
    types: [completed]
    branches: [main]
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
    title-prefix: "[workflow-health] "
    labels: [agentic-workflow, meta, workflow-infrastructure]
    max: 3
---

# Agentic Workflow Health Monitor

Meta-monitoring of all agentic workflows to ensure they are functioning correctly and not creating noise.

## Context

- Repository: `${{ github.repository }}`
- Compiled workflows directory: `.github/workflows/`
- Lock files: `.github/workflows/*.lock.yml`
- Dependency manifest: `.github/aw/actions-lock.json`

## Monitoring Focus

1. **Workflow Execution Health:**
   - Failed workflow runs in last 24 hours
   - Timeout patterns
   - Environment variable/secret issues

2. **Output Quality:**
   - Number of issues/discussions created per workflow
   - Mean time from workflow output to issue/PR closure
   - Merge rate down to implementation (if causal chain exists)

3. **Configuration Health:**
   - Lock file freshness (is it in sync with .md files?)
   - Permissions sufficiency
   - Trigger configuration correctness

4. **Noise Detection:**
   - Workflows creating duplicate issues
   - Workflows creating issues that are immediately closed
   - Workflows targeting already-addressed problems

## Process

1. **Fetch workflow run history** (last 7 days)
2. **Analyze execution patterns** per workflow
3. **Calculate health metrics** (success rate, output quality)
4. **Identify problems:**
   - Failing workflows (environment, logic)
   - Low-value outputs (immediately closed)
   - Configuration drift

5. **Create issue** for problems (max 3 per run)

## Output

- Issue title: `[workflow-health] <WorkflowName>: <Problem description>`
- Include: Metric data, recent runs, root cause analysis
- Suggest remediation (recompile, update config, adjust trigger)

## Notes

- This is the "monitor the monitors" workflow
- Focus on workflow infrastructure, not code quality
- Route implementation fixes to responsible owner
