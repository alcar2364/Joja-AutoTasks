---
name: "Verify Hook Legacy Coverage"
description: "Use when: validating that every legacy hook scenario is covered by executable runtime hook bundles."
argument-hint: "Runtime hooks path + legacy path + block-on-gap mode"
agent: "GodAgent"
---

Validate legacy-to-runtime hook coverage.

Inputs
- Runtime bundles path: <default .github/hooks>
- Legacy specs path: <default .github/hooks/legacy-md>
- Block on gap: <true|false>

Verification Steps
1. Enumerate all legacy hook specs (`*.hook.md`) in legacy path.
2. Load `.github/hooks/LEGACY_COVERAGE_MAP.md`.
3. Verify each legacy hook stem is mapped to at least one runtime bundle/script.
4. Verify each mapped runtime bundle has `hooks.json` and executable `*.sh` script(s).
5. Verify script paths in `hooks.json` exist.

Required Output
1. Coverage matrix summary
2. Uncovered legacy hooks (if any)
3. Invalid runtime hook configurations (if any)
4. If `block-on-gap=true`, return failure recommendation until all gaps are closed
