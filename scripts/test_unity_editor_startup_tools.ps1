param(
    [string]$ProjectRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
)

$ErrorActionPreference = 'Stop'

function Assert-Contains {
    param(
        [string]$Text,
        [string]$Expected,
        [string]$Message
    )

    if ($Text -notlike "*$Expected*") {
        throw "$Message Expected to find '$Expected'."
    }
}

function Assert-Path {
    param([string]$Path)

    if (-not (Test-Path -LiteralPath $Path)) {
        throw "Missing expected path: $Path"
    }
}

$diagnoseScript = Join-Path $ProjectRoot 'scripts\diagnose_unity_editor_startup.ps1'
$openScript = Join-Path $ProjectRoot 'scripts\open_unity_low_impact.ps1'

Assert-Path $diagnoseScript
Assert-Path $openScript

$manifestPath = Join-Path $ProjectRoot 'Packages\manifest.json'
Assert-Path $manifestPath
$manifest = Get-Content -Raw -LiteralPath $manifestPath | ConvertFrom-Json
$optionalPackages = @(
    'com.unity.ai.navigation',
    'com.unity.collab-proxy',
    'com.unity.ide.rider',
    'com.unity.multiplayer.center',
    'com.unity.timeline',
    'com.unity.visualscripting'
)

foreach ($packageName in $optionalPackages) {
    if ($manifest.dependencies.PSObject.Properties.Name -contains $packageName) {
        throw "Optional editor/startup package is still enabled in manifest: $packageName"
    }
}

$openOutput = & $openScript -ProjectPath $ProjectRoot -PrintOnly 2>&1 | Out-String
Assert-Contains $openOutput 'Priority: BelowNormal' 'Low-impact opener should show the priority it will apply.'
Assert-Contains $openOutput '-projectPath' 'Low-impact opener should launch Unity with an explicit project path.'
Assert-Contains $openOutput $ProjectRoot 'Low-impact opener should target this project root.'

$reportPath = Join-Path $env:TEMP ('unity-startup-diagnostics-{0}.md' -f ([Guid]::NewGuid().ToString('N')))
try {
    $diagnoseOutput = & $diagnoseScript -ProjectPath $ProjectRoot -OutputPath $reportPath -SkipLargeDirectoryScan 2>&1 | Out-String
    Assert-Path $reportPath
    $report = Get-Content -Raw -LiteralPath $reportPath
    Assert-Contains $report '# Unity Editor Startup Diagnostics' 'Diagnostics report should have a stable heading.'
    Assert-Contains $report '## Packages' 'Diagnostics report should include package information.'
    Assert-Contains $report '## Editor Log Signals' 'Diagnostics report should include editor log signals.'
    Assert-Contains $report '## Low-Impact Open Command' 'Diagnostics report should include the safer open command.'
    Assert-Contains $diagnoseOutput 'Diagnostics written' 'Diagnostics script should print where it wrote the report.'
}
finally {
    if (Test-Path -LiteralPath $reportPath) {
        Remove-Item -LiteralPath $reportPath -Force
    }
}

Write-Host 'Unity editor startup tool tests passed.'
