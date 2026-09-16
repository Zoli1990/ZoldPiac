<script setup>
import { ref } from 'vue'
import client from '../api/client'

const email = ref('')
const uzenet = ref('')
const hiba = ref('')
const betolt = ref(false)
const elkuldve = ref(false)

async function kuldes() {
  hiba.value = ''
  uzenet.value = ''
  betolt.value = true
  try {
    const res = await client.post('/auth/forgot-password', { email: email.value.trim() })
    uzenet.value = res.data.message
    elkuldve.value = true
  } catch (e) {
    hiba.value = e.response?.data?.message || 'A kérés nem sikerült.'
  } finally {
    betolt.value = false
  }
}
</script>

<template>
  <div class="card auth-card">
    <img class="brand" src="/brand-zoldpiac.png" alt="ZöldPiac" />
    <h2>Elfelejtett jelszó</h2>
    <p class="intro">Add meg az email-címed, és küldünk egy linket, amivel új jelszót állíthatsz be.</p>
    <form @submit.prevent="kuldes">
      <label>Email cím
        <input v-model="email" type="email" autocomplete="email" required :disabled="elkuldve" />
      </label>
      <p v-if="hiba" class="hiba">{{ hiba }}</p>
      <p v-if="uzenet" class="siker">{{ uzenet }}</p>
      <button v-if="!elkuldve" class="btn-primary" type="submit" :disabled="betolt">
        {{ betolt ? 'Küldés…' : 'Visszaállító link kérése' }}
      </button>
    </form>
    <p class="switch"><RouterLink to="/login">Vissza a belépéshez</RouterLink></p>
  </div>
</template>

<style scoped>
.auth-card { max-width: 420px; margin: 0 auto; }
.brand { display: block; width: 100%; max-width: 220px; margin: 0 auto 14px; height: auto; }
h2 { margin-top: 0; font-size: 18px; color: var(--chalk-green); text-align: center; }
.intro { font-size: 13px; line-height: 1.45; text-align: center; }
form { display: flex; flex-direction: column; gap: 12px; }
label { display: flex; flex-direction: column; gap: 5px; font-size: 12.5px; font-weight: 600; color: var(--olive); text-transform: uppercase; letter-spacing: 0.03em; }
button { margin-top: 6px; }
.switch { margin: 16px 0 0; font-size: 13px; text-align: center; }
.siker { padding: 10px; border-radius: 8px; background: #eaf5ea; }
</style>
