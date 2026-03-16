# .workflow/scripts/hooks/stop-validate.ps1
$input_data = $input | ConvertFrom-Json
if ($input_data.stop_hook_active -eq $true) { '{"continue":true}' ; exit 0 }

$stateFile = ".workflow/state/workflow-state.json"
if (-not (Test-Path $stateFile)) { '{"continue":true}' ; exit 0 }

$state = Get-Content $stateFile | ConvertFrom-Json
$issues = @()

foreach ($artifact in @("epic-brief", "core-flows", "tech-plan")) {
    $art = $state.artifacts.$artifact
    if ($art.exists -eq $true) {
        $fullPath = ".workflow/$($art.path)"
        if (-not (Test-Path $fullPath)) {
            $issues += "  • State claims $artifact.md exists but file not found at $fullPath"
        }
    }
}

$tc = $state.artifacts.tickets.count
$te = $state.artifacts.tickets.exists
if ($te -eq $true -and $tc -gt 0) {
    $actual = (Get-ChildItem ".workflow/artifacts/tickets/TICKET-*.md" -ErrorAction SilentlyContinue).Count
    if ($actual -eq 0) {
        $issues += "  • State claims $tc tickets exist but .workflow/artifacts/tickets/ contains none"
    }
}

$current = $state.current_step
if ($current -lt 0 -or $current -gt 10) {
    $issues += "  • current_step is $current — must be 0–10"
}

if ($issues.Count -gt 0) {
    $reason = "Workflow state inconsistency detected — please fix before continuing:`n" + ($issues -join "`n") + "`n`nUpdate .workflow/state/workflow-state.json to match actual files on disk, or create the missing artifacts."
    @{
        hookSpecificOutput = @{
            hookEventName = "Stop"
            decision = "block"
            reason = $reason
        }
    } | ConvertTo-Json -Depth 5
} else {
    '{"continue":true}'
}
