# Phase 1 Manual Report Generator - 2026-05-18

## Milestone

Add a report generator for the owner-run Phase 1 manual feel playtest.

## Created

- `scripts/new_phase1_playtest_report.ps1`.

## What It Does

- Reads the latest Phase 1 metrics file.
- Detects whether `CoverageComplete: True` is present.
- Creates a timestamped manual playtest report under `docs/prototype_reports`.
- Copies the metrics into the report.
- Adds the required run checklist from the Phase 1 prototype contract.
- Adds the feel gate table for camera, controller, driving, enter/exit, route tension, and interaction readability.

## Command

After running the build through `scripts/run_phase1_playtest.ps1`:

```text
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\new_phase1_playtest_report.ps1
```

Optional explicit paths:

```text
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\new_phase1_playtest_report.ps1 -MetricsPath path\to\phase1_latest_run.txt -OutputPath docs\prototype_reports\phase1_manual_playtest_test.md
```

## Validation

Command:

```text
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\new_phase1_playtest_report.ps1 -MetricsPath Logs\phase1_sample_metrics.txt -OutputPath Logs\phase1_sample_manual_report.md
```

Evidence:

- Script exits with code `0`.
- Generated report contains `Coverage gate: complete`.
- Generated report contains `10 minutes of walking and driving is not tiring`.

## Limit

The generated report is not a substitute for the manual playtest. It only prevents the morning review from becoming a vague memory of how the prototype felt.
