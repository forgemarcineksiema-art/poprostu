# Phase 1 Manual Decision Checker - 2026-05-18

## Milestone

Add a small gate script that reads the manual Phase 1 playtest report and turns the markdown decision checkboxes into a clear production status.

## Created

- `scripts/check_phase1_manual_decision.ps1`.

## Updated

- `scripts/run_phase1_manual_gate.ps1` now prints the decision checker path and the follow-up command.

## Statuses

- `accepted`: Phase 2 may start.
- `blocked`: Phase 1 needs controller, camera, vehicle, or route/layout iteration.
- `pending`: no manual decision is checked yet.
- `conflicting`: the report marks Phase 2 accepted and also marks one or more blockers.
- `unrecognized`: a Phase 1 decision is checked, but it does not match the current gate labels.

## Commands

Latest report:

```text
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\check_phase1_manual_decision.ps1
```

Specific report:

```text
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\check_phase1_manual_decision.ps1 -ReportPath docs\prototype_reports\phase1_manual_playtest_example.md
```

Status-only pending check:

```text
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\check_phase1_manual_decision.ps1 -AllowPending
```

## Validation

Commands:

```text
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\check_phase1_manual_decision.ps1 -AllowPending
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\run_phase1_manual_gate.ps1 -PrintPlanOnly
```

Temporary report fixtures were also used to test accepted, blocked, and conflicting decisions.

Evidence:

- Decision checker exits with code `0` under `-AllowPending`.
- Decision checker reports `pending` when no manual report has an accepted or blocked decision.
- Accepted fixture exits with code `0` and reports `Status: accepted`.
- Blocked fixture exits with code `3` and reports the checked blocker.
- Conflicting fixture exits with code `4` and reports accepted-plus-blocker conflict.
- Manual gate dry-run prints the decision checker path.
- Manual gate dry-run prints the follow-up decision step.

## Limit

This script does not judge feel. It only enforces that Phase 2 depends on an explicit manual report decision.
