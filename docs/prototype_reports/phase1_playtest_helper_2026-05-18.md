# Phase 1 Playtest Helper Report - 2026-05-18

## Milestone

Add a small local helper for the manual Phase 1 feel playtest.

## Created

- `scripts/run_phase1_playtest.ps1`.

## What It Does

- Prints the Phase 1 build path.
- Prints the expected metrics report path.
- Optionally rebuilds the Phase 1 developer build with `-BuildFirst`.
- Launches `Builds\Phase1\ValleDePlataPhase1.exe`.
- After the build closes, prints `phase1_latest_run.txt` if Unity wrote it.

## Why

The current hard gate is manual feel validation. This helper does not pretend to automate taste or controller feel; it just makes the required playtest easier to run and harder to forget.

## Validation

Command:

```text
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\run_phase1_playtest.ps1 -PrintPathsOnly
```

Expected result:

- exits with code `0`,
- prints repo path,
- prints `Builds\Phase1\ValleDePlataPhase1.exe`,
- prints the expected `phase1_latest_run.txt` path under `AppData\LocalLow\DefaultCompany\My project`.

## Handoff

Use this when doing the owner playtest:

```text
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\run_phase1_playtest.ps1
```

Use this if the local build is missing or stale:

```text
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\run_phase1_playtest.ps1 -BuildFirst
```
