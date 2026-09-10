import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue'
import VerifyEmailView from '../views/VerifyEmailView.vue'
import FelvasarlasView from '../views/FelvasarlasView.vue'
import EladasView from '../views/EladasView.vue'
import EgyenlegView from '../views/EgyenlegView.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/login', name: 'login', component: LoginView, meta: { public: true } },
    { path: '/register', name: 'register', component: RegisterView, meta: { public: true } },
    { path: '/verify-email', name: 'verify-email', component: VerifyEmailView, meta: { public: true } },
    { path: '/', name: 'felvasarlas', component: FelvasarlasView },
    { path: '/eladas', name: 'eladas', component: EladasView },
    { path: '/egyenleg', name: 'egyenleg', component: EgyenlegView }
  ]
})

router.beforeEach((to) => {
  const auth = useAuthStore()
  if (!to.meta.public && !auth.token) return { name: 'login' }
  if ((to.name === 'login' || to.name === 'register') && auth.token) return { name: 'felvasarlas' }
})

export default router
