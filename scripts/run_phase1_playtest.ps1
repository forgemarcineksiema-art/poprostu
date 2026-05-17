param(
    [switch]$BuildFirst,
    [switch]$PrintPathsOnly,
    [string]$UnityExe = "C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Unity.exe"
)

$ErrorActionPreference = "Stop"

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path -Parent $scriptRoot
$buildExe = Join-Path $repoRoot "Builds\Phase1\ValleDePlataPhase1.exe"
$buildLog = Join-Path $repoRoot "Logs\phase1_manual_playtest_build.log"
$metricsPath = Join-Path $env:USERPROFILE "AppData\LocalLow\DefaultCompany\My project\phase1_latest_run.txt"

Write-Host "Phase 1 playtest helper"
Write-Host "Repo:        $repoRoot"
Write-Host "Build exe:   $buildExe"
Write-Host "Metrics:     $metricsPath"

if ($PrintPathsOnly) {
    exit 0
}

if ($BuildFirst) {
    if (-not (Test-Path -LiteralPath $UnityExe)) {
        throw "Unity executable not found: $UnityExe"
    }

    $arguments = '-batchmode -quit -projectPath "' + $repoRoot + '" -executeMethod ValleDePlata.Editor.PrototypeBuildPipeline.BuildPhase1Windows -logFile "' + $buildLog + '"'
    $build = Start-Process -FilePath $UnityExe -ArgumentList $arguments -Wait -PassThru -WindowStyle Hidden
    if ($build.ExitCode -ne 0) {
        throw "Phase 1 build failed with exit code $($build.ExitCode). See $buildLog"
    }
}

if (-not (Test-Path -LiteralPath $buildExe)) {
    throw "Phase 1 build not found. Run this script with -BuildFirst or build with PrototypeBuildPipeline.BuildPhase1Windows."
}

Write-Host ""
Write-Host "Manual feel checklist:"
Write-Host "  1. Walk the tight street."
Write-Host "  2. Enter the car."
Write-Host "  3. Drive the route and hit the pressure checkpoint."
Write-Host "  4. Exit at the workshop and interact."
Write-Host "  5. Return through Safe return."
Write-Host "  6. Quit the build and inspect metrics below."
Write-Host ""

$run = Start-Process -FilePath $buildExe -WorkingDirectory (Split-Path -Parent $buildExe) -Wait -PassThru

Write-Host ""
Write-Host "Build exited with code $($run.ExitCode)."

if (Test-Path -LiteralPath $metricsPath) {
    Write-Host ""
    Write-Host "Latest run metrics:"
    Get-Content -LiteralPath $metricsPath
} else {
    Write-Warning "Metrics file was not found yet: $metricsPath"
}

exit $run.ExitCode
