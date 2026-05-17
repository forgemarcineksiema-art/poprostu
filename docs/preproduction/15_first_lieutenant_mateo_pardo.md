# 15. Pierwszy porucznik: Mateo Pardo

## Funkcja w vertical slice

Mateo Pardo jest pierwszym człowiekiem, który może stać się porucznikiem Pablo. Ma sprawdzić, czy gracz traktuje ludzi jak partnerów, narzędzia czy zastraszone zasoby.

Mateo nie jest tutorialowym pomocnikiem. Jest lokalnym fachowcem od aut, tras i warsztatów, bez którego “Pierwszy Front” nie działa. Jego relacja z Pablo ma zapisać pierwszy realny ślad stylu rządzenia.

## Dane robocze

- Imię: Mateo Pardo.
- Wiek: 31.
- Dzielnica: Barrio Hondo.
- Rola: mechanik, kierowca, człowiek od tras i części.
- Przyszła specjalizacja: logistyka, auta, magazyny, ukryte przejazdy.
- Relacja startowa: zna Pablo z dzielnicy, ale nie jest jego podwładnym.
- Publiczny obraz: spokojny mechanik, który “zna wszystkie silniki i połowę sekretów dzielnicy”.

## Jedno zdanie

Mateo chce wejść wyżej bez zostania bandytą na smyczy, a Pablo jest dla niego jedyną windą, która może po drodze urwać rękę.

## Historia

Mateo dorastał w Barrio Hondo przy warsztacie wuja. Warsztat był miejscem pracy, schronienia i nieformalnej giełdy informacji: kto ma auto, kto ma dług, kto przewozi coś za dużo, kto płaci policji.

Kilka miesięcy przed slice'em lokalny rywal przejął warsztat przez dług i przemoc. Mateo został w środku jako mechanik, bo zna klientów i trasy. W praktyce pracuje dla człowieka, którego nienawidzi.

Mateo idzie do Pablo nie dlatego, że jest lojalny. Idzie, bo Pablo ma odwagę i ambicję potrzebną do odbicia miejsca. To układ, który może stać się lojalnością albo przyszłą raną.

## Pragnienie

Mateo chce odzyskać warsztat i mieć udział w czymś większym. Nie chce być chłopcem od kluczyków. Chce, żeby Pablo widział w nim człowieka, który potrafi utrzymać operację przy życiu.

## Lęk

Boi się, że każdy silniejszy człowiek w końcu nazwie go “mechanikiem” takim tonem, jakby to znaczyło “sługa”.

## Granica

Mateo zniesie ryzyko, pieniądze pod stołem i przemoc wobec rywali. Źle znosi publiczne upokorzenie oraz narażanie ludzi z warsztatu bez powodu.

Jeśli Pablo upokorzy go przy innych, Mateo nadal pomoże, ale relacja startuje jako rana.

## Kompetencje

- zna skróty i boczne ulice,
- potrafi przygotować auto do przerzutu,
- wie, które warsztaty i stacje benzynowe zadają za dużo pytań,
- ma kontakty wśród kierowców i mechaników,
- rozumie, jak ukrywać gotówkę lub towar w aucie,
- potrafi ocenić, czy patrol jest przypadkowy czy ustawiony.

## Słabości

- ambicja przykryta spokojem,
- nie lubi tracić twarzy,
- ma lokalne przywiązania, które mogą przeszkadzać w brutalnej decyzji,
- nie jest jeszcze gotowy na skalę prawdziwej wojny karteli,
- jeśli dostanie władzę bez szacunku, będzie budował własne zabezpieczenia.

## Głos

Mateo mówi sucho, technicznie i konkretnie. Używa języka mechanika nawet do ludzi.

Przykładowe linie:

- “Auto mówi, zanim człowiek zacznie kłamać.”
- “Ten patrol nie stoi tu dla pogody.”
- “Chcesz warsztat? To nie pal ludzi, którzy mają go jutro otworzyć.”
- “Mogę ci dać trasę. Nie mogę ci dać cudów.”
- “Partner pyta. Szef rozkazuje. Ty zdecyduj, kim dziś jesteś.”

## Relacja z Pablo

### Start

Mateo szanuje odwagę Pablo, ale nie uznaje go automatycznie za szefa. Ich relacja zaczyna się od transakcji: ja dam ci warsztat i trasy, ty dasz mi przyszłość.

### Jeśli gracz traktuje Mateo jak partnera

- Mateo ostrzega o dodatkowym patrolu,
- proponuje bezpieczniejszą trasę kosztem czasu,
- po finale mówi do Pablo jak do człowieka, który dotrzymuje układów,
- `lieutenantTrust = zaufany`.

### Jeśli gracz traktuje Mateo zawodowo

- Mateo wykonuje plan,
- daje minimum informacji,
- po finale relacja jest stabilna, ale bez osobistego ryzyka,
- `lieutenantTrust = zawodowy`.

### Jeśli gracz upokarza Mateo

- Mateo nadal pomaga, bo chce warsztatu,
- nie ostrzega o jednym ryzyku albo robi to za późno,
- po finale używa chłodnego tonu,
- `lieutenantTrust = upokorzony`.

## Funkcja systemowa

Mateo jest pierwszym nośnikiem lojalności porucznika.

Wejścia:

- czy gracz pyta go o plan,
- czy gracz używa jego trasy,
- czy gracz ryzykuje ludzi warsztatu,
- czy gracz upokarza go przy właścicielu/policjancie,
- czy gracz powierza mu brudną kasę.

Wyjścia:

- dostęp do wariantu trasy,
- stan zaufania,
- końcowy dialog,
- przyszła cena wsparcia logistycznego,
- potencjalny problem w kolejnej misji.

## Rola w “Pierwszym Froncie”

Mateo:

1. wprowadza Pablo w problem warsztatu,
2. daje pierwszą trasę przerzutu,
3. rozpoznaje, że warsztat jest obserwowany,
4. proponuje rozwiązanie mniej brutalne, jeśli gracz wcześniej okazał szacunek,
5. pomaga zabezpieczyć brudną kasę albo robi to niechętnie,
6. staje przy warsztacie po finale jako znak, czy front jest partnerski czy wymuszony.

## Visual direction

- koszula robocza z podwiniętymi rękawami,
- smar pod paznokciami,
- zegarek tańszy niż ambicje,
- papieros za uchem albo ołówek stolarski,
- mówi bez teatralności,
- patrzy częściej na auto niż w oczy, dopóki nie zostanie urażony.

## Anti-patterns

- wesoły sidekick kierowca,
- encyklopedia tutoriali,
- lojalny piesek od upgrade'ów,
- zdrajca z definicji,
- postać bez własnego interesu w warsztacie.

