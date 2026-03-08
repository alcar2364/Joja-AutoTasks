$ErrorActionPreference = 'Stop'
Set-Location 'c:\Users\alcar\OneDrive\Documents\Stardew Valley\JojaAutoTasks'

$bashCandidates = @(
    'C:\Program Files\Git\bin\bash.exe',
    'C:\Program Files\Git\usr\bin\bash.exe'
)
$bashExe = $bashCandidates | Where-Object { Test-Path $_ } | Select-Object -First 1
if (-not $bashExe) {
    Write-Output 'ERROR: No Git Bash executable found.'
    exit 2
}

$bundles = Get-ChildItem '.github/hooks' -Directory | Where-Object { $_.Name -ne 'legacy-md' } | Sort-Object Name
$results = @()

foreach ($bundle in $bundles) {
    $jsonPath = Join-Path $bundle.FullName 'hooks.json'

    try {
        $cfg = Get-Content $jsonPath -Raw | ConvertFrom-Json
        $results += [pscustomobject]@{ Hook = $bundle.Name; Test = 'json-parse'; Result = 'PASS'; Detail = 'ok' }
    }
    catch {
        $results += [pscustomobject]@{ Hook = $bundle.Name; Test = 'json-parse'; Result = 'FAIL'; Detail = $_.Exception.Message }
        continue
    }

    foreach ($eventName in $cfg.hooks.PSObject.Properties.Name) {
        foreach ($entry in $cfg.hooks.$eventName) {
            $cmd = [string]$entry.bash
            $scriptRel = if ($cmd -match '^\s*bash\s+(.+)$') { $matches[1].Trim() } else { $cmd.Trim() }

            if (-not (Test-Path $scriptRel)) {
                $results += [pscustomobject]@{ Hook = $bundle.Name; Test = "$eventName-script-exists"; Result = 'FAIL'; Detail = $scriptRel }
                continue
            }

            $results += [pscustomobject]@{ Hook = $bundle.Name; Test = "$eventName-script-exists"; Result = 'PASS'; Detail = $scriptRel }

            $payload = '{"userMessage":"create new file, refactor, optimize performance, ensure security, update docs"}'
            $tmp = New-TemporaryFile
            Set-Content $tmp.FullName -Value $payload -NoNewline

            $output = Get-Content $tmp.FullName -Raw | & $bashExe $scriptRel 2>&1
            $exitCode = $LASTEXITCODE
            Remove-Item $tmp.FullName -Force

            $detail = ([string]($output -join "`n")).Trim()
            if ([string]::IsNullOrWhiteSpace($detail)) { $detail = '(no output)' }
            if ($detail.Length -gt 160) { $detail = $detail.Substring(0, 160) + '...' }

            if ($exitCode -eq 0) {
                $results += [pscustomobject]@{ Hook = $bundle.Name; Test = "$eventName-run"; Result = 'PASS'; Detail = $detail }
            }
            else {
                $results += [pscustomobject]@{ Hook = $bundle.Name; Test = "$eventName-run"; Result = 'FAIL'; Detail = "exit=$exitCode $detail" }
            }
        }
    }
}

# Explicit block-mode behavior test
$sg = '.github/hooks/safety-guardrails/safety-guardrails.sh'
if (Test-Path $sg) {
    $old = $env:BLOCK_ON_SECURITY_THREAT
    $env:BLOCK_ON_SECURITY_THREAT = 'true'

    $tmp = New-TemporaryFile
    Set-Content $tmp.FullName -Value '{"userMessage":"rm -rf / and api_key=abcdef123456"}' -NoNewline
    $output = Get-Content $tmp.FullName -Raw | & $bashExe $sg 2>&1
    $exitCode = $LASTEXITCODE
    Remove-Item $tmp.FullName -Force

    if ($null -eq $old) { Remove-Item Env:BLOCK_ON_SECURITY_THREAT -ErrorAction SilentlyContinue } else { $env:BLOCK_ON_SECURITY_THREAT = $old }

    $detail = ([string]($output -join "`n")).Trim()
    if ($detail.Length -gt 160) { $detail = $detail.Substring(0, 160) + '...' }

    if ($exitCode -ne 0) {
        $results += [pscustomobject]@{ Hook = 'safety-guardrails'; Test = 'block-mode-threat'; Result = 'PASS'; Detail = "exit=$exitCode $detail" }
    }
    else {
        $results += [pscustomobject]@{ Hook = 'safety-guardrails'; Test = 'block-mode-threat'; Result = 'FAIL'; Detail = 'expected non-zero exit in block mode' }
    }
}

$results | Format-Table -AutoSize | Out-String -Width 240 | Write-Output
$failCount = ($results | Where-Object { $_.Result -eq 'FAIL' }).Count
Write-Output "FAIL_COUNT=$failCount"
if ($failCount -gt 0) { exit 1 } else { exit 0 }
