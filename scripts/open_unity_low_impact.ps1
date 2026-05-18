param(
    [string]$ProjectPath = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path,
    [string]$UnityExe = 'C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Unity.exe',
    [string]$LogPath,
    [ValidateSet('Idle', 'BelowNormal', 'Normal')]
    [string]$Priority = 'BelowNormal',
    [switch]$PrintOnly
)

$ErrorActionPreference = 'Stop'

if (-not $LogPath -or $LogPath.Trim().Length -eq 0) {
    $stamp = Get-Date -Format 'yyyyMMdd_HHmmss'
    $LogPath = Join-Path $ProjectPath ("Logs\unity_low_impact_open_{0}.log" -f $stamp)
}

$arguments = @(
    '-projectPath', $ProjectPath,
    '-logFile', $LogPath
)

$argumentText = ($arguments | ForEach-Object {
    if ($_ -match '\s') { '"' + $_ + '"' } else { $_ }
}) -join ' '

Write-Output "Unity exe: $UnityExe"
Write-Output "Project: $ProjectPath"
Write-Output "Log: $LogPath"
Write-Output "Priority: $Priority"
Write-Output "Arguments: $argumentText"

if ($PrintOnly) {
    return
}

if (-not (Test-Path -LiteralPath $UnityExe)) {
    throw "Unity executable was not found: $UnityExe"
}

if (-not (Test-Path -LiteralPath $ProjectPath)) {
    throw "Project path was not found: $ProjectPath"
}

$logDirectory = Split-Path -Parent $LogPath
if ($logDirectory -and -not (Test-Path -LiteralPath $logDirectory)) {
    New-Item -ItemType Directory -Path $logDirectory | Out-Null
}

$process = Start-Process -FilePath $UnityExe -ArgumentList $arguments -PassThru -WindowStyle Normal
Start-Sleep -Seconds 4

try {
    $priorityValue = [Enum]::Parse([System.Diagnostics.ProcessPriorityClass], $Priority)
    $process.PriorityClass = $priorityValue
    Write-Output "Unity process $($process.Id) priority set to $Priority."
}
catch {
    Write-Warning "Unity started, but priority could not be changed: $($_.Exception.Message)"
}
