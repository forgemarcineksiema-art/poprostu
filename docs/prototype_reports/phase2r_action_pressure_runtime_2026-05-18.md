# Phase 2R - Action Pressure Runtime Proof - 2026-05-18

## Scope

This is the Phase 2 restart on top of Foundation Lock 1.5-1.9. It does not add full combat, police AI, animations, new districts, or a new mission phase. It turns the existing action/pressure microtest into a clearer runtime beat with objective feedback and a small branch.

## Implementation

- `PrototypeWorldState` now has a pressure failure event: `PressureCrackdownTriggered`.
- `PrototypeMissionSpine` now derives Phase 2 pressure stages from the world state:
  - `ActionPressure` after public violence.
  - `PressureContained` after the pressure is contained through the bribe branch.
  - `PressureFailure` after pressure escalates into a crackdown.
- `PrototypeObjectiveMarker` continues to read the mission spine; no second objective truth source was added.
- Public violence now produces a visible objective: contain street pressure before patrol pressure locks the route.
- The success branch keeps pressure low through `BribeAccepted`.
- The failure branch pushes `StatePressure` to `High` through `PressureCrackdownTriggered`.

## Runtime Contract

| Beat | World State | Objective |
| --- | --- | --- |
| Public violence | `Fear = High`, `PeopleLove = Low`, `StatePressure = Medium`, `RuleStyle = ShowOfForce` | `Objective: contain street pressure before patrol locks the route` |
| Pressure contained | `LastEvent = BribeAccepted`, `StatePressure = Low` | `Objective: pressure contained, continue to El Respiro` |
| Pressure failure | `LastEvent = PressureCrackdownTriggered`, `StatePressure = High` | `Objective changed: escape the patrol pressure` |

## Tests Added

- EditMode: `Phase2RPressureBeatPublishesObjectiveBranchesFromWorldState`.
- PlayMode: public violence updates mission stage and objective marker.
- PlayMode: `Phase2RPressureBeatHasRuntimeSuccessAndFailureBranches`.

## Validation

Full gate:

~~~powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\verify_phase1.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\show_phase1_status.ps1
~~~

Result:

- Scene validator: passed.
- EditMode tests: total 28, passed 28, failed 0.
- PlayMode tests: total 24, passed 24, failed 0.
- Developer build: succeeded.
- Status coverage: `CoverageComplete: True`, `CoverageStatus: Coverage complete`.
