# Phase 1 Route Checkpoints Report - 2026-05-18

## Milestone

Dodać mierzalny przebieg Fazy 1: pieszo -> auto -> presja -> warsztat -> powrót.

## Co powstało

- `PrototypeRouteProgress` jako mały runtime spine trasy.
- `PrototypeRouteCheckpoint` jako trigger kolejnych kroków.
- Debug HUD pokazuje `Route` i `Checkpoint`.
- Builder sceny dodaje 5 checkpointów w `Phase1_FeelPrototype.unity`.
- Validator i EditMode testy sprawdzają obecność checkpointów.

## Checkpointy

1. `Start on foot`
2. `Enter vehicle lane`
3. `Patrol pressure turn`
4. `Workshop interaction stop`
5. `Safe return`

## Cel projektowy

Greybox nie ma być już tylko placem do jeżdżenia. Ma wymuszać podstawowy loop z Prototype Contract:

- start pieszo,
- wejście do auta,
- ciasna trasa,
- presja patrolu,
- interakcja przy warsztacie,
- powrót do bezpiecznego miejsca.

## Walidacja

Uruchomiono po zmianie:

```text
Unity.exe -batchmode -quit -projectPath . -executeMethod ValleDePlata.Editor.PrototypeSceneBuilder.BuildPhase1Scene -logFile Logs\phase1_scene_builder.log
Unity.exe -batchmode -quit -projectPath . -executeMethod ValleDePlata.Editor.PrototypeSceneValidator.ValidatePhase1Scene -logFile Logs\phase1_scene_validator.log
Unity.exe -batchmode -projectPath . -runTests -testPlatform EditMode -testResults Logs\phase1_editmode_results.xml -logFile Logs\phase1_editmode_tests.log
```

Wynik:

- builder zakończony `return code 0`,
- validator: `Phase 1 scene validation passed`,
- EditMode: `testcasecount="4"`, `passed="4"`, `failed="0"`,
- scena zawiera `Phase 1 Route Progress` i checkpointy `Route checkpoint 0` do `Route checkpoint 4`.

## Handoff

Jeśli walidacja przejdzie, kolejny najmniejszy milestone powinien być bardziej "feelowy":

- prosty playmode smoke test lub deterministic harness dla przejazdu autem,
- albo ręczny playtest i raport parametrów kamery/jazdy.
