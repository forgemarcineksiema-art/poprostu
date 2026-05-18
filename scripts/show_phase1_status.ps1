param(
    [string]$MetricsPath = (Join-Path $env:USERPROFILE "AppData\LocalLow\DefaultCompany\My project\phase1_latest_run.txt")
)

$ErrorActionPreference = "Stop"

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path -Parent $scriptRoot
$logsDir = Join-Path $repoRoot "Logs"

function Read-TestRunSummary {
    param(
        [string]$Name,
        [string]$Path
    )

    if (-not (Test-Path -LiteralPath $Path)) {
        Write-Host "${Name}: missing ($Path)"
        return
    }

    [xml]$results = Get-Content -LiteralPath $Path
    $run = $results."test-run"
    Write-Host "${Name}: total=$($run.total) passed=$($run.passed) failed=$($run.failed)"
}

function Read-BuildSummary {
    param([string]$Path)

    if (-not (Test-Path -LiteralPath $Path)) {
        Write-Host "Dev build: missing ($Path)"
        return
    }

    $success = Select-String -Path $Path -Pattern "Build Finished, Result: Success" -SimpleMatch -Quiet
    $summary = Select-String -Path $Path -Pattern "Phase 1 build result:" | Select-Object -Last 1
    if ($success -and $summary) {
        Write-Host "Dev build: success"
        Write-Host "  $($summary.Line)"
    } elseif ($success) {
        Write-Host "Dev build: success"
    } else {
        Write-Host "Dev build: no success marker found"
    }
}

Write-Host "Phase 1 status"
Write-Host "Repo: $repoRoot"

Push-Location $repoRoot
try {
    $branch = git branch --show-current
    $aheadBehind = git rev-list --left-right --count HEAD...origin/$branch 2>$null
    Write-Host "Branch: $branch"
    if ($aheadBehind) {
        Write-Host "HEAD...origin/${branch}: $aheadBehind"
    }

    Write-Host ""
    Write-Host "Automated gate:"
    $validatorLog = Join-Path $logsDir "phase1_verify_scene_validator.log"
    if (Test-Path -LiteralPath $validatorLog) {
        $validatorPassed = Select-String -Path $validatorLog -Pattern "Phase 1 scene validation passed." -SimpleMatch -Quiet
        Write-Host "Scene validator: $(if ($validatorPassed) { 'passed' } else { 'no pass marker found' })"
    } else {
        Write-Host "Scene validator: missing"
    }

    Read-TestRunSummary -Name "EditMode" -Path (Join-Path $logsDir "phase1_verify_editmode_results.xml")
    Read-TestRunSummary -Name "PlayMode" -Path (Join-Path $logsDir "phase1_verify_playmode_results.xml")
    Read-BuildSummary -Path (Join-Path $logsDir "phase1_verify_dev_build.log")

    Write-Host ""
    Write-Host "Manual coverage:"
    Write-Host "Metrics: $MetricsPath"
    if (Test-Path -LiteralPath $MetricsPath) {
        $coverage = Get-Content -LiteralPath $MetricsPath | Where-Object { $_ -like "CoverageComplete:*" } | Select-Object -First 1
        $status = Get-Content -LiteralPath $MetricsPath | Where-Object { $_ -like "CoverageStatus:*" } | Select-Object -First 1
        Write-Host "  $coverage"
        Write-Host "  $status"
    } else {
        Write-Host "  Metrics file not found yet."
    }

    Write-Host ""
    Write-Host "Next command:"
    Write-Host "  powershell -NoProfile -ExecutionPolicy Bypass -File scripts\run_phase1_manual_gate.ps1"

    Write-Host ""
    Write-Host "Working tree content diff:"
    $diffStat = git diff --stat
    if ($diffStat) {
        $diffStat
    } else {
        Write-Host "  No content diff."
    }
} finally {
    Pop-Location
}
