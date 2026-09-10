import axios from 'axios'

const base = import.meta.env.VITE_API_URL || 'http://localhost:5000'
const baseURL = `${base.replace(/\/$/, '')}/api`
const client = axios.create({ baseURL })

client.interceptors.request.use(config => {
  const token = localStorage.getItem('token')
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

client.interceptors.response.use(
  res => res,
  err => {
    if (err.response?.status === 401) {
      localStorage.removeItem('token')
      localStorage.removeItem('email')
      localStorage.removeItem('felhasznalonev')
      localStorage.removeItem('role')
      localStorage.removeItem('userId')
      if (!['/login', '/register', '/verify-email'].includes(location.pathname)) location.href = '/login'
    }
    return Promise.reject(err)
  }
)

export default client
