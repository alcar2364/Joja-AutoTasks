#!/bin/bash
set -euo pipefail

CHANGED=$( (git diff --name-only; git diff --name-only --cached) 2>/dev/null | sort -u || true)

if [[ -z "$CHANGED" ]]; then
  exit 0
fi

if echo "$CHANGED" | grep -qE '(^|/)Domain/Identifiers/.*\.cs$|(^|/)Domain/Tasks/.*\.cs$|Rule.*\.cs$|Generator.*\.cs$'; then
  echo "[validation-postflight] Identifier-sensitive files changed. Validate determinism for TaskID/RuleID/SubjectID/DayKey."
fi

if echo "$CHANGED" | grep -qE 'Persistence|Migration|Configuration/.*\.cs$'; then
  echo "[validation-postflight] Persistence/migration changes detected. Validate versioning and reconstruction safety."
fi

PROD_CHANGED=$(echo "$CHANGED" | grep -E '(^Domain/|^Events/|^Infrastructure/|^Lifecycle/|^Startup/|^UI/).*\.cs$' || true)
TEST_CHANGED=$(echo "$CHANGED" | grep -E '^Tests/.*\.cs$' || true)
if [[ -n "$PROD_CHANGED" && -z "$TEST_CHANGED" ]]; then
  echo "[validation-postflight] Production C# changed without test updates. Review unit-testing-contract coverage requirements."
fi

CODE_CHANGED=$(echo "$CHANGED" | grep -Ei '\.(cs|sml|json|csproj)$' || true)
DOC_CHANGED=$(echo "$CHANGED" | grep -Ei '(^README\.md$|\.md$)' || true)
if [[ -n "$CODE_CHANGED" && -z "$DOC_CHANGED" ]]; then
  echo "[validation-postflight] Code changed without markdown updates. Check docs sync requirements."
fi

exit 0
