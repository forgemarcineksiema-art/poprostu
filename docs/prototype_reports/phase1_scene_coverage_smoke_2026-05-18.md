# Phase 1 Scene Coverage Smoke Report - 2026-05-18

## Milestone

Add a PlayMode smoke test proving the Phase 1 scene can produce complete coverage through scene beats.

## Created

- `SceneBeatsCanProduceCompleteCoverage` PlayMode test.

## What It Proves

The test loads `Phase1_FeelPrototype` and uses the actual prototype components to cover the required run beats:

- enter the vehicle through `PrototypePlayerController.EnterVehicle`,
- generate real vehicle speed through `PrototypeVehicleController.ApplyDriveInput`,
- move through the authored route checkpoints,
- trigger the pressure zone during route traversal,
- exit the vehicle through `PrototypeVehicleController.ExitDriver`,
- use the workshop interaction through `PrototypeInteractable.Interact`,
- assert `PrototypeRunMetrics.HasRouteCoverage`.

## What It Does Not Prove

It does not prove the feel gate. Camera comfort, steering feel, disorientation, frustration, and 10-minute fatigue still require the owner playtest.

## Validation

Command:

```text
"C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Unity.exe" -batchmode -projectPath . -runTests -testPlatform PlayMode -testResults Logs\phase1_scene_coverage_playmode_results.xml -logFile Logs\phase1_scene_coverage_playmode_tests.log
```

Evidence:

- PlayMode includes `SceneBeatsCanProduceCompleteCoverage`.
- PlayMode reports `testcasecount="5"`, `passed="5"`, `failed="0"`.

## Handoff

The automated side now proves the route can close coverage through scene-authored objects. The next gate remains the manual 10-minute feel playtest via `scripts/run_phase1_playtest.ps1`.
