---
name: self-splitting-parallel-execution
description: "Universal protocol for self-splitting parallel execution across JAT subagents. Use when: implementing self-splitting in any subagent, delegating with CanSelfSplit, or auditing parallelization protocol compliance."
---

# Self-Splitting Parallel Execution Protocol

## Scope

This skill applies to all JAT subagents when delegated tasks from the Orchestrator.

## Purpose

Define the universal protocol for self-splitting parallel execution, allowing subagents to autonomously partition large tasks, spawn parallel instances of themselves, and aggregate results into a single unified response.

## Protocol Overview

When the orchestrator grants self-splitting permission (`CanSelfSplit: true`, the default for non-trivial tasks), the subagent is responsible for:

1. Assessing whether the task scope benefits from parallelization
2. Decomposing the work into file-based partitions with dependency grouping
3. Spawning temporary copies of itself, each scoped to one partition
4. Aggregating all results into a single unified response before returning to the orchestrator

**Key principle:** From the orchestrator's perspective, it makes one delegation and receives one merged result back. The parallelism is internal to the subagent.

## Assessment Criteria (Universal)

Each subagent must evaluate whether self-splitting is appropriate based on:

**Self-splitting IS beneficial when:**
- Task involves multiple files or components (typically 3+ files for implementation/analysis, 4+ for review/refactoring)
- Work can be partitioned along natural subsystem or architectural boundaries
- File dependencies allow independent work within each partition
- Parallelization reduces total time without fragmenting critical reasoning

**Self-splitting is NOT beneficial when:**
- Single-file or tightly-coupled scope
- Holistic reasoning required across entire scope (architecture coherence, cross-cutting concerns)
- Small scope where overhead exceeds benefit
- Task requires unified context that cannot be safely partitioned

**Domain-specific criteria:** Each agent defines additional domain-specific assessment criteria in its own agent file.

## Partitioning Strategy (Universal)

All self-splitting subagents must follow this partitioning approach:

### 1. Inventory

Build a complete list of all files involved in the task scope.

### 2. Dependency Scan

Before splitting, perform dependency analysis:
- Analyze imports, shared types, function calls across files
- Identify tightly-coupled modules and components
- Map cross-boundary interactions and interface contracts
- Identify shared state, command flows, or coordination requirements

**Critical rule:** Files that depend heavily on each other MUST be grouped into the same partition. Do not split tightly-coupled files across different instances.

### 3. Form Partitions

Group files into clusters where:
- Each cluster is as independent as possible from others
- Tightly-coupled files are in the same cluster
- Partition boundaries align with natural subsystem or architectural boundaries
- Number of partitions is driven by dependency boundaries, not an arbitrary target

**If the entire scope is tightly coupled and cannot be meaningfully split, do not split.** Run as a single instance and note why self-splitting was not applicable.

### 4. Assign Instances

Spawn one parallel copy of the subagent per partition. Each instance handles exactly its assigned cluster and must not act on anything outside its partition.

## Context Management (Universal)

Each parallel instance must receive carefully managed context to work independently without bloating memory or causing blind spots.

### Shared Context (given to all instances)

- Original prompt/task description
- Overall goal and definition of done
- Project-wide constraints and conventions (applicable contracts, style guides)
- Partition map summary (so each instance knows what the other instances are covering and where its boundaries are)
- Cross-partition coordination rules (interaction constraints, sequencing requirements)

### Instance-Specific Context

- Full file contents for files in that instance's partition only
- Dependency details and patterns specific to that partition
- Do NOT load the full codebase into every instance

### Boundary Context (lightweight)

For files at the edge of a partition - files that have some dependency on files in another partition but were grouped on this side of the boundary:

- Include interface-level summaries of adjacent files (function signatures, exported types, API contracts)
- Do NOT include full file contents for anything outside the partition

**Tradeoff guidance:** Too much shared context defeats the purpose of splitting. Too little causes blind spots. When in doubt, include interface summaries rather than full file contents for anything outside the direct partition.

## Result Aggregation (Universal)

After all parallel instances return, the coordinating subagent instance must:

1. **Merge findings** into a single coherent output that reads as if one agent produced it
2. **Deduplicate** overlapping observations, especially at partition boundaries
3. **Resolve conflicts** where two instances flagged the same thing differently (higher severity wins for review findings; most restrictive constraint wins for architectural decisions)
4. **Verify cross-partition consistency** (interaction wiring, interface contracts, style consistency)
5. **Identify missed interactions** between partitions that individual instances couldn't see
6. **Preserve traceability** as metadata (note which partition each finding originated from, but keep this as metadata, not the primary structure of the response)

## Progress and Failure Handling (Universal)

The self-splitting subagent must track the status of each spawned instance.

**Instance failure handling:**
- If one instance fails or stalls, reassign that partition to a new instance without restarting the others
- Log the failure reason for diagnostic purposes

**Progress reporting:**
- If the orchestrator requests progress mid-task, report partition-level status:
  - Complete: partition work finished
  - In Progress: partition instance still working
  - Failed: partition instance failed (include reason)
- Provide estimated completion based on partition progress

## Execution Pattern (Universal)

When a subagent decides to self-split:

1. **Announce the partition plan**
   - List file/component/subsystem clusters
   - Explain why these boundaries were chosen
   - Note any tightly-coupled files grouped together

2. **Spawn instances**
   - Use `runSubagent` tool with `agentName: "<same agent name>"` for each partition
   - Provide partition-scoped prompts with appropriate shared/instance-specific context
   - Track instance IDs for progress monitoring

3. **Aggregate results**
   - Wait for all instances to complete
   - Follow result aggregation protocol (merge, deduplicate, resolve conflicts)
   - Verify cross-partition consistency

4. **Return unified output**
   - Single coherent response to orchestrator
   - No evidence of internal parallelism in the final output structure
   - Traceability metadata optional (useful for debugging, not required)

## Agent-Specific Responsibilities

Each subagent must define in its own agent file:

1. **Domain-specific assessment criteria** (when to self-split for this agent's work domain)
2. **Domain-specific partitioning rules** (what constitutes a partition in this domain - subsystem clusters for GameAgent, UI surface clusters for UIAgent, test file clusters for UnitTestAgent, etc.)
3. **Domain-specific boundary context rules** (what interface-level information crosses partition boundaries in this domain)
4. **Domain-specific aggregation rules** (how to merge findings specific to this agent's output format)

## Anti-Patterns

Do NOT:
- Split arbitrarily without dependency analysis
- Use a fixed number of partitions regardless of natural boundaries
- Load full codebase into every instance
- Split tightly-coupled files across partitions
- Self-split when the task is already narrowly scoped or tightly coupled
- Self-split when holistic reasoning is required
- Expose internal parallelism in the final output structure
- Fail to track instance status or handle instance failures
- Ignore orchestrator progress requests

## Enforcement

This protocol is enforced via:
- Agent descriptions reference this skill
- Orchestrator includes `CanSelfSplit: true|false` flag in delegation
- Runtime hooks verify self-splitting assessment and execution (see `.github/hooks/self-splitting-enforcement/`)
