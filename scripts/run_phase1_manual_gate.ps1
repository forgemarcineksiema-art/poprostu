param(
    [switch]$SkipVerify,
    [switch]$SkipBuild,
    [switch]$PrintPlanOnly,
    [string]$UnityExe = "C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Unity.exe"
)

$ErrorActionPreference = "Stop"

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path -Parent $scriptRoot
$verifyScript = Join-Path $scriptRoot "verify_phase1.ps1"
$playtestScript = Join-Path $scriptRoot "run_phase1_playtest.ps1"
$reportScript = Join-Path $scriptRoot "new_phase1_playtest_report.ps1"

Write-Host "Phase 1 manual gate"
Write-Host "Repo:        $repoRoot"
Write-Host "Verify:      $verifyScript"
Write-Host "Playtest:    $playtestScript"
Write-Host "Report:      $reportScript"
Write-Host ""
Write-Host "Sequence:"

if ($SkipVerify) {
    Write-Host "  1. Skip automated verification."
} elseif ($SkipBuild) {
    Write-Host "  1. Run automated verification without rebuilding the player."
} else {
    Write-Host "  1. Run full automated verification including developer build."
}

if ($SkipVerify -and -not $SkipBuild) {
    Write-Host "  2. Build and launch Phase 1 playtest."
} else {
    Write-Host "  2. Launch Phase 1 playtest."
}

Write-Host "  3. Generate manual feel report."
Write-Host ""

if ($PrintPlanOnly) {
    exit 0
}

if (-not $SkipVerify) {
    $verifyArgs = @("-UnityExe", $UnityExe)
    if ($SkipBuild) {
        $verifyArgs += "-SkipBuild"
    }

    & $verifyScript @verifyArgs
}

$playtestArgs = @("-UnityExe", $UnityExe)
if ($SkipVerify -and -not $SkipBuild) {
    $playtestArgs += "-BuildFirst"
}

& $playtestScript @playtestArgs
& $reportScript

Write-Host ""
Write-Host "Phase 1 manual gate finished. Review the generated report before deciding on Phase 2."
