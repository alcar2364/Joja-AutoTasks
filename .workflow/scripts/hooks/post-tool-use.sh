#!/usr/bin/env bash
# .workflow/scripts/hooks/post-tool-use.sh
# Fires after every tool call completes.
# Watches for writes to .workflow/artifacts/ or .workflow/state/ and
# auto-commits them to git so every artifact change is snapshotted.
set -euo pipefail

INPUT=$(cat)
TOOL_NAME=$(echo "$INPUT" | jq -r '.tool_name // empty')

# Only care about file-writing tools
if [ "$TOOL_NAME" != "editFiles" ] && [ "$TOOL_NAME" != "createFile" ]; then
  echo '{"continue":true}'
  exit 0
fi

# Extract files touched by this tool call
TOOL_INPUT=$(echo "$INPUT" | jq -r '.tool_input // {}')

# Collect file paths — editFiles uses .files[], createFile uses .path
TOUCHED_FILES=$(echo "$TOOL_INPUT" | jq -r '
  if .files then .files[] elif .path then .path else empty end
' 2>/dev/null || true)

if [ -z "$TOUCHED_FILES" ]; then
  echo '{"continue":true}'
  exit 0
fi

# Check if any touched file is inside .workflow/
WORKFLOW_TOUCHED=false
for f in $TOUCHED_FILES; do
  if echo "$f" | grep -q "^\.workflow/"; then
    WORKFLOW_TOUCHED=true
    break
  fi
done

if [ "$WORKFLOW_TOUCHED" = "false" ]; then
  echo '{"continue":true}'
  exit 0
fi

# Only commit if inside a git repo
if ! command -v git &>/dev/null || ! git rev-parse --git-dir &>/dev/null 2>&1; then
  echo '{"continue":true}'
  exit 0
fi

STATE_FILE=".workflow/state/workflow-state.json"
EPIC="unnamed"
STEP_NUM="0"
STEP_NAME="unknown"

if [ -f "$STATE_FILE" ] && command -v jq &>/dev/null; then
  EPIC=$(jq -r '.epic_name // "unnamed"' "$STATE_FILE")
  STEP_NUM=$(jq -r '.current_step // 0' "$STATE_FILE")
  STEP_NAME=$(jq -r ".steps[\"$STEP_NUM\"].name // \"step-$STEP_NUM\"" "$STATE_FILE")
fi

# Stage workflow artifacts and state only
git add ".workflow/state/" ".workflow/artifacts/" 2>/dev/null || true

# Only commit if there's something staged
if git diff --cached --quiet 2>/dev/null; then
  echo '{"continue":true}'
  exit 0
fi

COMMIT_MSG="workflow($EPIC): step $STEP_NUM/$STEP_NAME artifact update"
git commit -m "$COMMIT_MSG" --no-verify 2>/dev/null || true

# Inject a brief confirmation as context so the agent knows the commit happened
jq -n \
  --arg msg "$COMMIT_MSG" \
  '{
    hookSpecificOutput: {
      hookEventName: "PostToolUse",
      additionalContext: ("✅ Workflow artifacts auto-committed: " + $msg)
    }
  }'
