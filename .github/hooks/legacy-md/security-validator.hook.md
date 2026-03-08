---
name: security-validator
description: >-
  Validates security practices before code generation, checking for common vulnerabilities,
  secret exposure, input validation, and OWASP compliance.
trigger: before-generation
applyTo: "**/*.{cs,sml,json}"
---

# Security Validator Hook #

**Trigger:** Before code generation, especially for security-critical paths.  
**Purpose:** Catch security vulnerabilities early and ensure OWASP-compliant implementation.

## Scope and Applicability ##

This hook activates when:

- Agent is generating authentication, authorization, or access control code
- Agent is handling user input or external data
- Agent is working with secrets, API keys, or sensitive data
- Agent is implementing persistence or data access
- Agent is generating code that interacts with external systems
- Request indicates security concerns or compliance needs

## Pre-Generation Security Review ##

**MANDATORY**: Load [`security-and-owasp.instructions.md`](../Instructions/security-and-owasp.instructions.md) before generating:

1. **Access control and API security (A01, A10):**
   - Check for least privilege enforcement
   - Verify deny-by-default patterns
   - Ensure proper authorization before resource access
   - Validate external URLs and prevent SSRF

2. **Cryptography and secrets (A02):**
   - Ensure no hardcoded secrets, API keys, or credentials
   - Verify secrets loaded from environment or secure stores
   - Check for strong hashing algorithms (Argon2, bcrypt)
   - Ensure encrypted data transmission (HTTPS required)

3. **Injection attacks (A03):**
   - Check for parameterized queries (no string concatenation in SQL)
   - Verify command-line argument escaping (if applicable)
   - Ensure XSS protection with context-aware encoding
   - Validate input sanitization

4. **Misconfiguration (A05, A06):**
   - Verify error messages don't leak system information
   - Check for secure headers if web-facing
   - Ensure dependencies are up-to-date and audited

5. **Authentication and session management (A07):**
   - Verify session tokens are cryptographically random
   - Check for secure cookie attributes (HttpOnly, Secure, SameSite)
   - Ensure brute-force protection (rate limiting, account lockout)

## Security Analysis Procedure ##

1. When code generation intent is detected, identify security-critical operations:
   - Database queries
   - Network requests
   - File I/O with user input
   - Authentication/authorization decisions
   - Secret handling

2. For each sensitive operation, consult relevant OWASP sections from [`security-and-owasp.instructions.md`](../Instructions/security-and-owasp.instructions.md).

3. Flag or fix violations using OWASP patterns:
   - Add parameterized queries instead of string concatenation
   - Add input validation before processing
   - Ensure secrets are not hardcoded
   - Verify secure defaults

4. If no violations found, document security reasoning in code comments.

## JAT-Specific Security Concerns ##

High-risk areas in JAT:

- **Mod configuration and save files**: Validate file paths (no directory traversal)
- **SMAPI API calls**: Verify proper error handling and permission checks
- **State persistence**: Ensure migration safety and data integrity
- **Event data**: Validate event payload structure and source
- **UI input**: Sanitize any user-provided text before display

## Conflict Resolution ##

If security guidance conflicts with other instructions:

1. **Security always wins** over convenience, performance, or style.
2. If a vulnerability exists, fix it before proceeding.
3. Document security-first trade-offs in code comments.
4. Propose performance optimizations as follow-up work if security requires overhead.

