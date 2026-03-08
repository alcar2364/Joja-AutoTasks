---
name: "Research Codebase Context"
description: "Use when: gathering file-level context, existing patterns, and constraints before planning or coding."
argument-hint: "Question + target subsystem + scope boundaries"
agent: "Researcher"
---

Research this request and return concrete, citation-backed context.

Research Inputs
- Problem/question: <required>
- Target subsystem(s): <Domain|UI|Events|Lifecycle|Tests|other>
- Scope boundaries: <required>
- Known files/symbols (optional): <list>

Research Expectations
- Prioritize high-signal files and symbols.
- Identify existing patterns to preserve.
- Call out contract constraints relevant to this request.
- Do not propose implementation details unless explicitly requested.

Required Output
1. Key findings with file/symbol citations
2. Relevant existing patterns and conventions
3. Risks and unknowns
4. Suggested next handoff (Planner, implementer, Reviewer, etc.)
