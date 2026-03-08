#!/bin/bash
set -euo pipefail

CHANGED=$( (git diff --name-only; git diff --name-only --cached) 2>/dev/null | sort -u || true)

if [[ -z "$CHANGED" ]]; then
  exit 0
fi

if echo "$CHANGED" | grep -qE '^\.local/Agents/.*\.agent\.md$'; then
  echo "[ecosystem-maintenance] Agent files changed. Re-check capability freshness (description/tools/body/handoffs)."
fi

if echo "$CHANGED" | grep -qE '^\.local/Agents/(Instructions|Contracts)/.*\.instructions\.md$|^\.local/Joja AutoTasks Design Guide/.*\.md$'; then
  echo "[ecosystem-maintenance] Instructions/design guide changed. Re-check design-guide and contract alignment."
fi

if echo "$CHANGED" | grep -qE '^\.local/Agents/Prompts/.*\.prompt\.md$'; then
  echo "[ecosystem-maintenance] Prompt files changed. Re-sync prompt index references."
fi

if echo "$CHANGED" | grep -qE '^\.local/Agents/Skills/.*/SKILL\.md$|^\.local/Agents/Skills/.*/references/'; then
  echo "[ecosystem-maintenance] Skill files changed. Re-sync skills index/catalog references."
fi

if echo "$CHANGED" | grep -qE '^\.local/Agents/'; then
  echo "[ecosystem-maintenance] Customization changes detected. Verify cross-file references and naming consistency."
fi

exit 0
