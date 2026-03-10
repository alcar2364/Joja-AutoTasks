---
name: repository-quality-holistic-analyzer
description: "Daily holistic analysis of repository quality from multiple dimensions: architecture, patterns, conventions."
on:
  schedule: daily
  workflow_dispatch:
permissions:
  contents: read
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
    title-prefix: "[quality] "
    labels: [agentic-workflow, quality, needs-review]
---

# Repository Quality Holistic Analyzer

Rotating daily analysis of repository quality from different architectural and quality dimensions.

## Rotation Schedule

Analyzes one focus area per day, cycling through:

1. **Domain Architecture** — Identifier usage, aggregate design, boundary clarity
2. **Lifecycle Coordination** — Signal propagation, event ordering, determinism
3. **State Management** — Command patterns, persistence contracts, determinism
4. **Testing Infrastructure** — Test coverage, determinism verification, architecture boundaries
5. **Documentation Alignment** — Design guides match implementation, contracts are current
6. **Code Consistency** — Naming conventions, style compliance, pattern adherence
7. **Configuration Safety** — Config migration integrity, version handling, immutability

## Analysis Process

1. **Select focus area** from 7-day rotation (cache-based)
2. **Deep dive analysis:**
   - Scan relevant code subsystem
   - Identify patterns and anomalies
   - Cross-reference with design guides
   - Score improvement opportunities

3. **Compile recommendations:**
   - What's working well (positive note)
   - What needs improvement
   - Concrete suggestions (don't implement)

4. **Create issue** with findings

## Output

- Post weekly issue for each focus area (7 issues/week)
- Title: `[quality] <FocusArea>: <Primary Finding>`
- Include:
  - Summary of current state
  - Strengths and areas for improvement
  - Specific examples from codebase
  - Actionable recommendations

## Notes

- Findings posted as issues for tracking and resolution
- Focus on systemic quality, not individual bug fixes
- Reference relevant architecture contracts in findings
