---
name: performance-advisor
description: >-
  Analyzes code generation for performance implications before implementation, flags
  bottlenecks, and suggests optimization strategies aligned with project domains.
trigger: before-generation
applyTo: "**/*.{cs,sml,json}"
---

# Performance Advisor Hook #

**Trigger:** Before code generation or complex refactoring.  
**Purpose:** Catch performance risks early and ensure implementations follow optimization best practices.

## Scope and Applicability ##

This hook activates when:

- Agent is generating backend code (domain, events, infrastructure)
- Agent is generating performance-sensitive features (sorting, filtering, persistence)
- Agent is designing data structures or algorithms
- Request indicates performance concerns
- Code path involves database queries, event dispatch, or rendering loops

## Pre-Generation Performance Analysis ##

**RECOMMENDED**: Load [`performance-optimization.instructions.md`](../Instructions/performance-optimization.instructions.md) when:

1. **Backend code generation:**
   - Check for N+1 query patterns (especially in persistence, event dispatch)
   - Verify efficient algorithm choice for sorting/filtering
   - Ensure bounded data structures (no unbounded collections)
   - Check for proper resource cleanup (file handles, connections)

2. **Frontend/UI code generation:**
   - Verify DOM manipulation batching
   - Check for unnecessary re-renders or re-evaluations
   - Ensure asset optimization (images, fonts)
   - Verify lazy loading where appropriate

3. **Refactoring or optimization requests:**
   - Profile candidates before proposing changes
   - Use concrete metrics (time, memory, throughput)
   - Propose A/B testing or canary rollout for verification
   - Document performance impact of proposed changes

## Analysis Procedure ##

1. When generation intent is detected, scan proposed code for common patterns:
   - Loops with internal database/network calls
   - Unbounded collection growth
   - Repeated expensive computations
   - Resource leaks (unclosed files, connections)

2. For high-risk patterns, consult [`performance-optimization.instructions.md`](../Instructions/performance-optimization.instructions.md).

3. Propose optimizations with:
   - Clear metric impact (expected improvement)
   - Tradeoff analysis (complexity vs. gain)
   - Testing strategy to verify improvement

4. If no obvious issues exist, proceed without flagging.

## JAT-Specific Performance Concerns ##

High-risk areas in JAT:

- **State Store queries**: Check for efficient filtering/sorting (bounded, indexed)
- **Event dispatch**: Ensure event handlers don't do expensive work synchronously
- **Persistence migration**: Verify non-blocking, streaming patterns
- **Task evaluation**: Check for unbounded loops or unguarded re-evaluation
- **UI rendering**: Verify lists don't render all items (virtual scrolling, pagination)

## Conflict Resolution ##

If performance guidance conflicts with correctness or contracts:

1. Correctness (safety, determinism, contracts) always wins.
2. Performance is secondary to design integrity.
3. Suggest performance improvements as follow-up work if safety requires suboptimal patterns.

