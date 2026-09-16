<script setup>
import { computed, ref, onMounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from './stores/auth'
import CalculatorModal from './components/CalculatorModal.vue'

const route = useRoute()
const router = useRouter()
const auth = useAuthStore()
const bejelentkezve = computed(() => !!auth.token && route.name !== 'login')
const showCalculator = ref(false)
const darkMode = ref(false)
const showInstallPrompt = ref(false)
const installEvent = ref(null)
const installBusy = ref(false)
const showIOSInstallHint = ref(false)

function isIOSDevice() {
  const ua = window.navigator.userAgent
  return /iphone|ipad|ipod/i.test(ua) ||
    (ua.includes('Macintosh') && navigator.maxTouchPoints > 1)
}

function alkalmazTema(dark) {
  darkMode.value = dark
  document.documentElement.dataset.theme = dark ? 'dark' : 'light'
  localStorage.setItem('zoldpiac:theme', dark ? 'dark' : 'light')
}

function pwaMarStandalone() {
  return window.matchMedia?.('(display-mode: standalone)').matches || window.navigator.standalone === true
}

function pwaElutasitasAktiv() {
  const t = Number(localStorage.getItem('zoldpiac:pwa-dismissed-until') || 0)
  return t > Date.now()
}

onMounted(() => {
  alkalmazTema(localStorage.getItem('zoldpiac:theme') === 'dark')

  window.addEventListener('beforeinstallprompt', (event) => {
    event.preventDefault()
    installEvent.value = event
    if (!pwaMarStandalone() && !pwaElutasitasAktiv()) {
      setTimeout(() => { showInstallPrompt.value = true }, 1200)
    }
  })

  window.addEventListener('appinstalled', () => {
    installEvent.value = null
    showInstallPrompt.value = false
  })

  if (isIOSDevice() && !pwaMarStandalone() && !pwaElutasitasAktiv()) {
    setTimeout(() => { showIOSInstallHint.value = true }, 1200)
  }
})

watch(darkMode, (dark) => {
  document.documentElement.dataset.theme = dark ? 'dark' : 'light'
})

function temaValtas() {
  alkalmazTema(!darkMode.value)
}

async function pwaTelepites() {
  if (!installEvent.value || installBusy.value) return
  installBusy.value = true
  try {
    await installEvent.value.prompt()
    const result = await installEvent.value.userChoice
    if (result.outcome === 'accepted') {
      showInstallPrompt.value = false
      installEvent.value = null
    }
  } finally {
    installBusy.value = false
  }
}

function pwaMostNem() {
  showInstallPrompt.value = false
  showIOSInstallHint.value = false
  localStorage.setItem('zoldpiac:pwa-dismissed-until', String(Date.now() + 7 * 24 * 60 * 60 * 1000))
}

function kijelentkezes() {
  auth.logout()
  router.push({ name: 'login' })
}
</script>

<template>
  <header v-if="bejelentkezve">
    <div class="brand">
      <div class="brand-logo" aria-hidden="true"></div>
      <div class="brand-copy">
        <div class="brand-text">ZöldPiac</div>
        <div class="brand-sub">Friss zöldségek, okosan. Online.</div>
      </div>
    </div>

    <nav aria-label="Fő navigáció">
      <RouterLink to="/" :class="{ active: route.name === 'felvasarlas' }">Felvásárlás</RouterLink>
      <RouterLink to="/eladas" :class="{ active: route.name === 'eladas' }">Eladás</RouterLink>
      <RouterLink to="/egyenleg" :class="{ active: route.name === 'egyenleg' }">Egyenleg</RouterLink>
    </nav>

    <div class="user">
      <button class="theme-button" type="button" :title="darkMode ? 'Világos mód' : 'Sötét mód'" @click="temaValtas">
        {{ darkMode ? '☀️' : '🌙' }} <span class="theme-label">{{ darkMode ? 'Világos' : 'Sötét' }}</span>
      </button>
      <button class="calc-button" title="Számológép" @click="showCalculator = true">🧮 <span class="calc-label">Számológép</span></button>
      <span class="user-email">{{ auth.email }}</span>
      <button class="btn-secondary logout-button" @click="kijelentkezes">Kilépés</button>
    </div>
  </header>

  <button v-else class="public-theme-button" type="button" :title="darkMode ? 'Világos mód' : 'Sötét mód'" @click="temaValtas">
    {{ darkMode ? '☀️' : '🌙' }}
  </button>

  <main :class="{ 'no-header': !bejelentkezve }"><RouterView /></main>

  <div v-if="showInstallPrompt" class="pwa-install-card" role="dialog" aria-label="ZöldPiac telepítése">
    <div class="pwa-install-icon" aria-hidden="true">🥬</div>
    <div class="pwa-install-copy">
      <strong>Telepítsd a ZöldPiacot</strong>
      <span>Gyorsabb elérés, külön alkalmazásként a készülékeden.</span>
    </div>
    <button type="button" class="btn-primary pwa-install-btn" :disabled="installBusy" @click="pwaTelepites">
      {{ installBusy ? 'Telepítés…' : 'Telepítés' }}
    </button>
    <button type="button" class="pwa-dismiss-btn" @click="pwaMostNem">Most nem</button>
  </div>

  <div v-if="showIOSInstallHint" class="pwa-install-card" role="dialog" aria-label="ZöldPiac telepítése">
    <div class="pwa-install-icon" aria-hidden="true">🥬</div>
    <div class="pwa-install-copy">
      <strong>Telepítsd a ZöldPiacot</strong>
      <span>Safariban koppints a Megosztás ikonra (⬆️), majd válaszd: „Kezdőképernyőhöz adás”.</span>
    </div>
    <button type="button" class="pwa-dismiss-btn" @click="pwaMostNem">Értem</button>
  </div>

  <CalculatorModal v-if="showCalculator" @close="showCalculator = false" />
</template>

<style scoped>
header {
  background: linear-gradient(135deg, var(--brand-deep), var(--brand-green));
  color: #fff;
  padding: 12px 20px;
  display: flex;
  align-items: center;
  gap: 18px;
  flex-wrap: wrap;
  position: sticky;
  top: 0;
  z-index: 10;
  border-bottom: 1px solid rgba(160, 230, 62, .28);
  box-shadow: 0 6px 24px rgba(4, 28, 13, .18);
}
.brand { display:flex; align-items:center; gap:10px; min-width:0; }
.brand-logo {
  width: 40px;
  height: 40px;
  flex: 0 0 40px;
  border-radius: 12px;
  background: #050805 url('/brand-zoldpiac.png') center / cover no-repeat;
  border: 1px solid rgba(163, 232, 53, .48);
  box-shadow: inset 0 0 0 1px rgba(255,255,255,.06), 0 5px 14px rgba(0,0,0,.22);
}
.brand-copy { min-width:0; }
.brand-text { font-weight:800; font-size:19px; line-height:1; letter-spacing:-.02em; }
.brand-sub { margin-top:4px; font-size:9px; opacity:.72; text-transform:uppercase; letter-spacing:.08em; white-space:nowrap; overflow:hidden; text-overflow:ellipsis; }
nav { display:flex; gap:5px; flex:1; }
nav a { color:rgba(255,255,255,.72); text-decoration:none; font-weight:700; font-size:13.5px; padding:9px 14px; border-radius:9px; transition:.18s ease; }
nav a:hover { color:#fff; background:rgba(255,255,255,.09); }
nav a.active { background:var(--brand-lime); color:var(--brand-deep); box-shadow:0 5px 14px rgba(124, 207, 26, .28); }
.user { display:flex; align-items:center; gap:8px; font-size:13px; min-width:0; }
.theme-button, .calc-button {
  border:1px solid rgba(255,255,255,.16);
  color:#fff;
  cursor:pointer;
  font-size:14px;
  font-weight:700;
  min-height:40px;
  padding:8px 11px;
  border-radius:10px;
  display:flex;
  align-items:center;
  gap:6px;
  transition:.18s ease;
}
.theme-button { background:rgba(0,0,0,.16); }
.calc-button { background:var(--brand-orange); color:#fff; border-color:transparent; }
.theme-button:hover, .calc-button:hover { transform:translateY(-1px); filter:brightness(1.06); }
.theme-label, .calc-label { font-size:12px; }
.user-email { max-width:190px; overflow:hidden; text-overflow:ellipsis; white-space:nowrap; opacity:.86; }
.logout-button { background:rgba(255,255,255,.08); color:#fff; border-color:rgba(255,255,255,.18); }
.public-theme-button {
  position:fixed;
  top:16px;
  right:16px;
  z-index:20;
  width:44px;
  height:44px;
  border-radius:50%;
  border:1px solid var(--line-strong);
  background:var(--paper);
  color:var(--ink);
  box-shadow:var(--shadow-soft);
}
main { max-width:1160px; margin:0 auto; padding:22px; }
main.no-header { max-width:440px; padding-top:80px; }

.pwa-install-card {
  position:fixed;
  right:18px;
  bottom:18px;
  z-index:100;
  width:min(440px, calc(100vw - 28px));
  display:grid;
  grid-template-columns:auto 1fr auto;
  grid-template-areas:
    "icon copy install"
    "icon copy dismiss";
  gap:10px 12px;
  align-items:center;
  padding:14px;
  border:1px solid var(--line-strong);
  border-radius:16px;
  background:var(--paper);
  color:var(--ink);
  box-shadow:0 14px 36px rgba(0,0,0,.22);
}
.pwa-install-icon {
  grid-area:icon;
  width:46px;
  height:46px;
  display:grid;
  place-items:center;
  border-radius:13px;
  background:var(--brand-lime-soft);
  font-size:24px;
}
.pwa-install-copy { grid-area:copy; min-width:0; display:flex; flex-direction:column; gap:3px; }
.pwa-install-copy strong { color:var(--brand-deep); font-size:14px; }
.pwa-install-copy span { color:var(--muted); font-size:12px; line-height:1.35; }
.pwa-install-btn { grid-area:install; min-height:38px; padding:8px 12px; white-space:nowrap; }
.pwa-dismiss-btn { grid-area:dismiss; border:0; background:transparent; color:var(--muted); font-size:11px; min-height:28px; padding:2px 6px; }
.pwa-dismiss-btn:hover { color:var(--ink); }
:root[data-theme="dark"] .pwa-install-copy strong { color:var(--brand-lime); }

@media (max-width: 840px) {
  header { padding:10px 14px; gap:10px; }
  .brand { flex:1; }
  nav { order:3; width:100%; flex-basis:100%; justify-content:space-between; }
  nav a { flex:1; text-align:center; padding:10px 6px; }
  .user-email { display:none; }
}
@media (max-width:640px) {
  .brand-logo { width:36px; height:36px; flex-basis:36px; border-radius:10px; }
  .brand-text { font-size:17px; }
  .brand-sub { display:none; }
  .theme-label, .calc-label { display:none; }
  .theme-button, .calc-button { min-width:42px; justify-content:center; padding:8px 10px; }
  .logout-button { width:auto !important; min-width:42px; padding:8px 11px !important; }
  main { padding:14px; }
  main.no-header { padding-top:44px; max-width:100%; }
  .public-theme-button { top:10px; right:10px; }
  .pwa-install-card {
    right:10px;
    bottom:10px;
    width:calc(100vw - 20px);
    grid-template-columns:auto 1fr;
    grid-template-areas:
      "icon copy"
      "icon install"
      "icon dismiss";
  }
  .pwa-install-btn { width:100%; }
}
</style>
