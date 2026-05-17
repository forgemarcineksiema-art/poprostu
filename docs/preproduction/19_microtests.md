# 19. Microtests Konsekwencji

## Cel

Te testy mają ochronić projekt przed największym ryzykiem systemowym: że miłość ludzi, strach, presja państwa i lojalność porucznika staną się paskami w UI.

Mikrotest nie jest misją. Jest krótkim dowodem, że jedna decyzja gracza zmienia World State i natychmiast zostawia ślad w świecie.

## Kolejność

Mikrotesty zaczynają się dopiero po Fazie 1, gdy controller, kamera, auto i podstawowa interakcja są wystarczająco stabilne. Nie wolno naprawiać feelu przez dodawanie konsekwencji.

## Test A: Przemoc publiczna

### Pytanie

Czy otwarta przemoc jest szybka, ale kosztowna?

### Setup

- Mały fragment Barrio Hondo.
- 4-8 cywilów.
- Jeden rival albo cel do zastraszenia.
- Jeden punkt obserwacji policji lub świadek.
- Debug overlay World State.

### Kroki gracza

1. Gracz podchodzi do celu.
2. Może rozwiązać sytuację bez strzału albo użyć przemocy.
3. Gracz strzela albo bije cel publicznie.
4. Cywile reagują.
5. Patrol lub świadek zwiększa presję.
6. Gracz odjeżdża albo odchodzi.

### Oczekiwany World State

```text
Fear: Low/Neutral -> High
PeopleLove: Neutral/High -> Low
StatePressure: Low -> Medium albo High
RuleStyle: ShowOfForce
LastEvent: PublicViolenceCommitted
```

### Widoczny feedback

- Cywile uciekają albo chowają się.
- Jeden sklep zamyka kratę lub drzwi.
- Ktoś mówi krótką barkę o tym, że Pablo przesadził.
- Patrol pojawia się bliżej albo radio zapowiada reakcję.
- Mateo może skomentować, że efekt był szybki, ale brud zostaje na ulicy.

### Pass

- Przemoc daje szybki wynik.
- Koszt jest widoczny bez otwierania menu.
- Debug overlay pokazuje zmianę.
- Ten sam fragment ulicy po zdarzeniu zachowuje się inaczej.

### Fail

- Jedynym kosztem jest chwilowy pościg.
- Cywile wracają do normalnego zachowania natychmiast.
- `Fear` rośnie, ale nikt w świecie tego nie pokazuje.
- Przemoc jest oczywiście najlepszą opcją.

## Test B: Łapówka

### Pytanie

Czy korupcja zmniejsza problem teraz, ale tworzy zależność później?

### Setup

- Jeden policjant Ríos lub funkcjonariusz testowy.
- Jedno miejsce zatrzymania auta.
- Jedna torba albo paczka oznaczona jako ryzyko.
- Debug overlay World State.

### Kroki gracza

1. Gracz jedzie z ryzykownym ładunkiem.
2. Policjant zatrzymuje auto.
3. Gracz może zapłacić, grozić albo próbować uciec.
4. Gracz wybiera łapówkę.
5. Policjant odpuszcza teraz, ale zapisuje zależność.

### Oczekiwany World State

```text
StatePressure: Medium/High -> Low albo Medium
RuleStyle: Bribe
DirtyCash: Carried -> Carried albo Hidden
LastEvent: BribeAccepted
```

Dodatkowy stan zależności może być osobnym eventem albo notatką w warstwie misji:

```text
RiosLeverage: Active
```

Jeśli tego pola jeszcze nie ma w modelu, nie dodawać całego systemu haków. Wystarczy jawny event i widoczny marker testowy.

### Widoczny feedback

- Patrol odchodzi albo otwiera przejazd.
- Policjant mówi lub gestem pokazuje, że teraz ma coś na Pablo.
- Radio lub barki ulicy nie eskalują od razu.
- Przy kolejnym kontakcie Ríos kosztuje więcej albo żąda przysługi.

### Pass

- Łapówka jest kusząca, bo rozwiązuje problem bez walki.
- Gracz rozumie, że to nie darmowe obniżenie heat.
- Świat zapamiętuje zależność.

### Fail

- Łapówka jest tylko przyciskiem "usuń policję".
- Nie ma żadnego późniejszego haka.
- Gracz nie widzi różnicy między łapówką a zwykłym uniknięciem patrolu.

## Test C: Mateo zaufany albo upokorzony

### Pytanie

Czy porucznik jest postacią z pamięcią, nie tutorialowym kierowcą?

### Setup

- Mateo jako pierwszy porucznik/kontakt.
- Ten sam beat jazdy albo transportu.
- Dwa wcześniejsze stany testowe:
  - `LieutenantTrust = Trusted`
  - `LieutenantTrust = Humiliated`
- Debug overlay World State.

### Wariant Trusted

#### Kroki

1. Gracz wcześniej ochronił Mateo albo dał mu zachować twarz.
2. Mateo jedzie z Pablo albo prowadzi przez radio.
3. Pojawia się zagrożenie na trasie.
4. Mateo ostrzega wcześniej.

#### Oczekiwany feedback

- Mateo daje ostrzeżenie przed patrolem zanim gracz go zobaczy.
- Sugeruje skrót.
- Ton jest rzeczowy, ale osobisty.
- Gracz może uniknąć gorszej presji.

### Wariant Humiliated

#### Kroki

1. Gracz wcześniej publicznie upokorzył Mateo albo naraził jego ludzi.
2. Ten sam beat jazdy zostaje uruchomiony.
3. Pojawia się to samo zagrożenie.
4. Mateo ostrzega później, chłodno albo wcale.

#### Oczekiwany feedback

- Ostrzeżenie przychodzi za późno.
- Mateo mówi formalnie albo zgryźliwie.
- Gracz nadal może przejść beat, ale z większym ryzykiem.
- Po beacie Mateo zaznacza, że pamięta.

### Oczekiwany World State

```text
LieutenantTrust: Trusted albo Humiliated
LastEvent: MateoProtected albo MateoHumiliated
```

### Pass

- Ten sam beat zmienia odczucie zależnie od relacji.
- Mateo pomaga inaczej, nie tylko ma inny tekst.
- Gracz rozumie, że traktowanie ludzi zmienia przyszłe operacje.

### Fail

- Mateo mówi inną linijkę, ale gameplay jest identyczny.
- Stan zaufania jest widoczny tylko w debug UI.
- Upokorzenie Mateo nie ma kosztu.

## Minimalny raport po mikrotestach

Po każdym teście zapisujemy:

- co zrobił gracz,
- jaki event został wysłany,
- jaki World State się zmienił,
- jaki efekt był widoczny w świecie,
- czy skutek przetrwał reset/wczytanie testu,
- czy decyzja miała koszt i zysk.

## Stop condition

Jeśli trzy mikrotesty nie potrafią pokazać konsekwencji bez pełnej misji, nie wolno składać "Pierwszego Frontu". Wtedy problemem jest model reakcji świata, nie brak contentu.
