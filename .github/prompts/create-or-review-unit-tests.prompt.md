---
name: "Create Or Review Unit Tests"
description: "Use when: designing, implementing, or reviewing deterministic C# unit tests for requested scope."
argument-hint: "Target code + test intent + scope + constraints"
agent: "UnitTestAgent"
---

Create or review unit tests for the target scope below.

Testing Inputs
- Target code under test: <required>
- Test intent: <new tests|expand coverage|review existing tests>
- Scope boundaries: <required>
- Constraints: <single file|no prod edits|other>

Testing Guardrails
- Tests must be deterministic and non-flaky.
- Verify architecture boundaries and mutation path safety.
- Cover stable identifiers, ordering, and persistence behavior when relevant.
- Include edge cases and failure modes.

Required Output
1. Test changes or review findings
2. Missing-case matrix
3. Verification steps executed
4. Confidence level with rationale
