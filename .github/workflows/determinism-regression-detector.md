---
name: determinism-regression-detector
description: "Scans C# source files for non-deterministic patterns that violate the JAT determinism contract."
on:
  pull_request:
    branches: [main]
    types: [opened, synchronize, reopened]
  schedule: weekly
  workflow_dispatch:
permissions:
  contents: read
  pull-requests: write
strict: true
network:
  allowed: [defaults, github]
engine:
  id: copilot
tools:
  github:
    toolsets: [default]
safe-outputs:
  create-discussion:
    category: general
    title-prefix: "[determinism] "
---

# Determinism Regression Detector — Non-Deterministic Pattern Scanner

Protect JAT's first-class determinism requirement by scanning for forbidden patterns that can cause
unstable TaskId generation, ordering regressions, or save-state corruption.

## Context

- Repository: `${{ github.repository }}`
- Determinism contract: Section 3 of the JAT Design Guide
- Source directories: `Domain/`, `StateStore/`, `Lifecycle/`, `Configuration/`, `Startup/`, `Events/`
- Excluded: `Tests/` (test code may legitimately use random/GUID for seeding and mock data)

## Forbidden Patterns (Production Code Only)

### Pattern 1: Random GUID Generation

```
Guid.NewGuid()
```

Context: Task IDs must be derived from stable inputs (source type + subject identity + day key).
Any use of `Guid.NewGuid()` in production code creates an unstable identity that breaks save/load.

### Pattern 2: Unseeded Randomness

```
new Random()
new Random(Environment.TickCount)
```

Context: A `Random` instance without a deterministic seed produces different values across runs.
If used in ID generation or ordering, this corrupts task state stability.

### Pattern 3: Wall-Clock Time in ID/Ordering Paths

```
DateTime.Now
DateTime.UtcNow
DateTimeOffset.Now
Environment.TickCount
Stopwatch.GetTimestamp()
```

Context: Wall-clock time must not feed into deterministic identifiers or sort keys. It is
acceptable in logging and performance measurements, but only those. Flag all occurrences for manual
review.

### Pattern 4: Unordered Collection Traversal into Snapshot Ordering

```
.GetEnumerator()
foreach.*Dictionary
foreach.*HashSet
```

followed immediately by a sort or projection without an explicit stable key. These are heuristic
patterns; flag for human review only when the traversal result is fed into a list or array used
as a final output.

### Pattern 5: Environment-Dependent Identifiers

```
Environment.MachineName
Environment.UserName
Environment.GetEnvironmentVariable(
```

Context: Identifiers that depend on the execution environment will differ between developer
machines and CI, breaking the deterministic ID contract.

## Scan Process

1. **On PR:** Scan only the changed `.cs` files in the diff
2. **On schedule:** Scan all `.cs` files in production source directories
3. **For each pattern:** Report file path and line number
4. **Classify severity:**
   - Patterns 1 and 2 in `Domain/` or `StateStore/`: **Blocker**
   - Patterns 1 and 2 in other directories: **Major**
   - Pattern 3: **Major** (manual review required)
   - Patterns 4 and 5: **Minor** (advisory)

## Output

### On PR

Post a PR review comment listing all matches with:

- Pattern type
- File path and line number
- Suggested deterministic alternative

### On Schedule

Post a discussion if any patterns found in full scan:

- Summary of patterns found per directory
- Trend vs. previous week (new, unchanged, resolved)
- If no patterns found: skip discussion (silence is success)

## Notes

- False positive rate is expected to be very low (these patterns rarely appear in domain code)
- Test exclusion is critical: `Tests/` directory must be excluded in all scans
- The goal is zero matches in production source; treat any match as a potential regression
- Reference: `.github/Project Planning/Joja AutoTasks Design Guide/Section 03 - Deterministic Identifier Model.md`
