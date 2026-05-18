param(
    [string]$ReportPath,
    [switch]$AllowPending
)

$ErrorActionPreference = "Stop"

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path -Parent $scriptRoot
$reportsDir = Join-Path $repoRoot "docs\prototype_reports"

function Get-LatestManualReport {
    if (-not (Test-Path -LiteralPath $reportsDir)) {
        return $null
    }

    Get-ChildItem -LiteralPath $reportsDir -Filter "phase1_manual_playtest_*.md" |
        Sort-Object LastWriteTime -Descending |
        Select-Object -First 1
}

function Test-CheckedLine {
    param(
        [string[]]$Lines,
        [string]$Label
    )

    $pattern = "^\s*-\s*\[[xX]\]\s*" + [regex]::Escape($Label) + "\s*$"
    foreach ($line in $Lines) {
        if ($line -match $pattern) {
            return $true
        }
    }

    return $false
}

function Test-AnyCheckedDecision {
    param([string[]]$Lines)

    foreach ($line in $Lines) {
        if ($line -match "^\s*-\s*\[[xX]\]\s*Phase 1 ") {
            return $true
        }
    }

    return $false
}

if ([string]::IsNullOrWhiteSpace($ReportPath)) {
    $latestReport = Get-LatestManualReport
    if ($null -eq $latestReport) {
        Write-Host "Phase 1 manual decision: pending"
        Write-Host "Reason: no docs\prototype_reports\phase1_manual_playtest_*.md report found."
        Write-Host "Run: powershell -NoProfile -ExecutionPolicy Bypass -File scripts\run_phase1_manual_gate.ps1"
        if ($AllowPending) {
            exit 0
        }

        exit 2
    }

    $ReportPath = $latestReport.FullName
}

if (-not (Test-Path -LiteralPath $ReportPath)) {
    throw "Report not found: $ReportPath"
}

$lines = @(Get-Content -LiteralPath $ReportPath)
$accepted = Test-CheckedLine -Lines $lines -Label "Phase 1 feel accepted for Phase 2."
$blockedReasons = @(
    "Phase 1 needs controller iteration.",
    "Phase 1 needs camera iteration.",
    "Phase 1 needs vehicle iteration.",
    "Phase 1 needs route/layout iteration."
)
$checkedBlockers = @()
foreach ($reason in $blockedReasons) {
    if (Test-CheckedLine -Lines $lines -Label $reason) {
        $checkedBlockers += $reason
    }
}

Write-Host "Phase 1 manual decision"
Write-Host "Report: $ReportPath"

if ($accepted -and $checkedBlockers.Count -gt 0) {
    Write-Host "Status: conflicting"
    Write-Host "Reason: report marks Phase 2 accepted and also marks blocker decisions."
    foreach ($blocker in $checkedBlockers) {
        Write-Host "  Blocker: $blocker"
    }
    exit 4
}

if ($accepted) {
    Write-Host "Status: accepted"
    Write-Host "Phase 2 may start, assuming the written notes do not contradict the checked decision."
    exit 0
}

if ($checkedBlockers.Count -gt 0) {
    Write-Host "Status: blocked"
    foreach ($blocker in $checkedBlockers) {
        Write-Host "  $blocker"
    }
    exit 3
}

if (Test-AnyCheckedDecision -Lines $lines) {
    Write-Host "Status: unrecognized"
    Write-Host "Reason: a Phase 1 decision is checked, but it does not match the current gate labels."
    exit 5
}

Write-Host "Status: pending"
Write-Host "Reason: no Phase 1 decision checkbox is marked."
if ($AllowPending) {
    exit 0
}

exit 2
