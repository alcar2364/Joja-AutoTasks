#!/bin/bash
set -euo pipefail

INPUT=$(cat || true)
PROMPT="$INPUT"

if command -v jq >/dev/null 2>&1; then
  PARSED=$(printf '%s' "$INPUT" | jq -r '.userMessage // .prompt // empty' 2>/dev/null || true)
  if [[ -n "$PARSED" ]]; then
    PROMPT="$PARSED"
  fi
fi

if echo "$PROMPT" | grep -qiE '(create|new file|new module|refactor|reorganize|folder|structure|rename|move file)'; then
  echo "[context-preflight] Load: .github/instructions/context-engineering.instructions.md"
  echo "[context-preflight] Load: .github/instructions/csharp-style-contract.instructions.md"
fi

if echo "$PROMPT" | grep -qiE '(workflow|github actions|ci/cd|pipeline|\.github/workflows|yaml)'; then
  echo "[context-preflight] Load: .github/instructions/github-actions-ci-cd-best-practices.instructions.md"
fi

if echo "$PROMPT" | grep -qiE '(plan|planning|spec|design guide|architecture decision|adr)'; then
  echo "[context-preflight] Load: .github/instructions/workspace-contracts.instructions.md"
fi

if echo "$PROMPT" | grep -qiE '(handoff|delegate|subagent|route to)'; then
  echo "[context-preflight] Apply handoff optimization and avoid circular routing."
fi

if echo "$PROMPT" | grep -qiE '(troubleshoot|troubleshooting|diagnose|diagnosis|debug|root cause|bug)'; then
  echo "[context-preflight] If root cause reveals major architecture problem (especially from agent code), route to WorkspaceAgent. Minor bugs: no doc routing."
fi

exit 0
