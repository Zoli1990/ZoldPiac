<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const auth = useAuthStore()
const router = useRouter()
const email = ref('')
const jelszo = ref('')
const hiba = ref('')
const betolt = ref(false)

async function bejelentkezes() {
  hiba.value = ''
  betolt.value = true
  try {
    await auth.login(email.value.trim(), jelszo.value)
    router.push({ name: 'felvasarlas' })
  } catch (e) {
    hiba.value = e.response?.data?.message || 'Sikertelen bejelentkezés.'
  } finally {
    betolt.value = false
  }
}
</script>

<template>
  <div class="card auth-card">
    <div class="brand-frame">
      <img class="brand" src="/brand-zoldpiac.png" alt="ZöldPiac — friss zöldségek, okosan. Online." />
    </div>
    <h2>Bejelentkezés</h2>
    <form @submit.prevent="bejelentkezes">
      <label>Email cím
        <input v-model="email" type="email" autofocus autocapitalize="none" autocorrect="off" spellcheck="false" autocomplete="email" required />
      </label>
      <label>Jelszó
        <input v-model="jelszo" type="password" autocapitalize="none" autocomplete="current-password" required />
      </label>
      <p v-if="hiba" class="hiba">{{ hiba }}</p>
      <button class="btn-primary" type="submit" :disabled="betolt">
        {{ betolt ? 'Bejelentkezés…' : 'Bejelentkezés' }}
      </button>
    </form>
    <p class="switch"><RouterLink to="/forgot-password">Elfelejtetted a jelszavad?</RouterLink></p>
    <p class="switch">Nincs még fiókod? <RouterLink to="/register">Regisztrálj</RouterLink></p>
  </div>
</template>

<style scoped>
.auth-card { max-width: 420px; margin: 0 auto; }
.brand-frame {
  width: 100%;
  max-width: 220px;
  margin: 0 auto 14px;
  padding: 8px 10px;
  border-radius: 12px;
  background: #000;
  display: flex;
  align-items: center;
  justify-content: center;
}
.brand { display: block; width: 100%; height: auto; }
h2 { margin-top: 0; font-size: 18px; color: var(--chalk-green); text-align: center; }
form { display: flex; flex-direction: column; gap: 12px; }
label { display: flex; flex-direction: column; gap: 5px; font-size: 12.5px; font-weight: 600; color: var(--olive); text-transform: uppercase; letter-spacing: 0.03em; }
button { margin-top: 6px; }
.switch { margin: 16px 0 0; font-size: 13px; text-align: center; }
</style>
