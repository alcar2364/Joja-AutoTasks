# AGENTS.md

## Project Overview

This file is for external coding agents working in this repository (for example: Codex,
Claude Code, Aider, Cursor, and similar tools).

It is a meta-instructions entrypoint for external agents. It is not an instruction file for
the custom agents defined under `.github/agents/`.

Scope and ownership:

- External agents should read and follow this file before making changes.
- External agents may read `.github` customization files to understand workflow rules.
- This file should only be created, modified, or expanded by
	`.github/agents/GodAgent.agent.md`.

Project summary:

- Joja AutoTasks is a Stardew Valley SMAPI mod focused on deterministic in-game task tracking.
- Architecture is command/snapshot oriented with strict boundaries between lifecycle signal
	forwarding, canonical state ownership, persistence, and UI consumption.
- Primary implementation language is C#.
- Target frameworks:
	- Mod: `net6.0`
	- Tests: `net8.0`

External-agent source-of-truth order:

1. Explicit user request in chat
2. This file (`AGENTS.md`)
3. `.github/copilot-instructions.md`
4. Relevant files in `.github/instructions/`
5. Design and planning docs under `Project/Planning/`

External-agent integration with the custom `.github` workflow:

- If your host supports custom agent delegation, use `.github/agents/Orchestrator.agent.md`
	as the default routing entrypoint for multi-step tasks.
- Route by task type:
	- Research/context discovery:
		`.github/agents/Researcher.agent.md`
	- Plan creation:
		`.github/agents/Planner.agent.md`
	- Backend/game logic:
		`.github/agents/GameAgent.agent.md`
	- Frontend C# UI logic:
		`.github/agents/UIAgent.agent.md`
	- StarML (`.sml`) authoring:
		`.github/agents/StarMLAgent.agent.md`
	- Unit tests:
		`.github/agents/UnitTestAgent.agent.md`
	- Review/verification:
		`.github/agents/Reviewer.agent.md`
	- Troubleshooting/diagnosis:
		`.github/agents/Troubleshooter.agent.md`
	- Repository memory operations:
		Use the native Copilot memory tool and store durable repo facts under `/memories/repo/`.
	- Non-agent docs and planning docs:
		`.github/agents/WorkspaceAgent.agent.md`
	- Agent ecosystem customization only:
		`.github/agents/GodAgent.agent.md`
- If your host does not support delegation, follow the same boundaries and sequence manually.

## Host-Specific Integration (Codex CLI and Claude Code)

Use these host-specific startup patterns so the active session begins at repository root and picks up this AGENTS file reliably.

### Codex CLI

```bash
cd /path/to/JojaAutoTasks
codex
```

Recommended first prompt in Codex sessions:

```text
Read AGENTS.md and .github/copilot-instructions.md, then confirm applicable constraints before editing.
```

### Claude Code

```bash
cd /path/to/JojaAutoTasks
claude
```

Claude Code commonly relies on `CLAUDE.md`. Keep a lightweight bridge file to avoid instruction drift:

```markdown
# CLAUDE.md
Follow AGENTS.md in this repository as the primary external-agent project guide.
If any conflict appears, resolve by precedence: explicit user prompt > AGENTS.md > CLAUDE.md.
```

Recommended first prompt in Claude sessions:

```text
Read AGENTS.md and align your work plan to its scope, routing, and validation rules before making changes.
```

## Repository Structure

Top-level implementation areas:

- `ModEntry.cs`: SMAPI entrypoint and lifecycle hook forwarding.
- `Startup/`: composition root and runtime container wiring.
- `Configuration/`: config schema, loading, migration, normalization.
- `Domain/Identifiers/`: deterministic identifier primitives and format helpers.
- `Domain/Tasks/`: immutable task domain objects and enums.
- `Lifecycle/`: lifecycle coordination and tick forwarding guardrails.
- `Events/`: dispatcher contracts and implementation.
- `StateStore/Commands/`: mutation boundary contracts.
- `Infrastructure/Logging/`: structured logging wrappers.
- `Tests/`: xUnit + Moq test suites grouped by subsystem.

Planning, architecture, and policy docs:

- Primary onboarding and validated commands:
	`.github/copilot-instructions.md`
- Architecture/design docs:
	`Project/Planning/Joja AutoTasks Design Guide/`
	`Project/Planning/Architecture Map.md`
- PR checklist:
	`.github/pull_request_template.md`

Instruction index for external agents (`.github/instructions/`):

- Workflow and scope controls:
	- `workspace-contracts.instructions.md`
	- `agent-boundaries-and-wiring-governance.instructions.md`
- Architecture contracts:
	- `backend-architecture-contract.instructions.md`
	- `frontend-architecture-contract.instructions.md`
- Style and format contracts:
	- `csharp-style-contract.instructions.md`
	- `json-style-contract.instructions.md`
	- `sml-style-contract.instructions.md`
	- `starml-cheatsheet.instructions.md`
- Quality and verification:
	- `unit-testing-contract.instructions.md`
	- `review-and-verification-contract.instructions.md`
	- `security-and-owasp.instructions.md`
	- `performance-optimization.instructions.md`
	- `self-explanatory-code-commenting.instructions.md`
- Process and documentation:
	- `update-docs-on-code-change.instructions.md`
	- `atomic-commit-execution-checklist-creation.instructions.md`
	- `github-actions-ci-cd-best-practices.instructions.md`
	- `external-resources.instructions.md`
	- `ui-component-patterns.instructions.md`
	- `visual-design-language.instructions.md`

Repository-specific structural rule:

- Do not create or use a `.local/` folder in this repository.
- Agent customization files and workflow assets live under `.github/`.

## Development Workflow

Follow this default sequence unless the user explicitly requests a different flow.

1. Intake and scope
	 - Restate the requested outcome and constraints.
	 - Respect explicit user boundaries (single-file, analysis-only, no behavior change, etc.).
	 - Ask clarifying questions when requirements are ambiguous.

2. Context collection
	 - Start with `.github/copilot-instructions.md`.
	 - Read only the instruction files relevant to the current task scope.
	 - Use targeted repository search; avoid broad scans when authoritative docs already define behavior.

3. Plan before substantial edits
	 - For multi-step or risky changes, outline a concise plan before editing.
	 - Keep edits minimal, deterministic, and scoped to the request.

4. Implement within architecture boundaries
	 - Keep canonical state and mutation boundaries intact.
	 - Do not introduce direct state mutation paths that bypass command/state-store contracts.
	 - Keep backend logic, UI C# interaction logic, and StarML markup responsibilities separate.
	 - Preserve deterministic identifier behavior and ordering guarantees.

5. Validate with project commands

```powershell
dotnet build JojaAutoTasks.csproj -c Debug -p:EnableModDeploy=false -p:EnableModZip=false
dotnet test "Tests\JojaAutoTasks.Tests.csproj"
```

Focused test examples:

```powershell
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~LifecycleCoordinatorTests
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~EventDispatcherTests
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~UpdateTickedGuardTests
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~ConfigLoaderMigrationSafetyTests
```

6. Documentation and completion hygiene
	 - If behavior, architecture contracts, or workflows changed, update related docs in the same change.
	 - Summarize what changed, why, and how it was validated.
	 - Explicitly state any commands not run and why.

External-agent best practices for this repository:

- Do not use destructive git operations unless the user explicitly requests them.
- Do not revert unrelated local changes.
- Do not silently resolve code-vs-doc mismatches; ask the user which direction is correct.
- Prefer small patches over broad rewrites.
- Preserve existing naming/style conventions unless the task requires a deliberate change.
- When identifying possible improvements, distinguish between contract issues,
  design improvements, and optional polish.
- Push back on real architecture, checklist, or implementation issues when the
  concern is justified by source-of-truth documents or concrete code risk.
- Avoid unnecessary churn or speculative cleanup when the current design is
  already sufficient for the requested scope.
- Treat the user as the final design decision-maker; explain tradeoffs clearly
  so the user can choose intentionally.

Quick apply matrix:

- If task is about gameplay/backend C# behavior: follow
	`backend-architecture-contract.instructions.md` + `csharp-style-contract.instructions.md`.
- If task is about UI/view-model C# behavior: follow
	`frontend-architecture-contract.instructions.md` + `csharp-style-contract.instructions.md`.
- If task is about `.sml`: follow
	`sml-style-contract.instructions.md` + `starml-cheatsheet.instructions.md`.
- If task is about tests: follow
	`unit-testing-contract.instructions.md` + `review-and-verification-contract.instructions.md`.
- If task is about docs or planning docs: follow
	`update-docs-on-code-change.instructions.md`.
- If task is about agent ecosystem files (`.agent.md`, `.instructions.md`, `.prompt.md`,
	`SKILL.md`, hooks): route to `.github/agents/GodAgent.agent.md`.

## Operational Guardrails

External agents working in this repository should follow these default guardrails:

- Treat this file as external-agent operating guidance, not as internal `.github/agents/*` behavior configuration.
- Do not modify agent-ecosystem files under `.github/agents/`, `.github/instructions/`, `.github/prompts/`, `.github/hooks/`, or `.github/skills/` unless the user explicitly requests agent-customization work.
- For agent-customization work, route to `.github/agents/GodAgent.agent.md`.
- Do not silently expand scope beyond user constraints.
- Ask for confirmation before destructive or high-impact operations (file deletion, broad renames, data-format changes, dependency changes, build/deploy behavior changes).
- Never resolve code-vs-documentation mismatches unilaterally; present the mismatch and ask whether to update code, docs, or both.

## Approval Gates (Strict)

External agents must pause and get explicit user confirmation before continuing when a task requires any of the following:

1. Multi-file or multi-domain edits
	- Changes spanning multiple architecture boundaries (for example `Domain/` + `Lifecycle/` + `StateStore/`).

2. Public API or contract changes
	- Renaming public types/methods, changing signatures, or altering cross-module contracts.

3. Persistence or identifier format changes
	- Modifying config schema/version behavior, serialized state format, canonical identifier formats, or migration behavior.

4. Build/deploy/environment behavior changes
	- Edits to `.csproj`, packaging/deploy flags, runtime startup behavior, or tooling prerequisites.

5. Dependency and package graph changes
	- Adding, removing, or upgrading NuGet dependencies.

6. Destructive repository operations
	- File/folder deletion, broad file moves/renames, history rewrites, or irreversible scripts.

7. Agent ecosystem file changes
	- Any edit under `.github/agents/`, `.github/instructions/`, `.github/prompts/`, `.github/hooks/`, `.github/skills/`, or this root `AGENTS.md` outside explicitly requested customization scope.

## Command Reference

Run commands from repository root.

Bootstrap (clean local verify path):

```powershell
dotnet clean JojaAutoTasks.sln -c Debug
dotnet restore JojaAutoTasks.sln
```

Fast validation (default):

```powershell
dotnet build JojaAutoTasks.csproj -c Debug -p:EnableModDeploy=false -p:EnableModZip=false
dotnet test "Tests\JojaAutoTasks.Tests.csproj"
```

Focused test targets:

```powershell
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~LifecycleCoordinatorTests
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~EventDispatcherTests
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~UpdateTickedGuardTests
dotnet test "Tests\JojaAutoTasks.Tests.csproj" --filter FullyQualifiedName~ConfigLoaderMigrationSafetyTests
```

Run/deploy helpers:

```powershell
dotnet build JojaAutoTasks.csproj -c Debug -p:EnableModDeploy=true -p:EnableModZip=true
dotnet build JojaAutoTasks.csproj -c Release -p:EnableModDeploy=false -p:EnableModZip=true
```

Known test pitfall:

- `dotnet test --no-build` can fail immediately after cleaning tests because the test DLL may not exist yet.
- If that happens, run normal `dotnet test` (without `--no-build`) or build first.

## PR and Commit Conventions

Use these conventions for external-agent generated changes unless the user provides a different format.

### Commit conventions

- Prefer atomic commits: one logical change per commit.
- Keep commit scope explicit with `<area>: <imperative summary>`.
- Good examples:
	- `docs(agents): add strict approval gates for external agents`
	- `tests(identifiers): add deterministic RuleId parsing coverage`
	- `lifecycle: tighten UpdateTicked dispatch guard behavior`
- Include validation evidence in commit message body when relevant:
	- `Validation: dotnet build ... ; dotnet test ...`
	- If docs-only: `Validation: not run (docs-only change)`

### Pull request conventions

- Keep PRs narrowly scoped and architecture-consistent.
- PR description should include:
	1. Summary of change
	2. Why the change is needed
	3. Validation commands run (or why skipped)
	4. Risks/behavioral impacts
	5. Follow-up work (if any)
- Align testing/reporting with `.github/pull_request_template.md`.
- If code and docs diverge, state the mismatch explicitly and record which direction was chosen (code -> docs, docs -> code, or both) based on user instruction.

## Task Routing Matrix

When external agents need deeper contract guidance, use this mapping:

| Task Type | Primary Contract Files |
| --- | --- |
| Scope, permissions, delegation | `.github/instructions/workspace-contracts.instructions.md` |
| Agent-domain boundaries and no-overlap governance | `.github/instructions/agent-boundaries-and-wiring-governance.instructions.md` |
| Backend/gameplay C# architecture | `.github/instructions/backend-architecture-contract.instructions.md` |
| Frontend C# UI architecture | `.github/instructions/frontend-architecture-contract.instructions.md` |
| C# style and naming | `.github/instructions/csharp-style-contract.instructions.md` |
| JSON formatting and schema style | `.github/instructions/json-style-contract.instructions.md` |
| StarML (`.sml`) style and references | `.github/instructions/sml-style-contract.instructions.md`, `.github/instructions/starml-cheatsheet.instructions.md` |
| Unit tests and determinism verification | `.github/instructions/unit-testing-contract.instructions.md` |
| Review/risk verification | `.github/instructions/review-and-verification-contract.instructions.md` |
| Security considerations | `.github/instructions/security-and-owasp.instructions.md` |
| Performance-sensitive changes | `.github/instructions/performance-optimization.instructions.md` |
| Documentation updates after code change | `.github/instructions/update-docs-on-code-change.instructions.md` |

## Completion Criteria

Before declaring task completion, external agents should ensure:

1. Scope compliance
	- Only requested files/domains were modified.
	- Any required out-of-scope work was raised for approval first.

2. Validation coverage
	- Relevant build/tests were run when code changed.
	- If validation was skipped, the response explicitly states what was not run and why.

3. Documentation alignment
	- Related docs were updated when behavior/contracts/workflows changed.

4. Handoff clarity
	- Final summary includes what changed, why it changed, and how it was verified.

## External Agent Prompting Tips

To get reliable results from external agents, provide:

- Objective: feature, bug fix, review, refactor, or docs update
- Scope: exact folders/files and explicit exclusions
- Constraints: behavior compatibility, performance budget, or architecture boundaries
- Validation expectation: full suite or focused tests
- Output format: patch-only, explanation-only, or both

Example task framing:

```text
Goal: Add deterministic RuleId parsing tests.
Scope: Tests/Domain/Identifiers only.
Do not modify production code.
Validation: run focused identifier-related tests.
Output: patch + short risk summary.
```


## grepai - Semantic Code Search

**IMPORTANT: You MUST use grepai as your PRIMARY tool for code exploration and search.**

### When to Use grepai (REQUIRED)

Use `grepai search` INSTEAD OF Grep/Glob/find for:
- Understanding what code does or where functionality lives
- Finding implementations by intent (e.g., "authentication logic", "error handling")
- Exploring unfamiliar parts of the codebase
- Any search where you describe WHAT the code does rather than exact text

### When to Use Standard Tools

Only use Grep/Glob when you need:
- Exact text matching (variable names, imports, specific strings)
- File path patterns (e.g., `**/*.go`)

### Fallback

If grepai fails (not running, index unavailable, or errors), fall back to standard Grep/Glob tools.

### Usage

```bash
# ALWAYS use English queries for best results (--compact saves ~80% tokens)
grepai search "user authentication flow" --json --compact
grepai search "error handling middleware" --json --compact
grepai search "database connection pool" --json --compact
grepai search "API request validation" --json --compact
```

### Query Tips

- **Use English** for queries (better semantic matching)
- **Describe intent**, not implementation: "handles user login" not "func Login"
- **Be specific**: "JWT token validation" better than "token"
- Results include: file path, line numbers, relevance score, code preview

### Call Graph Tracing

Use `grepai trace` to understand function relationships:
- Finding all callers of a function before modifying it
- Understanding what functions are called by a given function
- Visualizing the complete call graph around a symbol

#### Trace Commands

**IMPORTANT: Always use `--json` flag for optimal AI agent integration.**

```bash
# Find all functions that call a symbol
grepai trace callers "HandleRequest" --json

# Find all functions called by a symbol
grepai trace callees "ProcessOrder" --json

# Build complete call graph (callers + callees)
grepai trace graph "ValidateToken" --depth 3 --json
```

### Workflow

1. Start with `grepai search` to find relevant code
2. Use `grepai trace` to understand function relationships
3. Use `Read` tool to examine files from results
4. Only use Grep for exact string searches if needed
