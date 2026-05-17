# Phase 1 PlayMode Route Smoke Report - 2026-05-18

## Milestone

Sprawdzić w PlayMode, czy checkpoint route spine reaguje na auto w scenie.

## Co powstało

- Drugi PlayMode test: `RouteCheckpointsCompleteUnderVehicleTriggerContact`.

## Co test sprawdza

- ładuje `Phase1_FeelPrototype`,
- znajduje gracza, auto, route progress i 5 checkpointów,
- sadza gracza w aucie,
- przesuwa Rigidbody auta przez checkpointy w kolejności,
- wymaga `route.IsComplete == true`,
- wymaga `PrototypeDebugState.Route == "Complete"`,
- wymaga ostatniego checkpointu `Safe return`.

## Walidacja

Uruchomiono:

```text
Unity.exe -batchmode -projectPath . -runTests -testPlatform PlayMode -testResults Logs\phase1_playmode_results.xml -logFile Logs\phase1_playmode_tests.log
```

Wynik:

- PlayMode: `testcasecount="2"`, `passed="2"`, `failed="0"`,
- `PrototypeVehicleMovesUnderScriptedDriveInput` przeszedł,
- `RouteCheckpointsCompleteUnderVehicleTriggerContact` przeszedł,
- log kończy się `Test run completed. Exiting with code 0 (Ok).`

## Handoff

Jeśli przejdzie, Faza 1 ma już automatyczne dowody:

- scena istnieje i ma wymagane obiekty,
- wejście/wyjście z auta jest odzyskiwalne,
- auto rusza fizycznie,
- route checkpoint spine może dojść do końca.

Następny krok powinien być jakościowy: raport z ręcznego feel playtestu albo parametryzacja kamery/jazdy po realnym przejściu trasy.
