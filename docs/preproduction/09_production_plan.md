# 09. Plan produkcji

## Cel

Ten plan mówi, co robić po zatwierdzeniu preprodukcji. Nie zaczynamy od pełnej gry. Zaczynamy od dowodów, że fundament działa.

Szczegółowy podział kilku faz, kryteria akceptacji i stop conditions są rozpisane w [20_multiphase_roadmap.md](20_multiphase_roadmap.md). Ten dokument zostaje krótkim streszczeniem kierunku produkcyjnego.

## Faza 0: Dokumenty decyzyjne

Status: obecna faza.

Artefakty:

- manifest,
- taste bible,
- core loop,
- mapa funkcjonalna,
- postacie/frakcje,
- szkielet kampanii,
- karty systemów,
- mission grammar,
- vertical slice,
- roadmap,
- prototype contract,
- minimalny World State,
- mikrotesty konsekwencji,
- runbook autonomicznej pracy.

Warunek przejścia dalej: dokumenty nie mogą sobie przeczyć, a vertical slice musi jasno wynikać z filarów.

## Faza 1: Feel prototyp

Cel: sprawdzić, czy TPP, kamera i jazda mają sens zanim powstanie jakikolwiek większy content.

Zakres:

- szara scena testowa,
- TPP controller,
- kamera piesza,
- wejście/wyjście z auta,
- jazda jednym autem,
- podstawowa interakcja,
- prosty test ciasnej ulicy.

Nie robić jeszcze:

- pełnych misji,
- ekonomii,
- mapy wpływów,
- dialogów,
- dużego miasta.

Warunek sukcesu: 10 minut ruchu i jazdy nie męczy, kamera nie walczy z graczem, a przejście pieszo/auto jest płynne. Pełny kontrakt Fazy 1 jest w [17_prototype_contract.md](17_prototype_contract.md).

Równoległy warunek techniczny: ustalić bazowy target wydajności i zasady scen testowych dla Unity 6 URP, zanim projekt zacznie produkować miasto.

## Faza 2: Action and pressure prototyp

Cel: sprawdzić, czy przemoc, AI i heat dają napięcie bez robienia z gry strzelnicy. Faza powinna zacząć się od mikrotestów z [19_microtests.md](19_microtests.md), a nie od pełnej misji.

Zakres:

- pistolet,
- jeden typ przeciwnika,
- cywile reagujący paniką,
- prosty policjant,
- lokalny heat,
- krótki pościg,
- konsekwencja po przemocy.

Warunek sukcesu: otwarta przemoc zmienia sytuację i tworzy presję, nie tylko kolejną falę wrogów.

## Faza 3: Imperium prototyp

Cel: połączyć akcję z mapą wpływów. Minimalna prawda o świecie jest opisana w [18_world_state_model.md](18_world_state_model.md).

Zakres:

- jedna dzielnica,
- jeden front,
- brudna kasa,
- pranie pieniędzy albo łapówka,
- miłość ludzi/strach/presja państwa/lojalność w prostej wersji,
- stan przed/po przejęciu.

Warunek sukcesu: gracz po jednej operacji widzi zmianę świata.

## Faza 4: Mission slice

Cel: zbudować “Pierwszy Front” jako pełną miniaturę gry.

Zakres:

- sekwencja 30-60 minut,
- jedna dzielnica,
- jedna mała finca/posiadłość jako kontrast,
- jedna operacja przerzutu,
- jedna komplikacja,
- jedna decyzja stylu,
- jedna konfrontacja,
- jeden front,
- jeden porucznik lub ważny kontakt,
- jedna konsekwencja w świecie.

Warunek sukcesu: slice da się pokazać komuś bez tłumaczenia i ta osoba rozumie, że to gra o budowaniu kartelowego imperium, nie ogólny open world crime game.

## Faza 5: Pre-alpha expansion

Cel: dopiero po sprawdzonym vertical slice rozszerzać skalę.

Możliwe kierunki:

- druga dzielnica,
- drugi porucznik,
- bardziej zaawansowany heat,
- pierwsza większa zdrada,
- media i polityk,
- bardziej rozbudowany front,
- mała baza/willa jako przestrzeń konsekwencji.

Warunek: nie rozszerzać mapy, jeśli jedna dzielnica nie pamięta działań gracza.

## Ryzyka

### Średni GTA-klon

Objaw: dużo jazdy, strzelania i ikonek, mało imperium.

Kontra: każda aktywność musi zmieniać kasę, heat, terytorium, reputację, strach, lojalność lub relację.

### Serial bez gry

Objaw: świetne sceny, ale gracz tylko ogląda wzrost Pablo.

Kontra: kluczowe przełomy muszą mieć grywalne decyzje i systemowe konsekwencje.

### Tycoon bez ciała

Objaw: ekonomia działa w menu, ale TPP jest tylko chodzeniem do znaczników.

Kontra: najważniejsze operacje wymagają osobistego ryzyka w świecie.

### Za duża mapa za wcześnie

Objaw: wiele dzielnic, brak pamięci świata.

Kontra: jedna dzielnica musi mieć kilka stanów przed produkcją drugiej.

### Przemoc bez ceny

Objaw: brutalność jest zawsze najlepszym wyborem.

Kontra: brutalność ma dawać szybki wynik, ale podnosi heat, obniża lojalność albo psuje relacje cywilne.

## Następny konkretny krok

Po zaakceptowaniu tego pakietu preprodukcyjnego następnym zadaniem nie jest jeszcze “robić pełną grę”. Dokumenty gate'u są teraz rozpisane:

1. style lock,
2. protagonist brief Pablo Valery,
3. pierwszy porucznik/kontakt Mateo Pardo,
4. mission design doc “Pierwszy Front”,
5. vertical slice system cards.

Rekomendacja: zacząć Faza 1 prototypu według [17_prototype_contract.md](17_prototype_contract.md): wyłącznie feel TPP/kamera/jazda w szarej scenie. Nie zaczynać jeszcze pełnego kodu “Pierwszego Frontu”. Jeśli praca ma iść autonomicznie, użyć [21_autonomous_goal_runbook.md](21_autonomous_goal_runbook.md).
