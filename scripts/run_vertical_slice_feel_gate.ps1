param(
    [switch]$SkipVerify,
    [switch]$SkipBuild,
    [switch]$ReportOnly,
    [switch]$PrintPlanOnly,
    [string]$UnityExe = "C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Unity.exe"
)

$ErrorActionPreference = "Stop"

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path -Parent $scriptRoot
$verifyScript = Join-Path $scriptRoot "verify_phase1.ps1"
$playtestScript = Join-Path $scriptRoot "run_phase1_playtest.ps1"
$reportScript = Join-Path $scriptRoot "new_vertical_slice_feel_gate_report.ps1"

function Write-FeelChecklist {
    Write-Host "Manual retest required after readability pass 0.3"
    Write-Host ""
    Write-Host "Route loop:"
    Write-Host "  1. Start on foot and walk with camera-relative movement."
    Write-Host "  2. Rotate camera by about 90 degrees, then keep moving forward."
    Write-Host "  3. Enter the vehicle without losing orientation."
    Write-Host "  4. Drive the narrow route and test brake-before-reverse."
    Write-Host "  5. Trigger the pressure beat and make the pressure choice."
    Write-Host "  6. Pass the consequence gate toward Rios/roadblock."
    Write-Host "  7. Reach El Respiro, exit, interact, and return through Safe return."
    Write-Host ""
    Write-Host "Feel gates:"
    Write-Host "  - Camera/Input Feel"
    Write-Host "  - On-Foot Movement"
    Write-Host "  - Enter/Exit Orientation"
    Write-Host "  - Vehicle Brake/Reverse"
    Write-Host "  - Route Readability"
    Write-Host "  - HUD/Prompt Clarity"
}

Write-Host "Vertical Slice Feel Gate 0.4"
Write-Host "Repo:     $repoRoot"
Write-Host "Verify:   $verifyScript"
Write-Host "Playtest: $playtestScript"
Write-Host "Report:   $reportScript"
Write-Host ""
Write-Host "Sequence:"

if ($SkipVerify) {
    Write-Host "  1. Skip automated verification."
} elseif ($SkipBuild) {
    Write-Host "  1. Run automated verification without rebuilding the player."
} else {
    Write-Host "  1. Run full automated verification including developer build."
}

if ($ReportOnly) {
    Write-Host "  2. Skip launching the playtest and use the current metrics file."
} else {
    Write-Host "  2. Launch Phase 1 playtest."
}

Write-Host "  3. Generate 0.4 feel report."
Write-Host ""
Write-FeelChecklist
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

if (-not $ReportOnly) {
    $playtestArgs = @("-UnityExe", $UnityExe)
    if ($SkipVerify -and -not $SkipBuild) {
        $playtestArgs += "-BuildFirst"
    }

    & $playtestScript @playtestArgs
}

& $reportScript

Write-Host ""
Write-Host "Vertical Slice Feel Gate 0.4 report generated. Fill it before continuing past the readability gate."
