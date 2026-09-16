import { createApp } from 'vue'
import { createPinia } from 'pinia'
import './style.css'
import './mobile.css'
import App from './App.vue'
import router from './router'

createApp(App).use(createPinia()).use(router).mount('#app')

if ('serviceWorker' in navigator) {
  window.addEventListener('load', () => {
    navigator.serviceWorker.register('/sw.js').catch((error) => {
      console.warn('ZöldPiac PWA service worker nem regisztrálható:', error)
    })
  })
}
