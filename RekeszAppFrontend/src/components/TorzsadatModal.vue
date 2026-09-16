<script setup>
import { ref, onMounted } from 'vue'
import client from '../api/client'

const props = defineProps({
  title: { type: String, required: true },
  apiPath: { type: String, required: true },
  nevKotelezo: { type: Boolean, default: true },
  nevLabel: { type: String, default: 'Név' },
  nevPlaceholder: { type: String, default: '' },
  mutatMegjegyzes: { type: Boolean, default: true }
})
const emit = defineEmits(['close', 'changed'])
const lista = ref([]), hiba = ref(''), betolt = ref(true)
const ujNev = ref(''), ujMegjegyzes = ref(''), szerkesztId = ref(null), szerkNev = ref(''), szerkMegjegyzes = ref('')

async function frissit() {
  betolt.value = true; hiba.value = ''
  try { lista.value = (await client.get(props.apiPath)).data }
  catch { hiba.value = 'Nem sikerült betölteni a listát.' }
  finally { betolt.value = false }
}
onMounted(frissit)

async function ujFelvitel() {
  if (props.nevKotelezo && !ujNev.value.trim()) { hiba.value = 'A név kötelező.'; return }
  hiba.value = ''
  try {
    await client.post(props.apiPath, { nev: ujNev.value.trim() || null, megjegyzes: ujMegjegyzes.value.trim() || null })
    ujNev.value = ''; ujMegjegyzes.value = ''
    await frissit(); emit('changed')
  } catch (e) { hiba.value = e.response?.data?.message || 'Mentés sikertelen.' }
}
function szerkesztesInditasa(item) { szerkesztId.value = item.id; szerkNev.value = item.nev || ''; szerkMegjegyzes.value = item.megjegyzes || '' }
function megse() { szerkesztId.value = null }
async function mentSzerkesztes(id) {
  if (props.nevKotelezo && !szerkNev.value.trim()) { hiba.value = 'A név kötelező.'; return }
  hiba.value = ''
  try {
    await client.put(`${props.apiPath}/${id}`, { nev: szerkNev.value.trim() || null, megjegyzes: szerkMegjegyzes.value.trim() || null })
    szerkesztId.value = null; await frissit(); emit('changed')
  } catch (e) { hiba.value = e.response?.data?.message || 'Mentés sikertelen.' }
}
async function torol(item) {
  if (!confirm(`Biztosan törlöd: \"${item.nev || '(névtelen)'}\"?`)) return
  hiba.value = ''
  try { await client.delete(`${props.apiPath}/${item.id}`); await frissit(); emit('changed') }
  catch (e) { hiba.value = e.response?.data?.message || 'Törlés sikertelen.' }
}
</script>

<template>
  <div class="overlay" @click.self="emit('close')">
    <div class="modal">
      <div class="modal-head"><h3>{{ title }}</h3><button class="close" type="button" @click="emit('close')">✕</button></div>
      <p v-if="hiba" class="hiba">{{ hiba }}</p><p v-if="betolt">Betöltés…</p>
      <ul v-else class="lista">
        <li v-for="item in lista" :key="item.id">
          <template v-if="szerkesztId === item.id">
            <div class="edit-fields">
              <input v-model="szerkNev" :placeholder="nevPlaceholder || nevLabel" />
              <input v-if="mutatMegjegyzes" v-model="szerkMegjegyzes" placeholder="Megjegyzés (opcionális)" />
            </div>
            <div class="actions"><button class="action-btn save" type="button" title="Mentés" @click="mentSzerkesztes(item.id)">✓</button><button class="action-btn cancel" type="button" title="Mégse" @click="megse">✕</button></div>
          </template>
          <template v-else>
            <div class="item-info"><span class="nev">{{ item.nev || '(névtelen)' }}</span><span v-if="item.megjegyzes" class="megj">{{ item.megjegyzes }}</span></div>
            <div class="actions"><button class="action-btn" type="button" title="Szerkesztés" @click="szerkesztesInditasa(item)">✏️</button><button class="action-btn danger" type="button" title="Törlés" @click="torol(item)">🗑️</button></div>
          </template>
        </li>
        <li v-if="!lista.length" class="ures">Még nincs felvéve semmi.</li>
      </ul>
      <div class="uj-form">
        <input v-model="ujNev" :placeholder="nevPlaceholder || nevLabel" @keyup.enter="ujFelvitel" />
        <input v-if="mutatMegjegyzes" v-model="ujMegjegyzes" placeholder="Megjegyzés (opcionális)" @keyup.enter="ujFelvitel" />
        <button class="add" type="button" @click="ujFelvitel">+ Új</button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.overlay{position:fixed;inset:0;background:rgba(2,15,7,.56);backdrop-filter:blur(3px);display:flex;align-items:center;justify-content:center;z-index:50;padding:12px}.modal{background:var(--paper);border:1px solid var(--line);border-radius:16px;padding:18px;width:100%;max-width:480px;max-height:90vh;overflow-y:auto;box-shadow:var(--shadow)}.modal-head{display:flex;justify-content:space-between;align-items:center;margin-bottom:14px}.modal-head h3{margin:0;font-size:19px;color:var(--brand-deep)}.close{background:var(--paper-warm);border:1px solid var(--line);border-radius:10px;font-size:20px;cursor:pointer;padding:6px;color:var(--muted);min-width:42px}.close:hover{color:var(--brand-orange)}.hiba{color:var(--owe);font-size:13px;margin:4px 0 10px}.lista{list-style:none;margin:0 0 14px;padding:0}.lista li{display:flex;align-items:center;gap:10px;padding:11px 2px;border-bottom:1px solid var(--line)}.item-info{min-width:0;flex:1;display:flex;flex-direction:column;gap:3px}.nev{font-weight:750;color:var(--ink)}.megj{font-size:12px;color:var(--muted);overflow-wrap:anywhere}.edit-fields{display:grid;gap:8px;flex:1;min-width:0}.lista input,.uj-form input{box-sizing:border-box;width:100%;padding:11px 12px;border:1px solid var(--line);border-radius:10px;background:var(--paper);color:var(--ink);font-size:14px}.actions{display:flex;gap:6px;flex-shrink:0}.action-btn{width:42px;height:42px;border:1px solid var(--line-strong);border-radius:10px;background:var(--paper-warm);color:var(--ink);cursor:pointer;font-size:17px;display:flex;align-items:center;justify-content:center}.action-btn:hover{border-color:var(--brand-lime);background:var(--brand-lime-soft)}.action-btn.save{background:var(--brand-green);border-color:var(--brand-green);color:#fff}.action-btn.danger{color:var(--brand-orange-dark)}.action-btn.cancel{color:var(--muted)}.ures{color:var(--muted);text-align:center;padding:18px 4px}.uj-form{display:grid;grid-template-columns:1fr auto;gap:8px}.uj-form input:nth-child(2){grid-column:1/-1}.add{min-height:44px;background:linear-gradient(135deg,var(--brand-orange),var(--brand-orange-dark));color:#fff;border:0;border-radius:10px;padding:0 16px;font-weight:750;cursor:pointer}.uj-form input:nth-child(1):only-child + .add{grid-column:2}
@media(max-width:520px){.overlay{padding:8px;align-items:flex-end}.modal{max-width:none;max-height:92vh;border-radius:16px 16px 8px 8px;padding:16px}.lista li{align-items:flex-start}.action-btn{width:46px;height:46px}.uj-form{grid-template-columns:1fr}.add{width:100%;min-height:46px}.uj-form input:nth-child(2){grid-column:auto}}
</style>
