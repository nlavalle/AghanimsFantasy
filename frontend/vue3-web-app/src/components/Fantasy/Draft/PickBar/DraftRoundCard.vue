<template>
  <div class="round-card" :class="{ 'round-card--alert': hasUndraftedOpenRound }" @click="open = !open" ref="cardRef">
    <div class="round-label">ROUND</div>
    <div class="round-value">
      {{ current }}<span class="round-denom"> / {{ total }}</span>
    </div>

    <Teleport to="body">
      <div v-if="open && rounds.length > 1" class="round-dropdown"
        :style="{ top: dropdownPos.y + 'px', left: dropdownPos.x + 'px' }" @click.stop>
        <button v-for="(round, idx) in rounds" :key="round.id" class="round-option" :class="{
          'round-option--active': idx + 1 === current,
          'round-option--locked': !isDraftOpen(round)
        }" @click="select(idx, round)">
          <span class="option-label">Round {{ idx + 1 }}</span>
          <span class="option-name">{{ round.name }}</span>
          <span class="option-status">{{ isDraftOpen(round) ? 'Open' : 'Locked' }}</span>
        </button>
      </div>
    </Teleport>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted, onUnmounted } from 'vue'
import type { FantasyLeague } from '@/types/FantasyLeague'
import { isDraftOpen } from '@/stores/fantasyLeagueUtils'

const props = defineProps<{
  current: number
  total: number
  rounds: FantasyLeague[]
  hasUndraftedOpenRound?: boolean
}>()

const emit = defineEmits<{
  selectRound: [fantasyLeagueId: number]
}>()

const open = ref(false)
const cardRef = ref<HTMLElement | null>(null)
const dropdownPos = ref({ x: 0, y: 0 })

function updateDropdownPos() {
  if (!cardRef.value) return
  const rect = cardRef.value.getBoundingClientRect()
  dropdownPos.value = { x: rect.left, y: rect.bottom + 6 }
}

watch(open, (val) => {
  if (val) updateDropdownPos()
})

function select(_idx: number, round: FantasyLeague) {
  emit('selectRound', round.id)
  open.value = false
}

function onOutsideClick(e: MouseEvent) {
  if (cardRef.value && !cardRef.value.contains(e.target as Node)) {
    open.value = false
  }
}

onMounted(() => document.addEventListener('click', onOutsideClick))
onUnmounted(() => document.removeEventListener('click', onOutsideClick))
</script>

<style scoped>
.round-card {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 4px;
  width: 90px;
  height: 64px;
  background: var(--ot-bg-deep);
  border: 1px solid color-mix(in srgb, var(--ot-border) 25%, transparent);
  border-radius: 8px;
  cursor: pointer;
  transition: border-color 0.15s;
}

.round-card:hover {
  border-color: color-mix(in srgb, var(--rune-lavender) 50%, transparent);
}

.round-card--alert {
  border-color: color-mix(in srgb, var(--rune-gold) 60%, transparent);
  animation: round-card-pulse 2s ease-in-out infinite;
  box-shadow: 0 0 6px 3px color-mix(in srgb, var(--rune-gold) 35%, transparent);
}

.round-card--alert:hover {
  border-color: color-mix(in srgb, var(--rune-gold) 90%, transparent);
  box-shadow: 0 0 12px 3px color-mix(in srgb, var(--rune-gold) 35%, transparent);
}

/* @keyframes round-card-pulse {

  0%,
  100% {
    box-shadow: 0 0 0 0 color-mix(in srgb, var(--rune-gold) 0%, transparent);
  }

  50% {
    box-shadow: 0 0 12px 3px color-mix(in srgb, var(--rune-gold) 35%, transparent);
  }
} */

.round-label {
  font-family: var(--font-body);
  font-size: var(--text-sm);
  font-weight: 700;
  letter-spacing: 2px;
  color: color-mix(in srgb, var(--rune-lavender) 85%, transparent);
}

.round-value {
  font-family: var(--font-heading);
  font-size: var(--text-xl);
  font-weight: 800;
  line-height: 1;
  color: color-mix(in srgb, var(--rune-lavender) 85%, transparent);
}

.round-denom {
  font-size: var(--text-md);
  font-weight: 600;
  color: color-mix(in srgb, var(--ot-border) 50%, transparent);
}

.round-dropdown {
  position: fixed;
  z-index: 1000;
  background: var(--ot-bg-deep);
  border: 1px solid color-mix(in srgb, var(--ot-border) 30%, transparent);
  border-radius: 8px;
  padding: 4px;
  min-width: 180px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.6);
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.round-option {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 8px 10px;
  border-radius: 6px;
  border: none;
  background: transparent;
  cursor: pointer;
  text-align: left;
  transition: background 0.1s;
  width: 100%;
}

.round-option:hover {
  background: color-mix(in srgb, var(--rune-lavender) 10%, transparent);
}

.round-option--active {
  background: color-mix(in srgb, var(--rune-lavender) 15%, transparent);
}

.round-option--locked {
  opacity: 0.5;
  cursor: default;
}

.option-label {
  font-family: var(--font-stats);
  font-size: var(--text-sm);
  font-weight: 700;
  color: color-mix(in srgb, var(--rune-lavender) 85%, transparent);
  min-width: 52px;
}

.option-name {
  font-family: var(--font-body);
  font-size: var(--text-sm);
  color: var(--ot-text-dim);
  flex: 1;
}

.option-status {
  font-family: var(--font-body);
  font-size: var(--text-xs);
  color: color-mix(in srgb, var(--rune-lavender) 50%, transparent);
}

.round-option--locked .option-status {
  color: color-mix(in srgb, var(--rune-gray) 60%, transparent);
}
</style>
