---
name: ci-cd-workflow-loader
description: >-
  Loads GitHub Actions best practices before workflow file edits to ensure robust,
  secure, and efficient CI/CD pipeline design.
trigger: before-edit
applyTo: ".github/workflows/*.{yml,yaml}"
---

# CI/CD Workflow Loader Hook #

**Trigger:** Before editing `.github/workflows/*.yml` or `.github/workflows/*.yaml` files.  
**Purpose:** Ensure workflow designs follow GitHub Actions best practices for robustness, security, and efficiency.

## Scope ##

This hook activates when:

- Agent is creating or modifying GitHub Actions workflow files
- Intent includes CI/CD pipeline, build automation, deployment strategy
- Workflow file paths match `.github/workflows/**`

## Mandatory Context Load ##

Load [`github-actions-ci-cd-best-practices.instructions.md`](../Instructions/github-actions-ci-cd-best-practices.instructions.md) before:

1. **Workflow creation:**
   - Understand workflow structure (jobs, steps, environment variables)
   - Follow module and reusability patterns
   - Apply security best practices (secret management, access control)

2. **Modification of existing workflows:**
   - Ensure caching strategies are sound
   - Verify matrix strategies are efficient
   - Check for secret exposure vulnerabilities

3. **Testing and deployment strategies:**
   - Understand different testing approaches in CI
   - Apply deployment best practices
   - Set up appropriate gating and approval steps

## Loading Procedure ##

1. Detect `.github/workflows/` file edit or creation request.
2. Load [`github-actions-ci-cd-best-practices.instructions.md`](../Instructions/github-actions-ci-cd-best-practices.instructions.md) into context.
3. Apply workflow best practices to proposed changes before generation.
4. Verify workflow design aligns with:
   - Job dependency structure (no circular dependencies)
   - Secret management (no hardcoded credentials)
   - Caching patterns (appropriate reuse)
   - Matrix usage (efficient, not over-engineered)

## Conflict Resolution ##

If CI/CD guidance conflicts with other instructions:

1. Check [`WORKSPACE-CONTRACTS`](../Instructions/workspace-contracts.instructions.md) for operational constraints (takes precedence).
2. Follow GitHub Actions best practices for workflow design patterns.
3. JAT-specific tool versions and build commands take precedence in `.local/Agents/Instructions/`.

