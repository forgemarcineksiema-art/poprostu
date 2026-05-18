# Foundation Lock 1.7 - Vehicle Physics Decision Spike - 2026-05-18

## Scope

This milestone starts the vehicle-physics decision work without migrating runtime driving. The live player car remains the arcade Rigidbody baseline. WheelCollider is treated as an isolated A/B candidate and must earn promotion through comparable probe metrics.

## Implementation

- Added `PrototypeVehicleComparison` as a deterministic probe runner for arcade Rigidbody and WheelCollider candidates.
- Added shared vehicle metrics: distance, max speed, brake speed drop, reverse distance, yaw, handbrake yaw, collision recovery, upright state, completion, viability, and score.
- Added a decision function with three explicit outcomes: keep arcade, promote WheelCollider, or defer for a raycast-vehicle spike.
- Kept runtime migration at zero in this pass.
- Added test output reporting so future tuning changes expose the actual vehicle metrics instead of only a pass/fail result.

## Automated A/B Result

Single PlayMode probe on the same route fixture:

| Candidate | Distance | Max speed | Brake drop | Reverse | Yaw | Handbrake yaw | Recovery | Score |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Arcade Rigidbody baseline | 12.66 m | 4.66 m/s | 4.25 m/s | 0.00 m | 81.21 deg | 42.02 deg | 1.33 m | 87.17 |
| WheelCollider spike | 6.21 m | 5.26 m/s | 2.66 m/s | 0.05 m | 2.39 deg | 0.00 deg | 3.97 m | 47.63 |

## Decision

Decision: `KeepArcadeRigidbodyBaseline`.

Reason: WheelCollider runs to completion, but this first setup does not produce usable turning or handbrake response. It is not better enough to justify a runtime migration. Next vehicle work should either tune the WheelCollider fixture until it becomes a fair contender or add a custom raycast-vehicle spike before committing to a full migration.

## Tests Added

- EditMode: vehicle comparison keeps arcade when WheelCollider metrics are not viable and includes the decision in the report.
- PlayMode: arcade and WheelCollider candidates run the same A/B route, emit comparable metrics, and keep arcade as the runtime decision.

## Validation

Target full gate:

~~~powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\verify_phase1.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\show_phase1_status.ps1
~~~

The final gate is run before commit and push.
