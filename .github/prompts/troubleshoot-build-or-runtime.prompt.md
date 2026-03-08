---
name: "Troubleshoot Build Or Runtime Issue"
description: "Use when: diagnosing build failures, runtime bugs, or tooling/environment issues with root-cause focus."
argument-hint: "Symptoms + error output + recent changes + scope"
agent: "Troubleshooter"
---

Investigate the issue below and focus on root cause before proposing fixes.

Troubleshooting Inputs
- Symptoms: <required>
- Error logs/output: <required if available>
- Recent changes: <optional>
- Scope boundaries: <required>

Troubleshooting Expectations
- Reproduce or triangulate the failure path.
- Identify the most likely root cause with evidence.
- Propose minimal, safe fixes in priority order.
- Note validation steps to confirm resolution.

Required Output
1. Root cause analysis
2. Evidence summary
3. Candidate fixes (ordered)
4. Validation checklist and rollback notes
