# 10. Reference stack

Ten dokument nie jest listą rzeczy do skopiowania. To filtr inspiracji: co warto ukraść projektowo, czego unikać i jakie źródła sprawdzić przy dalszych decyzjach.

## Gry i lekcje projektowe

### Mafia III

Brać:

- mocną tożsamość miasta, epoki i muzyki,
- poczucie społecznego napięcia,
- przejmowanie struktur przestępczych,
- styl misji osadzonych w konkretnych dzielnicach.

Unikać:

- powtarzalności dzielnicowych aktywności,
- przejęć, które po czasie są tylko checklistą.

### Ghost Recon Wildlands

Brać:

- kartel jako strukturę ludzi, regionów i zależności,
- czytelność hierarchii celów,
- swobodę podejścia do operacji.

Unikać:

- sprowadzenia kartelu do listy bossów do odstrzelenia,
- świata, który jest efektowny, ale nie pamięta decyzji gracza.

### Scarface: The World Is Yours

Brać:

- fantazję odbudowy/przejmowania imperium,
- dirty money jako ryzyko,
- heat,
- biznesy i terytoria powiązane z akcją TPP.

Unikać:

- czystej power fantasy bez mocniejszego kosztu,
- ekonomii, która robi się arcade'ową liczbą.

### Cartel Tycoon

Brać:

- fronty,
- poruczników,
- rywali,
- państwo,
- opinię publiczną,
- logistykę i pranie pieniędzy.

Unikać:

- strategii ukrytej wyłącznie w menu,
- sytuacji, gdzie kamera TPP nie pokazuje skutków decyzji.

### Narcos: Rise of the Cartels

Brać:

- sezonową strukturę konfliktu,
- możliwość myślenia o konflikcie z kilku perspektyw.

Unikać:

- wiary, że temat albo licencjopodobny klimat sam uniosą gameplay.

## Filmy, seriale i ton

Brać:

- napięcie polityki, mediów, policji i rodziny,
- zdrady wynikające z ambicji i strachu,
- kontrast publicznego patrona i prywatnego tyrana,
- powolne odbieranie graczowi komfortu.

Unikać:

- ciągłego moralizowania,
- gloryfikacji bez konsekwencji,
- scen, które tylko pokazują, że bohater jest “cool”.

## Ryzyko realnego nazwiska

Projektowo lepsza jest fikcja z czytelnym archetypem. Prawdziwe nazwisko daje rozpoznawalność, ale zabiera wolność fabularną i podnosi ryzyko wizerunkowe.

Warto pamiętać o kontekście prawnym i społecznym: w 2024 r. Sąd UE podtrzymał odmowę rejestracji znaku towarowego “Pablo Escobar”, wskazując, że odbiorcy kojarzą tę nazwę z handlem narkotykami, narcoterroryzmem, zbrodniami i cierpieniem. To nie jest porada prawna, tylko argument za fikcyjnym bohaterem i światem.

Źródło: https://curia.europa.eu/jcms/upload/docs/application/pdf/2024-04/cp240067en.pdf

## Unity 6 URP: źródła techniczne

Te funkcje są kandydatami do testów wydajnościowych, nie obietnicami.

- GPU Resident Drawer w URP: Unity opisuje użycie BatchRendererGroup i GPU instancingu do redukcji draw calli i czasu CPU. Wymagania obejmują m.in. Forward+, Mesh Renderery i platformy wspierające compute shadery. Źródło: https://docs.unity3d.com/6000.0/Documentation/Manual/urp/gpu-resident-drawer.html
- STP / Spatial-Temporal Post-Processing: Unity opisuje STP jako software'owy upscaler używający technik spatial i temporal, wymagający compute shaderów i TAA. Źródło: https://docs.unity3d.com/6000.2/Documentation/Manual/urp/stp/stp-upscaler.html
- Render Graph w Unity 6 URP: Unity opisuje Render Graph jako system poprawiający customizację pipeline'u i zmniejszający użycie pamięci URP. Źródło: https://docs.unity3d.com/6000.1/Documentation/Manual/WhatsNewUnity6.html

