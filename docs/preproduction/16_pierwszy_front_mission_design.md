# 16. Mission design doc: Pierwszy Front

## Cel misji

“Pierwszy Front” ma być miniaturą całej gry: Pablo zdobywa pierwszy realny element imperium przez osobiste ryzyko, decyzję stylu rządzenia, brudną kasę, relację z porucznikiem i widoczną zmianę dzielnicy.

To nie jest tutorial strzelania. To test obietnicy: **akcja w TPP zmienia układ władzy**.

## Czas i miejsce

- Czas gry: 30-60 minut.
- Dzielnica: Barrio Hondo.
- Główna lokacja: warsztat “El Respiro”.
- Kontrast: mała finca na obrzeżu po finale.
- Zakres świata: kilka ulic, warsztat, punkt spotkania, trasa przerzutu, miejsce blokady, krótki dojazd do finci.

## Główne postacie

### Pablo Valera

Jeszcze nie jest królem. Ma ambicję, reputację lokalnego człowieka od problemów i głód większej gry.

### Mateo Pardo

Mechanik i przyszły porucznik logistyczny. Chce odzyskać warsztat wuja i wejść wyżej bez utraty twarzy.

### Sargento Julián Ríos

Lokalny policjant. Nie jest idealistą. Bierze pieniądze, ale boi się, że większa sprawa go spali. Dla gracza jest pierwszym testem: czy presję państwa kupuje się, przestrasza czy odkłada na później.

### Rómulo Ibarra

Mały lokalny rywal kontrolujący warsztat przez dług i przemoc. Nie jest bossem kartelu, tylko człowiekiem wystarczająco dużym, żeby blokować start Pablo.

### Doña Marisol

Kobieta z Barrio Hondo prowadząca mały sklep przy trasie. Może ostrzec Pablo przed patrolem, jeśli gracz wybierze przysługę dla dzielnicy zamiast szybkiej brutalności.

## Stany startowe World state

- `districtId = BarrioHondo`
- `frontId = ElRespiroWorkshop`
- `frontControl = rival`
- `dirtyCash = none`
- `statePressure = low`
- `peopleLove = neutral`
- `fear = low`
- `lieutenantTrust = professional`
- `ruleStyleDecision = none`

## Beat sheet

### Beat 1: Ulica wie, ale nie klęka

Czas: 0-5 minut.

Gracz zaczyna pieszo w Barrio Hondo i idzie do miejsca spotkania z Mateo. Ludzie rozpoznają Pablo, ale nie zachowują się jeszcze jak poddani.

Gameplay:

- krótki ruch TPP,
- kilka reakcji ambientowych,
- pierwszy widok warsztatu pod kontrolą ludzi Ibarry,
- opcjonalne podsłuchanie rozmowy o patrolu Ríosa.

Cel designu: pokazać, że Pablo ma lokalną obecność, ale nie władzę.

### Beat 2: Mateo składa układ

Czas: 5-10 minut.

Mateo wyjaśnia, że warsztat “El Respiro” jest idealnym pierwszym frontem: auta, części, ukryte skrytki, lokalne zaufanie. Problem: Ibarra kontroluje miejsce, a Ríos obserwuje ruch.

Decyzja mikro:

- Pablo traktuje Mateo jak partnera,
- Pablo traktuje go zawodowo,
- Pablo ucina rozmowę i wydaje rozkaz.

Efekt:

- ustawia początkowy ton `lieutenantTrust`,
- zmienia jedną linię dialogu i dostęp do ostrzeżenia w Beat 4.

### Beat 3: Pierwszy przerzut

Czas: 10-20 minut.

Gracz prowadzi auto z ukrytą paczką/gotówką testową przez Barrio Hondo. Trasa ma nauczyć jazdy i presji bez pełnego chaosu.

Możliwe komplikacje:

- patrol Ríosa stoi przy skrzyżowaniu,
- ludzie Ibarry obserwują warsztat,
- gracz może jechać główną drogą szybciej albo boczną trasą Mateo wolniej.

Fail without restart:

- jeśli gracz jedzie nieostrożnie, `statePressure` rośnie do medium,
- jeśli auto zostanie mocno uszkodzone, część gotówki przechodzi w `loose`.

### Beat 4: Warsztat jest obserwowany

Czas: 20-30 minut.

Mateo zauważa, że Ríos wie o ruchu. Ibarra jest w środku albo blisko warsztatu. Gracz musi wybrać styl rządzenia.

Główne warianty:

1. **Przysługa**
   - Pablo pomaga Doñi Marisol lub lokalnemu kierowcy, żeby zdobyć ostrzeżenie i poparcie ulicy.
   - Efekt: `peopleLove +`, wolniejszy postęp, niższe ryzyko cywilnej paniki.

2. **Łapówka**
   - Pablo płaci Ríosowi, żeby odwrócił wzrok.
   - Efekt: `statePressure - teraz`, ale Ríos ma hak i później zażąda więcej.

3. **Groźba**
   - Pablo zastrasza właściciela lub człowieka Ibarry.
   - Efekt: `fear +`, `peopleLove -`, ryzyko odwetu.

4. **Pokaz siły**
   - Pablo publicznie uderza w ludzi Ibarry.
   - Efekt: szybki dostęp do frontu, `fear ++`, `statePressure +`.

### Beat 5: Brudna kasa nie jest jeszcze wygraną

Czas: 30-38 minut.

Po rozwiązaniu wejścia do warsztatu pojawia się gotówka lub towar do zabezpieczenia. Gracz musi zdecydować, co zrobić z brudną kasą.

Warianty:

- wyprać przez warsztat natychmiast,
- ukryć w aucie,
- oddać Mateo,
- przepalić część na Ríosa,
- zostawić na miejscu i ryzykować nalot.

Efekt:

- ustawia `dirtyCash`,
- wpływa na `lieutenantTrust`,
- zmienia końcowy stan frontu.

### Beat 6: Ibarra wraca po swoje

Czas: 38-48 minut.

Rómulo Ibarra próbuje odzyskać kontrolę. Konfrontacja zależy od stylu:

- po przysłudze ludzie ostrzegają Pablo wcześniej,
- po łapówce policja nie reaguje od razu,
- po groźbie Ibarra przychodzi z mniejszą grupą, ale bardziej brutalnie,
- po pokazie siły konfrontacja jest większa i głośniejsza.

Gameplay:

- krótka walka lub pościg,
- cywile reagują,
- Mateo może pomóc albo spóźnić ostrzeżenie,
- brudna kasa może zostać zagrożona.

Fail without restart:

- Ibarra ucieka,
- część kasy przepada,
- warsztat zostaje `watched`,
- `statePressure` rośnie.

### Beat 7: Front zmienia właściciela

Czas: 48-55 minut.

Pablo przejmuje “El Respiro”. Warsztat nie jest tylko nagrodą. Musi zmienić dzielnicę.

Stany końcowe:

- `frontControl = Pablo`
- `frontControl = Pablo + watched`
- `dirtyCash = laundered / hidden / seized / carried`
- `lieutenantTrust = trusted / professional / humiliated`
- `ruleStyleDecision = chosen style`

Widoczne zmiany:

- ludzie Pablo przy warsztacie,
- Mateo w innej pozycji zależnie od relacji,
- patrol bliżej lub dalej,
- komentarze cywilów,
- mapa wpływów pokazuje pierwszy front.

### Beat 8: Finca

Czas: 55-60 minut.

Pablo jedzie do małej finci. To pierwszy smak awansu. Scena nie kończy się czystym triumfem: telefon, radio albo Mateo przypomina, że przejęcie frontu otworzyło większy konflikt.

Warianty końca:

- jeśli `peopleLove` wysokie: ktoś z Barrio ostrzega Pablo przed następnym ruchem Ríosa,
- jeśli `fear` wysokie: ludzie milczą i schodzą z drogi,
- jeśli `statePressure` wysokie: w radiu pojawia się wzmianka o przemocy,
- jeśli `lieutenantTrust` upokorzony: Mateo mówi krótko i chłodno.

## Wymagane lokacje

- ulica startowa Barrio Hondo,
- punkt spotkania z Mateo,
- warsztat “El Respiro” z wnętrzem i zewnętrzem,
- skrzyżowanie/punkt patrolu Ríosa,
- sklep Doñi Marisol,
- krótka boczna trasa,
- miejsce konfrontacji,
- mała finca.

## Wymagane assety minimalne

- Pablo jako tymczasowy model produkcyjny lub docelowy model w późniejszym etapie,
- Mateo,
- Ríos,
- Ibarra,
- Doña Marisol,
- cywile ambientowi,
- 2-3 ludzie Ibarry,
- jeden samochód Pablo,
- jeden radiowóz lub auto patrolowe,
- warsztat z bramą i miejscem ukrycia gotówki,
- gotówka jako fizyczny obiekt lub kontener,
- mapa wpływów w prostym UI.

## System requirements

- TPP controller,
- kamera piesza i jazdy,
- wejście/wyjście z auta,
- podstawowe celowanie i broń,
- proste AI przeciwników,
- cywilna panika/reakcja,
- lokalna presja państwa,
- World state,
- decyzja stylu,
- brudna kasa,
- front biznesowy,
- końcowa reakcja dzielnicy.

## Anti-average checks

Misja odpada albo wymaga przepisania, jeśli:

- warsztat jest tylko ikoną odblokowania,
- Ríos działa jak zwykła policja z wanted timerem,
- Mateo nie pamięta sposobu potraktowania,
- brudna kasa natychmiast znika w liczniku,
- finał nie pokazuje różnicy w dzielnicy,
- wszystkie warianty kończą się tą samą strzelaniną,
- finca jest tylko nagrodą, a nie kontrastem i zapowiedzią izolacji.

## Acceptance

Misja przechodzi, jeśli po 30-60 minutach gracz może powiedzieć:

- zdobyłem pierwszy front,
- wiem, kogo skrzywdziłem albo kupiłem,
- mam brudną kasę, która nadal jest ryzykiem,
- Mateo pamięta, jak go potraktowałem,
- dzielnica zachowuje się inaczej,
- Pablo jest bliżej władzy i dalej od spokoju.
