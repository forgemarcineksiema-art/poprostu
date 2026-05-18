param(
    [string]$DecisionScript = (Join-Path (Split-Path -Parent $MyInvocation.MyCommand.Path) "check_phase1_manual_decision.ps1")
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path -LiteralPath $DecisionScript)) {
    throw "Decision checker not found: $DecisionScript"
}

$tempDir = Join-Path $env:TEMP ("phase1-decision-tests-" + [guid]::NewGuid().ToString("N"))
New-Item -ItemType Directory -Path $tempDir | Out-Null

function New-DecisionFixture {
    param(
        [string]$Name,
        [string]$Body
    )

    $path = Join-Path $tempDir $Name
    Set-Content -LiteralPath $path -Encoding UTF8 -Value $Body
    return $path
}

function Invoke-DecisionCase {
    param(
        [string]$Name,
        [string[]]$Arguments,
        [int]$ExpectedExitCode,
        [string]$ExpectedText
    )

    Write-Host "Case: $Name"
    $output = & powershell -NoProfile -ExecutionPolicy Bypass -File $DecisionScript @Arguments 2>&1
    $exitCode = $LASTEXITCODE
    $output | ForEach-Object { Write-Host "  $_" }

    if ($exitCode -ne $ExpectedExitCode) {
        throw "Case '$Name' expected exit $ExpectedExitCode but got $exitCode."
    }

    if (-not ($output -match [regex]::Escape($ExpectedText))) {
        throw "Case '$Name' did not print expected text: $ExpectedText"
    }
}

try {
    $pending = New-DecisionFixture -Name "pending.md" -Body @'
## Decision

- [ ] Phase 1 feel accepted for Phase 2.
- [ ] Phase 1 needs controller iteration.
- [ ] Phase 1 needs camera iteration.
- [ ] Phase 1 needs vehicle iteration.
- [ ] Phase 1 needs route/layout iteration.
'@

    $accepted = New-DecisionFixture -Name "accepted.md" -Body @'
## Decision

- [x] Phase 1 feel accepted for Phase 2.
- [ ] Phase 1 needs controller iteration.
- [ ] Phase 1 needs camera iteration.
- [ ] Phase 1 needs vehicle iteration.
- [ ] Phase 1 needs route/layout iteration.
'@

    $blocked = New-DecisionFixture -Name "blocked.md" -Body @'
## Decision

- [ ] Phase 1 feel accepted for Phase 2.
- [ ] Phase 1 needs controller iteration.
- [X] Phase 1 needs camera iteration.
- [ ] Phase 1 needs vehicle iteration.
- [ ] Phase 1 needs route/layout iteration.
'@

    $conflicting = New-DecisionFixture -Name "conflicting.md" -Body @'
## Decision

- [x] Phase 1 feel accepted for Phase 2.
- [ ] Phase 1 needs controller iteration.
- [ ] Phase 1 needs camera iteration.
- [x] Phase 1 needs vehicle iteration.
- [ ] Phase 1 needs route/layout iteration.
'@

    $unrecognized = New-DecisionFixture -Name "unrecognized.md" -Body @'
## Decision

- [x] Phase 1 needs something renamed.
'@

    Invoke-DecisionCase -Name "pending strict" -Arguments @("-ReportPath", $pending) -ExpectedExitCode 2 -ExpectedText "Status: pending"
    Invoke-DecisionCase -Name "pending allowed" -Arguments @("-ReportPath", $pending, "-AllowPending") -ExpectedExitCode 0 -ExpectedText "Status: pending"
    Invoke-DecisionCase -Name "accepted" -Arguments @("-ReportPath", $accepted) -ExpectedExitCode 0 -ExpectedText "Status: accepted"
    Invoke-DecisionCase -Name "blocked" -Arguments @("-ReportPath", $blocked) -ExpectedExitCode 3 -ExpectedText "Status: blocked"
    Invoke-DecisionCase -Name "conflicting" -Arguments @("-ReportPath", $conflicting) -ExpectedExitCode 4 -ExpectedText "Status: conflicting"
    Invoke-DecisionCase -Name "unrecognized" -Arguments @("-ReportPath", $unrecognized) -ExpectedExitCode 5 -ExpectedText "Status: unrecognized"
    Invoke-DecisionCase -Name "blocked status only" -Arguments @("-ReportPath", $blocked, "-StatusOnly") -ExpectedExitCode 0 -ExpectedText "Status: blocked"
    Invoke-DecisionCase -Name "conflicting status only" -Arguments @("-ReportPath", $conflicting, "-StatusOnly") -ExpectedExitCode 0 -ExpectedText "Status: conflicting"

    Write-Host "Phase 1 manual decision checker tests passed."
} finally {
    Remove-Item -LiteralPath $tempDir -Recurse -Force
}
