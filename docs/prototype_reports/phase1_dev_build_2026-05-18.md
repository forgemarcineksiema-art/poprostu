# Phase 1 Dev Build Report - 2026-05-18

## Milestone

Add a repeatable developer build for the Phase 1 feel prototype.

## Created

- `PrototypeBuildPipeline.BuildPhase1Windows`.
- Build target: Windows x64.
- Build scene: `Assets/Scenes/Phase1_FeelPrototype.unity`.
- Local output: `Builds/Phase1/ValleDePlataPhase1.exe`.

## Why

Phase 1 needs a real feel playtest. The developer build lets the owner run the prototype in the morning without manually assembling or exporting the scene from the editor.

## Validation

Command:

```text
"C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Unity.exe" -batchmode -quit -projectPath . -executeMethod ValleDePlata.Editor.PrototypeBuildPipeline.BuildPhase1Windows -logFile Logs\phase1_dev_build.log
```

Evidence:

- Unity returned exit code `0`.
- `Logs\phase1_dev_build.log` contains `Build Finished, Result: Success.`
- `Logs\phase1_dev_build.log` contains `Phase 1 build result: Succeeded, size: 169298223, time: 00:06:02.1377218`.
- `Builds\Phase1\ValleDePlataPhase1.exe` exists with the generated data folder.

## Notes

- Build output stays local and ignored by git.
- Unity touched several existing project/render settings during the build preprocess. Those files were not staged for this milestone.

## Handoff

The next strongest validation is a manual 10-minute feel playtest:

- walk and drive for 10 minutes,
- test the camera in the tight street,
- enter and exit the car repeatedly,
- drive through the pressure checkpoint,
- stop and interact at the workshop,
- return through checkpoint `Safe return`.
