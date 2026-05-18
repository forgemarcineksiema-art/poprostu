# Phase 2R.3 - Playable Pressure Choice - 2026-05-18

## Scope

This milestone turns the Phase 2R pressure branch into an actual playable scene choice. It does not add police AI, combat, wanted levels, timers, animations, or new content outside the existing pressure/bribe beat.

## Implementation

- Added `PrototypePressureChoiceController`.
- `Pressure patrol marker` now has a pressure choice controller beside `PrototypePressureZone` and `PrototypePressureScenePlayback`.
- Entering the pressure zone after `PublicViolenceCommitted` applies `PressureCrackdownTriggered`.
- Entering the pressure zone after `BribeAccepted` stays contained and leaves the mission/objective on the safe branch.
- `PrototypePressureZone` still owns trigger contact and metrics; the choice controller owns world-state resolution.
- `PrototypeWorldState` remains the single source of truth for pressure, mission stage, and visible playback.

## Runtime Contract

| Player Route | World Result | Mission Result |
| --- | --- | --- |
| public violence -> bribe -> drive through pressure zone | `BribeAccepted` remains active | `PressureContained` |
| public violence -> drive into pressure zone without bribe | `PressureCrackdownTriggered` | `PressureFailure` |

## Tests Added

- EditMode: authored scene requires `PrototypePressureChoiceController`.
- EditMode: pressure choice only triggers crackdown when pressure is uncontained.
- PlayMode: bribe path lets the vehicle through the pressure zone without flipping to crackdown.
- PlayMode: uncontained zone entry triggers crackdown, mission failure, objective change, and scene playback.

## Validation

Red test:

- EditMode compilation failed before implementation because `PrototypePressureChoiceController` and `PrototypePressureChoiceResolution` did not exist.

Full gate:

~~~powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\verify_phase1.ps1 -RebuildScene -SkipBuild
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\verify_phase1.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\show_phase1_status.ps1
~~~

Result:

- Rebuild/skip-build gate: passed.
- EditMode tests: total 29, passed 29, failed 0.
- PlayMode tests: total 27, passed 27, failed 0.
- Developer build: succeeded.
- Status coverage: `CoverageComplete: True`, `CoverageStatus: Coverage complete`.
