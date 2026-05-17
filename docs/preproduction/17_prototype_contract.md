# 17. Prototype Contract

## Cel

Ten dokument zamienia werdykt review w kontrakt produkcyjny. Faza 1 nie ma udowodnić, że umiemy zrobić grę o kartelu. Ma udowodnić coś bardziej podstawowego: czy samo bycie Pablo Valerą w TPP ma napięcie w rękach.

Jeśli ruch, kamera, jazda i prosta interakcja nie działają, nie wolno przechodzić do pełnego "Pierwszego Frontu". Fabuła, fronty, brudna kasa i porucznicy nie uratują gry, która źle prowadzi się pieszo i w aucie.

## Pitch Fazy 1

> Dziesięciominutowy greybox, w którym gracz idzie ciasną ulicą Barrio Hondo, wsiada do auta, przejeżdża ryzykowną trasę, wysiada, wykonuje jedną interakcję i wraca pod presją otoczenia.

To nie jest misja. To test ciała gry.

## Zakres Fazy 1

### Must have

- TPP controller pieszy z chodzeniem, biegiem, obrotem, zatrzymaniem i prostą interakcją.
- Kamera piesza, która działa w ciasnej ulicy, przy ścianie, przy aucie i przy małej różnicy wysokości.
- Jedno auto z jazdą po wąskiej trasie, hamowaniem, zawracaniem i kontrolowanym poślizgiem.
- Wejście i wyjście z auta bez utraty orientacji kamery.
- Jedna interakcja z obiektem albo NPC, bez dialogu systemowego.
- Jedna trasa testowa: ulica, zakręt, wąskie przejście, punkt zatrzymania, powrót.
- Jedna forma napięcia bez pełnej policji: stojący patrol, blokada, radiowy komunikat albo obecność obserwatora.

### Should have

- Prosty cover feel przy ścianie lub aucie, nawet jeśli bez pełnego systemu osłon.
- Dwa typy nawierzchni: asfalt/beton i nierówna droga.
- Jeden punkt, w którym kamera musi rozwiązać ciasnotę bez przeskoku.
- Prosty debug readout prędkości, stanu gracza i aktywnej interakcji.

### Could have

- Jeden pieszy NPC jako przeszkoda społeczna.
- Jedno auto cywilne jako statyczna blokada.
- Tymczasowy dźwięk silnika, kroków i radia.

### Out of scope

- Pełna misja "Pierwszy Front".
- Strzelanie i system broni.
- Pełne AI przeciwników.
- System ekonomii.
- Brudna kasa.
- Pranie pieniędzy.
- Mapa wpływów.
- Dialogi zależne od decyzji.
- Finalna grafika, finalne animacje i finalne UI.
- Więcej niż jedna testowa trasa.
- Więcej niż jedno auto.
- Produkcja pełnej dzielnicy.

## Warunki przejścia

Faza 1 przechodzi tylko wtedy, gdy wszystkie poniższe punkty są prawdziwe po playteście:

- 10 minut chodzenia i jazdy nie męczy.
- Kamera nie walczy z graczem w ciasnej ulicy.
- Gracz rozumie, gdzie jest przód postaci, gdzie jest auto i gdzie ma jechać.
- Wejście i wyjście z auta jest płynne i nie dezorientuje.
- Auto daje napięcie na wąskiej trasie, ale nie czuje się jak przypadkowy chaos fizyki.
- Prosta interakcja z NPC albo obiektem działa bez frustracji.
- Da się stworzyć napięcie trasą i obecnością patroli, nawet bez pełnego pościgu.
- Po teście wiadomo, co poprawić w controllerze, kamerze i jeździe przed dodaniem broni.

## Warunki porażki

Faza 1 nie przechodzi, jeśli:

- najlepszy feedback brzmi "kiedy dojdą misje, będzie fajnie",
- kamera często ukrywa gracza albo obiekt interakcji,
- auto jest przyjemne tylko na szerokiej pustej drodze,
- wejście do auta wygląda jak przerwa w kontroli, nie jak naturalny ruch,
- gracz gubi orientację po każdym ostrym zakręcie,
- test potrzebuje cutscenek, dialogów albo nagród, żeby nie nudzić,
- controller działa na klawiaturze albo padzie, ale nie na obu, jeśli oba mają być wspierane w prototypie.

## Playtest kontrakt

Każdy playtest Fazy 1 powinien trwać 10-15 minut i obejmować ten sam przebieg:

1. Start pieszo w ciasnej ulicy.
2. Dojście do auta z jedną przeszkodą po drodze.
3. Wejście do auta.
4. Przejazd krótką trasą z przynajmniej jednym ostrym zakrętem.
5. Zatrzymanie przy punkcie interakcji.
6. Wyjście z auta.
7. Interakcja z obiektem albo NPC.
8. Powrót do auta.
9. Przejazd przez punkt presji.
10. Koniec przy bezpiecznym miejscu.

## Metryki jakości

### Feel pieszy

- Gracz zatrzymuje się tam, gdzie chciał.
- Postać nie wpada w ściany przy normalnym sterowaniu.
- Kamera nie potrzebuje ciągłego ratowania przez gracza.
- Interakcje są czytelne bez dużej ikony w centrum ekranu.

### Feel auta

- Auto ma masę, ale reaguje bez opóźnienia.
- Hamowanie jest przewidywalne.
- Zawracanie w ciasnym miejscu jest wykonalne.
- Kolizje nie niszczą testu przy małym błędzie.

### Przejście pieszo/auto

- Kamera utrzymuje orientację.
- Gracz nie traci kontroli na zbyt długo.
- Po wyjściu z auta gracz wie, gdzie iść.

### Napięcie

- Jedna stojąca przeszkoda może zmienić zachowanie gracza.
- Trasa ma moment ostrożności, nie tylko sprint od punktu do punktu.
- Da się odczuć "jestem obserwowany" bez pełnego wanted level.

## Zakaz kompensowania

Nie wolno kompensować słabego feelu:

- mocniejszym dialogiem,
- większą ilością obiektów,
- muzyką udającą napięcie,
- większą liczbą przeciwników,
- nowymi systemami,
- lore,
- cutscenką.

Jeśli Faza 1 nie działa w greyboxie, problem jest w fundamencie, nie w braku contentu.

## Wyjście z Fazy 1

Po udanej Fazie 1 powstaje krótki raport:

- co działa w ruchu,
- co działa w kamerze,
- co działa w jeździe,
- co jest nadal ryzykowne,
- jakie parametry są bazowe,
- co wolno dodać w Fazie 2,
- czego nadal nie wolno dodawać.

Tylko po tym raporcie można przejść do akcji, presji i pierwszych mikrotestów konsekwencji.
