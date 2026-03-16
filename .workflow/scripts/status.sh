#!/usr/bin/env bash
# .workflow/scripts/status.sh — Print current workflow state to terminal
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
WORKFLOW_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
STATE_FILE="$WORKFLOW_DIR/state/workflow-state.json"

if ! command -v jq &>/dev/null; then
  echo "❌ jq is required: brew install jq  OR  sudo apt install jq"
  exit 1
fi

if [ ! -f "$STATE_FILE" ]; then
  echo "⚠️  No workflow state found."
  echo "   Run: bash .workflow/scripts/init.sh"
  exit 1
fi

CURRENT=$(jq -r '.current_step' "$STATE_FILE")
EPIC=$(jq -r '.epic_name // "unnamed"' "$STATE_FILE")
UPDATED=$(jq -r '.updated_at // "never"' "$STATE_FILE")

STEP_NAMES=(""
  "Requirements Gathering"
  "Epic Brief"
  "Core Flows"
  "PRD Validation"
  "Tech Plan"
  "Arch Validation"
  "Cross-Artifact"
  "Ticket Breakdown"
  "Execution"
  "Impl Validation"
)

AGENTS=("" orchestrator planner planner reviewer planner reviewer reviewer planner orchestrator reviewer)

echo ""
echo "╔══════════════════════════════════════════════════════╗"
echo "║  SPEC-DRIVEN WORKFLOW                                ║"
echo "╠══════════════════════════════════════════════════════╣"
printf "║  Epic:    %-42s║\n" "$EPIC"
printf "║  Updated: %-42s║\n" "$UPDATED"
echo "╠══════════════════════════════════════════════════════╣"

for i in $(seq 1 10); do
  STATUS=$(jq -r ".steps[\"$i\"].status" "$STATE_FILE")
  ITERS=$(jq -r ".steps[\"$i\"].iterations" "$STATE_FILE")
  case "$STATUS" in
    complete)    ICON="✅" ;;
    in_progress) ICON="🔄" ;;
    *)           ICON="⬜" ;;
  esac
  POINTER=""
  [ "$i" -eq "$CURRENT" ] && POINTER=" ◀"
  ITER_STR=""
  [ "$ITERS" -gt 0 ] && ITER_STR=" (${ITERS}x)"
  printf "║  %s  %-2s  @%-12s  %-18s%s%s\n" \
    "$ICON" "$i" "${AGENTS[$i]}" "${STEP_NAMES[$i]}" "$ITER_STR" "$POINTER"
done

echo "╠══════════════════════════════════════════════════════╣"

for artifact in epic-brief core-flows tech-plan; do
  EXISTS=$(jq -r ".artifacts[\"$artifact\"].exists" "$STATE_FILE")
  VERSION=$(jq -r ".artifacts[\"$artifact\"].version" "$STATE_FILE")
  if [ "$EXISTS" = "true" ]; then
    printf "║  ✅  %-48s║\n" "$artifact.md (v$VERSION)"
  else
    printf "║  ⬜  %-48s║\n" "$artifact.md"
  fi
done

TICKET_COUNT=$(jq -r '.artifacts.tickets.count' "$STATE_FILE")
if [ "$TICKET_COUNT" -gt 0 ]; then
  printf "║  ✅  %-48s║\n" "tickets/ ($TICKET_COUNT tickets)"
else
  printf "║  ⬜  %-48s║\n" "tickets/"
fi

echo "╚══════════════════════════════════════════════════════╝"

REV_COUNT=$(jq '.revision_history | length' "$STATE_FILE")
if [ "$REV_COUNT" -gt 0 ]; then
  echo ""
  echo "📝 Revision History ($REV_COUNT):"
  jq -r '.revision_history[] | "   \(.timestamp)  \(.description)"' "$STATE_FILE"
fi

if [ "$CURRENT" -gt 0 ] && [ "$CURRENT" -le 10 ]; then
  echo ""
  echo "💬 Continue in Copilot Chat:"
  AGENT="${AGENTS[$CURRENT]}"
  SKILL=$(jq -r ".steps[\"$CURRENT\"].skill" "$WORKFLOW_DIR/agents.json" 2>/dev/null || echo "step-$CURRENT")
  echo "   @$AGENT /$SKILL"
fi
echo ""
