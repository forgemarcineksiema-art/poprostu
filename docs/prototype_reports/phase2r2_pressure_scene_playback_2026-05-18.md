# Phase 2R.2 - Pressure Scene Playback - 2026-05-18

## Scope

This milestone turns the Phase 2R pressure beat into a small visible scene playback. It does not add full police AI, combat, animations, a new district, or a new mission phase.

## Implementation

- Added `PrototypePressureScenePlayback`.
- The playback listens to `PrototypeWorldState` and drives existing scene markers:
  - `PublicViolenceCommitted` moves the police pressure marker closer to the route.
  - `BribeAccepted` moves the roadblock aside and disables its collider.
  - `PressureCrackdownTriggered` moves the patrol into the route pressure point and closes the roadblock again.
- The scene builder wires the playback onto `Pressure patrol marker`.
- The scene validator now requires the playback component in the authored scene.
- The existing objective/world-state flow remains the source of truth; this pass only adds scene movement for the already-authored consequences.

## Runtime Contract

| Event | Scene Playback |
| --- | --- |
| `PublicViolenceCommitted` | patrol marker shifts from witness pressure into the lane approach |
| `BribeAccepted` | bribe roadblock opens and stops blocking |
| `PressureCrackdownTriggered` | roadblock closes and patrol pressure locks onto the route |

## Tests Added

- EditMode: authored scene must include `PrototypePressureScenePlayback`.
- PlayMode: `Phase2RPressureScenePlaybackMovesPatrolAndOpensRoadblock`.

## Validation

Full gate:

~~~powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\verify_phase1.ps1 -RebuildScene -SkipBuild
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\verify_phase1.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\show_phase1_status.ps1
~~~

Result:

- Scene builder: passed.
- Scene validator: passed.
- EditMode tests: total 28, passed 28, failed 0.
- PlayMode tests: total 25, passed 25, failed 0.
- Developer build: succeeded.
- Status coverage: `CoverageComplete: True`, `CoverageStatus: Coverage complete`.
