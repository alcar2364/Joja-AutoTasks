#!/bin/bash
set -euo pipefail

collect_changed_files() {
  {
    git diff --name-only
    git diff --name-only --cached
    git ls-files --others --exclude-standard
  } 2>/dev/null | sed '/^$/d' | sort -u || true
}

collect_added_files() {
  {
    git diff --name-status | awk '$1=="A" { print $2 }'
    git diff --name-status --cached | awk '$1=="A" { print $2 }'
    git ls-files --others --exclude-standard
  } 2>/dev/null | sed '/^$/d' | sort -u || true
}

collect_agent_scope_diff() {
  {
    git diff --unified=0 -- '.github/agents/*.agent.md'
    git diff --cached --unified=0 -- '.github/agents/*.agent.md'
  } 2>/dev/null || true
}

extract_domain_table_from_stream() {
  awk '
    /^## Agent Domains \(Non-Overlapping\)/ { in_section=1; next }
    /^## / { if (in_section) exit }
    in_section { print }
  '
}

domain_table_changed_from_head() {
  local map_file="$1"

  if [[ ! -f "$map_file" ]]; then
    return 1
  fi

  if ! git cat-file -e "HEAD:${map_file}" >/dev/null 2>&1; then
    return 0
  fi

  local head_table
  local working_table
  head_table=$(git show "HEAD:${map_file}" | extract_domain_table_from_stream)
  working_table=$(extract_domain_table_from_stream < "$map_file")

  [[ "$head_table" != "$working_table" ]]
}

CHANGED=$(collect_changed_files)

if [[ -z "$CHANGED" ]]; then
  exit 0
fi

ADDED=$(collect_added_files)
BOUNDARIES_FILE=".github/instructions/agent-boundaries-and-wiring-governance.instructions.md"
BOUNDARIES_UPDATED=0

if echo "$CHANGED" | grep -qE "^${BOUNDARIES_FILE}$"; then
  BOUNDARIES_UPDATED=1
fi

DOMAIN_TABLE_UPDATED=0
if domain_table_changed_from_head "$BOUNDARIES_FILE"; then
  DOMAIN_TABLE_UPDATED=1
fi

if echo "$CHANGED" | grep -qE '^\.github/.*\.agent\.md$'; then
  echo "[ecosystem-maintenance] Agent files changed. Re-check capability freshness (description/tools/body/handoffs)."
fi

if echo "$CHANGED" | grep -qE '^\.github/instructions/.*\.instructions\.md$|^\.github/Joja AutoTasks Design Guide/.*\.md$'; then
  echo "[ecosystem-maintenance] Instructions/design guide changed. Re-check design-guide and contract alignment."
fi

if echo "$CHANGED" | grep -qE '^\.github/prompts/.*\.prompt\.md$'; then
  echo "[ecosystem-maintenance] Prompt files changed. Re-sync prompt index references."
fi

if echo "$CHANGED" | grep -qE '^\.github/skills/.*/SKILL\.md$|^\.github/skills/.*/references/'; then
  echo "[ecosystem-maintenance] Skill files changed. Re-sync skills index/catalog references."
fi

if echo "$CHANGED" | grep -qE '^\.github/'; then
  echo "[ecosystem-maintenance] Customization changes detected. Verify cross-file references and naming consistency."
fi

NEW_INSTRUCTION_ADDED=0
if echo "$ADDED" | grep -qE '^\.github/instructions/.+\.instructions\.md$'; then
  NEW_INSTRUCTION_ADDED=1
fi

NEW_SKILL_ADDED=0
if echo "$ADDED" | grep -qE '^\.github/skills/[^/]+/SKILL\.md$'; then
  NEW_SKILL_ADDED=1
fi

AGENT_SCOPE_CHANGE=0
AGENT_SCOPE_DIFF=$(collect_agent_scope_diff)
if [[ -n "$AGENT_SCOPE_DIFF" ]] && echo "$AGENT_SCOPE_DIFF" | grep -qE '^[+-](description:|argument-hint:|tools:|agents:|handoffs:)|^[+-][[:space:]]*##[[:space:]]*([0-9]+[.)]?[[:space:]]*)?(Responsibilities|Exclusions|Operating Model|Scope|Boundary|Boundaries)\b'; then
  AGENT_SCOPE_CHANGE=1
fi

if echo "$ADDED" | grep -qE '^\.github/agents/.+\.agent\.md$'; then
  AGENT_SCOPE_CHANGE=1
fi

FAIL=0

if [[ "$NEW_INSTRUCTION_ADDED" -eq 1 && "$BOUNDARIES_UPDATED" -eq 0 ]]; then
  echo "[ecosystem-maintenance] ERROR: New instruction file detected without updating ${BOUNDARIES_FILE}."
  echo "[ecosystem-maintenance] Required: update the Instruction-to-Agent Wiring table."
  FAIL=1
fi

if [[ "$NEW_SKILL_ADDED" -eq 1 && "$BOUNDARIES_UPDATED" -eq 0 ]]; then
  echo "[ecosystem-maintenance] ERROR: New skill detected without updating ${BOUNDARIES_FILE}."
  echo "[ecosystem-maintenance] Required: update the Skill-to-Agent Wiring table."
  FAIL=1
fi

if [[ "$AGENT_SCOPE_CHANGE" -eq 1 && "$BOUNDARIES_UPDATED" -eq 0 ]]; then
  echo "[ecosystem-maintenance] ERROR: Agent scope change detected without updating ${BOUNDARIES_FILE}."
  echo "[ecosystem-maintenance] Required: update the Agent Domains table first to prevent overlap drift."
  FAIL=1
fi

if [[ "$AGENT_SCOPE_CHANGE" -eq 1 && "$BOUNDARIES_UPDATED" -eq 1 && "$DOMAIN_TABLE_UPDATED" -eq 0 ]]; then
  echo "[ecosystem-maintenance] ERROR: Agent scope change detected but Agent Domains table was not updated in ${BOUNDARIES_FILE}."
  echo "[ecosystem-maintenance] Required: update the Agent Domains table first to prevent overlap drift."
  FAIL=1
fi

if [[ "$FAIL" -eq 1 ]]; then
  exit 1
fi

exit 0
