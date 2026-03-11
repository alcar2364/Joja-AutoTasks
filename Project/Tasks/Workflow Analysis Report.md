# GitHub Actions Workflow Analysis Report

**Repository:** `alcar2364/Joja-AutoTasks`  
**Prepared by:** Copilot Coding Agent  
**Date:** 2026-03-10  
**Status:** Proposed — pending maintainer review

---

## 1. Executive Summary

This report analyzes the Joja AutoTasks (JAT) repository to identify gaps in its GitHub Actions
automation coverage and recommend new agentic workflows.

**Existing coverage (11 agentic workflows + 1 maintenance):**

| Workflow | Type | Trigger |
|---|---|---|
| `build-health-monitor` | Passive monitoring | Daily + workflow_run |
| `unit-test-coverage-analyzer` | Analysis | Daily |
| `repository-quality-holistic-analyzer` | Analysis | Daily |
| `interactive-code-reviewer` | Interactive | `/review` comment |
| `daily-docs-updater` | Sync detection | Daily |
| `daily-repo-status` | Reporting | Daily |
| `agentic-workflow-health-monitor` | Meta-monitoring | Daily + workflow_run |
| `csharp-type-safety-analyzer` | Analysis | Daily |
| `dotnet-dependency-analyzer` | Analysis | Weekly |
| `interactive-performance-optimizer` | Interactive | `/optimize` comment |
| `csharp-large-class-simplifier` | Analysis | Weekly |
| `agentics-maintenance` | Maintenance | Daily cron |

**Coverage gaps identified:** 10 workflows proposed across 3 priority tiers.

---

## 2. Identified Gaps

### 2.1 Critical Gaps (Blocking for Healthy Repository)

| Gap | Risk |
|---|---|
| No PR-gated build/test pipeline | Broken builds and failing tests can be merged silently |
| No release automation | Mod packaging is entirely manual; error-prone |
| No security scanning | OWASP guidance exists in instructions but has no enforcement |

### 2.2 JAT-Specific Gaps (High Value for This Project)

| Gap | Risk |
|---|---|
| No architecture boundary enforcement on PRs | Contract violations (e.g., UI mutating canonical state) can go undetected |
| No determinism regression detection | Non-deterministic TaskId / ordering patterns can silently creep in |
| No SMAPI manifest validation | Manifest errors only surface at game runtime |
| No phase implementation progress tracking | Atomic checklist completion is invisible to automation |
| No deferments monitor | Deferred work items may remain forgotten indefinitely |

### 2.3 Supporting Quality Gaps

| Gap | Risk |
|---|---|
| No cross-platform build matrix | Windows/Linux differences in paths and tools may go undetected |
| No agent ecosystem consistency validator | Governance file may drift from actual agent/instruction/skill state |

---

## 3. Proposed Workflows

### 3.1 `pr-ci` — PR-Gated Build and Test Pipeline

**Priority:** 🔴 Critical  
**File:** `.github/workflows/pr-ci.md`  
**Trigger:** `pull_request` (opened, synchronize, reopened), `push` to main  

**What it does:**
- Runs `dotnet build JojaAutoTasks.csproj -c Debug -p:EnableModDeploy=false -p:EnableModZip=false`
- Runs `dotnet test Tests/JojaAutoTasks.Tests.csproj` and collects results
- Posts pass/fail status as a PR check that can block merge

**Why it is needed:**  
The existing `build-health-monitor` workflow is a *passive reporter* — it detects problems after the
fact and creates issues. It does not prevent broken code from being merged. There is currently no
gate that stops a PR merge when the build fails or tests regress. For a mod with strict determinism
contracts and architectural boundaries, a silent regression in `ConfigLoaderMigrationSafetyTests` or
`IdentifierReconstructionStabilityTests` could corrupt save-file compatibility. A PR CI gate is the
single most impactful automation a repository can have.

**Key considerations:**
- Uses `ubuntu-latest` runner with .NET 8 SDK (test target) and .NET 6 SDK (mod target)
- Publishes JUnit test results as annotations on the PR diff
- Cache layer for NuGet packages to keep runtime under 60 seconds
- Required status check that blocks merge on failure

---

### 3.2 `release-packaging` — Automated Mod Release Packaging

**Priority:** 🔴 Critical  
**File:** `.github/workflows/release-packaging.md`  
**Trigger:** `push` tag matching `v*.*.*`, `workflow_dispatch`

**What it does:**
- Runs `dotnet build JojaAutoTasks.csproj -c Release -p:EnableModDeploy=false -p:EnableModZip=true`
- Captures the generated `.zip` artifact from `bin/Release/net6.0/`
- Creates a GitHub Release with the zip attached and an auto-generated changelog
- Validates `manifest.json` metadata before packaging

**Why it is needed:**  
The mod targets end-users who install it through the Stardew Valley Mods directory or Nexus Mods.
Currently, producing a release requires a local build on a developer machine, followed by manual
upload. This process is error-prone — the wrong build configuration, a forgotten file, or a stale
`manifest.json` placeholder (`%ProjectVersion%` not replaced) can produce a broken release. An
automated release workflow guarantees that every tagged version is built from a clean state in the
same environment, with all artifacts correctly named and attached.

**Key considerations:**
- Version derived from the git tag (e.g., `v0.2.0` → `0.2.0`)
- Injects `%ProjectVersion%` before build
- Validates that `MinimumApiVersion` in `manifest.json` is still compatible with declared SMAPI version
- Attaches `JojaAutoTasks-v<VERSION>.zip` to the GitHub Release

---

### 3.3 `security-scanner` — CodeQL and Dependency Vulnerability Scanning

**Priority:** 🔴 Critical  
**File:** `.github/workflows/security-scanner.md`  
**Trigger:** `push` to main, `pull_request`, weekly schedule

**What it does:**
- Runs GitHub CodeQL static analysis on all C# source files
- Runs `dotnet list package --vulnerable` to detect NuGet advisories
- Reports findings in the GitHub Security tab and as PR annotations
- Creates issues for any high/critical findings that persist across multiple runs

**Why it is needed:**  
The repository already has a comprehensive `security-and-owasp.instructions.md` guideline that
agents are instructed to follow, and the `dotnet-dependency-analyzer` workflow watches for
dependency currency. However, neither of these provides *enforcement*. CodeQL can detect injection
patterns, insecure deserialization, and path traversal vulnerabilities in C# that are invisible to
style-only review. For a mod that reads/writes player save files and interacts with a running game
process, a security regression is especially damaging. This workflow closes the gap between
"security instructions exist" and "security is actually validated."

**Key considerations:**
- Uses `github/codeql-action@v3` with `csharp` language
- Runs on both PRs (to catch regressions before merge) and on a weekly schedule
- Integrates with GitHub's native Security Alerts tab (no extra tooling needed)
- Least-privilege `security-events: write` permission only

---

### 3.4 `architecture-contract-compliance` — PR Architecture Boundary Checker

**Priority:** 🟠 High  
**File:** `.github/workflows/architecture-contract-compliance.md`  
**Trigger:** `pull_request` (opened, synchronize), `issue_comment` with `/arch-check`

**What it does:**
- Analyzes changed `.cs` files in a PR against the backend and frontend architecture contracts
- Checks for forbidden patterns: UI directly mutating canonical state, bypassing the State Store,
  nondeterministic ID generation (e.g., `Guid.NewGuid()`), domain types used in the wrong layer
- Posts findings as a PR review comment categorized by severity (Blocker / Major / Minor)

**Why it is needed:**  
JAT has unusually strict architectural contracts compared to typical SMAPI mods. The
`BACKEND-ARCHITECTURE-CONTRACT.instructions.md` and `FRONTEND-ARCHITECTURE-CONTRACT.instructions.md`
define hard rules about the command/reducer/snapshot boundary, canonical state ownership, and
determinism requirements. Violations of these boundaries are not always visible as build errors —
they compile cleanly but silently corrupt the state model. The existing `interactive-code-reviewer`
(`/review` command) provides on-demand review, but does not run automatically. An architecture
compliance check on every PR ensures that boundary violations cannot be merged without explicit
human sign-off.

**Key considerations:**
- Reads the relevant instruction files from `.github/instructions/` as reference context
- Analyzes diff output, not the entire codebase, for efficiency
- Integrates with the existing `Reviewer` agent prompt pattern
- Non-blocking by default; posts warnings but does not fail the check (reviewer signs off)

---

### 3.5 `determinism-regression-detector` — Non-Deterministic Pattern Scanner

**Priority:** 🟠 High  
**File:** `.github/workflows/determinism-regression-detector.md`  
**Trigger:** `pull_request` (opened, synchronize), weekly schedule

**What it does:**
- Scans changed and all `.cs` source files for patterns that violate the determinism contract:
  - `Guid.NewGuid()` in non-test code
  - `Random` without a deterministic seed
  - `DateTime.Now` / `DateTime.UtcNow` outside explicitly-controlled time seams
  - `OrderBy()` on collections without a stable key fallback
  - `HashSet<T>` or `Dictionary<TKey, TValue>` traversal fed directly into snapshot ordering
- Posts findings as PR annotations or creates weekly discussion

**Why it is needed:**  
Section 3 of the JAT Design Guide establishes deterministic identifiers as a *first-class
requirement*. The existing tests (`IdentifierReconstructionStabilityTests`, `DayKeyTests`,
`TaskIdTests`) validate the *current* behavior, but they cannot catch a future commit that
accidentally introduces `Guid.NewGuid()` as a fallback for TaskId generation. A static pattern
scan catches these regressions at the source before they reach the test suite. Because JAT must
produce stable task state across save/load cycles — a requirement that directly affects gameplay —
a single non-deterministic ID generation path can corrupt player saves.

**Key considerations:**
- Uses `grep`/`ripgrep` pattern matching on source files (no compilation required)
- Runs within seconds; adds minimal latency to PR cycle
- Exclusion list for test files (where `Guid.NewGuid()` and `Random` are legitimate)
- Reports path + line number for each match

---

### 3.6 `smapi-manifest-validator` — Mod Manifest Integrity Checker

**Priority:** 🟠 High  
**File:** `.github/workflows/smapi-manifest-validator.md`  
**Trigger:** `pull_request` (on changes to `manifest.json` or `JojaAutoTasks.csproj`), `push` to main

**What it does:**
- Validates `manifest.json` against SMAPI's expected schema:
  - Required fields present (`Name`, `Author`, `Version`, `UniqueID`, `EntryDll`, `MinimumApiVersion`)
  - `%ProjectVersion%` placeholder is not present in a release build context
  - `UniqueID` follows the `Author.ModName` convention
  - `MinimumApiVersion` is a valid semantic version
- Cross-references the `<Version>` in `JojaAutoTasks.csproj` with `manifest.json` version field
- Posts a PR annotation on any validation failure

**Why it is needed:**  
The `manifest.json` file is the first thing SMAPI reads when loading the mod. A missing field,
malformed version string, or lingering `%ProjectVersion%` placeholder will prevent the mod from
loading at all — but only at game runtime, not at build time. The current `.csproj` build process
substitutes the version placeholder, but this substitution is configuration-dependent (it only
happens when `ModBuildConfig` is active). A developer who manually edits `manifest.json` or changes
the `<Version>` in `.csproj` without updating the corresponding constraint risks producing a
non-bootable mod. An automated validator catches these errors before they reach users.

**Key considerations:**
- Reads `manifest.json` and `JojaAutoTasks.csproj` as inputs
- Validates JSON schema validity as a first pass
- Checks `MinimumApiVersion` ≥ 4.4.0 (declared floor in the manifest)
- Fast check: completes in under 5 seconds

---

### 3.7 `phase-progress-tracker` — Atomic Checklist Implementation Tracker

**Priority:** 🟡 Medium  
**File:** `.github/workflows/phase-progress-tracker.md`  
**Trigger:** Daily schedule, `push` to main (when checklist files change)

**What it does:**
- Reads all `Phase N - Atomic Commit Execution Checklist.md` files from
  `Project/Tasks/Implementation Plan/`
- Counts checked (`[x]`) vs. unchecked (`[ ]`) items per phase
- Calculates and reports completion percentage per phase
- Identifies the current active phase (most recently started, not yet complete)
- Creates or updates a tracking issue with phase progress summary

**Why it is needed:**  
JAT uses a rigorous atomic commit execution checklist model with detailed phase plans. There are
already Phase 1, 2, and 3 checklists, each with dozens of substeps. As implementation progresses,
it becomes harder to understand which phase is active, what percentage of each phase is complete,
and whether any phase has stalled. This information currently requires manually opening and reading
each checklist file. An automated tracker surfaces this progress as a GitHub issue that stays
current, making it easy for the maintainer and any contributing agent to quickly orient on the state
of the implementation.

**Key considerations:**
- Parses Markdown checkbox syntax (`- [x]` vs `- [ ]`) from checklist files
- Respects the phase structure (Steps → Substeps → Final Gate)
- Updates a single tracking issue rather than creating new ones (reduces noise)
- Posts progress as a structured table with phase name, completion %, and blocking items

---

### 3.8 `deferments-monitor` — Deferred Work Item Tracker

**Priority:** 🟡 Medium  
**File:** `.github/workflows/deferments-monitor.md`  
**Trigger:** Weekly schedule, `workflow_dispatch`

**What it does:**
- Reads `Project/Tasks/Implementation Plan/Deferments Index.md`
- Identifies items that have been deferred for more than a configurable threshold (default: 4 weeks)
- Flags deferments whose scheduled phase is the current active phase but are not yet resolved
- Creates a discussion post summarizing the aging deferments report

**Why it is needed:**  
The atomic commit execution checklist process has a formal deferments lifecycle: active deferments
live in `Deferments Index.md` and resolved ones move to `Deferments Archive.md`. However, without
automation, there is no mechanism to surface deferments that have aged beyond their expected
resolution window or that should have been addressed during a now-active phase. Silent accumulation
of deferred work items creates invisible technical debt. This workflow makes the deferments
lifecycle observable by raising a periodic alert when items are aging.

**Key considerations:**
- Parses the DEF-NNN deferment ID format from the index file
- Reads the "Scheduled For" phase field to identify misaligned deferments
- Reports in a human-readable discussion rather than opening issues (advisory, not blocking)
- Deduplicates: does not re-report the same deferment until its status changes

---

### 3.9 `multi-platform-build-matrix` — Cross-Platform Build Validator

**Priority:** 🟡 Medium  
**File:** `.github/workflows/multi-platform-build-matrix.md`  
**Trigger:** Weekly schedule, `workflow_dispatch`, `push` to main

**What it does:**
- Builds the mod project on `ubuntu-latest`, `windows-latest`, and `macos-latest`
- Runs the test suite on each platform
- Reports any platform-specific failures as an issue

**Why it is needed:**  
SMAPI mods are primarily developed and consumed on Windows (where Stardew Valley's native
installation lives), but the build toolchain (MSBuild, .NET SDK, ModBuildConfig) must also work on
Linux and macOS for contributors who develop on those platforms. The existing `build-health-monitor`
only observes past workflow runs — it does not proactively test all platforms. A file path separator
difference, a platform-specific `Environment.GetFolderPath` call, or a case-sensitive filesystem
assumption can silently break non-Windows builds. A weekly matrix build catches these before they
affect contributors.

**Key considerations:**
- Uses `strategy.matrix` across `os: [ubuntu-latest, windows-latest, macos-latest]`
- Runs `dotnet build` with `EnableModDeploy=false` (game installation not required in CI)
- Archives build logs per platform as artifacts
- Creates one issue per failing platform (closes automatically when fixed)

---

### 3.10 `agent-ecosystem-validator` — Agent/Instruction/Skill Consistency Checker

**Priority:** 🟡 Medium  
**File:** `.github/workflows/agent-ecosystem-validator.md`  
**Trigger:** `push` (when `.github/agents/`, `.github/instructions/`, or `.github/skills/` change),
weekly schedule

**What it does:**
- Validates that every instruction file in `.github/instructions/` is mapped in
  `agent-boundaries-and-wiring-governance.instructions.md`
- Validates that every skill in `.github/skills/*/SKILL.md` is mapped in the same governance file
- Validates that all agent files referenced in the governance table exist in `.github/agents/`
- Checks YAML frontmatter validity in all `*.agent.md`, `*.instructions.md`, and `SKILL.md` files
- Creates an issue listing any unmapped or missing files

**Why it is needed:**  
The JAT agent ecosystem has a formal governance contract: every instruction file and skill must be
mapped in `agent-boundaries-and-wiring-governance.instructions.md`. This is currently enforced by
a runtime hook (`ecosystem-maintenance.sh`) that runs at the end of a Copilot session, but
in-session enforcement only catches violations during the session in which they are introduced.
A committed change that adds a new instruction file without updating the governance table will
break ecosystem integrity silently until the next Copilot session. A CI workflow provides a
persistent gate that catches these gaps regardless of how the change was made (direct push,
external tool, etc.).

**Key considerations:**
- Reads the governance file and extracts all mapped instruction and skill names
- Compares against the actual files on disk with `find`
- Parses YAML frontmatter to validate required `name:` and `description:` fields
- Complements (does not replace) the existing runtime hook

---

## 4. Implementation Priority Matrix

| # | Workflow | Priority | Effort | Impact |
|---|---|---|---|---|
| 1 | `pr-ci` | 🔴 Critical | Low | Very High — prevents broken merges |
| 2 | `release-packaging` | 🔴 Critical | Medium | Very High — enables reliable distribution |
| 3 | `security-scanner` | 🔴 Critical | Low | High — closes OWASP instruction gap |
| 4 | `architecture-contract-compliance` | 🟠 High | Medium | High — enforces design contracts |
| 5 | `determinism-regression-detector` | 🟠 High | Low | High — protects core invariant |
| 6 | `smapi-manifest-validator` | 🟠 High | Low | Medium — prevents mod boot failures |
| 7 | `phase-progress-tracker` | 🟡 Medium | Medium | Medium — improves planning visibility |
| 8 | `deferments-monitor` | 🟡 Medium | Low | Medium — surfaces aging technical debt |
| 9 | `multi-platform-build-matrix` | 🟡 Medium | Low | Medium — cross-platform safety |
| 10 | `agent-ecosystem-validator` | 🟡 Medium | Low | Medium — complements runtime hook |

---

## 5. Relationship to Existing Workflows

The proposed workflows complement, not replace, the existing agentic workflow suite.

```
Existing workflows (passive analysis + interactive):
  build-health-monitor      ←→  pr-ci (adds pre-merge gate)
  unit-test-coverage-analyzer ←→ pr-ci (adds enforcement)
  dotnet-dependency-analyzer  ←→ security-scanner (adds vulnerability enforcement)
  interactive-code-reviewer   ←→ architecture-contract-compliance (adds auto-trigger)
  csharp-type-safety-analyzer ←→ determinism-regression-detector (adds pattern scan)
  daily-docs-updater          ←→ phase-progress-tracker (adds checklist parsing)
  repository-quality-holistic ←→ deferments-monitor (adds aging alerts)
  agentic-workflow-health     ←→ agent-ecosystem-validator (adds file-level validation)
  (none)                      →  release-packaging (net new)
  (none)                      →  smapi-manifest-validator (net new)
  (none)                      →  multi-platform-build-matrix (net new)
```

---

## 6. Coverage After Implementation

| Area | Before | After |
|---|---|---|
| PR merge safety | ❌ No gate | ✅ Build + test gate |
| Release distribution | ❌ Manual | ✅ Automated |
| Security enforcement | ⚠️ Instructions only | ✅ CodeQL scanning |
| Architecture boundaries | ⚠️ On-demand only | ✅ Auto-checked on PR |
| Determinism invariants | ⚠️ Tests only | ✅ Static pattern scan |
| Manifest validity | ❌ Runtime only | ✅ Pre-merge checked |
| Phase progress | ❌ Manual | ✅ Tracked dashboard |
| Deferred work visibility | ❌ Manual | ✅ Aging alerts |
| Cross-platform builds | ❌ None | ✅ Weekly matrix |
| Agent ecosystem integrity | ⚠️ Session-only hook | ✅ CI persistent gate |

---

## 7. Next Steps

1. Review this report and select which workflows to implement.
2. For each approved workflow, the `.md` specification file in `.github/workflows/` can be compiled
   to a `.lock.yml` file using `gh aw compile` (the existing toolchain already handles this).
3. Prioritize `pr-ci` and `release-packaging` first — these provide the highest return for the
   lowest effort and address the most significant gaps.
4. After implementing `pr-ci`, configure it as a required status check in the branch protection
   rules for the main branch.

---

*This report was generated by automated codebase analysis. All proposed workflows follow the
conventions established by the existing agentic workflow suite.*

---

## 8. Workflow Consolidation — Merge Audit (2026-03-10)

**Status:** Implemented — 5 workflows eliminated (net), premium request usage reduced ~29%

After all 10 proposed workflows from Section 3 were implemented (totaling 21 agentic workflows),
the workflow suite was evaluated for consolidation opportunities to reduce premium request usage.

### Merges Implemented

| Merge | Before | After | Weekly Savings |
|-------|--------|-------|---------------|
| **Merge 1** | `csharp-type-safety-analyzer` (daily) + `unit-test-coverage-analyzer` (daily) | Absorbed into `repository-quality-holistic-analyzer` Day 6 (type safety) and Day 4 (test coverage) | **14 runs/week** |
| **Merge 2** | `phase-progress-tracker` (daily) + `deferments-monitor` (weekly) | New `project-progress-monitor` (weekly) | **7 runs/week** |
| **Merge 3a** | `csharp-large-class-simplifier` (weekly) + `dotnet-dependency-analyzer` (weekly) + `determinism-regression-detector` (weekly schedule) | New `weekly-codebase-health` (weekly) | **2 runs/week** |
| **Merge 3b** | `determinism-regression-detector` (PR trigger) | Merged into `architecture-contract-compliance` | PR runs |

**Total: 7 workflows eliminated, 2 new combined workflows created, net −5 workflows**

### Post-Consolidation Workflow Inventory (16 agentic workflows)

| Workflow | Type | Schedule | Notes |
|----------|------|----------|-------|
| `repository-quality-holistic-analyzer` | Analysis | Daily | Now includes type safety (Day 6) and test coverage (Day 4) |
| `build-health-monitor` | Monitoring | Daily + workflow_run | Unchanged |
| `daily-docs-updater` | Sync detection | Daily | Unchanged |
| `daily-repo-status` | Reporting | Daily | Unchanged |
| `agentic-workflow-health-monitor` | Meta-monitoring | Weekly + workflow_run | Unchanged |
| `agent-ecosystem-validator` | Governance | Weekly + push | Unchanged |
| `project-progress-monitor` | Progress | **Weekly** | Replaces daily `phase-progress-tracker` + weekly `deferments-monitor` |
| `weekly-codebase-health` | Analysis | **Weekly** | Replaces `csharp-large-class-simplifier` + `dotnet-dependency-analyzer` + determinism weekly scan |
| `architecture-contract-compliance` | PR gate | PR + `/arch-check` | Now includes determinism PR scan (was `determinism-regression-detector`) |
| `interactive-code-reviewer` | Interactive | `/review` comment | Unchanged |
| `interactive-performance-optimizer` | Interactive | `/optimize` comment | Unchanged |
| `pr-ci` | PR gate | PR + push to main | Unchanged |
| `multi-platform-build-matrix` | Build matrix | On-demand | Unchanged |
| `security-scanner` | Security | Weekly + PR + push | Unchanged |
| `smapi-manifest-validator` | Validation | PR + push to main | Unchanged |
| `release-packaging` | Release | Tag push + on-demand | Unchanged |

### Premium Request Usage: Before vs After

| Metric | Before | After | Change |
|--------|--------|-------|--------|
| Daily agentic runs/week | 49 (7×7) | 28 (4×7) | −21 runs/week |
| Weekly agentic runs/week | 6 | 4 | −2 runs/week |
| Total scheduled runs/week | **55** | **32** | **−23 runs/week (−42%)** |

### Merge Rationale

**Merge 1** was the highest-impact consolidation. Both `csharp-type-safety-analyzer` and
`unit-test-coverage-analyzer` ran daily and overlapped significantly with two rotation days
of `repository-quality-holistic-analyzer` (Day 4 "Testing Infrastructure" and Day 6 "Code
Consistency"). Expanding those rotation days to incorporate the specific analysis goals of
the removed workflows costs nothing extra (it's one existing daily run) while eliminating 14
redundant daily runs per week.

**Merge 2** recognized that phase progress and deferment aging are two aspects of the same
concern — tracking what implementation work remains. The daily cadence of `phase-progress-tracker`
was excessive for a weekly checkpoint workflow; implementation checklists rarely change daily.
Weekly frequency is appropriate and consistent with `deferments-monitor`'s original cadence.

**Merge 3** grouped three structurally similar weekly code-health workflows (class structure,
dependencies, determinism patterns) into a single comprehensive weekly pass. Their domains are
complementary: a class that is too large often has the same root cause as a dependency that has
grown stale — growing technical debt. Running them together in one weekly analysis allows the AI
to identify cross-cutting patterns. The PR-triggered determinism scan was preserved by folding
it into `architecture-contract-compliance`, ensuring PR protection is not degraded.
