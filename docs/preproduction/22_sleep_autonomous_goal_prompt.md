# 22. Sleep Autonomous Goal Prompt

## Cel dokumentu

To jest gotowy prompt do uruchomienia autonomicznej pracy Codexa, kiedy właściciel projektu śpi albo nie nadzoruje sesji. Prompt celowo prowadzi agenta przez małe, walidowane kroki i blokuje skok w pełną produkcję gry.

Najbliższy realny cel to nie "zrobić grę" i nie "zrobić Pierwszy Front". Najbliższy realny cel to Faza 1: feel prototyp.

## Prompt do wklejenia

```text
/goal
Objective:
  Pracuj autonomicznie nad pierwszym kodowanym dowodem gry: Faza 1 feel prototype dla TPP/kamera/jazda. Celem jest greyboxowy test pieszo -> auto -> ciasna trasa -> wyjście -> interakcja -> powrót przez punkt presji. Nie zaczynaj pełnej gry ani pełnego "Pierwszego Frontu".

Repository:
  C:\Users\Marcin\Documents\hahahaha\My project
  Remote: forgemarcineksiema-art/poprostu.git
  Branch: main

Read first:
  - docs/preproduction/README.md
  - docs/preproduction/11_preproduction_review_gate.md
  - docs/preproduction/17_prototype_contract.md
  - docs/preproduction/20_multiphase_roadmap.md
  - docs/preproduction/21_autonomous_goal_runbook.md
  - docs/preproduction/22_sleep_autonomous_goal_prompt.md

Operating mode:
  Zachowuj się jak project lead, nie jak task runner. Najpierw sprawdź repo, branch, status i istniejącą strukturę Unity. Potem wybierz maksymalnie 3 małe milestone'y, wybierz najlepszy i wykonuj kolejne małe, walidowane kroki. Możesz używać subagentów do niezależnego researchu/review, jeśli pomaga to utrzymać tempo i nie blokuje głównej pracy.

Primary phase:
  Faza 1: Feel prototype.

Allowed scope:
  - jedna greyboxowa scena testowa lub minimalne rozszerzenie istniejącej sceny testowej,
  - minimalny TPP controller, jeśli projekt nie ma jeszcze sensownego,
  - kamera piesza działająca w ciasnej ulicy,
  - jedno auto i prosta jazda,
  - wejście/wyjście z auta,
  - jedna interakcja z obiektem albo NPC,
  - jedna krótka trasa z ciasnym zakrętem i punktem presji,
  - prosty debug readout tylko wtedy, gdy pomaga walidować feel,
  - krótki raport po playteście/verification.

Non-goals:
  - nie rób pełnego "Pierwszego Frontu",
  - nie rób ekonomii,
  - nie rób mapy wpływów,
  - nie rób pełnego World State,
  - nie rób systemu brudnej kasy,
  - nie rób pełnego combat systemu,
  - nie rób pełnej policji ani pełnych pościgów,
  - nie dodawaj nowych dzielnic,
  - nie pisz pełnego scenariusza,
  - nie produkuj finalnych assetów,
  - nie przebudowuj projektu szeroko bez konieczności.

Acceptance:
  Faza 1 może zostać uznana za udaną tylko jeśli:
  - 10 minut chodzenia i jazdy nie męczy,
  - kamera nie walczy z graczem w ciasnej ulicy,
  - wejście i wyjście z auta nie dezorientuje,
  - auto działa na krótkiej, ciasnej trasie,
  - jedna interakcja działa bez frustracji,
  - trasa ma napięcie bez pełnej policji,
  - wiadomo, co musi zostać poprawione przed Fazą 2.

Validation:
  Uruchom dostępne testy/checki projektu, jeśli istnieją. Jeśli Unity CLI/build nie jest dostępny, zweryfikuj przez statyczne sprawdzenie plików, scen i konfiguracji, a w raporcie jasno napisz, czego nie dało się uruchomić. Po każdej istotnej zmianie sprawdź git diff i upewnij się, że nie ruszasz niezwiązanych plików.

Git:
  Pracuj na main. Nie twórz nowego brancha. Commituj i pushuj tylko zweryfikowane, sensowne milestone'y. Stage'uj wyłącznie pliki należące do aktualnego goala. Nie stage'uj starych niezwiązanych zmian Unity, jeśli nie wynikają z tej pracy.

Autonomous loop:
  1. Zrób krótki plan.
  2. Wybierz maksymalnie 3 kandydackie milestone'y.
  3. Wykonaj najmniejszy milestone, który daje realny dowód.
  4. Zweryfikuj.
  5. Zaktualizuj raport/handoff.
  6. Jeśli milestone jest dobry i nie ma stop condition, commit/push.
  7. Przejdź do następnego małego milestone'u w ramach Fazy 1.
  8. Nie przechodź do Fazy 2, dopóki Faza 1 nie ma wiarygodnego dowodu feelu.

Stop conditions:
  Zatrzymaj się i zostaw jasny raport, jeśli:
  - projekt Unity nie otwiera się/build nie działa z powodu środowiska,
  - wejście/wyjście z auta wymaga większej decyzji architektonicznej,
  - musiałbyś ruszyć niezwiązane zmiany Unity,
  - nie da się zweryfikować sensownie zmian,
  - scope zaczyna przesuwać się w pełną misję, ekonomię albo full game,
  - pojawia się konflikt git/push wymagający decyzji właściciela.

Final handoff:
  Na końcu zostaw:
  - co działa,
  - co nie działa,
  - jak zweryfikowano,
  - jakie commity/pushe powstały,
  - które pliki zmieniłeś,
  - których niezwiązanych plików nie ruszałeś,
  - najmocniejszy następny goal.
```

## Intencja dla następnego Codexa

Ten prompt daje zgodę na autonomiczną pracę, ale tylko w wąskim korytarzu. Agent ma produkować dowody, nie rozszerzać marzenia. Jeśli nie ma pewności, ma wrócić do Prototype Contract i zrobić mniejszy krok.
