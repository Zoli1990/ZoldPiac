<template>
  <article class="card legal">
    <h1>ZoldPiac – Adatkezelési tájékoztató</h1>
    <p class="muted">Verzió: 1.0</p>
    <p><strong>Fontos:</strong> a dokumentum a jelenlegi tesztverzió adatkezelési szövegjavaslata. Éles közzététel előtt a szolgáltatói és adatfeldolgozói adatok kitöltése, valamint jogi ellenőrzés szükséges.</p>

    <h2>1. Adatkezelő</h2>
    <p><strong>[ADATKEZELŐ / SZOLGÁLTATÓ NEVE]</strong><br>[CÍM]<br>[KAPCSOLATTARTÁSI EMAIL]</p>

    <h2>2. Milyen adatokat kezelünk?</h2>
    <p>A fiókhoz e-mail cím, jelszó hash, visszaigazolási adatok, regisztrációs időpont és az elfogadott ÁSZF verziója kapcsolódik. A jelszót a rendszer nem tárolja visszafejthető formában.</p>
    <p>A felhasználó a szolgáltatásban üzleti adatokat is rögzíthet, például partnereket, vevőket, zöldségeket, felvásárlási és eladási tételeket, valamint megjegyzéseket és képeket.</p>

    <h2>3. Az adatkezelés célja</h2>
    <p>A cél a felhasználói fiók működtetése, a bejelentkezés és e-mail visszaigazolás, az alkalmazás szolgáltatásainak biztosítása, a hibakeresés és biztonság, valamint az esetleges jogszabályi kötelezettségek teljesítése.</p>

    <h2>4. Felhasználónkénti elkülönítés</h2>
    <p>A ZoldPiac többfelhasználós rendszer. A felhasználó üzleti adatai a saját felhasználói azonosítójához kapcsolódnak. A szolgáltató technikai üzemeltetőként a szerver és az adatbázis működéséhez szükséges mértékben hozzáférhet.</p>

    <h2>5. Képek</h2>
    <p>A feltöltött képeket a rendszer a tárolás előtt lekicsinyíti és tömöríti. Az eredeti nagyméretű kép nem feltétlenül kerül megőrzésre. A képek felhasználónként elkülönített könyvtárban és egyedi fájlnévvel kerülnek tárolásra.</p>

    <h2>6. Szolgáltatók és adatfeldolgozók</h2>
    <p>A működéshez külső tárhely- és e-mail szolgáltató vehető igénybe. A végleges dokumentumban fel kell tüntetni a tényleges szolgáltatók nevét és az alkalmazandó adatfeldolgozási információkat.</p>

    <h2>7. Megőrzés</h2>
    <p>A fiókadatokat főszabály szerint a fiók fennállásáig kezeljük, a jogszabály által előírt megőrzésekre is figyelemmel. A felhasználó által rögzített üzleti adatok megőrzési idejét a szolgáltatás működése és a jogi kötelezettségek határozzák meg.</p>

    <h2>8. Érintetti jogok</h2>
    <p>Az alkalmazandó adatvédelmi szabályok alapján az érintett kérheti többek között személyes adataihoz való hozzáférést, azok helyesbítését vagy törlését, az adatkezelés korlátozását, illetve az adathordozhatóságot, ahol annak feltételei fennállnak.</p>
    <p>Kérelmek: <strong>[KAPCSOLATTARTÁSI EMAIL]</strong></p>

    <h2>9. Felügyeleti hatóság</h2>
    <p>Magyarországon az adatvédelmi felügyeleti hatóság a Nemzeti Adatvédelmi és Információszabadság Hatóság (NAIH).</p>
    <p><a href="https://naih.hu/" target="_blank" rel="noopener noreferrer">NAIH hivatalos oldala</a></p>

    <h2>10. Sütik és helyi tárolás</h2>
    <p>A webes kliens a hitelesítés és az alkalmazás működése érdekében technikai adatokat, többek között helyi tárhelyben tárolt tokent használhat. A szolgáltatás jelenlegi terve szerint nem alkalmaz marketingcélú vagy viselkedésalapú nyomkövetést.</p>

    <p class="back"><RouterLink to="/register">Vissza a regisztrációhoz</RouterLink></p>
  </article>
</template>

<style scoped>
.legal { max-width: 820px; margin: 0 auto; line-height: 1.65; }
h1 { margin-top: 0; font-size: 24px; } h2 { margin-top: 26px; font-size: 18px; }
.muted { opacity: .65; font-size: 13px; } .back { margin-top: 30px; }
</style>
