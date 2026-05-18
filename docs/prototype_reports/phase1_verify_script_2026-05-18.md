# Phase 1 Verify Script Report - 2026-05-18

## Milestone

Add one repeatable command for the automated Phase 1 gate.

## Created

- `scripts/verify_phase1.ps1`.

## What It Runs

- scene validator,
- EditMode tests,
- PlayMode tests,
- Phase 1 developer build unless `-SkipBuild` is passed.
- scene builder only when `-RebuildScene` is passed.

## Why

Phase 1 now has several useful proofs, but they were spread across separate commands. This script makes the automated gate repeatable before the owner performs the manual feel playtest.

The script does not rebuild the scene by default because Unity regenerates scene file IDs when the procedural builder saves a new scene. That is useful when intentionally refreshing the authored scene, but it is noisy for normal verification.

## Command

```text
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\verify_phase1.ps1
```

Faster code/test loop without rebuilding the player:

```text
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\verify_phase1.ps1 -SkipBuild
```

Intentional scene regeneration:

```text
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\verify_phase1.ps1 -RebuildScene
```

## Evidence

- `Phase 1 automated verification passed.`
- Scene validator log contains `Phase 1 scene validation passed.`
- EditMode results: `testcasecount="4"`, `passed="4"`, `failed="0"`.
- PlayMode results: `testcasecount="5"`, `passed="5"`, `failed="0"`.
- Dev build log contains `Build Finished, Result: Success.`
- Dev build log contains `Phase 1 build result: Succeeded, size: 169306759, time: 00:00:17.0959986`.

## Limit

This is not the Phase 1 greenlight. It proves the automated side. The manual 10-minute feel playtest is still required before Phase 2.
