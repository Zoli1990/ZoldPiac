import { defineStore } from 'pinia'
import { ref } from 'vue'
import client from '../api/client'

export const useAuthStore = defineStore('auth', () => {
  const token = ref(localStorage.getItem('token') || '')
  const email = ref(localStorage.getItem('email') || '')
  const role = ref(localStorage.getItem('role') || '')
  const userId = ref(Number(localStorage.getItem('userId') || 0))

  async function login(emailCim, jelszo) {
    const res = await client.post('/auth/login', { email: emailCim, jelszo })
    token.value = res.data.token
    email.value = res.data.email
    role.value = res.data.role
    userId.value = Number(res.data.userId)
    localStorage.setItem('token', token.value)
    localStorage.setItem('email', email.value)
    localStorage.setItem('role', role.value)
    localStorage.setItem('userId', String(userId.value))
  }

  function logout() {
    token.value = ''
    email.value = ''
    role.value = ''
    userId.value = 0
    localStorage.removeItem('token')
    localStorage.removeItem('email')
    localStorage.removeItem('role')
    localStorage.removeItem('userId')
  }

  return { token, email, role, userId, login, logout }
})
