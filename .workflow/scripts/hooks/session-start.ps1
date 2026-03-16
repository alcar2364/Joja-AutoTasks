# .workflow/scripts/hooks/session-start.ps1
$StateFile = ".workflow/state/workflow-state.json"

if (-not (Test-Path $StateFile) -or -not (Get-Command jq -ErrorAction SilentlyContinue)) {
    @{
        hookSpecificOutput = @{
            hookEventName = "SessionStart"
            additionalContext = "No workflow state found. To start a new epic, run: bash .workflow/scripts/init.sh — then use the SpecOrchestrator agent with /step-1-requirements."
        }
    } | ConvertTo-Json -Depth 5
    exit 0
}

$state = Get-Content $StateFile | ConvertFrom-Json
$current = $state.current_step
$epic = if ($state.epic_name) { $state.epic_name } else { "unnamed" }
$updated = if ($state.updated_at) { $state.updated_at } else { "unknown" }

$stepNames = @("", "Requirements", "Epic Brief", "Core Flows", "PRD Validation", "Tech Plan", "Arch Validation", "Cross-Artifact", "Ticket Breakdown", "Execution", "Impl Validation")
$agents    = @("", "SpecOrchestrator", "SpecPlanner", "SpecPlanner", "SpecReviewer", "SpecPlanner", "SpecReviewer", "SpecReviewer", "SpecPlanner", "SpecOrchestrator", "SpecReviewer")

$stepsSummary = ""
for ($i = 1; $i -le 10; $i++) {
    $step = $state.steps."$i"
    $icon = switch ($step.status) { "complete" { "✅" } "in_progress" { "🔄" } default { "⬜" } }
    $marker = if ($i -eq $current) { " ◀ CURRENT" } else { "" }
    $iters = if ($step.iterations -gt 0) { " ($($step.iterations)x)" } else { "" }
    $stepsSummary += "  $icon Step $i`: $($stepNames[$i])$iters$marker`n"
}

$artifactSummary = ""
foreach ($a in @("epic-brief", "core-flows", "tech-plan")) {
    $art = $state.artifacts.$a
    if ($art.exists) {
        $artifactSummary += "  ✅ $a.md (v$($art.version))`n"
    } else {
        $artifactSummary += "  ⬜ $a.md`n"
    }
}
$tc = $state.artifacts.tickets.count
if ($tc -gt 0) { $artifactSummary += "  ✅ tickets/ ($tc tickets)`n" } else { $artifactSummary += "  ⬜ tickets/`n" }

$nextAgent = $agents[$current]
$nextSkill = $state.steps."$current".name

$ctx = @"
=== WORKFLOW STATE (injected by SessionStart hook) ===
Epic: $epic
Step: $current — $($stepNames[$current])
Updated: $updated

Steps:
$stepsSummary
Artifacts:
$artifactSummary
Next action: switch to $nextAgent and run /$nextSkill
===
"@

@{
    hookSpecificOutput = @{
        hookEventName = "SessionStart"
        additionalContext = $ctx
    }
} | ConvertTo-Json -Depth 5
