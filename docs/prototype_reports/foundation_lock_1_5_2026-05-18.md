# Foundation Lock 1.5 - 2026-05-18

## Scope

This milestone starts the Foundation Lock pass before expanding the game beyond the current Phase 1-5 proof skeletons.

## Decisions Locked

- Camera target: GTA V-like control camera with cinematic-biased framing.
- Player movement target: custom kinematic character motor, not long-term `CharacterController` and not dynamic Rigidbody locomotion.
- Vehicle target: keep arcade Rigidbody as baseline and add an isolated WheelCollider spike path.
- Unity foundation: custom GameObject layers and masks are now required for camera, interaction, exit checks, triggers, and validation.

## Implementation

- Added prototype layers and runtime masks for world, player, vehicle, interactables, route triggers, sensors, camera-ignore objects, and NPCs.
- Rebuilt the Phase 1 scene through the builder so critical objects are assigned to their foundation layers.
- Added `PrototypeCharacterMotor` and moved player locomotion delegation out of `PrototypePlayerController`.
- Expanded camera modes to `OnFootFree`, `OnFootInteractionFocus`, `DrivingChase`, and `TightSpaceRecovery`.
- Added interaction targeting with visible/unblocked preference and single active prompt selection.
- Added a minimal objective marker that reads `PrototypeMissionSpine`.
- Added a lightweight `PrototypeSliceDefinition` asset for Phase 1 route data.
- Added `PrototypeWheelVehicleController` as the isolated WheelCollider spike entry point.

## Validation

Command:

~~~powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\verify_phase1.ps1
~~~

Result:

- Scene validator passed.
- EditMode tests: total 22, passed 22, failed 0.
- PlayMode tests: total 19, passed 19, failed 0.
- Developer build passed.

Status command:

~~~powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\show_phase1_status.ps1
~~~

Result:

- Automated gate passed.
- Manual decision remains accepted.
- Manual coverage file still reflects the last stored manual run, not a new manual playthrough.

## Next

Continue Foundation Lock with deeper feel tests: camera transition blends, motor slope/step cases in the authored scene, and a measured A/B route for arcade Rigidbody versus the WheelCollider spike.
