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
$decisionScript = Join-Path $scriptRoot "check_phase1_manual_decision.ps1"

function Write-ManualChecklist {
    Write-Host "Manual playtest route:"
    Write-Host "  1. Walk the tight street for several minutes."
    Write-Host "  2. Enter the car."
    Write-Host "  3. Drive the narrow route with at least one sharp turn."
    Write-Host "  4. Pass the pressure checkpoint."
    Write-Host "  5. Stop at the workshop."
    Write-Host "  6. Exit the car."
    Write-Host "  7. Use the workshop interaction."
    Write-Host "  8. Return through Safe return."
    Write-Host "  9. Keep the run active for at least 10 minutes."
    Write-Host ""
    Write-Host "Feel gates to judge before Phase 2:"
    Write-Host "  - 10 minutes of walking and driving is not tiring."
    Write-Host "  - Camera does not fight the player in the tight street."
    Write-Host "  - Player understands character/front/car orientation."
    Write-Host "  - Entering/exiting the car is smooth and not disorienting."
    Write-Host "  - Car has tension on the narrow route without random physics chaos."
    Write-Host "  - Workshop interaction is readable without frustration."
    Write-Host "  - Route and patrol presence create tension without a full chase."
    Write-Host "  - Controller/camera/driving fixes are clear after the test."
}

Write-Host "Phase 1 manual gate"
Write-Host "Repo:        $repoRoot"
Write-Host "Verify:      $verifyScript"
Write-Host "Playtest:    $playtestScript"
Write-Host "Report:      $reportScript"
Write-Host "Decision:    $decisionScript"
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
Write-Host "  4. Check whether the report decision allows Phase 2."
Write-Host ""
Write-ManualChecklist
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
Write-Host "Phase 1 manual gate finished. Fill the generated report, then run:"
Write-Host "  powershell -NoProfile -ExecutionPolicy Bypass -File scripts\check_phase1_manual_decision.ps1"
