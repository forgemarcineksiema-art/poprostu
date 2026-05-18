param(
    [string]$MetricsPath = (Join-Path $env:USERPROFILE "AppData\LocalLow\DefaultCompany\My project\phase1_latest_run.txt"),
    [string]$ManualReportPath,
    [string]$OutputPath
)

$ErrorActionPreference = "Stop"

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path -Parent $scriptRoot
$reportsDir = Join-Path $repoRoot "docs\prototype_reports"

function Find-LatestManualReport {
    if (-not (Test-Path -LiteralPath $reportsDir)) {
        return $null
    }

    $latest = Get-ChildItem -LiteralPath $reportsDir -Filter "phase1_manual_playtest_*.md" |
        Sort-Object LastWriteTime -Descending |
        Select-Object -First 1

    if ($latest) {
        return $latest.FullName
    }

    return $null
}

function Read-ManualDecisionStatus {
    param([string]$Path)

    if ([string]::IsNullOrWhiteSpace($Path) -or -not (Test-Path -LiteralPath $Path)) {
        return "missing"
    }

    $text = Get-Content -LiteralPath $Path -Raw
    $accepted = $text -match "- \[[xX]\] Phase 1 feel accepted for Phase 2\."
    $blocked = $text -match "- \[[xX]\] Phase 1 needs (controller|camera|vehicle|route/layout) iteration\."

    if ($accepted -and $blocked) {
        return "conflicting"
    }

    if ($accepted) {
        return "accepted"
    }

    if ($blocked) {
        return "blocked"
    }

    return "pending"
}

function Read-MetricValue {
    param(
        [string]$Text,
        [string]$Name,
        [string]$Fallback
    )

    $match = [regex]::Match($Text, "(?m)^${Name}:\s*(.+?)\s*$")
    if ($match.Success) {
        return $match.Groups[1].Value.Trim()
    }

    return $Fallback
}

if ([string]::IsNullOrWhiteSpace($ManualReportPath)) {
    $ManualReportPath = Find-LatestManualReport
}

if ([string]::IsNullOrWhiteSpace($OutputPath)) {
    $stamp = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"
    $OutputPath = Join-Path $reportsDir "vertical_slice_feel_gate_0_4_$stamp.md"
}

$metricsText = if (Test-Path -LiteralPath $MetricsPath) {
    Get-Content -LiteralPath $MetricsPath -Raw
} else {
    "Metrics file not found: $MetricsPath"
}

$coverageComplete = $metricsText -match "CoverageComplete:\s*True"
$coverageLabel = if ($coverageComplete) { "complete" } else { "missing_or_unverified" }
$averageFps = Read-MetricValue -Text $metricsText -Name "AverageFps" -Fallback "not recorded"
$worstFrameMs = Read-MetricValue -Text $metricsText -Name "WorstFrameMs" -Fallback "not recorded"
$performanceStatus = Read-MetricValue -Text $metricsText -Name "PerformanceStatus" -Fallback "not recorded"
$manualDecisionStatus = Read-ManualDecisionStatus -Path $ManualReportPath
$manualReportLabel = if ([string]::IsNullOrWhiteSpace($ManualReportPath)) { "not found" } else { $ManualReportPath }
$now = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

$coverageStop = if ($coverageComplete) {
    "Coverage is complete, so 0.4 can focus on feel, readability, and disorientation risk."
} else {
    "Do not accept 0.4 until coverage is complete."
}

$report = @"
# Vertical Slice Feel Gate 0.4 - $now

## Purpose

Manual retest required after readability pass 0.3.

This gate answers one question: after the HUD, scene dressing, and readable prop grouping passes, does the current route actually play cleanly enough to continue past the readability gate?

It does not add combat, police AI, new missions, new districts, animation systems, or extra pressure states.

## Evidence Inputs

- Metrics path: $MetricsPath
- Coverage gate: $coverageLabel
- Average FPS: $averageFps
- Worst frame: ${worstFrameMs}ms
- Performance status: $performanceStatus
- Manual decision source: $manualReportLabel
- Manual decision status before 0.4: $manualDecisionStatus

~~~text
$metricsText
~~~

## Required Route Loop

- [ ] Start on foot and walk with camera-relative movement.
- [ ] Rotate the camera through at least one 90 degree turn, then keep moving forward.
- [ ] Enter the vehicle without losing orientation.
- [ ] Drive through the narrow route and brake before reversing.
- [ ] Trigger the pressure beat.
- [ ] Make the playable pressure choice.
- [ ] Pass the route gate consequence.
- [ ] Reach El Respiro.
- [ ] Exit the vehicle at the workshop/front area.
- [ ] Interact with the readable objective.
- [ ] Return through Safe return.

## Feel Risk Matrix

| Area | Question | Pass? | Notes |
| --- | --- | --- | --- |
| Camera/Input Feel | Camera yaw, recenter, tight-space recovery, mouse look, and gamepad look keep the player oriented. |  |  |
| On-Foot Movement | W follows camera forward, A/D do not create a spiral, acceleration/deceleration feel legible. |  |  |
| Enter/Exit Orientation | Entering and exiting the car keeps yaw and target context stable without a jump. |  |  |
| Vehicle Brake/Reverse | S brakes before reverse, steering stays predictable, handbrake does not feel like random physics. |  |  |
| Route Readability | Barrio, Rios, roadblock, El Respiro, pressure, and safe return are spatially distinct while playing. |  |  |
| HUD/Prompt Clarity | Objective, pressure state, and interaction prompt make the next action obvious without debug-reading. |  |  |

## Decision

- [ ] 0.4 accepted: continue past the readability gate.
- [ ] Needs camera/input feel fix pack.
- [ ] Needs on-foot movement fix pack.
- [ ] Needs vehicle feel fix pack.
- [ ] Needs route/readability fix pack.
- [ ] Needs HUD/interaction prompt fix pack.

## Stop Rules

- $coverageStop
- Do not add new mission content if any Feel Risk Matrix row is blocked.
- Do not add full police AI, combat, or new districts as a response to a feel blocker.
- If 0.4 is blocked, the next milestone must be a targeted 0.5 fix pack, not Phase 3 content.

## Top Fixes Before Continuing

1.
2.
3.

## Notes

-
"@

$outputDirectory = Split-Path -Parent $OutputPath
if (-not [string]::IsNullOrWhiteSpace($outputDirectory) -and -not (Test-Path -LiteralPath $outputDirectory)) {
    New-Item -ItemType Directory -Path $outputDirectory | Out-Null
}

Set-Content -LiteralPath $OutputPath -Value $report -Encoding UTF8
Write-Host "Vertical slice feel gate report written:"
Write-Host $OutputPath

if (-not $coverageComplete) {
    Write-Warning "Coverage gate is not complete. Finish the route before accepting 0.4."
}
