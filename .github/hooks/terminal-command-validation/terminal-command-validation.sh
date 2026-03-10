#!/bin/bash
set -euo pipefail

INPUT=$(cat || true)
COMMAND="$INPUT"

# Extract command if JSON wrapped
if command -v jq >/dev/null 2>&1; then
  PARSED=$(printf '%s' "$INPUT" | jq -r '.command // empty' 2>/dev/null || true)
  if [[ -n "$PARSED" ]]; then
    COMMAND="$PARSED"
  fi
fi

# Check for tool alternatives (file/content operations)
if echo "$COMMAND" | grep -qiE '(Get-Content|gc\s|cat\s|\$lines|Select-String|findstr)'; then
  echo "[terminal-validation] ⚠️  Consider read_file or grep_search instead of terminal commands for file content access."
fi

if echo "$COMMAND" | grep -qiE '(Get-ChildItem|gci\s|ls\s|dir\s|\bls\b)'; then
  echo "[terminal-validation] ⚠️  Consider list_dir instead of terminal directory listing."
fi

if echo "$COMMAND" | grep -qiE '(Test-Path|check.*exist|file.*exist)'; then
  echo "[terminal-validation] ⚠️  Consider read_file (fails if missing) instead of Test-Path."
fi

# Check command complexity
LINE_COUNT=$(echo "$COMMAND" | wc -l)
COMMAND_LENGTH=$(echo "$COMMAND" | wc -c)
PIPE_COUNT=$(echo "$COMMAND" | grep -o '|' | wc -l || echo 0)
FOREACH_COUNT=$(echo "$COMMAND" | grep -ioE '(foreach|for\s*\(|%\s*{)' | wc -l || echo 0)

if [[ "$COMMAND_LENGTH" -gt 500 ]]; then
  echo "[terminal-validation] ⚠️  Command exceeds 500 characters. Consider breaking into multiple tool calls or simplifying."
fi

if [[ "$PIPE_COUNT" -gt 3 ]]; then
  echo "[terminal-validation] ⚠️  Command has $PIPE_COUNT pipes (limit: 3). Simplify or use alternative tools."
fi

if [[ "$FOREACH_COUNT" -gt 0 ]]; then
  echo "[terminal-validation] ⚠️  Command contains loops. Consider using parallel read_file/grep_search calls instead."
fi

# Check quoting patterns (PowerShell specific warnings)
if echo "$COMMAND" | grep -qE '"\$[a-zA-Z_][a-zA-Z0-9_]*[^`]'; then
  echo "[terminal-validation] ⚠️  Unescaped \$ in double quotes may cause variable expansion issues. Use backtick: \`\$"
fi

if echo "$COMMAND" | grep -qE '"[^"]*\$[0-9]+[^"\`]'; then
  echo "[terminal-validation] ⚠️  Positional parameter \$N in double quotes requires escaping: \`\$N"
fi

# Check for common escaping mistakes
if echo "$COMMAND" | grep -qE '\\["'\'']'; then
  echo "[terminal-validation] ⚠️  Backslash doesn't escape in PowerShell. Use backtick (\`) as escape character."
fi

# Check for file path spaces without quotes
if echo "$COMMAND" | grep -qE '[A-Z]:\\[^"'\'']*\s+[^"'\'']*\.'; then
  echo "[terminal-validation] ⚠️  File path may contain spaces without quotes. Quote paths with spaces."
fi

# Suggest instruction reference
if echo "$COMMAND" | grep -qiE '(Get-Content|foreach|quote|escape|\$|@)'; then
  echo "[terminal-validation] 📖 Reference: .github/instructions/powershell-terminal-best-practices.instructions.md"
fi

exit 0
