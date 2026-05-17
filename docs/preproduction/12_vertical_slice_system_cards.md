# 12. Vertical slice system cards

Te karty definiują minimalne systemy, które muszą działać w “Pierwszym Froncie”. To nie jest pełna implementacja, tylko decyzje, które usuwają mgłę przed prototypem.

Szczegółowy minimalny model danych i eventów dla World State jest w [18_world_state_model.md](18_world_state_model.md). Te karty opisują intencję systemów, a dokument World State opisuje pierwszą prawdę danych dla slice'a.

## Card 1: World state

### Cel

Jedno miejsce prawdy dla stanu slice'a: kto kontroluje warsztat, jaka jest presja państwa, ile brudnej kasy istnieje, jaką decyzję stylu podjął gracz i jak porucznik/kontakt został potraktowany.

### Minimalny stan

- `districtId`: Barrio Hondo.
- `frontId`: warsztat.
- `frontControl`: neutralny, rywal, Pablo.
- `dirtyCash`: brak, w transporcie, ukryta, wyprana, przejęta.
- `statePressure`: niska, średnia, wysoka.
- `peopleLove`: niska, neutralna, wysoka.
- `fear`: niska, średnia, wysoka.
- `lieutenantTrust`: upokorzony, zawodowy, zaufany.
- `ruleStyleDecision`: przysługa, łapówka, groźba, pokaz siły.

### Wejścia

- ukończenie beatów misji,
- przemoc publiczna,
- łapówka,
- decyzja wobec porucznika,
- decyzja wobec cywilów,
- zabezpieczenie lub utrata brudnej kasy.

### Wyjścia

- wariant końcowego dialogu,
- zachowanie cywilów przy warsztacie,
- patrol lub brak patrolu,
- reakcja porucznika,
- stan mapy wpływów,
- dostępność frontu.

### Acceptance test

Po finale “Pierwszego Frontu” da się bez sceny i bez UI odczytać z World state:

- kto kontroluje warsztat,
- gdzie jest brudna kasa,
- jaki jest poziom presji państwa,
- jaką decyzję stylu podjął gracz,
- czy porucznik ufa Pablo.

## Card 2: Brudna kasa

### Cel

Pieniądze po operacji mają być napięciem, nie automatyczną nagrodą.

### Stany

- `none`: brak gotówki w operacji,
- `loose`: gotówka zdobyta, ale nie zabezpieczona,
- `carried`: gotówka jest fizycznie przewożona,
- `hidden`: gotówka ukryta w warsztacie lub pojeździe,
- `laundered`: gotówka przepuszczona przez front,
- `seized`: gotówka przejęta przez policję/rywala.

### Decyzje gracza

- szybko wyprać przez warsztat, ryzykując obserwację,
- użyć części na łapówkę, obniżając zysk,
- ukryć gotówkę, ale zostawić przyszły punkt ryzyka,
- powierzyć gotówkę porucznikowi, testując zaufanie.

### Efekty

- więcej czystej kasy,
- niższa albo wyższa presja państwa,
- wzrost zaufania lub napięcia z porucznikiem,
- możliwość późniejszego nalotu,
- rywal wie, gdzie uderzyć.

### Acceptance test

Gracz po misji ma czuć: “mam pieniądze, ale jeszcze nie wygrałem, bo muszę je zabezpieczyć”.

## Card 3: Miłość ludzi / strach / presja państwa / lojalność

### Cel

Jedna decyzja stylu w slice'ie musi pokazać, że władza ma różne waluty.

### Minimalne wartości

Każda wartość ma trzy poziomy: niska, neutralna, wysoka.

- `peopleLove`: schronienie, plotki, cywilna ochrona.
- `fear`: szybkie posłuszeństwo, panika, zemsta.
- `statePressure`: patrole, blokady, naloty, koszt łapówek.
- `lieutenantTrust`: gotowość do pomocy, ambicja, ryzyko zdrady.

### Decyzje i skutki

- Przysługa dla dzielnicy: +miłość ludzi, wolniejszy postęp, mniej strachu.
- Łapówka: -presja teraz, +ryzyko szantażu później.
- Groźba: +strach, -miłość ludzi, możliwy wzrost presji.
- Pokaz siły: szybkie przejęcie, duży wzrost strachu i presji.
- Upokorzenie porucznika: kontrola krótkoterminowa, spadek zaufania.
- Traktowanie porucznika jak partnera: większe zaufanie, większa jego ambicja.

### Acceptance test

Końcowa scena dzielnicy musi mieć co najmniej dwa warianty widoczne bez menu:

- ludzie ostrzegają Pablo albo unikają wzroku,
- policja patroluje bliżej albo bierze pieniądze,
- porucznik mówi jak partner albo jak człowiek, który zapamiętał upokorzenie.

## Card 4: Front biznesowy

### Cel

Warsztat ma być pierwszym dowodem, że przejęte miejsce nie jest ikoną, tylko częścią imperium.

### Stany

- `locked`: gracz nie ma dostępu,
- `contested`: lokal jest przedmiotem konfliktu,
- `controlled`: Pablo kontroluje lokal,
- `watched`: lokal działa, ale jest obserwowany,
- `burned`: lokal został spalony przez nalot lub rywala.

### Wejścia

- misja przerzutu,
- decyzja wobec właściciela,
- decyzja wobec policjanta,
- poziom presji państwa,
- obecność brudnej kasy.

### Wyjścia

- możliwość prania pieniędzy,
- miejsce spotkań,
- NPC Pablo przy lokalu,
- ryzyko nalotu,
- reakcja rywala,
- zmiana mapy wpływów.

### Acceptance test

Po przejęciu warsztatu gracz widzi przynajmniej trzy zmiany:

- inna obsada lokacji,
- inny dialog lub ambient,
- funkcja prania pieniędzy albo ukrycia kasy,
- reakcja policji lub rywala.

## Card 5: Mission pressure

### Cel

Misja nie może być liniowym tutorialem. Presja ma rosnąć przez decyzje i błędy gracza.

### Źródła presji

- patrol widzi podejrzane zachowanie,
- rywal blokuje trasę,
- porucznik źle reaguje na upokorzenie,
- brudna kasa zostaje zbyt długo niezabezpieczona,
- cywile widzą przemoc,
- policjant po łapówce chce więcej.

### Fail states bez game over

- część kasy przepada,
- warsztat zostaje `watched` zamiast czysto `controlled`,
- porucznik traci zaufanie,
- presja państwa startuje w kolejnym etapie jako średnia/wysoka,
- rywal przeżywa i wraca szybciej,
- dzielnica daje strach, ale nie miłość ludzi.

### Acceptance test

Co najmniej jedna porażka częściowa musi być możliwa bez restartu misji i musi zostać zapisana w World state.

