# 09. Plan produkcji

## Cel

Ten plan mówi, co robić po zatwierdzeniu preprodukcji. Nie zaczynamy od pełnej gry. Zaczynamy od dowodów, że fundament działa.

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
- roadmap.

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

Warunek sukcesu: 10 minut ruchu i jazdy nie męczy, kamera nie walczy z graczem, a przejście pieszo/auto jest płynne.

Równoległy warunek techniczny: ustalić bazowy target wydajności i zasady scen testowych dla Unity 6 URP, zanim projekt zacznie produkować miasto.

## Faza 2: Action and pressure prototyp

Cel: sprawdzić, czy przemoc, AI i heat dają napięcie bez robienia z gry strzelnicy.

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

Cel: połączyć akcję z mapą wpływów.

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

Rekomendacja: zrobić krótki review dokumentów 13-16 przeciwko `11_preproduction_review_gate.md`, a potem zacząć Faza 1 prototypu: wyłącznie feel TPP/kamera/jazda w szarej scenie. Nie zaczynać jeszcze pełnego kodu “Pierwszego Frontu”.
