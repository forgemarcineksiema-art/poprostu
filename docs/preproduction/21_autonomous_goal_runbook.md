# 21. Autonomous Goal Runbook

## Cel

Ten dokument przygotowuje Codexa do pracy autonomicznej z funkcją `/goal`. Ma zapobiec temu, że agent zacznie "robić grę", kiedy prawdziwym zadaniem jest dowód z konkretnej fazy.

Autonomia ma przyspieszać walidację, nie rozszerzać scope.

## Zasada główna

> Jeden goal = jeden mierzalny dowód, jedna faza, jasne non-goals, konkretna walidacja.

Jeśli goal nie ma stop condition, jest zbyt szeroki.

## Repo policy

- Pracujemy w repo `forgemarcineksiema-art/poprostu.git`.
- Domyślna gałąź robocza w tym repo to `main`, jeśli użytkownik nie poprosi inaczej.
- Nie tworzyć branchy tylko dlatego, że Codex zwykle lubi branch.
- Nie ruszać niezwiązanych zmian Unity w worktree bez wyraźnej zgody.
- Nie stage'ować plików spoza zakresu goala.
- Nie commitować ani pushować, dopóki użytkownik o to nie poprosi albo goal jawnie tego nie wymaga.
- Jeśli użytkownik mówi "pushuj", stage'ować tylko zamierzony zakres, commitować i pushować na aktualny branch.

## Dokumenty startowe

Codex przed pracą nad tym projektem czyta w tej kolejności:

1. [README](README.md)
2. [Preproduction review gate](11_preproduction_review_gate.md)
3. [Multiphase roadmap](20_multiphase_roadmap.md)
4. Dokument aktualnej fazy:
   - Faza 1: [Prototype Contract](17_prototype_contract.md)
   - Faza 2: [Microtests](19_microtests.md)
   - Faza 3: [Minimal World State Model](18_world_state_model.md)
   - Faza 4-5: [Pierwszy Front](16_pierwszy_front_mission_design.md)
5. [Game systems](06_game_systems.md)
6. [Mission grammar](07_mission_grammar.md)

Nie zaczynać od pełnej kampanii, świata ani nowych postaci.

## Goal packet template

```text
/goal
Objective:
  [Jedno zdanie: jaki dowód ma powstać.]

Phase:
  [Faza z 20_multiphase_roadmap.md.]

Scope:
  [Lista rzeczy, które wolno zmieniać.]

Non-goals:
  [Lista rzeczy, których nie wolno robić.]

Read first:
  - docs/preproduction/README.md
  - docs/preproduction/11_preproduction_review_gate.md
  - docs/preproduction/20_multiphase_roadmap.md
  - [dokument fazy]

Acceptance:
  [Mierzalne kryteria przejścia.]

Validation:
  [Komendy, playtest steps, screenshots, logs albo docs checks.]

Stop conditions:
  [Kiedy Codex ma się zatrzymać i nie zgadywać.]

Git:
  [Czy commit/push jest wymagany. Jeśli nie: zostawić zmiany lokalnie.]

Handoff:
  [Co ma być jasne dla następnego goala.]
```

## Przykładowy goal: Faza 1

```text
/goal
Objective:
  Zbudować pierwszy greybox feel prototype: pieszo -> auto -> ciasna trasa -> wyjście -> interakcja -> powrót przez punkt presji.

Phase:
  Faza 1: Feel prototype.

Scope:
  Minimalny controller TPP, kamera, jedno auto, wejście/wyjście, jedna interakcja, jedna scena testowa.

Non-goals:
  Brak strzelania, misji, ekonomii, World State, dialogów fabularnych, finalnego artu, mapy wpływów i pełnego "Pierwszego Frontu".

Read first:
  - docs/preproduction/README.md
  - docs/preproduction/11_preproduction_review_gate.md
  - docs/preproduction/17_prototype_contract.md
  - docs/preproduction/20_multiphase_roadmap.md

Acceptance:
  10 minut chodzenia i jazdy nie męczy; kamera działa w ciasnej ulicy; wejście/wyjście z auta nie dezorientuje; jedna interakcja działa; trasa ma napięcie bez pełnej policji.

Validation:
  Uruchomić projekt, przejść trasę testową, zapisać obserwacje, dołączyć status build/playtest. Jeśli test automatyczny jest możliwy, dodać prosty smoke test sceny.

Stop conditions:
  Zatrzymać się, jeśli Unity project/build nie działa, jeśli wejście/wyjście z auta wymaga większej decyzji architektonicznej, albo jeśli trzeba dotknąć niezwiązanych zmian Unity.

Git:
  Nie commitować i nie pushować bez osobnej zgody.

Handoff:
  Zostawić raport feelu, parametry bazowe i listę blokad przed Fazą 2.
```

## Przykładowy goal: Faza 2

```text
/goal
Objective:
  Zrobić trzy mikrotesty konsekwencji: przemoc publiczna, łapówka i Mateo zaufany/upokorzony.

Phase:
  Faza 2: Action and pressure microtests.

Scope:
  Najprostsza przemoc, cywile, jeden policjant, debug overlay World State, eventy testowe, dwa warianty Mateo.

Non-goals:
  Brak pełnej walki, fal policji, misji "Pierwszy Front", ekonomii, stealthu i kilku broni.

Read first:
  - docs/preproduction/18_world_state_model.md
  - docs/preproduction/19_microtests.md
  - docs/preproduction/20_multiphase_roadmap.md

Acceptance:
  Każdy mikrotest zmienia World State i ma widoczny efekt w świecie bez otwierania menu.

Validation:
  Przejść każdy mikrotest, zanotować event, stan przed/po i widoczny skutek. Jeśli możliwe, dodać test automatyczny dla event -> state.

Stop conditions:
  Zatrzymać się, jeśli konsekwencje istnieją tylko w debug overlay albo jeśli trzeba budować pełny system walki.

Git:
  Nie commitować i nie pushować bez osobnej zgody.

Handoff:
  Powiedzieć, które eventy są gotowe do Fazy 3 i które reakcje świata są nadal sztuczne.
```

## Tryb pracy autonomicznej

Codex powinien pracować tak:

1. Sprawdzić repo, branch i status.
2. Przeczytać dokumenty startowe.
3. Nazwać aktualną fazę.
4. Wypisać maksymalnie trzy możliwe małe milestone'y.
5. Wybrać jeden najmniejszy milestone, który daje dowód.
6. Wprowadzić zmiany.
7. Zweryfikować je komendą, playtestem albo checklistą.
8. Zaktualizować dokumenty tylko wtedy, gdy wynik zmienia decyzję.
9. Zostawić handoff dla następnego goala.

Jeśli użytkownik jawnie pozwoli na subagentów, można ich używać do niezależnych zadań, np. osobno review docsów, osobno sprawdzenie Unity setupu. Bez takiej zgody Codex pracuje lokalnie.

## Stop rules

Codex zatrzymuje się i raportuje, jeśli:

- żądanie wymaga pełnej produkcji zamiast fazowego dowodu,
- obecna faza nie ma spełnionych kryteriów akceptacji,
- trzeba ruszyć niezwiązane zmiany w worktree,
- Unity/build jest zablokowany problemem środowiska,
- implementacja wymaga decyzji projektowej spoza dokumentów,
- użytkownik mówi stop, pauza, koniec goala albo "zakończ po tym pushu".

## Co Codex ma ignorować

- Pokusę dopisania drugiej dzielnicy.
- Pokusę robienia pełnego scenariusza.
- Pokusę zaczynania od UI mapy wpływów.
- Pokusę budowania ekonomii przed feel prototypem.
- Pokusę naprawiania słabego feelu nowym contentem.
- Pokusę robienia branchy, jeśli użytkownik pracuje na `main`.

## Minimalny raport końcowy goala

Każdy goal kończy się krótkim raportem:

```text
Done:
  [Co powstało.]

Evidence:
  [Jak zweryfikowano.]

Changed files:
  [Lista plików.]

Not touched:
  [Ważne niezwiązane pliki, których nie ruszono.]

Next:
  [Najmocniejszy następny goal.]
```

## Najbliższy sensowny goal

Najbliższy goal po obecnej preprodukcji:

> Zbudować Faza 1 feel prototype w greyboxie: TPP, kamera, jedno auto, wejście/wyjście, jedna interakcja i krótka ciasna trasa z punktem presji. Bez misji, bez ekonomii, bez "Pierwszego Frontu".

To jest pierwszy kodowany dowód. Wszystko większe jest za wcześnie.
