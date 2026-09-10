# ZoldPiac – Adatkezelési tájékoztató

**Verzió:** 1.0  
**Hatálybalépés:** [KITÖLTENDŐ DÁTUM]

> **Fontos:** ez a dokumentum a jelenlegi tesztverzióhoz készült adatvédelmi szövegjavaslat. Éles közzététel előtt a szolgáltató azonosító és kapcsolattartási adatait ki kell tölteni, a tényleges adatfeldolgozókat és tárhelyszolgáltatókat pontosítani kell, és célszerű adatvédelmi szakemberrel ellenőriztetni.

## 1. Adatkezelő

- Név: **[ADATKEZELŐ / SZOLGÁLTATÓ NEVE]**
- Cím: **[CÍM]**
- E-mail: **[KAPCSOLATTARTÁSI EMAIL]**
- Adatvédelmi kapcsolattartó, ha eltér: **[EMAIL / NÉV]**

## 2. Az adatkezelés célja

A ZoldPiac szolgáltatás használatához felhasználói fiók szükséges. Az adatkezelés fő céljai:

- felhasználói fiók létrehozása és kezelése;
- a belépés és a jogosultságok biztosítása;
- az e-mail cím visszaigazolása;
- a szolgáltatás működtetése;
- a felhasználó által rögzített üzleti nyilvántartás technikai tárolása;
- információbiztonság, hibakeresés és visszaélések megelőzése;
- jogszabályi kötelezettségek teljesítése.

A szolgáltató a személyes adatokat nem használja fel az itt meghatározott célokkal össze nem egyeztethető módon.

## 3. Kezelt adatok

### 3.1. Regisztráció és fiók

A rendszer a következő fiókadatokat kezeli:

- e-mail cím;
- jelszó hash formában;
- e-mail cím visszaigazolási állapota;
- a visszaigazolási token hash-e és lejárata;
- regisztráció időpontja;
- elfogadott ÁSZF verziója;
- ÁSZF elfogadásának időpontja;
- technikai felhasználói azonosító.

A rendszer a jelszót nem tárolja visszafejthető formában.

### 3.2. A felhasználó által rögzített üzleti adatok

A szolgáltatás a működéséhez a felhasználó által megadott adatokat is tárolhatja, például:

- partnerek neve és megjegyzése;
- vevők neve és megjegyzése;
- zöldségek neve, alapértelmezett rekesztípusa és képe;
- felvásárlási és eladási tételek;
- dátum, időpont, mennyiség, árak és fizetési/rekeszállapotok;
- a felhasználó által megadott megjegyzések.

A felhasználó felel azért, hogy harmadik személyek személyes adatainak rögzítésére megfelelő jogalappal rendelkezzen.

## 4. Adatkezelés jogalapja

A szolgáltatás működéséhez szükséges adatkezelések jogalapját elsősorban a szerződés teljesítéséhez szükséges adatkezelés, illetve az adatkezelő jogos érdeke vagy jogi kötelezettsége adhatja. Egyes adatkezelések esetében hozzájárulás is alkalmazható.

A konkrét jogalapokat a végleges közzétett adatkezelési folyamatokkal összhangban kell meghatározni. A jelen dokumentum nem helyettesíti az egyedi jogi minősítést.

## 5. E-mailes kommunikáció

A regisztrációkor megadott e-mail címet a szolgáltató a fiók létrehozásához és az e-mail cím visszaigazolásához használja.

A visszaigazoló e-mail kiküldéséhez külső levelezési/SMTP szolgáltatás is igénybe vehető. A konkrét szolgáltató nevét és a kapcsolódó adatfeldolgozási információkat a végleges verzióban fel kell tüntetni.

## 6. Tárhely és adatfeldolgozók

A szolgáltatás működéséhez külső tárhely- és infrastruktúra-szolgáltatók vehetők igénybe. A jelenlegi technikai terv szerint az alkalmazás és adatbázisa megosztott tárhelyen, a feltöltött képek pedig a backend által kezelt tárhelyen kerülnek tárolásra.

A végleges dokumentumban fel kell sorolni az összes tényleges adatfeldolgozót, így különösen:

- tárhelyszolgáltató;
- adatbázis-szolgáltató, ha külön szolgáltató;
- SMTP/e-mail szolgáltató;
- egyéb, személyes adatot érintő technikai szolgáltató.

## 7. Adatok elkülönítése

A ZoldPiac többfelhasználós rendszer. A felhasználó üzleti adatai felhasználói azonosítóhoz kapcsolódnak, és a rendszer a hozzáférést a bejelentkezett felhasználóhoz kötött adatokra korlátozza.

A szolgáltató technikai üzemeltetőként a szerverhez és az adatbázishoz hozzáférhet. Ez a hozzáférés kizárólag az üzemeltetéshez, karbantartáshoz, hibakereséshez, biztonsághoz és jogi kötelezettségek teljesítéséhez szükséges mértékben történhet.

## 8. Képek kezelése

A felhasználó által feltöltött képeket a rendszer a tárolás előtt feldolgozza és tömöríti. Az eredeti, több megabájtos kép nem feltétlenül kerül változatlan formában tárolásra.

A képek felhasználói mappákhoz kapcsolódó fizikai tárhelyen kerülnek elhelyezésre. A fájlnevek egyedileg generáltak.

## 9. Megőrzési idő

A regisztrációhoz szükséges fiókadatokat a fiók fennállásáig kezeljük, kivéve azokat az adatokat, amelyek megőrzését jogszabály vagy jogos érdek indokolja.

A felhasználó által rögzített üzleti adatok megőrzési időtartamát a szolgáltatás működése, a felhasználó kérése és az esetlegesen fennálló jogi kötelezettségek határozzák meg.

A pontos törlési szabályokat a szolgáltatás éles változatának véglegesítésekor kell meghatározni.

## 10. Adatbiztonság

A szolgáltató megfelelő technikai és szervezési intézkedésekkel törekszik az adatok jogosulatlan hozzáférésének, megváltoztatásának, elvesztésének vagy megsemmisülésének megelőzésére.

Ennek része többek között:

- jelszavak hash-elt tárolása;
- hitelesített API-hozzáférés;
- felhasználónként elkülönített adatkezelés;
- e-mail visszaigazolási tokenek hash-elt tárolása;
- feltöltött képek feldolgozása és egyedi fájlnevek alkalmazása.

A szolgáltató ugyanakkor nem garantálhatja abszolút módon, hogy internetes vagy informatikai környezetben adatbiztonsági incidens soha nem következik be.

## 11. Az érintett jogai

A vonatkozó adatvédelmi jogszabályok alapján az érintett – az alkalmazandó feltételek mellett – kérheti különösen:

- a személyes adataihoz való hozzáférést;
- azok helyesbítését;
- törlését;
- az adatkezelés korlátozását;
- az adathordozhatóságot, ha annak feltételei fennállnak;
- tiltakozhat bizonyos adatkezelések ellen.

Az érintett kérelmét a **[KAPCSOLATTARTÁSI EMAIL]** címre küldheti.

A szolgáltató a kérelmeket a vonatkozó jogszabályban meghatározott határidőben kezeli.

## 12. Panasz és felügyeleti hatóság

Az érintett jogosult arra, hogy adatvédelmi jogainak vélt megsértése esetén a hatáskörrel rendelkező felügyeleti hatósághoz forduljon. Magyarországon az adatvédelmi felügyeleti hatóság:

**Nemzeti Adatvédelmi és Információszabadság Hatóság (NAIH)**

Weboldal: https://naih.hu/

A hatóság elérhetőségeit és az aktuális panaszkezelési információkat a hatóság hivatalos oldalán kell ellenőrizni.

## 13. Adatvédelmi incidens

Adatvédelmi incidens észlelése esetén a szolgáltató a vonatkozó jogszabályoknak megfelelően megteszi a szükséges intézkedéseket, és ahol jogszabály előírja, értesíti az érintett hatóságot és/vagy az érintetteket.

## 14. Sütik és helyi tárolás

A webes kliens a szolgáltatás működéséhez technikai adatokat, többek között hitelesítési tokent és kapcsolódó kliensoldali állapotot tárolhat a böngésző helyi tárhelyén.

A végleges sütikezelési tájékoztatót a ténylegesen használt cookie-k és egyéb nyomkövető technológiák alapján kell elkészíteni.

A szolgáltatás jelenlegi terve szerint nem szükséges marketing- vagy viselkedésalapú nyomkövetés alkalmazása.

## 15. Módosítások

A szolgáltató jogosult a jelen tájékoztatót a jogszabályok, a szolgáltatás műszaki működése vagy az adatkezelések változása miatt módosítani.

Lényeges változás esetén a felhasználókat megfelelő módon tájékoztatni kell.

---

**Közzététel előtt feltétlenül kitöltendő és ellenőrizendő:** adatkezelő neve, címe, e-mail címe, tényleges tárhely- és SMTP-szolgáltató, tényleges megőrzési idők, a szolgáltatásban használt sütik/technikai tárolók, valamint a végleges jogalapok és érintetti joggyakorlási folyamat.
