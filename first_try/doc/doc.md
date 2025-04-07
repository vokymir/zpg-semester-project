# 3D bludiště - Semestrální projekt ZPG

Jakub Vokoun, 5. dubna 2025, javok@students.zcu.cz, A23B0235P

## Spuštění

V příkazové řádce ve složce *src* spustit *dotnet run*.
Lze předat prvním parametrem cestu k mapě (relativní vůči aktuální cestě).
Při absenci parametru se načte výchozí mapa ve složce *src/Levels*.

## Kolize

Každý renderovaný objekt ve scéně a kamera má atribut typu *CollisionCube*,
tedy kvádr se středem a třemi rozměry. Při každém pohybu kamery se kamera nejprve
pokusí posunout na žádané místo, pokud by tím ale vznikla kolize, podívá se do
obou horizontálních směrů os X a Z a pokud posunutím v daném směru vzniká
kolize, tak nastaví pohyb v tomto směru na maximální možný, který však nekoliduje.
Tím zároveň vzniká pomalejší pohyb, pokud se hráč *otírá* o stěnu.

## FPS

Počítadlo snímů za vteřinu je umístěné v názvu okna, protože je to jednodušší,
než implementovat zobrazování textu. Počítadlo je vpravdě triviální, totiž při
každém volání funkce *OnRenderFrame()* se k FPS přičte 1 a pomocí systémové
knihovny Timers vytvořený časovač každou vteřinu zobrazí FPS a opět je vynuluje.

## Načítání levelů

Pokud uživatel při spuštění aplikace předá prvním parametrem cestu k souboru
s mapou, pokusí se tuto mapu aplikace načíst. Pokud to nebude možné, aplikace
skončí a vypíše chybu. Pro zabránění hráči opustit mapu i v místech kde není stěna
slouží přidání stěny z krychlí po takových okrajích mapy, kde chybí zeď. Tyto krychle
mají jinou texturu, aby tak indikovaly konec mapy, interakce (sliding) s okrajem
mapy je tedy již implementovaný v kolizích.

## Povolení užití

Souhlasím s vystavením této semestrální práce na stránkách katedry informatiky
a výpočetní techniky a jejímu využití pro prezentaci pracoviště.
Nekontroloval jsem ale autorská práva textur, tuto zodpovědnost přenechávám katedře.
