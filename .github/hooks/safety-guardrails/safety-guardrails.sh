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

BLOCK=0

if echo "$PROMPT" | grep -qiE '(rm -rf /|drop database|truncate table|chmod 777|ignore previous instructions|api[_-]?key\s*[:=])'; then
  echo "[safety-guardrails] Security risk pattern detected. Apply: .github/instructions/security-and-owasp.instructions.md"
  if [[ "${BLOCK_ON_SECURITY_THREAT:-false}" == "true" ]]; then
    BLOCK=1
  fi
fi

if echo "$PROMPT" | grep -qiE '(just make it work|whatever|quick and dirty|hack it)'; then
  echo "[safety-guardrails] Anti-slop guard: request is under-specified or quality-risky."
fi

if echo "$PROMPT" | grep -qiE '(directly mutate state|bypass state store|write snapshot directly|update canonical state in ui)'; then
  echo "[safety-guardrails] State mutation boundary risk detected. Preserve command -> state store -> snapshot flow."
fi

if echo "$PROMPT" | grep -qiE '(put business logic in ui|ui writes persistence|frontend persistence)'; then
  echo "[safety-guardrails] UI/backend boundary risk detected."
fi

if echo "$PROMPT" | grep -qiE '(performance|slow|latency|throughput|memory|cpu|n\+1|bottleneck|optimize)'; then
  echo "[safety-guardrails] Apply: .github/instructions/performance-optimization.instructions.md"
fi

if echo "$PROMPT" | grep -qiE '(comment every line|add lots of comments|comment everything)'; then
  echo "[safety-guardrails] Prefer self-explanatory code; comments should explain intent (why), not mechanics (what)."
fi

if [[ "$BLOCK" -eq 1 ]]; then
  echo "[safety-guardrails] Prompt blocked by BLOCK_ON_SECURITY_THREAT=true"
  exit 1
fi

exit 0
