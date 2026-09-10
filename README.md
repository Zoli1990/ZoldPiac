# ZoldPiac – fejlesztési és üzemeltetési dokumentáció

Ez a repository a **Piac / RekeszApp projekt továbbfejlesztett, többfelhasználós változata**.

A `ZoldPiac` önálló fejlesztési és tesztelési ágként működik, saját frontenddel, backenddel és adatbázissal. A projekt célja, hogy a korábbi, egyfelhasználós működésből egy valóban több ügyfél által használható webes szolgáltatás alakuljon ki.

> **Fontos:** ez a dokumentum az aktuális `ZoldPiac` projekt állapotát és a későbbi fejlesztések üzleti/technikai irányát rögzíti. A korábbi `Piac` repository már nem a fejlesztés alapja.

---

## 1. Projektcél

A rendszer egy piaci / zöldség-kereskedelmi működéshez készült webalkalmazás.

Fő funkciói:

- felvásárlások rögzítése;
- saját termés rögzítése;
- eladások rögzítése;
- rekeszek mozgásának és tartozásának követése;
- kocsi- és raktárkészlet kezelése;
- eladók és vevők nyilvántartása;
- egyenlegek és tartozások áttekintése;
- forgalmi / könyvelési riportok;
- zöldségekhez kapcsolódó képek kezelése.

A rendszer nem általános vállalatirányítási szoftver. A fejlesztés elsődleges célja a tényleges piaci használat egyszerű, gyors és megbízható támogatása.

---

## 2. Technológiai felépítés

### Backend

- ASP.NET Core / .NET 8 Web API
- Entity Framework Core
- MySQL / MariaDB kompatibilis adatbázis
- Pomelo Entity Framework Core MySQL provider
- JWT alapú autentikáció
- BCrypt jelszóhash-elés
- REST API
- ImageSharp képfeldolgozás

### Frontend

- Vue 3
- Vite
- Pinia
- Vue Router
- mobil / iPad / asztali használatra optimalizált felület

### Üzemeltetés

A `ZoldPiac` külön infrastruktúrán fut a korábbi rendszerhez képest:

- külön frontend URL;
- külön backend URL;
- külön adatbázis;
- külön GitHub repository.

A régi rendszer adatai és működése nem része ennek a repositorynak.

---

# 3. Aktuális fejlesztési fázis – többfelhasználós tesztverzió

A projekt jelenleg a **nyíltabb felhasználói tesztelés előtti / alatti fejlesztési szakaszban** van.

A cél, hogy néhány korábbi érdeklődő mellett más érdeklődők is önállóan regisztrálhassanak, kipróbálhassák a rendszert, és továbbajánlhassák azt.

A jelenlegi fejlesztési irány:

1. `admin/admin` belépés megszüntetése.
2. Email-címes regisztráció bevezetése.
3. Emailes visszaigazolás bevezetése.
4. Minden regisztrált ügyfél saját adatterületének kialakítása.
5. A backendben kötelező felhasználói adatizoláció.
6. Képek felhasználónként elkülönített tárolása.
7. Képek automatikus átméretezése és tömörítése.
8. ÁSZF és adatkezelési dokumentáció kialakítása.
9. Lokális és éles tesztelés.

---

# 4. Felhasználói és jogosultsági modell

## 4.1. Nincs többé `admin/admin`

A korábbi fejlesztési modellben az alkalmazás egy előre létrehozott `admin` felhasználóval indult.

Ez a publikusabb tesztverzióban megszűnik.

Nem lesz előre létrehozott közös `admin/admin` fiók.

A felhasználók saját email-címmel regisztrálnak.

## 4.2. Regisztráció

A tervezett folyamat:

```text
email + jelszó
      ↓
regisztráció
      ↓
emailes visszaigazolás
      ↓
fiók aktiválása
      ↓
bejelentkezés
      ↓
alkalmazás használata
```

A regisztráció minden érdeklődő számára nyitott.

## 4.3. Jogosultság

A jelenlegi alkalmazási jogosultsági szint marad az alapértelmezett jogosultság minden regisztrált ügyfél számára.

Nincs szükség külön, alkalmazáson belüli üzemeltetői adminisztrátori szerepre.

A szolgáltatás üzemeltetője a rendszer működését közvetlenül a szerver / adatbázis oldaláról követi.

---

# 5. Többfelhasználós adatmodell

A többfelhasználós működés alapelve:

> **Minden üzleti adat egy konkrét felhasználóhoz tartozik.**

A felhasználó saját adatai nem keveredhetnek más felhasználók adataival.

Ennek megfelelően a fő üzleti entitásoknak felhasználói tulajdonosi kapcsolattal kell rendelkezniük, például:

```text
User
 └── UserId

Partner
 └── UserId

Vevo
 └── UserId

Zoldseg
 └── UserId

RekeszTipus
 └── UserId

FelvasarlasTetel
 └── UserId

EladasTetel
 └── UserId
```

A pontos migrációs megvalósítást az aktuális adatmodellhez kell igazítani.

## 5.1. Backend oldali adatizoláció

A felhasználó azonosítása nem a frontend által beküldött `UserId`-ra épülhet.

A backendnek a hitelesített felhasználóból kell meghatároznia az aktuális `UserId`-t, majd minden lekérdezésnél és módosításnál ezt azonosítóként használnia.

Például:

```text
JWT UserId = 17

rekord UserId = 17
→ engedélyezett

rekord UserId = 18
→ nem hozzáférhető
```

Ez a rendszer egyik legfontosabb biztonsági követelménye.

---

# 6. Adatbázis és migrationök

A repositoryban az Entity Framework Core migrationök verziózott adatbázis-változásokat tartalmaznak.

A jelenlegi migration-sorozat:

```text
20260824060000_InitialCreate
        ↓
20260831120000_EladasHianyFelvasarlasKiegeszites
        ↓
20260902201337_RaktarHelyszin
```

A `AppDbContextModelSnapshot.cs` a jelenlegi modell állapotát követi.

## Fontos szabály

A már elkészített migrationöket nem írjuk visszamenőlegesen át csak azért, mert a modell tovább fejlődik.

Az új adatmodellhez új migration készül.

Ez biztosítja, hogy az adatbázis fejlődése követhető maradjon.

## Startup migration

A backend indításakor a rendszer jelenleg automatikusan megpróbálja alkalmazni a függőben lévő migrationöket.

A seedelés külön `DbInitializer` feladata.

A publikus többfelhasználós verzióban az automatikus `admin/admin` seed megszűnik.

---

# 7. Felvásárlás és készletmodell

## 7.1. Felvásárlás

Egy felvásárlási tétel egy konkrét rögzített tranzakció.

Fontos adatai:

- dátum;
- eladó / partner;
- zöldség;
- rekesztípus;
- mennyiség;
- egységár;
- fizetve állapot;
- adott üres rekesz;
- helyszín;
- saját termés jelölése;
- megjegyzés.

### Vásárolt áru

Vásárolt árunál az egységár kötelező.

### Saját termés

Saját termésnél nincs partner. Az egységár opcionális, korábbi döntés szerint becsült önköltségként használható ott, ahol erre szükség van.

### Adott üres rekesz

Az adott üres rekeszek száma nem lehet nagyobb a felvásárolt mennyiségnél.

Alapértéke 0.

A mennyiség megváltoztatása nem írhatja át automatikusan az adott rekeszek számát.

---

# 8. Készletmodell – nincs FIFO

A rendszerben nincs automatikus FIFO készletkezelés.

Nem vezetünk be rejtett készletforrás-allokációt sem.

Az `EladasTetel` nem tartja nyilván, hogy az adott eladás pontosan melyik `FelvasarlasTetel` rekordból fogyott.

Ez szándékos üzleti döntés.

## 8.1. Készletcsoportosítás

A készlet elsődleges csoportosítási kulcsa:

**zöldség + rekesztípus**

Nem része a készletcsoportosításnak:

- partner / eladó;
- dátum;
- felvásárlási ár.

Példa:

```text
Alma M10 – 20 db
Alma M10 – 15 db

=> Alma M10 készlet: 35 db
```

Az adatbázisban az eredeti felvásárlási rekordok továbbra is különálló tételek maradnak.

## 8.2. Átlagos vételár

A készletnézetben nem számolunk becsült vagy súlyozott átlag vételárat.

Ennek oka, hogy a forrásallokáció hiányában ez nem lenne egzakt.

Ha a jövőben önköltség- vagy árrésszámításra szükség lesz, az külön fejlesztési és üzleti döntési feladat.

---

# 9. Raktár és kocsi

A rendszer két fő helyszínt kezel:

- `Kocsi`
- `Raktar`

A felvásárlási tételhez tartozó `Helyszin` jelzi a fizikai helyet.

Raktári tételnél az `AthelyezveDb` mutatja, hogy az adott eredeti sorból mennyi került át a kocsira.

```text
raktáron maradt mennyiség = Mennyiseg - AthelyezveDb
```

## 9.1. Raktár → kocsi

A csoportosított készletkártya nem önálló adatbázisrekord.

Backend művelethez mindig egy valódi felvásárlási tétel ID-ját kell használni.

A csoportosítás kizárólag megjelenítési segítség; nem hozunk létre szintetikus csoport-ID-t.

---

# 10. Egyenleg és rekesztartozás

## 10.1. Mi tartozunk – eladóknak

A felvásárlási tételek alapján számítható.

Pénzbeli tartozás csak a nem fizetett tételekből számítódik.

A rekesztartozásnál a felvásárláskor kapott és adott rekeszek különbsége számít.

## 10.2. Nekünk tartoznak – vevők

Az eladási tételekből számítódik.

Pénztartozás:

```text
mennyiség × egységár
```

csak a nem fizetett tételeknél.

Rekesztartozás:

```text
elvitt mennyiség
- visszahozott rekesz
- kifizetett hiány
```

A hiány rendezése darabszám alapú.

---

# 11. Képtárolás

A képek a többfelhasználós rendszerben nem kerülhetnek minden felhasználót közös könyvtárba.

A tervezett könyvtárstruktúra:

```text
private/
└── images/
    └── users/
        ├── 1/
        ├── 2/
        ├── 3/
        └── ...
```

Minden felhasználó saját könyvtárat kap.

## 11.1. Fájlnév

Nem használjuk tartós fájlazonosítóként a feltöltött kép eredeti nevét.

A szerver generáljon egyedi fájlnevet, például GUID-alapú azonosítót.

Ez megakadályozza az olyan ütközéseket, mint:

```text
users/1/burgonya.jpg
users/2/burgonya.jpg
```

és ugyanazon felhasználón belül sem fordulhat elő véletlen felülírás az eredeti fájlnév miatt.

## 11.2. Képátméretezés és tömörítés

A jelenlegi 5 MB-os feltöltési korlát csak a **bemeneti fájl maximális méretét** jelentse.

A rendszer a feltöltött képet feldolgozza:

```text
mobiltelefon / eredeti fotó
        ↓
fájlméret-ellenőrzés
        ↓
kép dekódolása
        ↓
állandó maximális képméret
        ↓
JPEG / WebP tömörítés
        ↓
tárolás
```

A ténylegesen tárolt képnek jóval kisebbnek kell lennie az eredeti feltöltésnél.

Mivel a képek az alkalmazásban kisméretű kártyaképként jelennek meg, nincs szükség több megabájtos, teljes felbontású eredetik tárolására.

A cél a szemmel érzékelhető minőség megőrzése mellett a lehető legkisebb fájlméret.

## 11.3. Képreferencia az adatbázisban

A `Zoldseg.KepUrl` / képhivatkozás ne tartalmazzon környezethez kötött abszolút URL-t.

Előnyben részesítendő egy relatív fájlreferencia, például:

```text
17/8c1f...webp
```

Így később a fájltárolás helye módosítható anélkül, hogy az adatbázisban minden rekordot át kellene írni.

---

# 12. Biztonsági alapelvek

A publikus tesztverzióban különösen fontos:

- jelszavak soha nem kerülhetnek plaintext formában az adatbázisba;
- a JWT signing key és a connection string nem kerülhet GitHubra;
- a konfigurációs titkok environment variable-ból vagy lokális konfigurációból származzanak;
- minden felhasználó csak a saját adatait érhesse el;
- frontend által küldött UserId nem tekinthető megbízható jogosultsági információnak;
- feltöltött fájloknál fájltípus- és méretellenőrzés szükséges;
- a fájlneveket a szerver generálja;
- publikus repositoryba adatbázismentés, jelszó, token, kulcs vagy egyéb titok nem kerülhet.

---

# 13. Jogi és szolgáltatási keretek

A `ZoldPiac` jelenlegi célja egy ingyenesen kipróbálható szolgáltatás biztosítása.

A regisztráció nyitott, a szolgáltatás kipróbálása minden érdeklődő számára lehetséges.

A tervezett modell szerint a szolgáltatás a jelenlegi tesztidőszakban **előreláthatóan egy évig díjmentesen** vehető igénybe. Ezt követően a szolgáltatás fizetős formában folytatódhat.

A későbbi fizetős működés nem tekintendő garantált jövőbeli feltételnek; annak szükségességéről és feltételeiről a tesztidőszak eredménye alapján születik döntés.

A részletes jogi dokumentációban külön kell kezelni:

- ÁSZF;
- adatkezelési tájékoztató;
- szolgáltatás rendelkezésre állása;
- felhasználói felelősség;
- feltöltött adatok kezelése;
- adatmentés és adatvesztés kockázata;
- a szolgáltatás megszüntetésének / szüneteltetésének lehetősége;
- esetleges későbbi díjfizetés feltételei.

> A jogi dokumentumok végleges szövegét a tényleges üzemeltetési és adatkezelési folyamatokkal összehangolva kell elkészíteni.

---

# 14. Tesztidőszak célja

A jelenlegi időszak nem csak technikai tesztelés, hanem **piaci validáció** is.

A legfontosabb kérdés:

> Van-e elegendő valódi érdeklődés és rendszeres használat ahhoz, hogy érdemes legyen a szolgáltatást később fizetős termékként továbbvinni?

A kezdeti időszakban nem szükséges külön analitikai rendszer.

Az üzemeltető számára alapvető információ lehet például:

```text
5 regisztrált felhasználó
50 regisztrált felhasználó
150 regisztrált felhasználó
```

A felhasználói darabszám az adatbázis `User.Id` értékei és a tényleges rekordok alapján követhető.

Az AUTO_INCREMENT érték önmagában nem feltétlenül egyenlő az aktuális rekordok számával, ha később felhasználói rekordok törlésére kerülne sor.

Később készülhet külön üzemeltetői / elemző frontend, amely részletesebb használati adatokat vizsgál. Ez jelenleg nem része a projekt elsődleges céljának.

---

# 15. Fejlesztési alapelvek

A későbbi fejlesztéseknél ezeket az alapelveket meg kell őrizni:

1. A tényleges üzleti használat fontosabb az elméleti túltervezésnél.
2. Nincs FIFO, hacsak arra külön üzleti döntés nem születik.
3. Nincs rejtett készletforrás-allokáció.
4. A csoportosított frontend-nézet nem jelent összevont adatbázisrekordot.
5. A felhasználói adatizoláció backend oldali követelmény.
6. Egy felhasználó nem láthatja vagy módosíthatja más felhasználó üzleti adatait.
7. A már alkalmazott migrationöket nem írjuk át visszamenőlegesen.
8. Új adatbázisváltozás új migrationnel történik.
9. A képek tárolása felhasználónként elkülönített.
10. A képek méretét és minőségét a tényleges webes megjelenítéshez kell igazítani.
11. Titkok és éles konfiguráció nem kerülhetnek a repositoryba.
12. A lehető legegyszerűbb megoldást választjuk, amely üzletileg és technikailag korrekt.

---

# 16. Következő fejlesztési sorrend

A jelenlegi szakaszban a javasolt végrehajtási sorrend:

```text
README frissítése
       ↓
User / regisztrációs modell
       ↓
emailes visszaigazolás
       ↓
admin/admin megszüntetése
       ↓
UserId alapú adatizoláció
       ↓
új EF migration
       ↓
képtárolás felhasználónként
       ↓
képátméretezés + tömörítés
       ↓
ÁSZF / adatkezelési dokumentáció
       ↓
lokális teljes teszt
       ↓
éles tesztverzió
```

A következő fejlesztéseknek mindig a jelen dokumentumban rögzített multi-user működésből kell kiindulniuk.
