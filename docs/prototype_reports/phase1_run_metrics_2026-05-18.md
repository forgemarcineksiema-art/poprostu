# Phase 1 Run Metrics Report - 2026-05-18

## Milestone

Add lightweight runtime metrics for the Phase 1 feel prototype.

## Created

- `PrototypeRunMetrics` runtime component.
- `Phase 1 Run Metrics` scene object.
- HUD metrics line for live playtest feedback.
- Runtime report writer to `Application.persistentDataPath/phase1_latest_run.txt` on application quit.

## What It Tracks

- elapsed run time,
- vehicle entries and exits,
- interactable use,
- pressure-zone entries,
- completed route checkpoints,
- route completion,
- max vehicle speed,
- last interaction,
- last checkpoint.

## Why

Phase 1 cannot be greenlit by automated tests alone, because the real gate is feel. These metrics make the required manual playtest less subjective: the owner can confirm that a run actually covered walking, entering the car, driving, pressure, interaction, and safe return.

## Validation

Commands:

```text
"C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Unity.exe" -batchmode -quit -projectPath . -executeMethod ValleDePlata.Editor.PrototypeSceneBuilder.BuildPhase1Scene -logFile Logs\phase1_metrics_scene_builder.log
"C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Unity.exe" -batchmode -quit -projectPath . -executeMethod ValleDePlata.Editor.PrototypeSceneValidator.ValidatePhase1Scene -logFile Logs\phase1_metrics_scene_validator.log
"C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Unity.exe" -batchmode -projectPath . -runTests -testPlatform EditMode -testResults Logs\phase1_metrics_editmode_results.xml -logFile Logs\phase1_metrics_editmode_tests.log
"C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Unity.exe" -batchmode -projectPath . -runTests -testPlatform PlayMode -testResults Logs\phase1_metrics_playmode_results.xml -logFile Logs\phase1_metrics_playmode_tests.log
"C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Unity.exe" -batchmode -quit -projectPath . -executeMethod ValleDePlata.Editor.PrototypeBuildPipeline.BuildPhase1Windows -logFile Logs\phase1_metrics_dev_build.log
```

Evidence:

- Scene builder exited with code `0`.
- Scene validator log contains `Phase 1 scene validation passed.`
- EditMode results: `testcasecount="4"`, `passed="4"`, `failed="0"`.
- PlayMode results: `testcasecount="3"`, `passed="3"`, `failed="0"`.
- PlayMode covers metric recording for interaction, vehicle entry, max speed, checkpoint completion, and route completion.
- Dev build log contains `Build Finished, Result: Success.`
- Dev build log contains `Phase 1 build result: Succeeded, size: 169305487, time: 00:01:11.9177894`.

## Handoff

Run the Phase 1 build manually and finish the 10-minute feel checklist. After quitting the build, inspect `phase1_latest_run.txt` under Unity's persistent data path for the run metrics. Do not move to Phase 2 until the manual feel gate says the controller, camera, driving, enter/exit, pressure route, and interaction are not fighting the player.
