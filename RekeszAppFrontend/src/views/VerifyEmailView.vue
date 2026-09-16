<script setup>
import { onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import client from '../api/client'

const route = useRoute()
const router = useRouter()
const uzenet = ref('Email-cím visszaigazolása folyamatban…')
const hiba = ref(false)

onMounted(async () => {
  const token = String(route.query.token || '')
  if (!token) {
    hiba.value = true
    uzenet.value = 'Hiányzik a visszaigazoló token.'
    return
  }

  try {
    const res = await client.get('/auth/verify-email', { params: { token } })
    uzenet.value = res.data.message
    setTimeout(() => router.push('/login'), 1800)
  } catch (e) {
    hiba.value = true
    uzenet.value = e.response?.data?.message || 'A visszaigazolás nem sikerült.'
  }
})
</script>

<template>
  <div class="card auth-card">
    <img class="brand" src="/brand-zoldpiac.png" alt="ZöldPiac — friss zöldségek, okosan. Online." />
    <h2>Email visszaigazolás</h2>
    <p :class="{ hiba, siker: !hiba }">{{ uzenet }}</p>
    <RouterLink class="btn-primary" to="/login">Belépés</RouterLink>
  </div>
</template>

<style scoped>
.auth-card { max-width: 420px; margin: 0 auto; }
.brand { display: block; width: 100%; max-width: 220px; margin: 0 auto 14px; height: auto; }
h2 { margin-top: 0; font-size: 18px; color: var(--chalk-green); text-align: center; }
p { line-height: 1.5; padding: 10px; border-radius: 8px; }
.siker { background: #eaf5ea; }
.hiba { background: #faece8; }
.btn-primary { display: block; text-align: center; text-decoration: none; margin-top: 12px; }
</style>
