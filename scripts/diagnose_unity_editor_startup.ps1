param(
    [string]$ProjectPath = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path,
    [string]$UnityExe = 'C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Unity.exe',
    [string]$EditorLogPath = (Join-Path $env:LOCALAPPDATA 'Unity\Editor\Editor.log'),
    [string]$UpmLogPath = (Join-Path $env:LOCALAPPDATA 'Unity\Editor\upm.log'),
    [string]$OutputPath,
    [switch]$SkipLargeDirectoryScan
)

$ErrorActionPreference = 'Stop'

function Format-Mb {
    param([Nullable[double]]$Bytes)

    if ($null -eq $Bytes) {
        return 'n/a'
    }

    return ('{0:N1} MB' -f ($Bytes / 1MB))
}

function Get-DirectoryFootprint {
    param(
        [string]$Path,
        [switch]$SkipScan
    )

    if (-not (Test-Path -LiteralPath $Path)) {
        return [pscustomobject]@{
            Path = $Path
            Exists = $false
            FileCount = 0
            Bytes = $null
            ScanSeconds = 0
        }
    }

    if ($SkipScan) {
        return [pscustomobject]@{
            Path = $Path
            Exists = $true
            FileCount = $null
            Bytes = $null
            ScanSeconds = 0
        }
    }

    $timer = [Diagnostics.Stopwatch]::StartNew()
    $files = Get-ChildItem -LiteralPath $Path -Recurse -File -Force -ErrorAction SilentlyContinue
    $stats = $files | Measure-Object Length -Sum
    $timer.Stop()

    return [pscustomobject]@{
        Path = $Path
        Exists = $true
        FileCount = $stats.Count
        Bytes = $stats.Sum
        ScanSeconds = $timer.Elapsed.TotalSeconds
    }
}

function Get-TopLevelPackages {
    param([string]$ManifestPath)

    if (-not (Test-Path -LiteralPath $ManifestPath)) {
        return @()
    }

    $json = Get-Content -Raw -LiteralPath $ManifestPath | ConvertFrom-Json
    return @($json.dependencies.PSObject.Properties | ForEach-Object {
        [pscustomobject]@{
            Name = $_.Name
            Version = [string]$_.Value
        }
    })
}

function Get-ProjectInfoEvents {
    param([string]$LogPath)

    if (-not (Test-Path -LiteralPath $LogPath)) {
        return @()
    }

    $events = @()
    foreach ($line in (Get-Content -LiteralPath $LogPath -Tail 4000 -ErrorAction SilentlyContinue)) {
        if ($line -like '##utp:*') {
            $payload = $line.Substring(6)
            try {
                $event = $payload | ConvertFrom-Json
                if ($event.type -eq 'ProjectInfo') {
                    $events += $event
                }
            }
            catch {
                # Keep the report resilient when Unity writes partial telemetry lines.
            }
        }
    }

    return $events
}

function Get-LogSignals {
    param([string]$LogPath)

    if (-not (Test-Path -LiteralPath $LogPath)) {
        return @("Missing log: $LogPath")
    }

    $patterns = 'Package Manager|AssetDatabase|Refresh|Import|Shader|Bee|ScriptCompilation|Domain Reload|Safe Mode|GUID|Licensing|error|warning|crash|hang'
    return @(Select-String -Path $LogPath -Pattern $patterns -CaseSensitive:$false | Select-Object -Last 120 | ForEach-Object { $_.Line })
}

if (-not $OutputPath -or $OutputPath.Trim().Length -eq 0) {
    $stamp = Get-Date -Format 'yyyyMMdd_HHmmss'
    $OutputPath = Join-Path $ProjectPath ("docs\prototype_reports\unity_editor_startup_diagnostics_{0}.md" -f $stamp)
}

$manifestPath = Join-Path $ProjectPath 'Packages\manifest.json'
$lockPath = Join-Path $ProjectPath 'Packages\packages-lock.json'
$packages = Get-TopLevelPackages -ManifestPath $manifestPath
$optionalPackageNames = @(
    'com.unity.ai.navigation',
    'com.unity.collab-proxy',
    'com.unity.ide.rider',
    'com.unity.multiplayer.center',
    'com.unity.timeline',
    'com.unity.visualscripting'
)
$enabledOptionalPackages = @($packages | Where-Object { $optionalPackageNames -contains $_.Name })

$footprintPaths = @(
    'Assets',
    'Packages',
    'ProjectSettings',
    'UserSettings',
    'Logs',
    'Library',
    'Library\PackageCache',
    'Library\Bee',
    'Library\ShaderCache',
    'Library\Artifacts',
    'Library\SourceAssetDB'
)

$footprints = @()
foreach ($relativePath in $footprintPaths) {
    $footprints += Get-DirectoryFootprint -Path (Join-Path $ProjectPath $relativePath) -SkipScan:$SkipLargeDirectoryScan
}

$projectInfoEvents = Get-ProjectInfoEvents -LogPath $EditorLogPath
$lastProjectInfo = $projectInfoEvents | Select-Object -Last 1
$editorSignals = Get-LogSignals -LogPath $EditorLogPath
$upmSignals = if (Test-Path -LiteralPath $UpmLogPath) { @(Get-Content -LiteralPath $UpmLogPath -Tail 80) } else { @("Missing log: $UpmLogPath") }
$unityProcesses = @(Get-Process -Name Unity -ErrorAction SilentlyContinue)

$lines = New-Object System.Collections.Generic.List[string]
$lines.Add('# Unity Editor Startup Diagnostics')
$lines.Add('')
$lines.Add(('- Generated: {0:yyyy-MM-dd HH:mm:ss zzz}' -f (Get-Date)))
$lines.Add(('- Project: `{0}`' -f $ProjectPath))
$lines.Add(('- Unity exe: `{0}`' -f $UnityExe))
$lines.Add("- Unity exe exists: $((Test-Path -LiteralPath $UnityExe))")
$lines.Add(('- Editor log: `{0}`' -f $EditorLogPath))
$lines.Add(('- UPM log: `{0}`' -f $UpmLogPath))
$lines.Add("- Skip large directory scan: $SkipLargeDirectoryScan")
$lines.Add('')

$lines.Add('## Startup Summary')
if ($lastProjectInfo) {
    $lines.Add(('- Last project load: {0:N2}s' -f [double]$lastProjectInfo.projectLoad))
    $lines.Add(('- Last project init: {0:N2}s' -f [double]$lastProjectInfo.projectInit))
    $lines.Add(('- Package Manager init: {0:N2}s' -f [double]$lastProjectInfo.packageManagerInit))
    $lines.Add(('- Asset Database refresh: {0:N2}s' -f [double]$lastProjectInfo.assetDatabaseRefresh))
    $lines.Add(('- Scene opening: {0:N2}s' -f [double]$lastProjectInfo.sceneOpening))
}
else {
    $lines.Add('- No Unity ProjectInfo event found in the last part of Editor.log.')
}
$lines.Add('')

$lines.Add('## Packages')
$lines.Add(('- Top-level manifest packages: {0}' -f $packages.Count))
$lines.Add(('- Packages lock exists: {0}' -f (Test-Path -LiteralPath $lockPath)))
if ($enabledOptionalPackages.Count -gt 0) {
    $lines.Add('- Optional startup-heavy packages still enabled:')
    foreach ($package in $enabledOptionalPackages) {
        $lines.Add(("  - `{0}` {1}" -f $package.Name, $package.Version))
    }
}
else {
    $lines.Add('- Optional startup-heavy packages are not enabled in the manifest.')
}
$lines.Add('')
$lines.Add('| Package | Version |')
$lines.Add('| --- | --- |')
foreach ($package in ($packages | Sort-Object Name)) {
    $lines.Add(("| `{0}` | `{1}` |" -f $package.Name, $package.Version))
}
$lines.Add('')

$lines.Add('## Project Footprint')
$lines.Add('| Path | Exists | Files | Size | Scan |')
$lines.Add('| --- | --- | ---: | ---: | ---: |')
foreach ($item in $footprints) {
    $files = if ($null -eq $item.FileCount) { 'skipped' } else { '{0:N0}' -f $item.FileCount }
    $size = Format-Mb -Bytes $item.Bytes
    $scan = '{0:N2}s' -f $item.ScanSeconds
    $relative = $item.Path.Replace($ProjectPath, '').TrimStart('\')
    if ($relative.Length -eq 0) { $relative = '.' }
    $lines.Add(("| `{0}` | {1} | {2} | {3} | {4} |" -f $relative, $item.Exists, $files, $size, $scan))
}
$lines.Add('')

$lines.Add('## Unity Processes')
if ($unityProcesses.Count -eq 0) {
    $lines.Add('- No Unity.exe process is currently running.')
}
else {
    foreach ($process in $unityProcesses) {
        $memoryMb = '{0:N1} MB' -f ($process.WorkingSet64 / 1MB)
        $cpu = if ($null -ne $process.CPU) { '{0:N1}s' -f $process.CPU } else { 'n/a' }
        $lines.Add(("- PID {0}: CPU {1}, memory {2}" -f $process.Id, $cpu, $memoryMb))
    }
}
$lines.Add('')

$lines.Add('## Editor Log Signals')
if ($editorSignals.Count -eq 0) {
    $lines.Add('- No matching editor log signals found.')
}
else {
    foreach ($signal in $editorSignals) {
        $cleanSignal = $signal.TrimEnd().Replace('|', '\|')
        $lines.Add(('- {0}' -f $cleanSignal))
    }
}
$lines.Add('')

$lines.Add('## Package Manager Log Tail')
foreach ($signal in $upmSignals) {
    $cleanSignal = $signal.TrimEnd().Replace('|', '\|')
    $lines.Add(('- {0}' -f $cleanSignal))
}
$lines.Add('')

$lines.Add('## Low-Impact Open Command')
$lines.Add('```powershell')
$lines.Add('powershell -NoProfile -ExecutionPolicy Bypass -File scripts\open_unity_low_impact.ps1')
$lines.Add('```')
$lines.Add('')

$lines.Add('## Reading')
$lines.Add('- If `Library\PackageCache` is much larger than `Assets`, the first fix is package/cache hygiene, not gameplay optimization.')
$lines.Add('- If Editor.log shows long AssetDatabase refresh, Bee script compilation, or ShaderGraph work, expect startup freezes even when play mode FPS is fine.')
$lines.Add('- Use the low-impact opener while diagnosing so Unity starts at BelowNormal priority and writes a project-local log.')
$lines.Add('- Do not repeatedly delete the whole `Library` folder unless the log shows stale or corrupted package/cache evidence; it forces an expensive full reimport.')

$outputDirectory = Split-Path -Parent $OutputPath
if ($outputDirectory -and -not (Test-Path -LiteralPath $outputDirectory)) {
    New-Item -ItemType Directory -Path $outputDirectory | Out-Null
}

Set-Content -LiteralPath $OutputPath -Value ($lines -join [Environment]::NewLine) -Encoding UTF8
Write-Output "Diagnostics written: $OutputPath"
