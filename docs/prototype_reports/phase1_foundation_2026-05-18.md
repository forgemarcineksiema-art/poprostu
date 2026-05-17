# Phase 1 Foundation Report - 2026-05-18

## Milestone

Pierwszy kodowany dowód Fazy 1: greybox foundation dla TPP/kamera/jazda.

## Co powstało

- Runtime prototype scripts w `Assets/Scripts/Prototype`.
- Editorowy builder sceny w `Assets/Editor/PrototypeSceneBuilder.cs`.
- Editorowy validator sceny w `Assets/Editor/PrototypeSceneValidator.cs`.
- Scena `Assets/Scenes/Phase1_FeelPrototype.unity`.
- Greybox materiały w `Assets/PrototypeMaterials`.

## Zakres grywalny

Scena zawiera:

- start pieszo jako Pablo Valera prototype controller,
- kamerę TPP z prostym collision sphere cast,
- jedno auto prototype sedan,
- wejście/wyjście z auta przez interakcję,
- ciasną trasę z zakrętem i przeszkodą,
- marker presji patrolu,
- warsztatowy obiekt interakcji,
- debug HUD pokazujący tryb, prędkość, fokus, interakcję i presję.

## Co celowo nie powstało

- pełna misja "Pierwszy Front",
- broń,
- ekonomia,
- brudna kasa,
- pełny World State,
- policja jako system,
- final art,
- nowa dzielnica poza greyboxowym korytarzem testowym.

## Walidacja

Uruchomiono Unity 6000.4.7f1 w batchmode dla buildera sceny:

```text
Unity.exe -batchmode -quit -projectPath . -executeMethod ValleDePlata.Editor.PrototypeSceneBuilder.BuildPhase1Scene -logFile Logs\phase1_scene_builder.log
```

Wynik:

- skrypty kompilują się po naprawie błędu Input System `DeltaControl.wasUpdatedThisFrame`,
- `Assets/Scenes/Phase1_FeelPrototype.unity` została wygenerowana,
- log kończy się `Exiting batchmode successfully now!` i `return code 0`.

Uruchomiono też validator sceny:

```text
Unity.exe -batchmode -quit -projectPath . -executeMethod ValleDePlata.Editor.PrototypeSceneValidator.ValidatePhase1Scene -logFile Logs\phase1_scene_validator.log
```

Wynik:

- validator znalazł wymagane obiekty: Pablo controller, prototype sedan, camera rig, debug HUD, pressure marker i workshop interactable,
- log zawiera `Phase 1 scene validation passed.`,
- log kończy się `Exiting batchmode successfully now!` i `return code 0`.

## Handoff

To nie zamyka Fazy 1. To pierwszy fundament. Następny najmniejszy milestone powinien sprawdzić feel bardziej bezpośrednio:

1. dodać prosty automatyczny smoke route albo playmode harness dla wejścia/wyjścia z auta,
2. sprawdzić, czy auto nie blokuje się na ciasnym zakręcie,
3. dopiero potem ręcznie/automatycznie oceniać 10-minutowy loop z Prototype Contract.
