# Phase 1 Status Checker - 2026-05-18

## Milestone

Add a read-only status checker for the Phase 1 gate.

## Created

- `scripts/show_phase1_status.ps1`.

## What It Shows

- current branch,
- `HEAD...origin/<branch>` ahead/behind count,
- latest scene validator marker,
- latest EditMode result summary,
- latest PlayMode result summary,
- latest developer build summary,
- latest manual metrics coverage if present,
- next manual gate command,
- current working-tree content diff summary.

## Command

```text
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\show_phase1_status.ps1
```

## Validation

Command:

```text
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\show_phase1_status.ps1
```

Evidence:

- Script exits with code `0`.
- Output includes `Branch: main`.
- Output includes `HEAD...origin/main: 0 0`.
- Output includes `EditMode: total=4 passed=4 failed=0`.
- Output includes `PlayMode: total=5 passed=5 failed=0`.
- Output includes `Dev build: success`.
- Output includes the manual gate command.

## Limit

This is diagnostic only. It does not run Unity, does not update the build, and does not judge feel.
