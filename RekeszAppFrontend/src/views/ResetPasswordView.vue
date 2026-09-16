<script setup>
import { ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import client from '../api/client'

const route = useRoute()
const router = useRouter()
const token = String(route.query.token || '')
const jelszo = ref('')
const jelszo2 = ref('')
const hiba = ref(token ? '' : 'Hiányzik a visszaállító token.')
const siker = ref('')
const betolt = ref(false)

async function kuldes() {
  hiba.value = ''
  siker.value = ''
  if (jelszo.value !== jelszo2.value) {
    hiba.value = 'A két jelszó nem egyezik.'
    return
  }
  betolt.value = true
  try {
    const res = await client.post('/auth/reset-password', { token, ujJelszo: jelszo.value })
    siker.value = res.data.message
    setTimeout(() => router.push('/login'), 1800)
  } catch (e) {
    hiba.value = e.response?.data?.message || 'A jelszó módosítása nem sikerült.'
  } finally {
    betolt.value = false
  }
}
</script>

<template>
  <div class="card auth-card">
    <img class="brand" src="/brand-zoldpiac.png" alt="ZöldPiac" />
    <h2>Új jelszó beállítása</h2>
    <form v-if="token" @submit.prevent="kuldes">
      <label>Új jelszó
        <input v-model="jelszo" type="password" autocomplete="new-password" minlength="8" required :disabled="!!siker" />
      </label>
      <label>Új jelszó ismét
        <input v-model="jelszo2" type="password" autocomplete="new-password" minlength="8" required :disabled="!!siker" />
      </label>
      <p v-if="hiba" class="hiba">{{ hiba }}</p>
      <p v-if="siker" class="siker">{{ siker }}</p>
      <button v-if="!siker" class="btn-primary" type="submit" :disabled="betolt">
        {{ betolt ? 'Mentés…' : 'Jelszó mentése' }}
      </button>
    </form>
    <p v-else class="hiba">{{ hiba }}</p>
    <p class="switch"><RouterLink to="/login">Vissza a belépéshez</RouterLink></p>
  </div>
</template>

<style scoped>
.auth-card { max-width: 420px; margin: 0 auto; }
.brand { display: block; width: 100%; max-width: 220px; margin: 0 auto 14px; height: auto; }
h2 { margin-top: 0; font-size: 18px; color: var(--chalk-green); text-align: center; }
form { display: flex; flex-direction: column; gap: 12px; }
label { display: flex; flex-direction: column; gap: 5px; font-size: 12.5px; font-weight: 600; color: var(--olive); text-transform: uppercase; letter-spacing: 0.03em; }
button { margin-top: 6px; }
.switch { margin: 16px 0 0; font-size: 13px; text-align: center; }
.siker { padding: 10px; border-radius: 8px; background: #eaf5ea; }
</style>
