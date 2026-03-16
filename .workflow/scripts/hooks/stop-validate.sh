#!/usr/bin/env bash
# .workflow/scripts/hooks/stop-validate.sh
# Fires when the agent session is about to end.
# Validates that workflow state is consistent with actual files on disk.
# Blocks the session from ending if inconsistencies are found.
set -euo pipefail

INPUT=$(cat)

# Prevent infinite loop — if we already blocked once, let it stop
STOP_HOOK_ACTIVE=$(echo "$INPUT" | jq -r '.stop_hook_active // false')
if [ "$STOP_HOOK_ACTIVE" = "true" ]; then
  echo '{"continue":true}'
  exit 0
fi

STATE_FILE=".workflow/state/workflow-state.json"

# No state file — nothing to validate
if [ ! -f "$STATE_FILE" ] || ! command -v jq &>/dev/null; then
  echo '{"continue":true}'
  exit 0
fi

ISSUES=""

# Check each artifact: if state says it exists, the file must exist
for artifact in epic-brief core-flows tech-plan; do
  CLAIMED=$(jq -r ".artifacts[\"$artifact\"].exists" "$STATE_FILE")
  if [ "$CLAIMED" = "true" ]; then
    ARTIFACT_PATH=$(jq -r ".artifacts[\"$artifact\"].path" "$STATE_FILE")
    FULL_PATH=".workflow/$ARTIFACT_PATH"
    if [ ! -f "$FULL_PATH" ]; then
      ISSUES="${ISSUES}  • State claims ${artifact}.md exists but file not found at ${FULL_PATH}\n"
    fi
  fi
done

# Check ticket count consistency
TICKET_COUNT_CLAIMED=$(jq -r '.artifacts.tickets.count' "$STATE_FILE")
TICKET_EXISTS=$(jq -r '.artifacts.tickets.exists' "$STATE_FILE")
if [ "$TICKET_EXISTS" = "true" ] && [ "$TICKET_COUNT_CLAIMED" -gt 0 ]; then
  ACTUAL_COUNT=$(find ".workflow/artifacts/tickets" -name "TICKET-*.md" 2>/dev/null | wc -l | tr -d ' ')
  if [ "$ACTUAL_COUNT" -eq 0 ]; then
    ISSUES="${ISSUES}  • State claims $TICKET_COUNT_CLAIMED tickets exist but .workflow/artifacts/tickets/ contains none\n"
  fi
fi

# Check that current_step is within valid range
CURRENT=$(jq -r '.current_step' "$STATE_FILE")
if [ "$CURRENT" -lt 0 ] || [ "$CURRENT" -gt 10 ]; then
  ISSUES="${ISSUES}  • current_step is $CURRENT — must be 0–10\n"
fi

if [ -n "$ISSUES" ]; then
  REASON="Workflow state inconsistency detected — please fix before continuing:\n${ISSUES}\nUpdate .workflow/state/workflow-state.json to match actual files on disk, or create the missing artifacts."
  jq -n \
    --arg reason "$(printf "$REASON")" \
    '{
      hookSpecificOutput: {
        hookEventName: "Stop",
        decision: "block",
        reason: $reason
      }
    }'
else
  echo '{"continue":true}'
fi
