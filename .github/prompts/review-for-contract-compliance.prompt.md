---
name: "Review For Contract Compliance"
description: "Use when: reviewing code or plans for correctness, contract compliance, and regression risks."
argument-hint: "What to review + scope + expected constraints"
agent: "Reviewer"
---

Review the target change with a contract-first mindset.

Review Inputs
- Change under review: <patch|files|plan>
- Scope constraints: <required>
- Context and assumptions: <optional>

Review Expectations
- Prioritize bugs, regressions, determinism risks, and boundary violations.
- Separate contract issues from style preferences.
- Call out missing verification coverage.

Required Output
1. Findings by severity (Blocker, Major, Minor, Note)
2. Open questions and assumptions
3. Verification checklist
4. Acceptance statement
