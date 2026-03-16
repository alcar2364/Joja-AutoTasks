# .workflow/scripts/hooks/post-tool-use.ps1
$input_data = $input | ConvertFrom-Json
$toolName = $input_data.tool_name

if ($toolName -ne "editFiles" -and $toolName -ne "createFile") {
    '{"continue":true}' ; exit 0
}

$toolInput = $input_data.tool_input
$touchedFiles = @()
if ($toolInput.files) { $touchedFiles = $toolInput.files }
elseif ($toolInput.path) { $touchedFiles = @($toolInput.path) }

$workflowTouched = $touchedFiles | Where-Object { $_ -like ".workflow/*" }
if (-not $workflowTouched) { '{"continue":true}' ; exit 0 }

if (-not (Get-Command git -ErrorAction SilentlyContinue)) { '{"continue":true}' ; exit 0 }

$stateFile = ".workflow/state/workflow-state.json"
$epic = "unnamed"; $stepNum = 0; $stepName = "unknown"
if (Test-Path $stateFile) {
    $state = Get-Content $stateFile | ConvertFrom-Json
    $epic = if ($state.epic_name) { $state.epic_name } else { "unnamed" }
    $stepNum = $state.current_step
    $stepName = $state.steps."$stepNum".name
}

git add ".workflow/state/" ".workflow/artifacts/" 2>$null
$staged = git diff --cached --quiet 2>$null ; $hasChanges = $LASTEXITCODE -ne 0
if (-not $hasChanges) { '{"continue":true}' ; exit 0 }

$msg = "workflow($epic): step $stepNum/$stepName artifact update"
git commit -m $msg --no-verify 2>$null

@{
    hookSpecificOutput = @{
        hookEventName = "PostToolUse"
        additionalContext = "✅ Workflow artifacts auto-committed: $msg"
    }
} | ConvertTo-Json -Depth 5
