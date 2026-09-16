<script setup>
import { computed, ref } from 'vue'

const emit = defineEmits(['close'])
const display = ref('0')
const previous = ref(null)
const operator = ref(null)
const waiting = ref(false)
const expression = ref('')
const afterEquals = ref(false)

const buttons = [
  ['AC', 'clear'], ['+/-', 'sign'], ['%', 'percent'], ['÷', 'operator'],
  ['7', 'digit'], ['8', 'digit'], ['9', 'digit'], ['×', 'operator'],
  ['4', 'digit'], ['5', 'digit'], ['6', 'digit'], ['−', 'operator'],
  ['1', 'digit'], ['2', 'digit'], ['3', 'digit'], ['+', 'operator'],
  ['0', 'zero'], ['.', 'digit'], ['=', 'equals']
]

const number = computed(() => Number(display.value.replace(',', '.')))

function formazSzamot(value) {
  return String(Number(value))
}

function inputDigit(d) {
  if (afterEquals.value) {
    display.value = d
    previous.value = null
    operator.value = null
    expression.value = ''
    waiting.value = false
    afterEquals.value = false
    return
  }

  if (waiting.value) {
    display.value = d
    waiting.value = false
    return
  }

  if (display.value === '0') display.value = d
  else if (display.value.length < 12) display.value += d
}

function inputDecimal() {
  if (afterEquals.value) {
    display.value = '0.'
    previous.value = null
    operator.value = null
    expression.value = ''
    waiting.value = false
    afterEquals.value = false
    return
  }

  if (waiting.value) {
    display.value = '0.'
    waiting.value = false
    return
  }

  if (!display.value.includes('.')) display.value += '.'
}

function clear() {
  display.value = '0'
  previous.value = null
  operator.value = null
  waiting.value = false
  expression.value = ''
  afterEquals.value = false
}

function sign() {
  if (display.value !== '0' && display.value !== 'Error') {
    display.value = display.value.startsWith('-') ? display.value.slice(1) : '-' + display.value
  }
}

function percent() {
  display.value = String(number.value / 100)
}

function calculate(a, b, op) {
  if (op === '+') return a + b
  if (op === '−') return a - b
  if (op === '×') return a * b
  if (op === '÷') return b === 0 ? NaN : a / b
  return b
}

function chooseOperator(op) {
  if (display.value === 'Error') {
    clear()
    return
  }

  const current = number.value

  if (afterEquals.value) {
    expression.value = `${formazSzamot(current)}${op}`
    previous.value = current
    operator.value = op
    waiting.value = true
    afterEquals.value = false
    return
  }

  if (operator.value && waiting.value) {
    expression.value = expression.value.slice(0, -1) + op
    operator.value = op
    return
  }

  if (operator.value && previous.value !== null) {
    const result = calculate(previous.value, current, operator.value)
    if (!Number.isFinite(result)) {
      display.value = 'Error'
      expression.value = `${expression.value}${formazSzamot(current)}=`
      previous.value = null
      operator.value = null
      waiting.value = true
      afterEquals.value = true
      return
    }
    previous.value = result
    display.value = formazSzamot(result)
    expression.value = `${expression.value}${formazSzamot(current)}${op}`
  } else {
    previous.value = current
    expression.value = `${formazSzamot(current)}${op}`
  }

  operator.value = op
  waiting.value = true
}

function equals() {
  if (!operator.value || previous.value === null || display.value === 'Error') return

  const current = number.value
  expression.value = `${expression.value}${formazSzamot(current)}=`

  const result = calculate(previous.value, current, operator.value)
  display.value = Number.isFinite(result) ? formazSzamot(result) : 'Error'
  previous.value = null
  operator.value = null
  waiting.value = true
  afterEquals.value = true
}

function press(value, type) {
  if (display.value === 'Error' && type !== 'clear') clear()
  if (type === 'digit') value === '.' ? inputDecimal() : inputDigit(value)
  else if (type === 'zero') inputDigit('0')
  else if (type === 'clear') clear()
  else if (type === 'sign') sign()
  else if (type === 'percent') percent()
  else if (type === 'operator') chooseOperator(value)
  else if (type === 'equals') equals()
}
</script>

<template>
  <div class="calc-overlay" @click.self="emit('close')">
    <div class="calculator" role="dialog" aria-label="Számológép">
      <div class="calc-top">
        <button type="button" @click="emit('close')">Kész</button>
      </div>
      <div class="expression">{{ expression }}</div>
      <div class="display">{{ display }}</div>
      <div class="keys">
        <button v-for="([value, type], i) in buttons" :key="i" :class="[type]" @click="press(value, type)">{{ value }}</button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.calc-overlay { position:fixed; inset:0; z-index:100; background:rgba(6,24,12,.56); display:flex; align-items:center; justify-content:center; padding:20px; backdrop-filter:blur(3px); }
.calculator { width:min(360px,100%); background:var(--paper); color:var(--ink); border:1px solid var(--line); border-radius:16px; padding:16px; box-shadow:var(--shadow); }
.calc-top { min-height:28px; display:flex; justify-content:flex-end; align-items:center; }
.calc-top button { border:0; background:transparent; color:var(--brand-green); font-size:14px; cursor:pointer; font-weight:700; padding:5px 7px; }
.expression { min-height:28px; text-align:right; padding:4px 8px 0; font-size:18px; color:var(--muted); overflow:hidden; white-space:nowrap; text-overflow:ellipsis; font-variant-numeric:tabular-nums; }
.display { color:var(--ink); height:66px; display:flex; align-items:flex-end; justify-content:flex-end; padding:0 8px 8px; font-size:42px; font-weight:700; overflow:hidden; font-variant-numeric:tabular-nums; }
.keys { display:grid; grid-template-columns:repeat(4,1fr); gap:8px; }
.keys button { height:58px; border:1px solid var(--line); border-radius:11px; font-size:21px; font-weight:700; cursor:pointer; background:var(--paper-warm); color:var(--ink); }
.keys button:active { transform:translateY(1px); }
.keys .clear,.keys .sign,.keys .percent { background:var(--paper-dim); color:var(--brand-deep); }
.keys .operator { background:var(--brand-green); color:#fff; border-color:var(--brand-green); }
.keys .equals { background:var(--brand-orange); color:#fff; border-color:var(--brand-orange); }
.keys .zero { grid-column:span 2; text-align:left; padding-left:22px; }
</style>
