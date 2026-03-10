---
name: architecture-contract-compliance
description: "Validates architectural boundary contracts on PRs; detects command/state/UI boundary violations."
on:
  pull_request:
    branches: [main]
    types: [opened, synchronize, reopened]
  issue_comment:
    types: [created]
  workflow_dispatch:
permissions:
  contents: read
  pull-requests: read
  issues: read
strict: true
network:
  allowed: [defaults, github]
engine:
  id: copilot
tools:
  github:
    toolsets: [default]
safe-outputs:
  add-comment:
    type: pr-review
if: |
  github.event_name == 'pull_request' ||
  github.event_name == 'workflow_dispatch' ||
  (github.event_name == 'issue_comment' && contains(github.event.comment.body, '/arch-check'))
---

# Architecture Contract Compliance — PR Boundary Checker

Analyze changed C# files in pull requests for violations of JAT's strict architectural boundaries.
Posts findings as a PR review comment; does not auto-block (requires human sign-off for Blockers).

## Context

- Repository: `${{ github.repository }}`
- Architecture contracts:
  - `.github/instructions/backend-architecture-contract.instructions.md`
  - `.github/instructions/frontend-architecture-contract.instructions.md`
- Key boundaries:
  - State Store is the sole owner of canonical task state
  - All mutations go through command/reducer path only
  - UI consumes snapshots as read-only; never mutates canonical state
  - Task IDs must be deterministic (no `Guid.NewGuid()`, no random seeds)
  - Frontend must not resolve I18n keys in backend paths

## Analysis Focus

### 1. State Ownership Violations

Check for patterns where non-State-Store code directly mutates canonical state:

- Direct property assignment on domain objects outside the reducer path
- Collection manipulation (`Add`, `Remove`, `Clear`) on canonical task lists outside the State Store
- `TaskState` / `TaskObject` mutation in UI, Lifecycle, or Events subsystems

### 2. Command/Mutation Path Bypass

Check that state changes flow through commands:

- Any method named `Set*`, `Update*`, `Mutate*` outside `StateStore/` should be flagged
- Direct field assignments to canonical state fields outside designated reducers

### 3. Non-Deterministic ID Generation

Check for forbidden ID generation patterns in non-test source:

- `Guid.NewGuid()` usage in `Domain/`, `StateStore/`, `Lifecycle/`, `Configuration/` directories
- `new Random()` without an explicit deterministic seed in production code paths
- `Environment.TickCount` or `DateTime.Now`-based identifiers

### 4. UI / Backend Boundary

Check for cross-layer violations:

- Backend types (commands, reducers) instantiated directly in UI code
- UI code dispatching state mutations without going through the command interface
- I18n / localization calls (`I18n.*`) in non-UI subsystems

### 5. Identifier Type Safety

Check that canonical identifiers use correct value types:

- `TaskId`, `RuleId`, `SubjectId`, `DayKey` used as `string` where the value type is required
- Canonical identifiers constructed via string concatenation instead of factory methods

## Output Format

Post a single PR review comment with sections:

```
## Architecture Contract Compliance Report

### Blockers (must fix before merge)
[list of violations that break hard contracts]

### Major (strongly recommended)
[list of likely bugs or architectural drift]

### Minor (advisory)
[list of style/naming drifts]

### ✅ Clean areas
[subsystems with no findings]
```

## Notes

- This workflow complements the interactive `/review` command (interactive-code-reviewer)
- Blockers should be flagged as a review request requiring explicit maintainer dismissal
- If no PR diff is available (workflow_dispatch), scan all files in Domain/, StateStore/, Lifecycle/
- Reference the contract files when explaining each finding (link to the relevant contract section)
