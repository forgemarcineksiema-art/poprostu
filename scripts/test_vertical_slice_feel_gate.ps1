param(
    [string]$ReportScript = (Join-Path (Split-Path -Parent $MyInvocation.MyCommand.Path) "new_vertical_slice_feel_gate_report.ps1"),
    [string]$RunnerScript = (Join-Path (Split-Path -Parent $MyInvocation.MyCommand.Path) "run_vertical_slice_feel_gate.ps1")
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path -LiteralPath $ReportScript)) {
    throw "Feel gate report script not found: $ReportScript"
}

if (-not (Test-Path -LiteralPath $RunnerScript)) {
    throw "Feel gate runner script not found: $RunnerScript"
}

$tempDir = Join-Path $env:TEMP ("vertical-slice-feel-gate-tests-" + [guid]::NewGuid().ToString("N"))
New-Item -ItemType Directory -Path $tempDir | Out-Null

function Assert-Contains {
    param(
        [string]$Name,
        [string]$Content,
        [string]$Expected
    )

    if (-not $Content.Contains($Expected)) {
        throw "$Name did not contain expected text: $Expected"
    }
}

try {
    $planOutput = & powershell -NoProfile -ExecutionPolicy Bypass -File $RunnerScript -PrintPlanOnly 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw "Runner print-plan exited with $LASTEXITCODE."
    }

    $planText = $planOutput -join "`n"
    Assert-Contains -Name "runner plan" -Content $planText -Expected "Vertical Slice Feel Gate 0.4"
    Assert-Contains -Name "runner plan" -Content $planText -Expected "Manual retest required after readability pass 0.3"
    Assert-Contains -Name "runner plan" -Content $planText -Expected "Generate 0.4 feel report."

    $completeMetrics = Join-Path $tempDir "complete_metrics.txt"
    Set-Content -LiteralPath $completeMetrics -Encoding UTF8 -Value @'
Phase 1 Feel Prototype Run
ElapsedSeconds: 612.4
VehicleEntries: 1
VehicleExits: 1
Interactions: 3
PressureEntries: 1
CompletedCheckpoints: 5
RouteCompleted: True
RouteOutcome: SafeReturnComplete
MaxSpeed: 8.8
LastInteraction: Pay Rios bribe
LastCheckpoint: Safe return
CoverageComplete: True
CoverageStatus: Coverage complete
ManualFeelGate: Required
'@

    $manualReport = Join-Path $tempDir "phase1_manual_playtest.md"
    Set-Content -LiteralPath $manualReport -Encoding UTF8 -Value @'
## Decision

- [x] Phase 1 feel accepted for Phase 2.
- [ ] Phase 1 needs controller iteration.
- [ ] Phase 1 needs camera iteration.
- [ ] Phase 1 needs vehicle iteration.
- [ ] Phase 1 needs route/layout iteration.
'@

    $completeReport = Join-Path $tempDir "vertical_slice_feel_gate_complete.md"
    & powershell -NoProfile -ExecutionPolicy Bypass -File $ReportScript -MetricsPath $completeMetrics -ManualReportPath $manualReport -OutputPath $completeReport
    if ($LASTEXITCODE -ne 0) {
        throw "Complete report generation exited with $LASTEXITCODE."
    }

    $completeText = Get-Content -LiteralPath $completeReport -Raw
    Assert-Contains -Name "complete report" -Content $completeText -Expected "# Vertical Slice Feel Gate 0.4"
    Assert-Contains -Name "complete report" -Content $completeText -Expected "Coverage gate: complete"
    Assert-Contains -Name "complete report" -Content $completeText -Expected "Manual decision source: $manualReport"
    Assert-Contains -Name "complete report" -Content $completeText -Expected "Manual retest required after readability pass 0.3"
    Assert-Contains -Name "complete report" -Content $completeText -Expected "Camera/Input Feel"
    Assert-Contains -Name "complete report" -Content $completeText -Expected "Vehicle Brake/Reverse"
    Assert-Contains -Name "complete report" -Content $completeText -Expected "Route Readability"
    Assert-Contains -Name "complete report" -Content $completeText -Expected "HUD/Prompt Clarity"
    Assert-Contains -Name "complete report" -Content $completeText -Expected "- [ ] 0.4 accepted: continue past the readability gate."

    $incompleteMetrics = Join-Path $tempDir "incomplete_metrics.txt"
    Set-Content -LiteralPath $incompleteMetrics -Encoding UTF8 -Value @'
Phase 1 Feel Prototype Run
CoverageComplete: False
CoverageStatus: Missing: enter car, drive, pressure, interaction, safe return
'@

    $incompleteReport = Join-Path $tempDir "vertical_slice_feel_gate_incomplete.md"
    & powershell -NoProfile -ExecutionPolicy Bypass -File $ReportScript -MetricsPath $incompleteMetrics -ManualReportPath $manualReport -OutputPath $incompleteReport
    if ($LASTEXITCODE -ne 0) {
        throw "Incomplete report generation exited with $LASTEXITCODE."
    }

    $incompleteText = Get-Content -LiteralPath $incompleteReport -Raw
    Assert-Contains -Name "incomplete report" -Content $incompleteText -Expected "Coverage gate: missing_or_unverified"
    Assert-Contains -Name "incomplete report" -Content $incompleteText -Expected "Do not accept 0.4 until coverage is complete."

    Write-Host "Vertical slice feel gate report tests passed."
} finally {
    Remove-Item -LiteralPath $tempDir -Recurse -Force
}
