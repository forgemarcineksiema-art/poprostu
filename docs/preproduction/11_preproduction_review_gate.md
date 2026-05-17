# 11. Preproduction review gate

## Werdykt v0.1

Pakiet preprodukcji ma mocny kierunek, ale nie jest jeszcze gotowy do pełnego startu produkcji. Jest wystarczający, żeby prowadzić dalszą rozmowę bez błądzenia, ale nie wystarczający, żeby bezpiecznie kodować vertical slice.

Najlepszy obecny rdzeń:

> Fabularny sandbox TPP o budowaniu kartelowego imperium, gdzie chaos wynika z systemów, fabuła trzyma stawkę, a każdy sukces zwiększa presję, paranoję i ryzyko utraty kontroli.

## Co jest już ostre

- Model: fabularny sandbox, nie czysty chaos ani liniowy dramat.
- Łuk: wzrost -> imperium -> oblężenie.
- USP: człowiek, który może kupić prawie wszystko oprócz spokoju.
- Systemowe serce: miłość ludzi / strach / presja państwa / lojalność poruczników.
- Skala slice'a: jedna dzielnica, jeden front, jeden porucznik/kontakt, brudna kasa, jedna decyzja stylu i konsekwencja w świecie.
- Techniczna zasada: jedna prawda o świecie w World state, scena tylko prezentuje i reaguje.

## Co nadal jest za miękkie

### Styl

Status: uzupełniony w [13_style_lock.md](13_style_lock.md).

Pozostałe ryzyko: przed asset production nadal przyda się moodboard obrazkowy, ale zasady stylu, trzy 10-sekundowe testy, audio, UI i zakazy produkcyjne są opisane.

### Bohater

Status: uzupełniony w [14_pablo_valera_protagonist_brief.md](14_pablo_valera_protagonist_brief.md).

Pozostałe ryzyko: po pierwszym pisaniu dialogów trzeba sprawdzić, czy Pablo nie wpada w memicznego gangstera albo zimnego geniusza bez pęknięć.

### Porucznicy

Status: pierwszy porucznik/kontakt uzupełniony w [15_first_lieutenant_mateo_pardo.md](15_first_lieutenant_mateo_pardo.md).

Pozostałe ryzyko: Mateo musi zostać utrzymany jako postać z własnym interesem, nie tutorialowy kierowca.

### Systemy

Status: slice system cards są uzupełnione w [12_vertical_slice_system_cards.md](12_vertical_slice_system_cards.md).

Pozostałe ryzyko: przy implementacji wartości “miłość ludzi” i “presja państwa” nadal mogą zostać zredukowane do abstrakcyjnych pasków. Test akceptacji musi wymuszać reakcję dzielnicy, porucznika, patrolu albo frontu.

### Vertical slice

Status: mission design doc uzupełniony w [16_pierwszy_front_mission_design.md](16_pierwszy_front_mission_design.md).

Pozostałe ryzyko: przed kodem trzeba jeszcze zdecydować, czy Faza 1 jest czystym feel prototypem, czy mini-prototypem jednego beatu misji. Rekomendacja pozostaje: najpierw feel TPP/kamera/jazda.

## Definition of average

Jeśli pojawi się którykolwiek z tych objawów, projekt skręca w średnią grę:

- misja mogłaby wydarzyć się w dowolnym crime open worldzie po zmianie nazw,
- nagrodą jest tylko kasa, broń albo ikona na mapie,
- policja działa tylko jako pościgowy timer,
- brudna kasa natychmiast zmienia się w licznik,
- porucznik jest tylko bonusem statystyk,
- dzielnica po przejęciu różni się tylko kolorem na mapie,
- brutalność jest zawsze najskuteczniejszym wyborem bez kosztu,
- “klimat Kolumbii lat 80.” oznacza tylko żółty filtr, palmy i hiszpańskie słowa,
- fabuła mówi, że Pablo rośnie, ale gameplay tego nie pokazuje,
- systemy pozwalają na chaos, który nie zostawia śladu w świecie.

## Hard gates przed prototypem

### Gate 1: Manifest lock

Pass:

- jednym zdaniem da się odróżnić grę od GTA, Mafii, Narcos i Cartel Tycoon,
- wiadomo, co zawsze chronimy przy cięciu,
- wiadomo, czego gra nigdy nie robi.

Fail:

- pitch brzmi jak “Pablo Escobar open world”,
- nie wiadomo, czy ważniejsza jest fabuła, chaos czy imperium.

### Gate 2: Style lock

Pass:

- istnieje moodboard lub opis referencyjny dla biedy, luksusu, przemocy, UI, dźwięku i kamery,
- style guide mówi, jak wygląda scena po przemocy i scena luksusu,
- da się zrobić 10-sekundowy test tonu bez mechanik.

Fail:

- styl opiera się na ogólnych słowach: brudny, gorący, nerwowy,
- brak zasad, czego kamera i muzyka nie robią.

### Gate 3: System lock

Pass:

- każdy system slice'a ma wejścia, stany, efekty w świecie i test akceptacji,
- World state jest jedyną prawdą o froncie, presji, kasie i decyzji stylu,
- brudna kasa, porucznik i dzielnica mają widoczne konsekwencje po misji.

Fail:

- system działa tylko jako UI licznik,
- konsekwencje są ręcznie odpalonymi scenkami bez stanu świata.

### Gate 4: Slice lock

Pass:

- “Pierwszy Front” ma beat sheet, fail states, wymagane assety i trzy warianty decyzji,
- po 30-60 minutach gracz czuje wzrost, koszt i zmianę dzielnicy,
- slice można opowiedzieć jako miniaturę całej gry.

Fail:

- slice jest tylko tutorialem ruchu, strzelania i jazdy,
- finał nie zmienia świata.

## Decyzja

Nie projektować teraz szerzej. Nie dodawać kolejnych dzielnic, mechanik ani aktów. Następny krok:

1. review dokumentów 13-16 przeciwko temu gate'owi,
2. ewentualne korekty tonu/postaci/misji,
3. Faza 1: feel prototyp TPP/kamera/jazda.

Po Faza 1 dopiero wracać do “Pierwszego Frontu” jako pełnego vertical slice.
