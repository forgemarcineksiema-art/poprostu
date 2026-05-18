# Phase 2R.4 - Pressure Consequence Route Gate - 2026-05-18

## Scope

This milestone makes the Phase 2R pressure choice affect route completion. It does not add combat, police AI, wanted levels, timers, or a new mission system.

## Implementation

- Added `PrototypeRouteOutcome`.
- `PrototypeRouteProgress` can now resolve route progress against `PrototypeWorldState`.
- A contained pressure path can complete the normal route as `PressureContained`.
- A crackdown path blocks forward route progression and redirects the player to `Safe return`.
- `Safe return` after crackdown records `PressureFailureEscape` instead of normal route completion.
- `PrototypeRunMetrics` now records `RouteOutcome` in debug state and run summaries.

## Runtime Contract

| Route State | Allowed Result |
| --- | --- |
| `BribeAccepted` | Route can complete normally with `PressureContained` outcome |
| `PressureCrackdownTriggered` + forward checkpoint | Forward route is blocked |
| `PressureCrackdownTriggered` + `Safe return` | Escape is recorded as `PressureFailureEscape`, while `RouteCompleted` stays false |

## Tests Added

- EditMode: contained pressure can complete the normal route.
- EditMode: crackdown blocks forward route and allows safe-return escape.
- PlayMode: contained pressure route completes normal coverage.
- PlayMode: pressure failure blocks normal route and makes safe return an escape outcome.

## Validation

Red test:

- EditMode compilation failed before implementation because `PrototypeRouteOutcome`, `PrototypeRouteProgress.AttachWorldState`, `PrototypeRouteProgress.Outcome`, and `PrototypeRunMetrics.RouteOutcome` did not exist.

Targeted green:

- EditMode: total 31, passed 31, failed 0.
- PlayMode: total 29, passed 29, failed 0.

Full gate:

~~~powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\verify_phase1.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\show_phase1_status.ps1
~~~

Result:

- Scene validator: passed.
- EditMode tests: total 31, passed 31, failed 0.
- PlayMode tests: total 29, passed 29, failed 0.
- Developer build: succeeded.
