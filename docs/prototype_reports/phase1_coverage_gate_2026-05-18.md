# Phase 1 Coverage Gate Report - 2026-05-18

## Milestone

Add a route-coverage gate to the Phase 1 run metrics.

## Created

- `PrototypeRunMetrics.HasRouteCoverage`.
- `PrototypeRunMetrics.CoverageStatus`.
- `PrototypeRunMetrics.BuildMissingCoverageSummary()`.
- HUD line now shows either `coverage OK` or the missing playtest beats.
- Run report now writes `CoverageComplete`, `CoverageStatus`, and `ManualFeelGate: Required`.

## Coverage Beats

Coverage is complete only after the run includes:

- entering the car,
- exiting the car,
- driving faster than `1.0 m/s`,
- entering the pressure zone,
- using the interaction,
- completing the route back through `Safe return`.

## Important Limit

This does not greenlight Phase 1. It only proves the manual run touched the required beats. The feel gate still needs the owner to judge camera, controller, vehicle handling, orientation, and frustration over a 10-minute run.

## Validation

Commands:

```text
"C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Unity.exe" -batchmode -projectPath . -runTests -testPlatform PlayMode -testResults Logs\phase1_coverage_playmode_results.xml -logFile Logs\phase1_coverage_playmode_tests.log
```

Evidence:

- PlayMode includes `MetricsCoverageGateRequiresAllPhase1Beats`.
- PlayMode reports `testcasecount="4"`, `passed="4"`, `failed="0"`.
- Dev build log contains `Build Finished, Result: Success.`
- Dev build log contains `Phase 1 build result: Succeeded, size: 169306759, time: 00:00:46.2050843`.

## Handoff

During the next manual playtest, do not accept a feel note unless the HUD or saved report shows coverage complete. If coverage is incomplete, rerun the same build path before judging Phase 1.
