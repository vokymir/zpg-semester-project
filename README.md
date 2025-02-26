# ZPG Semester project

# Assignment (copy and pasted)
 Informace o samostatné práci

Semestrální práce je myšlenkově poměrně jednoduchá, ale začátečníci se jistě potrápí se spoustou záludností 3D grafiky. Navíc je implementace časově náročná, proto je vhodné začít programovat co nejdříve.

Práce má dvě části, povinnou a volitelnou. Povinná část obsahuje základní funkcionalitu a je skutečně povinná. Jinými slovy, nebude-li finální produkt splňovat povinnou část, nebude v žádném případě udělen zápočet. Základní část se odevzdává přibližně v polovině semestru (viz harmonogram cvičení), hodnotí se nejvýše 20 body. Pokud budou nějaké body strženy, znamená to, že povinná část nefunguje tak, jak by měla. Z výše uvedeného plyne, že do konce semestru musí být nedostatky odstraněné. Aby nebylo bodování tak tvrdé, polovinu sebraných bodů vrátíme.

V prvním odevzdání prosím odevzdávejte pouze povinnou část. Jakákoliv dodatečná funkcionalita se bude hodnotit až při druhém odevzdání.

Volitelná část slouží k získání dalších bodů. Je možné vybrat si z řady lehčích i těžších úloh, dle osobního vkusu. Druhá část se odevzdává na konci semestru (viz časový harmonogram cvičení). Tento termín je deadline! Jinými slovy, po tomto datu se další odevzdání nepřijímají!

Protože není tato filosofie na naší škole tak obvyklá, uvedeme si příklad z praxe. Šéf chce prezentovat novou hru (výrobek vaší softwarové firmy) potenciálním investorům. Na prezentaci tedy potřebuje mít funkční demo. Show proběhne řekneme 14. prosince večer a proběhne za jakýchkoliv okolností - investoři jsou pozvaní, je zaplacený sál, ... Pokud tedy bude demo hotové v super kvalitě 15. prosince, není to nic platné - prezentace už proběhla, je po všem. Zkrátka, 14. prosince musí být něco hotovo, tečka. V rámci přípravy do profesionální kariéry je dobré si něco takového zkusit. V našem případě - semestrální práce striktně do termínu.
Bodové ztráty

Pokud při druhém odevzdání nebude program splňovat základní funkčnost, tj. povinnou část, bude vrácen k přepracování. Toto vrácení bude postiženo ztrátou 15 bodů. Pokud nebude fungovat ani přepracovaná verze, dáme ještě jednu šanci; tentokrát bude bodový postih již 25 bodů.
Zadání
Implementace

Implementace bude provedena v prostředí .NET, standardně v prostředí OpenGL pomocí knihovny OpenTK.
Povinná část - nutná podmínka udělení zápočtu

    jednopatrové bludiště tvořené pravoúhlou sítí kostek (viz Wolfenstein 3D)

    interaktivní průchod bludištěm
        kolize se stěnami
            kolize se stěnou nesmí způsobovat zaseknutí či poskočení (viz Wolfenstein 3D)
            mapu není možné opustit, byť chybí zeď, která by opuštění bránila není možné vstoupit ani nahlédnout do zdi
        pozorovatel se pohybuje rovnoměrnou rychlostí vzhledem ke skutečnému času (viz níže)
            jednotlivé pohyby vpřed a vzad lze libovolně kombinovat s úkroky do stran, vzniká tak vektor pohybu, který učuje směr, nikoliv rychlost, tzn. hráč se stále pohybuje konstantní rychlostí.
        rychlost otáčení pohledu myší je závislá pouze na rychlosti pohybu myši
            pozorovatel se nemůže přetočit hlavou dolů, tj. natáčení ve vertikálním směru lze pouze v rozsahu (−90°; +90°), kde 0° je přímo vpřed

    veškeré animace a pohyb pozorovatele jsou závislé na reálném čase, nikoliv na snímkovém kmitočtu, tj. změna doby nutné na zobrazení jednoho snímku nesmí způsobit změnu rychlosti pohybu či animace v zobrazovaném světě

    scéna osvětlena reflektorem ze svítilny držené pozorovatelem ve výšce 2,05 m a směrovém vektoru o depresi 2°; úhel u kořene kuželu je menší, než FOV pozorovatele

    zobrazení počítadla snímků za vteřinu (FPS) -výpočet počtu snímků za předchozí vteřinu počínaje aktuálním snímkem, tj. počet předchozích snímků kteréžto se vejdou do okénka o velikosti jedné vteřiny

    geometrie a údaje
        1 jednotka = 1 metr
        rozměry kostky
            šířka: 2 m
            hloubka: 2 m
            výška: 3 m
        pozorovatel
            výška očí: 1,70 m
            průměrná rychlost chůze: 1,4 m/s
            hmotnost hráče: 80 kg

    vstup: soubor s mapou patra
        formát: textový soubor
            první řádka: WxH (např.: 32x40 = mapa 32 sloupců široká a 40 řádků vysoká)
            1 znak = 1 kostka mapy
        mapa uložená po řádcích (W sloupců; min. H řádek)
        význam znaků pevně stanoven
        vlastní znaky povoleny, ale pouze v definovaném rozsahu (viz tabulka)
            není-li v aplikaci daný znak využit, pak je dekódován dle významu skupiny znaků do níž náleží nebo jako volný prostor, např.
                je-li nalezen znak 'f' (druh zdi) a aplikace nerozlišuje druhy zdí, pak je tento znak považován za zeď podobně jako znaky 'a', 'b', ..., 'n'.
                v případě, že je nalezen znak 'O' (cizí postava) a není řešena úloha Protivník, pak je tento znak interpretován jako volný prostor dle znaku 'o'.
                je-li nalezen znak '|' (volné užití), pak je tento interpretován jako volný prostor.

znak 	význam
mezera, 'a' až 'n' 	volný prostor
'o' až 'z' 	zeď (obecně neprůchodné pole)
'@' 	startovní pozice pozorovatele. Vždy právě jeden na celé mapě.
'*', '^', '!' 	světla
'A' až 'G' 	dveře a vstupy tajných chodeb
'H' až 'N' 	pevné objekty
'O' až 'R' 	cizí postavy (protivníci)
'T' až 'Z' 	předměty určené ke sběru
zbylé znaky (ASCII 32 až 127) 	volné použití

    ovládání
        pouze standardní vstupní zařízení

vstup 	význam
myš 	nahlížení vlevo/vpravo, vzhůru/dolů (pohyb myši vpřed = nahoru)
klávesa 'W' 	pohyb kupředu ve směru pohledu
klávesa 'S' 	pohyb vzad
klávesa 'A' 	pohyb vlevo (nikoliv otáčení)
klávesa 'D' 	pohyb vpravo (nikoliv otáčení)
klávesa 'E' 	otevření dveří/stisk tlačítka

    ostatní
        dle potřeb, ale nutno popsat v dokumentaci
        pouze standardní vstupní zařízení (101 tlačítková klávesnice a 3 tlačítková myš s kolečkem)

Variabilní část

rozšíření povinné části dle výběru studenta z předložených možností.

    grafický vzhled a efekty

        drátěný model (2 b) Zobrazení drátěného modelu scény s respekotováním viditelnosti jendotlivých hran. Čárový mód nesmí ovlivnit zobrazení UI (User Interface) a HUD (Heads Up Display).

        model (5 b)
        Vytvoření funkce/struktury pro načtení a uložení připraveného modelu (OBJ nebo Collada formát) pro daný element mapy (zeď, strop, podlaha) nebo objekt (protivník, sbírané předměty). Model musí obsahovat minimálně 2 různé materiály a při zobrazování je nutné využití pole vrcholů (VertexPointer/VertexBuffer) nebo vertex buffer (GL_ARB_vertex_buffer_object).

        LOD modelů (5 b) Implementace LOD pro modely. Min. 3 úrovně detailů. Změny LOD nesmí být příliš patrné pozorovateli.

        Skinned mesh (15 bodů) Model je popsaný jednoduchou kostrou, trojúhelníková síť se pro daný frame algoritmicky deformuje podle polohy kostry.

        textura (5 b) Načtení a aplikace textury s visuálně nejlepším možným způsobem vzorkování textury dostupným na daném HW. Úloha předpokládá vytvoření autodetekce pro detekováni nejlepšího přístupu jaký je daným HW podporován: nearest-neighbor, linear, anisotropic, nearest-neighbor mip-map, linear mip-map (též trilineární filtrování), atd.

        animovaná textura (5 b) Animace textury (posunující se obraz, žhnoucí láva, ...). Animace musí být tvořena změnou obrazu, nikoliv jen posunutím texturovacích souřadnic, apod.

        mlha (10 b) Mlha při podlaze a pouze ve vybraných částech mapy. Lze implementovat pomocí systému částic nebo billboardy.

        vodní stěna (15 b) Poloprůhledná krychle deformující oblast viditelnou skrze ni.

        zrcadlo (10b) Vytvoření iluze rovinného zrcadla na stěně (celá stěna nebo část) nebo na podlaze (lesklá podlaha). Pouze jedna úroveň zrcadlení: zrcadlené zrcadlovité stěny se jeví jako bílé. Zrcadlí se všechny objekty, nejen geometrie.

        chrom a lesklé objekty (15 b) Generování CubeMap pro vybrané objekty a její následné využití pro navození dojmu chromovaného objektu. Nejsou-li přítomna světla ve scéně (kromě světla hráče), pak je scéna osvětlena ze středu objektu. V případě animací: automatické znovuvytvoření CubeMap v případě, že změna (animace) se provádí do určité vzdálenosti od objektu. Ostatní lesklé objekty při generování CubeMapy se jeví jako bílé.

        průhlednost (10 b) Správné zobrazení průhledných krychlí (stěn). Aplikace malířova algoritmu bez sledování částečného překrytu polygonů.

        zaostření (15 b) Implementace Depth of View se zaostřením na objekt ve středu zorného pole.

    hra

        sběr předmětů (5 b) V mapě rozloženy předměty. Hráč předměty sbírá a počítá se počet sebraných předmětů, který je zobrazen na obrazovce. Předměty poskakují, točí se na místě, popř. provádějí nějaký jiný zajímavý pohyb. Rychlost pohybu předmětů je nezávislá na snímkovém kmitočtu.

        teleport (5 b) Teleportace z místa na místo. Implementace efektu pro hráče: přechod obrazu do bíla/z bílé do cílové scény.

    osvětlení

        sluníčko (5 b) Přidání osvětlení směrovým světlem (slunce, globální osvětlení) produkující phongovo stínování s využitím předpočítané cube-mapy.

        světlo I (5 b) Implementace dynamického osvětlení bodovými (reflektor) zdroji s omezeným dosahem. Počet světel je větší, než zvládne HW. Světla, která nezasahují do renderovaného výhledu nejsou aktivní. Při aplikaci světel se vynechají světla od nevzdálenějších k nejbližším tak, aby zároveň bylo aktivních pouze 6 světel bez světla z pohledu hráče.
            omezení dosahu (10 b) Rozšíření úlohy 'světlo I- a 'stín' o omezení dosahu světla. Jedná se o zabránění situaci, kdy světlo svítí přes zeď: jedná se tedy o výpočet viditelnosti z pohledu světla na úrovní jednotlivých kostek (tj. která kostka je světlem zasažena a která nikoliv).

        světlo II (15 b) Vytvoření light-mapy emulující osvětlení prostoru pro danou mapu a následné užití. Pro vytváření užijte offline renderer (PovRay, YafRay, atd.) a jednoduchý skript/aplikaci, která vytvoří scénu a nechá narenderovat jednotlivé stěny viditelných kostek pomocí vhodného nastavení kamery. Vytvořené obrazy nasvícení stěn rozložte do textury a užijte při vykreslování jakou druhou texturu. Respektujte možné omezení maximální velikosti textury, předpokládejte, že min. velikost textury je 256×256.

        svítilna (5 b) Implementace svítilny pomocí projekčního mapování textury.

        stín I(15 b) Rozšíření úlohy 'svítilna' o generování stínu technikou shadow-map.

        stín II (20 b) Rozšíření úlohy 'svítilna' o generování stínu s využitím stencil buffer.

    cizí postavy a jevy

        částice (10 b) Otexturované částice (particles: sršení, pršení, kouř, ...) a jednoduchou fyzikou bez řešení kolizí s objekty ve scéně. Jednotlivé částice budou implementovány jako bilboard.

        protivník (10 b) Realizace protivníka v podobě billboardu (sprite) neschopného procházet zdmi a dveřmi. Protivník je animován. Postačuje AI v podobě beží k hráči. Postačuje 1 protivník / 1 políčko. Rozšíření pokrývá i rozšíření animovaná textura, viz výše (celkem tedy 20 b).

        protivník II (10 b) Rozšíření úlohy Protivník o interakci avatara s protivníkem a protivníků navzájem. Předpokládá se střelba, alterantivní možnosti (chytání motýlů) nutno konzultovat s cvičícím.

    mapa

        dveře a tajné chodby (5 b) Animace otvírání + interakce s hráčem (otevřeno/zavřeno/mění stav). Postačuje animace na úrovni Wolfenstein 3D.

        vícepatrové bludiště (15 b) Doplnění podpory pro více pater včetně kolize. Patra definována jako matice n×m×k. Implementace musí umožnovat vytvořit vícepatrové volné prostory a průlezy, přes které je možné nahlížet do dalších pater. V bludišti se musí nacházet místa kde je možno plynule propadnout do nižšího patra.

    avatar, uživatelské rozhranní a pomoc hráči

        minimapa (5 b) Vykreslení zmenšené mapy blízkého okolí hráče do levého horního rohu. Ukazatel ukazuje směrem k vrcholu obrazu a mapa se natáčí dle otočení hráče. U vícepatrového bludiště se zobrazuje pouze aktuální patro a průlezy/propadlište.

        dynamika avatara (hráče) (5 b) Pohupování při chůzi a setrvačnosti hráče při pohybech. Při implementaci vycházejte z definovaných parametrů hráče a použijte základní fyzikální vztahy.

        cesta (10 b) Rozšíření úlohy minimapa. V mapě vyznačeny významné body. Na požádání nalezne aplikace cestu k vybranému bodu a zobrazí ji hráči jako blikající šipky na zdech (nikoliv podlaze).

        cesta II(10 b) Rozšíření úlohy minimapa. V mapě vyznačeny významné body. Na požádání nalezne aplikace cestu k vybranému bodu a zobrazí démona, bludičku nebo sukubu, které hráče na dané místo odvede. Odvádějící bytost musí na hráče čekat, pokud ji nenásleduje, a vizuálně vyjadřovat svoji netrpělivost.

    výstup

        snapshot (až 20 b) Export aktuální konfigurace scény do formátu popisu scény pro Povray 3.6.0 nebo Rendermana 3.1. Export bude obsahovat všechny ostatní implementované variace (efekty) vyjma variace této. Efekty je možné implementovat prostředky Povraye/Rendermana (týká se především materiálů a osvětlení). Skutečný počet bodů se odvíjí od počtu vyvážených efektů.

        stereo (10 b) Doplnění podpory pro stereo stěnu (dual-headový výstup) a anaglyfické brýle nebo shutter glasses a anaglyfické brýle včetně možnosti konfigurace vzdálenosti uživatele od promítací plochy. Pouze pro omezený počet účastníků: nutno nahlásit předem.

        trackování uživatele (15 b) S využitím kamery nebo eyetrackeru bude sledována pozice uživatele a na jejím základě se bude měnit ořezová pyramida tak, aby pohled odpovídal "pohledu skrze okno" monitoru.

    ostatní
        konzole (5 b) Jednoduchá poloprůhledná konzole obvyklá z počítačových her. Minimálně 3 příkazy s dopadem na scénu. Za vzor je považována konzole ze hry Quake III.

Prostředky

    programovací jazyky: C#
    grafické rozhranní: OpanGL v 3.3
        v případě potřeby je možné se domluvit na vyšší verzi

Dokumentace

    povoleny pouze formáty PDF
        jméno, datum, e-mail, studijní číslo
        popis řešení kolizí

    seznam implementovaných variací a popis postupu/funkcionality variace

    dokumentace stručná a k věci

    budeme rádi, pokud na začátek dokumentace vložíte následující odstavec: Souhlasím s vystavením této semestrální práce na stránkách katedry informatiky a výpočetní techniky a jejímu využití pro prezentaci pracoviště.

Náležitosti odevzdání

    zdrojové texty + makefile/projekt (musí být přeložitelné na první pokus bez úprav) pro prostředí Windows .Net 8+

    spustitelná podoba úlohy bez virů a jiných zvířátek (musí běžet)
        všechna potřebná data na správných (relativních) cestách

    dokumentace

    struktura archivu:
        src
            kompletní zdrojové kódy + projekt tak, aby šlo spustit pomocí dotnet run)
        doc
            pdf/html
        bin
            spustitelný program + všechna potřebná data

