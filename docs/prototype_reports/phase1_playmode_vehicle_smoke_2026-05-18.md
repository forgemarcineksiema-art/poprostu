# Phase 1 PlayMode Vehicle Smoke Report - 2026-05-18

## Milestone

Dodać pierwszy fizyczny smoke test auta w scenie Fazy 1.

## Co powstało

- `PrototypeVehicleController.ApplyDriveInput(Vector2 move, bool handbrake)` jako kontrolowany drive seam.
- PlayMode test `PrototypeVehicleMovesUnderScriptedDriveInput`.
- PlayMode asmdef `ValleDePlata.Prototype.PlayModeTests`.

## Co test sprawdza

- ładuje `Phase1_FeelPrototype`,
- znajduje gracza i auto,
- sadza gracza w aucie,
- przez 90 kroków fizyki podaje kontrolowany input do przodu,
- wymaga, żeby auto przejechało ponad 2.5 metra.

## Walidacja

Uruchomiono:

```text
Unity.exe -batchmode -projectPath . -runTests -testPlatform PlayMode -testResults Logs\phase1_playmode_results.xml -logFile Logs\phase1_playmode_tests.log
```

Wynik:

- PlayMode: `testcasecount="1"`, `passed="1"`, `failed="0"`,
- test `PrototypeVehicleMovesUnderScriptedDriveInput` przeszedł,
- po zmianie odpalono też EditMode suite: `testcasecount="4"`, `passed="4"`, `failed="0"`.

## Handoff

Jeśli przejdzie, następny najmniejszy krok to route-aware playmode test: przejazd przez checkpoint presji albo wykrycie, że checkpoint route spine reaguje na auto.
