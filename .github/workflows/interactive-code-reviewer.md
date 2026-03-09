---
name: interactive-code-reviewer
description: "Interactive code review triggered by /review command to provide architectural and quality feedback."
on:
  issue_comment:
    types: [created]
  pull_request_review_comment:
    types: [created]
permissions:
  contents: read
  pull-requests: read
  issues: read
strict: true
network:
  allowed: [defaults, github]
engine:
  id: copilot
if: contains(github.event.comment.body, '/review')
---

# Interactive Code Reviewer

On-demand architectural and quality code review triggered by `/review` command.

## Trigger

- `/review` — Request architectural code review for current PR
- Response: Structural analysis + recommendations via comment

## Review Focus

1. **Architecture Compliance:**
   - Domain module boundaries respected
   - Command/Event dispatcher contracts followed
   - Lifecycle signal handling patterns correct
   - StateStore mutation boundaries preserved

2. **Design Patterns:**
   - Identifier determinism maintained
   - Configuration immutability preserved
   - Proper use of domain value types

3. **Code Quality:**
   - SOLID principles
   - Error handling completeness
   - Testability of changes

## Output

- Post review analysis as PR comment
- Flag architecture violations immediately
- Provide improvement suggestions
- Reference relevant architecture contracts

## Notes

- Review code, not implementation details
- Focus on structure and contracts, not style
- If multiple issues found, prioritize architecture first
- For complex refactorings, suggest breaking into phases
