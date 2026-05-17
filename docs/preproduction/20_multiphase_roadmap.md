# 20. Multiphase Roadmap

## Cel

Ten roadmap rozbija dalszą pracę na fazy, które dają dowód po dowodzie. Nie przechodzimy od preprodukcji prosto do gry. Najpierw sprawdzamy ciało gry, potem presję, potem pamięć świata, potem imperium, dopiero potem "Pierwszy Front".

Każda faza ma:

- cel,
- playable output,
- zakres,
- rzeczy zakazane,
- kryteria akceptacji,
- stop condition,
- dowód końcowy.

## Faza 0: Preproduction lock

### Cel

Zamknąć obecny pakiet jako fundament decyzyjny, nie jako nieskończony notes z pomysłami.

### Playable output

Brak. To faza dokumentacyjna.

### Zakres

- Manifest.
- Style lock.
- Core loop.
- World/map direction.
- Pablo Valera brief.
- Mateo Pardo brief.
- Mission grammar.
- "Pierwszy Front" jako docelowy vertical slice.
- Prototype Contract.
- Minimal World State.
- Mikrotesty.
- Ten roadmap.
- Runbook autonomicznej pracy.

### Zakazane

- Dopisywanie kolejnych dzielnic.
- Dopisywanie kolejnych poruczników.
- Rozpisywanie pełnej kampanii dialogowej.
- Projektowanie finalnej ekonomii.
- Produkcja assetów finalnych.

### Kryteria akceptacji

- Dokumenty nie przeczą sobie.
- Każdy dokument odpowiada na pytanie: jak gracz buduje imperium i płaci cenę?
- Wiadomo, że najbliższy kodowany krok to Faza 1, nie pełny slice.

### Stop condition

Jeśli podczas review pojawia się nowy wielki kierunek kreatywny, nie kodować. Najpierw zaktualizować manifest i style lock.

### Dowód końcowy

Commit z docsami i czysty opis następnego celu.

## Faza 1: Feel prototype

### Cel

Udowodnić, że TPP, kamera, auto, wejście/wyjście z auta i jedna interakcja mają sens w rękach.

### Playable output

10-15 minut greyboxu:

- start pieszo,
- dojście do auta,
- wejście do auta,
- przejazd ciasną trasą,
- wyjście,
- interakcja,
- powrót przez punkt presji.

### Zakres

- TPP controller.
- Kamera piesza.
- Jedno auto.
- Kamera jazdy.
- Wejście/wyjście z auta.
- Jedna interakcja.
- Jeden punkt presji bez pełnego AI.
- Debug readout dla stanu gracza.

### Zakazane

- Strzelanie.
- Misje.
- Dialogi fabularne.
- Brudna kasa.
- World State poza prostym debug stanem gracza.
- Produkcja dzielnicy.
- Ekonomia.

### Kryteria akceptacji

Opisane dokładnie w [17_prototype_contract.md](17_prototype_contract.md). Najważniejsze:

- 10 minut ruchu i jazdy nie męczy.
- Kamera nie walczy z graczem.
- Auto działa w ciasnej ulicy.
- Interakcja jest czytelna.
- Napięcie może wynikać z trasy i obecności, nie z fali wrogów.

### Stop condition

Jeśli feel nie działa, nie przechodzić do Fazy 2. Poprawiać controller, kamerę, auto i trasę testową.

### Dowód końcowy

- Krótki raport feelu.
- Lista parametrów bazowych.
- Lista problemów, które blokują akcję.
- Playtest capture albo opis dokładnego przebiegu testu.

## Faza 2: Action and pressure microtests

### Cel

Sprawdzić, czy przemoc i presja państwa tworzą napięcie, zamiast zamieniać grę w strzelnicę.

### Playable output

Trzy krótkie testy po 2-5 minut:

- publiczna przemoc,
- łapówka,
- relacja Mateo w tym samym beacie.

### Zakres

- Pistolet albo najprostsza forma przemocy.
- Jeden typ przeciwnika.
- Cywile reagujący paniką.
- Jeden policjant/patrol.
- Jeden mechanizm presji.
- Debug overlay World State.
- Eventy opisane w [18_world_state_model.md](18_world_state_model.md).
- Testy opisane w [19_microtests.md](19_microtests.md).

### Zakazane

- Pełny system walki.
- Pełne fale policji.
- Kilka broni.
- Pełny stealth.
- Mission flow "Pierwszego Frontu".
- Rozbudowane dialogi.

### Kryteria akceptacji

- Przemoc daje szybki wynik i natychmiastowy koszt.
- Łapówka rozwiązuje problem teraz, ale tworzy hak.
- Mateo reaguje gameplayowo, nie tylko barką.
- Każdy test zmienia World State.
- Każdy test ma widoczny skutek w świecie.

### Stop condition

Jeśli konsekwencje są widoczne tylko w debug overlay, zatrzymać się. Najpierw naprawić reakcje świata.

### Dowód końcowy

- Raport z trzech mikrotestów.
- Lista eventów, które działają.
- Lista reakcji świata, które są faktycznie widoczne.

## Faza 3: World State and save/load spine

### Cel

Ustalić jedną prawdę o stanie slice'a i sprawdzić, czy scena umie się z niej odbudować.

### Playable output

Mała scena testowa, w której gracz może zmienić:

- kontrolę frontu,
- stan kasy,
- presję,
- miłość/strach,
- relację Mateo,
- styl rządzenia.

Po restarcie testu scena pokazuje ten sam stan.

### Zakres

- Minimalny `SliceWorldState`.
- Jawne eventy.
- Debug overlay.
- Prosty zapis i odczyt testowy.
- Rekonstrukcja widocznych elementów sceny ze stanu.
- Mały panel developerski do wymuszania stanów.

### Zakazane

- Finalny system save/load całej gry.
- Pełne UI mapy wpływów.
- Rozbudowana ekonomia.
- Fronty poza El Respiro.
- Kilka dzielnic.

### Kryteria akceptacji

- Po zmianie stanu scena reaguje bez ręcznych cutscenek.
- Po wczytaniu front, patrol, ambient i Mateo zgadzają się ze stanem.
- Misja nie ma własnych równoległych flag dla tych samych rzeczy.

### Stop condition

Jeśli stan jest rozproszony między prefabami, mission scriptami i UI, zatrzymać się i uprościć model.

### Dowód końcowy

- Screenshot/debug log kilku stanów.
- Krótka tabela: event -> World State -> reakcja świata.

## Faza 4: Imperium prototype

### Cel

Połączyć ciało TPP z pierwszym elementem imperium: frontem, brudną kasą i reakcją dzielnicy.

### Playable output

15-20 minut:

- gracz poznaje warsztat El Respiro,
- transportuje albo zabezpiecza brudną kasę,
- wybiera styl przejęcia wpływu,
- widzi zmianę frontu i ulicy.

### Zakres

- Jeden front.
- Jedna forma brudnej kasy.
- Jedna decyzja stylu.
- Prosta reakcja Barrio Hondo.
- Jeden koszt presji.
- Mateo jako kontakt operacyjny.

### Zakazane

- Pełny "Pierwszy Front".
- Finca jako hub.
- Wieloetapowa kampania.
- Druga dzielnica.
- Drugi porucznik.
- Customizacja aut.

### Kryteria akceptacji

- Pieniądze nie są tylko licznikiem.
- Front zmienia stan świata.
- Dzielnica po operacji zachowuje się inaczej.
- Mateo ma co najmniej jeden realny wpływ na przebieg.

### Stop condition

Jeśli front jest tylko sklepem/ikoną, wrócić do World State i reakcji świata.

### Dowód końcowy

- Jedna ścieżka sukcesu.
- Jedna ścieżka częściowej porażki.
- Raport zmian World State.

## Faza 5: Mission slice assembly

### Cel

Złożyć "Pierwszy Front" jako pełną miniaturę gry, ale nadal z tymczasową grafiką i kontrolowanym zakresem.

### Playable output

30-60 minut:

- "Ulica wie, ale nie klęka",
- pierwsza operacja,
- komplikacja,
- decyzja stylu rządzenia,
- przejęcie El Respiro,
- konsekwencja w Barrio Hondo,
- przejście do finci jako gorzki awans.

### Zakres

- Beat sheet z [16_pierwszy_front_mission_design.md](16_pierwszy_front_mission_design.md).
- TPP i jazda z Fazy 1.
- Presja i mikrotesty z Fazy 2.
- World State z Fazy 3.
- Front i kasa z Fazy 4.
- Minimum dialogów potrzebnych do testu Pablo i Mateo.

### Zakazane

- Finalne cutscenki.
- Pełne voice acting.
- Więcej niż jeden front.
- Druga dzielnica.
- Rozbudowane poboczne aktywności.
- Pełna ekonomia kartelu.

### Kryteria akceptacji

- Slice można pokazać bez tłumaczenia manifestu.
- Gracz rozumie, że przejął strukturę, nie tylko ukończył misję.
- Co najmniej jedna decyzja ma koszt później w slice.
- Częściowa porażka nie wymusza restartu.
- Finca nie jest tylko nagrodą, ale znakiem izolacji.

### Stop condition

Jeśli slice działa tylko dzięki dialogom i skryptom, wrócić do Faz 2-4.

### Dowód końcowy

- Playthrough notes.
- Lista decyzji i konsekwencji.
- Lista systemów, które naprawdę pracują.
- Lista elementów wyciętych z powodu scope'u.

## Faza 6: Style target pass

### Cel

Udowodnić obrazem i dźwiękiem, że styl działa, zanim zacznie się produkcja większej liczby assetów.

### Playable output

Trzy krótkie testy tonu:

- Barrio Hondo dzień,
- warsztat po przemocy,
- finca po sukcesie.

### Zakres

- Światło.
- Kolor.
- Materiały tymczasowe.
- Kamera.
- Ambient audio.
- Zachowanie kilku NPC.
- Minimalne UI w tonie gry.

### Zakazane

- Pełny art pass dzielnicy.
- Produkcja dużej biblioteki assetów.
- Rozbudowa mapy.
- Dodawanie nowych misji.

### Kryteria akceptacji

- Barrio Hondo nie wygląda jak generyczna biedna dzielnica.
- Przemoc nie jest tylko efektem cząsteczkowym; scena po niej ma ciszę, wstyd i napięcie.
- Finca wygląda jak luksus i klatka jednocześnie.
- UI nie robi z tematu arcade'owej zabawki.

### Stop condition

Jeśli styl wymaga wyłącznie opisów słownych, nie produkować assetów finalnych. Najpierw zrobić target render albo animatic.

### Dowód końcowy

- Trzy capture'y albo krótkie sekwencje.
- Lista reguł dla modularnych assetów.
- Lista zakazów wizualnych.

## Faza 7: Vertical slice hardening

### Cel

Zamienić działający slice w wiarygodny materiał decyzyjny: stabilny, powtarzalny, mierzalny.

### Playable output

Jedna zamknięta wersja "Pierwszego Frontu" do review.

### Zakres

- Stabilność przejścia.
- Naprawa największych problemów feelu.
- Czytelność celów.
- Debug i telemetry notes.
- Podstawowy performance pass.
- Ograniczona liczba wariantów decyzji.
- Fail states bez restartu.

### Zakazane

- Nowe systemy poza tym, co potrzebne do slice.
- Rozszerzanie miasta.
- Dodatkowe misje.
- Pełna produkcja kampanii.

### Kryteria akceptacji

- Slice da się przejść powtarzalnie.
- Najważniejsze konsekwencje są widoczne i zapisane.
- Performance nie uniemożliwia oceny.
- Review może odpowiedzieć: skalować, ciąć, czy przebudować.

### Stop condition

Jeśli stabilizacja wymaga ciągłego dopisywania systemów, scope jest za duży.

### Dowód końcowy

- Build/slice candidate.
- Lista znanych problemów.
- Decyzja: greenlight pre-alpha expansion albo wrócić do fundamentów.

## Faza 8: Pre-alpha expansion decision

### Cel

Zdecydować, czy projekt zasługuje na rozszerzenie poza jedną dzielnicę.

### Możliwe kierunki

- Druga dzielnica.
- Drugi porucznik.
- Dziennikarz albo polityk jako ideowy antagonista.
- Bardziej zaawansowana presja państwa.
- Rozbudowany front.
- Finca jako przestrzeń konsekwencji.

### Zakazane

- Rozszerzanie wszystkiego naraz.
- Pełna mapa.
- Pełna kampania.
- Produkcja dużej liczby assetów bez testu modularności.

### Kryteria akceptacji

- Jedna dzielnica naprawdę pamięta działania gracza.
- Jedna misja naprawdę zmienia układ sił.
- TPP i jazda są wystarczająco dobre, żeby nie zasłaniać systemów.
- Projekt ma dowód, że nie jest GTA-klonem ani serialem z chodzeniem.

### Dowód końcowy

Decyzja produkcyjna:

- skalować,
- iterować vertical slice,
- przebudować core,
- zamknąć kierunek.

## Reguła autonomii

Autonomiczny Codex nie wybiera dowolnego zadania z całej wizji. Wybiera najbliższy najmniejszy dowód z aktualnej fazy.

Jeśli nie wiadomo, co robić dalej, priorytet jest zawsze taki:

1. Naprawić blokery aktualnej fazy.
2. Udowodnić kryterium akceptacji aktualnej fazy.
3. Zaktualizować dokumenty, jeśli odkrycie zmienia decyzję.
4. Dopiero potem proponować następną fazę.
