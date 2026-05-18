# Foundation Lock 1.8 - Loop Truth Coverage Lock - 2026-05-18

## Scope

This milestone locks the Phase 1 loop coverage artifact. It does not add combat, police, new mission content, new districts, animations, or a vehicle migration.

The problem was not missing scene systems. The scene already had tests for the full loop, but batch PlayMode runs could leave `phase1_latest_run.txt` with a later incomplete short run. That made `show_phase1_status.ps1` report missing coverage after a clean automated verify.

## Implementation

- `PrototypeRunMetrics.WriteReport()` now preserves an existing complete coverage report when the current run is incomplete.
- Incomplete follow-up runs are written to a diagnostic sidecar: `phase1_latest_run.txt.incomplete`.
- `SceneBeatsCanProduceCompleteCoverage` now writes the complete loop report during PlayMode verification.
- Added a regression test proving an incomplete run cannot overwrite a complete coverage report.

## Loop Truth Result

After full verification, `phase1_latest_run.txt` reports:

~~~text
VehicleEntries: 1
VehicleExits: 1
Interactions: 1
PressureEntries: 1
CompletedCheckpoints: 5
RouteCompleted: True
CoverageComplete: True
CoverageStatus: Coverage complete
~~~

If a later short test run quits with partial metrics, it is preserved as diagnostics without corrupting the accepted coverage artifact.

## Tests Added

- PlayMode: `MetricsReportPreservesCompleteCoverageWhenCurrentRunIsIncomplete`.
- PlayMode: full scene coverage test now writes and verifies `CoverageComplete: True` in the runtime metrics file.

## Validation

~~~powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\verify_phase1.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File scripts\show_phase1_status.ps1
~~~

Result:

- Scene validator: passed.
- EditMode tests: total 24, passed 24, failed 0.
- PlayMode tests: total 23, passed 23, failed 0.
- Developer build: succeeded.
- Status coverage: `CoverageComplete: True`, `CoverageStatus: Coverage complete`.
