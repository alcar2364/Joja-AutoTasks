#!/usr/bin/env bash
# .workflow/scripts/init.sh
# Initialize workflow state for a new epic
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
WORKFLOW_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
STATE_FILE="$WORKFLOW_DIR/state/workflow-state.json"

if [ ! -f "$STATE_FILE" ]; then
  echo "❌ State file not found at $STATE_FILE"
  exit 1
fi

# Check for existing in-progress epic
CURRENT=$(jq -r '.current_step' "$STATE_FILE" 2>/dev/null || echo "0")
EPIC=$(jq -r '.epic_name // empty' "$STATE_FILE" 2>/dev/null || echo "")

if [ -n "$EPIC" ] && [ "$CURRENT" -gt "0" ]; then
  echo "⚠️  An epic is already in progress: \"$EPIC\" (Step $CURRENT)"
  printf "Start fresh and overwrite? [y/N] "
  read -r CONFIRM
  if [ "${CONFIRM:-n}" != "y" ]; then
    echo "Aborted. Current workflow preserved."
    exit 0
  fi
fi

printf "Epic name: "
read -r EPIC_NAME

if [ -z "$EPIC_NAME" ]; then
  echo "❌ Epic name is required."
  exit 1
fi

EPIC_ID=$(echo "$EPIC_NAME" | tr '[:upper:]' '[:lower:]' | sed 's/[^a-z0-9]/-/g' | sed 's/--*/-/g' | sed 's/^-\|-$//g')
NOW=$(date -u +"%Y-%m-%dT%H:%M:%SZ")

jq \
  --arg id "$EPIC_ID" \
  --arg name "$EPIC_NAME" \
  --arg now "$NOW" \
  '.epic_id = $id
  | .epic_name = $name
  | .current_step = 1
  | .created_at = $now
  | .updated_at = $now
  | .revision_history = []
  | .steps["1"].status = "in_progress"
  | .steps["2"].status = "not_started"
  | .steps["3"].status = "not_started"
  | .steps["4"].status = "not_started"
  | .steps["5"].status = "not_started"
  | .steps["6"].status = "not_started"
  | .steps["7"].status = "not_started"
  | .steps["8"].status = "not_started"
  | .steps["9"].status = "not_started"
  | .steps["10"].status = "not_started"
  | .artifacts["epic-brief"].exists = false
  | .artifacts["core-flows"].exists = false
  | .artifacts["tech-plan"].exists = false
  | .artifacts["tickets"].exists = false' \
  "$STATE_FILE" > "$STATE_FILE.tmp" && mv "$STATE_FILE.tmp" "$STATE_FILE"

echo ""
echo "✅ Workflow initialized"
echo "   Epic: $EPIC_NAME ($EPIC_ID)"
echo "   Step 1 is now active"
echo ""
echo "💬 In Copilot Chat, start with:"
echo "   @orchestrator /step-1-requirements"
