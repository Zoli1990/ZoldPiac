# ZoldPiac

Zöldségpiaci nyilvántartó webalkalmazás többfelhasználós működéssel.

A rendszer célja a zöldségtermékek, rekeszek, felvásárlások, eladások, vevők és partnerek egyszerű nyilvántartása.

## Technológiák

### Backend
- .NET 8 WebAPI
- C#
- Entity Framework Core
- Pomelo.EntityFrameworkCore.MySql
- MySQL / MariaDB
- JWT autentikáció
- BCrypt jelszóhash-elés
- CORS
- EF Core Migrations

### Frontend
- Vue 3
- Composition API
- Pinia
- Vue Router
- Axios
- Vite
- i18n
- mobil-first, reszponzív felület

### Adatbázis
- MySQL / MariaDB
- MonsterASP / DatabaseASP

---

# Production

Frontend:

`https://zoldpiac.tryasp.net/`

Backend API:

`https://zoldpiac.runasp.net/`

---

# Többfelhasználós működés

A rendszer többfelhasználós működésre lett kialakítva.

Jelenleg nincs külön adminisztrátori és normál felhasználói funkcionalitás.

Minden regisztrált felhasználó ugyanazt a teljes funkcionalitást kapja.

A felhasználók saját adatai egymástól elkülönítve kezelendők.

Cél:

```text
Felhasználó A
    ↓
saját adatok

Felhasználó B
    ↓
saját adatok
```

A felhasználók ne férhessenek hozzá egymás üzleti adataihoz.

A tesztidőszakban a felhasználói rekordok ellenőrzése phpMyAdmin segítségével történik.

---

# Adatbázis

A production adatbázis MonsterASP / DatabaseASP környezetben fut.

Jelenlegi adatok:

```text
Database: db65395
Server: db65395.public.databaseasp.net
Port: 3306
```

MySQL felhasználónév:

```text
db65395.databaseasp.net
```

A tényleges jelszót sem ebben a README-ben, sem GitHubon, sem commitban nem szabad tárolni.

Példa connection string:

```text
Server=db65395.public.databaseasp.net;
Port=3306;
Database=ADATBAZIS_NEV;
Uid=db65395.databaseasp.net;
Pwd=SAJAT_JELSZO;
SslMode=Preferred;
```

---

# Adatbázis kapcsolat ellenőrzése

A MonsterASP Remote Access funkciója engedélyezve van.

A MonsterASP oldali Remote Check sikeres volt:

```text
OK
Connected in 14 ms
```

A lokális számítógépről a TCP kapcsolat is tesztelve lett:

```powershell
Test-NetConnection db65395.public.databaseasp.net -Port 3306
```

Eredmény:

```text
TcpTestSucceeded : True
```

Ez azt jelenti, hogy a számítógép eléri a MySQL szervert a 3306-os porton.

---

# MySQL és SQL Server közötti különbség

A projekt adatbázisa:

```text
MySQL / MariaDB
```

Nem Microsoft SQL Server.

Ezért SQL Server kapcsolatot vagy SQL Server UDL kapcsolatot nem szabad használni a MySQL kapcsolat tesztelésére.

MySQL kapcsolat teszteléséhez például:

- HeidiSQL
- DBeaver
- MySQL kliens

használható.

## UDL teszt

A Microsoft Data Link / UDL teszt nem megfelelő ehhez a projekthez, mert SQL Server providerrel próbál MySQL adatbázishoz csatlakozni.

A kapott:

```text
Login failed for user 'db65395'
```

hiba ezért nem tekinthető érvényes MySQL kapcsolat-tesztnek.

---

# HeidiSQL kapcsolat

Közvetlen MySQL kapcsolat teszteléséhez HeidiSQL-ben:

```text
Network type:
MySQL (TCP/IP)

Hostname / IP:
db65395.public.databaseasp.net

User:
db65395.databaseasp.net

Password:
MonsterASP jelszó

Port:
3306
```

Első tesztnél az adatbázis mező üresen is hagyható.

A cél annak eldöntése, hogy a MonsterASP által megadott MySQL felhasználónév és jelszó közvetlenül működik-e.

---

# Jelenlegi EF Core kapcsolat

A backendben található:

```text
AppDbContextFactory.cs
```

Ez implementálja:

```csharp
IDesignTimeDbContextFactory<AppDbContext>
```

A factory a konfigurációból tölti be:

```text
ConnectionStrings:DefaultConnection
```

majd MySQL providerrel hozza létre az `AppDbContext` példányt.

A `dotnet ef` parancsok futtatásakor ezt a factoryt használja az EF Core.

---

# Jelenlegi EF hiba

A `dotnet ef` build sikeres.

A MySQL kapcsolatnál azonban authentication hiba jelentkezett:

```text
Access denied for user 'db65395.databaseasp.net'@'84.3.153.75'
(using password: YES)
```

A hibakeresés alapján:

- a projekt buildel
- az EF Core működik
- az `AppDbContextFactory` működik
- a connection string betöltődik
- a MySQL szerver elérhető
- a 3306-os port elérhető
- a MySQL szerver eléri a kapcsolatot
- a MySQL hitelesítés jelenleg nem sikeres

A következő lépés ezért a közvetlen MySQL klienses bejelentkezés.

---

# EF Core Migrationök

A repositoryban jelenleg az alábbi migrationök találhatók:

```text
20260824060000_InitialCreate
20260831120000_EladasHianyFelvasarlasKiegeszites
20260902201337_RaktarHelyszin
20260910233000_MultiUserAccounts
```

A production adatbázisban jelenleg ezek vannak a `__efmigrationshistory` táblában:

```text
20260824060000_InitialCreate
20260831120000_EladasHianyFelvasarlasKiegeszites
20260902201337_RaktarHelyszin
```

A még hiányzó migration:

```text
20260910233000_MultiUserAccounts
```

---

# KRITIKUS: MultiUserAccounts migration

A `20260910233000_MultiUserAccounts` migration jelenlegi változatát alkalmazás előtt át kell vizsgálni.

A migrationben jelenleg szerepel:

```csharp
migrationBuilder.Sql(
    "DELETE FROM `Users` WHERE `Felhasznalonev` = 'admin';"
);
```

Ez production adatbázison nem elfogadható.

A production adatbázis nem üres adatbázis.

Valós adatok vannak benne.

Ezért:

```text
A 20260910233000_MultiUserAccounts migrationt
a jelenlegi formájában NEM SZABAD PRODUCTION ADATBÁZISON LEFUTTATNI.
```

---

# Miért problémás?

A régi adatbázisban létező felhasználó és a hozzá tartozó üzleti adatok megőrzése szükséges.

A régi adatbázis még az egyszemélyes rendszer struktúráját használja.

A migrationnek ezért:

1. meg kell őriznie a régi felhasználót
2. meg kell őriznie a régi adatokat
3. a régi adatokat hozzá kell rendelnie a megfelelő felhasználóhoz
4. létre kell hoznia az új `UserId` kapcsolatokat
5. támogatnia kell az új felhasználókat
6. el kell különítenie a felhasználók adatait
7. nem okozhat adatvesztést

---

# Migration biztonsági szabály

A migration javításáig production adatbázison nem szabad futtatni:

```powershell
dotnet ef database update
```

A migrationt először tesztadatbázison kell ellenőrizni.

Production előtt teljes adatbázis mentést kell készíteni.

---

# DbInitializer

A backendben található:

```text
DbInitializer.cs
```

Jelenlegi működése:

```csharp
public static async Task InitializeAsync(AppDbContext db)
{
    await db.Database.MigrateAsync();
}
```

Ez azt jelenti, hogy az alkalmazás indulásakor az EF Core automatikusan ellenőrzi a migrationöket, és alkalmazza a hiányzókat.

Működés:

```text
API indul
    ↓
AppDbContext létrejön
    ↓
DbInitializer
    ↓
Database.MigrateAsync()
    ↓
hiányzó migrationök alkalmazása
    ↓
API folytatja az indulást
```

Ezért különösen fontos, hogy a productionre kerülő migration biztonságos legyen.

---

# Production adatbázis táblái

A jelenlegi production adatbázis fő táblái:

```text
eladastetelek
felvasarlastetelek
partnerek
rekesztipusok
users
vevek
zoldsegek
__efmigrationshistory
```

---

# Users tábla – régi struktúra

A production adatbázis régi `Users` táblája:

```text
Id
Felhasznalonev
JelszoHash
Role
```

A tényleges production rekordokat migration módosítás előtt ellenőrizni kell.

Felhasználó-ID-t vagy email címet nem szabad találomra feltételezni.

---

# EladasTetel

A régi `eladastetelek` tábla főbb mezői:

```text
Id
NapiSorszam
Datum
Ido
VevoId
ZoldsegId
RekeszTipusId
Mennyiseg
Fizetve
Elvitte
VisszahozottDb
Egysegar
Megjegyzes
HianyFizettDb
```

A többfelhasználós rendszerben ezekhez az adatokhoz felhasználói tulajdonosi kapcsolat szükséges.

---

# Adatmegőrzés

A production adatbázis valódi adatokat tartalmaz.

Minden adatbázis-módosításnál elsődleges szempont:

```text
ADATMEGŐRZÉS
```

Különösen nem veszhetnek el:

- régi felhasználók
- eladások
- felvásárlások
- partnerek
- vevők
- zöldségek
- rekeszadatok
- egyéb üzleti adatok

---

# Backup

Production migration előtt kötelező:

1. teljes adatbázis mentés
2. mentés ellenőrzése
3. csak ezután migration

A mentést lehetőleg külön helyen kell tárolni.

---

# Konfiguráció és titkok

Érzékeny adatok:

- MySQL jelszó
- JWT secret
- SMTP jelszó
- SMTP felhasználónév
- API kulcsok
- production connection string

Ezek nem kerülhetnek GitHub repositoryba.

A repositoryban csak példa / placeholder szerepelhet.

---

# appsettings.json

A konfiguráció logikailag ilyen:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Port=3306;Database=...;Uid=...;Pwd=CHANGE_ME;SslMode=Preferred;"
  },

  "Jwt": {
    "Key": "CHANGE-THIS-TO-A-LONG-RANDOM-SECRET",
    "Issuer": "ZoldPiac",
    "Audience": "ZoldPiac",
    "ExpiryHours": "12"
  },

  "Cors": {
    "Origins": [
      "https://zoldpiac.tryasp.net"
    ]
  },

  "Auth": {
    "FrontendUrl": "https://zoldpiac.tryasp.net"
  },

  "Uploads": {
    "Path": "../private/images/users"
  },

  "Smtp": {
    "Host": "smtp.example.com",
    "Port": "587",
    "EnableSsl": "true",
    "Username": "SMTP_USERNAME",
    "Password": "SMTP_PASSWORD",
    "FromAddress": "noreply@example.com",
    "FromName": "ZoldPiac"
  }
}
```

A production titkokat nem szabad GitHubra feltölteni.

---

# GitIgnore

Különösen fontos a helyi és érzékeny fájlok kizárása:

```gitignore
bin/
obj/

*.user
*.suo
*.userosscache
*.sln.docstates

.vs/
.vscode/

node_modules/
dist/

.env
.env.*
!.env.example

appsettings.Local.json
appsettings.Development.json
appsettings.Production.json

secrets.json
secrets.*.json
```

A pontos `.gitignore`-t a repository tényleges szerkezete alapján kell karbantartani.

---

# Biztonsági figyelmeztetés

Ha valódi adatbázis-jelszó vagy más secret korábban GitHub repositoryba került, annak egyszerű törlése nem elegendő.

A titkot meg kell változtatni.

Ajánlott folyamat:

```text
régi DB jelszó
    ↓
jelszó megváltoztatása a szolgáltatónál
    ↓
új jelszó biztonságos konfigurációba helyezése
    ↓
GitHubban csak placeholder
```

---

# Frontend

A frontend egy egyszerű, használható zöldségpiaci nyilvántartó felület.

## Mobile-first

Minden oldalnak és minden modalnak használhatónak kell lennie mobiltelefonon is.

Ellenőrizni kell:

- listákat
- táblázatokat
- kártyákat
- modalokat
- űrlapokat
- gombokat
- navigációt
- képeket
- menüket
- hosszabb szövegeket
- kisebb kijelzőket

---

# Termékkártyák

A frontendben megjelenő termékképek a csempe/kártya háttérképei.

Cél:

```text
közel 4:3 képarány
```

A képek ne legyenek indokolatlanul magasak vagy keskenyek.

A kártyás elrendezés mobilon is használható legyen.

---

# Saját termék

A „Saját termék” résznél külön követelmény:

A következő gomb nem szükséges:

```text
Fizetve
```

Ezért a „Saját termék” modalból / felületről el kell távolítani.

---

# Backend és frontend kapcsolat

Frontend:

```text
https://zoldpiac.tryasp.net
```

Backend:

```text
https://zoldpiac.runasp.net
```

A backend CORS konfigurációjában a frontend origin szerepel:

```text
https://zoldpiac.tryasp.net
```

---

# JWT

A rendszer JWT alapú autentikációt használ.

Főbb konfigurációs elemek:

```text
Issuer
Audience
ExpiryHours
Key
```

A JWT secret productionben biztonságos konfigurációból származzon.

A secretet nem szabad:

- GitHubra feltölteni
- README-be írni
- frontend kódba beégetni

---

# Fejlesztési folyamat

A projekt módosításánál az ajánlott sorrend:

```text
1. Repository ellenőrzése
        ↓
2. Jelenlegi működés megértése
        ↓
3. Probléma pontos meghatározása
        ↓
4. Módosítás
        ↓
5. Build
        ↓
6. Teszt
        ↓
7. Migration esetén külön DB teszt
        ↓
8. Git commit
        ↓
9. Push
        ↓
10. Production ellenőrzés
```

---

# Adatbázis-módosítás szabály

Minden migration előtt meg kell vizsgálni:

- milyen táblát módosít
- milyen oszlopokat ad hozzá
- milyen oszlopokat töröl
- van-e adatvesztési lehetőség
- milyen foreign key-k jönnek létre
- nullable-e az új mező
- mi történik a meglévő rekordokkal
- hogyan kerülnek hozzárendelésre a régi rekordok
- hogyan működik üres és már feltöltött adatbázison

---

# Migration tesztelés

Migrationt kétféle adatbázison kell ellenőrizni.

## 1. Üres adatbázis

Ellenőrizni kell, hogy az összes migration egymás után hibamentesen létrehozza-e az aktuális sémát.

## 2. Meglévő adatokkal rendelkező adatbázis

Ellenőrizni kell:

```text
régi séma
+
régi adatok
+
új migration
```

után:

- minden régi adat megmarad-e
- a régi felhasználó megmarad-e
- minden rekord megfelelő UserId-t kap-e
- a foreign key-k megfelelőek-e
- az új felhasználók működnek-e

---

# Feladatlista

## Adatbázis kapcsolat

- [x] MonsterASP Remote Access engedélyezve
- [x] MonsterASP Remote Check sikeres
- [x] TCP 3306 kapcsolat sikeres
- [x] EF Core build sikeres
- [x] AppDbContextFactory működik
- [x] Connection string betöltődik
- [ ] közvetlen MySQL klienses bejelentkezés
- [ ] MonsterASP pontos connection string ellenőrzése
- [ ] szükség esetén DB jelszó cseréje

## Multi-user migration

- [ ] `20260910233000_MultiUserAccounts` teljes átvizsgálása
- [ ] `User` model ellenőrzése
- [ ] `EladasTetel` model ellenőrzése
- [ ] `FelvasarlasTetel` model ellenőrzése
- [ ] többi érintett model ellenőrzése
- [ ] UserId kapcsolatok ellenőrzése
- [ ] régi user megőrzése
- [ ] régi adatok hozzárendelése
- [ ] admin törlésének eltávolítása
- [ ] migration adatvesztés elleni javítása
- [ ] teszt migration
- [ ] production backup
- [ ] production migration

## Multi-user működés

- [ ] új user regisztráció
- [ ] login
- [ ] JWT
- [ ] UserId alapján szűrés
- [ ] saját eladások
- [ ] saját felvásárlások
- [ ] saját partnerek
- [ ] saját vevők
- [ ] saját zöldségek
- [ ] saját rekeszadatok
- [ ] másik user adatainak elrejtése

## Frontend

- [ ] mobile-first reszponzivitás
- [ ] minden oldal
- [ ] minden modal
- [ ] kártyás megjelenítés
- [ ] képek közel 4:3 arányban
- [ ] „Saját termék” modal
- [ ] „Fizetve” gomb eltávolítása

---

# Aktuális fejlesztési állapot

```text
ZoldPiac
.NET 8 WebAPI + Vue + MySQL/MariaDB
MonsterASP

Frontend:
https://zoldpiac.tryasp.net/

Backend:
https://zoldpiac.runasp.net/

MySQL:
db65395.public.databaseasp.net:3306

User:
db65395.databaseasp.net

TCP 3306:
OK

MonsterASP Remote Check:
OK

EF:
connection string beolvasódik

EF hiba:
Access denied for user
'db65395.databaseasp.net'@'84.3.153.75'

UDL:
NEM megfelelő, mert SQL Serverhez próbál csatlakozni.

Production DB:
létező, valódi adatokkal rendelkező adatbázis.

Hiányzó migration:
20260910233000_MultiUserAccounts

KRITIKUS:
ezt még NEM szabad alkalmazni, mert a jelenlegi migration
törli az admin usert:

DELETE FROM Users WHERE Felhasznalonev = 'admin';

A migrationt előbb adatmegőrzőre kell javítani.
```

---

# Következő konkrét lépés

## 1. MySQL kapcsolat tesztelése

Közvetlen MySQL klienssel, például HeidiSQL-lel:

```text
Host:
db65395.public.databaseasp.net

Port:
3306

User:
db65395.databaseasp.net

Password:
MonsterASP jelszó
```

Ha a belépés sikeres:

```text
MonsterASP MySQL hitelesítés OK
```

Ezután az EF connection stringet ehhez kell igazítani.

Ha a belépés sikertelen:

```text
Access denied
```

akkor a MonsterASP MySQL felhasználónév / jelszó / remote jogosultság környékén kell tovább keresni.

## 2. Multi-user migration javítása

A MySQL kapcsolat után a következő feladat:

```text
20260910233000_MultiUserAccounts
```

teljes átvizsgálása és olyan módosítása, amely:

- nem töröl régi felhasználót
- nem töröl régi adatot
- a régi adatokat a megfelelő userhez rendeli
- létrehozza a többfelhasználós kapcsolatokat
- production adatbázison biztonságosan alkalmazható.

## 3. Csak ezután

```powershell
dotnet ef database update
```

majd production deployment.

---

# Rövid összefoglaló

A projekt fő célja egy többfelhasználós, webes zöldségpiaci nyilvántartó rendszer.

A backend .NET 8 WebAPI, a frontend Vue 3, az adatbázis MySQL/MariaDB.

A production környezet MonsterASP-on fut.

A MySQL szerver hálózatilag már elérhető, de a lokális EF kapcsolat jelenleg MySQL authentication hibával áll meg.

A legközelebbi lépés a közvetlen MySQL bejelentkezés tesztelése.

A másik kritikus feladat a többfelhasználós migration biztonságossá tétele, mivel a jelenlegi migration törölné a régi `admin` rekordot, és production adatbázisról van szó.

**Production adatbázison adatvesztést okozó migrationt nem szabad lefuttatni.**