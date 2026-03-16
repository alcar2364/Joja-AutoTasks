#!/usr/bin/env bash
# .workflow/scripts/hooks/session-start.sh
# Fires at the start of every agent session.
# Injects current workflow state as additionalContext so agents always
# know position, active epic, and artifact status without reading files.
set -euo pipefail

STATE_FILE=".workflow/state/workflow-state.json"

# No state yet — workflow hasn't been initialized
if [ ! -f "$STATE_FILE" ] || ! command -v jq &>/dev/null; then
  cat <<'EOF'
{
  "hookSpecificOutput": {
    "hookEventName": "SessionStart",
    "additionalContext": "No workflow state found. To start a new epic, run: bash .workflow/scripts/init.sh — then use the SpecOrchestrator agent with /step-1-requirements."
  }
}
EOF
  exit 0
fi

CURRENT=$(jq -r '.current_step' "$STATE_FILE")
EPIC=$(jq -r '.epic_name // "unnamed"' "$STATE_FILE")
UPDATED=$(jq -r '.updated_at // "unknown"' "$STATE_FILE")

STEP_NAMES=("" "Requirements" "Epic Brief" "Core Flows" "PRD Validation" "Tech Plan" "Arch Validation" "Cross-Artifact" "Ticket Breakdown" "Execution" "Impl Validation")
AGENTS=("" "SpecOrchestrator" "SpecPlanner" "SpecPlanner" "SpecReviewer" "SpecPlanner" "SpecReviewer" "SpecReviewer" "SpecPlanner" "SpecOrchestrator" "SpecReviewer")

# Build step summary
STEPS_SUMMARY=""
for i in $(seq 1 10); do
  STATUS=$(jq -r ".steps[\"$i\"].status" "$STATE_FILE")
  ITERS=$(jq -r ".steps[\"$i\"].iterations" "$STATE_FILE")
  case "$STATUS" in
    complete)    ICON="✅" ;;
    in_progress) ICON="🔄" ;;
    *)           ICON="⬜" ;;
  esac
  CURRENT_MARKER=""
  [ "$i" -eq "$CURRENT" ] && CURRENT_MARKER=" ◀ CURRENT"
  ITER_STR=""
  [ "$ITERS" -gt 0 ] && ITER_STR=" (${ITERS}x)"
  STEPS_SUMMARY="${STEPS_SUMMARY}  ${ICON} Step ${i}: ${STEP_NAMES[$i]}${ITER_STR}${CURRENT_MARKER}\n"
done

# Build artifact summary
ARTIFACT_SUMMARY=""
for artifact in epic-brief core-flows tech-plan; do
  EXISTS=$(jq -r ".artifacts[\"$artifact\"].exists" "$STATE_FILE")
  VERSION=$(jq -r ".artifacts[\"$artifact\"].version" "$STATE_FILE")
  if [ "$EXISTS" = "true" ]; then
    ARTIFACT_SUMMARY="${ARTIFACT_SUMMARY}  ✅ ${artifact}.md (v${VERSION})\n"
  else
    ARTIFACT_SUMMARY="${ARTIFACT_SUMMARY}  ⬜ ${artifact}.md\n"
  fi
done

TICKET_COUNT=$(jq -r '.artifacts.tickets.count' "$STATE_FILE")
if [ "$TICKET_COUNT" -gt 0 ]; then
  ARTIFACT_SUMMARY="${ARTIFACT_SUMMARY}  ✅ tickets/ (${TICKET_COUNT} tickets)\n"
else
  ARTIFACT_SUMMARY="${ARTIFACT_SUMMARY}  ⬜ tickets/\n"
fi

# Next action
NEXT_AGENT="${AGENTS[$CURRENT]:-unknown}"
NEXT_SKILL=$(jq -r ".steps[\"$CURRENT\"].name // \"step-$CURRENT\"" "$STATE_FILE" 2>/dev/null || echo "step-$CURRENT")

CONTEXT="=== WORKFLOW STATE (injected by SessionStart hook) ===
Epic: $EPIC
Step: $CURRENT — ${STEP_NAMES[$CURRENT]:-Unknown}
Updated: $UPDATED

Steps:
$(printf "$STEPS_SUMMARY")
Artifacts:
$(printf "$ARTIFACT_SUMMARY")
Next action: switch to $NEXT_AGENT and run /$NEXT_SKILL
==="

# Output valid JSON with the context
jq -n \
  --arg ctx "$CONTEXT" \
  '{
    hookSpecificOutput: {
      hookEventName: "SessionStart",
      additionalContext: $ctx
    }
  }'
