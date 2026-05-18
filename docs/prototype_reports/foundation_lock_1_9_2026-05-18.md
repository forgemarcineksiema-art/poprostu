# Foundation Lock 1.9 - Camera Input Feel Polish - 2026-05-18

## Scope

This milestone tightens camera and input feel without adding content, missions, police, combat, animations, a vehicle migration, or new districts.

## Implementation

- Added explicit camera look helpers for both yaw and pitch so mouse and gamepad behavior are testable.
- Confirmed mouse look remains raw delta and does not scale with frame time.
- Confirmed gamepad look scales by elapsed time for frame-rate parity.
- Added an explicit recenter helper so on-foot and driving profiles can be tuned through tested delay and speed values.
- Added a short `TightSpaceRecovery` hold after camera collision clears so tight alleys do not flicker between free and recovery modes.
- Kept camera collision tied to world layers through the existing `PrototypeLayers.CameraCollisionMask`.

## Feel Contract

- Mouse: raw delta through `LookMouseDelta`.
- Gamepad: stick input scaled by degrees per second and `deltaTime`.
- On foot: recenter starts only after the profile delay.
- Driving: uses the shorter driving recenter profile already established in Foundation Lock 1.5.
- Tight space: collision switches to `TightSpaceRecovery`; recovery mode remains briefly active after the obstacle clears.

## Tests Added

- EditMode: mouse yaw delta is independent of frame time.
- EditMode: gamepad pitch at 30 FPS and 120 FPS produces the same rotation over the same simulated second.
- EditMode: camera recenter waits for idle delay before moving toward pivot yaw.
- EditMode: tight-space recovery stays active briefly after collision clears.

## Validation

Full gate:

~~~powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\verify_phase1.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\show_phase1_status.ps1
~~~

Result:

- Scene validator: passed.
- EditMode tests: total 27, passed 27, failed 0.
- PlayMode tests: total 23, passed 23, failed 0.
- Developer build: succeeded.
- Status coverage: `CoverageComplete: True`, `CoverageStatus: Coverage complete`.
