<template>
  <article class="card legal">
    <h1>ZoldPiac – Általános Szerződési Feltételek</h1>
    <p class="muted">Verzió: 1.0</p>
    <p><strong>Fontos:</strong> a szolgáltatás jelenleg nyilvános, díjmentes tesztidőszakban működik. A végleges közzététel előtt a szolgáltatói adatok és a jogi szöveg ellenőrzése szükséges.</p>

    <h2>1. A szolgáltatás</h2>
    <p>A ZoldPiac interneten elérhető webes alkalmazás piaci és zöldség-kereskedelmi nyilvántartás vezetésére. A szolgáltatás fejlesztés és tesztelés alatt áll; funkciói változhatnak.</p>

    <h2>2. Regisztráció</h2>
    <p>A regisztráció nyitott. A használathoz e-mail cím és jelszó szükséges, és az e-mail címet vissza kell igazolni. Egy igazolt fiók a jelenlegi alkalmazási funkciók használatára jogosít.</p>

    <h2>3. Ingyenes tesztidőszak</h2>
    <p>A szolgáltatás jelenleg díjmentesen használható. A Szolgáltató jelenlegi terve szerint a regisztrációtól számított első egy évben díjmentes marad. Ezt követően fizetős szolgáltatássá válhat, de a későbbi díj fizetési kötelezettségét külön feltételeknek kell rendezniük.</p>

    <h2>4. Felhasználói adatok</h2>
    <p>A felhasználó felel az általa rögzített vagy feltöltött adatok jogszerűségéért és pontosságáért. Különösen harmadik személy személyes adatát csak megfelelő jogosultság alapján rögzítse.</p>

    <h2>5. Adatmentés és rendelkezésre állás</h2>
    <p>A szolgáltatás tesztverzió. A Szolgáltató törekszik a működésre és az adatok védelmére, de nem vállal korlátlan garanciát a folyamatos rendelkezésre állásra vagy az adatok végleges megőrzésére. Fontos üzleti adatok esetén a felhasználónak saját mentésről is gondoskodnia kell.</p>

    <h2>6. Tiltott használat</h2>
    <p>Tilos más felhasználó adatainak megkísérelt elérése, a biztonsági korlátozások megkerülése, a rendszer túlterhelése vagy egyéb visszaélésszerű használata.</p>

    <h2>7. Megszüntetés</h2>
    <p>A felhasználó a fiókja megszüntetését a <strong>[KAPCSOLATTARTÁSI EMAIL]</strong> címen kérheti. A részletes adatkezelési szabályokat az Adatkezelési tájékoztató tartalmazza.</p>

    <h2>8. Szolgáltató</h2>
    <p><strong>[SZOLGÁLTATÓ NEVE]</strong><br>[CÍM]<br>[KAPCSOLATTARTÁSI EMAIL]</p>

    <p class="back"><RouterLink to="/register">Vissza a regisztrációhoz</RouterLink></p>
  </article>
</template>

<style scoped>
.legal { max-width: 820px; margin: 0 auto; line-height: 1.65; }
h1 { margin-top: 0; font-size: 24px; } h2 { margin-top: 26px; font-size: 18px; }
.muted { opacity: .65; font-size: 13px; } .back { margin-top: 30px; }
</style>
