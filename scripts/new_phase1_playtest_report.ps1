param(
    [string]$MetricsPath = (Join-Path $env:USERPROFILE "AppData\LocalLow\DefaultCompany\My project\phase1_latest_run.txt"),
    [string]$OutputPath
)

$ErrorActionPreference = "Stop"

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path -Parent $scriptRoot

if ([string]::IsNullOrWhiteSpace($OutputPath)) {
    $stamp = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"
    $OutputPath = Join-Path $repoRoot "docs\prototype_reports\phase1_manual_playtest_$stamp.md"
}

$metricsText = if (Test-Path -LiteralPath $MetricsPath) {
    Get-Content -LiteralPath $MetricsPath -Raw
} else {
    "Metrics file not found: $MetricsPath"
}

$coverageComplete = $metricsText -match "CoverageComplete:\s*True"
$coverageLabel = if ($coverageComplete) { "complete" } else { "missing_or_unverified" }
$now = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

$report = @"
# Phase 1 Manual Feel Playtest - $now

## Automated Coverage

- Metrics path: $MetricsPath
- Coverage gate: $coverageLabel

~~~text
$metricsText
~~~

## Required Run

- [ ] Walk the tight street for several minutes.
- [ ] Enter the car.
- [ ] Drive the narrow route with at least one sharp turn.
- [ ] Pass the pressure checkpoint.
- [ ] Stop at the workshop.
- [ ] Exit the car.
- [ ] Use the workshop interaction.
- [ ] Return through Safe return.
- [ ] Total run lasts at least 10 minutes.

## Feel Gate

Mark each item after playing. Do not greenlight Phase 2 if any blocking item fails.

| Gate | Pass? | Notes |
| --- | --- | --- |
| 10 minutes of walking and driving is not tiring |  |  |
| Camera does not fight the player in the tight street |  |  |
| Player understands character/front/car orientation |  |  |
| Entering/exiting the car is smooth and not disorienting |  |  |
| Car has tension on the narrow route without random physics chaos |  |  |
| Workshop interaction is readable without frustration |  |  |
| Route and patrol presence create tension without a full chase |  |  |
| After the test, controller/camera/driving fixes are clear |  |  |

## Decision

- [ ] Phase 1 feel accepted for Phase 2.
- [ ] Phase 1 needs controller iteration.
- [ ] Phase 1 needs camera iteration.
- [ ] Phase 1 needs vehicle iteration.
- [ ] Phase 1 needs route/layout iteration.

## Top Fixes Before Phase 2

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
Write-Host "Phase 1 manual playtest report written:"
Write-Host $OutputPath

if (-not $coverageComplete) {
    Write-Warning "Coverage gate is not complete. Finish the route before using this report for a Phase 1 decision."
}
