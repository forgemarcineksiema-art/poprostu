param(
    [switch]$RebuildScene,
    [switch]$SkipBuild,
    [string]$UnityExe = "C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Unity.exe"
)

$ErrorActionPreference = "Stop"

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path -Parent $scriptRoot
$logsDir = Join-Path $repoRoot "Logs"

if (-not (Test-Path -LiteralPath $UnityExe)) {
    throw "Unity executable not found: $UnityExe"
}

if (-not (Test-Path -LiteralPath $logsDir)) {
    New-Item -ItemType Directory -Path $logsDir | Out-Null
}

function Invoke-UnityStep {
    param(
        [string]$Name,
        [string]$Arguments,
        [string]$LogFile
    )

    Write-Host ""
    Write-Host "== $Name =="
    $fullArguments = "$Arguments -logFile `"$LogFile`""
    $process = Start-Process -FilePath $UnityExe -ArgumentList $fullArguments -Wait -PassThru -WindowStyle Hidden
    if ($process.ExitCode -ne 0) {
        throw "$Name failed with exit code $($process.ExitCode). See $LogFile"
    }
}

function Assert-TestResults {
    param(
        [string]$Name,
        [string]$ResultsPath
    )

    if (-not (Test-Path -LiteralPath $ResultsPath)) {
        throw "$Name did not write test results: $ResultsPath"
    }

    [xml]$results = Get-Content -LiteralPath $ResultsPath
    $run = $results."test-run"
    $failed = [int]$run.failed
    $passed = [int]$run.passed
    $total = [int]$run.total

    Write-Host "$Name results: total=$total passed=$passed failed=$failed"
    if ($failed -ne 0) {
        throw "$Name has failed tests. See $ResultsPath"
    }
}

$projectArg = "-projectPath `"$repoRoot`""
$builderLog = Join-Path $logsDir "phase1_verify_scene_builder.log"
$validatorLog = Join-Path $logsDir "phase1_verify_scene_validator.log"
$editModeLog = Join-Path $logsDir "phase1_verify_editmode_tests.log"
$editModeResults = Join-Path $logsDir "phase1_verify_editmode_results.xml"
$playModeLog = Join-Path $logsDir "phase1_verify_playmode_tests.log"
$playModeResults = Join-Path $logsDir "phase1_verify_playmode_results.xml"
$buildLog = Join-Path $logsDir "phase1_verify_dev_build.log"

Write-Host "Phase 1 verification"
Write-Host "Repo:  $repoRoot"
Write-Host "Unity: $UnityExe"

if ($RebuildScene) {
    Invoke-UnityStep `
        -Name "Build Phase 1 scene" `
        -Arguments "-batchmode -quit $projectArg -executeMethod ValleDePlata.Editor.PrototypeSceneBuilder.BuildPhase1Scene" `
        -LogFile $builderLog
} else {
    Write-Host ""
    Write-Host "== Build Phase 1 scene =="
    Write-Host "Skipped. Pass -RebuildScene when intentionally regenerating the authored test scene."
}

Invoke-UnityStep `
    -Name "Validate Phase 1 scene" `
    -Arguments "-batchmode -quit $projectArg -executeMethod ValleDePlata.Editor.PrototypeSceneValidator.ValidatePhase1Scene" `
    -LogFile $validatorLog

Invoke-UnityStep `
    -Name "Run EditMode tests" `
    -Arguments "-batchmode $projectArg -runTests -testPlatform EditMode -testResults `"$editModeResults`"" `
    -LogFile $editModeLog
Assert-TestResults -Name "EditMode" -ResultsPath $editModeResults

Invoke-UnityStep `
    -Name "Run PlayMode tests" `
    -Arguments "-batchmode $projectArg -runTests -testPlatform PlayMode -testResults `"$playModeResults`"" `
    -LogFile $playModeLog
Assert-TestResults -Name "PlayMode" -ResultsPath $playModeResults

if (-not $SkipBuild) {
    Invoke-UnityStep `
        -Name "Build Phase 1 developer player" `
        -Arguments "-batchmode -quit $projectArg -executeMethod ValleDePlata.Editor.PrototypeBuildPipeline.BuildPhase1Windows" `
        -LogFile $buildLog
}

Write-Host ""
Write-Host "Phase 1 automated verification passed."
Write-Host "Manual feel gate status is tracked by scripts\check_phase1_manual_decision.ps1."
