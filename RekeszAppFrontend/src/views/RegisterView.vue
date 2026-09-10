<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import client from '../api/client'

const router = useRouter()
const email = ref('')
const jelszo = ref('')
const jelszo2 = ref('')
const aszf = ref(false)
const hiba = ref('')
const siker = ref('')
const betolt = ref(false)

async function regisztracio() {
  hiba.value = ''
  siker.value = ''
  if (jelszo.value !== jelszo2.value) {
    hiba.value = 'A két jelszó nem egyezik.'
    return
  }
  betolt.value = true
  try {
    const res = await client.post('/auth/register', {
      email: email.value.trim(),
      jelszo: jelszo.value,
      elfogadjaAszf: aszf.value
    })
    siker.value = res.data.message
  } catch (e) {
    hiba.value = e.response?.data?.message || 'A regisztráció nem sikerült.'
  } finally {
    betolt.value = false
  }
}
</script>

<template>
  <div class="card auth-card">
    <h2>ZoldPiac — regisztráció</h2>
    <p class="intro">A szolgáltatás jelenleg ingyenes tesztidőszakban használható.</p>
    <form @submit.prevent="regisztracio">
      <label>Email cím
        <input v-model="email" type="email" autocomplete="email" required :disabled="!!siker" />
      </label>
      <label>Jelszó
        <input v-model="jelszo" type="password" autocomplete="new-password" minlength="8" required :disabled="!!siker" />
      </label>
      <label>Jelszó ismét
        <input v-model="jelszo2" type="password" autocomplete="new-password" minlength="8" required :disabled="!!siker" />
      </label>
      <label class="check">
        <input v-model="aszf" type="checkbox" required :disabled="!!siker" />
        <span>Elfogadom az ÁSZF-et és tudomásul veszem az adatkezelési tájékoztatót.</span>
      </label>
      <p v-if="hiba" class="hiba">{{ hiba }}</p>
      <p v-if="siker" class="siker">{{ siker }}</p>
      <button v-if="!siker" class="btn-primary" type="submit" :disabled="betolt">
        {{ betolt ? 'Regisztráció…' : 'Regisztráció' }}
      </button>
      <RouterLink v-else class="btn-primary" to="/login">Tovább a belépéshez</RouterLink>
    </form>
    <p class="switch">Már van fiókod? <RouterLink to="/login">Bejelentkezés</RouterLink></p>
  </div>
</template>

<style scoped>
.auth-card { max-width: 420px; margin: 0 auto; }
h2 { margin-top: 0; font-size: 18px; color: var(--chalk-green); }
.intro { font-size: 13px; line-height: 1.45; margin-top: -4px; }
form { display: flex; flex-direction: column; gap: 12px; }
label { display: flex; flex-direction: column; gap: 5px; font-size: 12.5px; font-weight: 600; color: var(--olive); text-transform: uppercase; letter-spacing: 0.03em; }
.check { flex-direction: row; align-items: flex-start; gap: 8px; text-transform: none; letter-spacing: 0; font-weight: 500; }
.check input { margin-top: 2px; }
button, .btn-primary { margin-top: 6px; text-align: center; text-decoration: none; }
.switch { margin: 16px 0 0; font-size: 13px; text-align: center; }
.siker { padding: 10px; border-radius: 8px; background: #eaf5ea; }
</style>
