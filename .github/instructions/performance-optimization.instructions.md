---
name: performance-optimization
description: "JAT performance guardrails for game-tick, HUD rendering, State Store, and persistence. Use when: editing UpdateTicked callbacks, HUD/menu rendering, State Store evaluation paths, or persistence operations."
applyTo: "{Domain,Events,Infrastructure,Lifecycle,Startup,UI}/**/*.cs"
---

# Performance Guardrails — JAT

## Purpose

JAT runs inside the Stardew Valley game loop. Every hot path must be bounded,
allocation-conscious, and deterministic.

Apply these rules when modifying UpdateTicked callbacks, HUD/menu rendering, State Store
evaluation, or persistence operations.

## 1. Game Tick Rules

### 1.1 UpdateTicked throttling

- Do NOT run heavy evaluation on every game tick.
- The mod entry point throttles UpdateTicked; evaluation intervals must respect the configured
  throttle.
- Keep per-tick work O(small constant), not O(task count) or O(rule count).

### 1.2 Avoid allocations per tick

- Do NOT allocate new collections inside UpdateTicked hot paths.
- Preallocate and reuse where feasible.
- Avoid LINQ chains that allocate enumerators on every tick.

### 1.3 Bounded evaluation passes

- Evaluation engine passes MUST be bounded.
- No unbounded scans over all game items per tick.
- If a scan is needed, cache results and invalidate on specific game events (not every tick).

## 2. HUD Rendering Rules

### 2.1 No per-frame rebuilds

- HUD must not rebuild layout or allocate new view trees every frame.
- Rebuild only on: new State Store snapshot, relevant config change, or explicit layout
  invalidation.

### 2.2 CompletedTaskSection behind visibility gate

- Do NOT render a completed task section unconditionally on every frame if it is hidden.

### 2.3 Stable list rendering

- Repeated task rows must use stable templates.
- Avoid structural lane/grid rebuilds triggered by selection state changes alone.

## 3. State Store Rules

### 3.1 Snapshots are read-only projections

- Snapshots MUST be produced from canonical state, not recomputed on every consumer access.
- Publish a new snapshot only when canonical state actually changes.

### 3.2 Command processing is synchronous and bounded

- Commands MUST apply fully and deterministically in one step.
- Do NOT queue unbounded command batches.

## 4. Persistence Rules

### 4.1 Load/save is not in the hot path

- Persistence operations occur at day-end / save-triggered hooks, not in UpdateTicked.
- Never call save/load from the render or tick path.

### 4.2 Serialize only essential data

- Persist minimal essential state: no transient caches, scroll positions, or derived values.

## 5. LINQ and Collection Rules

- Prefer `OrderBy` with a stable `StringComparer.Ordinal` comparator when ordering matters.
- Avoid `.ToList()` / `.ToArray()` allocations in hot paths; prefer direct enumeration.
- Do not use hash-based collection iteration when stable ordering is required.

## 6. When Performance Work Is Warranted

- Before optimizing, establish a profile or reproduction case. Do not optimize speculatively.
- Use SMAPI console logs with timestamps to measure tick-time impact.
- Only apply micro-optimizations after confirming they address a measured bottleneck.
