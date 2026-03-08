#!/usr/bin/env pwsh
#
# Self-Splitting Assessment Verification Hook
#
# Verifies that subagents assess whether to self-split when delegated tasks.
# Runs after runSubagent tool invocations to check for:
# - Partition plan announcements (evidence of self-splitting)
# - Explicit reasoning about why self-splitting was not applicable
# - Missing assessment (warning condition)
#
# Exit codes:
#   0 - Assessment found (self-split occurred or explicitly reasoned not to)
#   1 - Warning: Assessment appears to be missing
#   2 - Error: Critical failure

param()

$ErrorActionPreference = 'Stop'

# This hook receives the tool invocation context via environment variables
# or stdin (depending on VS Code Copilot hook implementation).
# For now, this is a placeholder implementation that demonstrates the check logic.

# Expected input: The tool result/output from the runSubagent invocation
# We check for keywords indicating self-splitting assessment occurred.

$toolOutput = $env:COPILOT_HOOK_TOOL_OUTPUT
if (-not $toolOutput) {
    # If no output available via env, read from stdin
    $toolOutput = $input | Out-String
}

if (-not $toolOutput) {
    Write-Host '[Self-Split Hook] No tool output available for verification.' -ForegroundColor DarkYellow
    exit 0
}

# Indicators that self-splitting was assessed:
$selfSplitIndicators = @(
    'partition plan',
    'self-splitting',
    'self-split',
    'spawning.*instances',
    'parallel instances',
    'not applicable.*holistic',
    'not beneficial.*tightly-coupled',
    'single-file.*no self-split'
)

$assessmentFound = $false

foreach ($indicator in $selfSplitIndicators) {
    if ($toolOutput -match $indicator) {
        $assessmentFound = $true
        break
    }
}

if ($assessmentFound) {
    Write-Host '[Self-Split Hook] ✓ Self-splitting assessment detected in subagent response.' -ForegroundColor Green
    exit 0
} else {
    Write-Host '[Self-Split Hook] ⚠ Self-splitting assessment not found in subagent response.' -ForegroundColor Yellow
    Write-Host '  This may indicate the subagent did not assess whether to self-split.' -ForegroundColor Yellow
    Write-Host '  Review the subagent output to verify assessment occurred.' -ForegroundColor Yellow
    exit 1
}
