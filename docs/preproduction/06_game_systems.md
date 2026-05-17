# 06. Systemy gry

Ten dokument definiuje systemy na poziomie preprodukcji. Nie jest jeszcze implementacją. Każdy system ma cel, wejścia, wyjścia, zależności, ryzyka i minimalny zakres dla vertical slice.

## Zasada ogólna

System zostaje w vertical slice tylko wtedy, gdy wzmacnia co najmniej jeden filar:

- wzrost imperium,
- osobiste ryzyko w TPP,
- cena władzy,
- żywe miasto.

Sercem gry nie jest licznik pieniędzy ani klasyczny wanted level. Sercem jest układ **miłość ludzi / strach / presja państwa / lojalność poruczników**. Te wartości są ze sobą spięte: brutalność może szybko podnieść strach, ale pogarszać miłość ludzi i prowokować presję państwa; inwestycje w barrio mogą zwiększać ochronę tłumu, ale przyciągać media i polityków.

## Architektura minimalnych systemów w Unity

Pierwsza implementacja ma być prosta, ale nie może mieszać wszystkiego w scenie. Prototyp może być lekki, natomiast źródło prawdy musi być czytelne od początku.

Warstwy:

- **Player layer**: TPP controller, kamera, wejście do pojazdu, celowanie i interakcje. Ta warstwa wysyła intencje gracza, ale nie powinna sama decydować o heat, terytorium ani ekonomii.
- **World state**: czyste klasy C# trzymające stan dzielnicy, frontów, heat, reputacji, strachu, lojalności, brudnej kasy i decyzji stylu. To jest jedna prawda o świecie.
- **Mission layer**: misje zapisują ukończone beaty, decyzje i konsekwencje przez World state, a nie przez przypadkowe flagi na obiektach sceny.
- **Presentation layer**: MonoBehavioury, NPC, ikony, ambient i UI odczytują World state i pokazują skutki w świecie.
- **Data layer**: ScriptableObjecty lub pliki danych opisują konfigurację dzielnic, frontów, misji, NPC i progów heat. Nie trzymają runtime state.

Zasady:

- Runtime state nie może być rozrzucony po pojedynczych prefabach.
- Każda duża akcja gracza powinna generować jawne zdarzenie: np. front przejęty, heat podniesiony, łapówka zapłacona, styl rządzenia zapisany.
- UI mapy wpływów czyta stan świata, nie liczy go samodzielnie.
- Misja nie odblokowuje contentu “na skróty”; zapisuje zmianę w World state, a systemy reagują na tę zmianę.
- Save dla vertical slice może być prostym JSON-em stanu świata, ale format powinien odzwierciedlać realne systemy: dzielnice, fronty, heat, kasa, decyzje, relacje.
- Prototyp może używać tymczasowych assetów graficznych, ale nazwy systemów i przepływ danych mają być zgodne z docelową architekturą.

Kryterium jakości architektury: po ukończeniu misji “Pierwszy Front” da się wskazać jedno miejsce, które mówi, kto kontroluje warsztat, jaki jest heat i jaką decyzję stylu podjął gracz.

## TPP controller

Cel: sprawić, że gracz czuje ciało bohatera, ciężar decyzji i kontrolę w ciasnych miejskich przestrzeniach.

Wejścia:

- ruch,
- sprint,
- osłona lub przykucnięcie,
- interakcja,
- broń schowana/wyciągnięta,
- wejście do pojazdu.

Wyjścia:

- pozycja gracza,
- stan widoczności/agresji,
- możliwość rozmowy lub zastraszenia,
- możliwość rozpoczęcia walki.

Vertical slice minimum:

- stabilny ruch,
- kamera za plecami,
- wejście/wyjście z auta,
- interakcja z NPC i punktami misji,
- brak walki z kamerą w ciasnych ulicach.

Kryterium jakości: poruszanie musi być wystarczająco dobre, żeby nie wstydzić się 10 minut chodzenia i jazdy bez strzelaniny.

## Kamera

Cel: wspierać czytelność i napięcie, bez robienia z gry taniego arcade'u.

Wejścia:

- ruch gracza,
- celowanie,
- jazda,
- przestrzeń ciasna/otwarta,
- zagrożenia.

Wyjścia:

- framing bohatera,
- czytelność celu,
- poczucie prędkości,
- brak zasłaniania najważniejszych decyzji.

Vertical slice minimum:

- kamera piesza,
- kamera celowania,
- kamera jazdy,
- unikanie ścian i przeszkód,
- osobne parametry dla pieszo/jazda.

Ryzyko: jeśli kamera jest słaba, żaden system imperium nie uratuje pierwszego wrażenia.

## Jazda

Cel: dać rytm open worlda, pościgi, przerzuty i status.

Wejścia:

- gaz, hamulec, skręt,
- typ pojazdu,
- nawierzchnia,
- heat i patrole.

Wyjścia:

- transport gracza,
- pościgi,
- stan pojazdu,
- wykrycie przez policję,
- możliwość przewozu ludzi lub towaru.

Vertical slice minimum:

- jeden zwykły samochód,
- jeden pojazd policyjny/rywala,
- pościg uliczny,
- punkt dostawy,
- czytelne uszkodzenie lub zatrzymanie pojazdu.

Kryterium jakości: jazda nie musi być symulacyjna, ale musi mieć ciężar i kontrolę.

## Broń i walka

Cel: przemoc ma być szybka, ryzykowna i systemowo kosztowna.

Wejścia:

- wyciągnięcie broni,
- celowanie,
- strzał,
- osłona/przeszkody,
- typ przeciwnika,
- obecność cywilów.

Wyjścia:

- obrażenia,
- alarm,
- heat,
- strach,
- panika cywilów,
- zmiana relacji z frakcją.

Vertical slice minimum:

- pistolet,
- broń długa dla wybranych przeciwników,
- prosta osłona środowiskowa,
- cywile reagujący paniką,
- policja reagująca na otwartą przemoc.

Kryterium jakości: strzelanie ma być czytelne i ciężkie, nie powinno zamieniać każdej misji w strzelnicę.

## AI NPC i frakcje

Cel: dać wrogom, sojusznikom i cywilom zachowania, które wzmacniają stan miasta.

Wejścia:

- frakcja,
- terytorium,
- heat,
- strach/lojalność,
- stan misji,
- obecność broni.

Wyjścia:

- patrol,
- ucieczka,
- atak,
- wezwanie wsparcia,
- poddanie się,
- reakcja dialogowa.

Vertical slice minimum:

- cywile uciekają i komentują,
- przeciwnicy walczą lub uciekają,
- policja patroluje i eskaluje,
- sojusznicy utrzymują pozycję albo pomagają w misji.

Ryzyko: AI nie może udawać pełnej symulacji życia. Ma robić niewiele, ale czytelnie.

## Heat i policja

Cel: pokazać, że państwo pamięta działania gracza. W dokumentach projektowych preferowana nazwa to **presja państwa**, a `heat` może zostać nazwą techniczną dla lokalnego poziomu eskalacji.

Wejścia:

- publiczna przemoc,
- martwi cywile,
- spalony front,
- pościg,
- łapówki,
- media,
- dowody.

Wyjścia:

- patrole,
- blokady,
- naloty,
- koszt łapówek,
- zainteresowanie prokuratora,
- ryzyko aresztowania ludzi.

Vertical slice minimum:

- lokalny heat dzielnicy,
- eskalacja patrolu po strzelaninie,
- możliwość zredukowania presji przez łapówkę lub ukrycie dowodu,
- jedna konsekwencja po misji.

Kryterium jakości: heat nie może być tylko timerem ucieczki. Ma wracać w stanie świata.

## Miłość, strach, presja, lojalność

Cel: zastąpić płaski morality/wanted model zestawem naczyń połączonych.

Wejścia:

- przysługi dla dzielnicy,
- brutalność publiczna lub ukryta,
- propaganda i inwestycje,
- łapówki,
- traktowanie poruczników,
- ofiary cywilne,
- naloty i konfiskaty,
- sukces lub porażka operacji.

Wyjścia:

- schronienie lub odmowa pomocy,
- plotki i informacje,
- koszt rekrutacji,
- ryzyko zdrady,
- intensywność patroli,
- blokady dzielnic,
- reakcje mediów,
- dostęp do polityków.

Vertical slice minimum:

- jedna decyzja, która wzmacnia miłość ludzi kosztem pieniędzy lub tempa,
- jedna decyzja, która wzmacnia strach kosztem presji państwa albo lojalności,
- jeden porucznik lub ważny kontakt, który zapamiętuje sposób potraktowania,
- jedna widoczna reakcja dzielnicy po misji.

Kryterium jakości: gracz nie pyta tylko “ile mam pieniędzy?”, ale “czy ja jeszcze kontroluję to, co zbudowałem?”.

## Terytoria i wpływy

Cel: pokazać wzrost imperium na mapie i ulicy.

Wejścia:

- ukończone operacje,
- fronty,
- porucznicy,
- strach,
- lojalność,
- działania rywali.

Wyjścia:

- kontrola dzielnicy,
- dostęp do tras,
- bezpieczeństwo,
- przychód,
- ryzyko nalotu,
- reakcje NPC.

Vertical slice minimum:

- jedna startowa dzielnica,
- jeden obszar sporny,
- jeden front do przejęcia,
- stan przed i po przejęciu widoczny w świecie.

Kryterium jakości: gracz ma zobaczyć przejęcie dzielnicy bez czytania tabelki.

## Ekonomia i pranie pieniędzy

Cel: sprawić, że pieniądze są zasobem władzy, ale też ryzykiem.

Wejścia:

- przychód z operacji,
- brudna kasa,
- fronty,
- łapówki,
- utrzymanie ludzi,
- straty.

Wyjścia:

- czysta kasa,
- inwestycje,
- ryzyko śledztwa,
- dostęp do ludzi i zasobów,
- rozwój frontów.

Vertical slice minimum:

- brudna kasa po operacji,
- jeden front do prania,
- jedna decyzja: zainwestuj, przepal na łapówkę albo zachowaj ryzyko.

Kryterium jakości: ekonomia ma być prosta, ale nie może być tylko licznikiem pieniędzy.

## Brudna kasa jako fizyczne ryzyko

Cel: sprawić, że pieniądze zanim trafią do frontu są obiektem napięcia, nie abstrakcyjną nagrodą.

Wejścia:

- gotówka z operacji,
- miejsce ukrycia,
- transport,
- zaufany człowiek,
- aktywna presja państwa,
- rywal albo informator.

Wyjścia:

- utrata części kasy,
- dowód dla policji,
- konieczność pościgu,
- konflikt z porucznikiem,
- możliwość szybkiej łapówki,
- większy zysk po bezpiecznym praniu.

Vertical slice minimum:

- po operacji pojawia się brudna kasa jako ryzykowny zasób,
- gracz musi ją dowieźć, ukryć, przepalić lub przepuścić przez front,
- jeśli gracz ignoruje ryzyko, presja państwa albo rywal ma konkretną okazję do uderzenia.

Kryterium jakości: duża wypłata powinna cieszyć i niepokoić jednocześnie.

## Fronty biznesowe

Cel: osadzić ekonomię w lokacjach.

Wejścia:

- kupno/przejęcie,
- reputacja,
- heat,
- ochrona,
- menedżer/porucznik.

Wyjścia:

- pranie pieniędzy,
- legalna przykrywka,
- misje obrony,
- lokalna lojalność lub strach,
- ryzyko nalotu.

Vertical slice minimum:

- jeden front w dzielnicy,
- stan przed przejęciem,
- stan po przejęciu,
- jedna konsekwencja policyjna lub rywalizacyjna.

## Mit publiczny

Cel: oddzielić publiczną legendę Pablo od miłości ludzi, strachu i lojalności poruczników. Mit mówi, jak szeroko świat rozpoznaje Pablo i jak media, politycy oraz rywale opowiadają jego postać.

Wejścia:

- publiczne akcje,
- przysługi,
- brutalność,
- wypłaty i inwestycje,
- dziennikarskie materiały,
- propaganda.

Wyjścia:

- większy rozgłos,
- łatwiejszy dostęp do polityków i rywali wysokiego szczebla,
- większa presja mediów,
- dzieciaki i młodzi rekruci naśladujący kartel,
- trudniejsze ukrywanie się w akcie oblężenia.

Vertical slice minimum:

- jedna wzmianka radiowa, plotka albo lokalny komentarz po przejęciu frontu,
- różny ton komentarza zależnie od decyzji: patron, bandyta, człowiek od problemów albo zagrożenie.

Kryterium jakości: gracz powinien rozumieć, że legenda pomaga rosnąć, ale później utrudnia zniknięcie.

## Mission state i zapis

Cel: umożliwić światu pamięć decyzji bez pełnej symulacji wszystkiego.

Wejścia:

- ukończone misje,
- styl rozwiązania,
- straty,
- zabici/ocaleni NPC,
- stan frontów,
- heat.

Wyjścia:

- odblokowania,
- dialogi,
- patrole,
- ceny,
- reakcje frakcji,
- warunki kolejnych misji.

Vertical slice minimum:

- zapis ukończenia operacji,
- zapis decyzji stylu,
- zapis kontroli frontu,
- odczyt tych stanów w końcowej scenie dzielnicy.

## Do not build yet

Nie budować w pierwszym vertical slice:

- pełnego systemu życia każdego NPC,
- pełnego kraju,
- zaawansowanej gospodarki narkobiznesu,
- wielu klas broni,
- customizacji willi,
- wielu zakończeń,
- rozbudowanego stealth,
- złożonej polityki krajowej,
- proceduralnych gang wars bez ręcznie zaprojektowanych konsekwencji.

## Unity 6 URP: założenia techniczne do prototypów

Ten projekt nie powinien próbować od razu skali GTA V ani Red Dead Redemption 2. Cel techniczny to gęsty, kontrolowany obszar z dobrą iluzją miasta.

Od początku projektować pod:

- dzielnice jako osobne sceny addytywne lub przyszłe pakiety Addressables,
- modularne budynki, fasady, wnętrza i powtarzalne zestawy ulic,
- LODGroup dla architektury, pojazdów, roślinności i tłumu,
- AI LOD: pełna logika blisko gracza, uproszczony stan daleko,
- pooling dla NPC, aut, efektów, pocisków i tymczasowych znaczników,
- ruch uliczny jako iluzję wokół gracza, nie pełną symulację każdego auta,
- misje wokół małych dopracowanych lokacji, nie losowego chaosu na całej mapie.

Funkcje Unity 6 URP do sprawdzenia w profilowaniu:

- **GPU Resident Drawer**: oficjalna dokumentacja Unity opisuje użycie BatchRendererGroup i GPU instancingu do redukcji draw calli i czasu CPU; wymaga m.in. Forward+, Mesh Rendererów oraz platform z compute shader support. Nie zakładać zysku bez pomiaru.
- **STP / Spatial-Temporal Post-Processing**: software'owy upscaler używający technik spatial i temporal do uzyskania wysokiej jakości antyaliasowanego obrazu; wymaga compute shaderów i TAA. Testować dopiero na realnym buildzie.
- **Render Graph w URP**: Unity 6 opisuje go jako system poprawiający customizację pipeline'u oraz zużycie pamięci URP. Traktować jako domyślny kierunek zgodny z wersją Unity, nie jako osobny ficzer gry.
