# ZöldPiac – fejlesztési és üzemeltetési dokumentáció

**Friss zöldségek, okosan. Online.**

Zöldségpiaci nyilvántartó webalkalmazás többfelhasználós működéssel: felvásárlások, saját termés, eladások, rekeszek és készlet, partnerek és vevők, egyenlegek és tartozások nyilvántartása.

Ez a `ZoldPiac` repository a `Piac / RekeszApp` projekt továbbfejlesztett, **többfelhasználós** változata: saját frontend, backend és adatbázis, önálló fejlesztési ágként. A korábbi `Piac` repository már nem a fejlesztés alapja.

> Ez az egyetlen, karbantartott README. Korábban két, részben átfedő/elavult dokumentum élt egymás mellett (`README.md` és `README_folyt-kov.md`); ezek tartalma itt lett egyesítve és frissítve a jelenlegi kódállapotra.

---

## 1. Jelenlegi állapot (röviden)

A többfelhasználós alapok **el vannak készítve és működnek**:

- ✅ `admin/admin` megszűnt, nincs előre létrehozott közös fiók.
- ✅ Email-címes regisztráció.
- ✅ Emailes visszaigazolás (24 órás, hash-elt token) — bejelentkezni csak visszaigazolt email-lel lehet.
- ✅ Jelszó-visszaállítás ("elfelejtett jelszó") — 1 órás, hash-elt token.
- ✅ Visszaigazoló/visszaállító email újraküldése rate-limitelve (60 mp / felhasználó), user-enumeration ellen generikus válaszüzenetekkel.
- ✅ Backend oldali, JWT-alapú felhasználói adatizoláció (EF Core globális query filter + automatikus tulajdonos-hozzárendelés mentéskor).
- ✅ Képfeltöltés, átméretezés/tömörítés, felhasználónkénti tárolás.
- ✅ Favicon és márkajelzés (`NewStile.jpg` alapján) a login/regisztráció felületen.

Folyamatban / még nincs kész:

- [ ] ÁSZF és adatkezelési tájékoztató végleges (jelenleg placeholder szövegek: `ASZF.md`, `ADATKEZELESI_TAJEKOZTATO.md`).
- [ ] Frontend redesign (a jelenlegi "papír/krétazöld" stílusból az új `ZöldPiac` márka felé — lásd 9. szakasz).
- [ ] `Vendeg` (korlátozott) szerepkör tényleges kidolgozása — az adatmodellben már létezik a `FelhasznaloSzerepkor` enum, de jelenleg minden regisztráló `Admin` szerepet kap, és a kód nem tesz különbséget. Ez tudatosan később eldöntendő kérdés.
- [ ] Éles adatbázis-migráció ellenőrzése tényleges production környezeten.

---

## 2. Technológiai felépítés

### Backend
- ASP.NET Core / .NET 8 Web API, C#
- Entity Framework Core + Pomelo MySQL provider
- MySQL / MariaDB
- JWT-alapú autentikáció, BCrypt jelszóhash-elés
- ImageSharp képfeldolgozás
- REST API, Swagger

### Frontend
- Vue 3 (Composition API), Vite, Pinia, Vue Router, Axios
- Mobil-first, reszponzív felület (mobil / iPad / asztali)

### Üzemeltetés
A `ZoldPiac` külön infrastruktúrán fut a korábbi (`Piac`) rendszerhez képest: külön frontend URL, backend URL, adatbázis, GitHub repository. A régi rendszer adatai nem részei ennek a repónak.

Példa production topológia (a tényleges értékeket a szolgáltatói fiók adja):

```text
Frontend:  https://<frontend-alias>.tryasp.net/
Backend:   https://<backend-alias>.runasp.net/
MySQL:     <db-név>.public.databaseasp.net:3306
```

---

## 3. Felhasználói és jogosultsági modell

Nincs előre létrehozott közös `admin/admin` fiók. A felhasználók saját email-címmel regisztrálnak.

```text
email + jelszó → regisztráció → emailes visszaigazolás → fiók aktiválása → bejelentkezés → alkalmazás használata
```

Elfelejtett jelszó esetén:

```text
email megadása → visszaállító email → új jelszó megadása a linken → bejelentkezés
```

Jelenleg minden regisztrált felhasználó ugyanazt a teljes funkcionalitást kapja (nincs külön alkalmazáson belüli admin szerepkör — az üzemeltető a rendszer működését közvetlenül a szerver/adatbázis oldaláról követi). A `Role` mező (`Admin`/`Vendeg`) az adatmodellben megvan egy jövőbeli, korlátozottabb "vendég" szerepkör lehetőségének, de ma nincs mögötte logika.

---

## 4. Többfelhasználós adatmodell és adatizoláció

Alapelv: **minden üzleti adat egy konkrét felhasználóhoz tartozik**, és egy felhasználó nem férhet hozzá más felhasználó adataihoz.

```text
User ── UserId
Partner, Vevo, Zoldseg, RekeszTipus, FelvasarlasTetel, EladasTetel ── mind: UserId
```

Ez a gyakorlatban meg is van valósítva:

- A backend a hitelesített felhasználóból (JWT `NameIdentifier` claim) határozza meg az aktuális `UserId`-t — a frontend által küldött `UserId` sosem megbízható jogosultsági információ.
- `AppDbContext` globális **query filter**-t alkalmaz minden üzleti entitáson (`Partner`, `Vevo`, `Zoldseg`, `RekeszTipus`, `FelvasarlasTetel`, `EladasTetel`): lekérdezéskor automatikusan csak a bejelentkezett felhasználó rekordjai látszanak.
- Mentéskor (`SaveChanges`) az új rekordok automatikusan megkapják a bejelentkezett felhasználó `UserId`-ját; módosítás/törlés esetén kivétel dobódik, ha a rekord nem az övé.

```text
JWT UserId = 17
rekord UserId = 17  → engedélyezett
rekord UserId = 18  → nem hozzáférhető
```

---

## 5. Adatbázis és migrációk

A migráció-lánc jelenlegi állapota:

```text
20260824060000_InitialCreate
        ↓
20260831120000_EladasHianyFelvasarlasKiegeszites
        ↓
20260902201337_RaktarHelyszin
        ↓
20260910233000_MultiUserAccounts       (admin/admin → email-alapú multi-user)
        ↓
20260912120000_PasswordReset           (jelszó-visszaállítás mezői)
```

A backend indításakor (`DbInitializer.InitializeAsync`) automatikusan lefutnak a függőben lévő migrációk (`Database.MigrateAsync()`).

### Fontos szabályok
- A már elkészített migrációkat nem írjuk vissza menőlegesen át — új adatmodellhez új migráció készül.
- Minden migráció előtt meg kell vizsgálni: milyen táblát/oszlopot érint, van-e adatvesztési kockázat, nullable-e az új mező, mi történik a meglévő rekordokkal.
- Migrációt lehetőleg üres és már feltöltött adatbázison is le kell tesztelni.
- Production migráció előtt: teljes adatbázis-mentés, mentés ellenőrzése, csak utána migráció.

> A `MultiUserAccounts` migráció korábbi verziója tartalmazott egy `DELETE FROM Users WHERE Felhasznalonev = 'admin'` sort a régi egyfelhasználós rendszer maradványaként. Ez el lett távolítva — friss adatbázison ártalmatlan volt, de felesleges és félrevezető.

---

## 6. Felvásárlás és készletmodell

### 6.1. Felvásárlás
Egy felvásárlási tétel egy konkrét rögzített tranzakció. Fontos adatai: dátum, eladó/partner, zöldség, rekesztípus, mennyiség, egységár, fizetve állapot, adott üres rekesz, helyszín, saját termés jelölése, megjegyzés.

- **Vásárolt áru:** az egységár kötelező.
- **Saját termés:** nincs partner; az egységár opcionális (becsült önköltségként használható, ha szükséges).
- **Adott üres rekesz:** száma nem lehet nagyobb a felvásárolt mennyiségnél, alapértéke 0; a mennyiség módosítása nem írja át automatikusan.

### 6.2. Nincs FIFO
A rendszer nem vezet automatikus FIFO készletkezelést, és nincs rejtett készletforrás-allokáció sem — az `EladasTetel` nem tartja nyilván, melyik `FelvasarlasTetel`-ből fogyott. Ez szándékos üzleti döntés.

A készlet elsődleges csoportosítási kulcsa **zöldség + rekesztípus** (nem része: partner, dátum, felvásárlási ár). A csoportosított nézet nem önálló adatbázisrekord — backend művelethez mindig egy valódi `FelvasarlasTetel` ID kell.

Nincs becsült/súlyozott átlag vételár számítás a készletnézetben (forrásallokáció hiányában nem lenne egzakt).

### 6.3. Raktár és kocsi
Két fő helyszín: `Kocsi` és `Raktar`. A `Helyszin` mező jelzi a fizikai helyet; raktári tételnél az `AthelyezveDb` mutatja, mennyi került át a kocsira:

```text
raktáron maradt mennyiség = Mennyiseg - AthelyezveDb
```

### 6.4. Egyenleg és rekesztartozás

**Mi tartozunk – eladóknak:** pénzbeli tartozás csak a nem fizetett felvásárlási tételekből; rekesztartozás a kapott/adott rekeszek különbsége.

**Nekünk tartoznak – vevők:** pénztartozás = `mennyiség × egységár` a nem fizetett eladási tételeknél; rekesztartozás = `elvitt mennyiség − visszahozott rekesz − kifizetett hiány` (a hiány rendezése darabszám alapú).

---

## 7. Képtárolás

Minden felhasználó saját könyvtárat kap:

```text
private/
└── images/
    └── users/
        ├── 1/
        ├── 2/
        └── ...
```

- A fájlnév szerver-generált, egyedi (nem az eredeti feltöltött név) — ütközések és felülírás elkerülésére.
- Feltöltéskor 5 MB-os bemeneti korlát; a rendszer feldolgozza a képet (dekódolás → állandó max. méret → JPEG/WebP tömörítés → tárolás), mivel a képek kis kártyaképként jelennek meg.
- A `Zoldseg.KepUrl` relatív fájlreferencia (pl. `17/8c1f...webp`), nem környezethez kötött abszolút URL — a tárolás helye így később módosítható.
- Az uploads mappa a `wwwroot`-on kívül van (`Uploads:Path` appsettings kulcs, `UploadsPaths.Resolve()`), mert megosztott hosztingon a `wwwroot` redeploykor törlődhet.

---

## 8. Biztonsági alapelvek

- Jelszavak soha nem kerülnek plaintext formában adatbázisba (BCrypt).
- Email-visszaigazoló és jelszó-visszaállító tokenek SHA-256 hash-elve tárolódnak, lejárattal (24 óra, illetve 1 óra).
- A visszaigazoló/visszaállító email újraküldése rate-limitelt (60 mp/felhasználó); a válaszüzenet generikus, hogy ne áruljon el, létezik-e a fiók az adott email-címmel.
- A JWT signing key és a connection string nem kerül GitHubra — env variable-ból vagy lokális konfigurációból származik (`appsettings.example.json` csak placeholdereket tartalmaz).
- Minden felhasználó csak a saját adatait éri el (ld. 4. szakasz); a frontend által küldött `UserId` sosem megbízható.
- Feltöltött fájloknál fájltípus- és méretellenőrzés; a fájlneveket a szerver generálja.
- Publikus repositoryba adatbázismentés, jelszó, token, kulcs nem kerülhet.

### Konfiguráció és titkok — `appsettings.json` váz

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
  "Cors": { "Origins": ["https://<frontend-alias>.tryasp.net"] },
  "Auth": { "FrontendUrl": "https://<frontend-alias>.tryasp.net" },
  "Uploads": { "Path": "../private/images/users" },
  "Smtp": {
    "Host": "smtp.example.com", "Port": "587", "EnableSsl": "true",
    "Username": "SMTP_USERNAME", "Password": "SMTP_PASSWORD",
    "FromAddress": "noreply@example.com", "FromName": "ZoldPiac"
  }
}
```

A tényleges titkokat és éles konfigurációt (`appsettings.Local.json`, `appsettings.Production.json`, `secrets*.json`, `.env*`) a `.gitignore` zárja ki a repóból; a repóban csak `appsettings.example.json` szerepel placeholderekkel.

> Ha valódi jelszó/secret korábban mégis GitHub repóba került, önmagában a törlése nem elég — a titkot le kell cserélni a szolgáltatónál, és csak placeholder maradhat a repóban.

---

## 9. Márka és design irány

Az új márka: **ZöldPiac — Friss zöldségek, okosan. Online.**

A repo gyökerében lévő `NewStile.jpg` az új vizuális stílust reprezentálja (élénk zöld/narancs, piaci-térkép "pin" ikon, zöldségekkel). Ebből készült:

- `RekeszAppFrontend/public/favicon.ico`, `favicon-16/32/48/192/512.png`, `apple-touch-icon.png` — a böngésző/mobil favicon-készlet.
- `RekeszAppFrontend/public/brand-zoldpiac.png` — ikon + felirat + szlogen lockup, jelenleg a Login / Regisztráció / Email-visszaigazolás felület fejlécében.

Fontos: **a kép csak stílusirányzat**, nem szó szerint lemásolandó vizuális terv — a további redesign (színpaletta, tipográfia, elrendezés) ez alapján, de nem ebből kimásolva készül. A jelenlegi frontend stílusa és funkcionalitása nagyjából megfelelő, ehhez képest néhány pontosított módosítás várható a redesign körben.

---

## 10. Fejlesztési alapelvek

1. A tényleges üzleti használat fontosabb az elméleti túltervezésnél.
2. Nincs FIFO, hacsak arra külön üzleti döntés nem születik.
3. Nincs rejtett készletforrás-allokáció.
4. A csoportosított frontend-nézet nem jelent összevont adatbázisrekordot.
5. A felhasználói adatizoláció backend oldali követelmény, nem bízható a frontendre.
6. Egy felhasználó nem láthatja vagy módosíthatja más felhasználó üzleti adatait.
7. A már alkalmazott migrációkat nem írjuk át visszamenőlegesen; új adatbázisváltozás új migrációval történik.
8. A képek tárolása felhasználónkénti; méretük/minőségük a tényleges webes megjelenítéshez igazított.
9. Titkok és éles konfiguráció nem kerülhetnek a repositoryba.
10. A lehető legegyszerűbb, üzletileg és technikailag korrekt megoldást választjuk.

---

## 11. Jogi és szolgáltatási keretek

A `ZoldPiac` jelenlegi célja egy ingyenesen kipróbálható szolgáltatás biztosítása, nyitott regisztrációval. A tervek szerint a jelenlegi tesztidőszakban előreláthatóan egy évig díjmentesen vehető igénybe; a későbbi fizetős működés nem garantált, erről a tesztidőszak eredménye alapján születik döntés.

A jogi dokumentáció vázlatai a repóban:
- `ASZF.md`
- `ADATKEZELESI_TAJEKOZTATO.md`

Ezek **placeholder szövegek** — éles közzététel előtt a szolgáltatói adatokkal, tényleges adatfeldolgozókkal/tárhelyszolgáltatóval kell kiegészíteni, célszerűen szakemberrel átnézetve.

---

## 12. Tesztidőszak célja

A jelenlegi időszak nem csak technikai tesztelés, hanem piaci validáció is: van-e elegendő valódi érdeklődés és rendszeres használat ahhoz, hogy érdemes legyen a szolgáltatást később fizetős termékként továbbvinni. Kezdetben nem szükséges külön analitikai rendszer — a felhasználószám a `User.Id` rekordok alapján követhető (az `AUTO_INCREMENT` érték önmagában nem feltétlenül egyezik a tényleges rekordszámmal, ha törlés is történik).

---

## 13. Fejlesztési folyamat

```text
1. Repository ellenőrzése
2. Jelenlegi működés megértése
3. Probléma pontos meghatározása
4. Módosítás
5. Build
6. Teszt
7. Migráció esetén külön DB teszt (üres DB + meglévő adatokkal rendelkező DB)
8. Git commit → Push
9. Production ellenőrzés
```

---

## 14. Feladatlista

### Multi-user működés
- [x] regisztráció email-visszaigazolással
- [x] login (csak visszaigazolt email-lel)
- [x] jelszó-visszaállítás
- [x] JWT
- [x] UserId alapján szűrés (globális query filter + automatikus ownership)
- [x] saját eladások / felvásárlások / partnerek / vevők / zöldségek / rekeszadatok
- [x] másik user adatainak elrejtése

### Frontend
- [x] mobile-first reszponzivitás (alapok)
- [x] favicon + márkajelzés a login/regisztráció felületen
- [ ] teljes redesign az új `ZöldPiac` márka szerint
- [ ] 1-2 még egyeztetendő funkcionális módosítás

### Jogi / üzemeltetési
- [ ] ÁSZF és adatkezelési tájékoztató véglegesítése
- [ ] production migráció ellenőrzése tényleges környezeten, teljes DB-mentéssel
