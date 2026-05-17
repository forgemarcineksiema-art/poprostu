# 18. Minimal World State Model

## Cel

World State ma być jedną prawdą o świecie. Misje, AI, UI, ambient i dialogi nie mogą mieć osobnych wersji tego, kto kontroluje front, jak wysoka jest presja państwa i jak Pablo potraktował porucznika.

Ten dokument opisuje minimalny model dla vertical slice. Nie jest pełną architekturą całej gry. Ma wystarczyć do Fazy 2-5: mikrotestów konsekwencji, prototypu frontu i "Pierwszego Frontu".

## Zasada główna

> Jeśli zmiana nie trafia do World State albo jawnego eventu, nie jest konsekwencją systemową.

Cutscenka może pokazać skutek, ale nie może być jedynym miejscem, gdzie ten skutek istnieje.

## Minimalny model danych

```csharp
public enum FrontControl { Rival, Pablo, PabloWatched, Burned }
public enum DirtyCashState { None, Loose, Carried, Hidden, Laundered, Seized }
public enum PressureLevel { Low, Medium, High }
public enum SocialLevel { Low, Neutral, High }
public enum LieutenantTrust { Humiliated, Professional, Trusted }
public enum RuleStyle { None, Favor, Bribe, Threat, ShowOfForce }

public sealed class SliceWorldState
{
    public string DistrictId = "BarrioHondo";
    public string FrontId = "ElRespiroWorkshop";

    public FrontControl FrontControl = FrontControl.Rival;
    public DirtyCashState DirtyCash = DirtyCashState.None;
    public PressureLevel StatePressure = PressureLevel.Low;
    public SocialLevel PeopleLove = SocialLevel.Neutral;
    public SocialLevel Fear = SocialLevel.Low;
    public LieutenantTrust LieutenantTrust = LieutenantTrust.Professional;
    public RuleStyle RuleStyleDecision = RuleStyle.None;
}
```

## Dlaczego tylko tyle

Ten model nie próbuje symulować całego kartelu. Ma odpowiedzieć na pytania vertical slice:

- kto kontroluje warsztat El Respiro,
- czy kasa jest luźna, niesiona, ukryta, wyprana albo przejęta,
- czy państwo zaczyna patrzeć na Pablo,
- czy dzielnica kocha Pablo, boi się go albo odwraca wzrok,
- czy Mateo ufa Pablo, traktuje go zawodowo albo czuje się upokorzony,
- jakim stylem Pablo przejął wpływ.

Każde dodatkowe pole musi przejść test: czy "Pierwszy Front" bez tego traci czytelny skutek w świecie?

## Eventy

World State powinien być zmieniany przez jawne eventy. Nazwy są robocze, ale intencja jest obowiązkowa.

| Event | Zmienia | Użycie |
| --- | --- | --- |
| `FrontDiscovered` | brak albo metadane celu | gracz poznaje warsztat jako potencjalny front |
| `DirtyCashPickedUp` | `DirtyCash = Carried` | Pablo fizycznie niesie ryzyko |
| `DirtyCashHidden` | `DirtyCash = Hidden` | kasa jest zabezpieczona, ale nielegalna |
| `DirtyCashLaundered` | `DirtyCash = Laundered` | front zaczyna działać jako system |
| `DirtyCashSeized` | `DirtyCash = Seized` | porażka bez restartu |
| `FrontTakenByPablo` | `FrontControl = Pablo` | czyste przejęcie |
| `FrontTakenUnderWatch` | `FrontControl = PabloWatched` | sukces z presją |
| `FrontBurned` | `FrontControl = Burned` | zwycięstwo, które niszczy wartość frontu |
| `PublicViolenceCommitted` | `Fear +`, `PeopleLove -`, `StatePressure +` | przemoc zostawia ślad |
| `BribeAccepted` | `StatePressure - teraz`, hak na przyszłość | Ríos albo inny funkcjonariusz pamięta zależność |
| `FavorCompleted` | `PeopleLove +`, wolniejszy dostęp | dzielnica pomaga, ale tempo spada |
| `MateoProtected` | `LieutenantTrust = Trusted` | porucznik widzi ryzyko podjęte przez Pablo |
| `MateoHumiliated` | `LieutenantTrust = Humiliated` | porucznik pomaga, ale pamięta koszt |
| `RuleStyleChosen` | `RuleStyleDecision = ...` | styl rządzenia zapisany po decyzji |

## Debug overlay

Ładne UI może poczekać. Debug overlay musi powstać wcześniej, bo bez niego łatwo oszukać się dialogiem.

Minimalny widok:

```text
District: BarrioHondo
Front: ElRespiroWorkshop
Control: PabloWatched
DirtyCash: Hidden
StatePressure: Medium
PeopleLove: Low
Fear: High
LieutenantTrust: Humiliated
RuleStyle: ShowOfForce
LastEvent: PublicViolenceCommitted
```

Overlay ma odpowiadać na jedno pytanie: czy świat naprawdę pamięta to, co zrobił gracz?

## Widoczne reakcje

Każda ważna zmiana World State musi mieć minimum jeden efekt poza UI.

| Stan | Efekt w świecie |
| --- | --- |
| `Fear = High` | cywile schodzą z drogi, sklep szybciej się zamyka, świadek milczy |
| `PeopleLove = High` | ktoś ostrzega przed patrolem, cywil zasłania widok policjantowi |
| `StatePressure = High` | patrol stoi bliżej warsztatu, radio mówi o nalocie, łapówka kosztuje więcej |
| `LieutenantTrust = Humiliated` | Mateo mówi chłodniej, spóźnia ostrzeżenie albo wymusza większy udział |
| `LieutenantTrust = Trusted` | Mateo ostrzega wcześniej i bierze na siebie część ryzyka |
| `FrontControl = PabloWatched` | warsztat działa, ale ma policjanta/obserwatora w pobliżu |
| `FrontControl = Burned` | miejsce jest przejęte, ale mniej użyteczne i społecznie toksyczne |
| `DirtyCash = Carried` | gracz czuje ryzyko fizyczne: auto, torba, zatrzymanie, patrol |

## Save/load kierunek

Na etapie slice model może być serializowany prosto, ale zasada jest już docelowa:

- zapisujemy stan świata, nie stan pojedynczych triggerów,
- po wczytaniu scena rekonstruuje widoczne elementy z World State,
- jeśli front jest `PabloWatched`, scena sama ustawia obserwatora/patrol,
- jeśli Mateo jest `Humiliated`, dialog i zachowanie czytają tę wartość,
- jeśli kasa jest `Seized`, nie wolno jej przywracać przez reset sceny.

## Zależności

World State jest czytany przez:

- mission layer,
- ambient NPC,
- patrole,
- dialog barki,
- debug overlay,
- mapa wpływów w przyszłości,
- save/load,
- raport po misji.

World State nie powinien zależeć od:

- prefabów w scenie,
- nazw GameObjectów,
- kolejności cutscenek,
- tymczasowego UI,
- pojedynczych trigger colliderów jako prawdy.

## Anti-patterns

- Osobna flaga `workshopTaken` w misji i osobna flaga `frontOwner` w UI.
- Dialog Mateo ręcznie odpalany po misji bez zmiany `LieutenantTrust`.
- Policja reagująca tylko w scenie, bez zmiany `StatePressure`.
- Kasa jako licznik pieniędzy bez stanu fizycznego ryzyka.
- Restart misji jako jedyny sposób obsługi utraconej kasy.
- Dzielnica po przejęciu różni się tylko ikoną na mapie.

## Warunek sukcesu

Model działa, jeśli można przejść trzy mikrotesty konsekwencji i po każdym:

- debug overlay pokazuje właściwy stan,
- przynajmniej jeden element świata reaguje,
- stan da się zachować i odtworzyć,
- misja nie musi ręcznie udawać konsekwencji.
