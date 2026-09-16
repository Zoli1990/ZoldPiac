import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue()],
  build: {
    // A publikuson (runasp.net) a webhely gyökere "wwwroot" néven várja a
    // statikus fájlokat, így közvetlenül ebbe épít a "dist" helyett - nem
    // kell buildenkénti átnevezés/másolás, és a public/web.config is
    // automatikusan bekerül minden build kimenetébe.
    outDir: 'wwwroot',
    emptyOutDir: true,
  },
})
