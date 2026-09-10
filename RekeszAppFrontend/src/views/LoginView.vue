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
    <h2>ZoldPiac — bejelentkezés</h2>
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
    <p class="switch">Nincs még fiókod? <RouterLink to="/register">Regisztrálj</RouterLink></p>
  </div>
</template>

<style scoped>
.auth-card { max-width: 420px; margin: 0 auto; }
h2 { margin-top: 0; font-size: 18px; color: var(--chalk-green); }
form { display: flex; flex-direction: column; gap: 12px; }
label { display: flex; flex-direction: column; gap: 5px; font-size: 12.5px; font-weight: 600; color: var(--olive); text-transform: uppercase; letter-spacing: 0.03em; }
button { margin-top: 6px; }
.switch { margin: 16px 0 0; font-size: 13px; text-align: center; }
</style>
