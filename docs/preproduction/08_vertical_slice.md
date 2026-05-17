# 08. Vertical slice

## Cel vertical slice

Vertical slice ma udowodnić, że gra działa jako całość: TPP feel, jazda, przemoc, korupcja, front, terytorium, heat, postać i konsekwencje. Nie ma pokazać wielkości gry. Ma pokazać jej smak.

Docelowy czas: 30-60 minut.

Obszar: jedna dzielnica startowa, jeden sporny fragment miasta i jedna mała finca/posiadłość jako kontrast biedy i pierwszego smaku luksusu.

## Roboczy tytuł slice'a

**Pierwszy Front**

Pablo Valera dostaje szansę wejścia w większy przerzut przez lokalny warsztat, który działa jako przykrywka. Warsztat kontroluje mały rywal, lokalny policjant wie więcej, niż powinien, a pierwszy zaufany porucznik może zostać potraktowany jak partner albo narzędzie.

## Sekwencja

### 1. Wejście w dzielnicę

Gracz zaczyna w Barrio Hondo. Krótki grywalny spacer i jazda pokazują biedę, lokalne relacje i to, że Pablo jeszcze nie jest królem.

Systemy: TPP, kamera, interakcje, ambient NPC.

### 2. Kontakt

Pablo spotyka człowieka, który oferuje dostęp do przerzutu przez warsztat. Gracz poznaje stawkę: bez frontu nie da się prać pieniędzy i utrzymać ludzi.

Systemy: dialog, wybór podejścia, mapa celu.

### 3. Pierwsza operacja

Gracz musi przewieźć paczkę lub człowieka przez dzielnicę. Pojawia się patrol albo blokada rywali.

Systemy: jazda, prosta policja/heat, trasa, pościg.

### 4. Komplikacja

Warsztat okazuje się obserwowany. Gracz może:

- przekupić policjanta,
- zastraszyć właściciela,
- zrobić pokaz siły wobec rywala,
- wykonać przysługę dla lokalnej osoby, żeby zdobyć lojalność.

Systemy: decyzja stylu, reputacja/strach/lojalność, koszt pieniędzy lub heat.

### 5. Konfrontacja

Rywal próbuje odzyskać kontrolę. Krótka walka lub pościg powinny być konsekwencją wcześniejszego wyboru.

Systemy: broń, AI, cywile, heat, pojazdy.

### 6. Przejęcie frontu

Gracz zdobywa warsztat jako pierwszy front. Brudna kasa może zostać przepuszczona przez lokal, ale rośnie ryzyko obserwacji.

Systemy: ekonomia, front, mapa wpływów.

### 7. Konsekwencja w świecie

Dzielnica zmienia zachowanie:

- ludzie Pablo pojawiają się przy warsztacie,
- policja patroluje częściej albo bierze łapówki,
- cywile komentują brutalność albo przysługę,
- rywal traci kontrolę, ale planuje odwet,
- mapa pokazuje pierwszy obszar wpływu.

Systemy: world state, NPC reakcje, mapa, zapis decyzji.

### 8. Wow moment: pierwszy smak posiadłości

Po przejęciu frontu gracz jedzie na małą fincę albo do skromnej posiadłości finansowanej pierwszymi większymi pieniędzmi. To nie jest jeszcze pałac. To kontrast: chwilowy luksus po brudnej operacji i pierwsza scena, w której gracz czuje, że awans ma cenę.

Systemy: jazda, prywatna lokacja, dialog z rodziną/porucznikiem, konsekwencja decyzji stylu.

## Minimalny zakres systemów

Must have:

- TPP controller,
- kamera piesza i jazdy,
- jeden samochód gracza,
- jeden typ wroga,
- jeden typ policjanta,
- podstawowa broń,
- jeden front,
- jeden porucznik lub ważny kontakt, który może zostać zaufany albo upokorzony,
- brudna kasa i jedna decyzja prania/łapówki,
- lokalny heat,
- miłość ludzi/strach/presja państwa/lojalność w najprostszej formie,
- mapa wpływów dla jednej dzielnicy,
- zapis decyzji stylu.

Should have:

- cywile reagujący na broń i strzały,
- prosty pościg,
- dialog zależny od decyzji,
- mała scena rodzinna lub prywatna po sukcesie,
- finca jako krótki, kontrolowany kontrast wizualny po dzielnicy.

Cut if needed:

- customizacja auta,
- większa liczba broni,
- skradanie jako pełny system,
- kilka dzielnic,
- rozbudowana ekonomia,
- pełna baza/willa,
- proceduralne wydarzenia poboczne.

## Kryteria odbioru

Vertical slice jest udany, jeśli:

- gracz rozumie główną fantazję bez czytania dokumentów,
- TPP i jazda nie przeszkadzają w odbiorze,
- po finalnej misji dzielnica wygląda lub zachowuje się inaczej,
- gracz podjął decyzję stylu rządzenia,
- ta decyzja ma widoczny efekt,
- heat wraca jako konsekwencja, nie tylko chwilowy alarm,
- front ma funkcję w świecie,
- porucznik albo ważny kontakt pamięta sposób potraktowania,
- brudna kasa wymaga zabezpieczenia, a nie znika w liczniku,
- slice zostawia pytanie: “co zrobię z tą władzą dalej?”.

## Fail states bez restartu

Slice nie powinien karać każdej porażki ekranem game over. Lepsze porażki częściowe:

- warsztat jest przejęty, ale oznaczony jako obserwowany,
- część brudnej kasy przepada,
- porucznik pomaga, ale zapamiętuje upokorzenie,
- dzielnica boi się Pablo, ale nie daje mu schronienia,
- policjant przyjmuje łapówkę i później żąda więcej,
- rywal przeżywa, traci lokal i planuje odwet,
- końcowy heat/presja państwa startuje na wyższym poziomie.

Co najmniej jedna taka porażka musi zostać zapisana w World state.

## Mission design doc

Mission design doc “Pierwszy Front” jest rozpisany w [16_pierwszy_front_mission_design.md](16_pierwszy_front_mission_design.md). Przed implementacją slice'a ten dokument musi pozostać źródłem prawdy dla:

- beat sheet minuta po minucie,
- trzy warianty decyzji stylu,
- konkretnego porucznika/kontaktu,
- konkretnego policjanta i rywala,
- listę wymaganych lokacji,
- listę wymaganych assetów,
- fail states,
- stany World state przed i po misji,
- końcowe warianty dzielnicy.
