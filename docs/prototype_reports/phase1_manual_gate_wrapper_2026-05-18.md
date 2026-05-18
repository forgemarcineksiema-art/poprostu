# Phase 1 Manual Gate Wrapper - 2026-05-18

## Milestone

Add one morning command that chains the automated Phase 1 gate, manual playtest helper, and manual report generator.

## Created

- `scripts/run_phase1_manual_gate.ps1`.

## What It Does

- Runs `scripts/verify_phase1.ps1` unless `-SkipVerify` is passed.
- Launches `scripts/run_phase1_playtest.ps1`.
- Generates the manual feel report with `scripts/new_phase1_playtest_report.ps1`.
- Prints the exact sequence before doing work.
- Supports `-PrintPlanOnly` for safe dry-run validation.

## Command

Normal morning gate:

```text
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\run_phase1_manual_gate.ps1
```

Dry-run:

```text
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\run_phase1_manual_gate.ps1 -PrintPlanOnly
```

Faster path when the automated gate already ran:

```text
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\run_phase1_manual_gate.ps1 -SkipVerify
```

## Validation

Command:

```text
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\run_phase1_manual_gate.ps1 -PrintPlanOnly
```

Evidence:

- Script exits with code `0`.
- Output prints the verify, playtest, and report paths.
- Output prints the three-step sequence.

## Limit

This wrapper still cannot judge feel. It only makes the owner-run gate hard to skip.
