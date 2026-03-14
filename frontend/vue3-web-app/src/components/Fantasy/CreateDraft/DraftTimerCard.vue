<template>
  <div class="timer-card" :class="{ 'timer-card--urgent': isUrgent }">
    <div class="timer-row">
      <font-awesome-icon :icon="['fas', 'clock']" class="timer-icon" />
      <span class="timer-value">{{ formattedTimeLeft }}</span>
    </div>
    <div class="timer-label">TIME LEFT</div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref } from 'vue'

const props = defineProps<{
  targetTime: number
}>()

const timeLeft = ref(0)
let interval: ReturnType<typeof setInterval>

const updateTimer = () => {
  const now = Math.floor(Date.now() / 1000)
  timeLeft.value = Math.max(props.targetTime - now, 0)
}

const isUrgent = computed(() => timeLeft.value > 0 && timeLeft.value < 86400)

const formattedTimeLeft = computed(() => {
  const t = timeLeft.value
  if (t <= 0) return 'LOCKED'
  const d = Math.floor(t / 86400)
  const h = Math.floor((t % 86400) / 3600)
  const m = Math.floor((t % 3600) / 60)
  const s = t % 60
  if (d > 0) return `${d}d ${String(h).padStart(2, '0')}h`
  return `${String(h).padStart(2, '0')}:${String(m).padStart(2, '0')}:${String(s).padStart(2, '0')}`
})

onMounted(() => {
  updateTimer()
  interval = setInterval(updateTimer, 1000)
})

onUnmounted(() => clearInterval(interval))
</script>

<style scoped>
.timer-card {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 4px;
  width: 150px;
  height: 64px;
  background: var(--ot-bg-deep);
  border: 1px solid color-mix(in srgb, var(--ot-border) 25%, transparent);
  border-radius: 8px;
  transition: border-color 0.3s, box-shadow 0.3s, background 0.3s;
}

.timer-card--urgent {
  background: color-mix(in srgb, var(--aghanims-fantasy-accent) 3%, black);
  border: 2px solid var(--aghanims-fantasy-accent);
  box-shadow: 0 0 16px color-mix(in srgb, var(--aghanims-fantasy-accent) 40%, transparent);
}

.timer-row {
  display: flex;
  align-items: center;
  gap: 6px;
}

.timer-icon {
  font-size: var(--text-sm);
  color: color-mix(in srgb, var(--ot-border) 50%, transparent);
}

.timer-card--urgent .timer-icon {
  color: var(--accent-light);
}

.timer-value {
  font-family: var(--font-heading);
  font-size: var(--text-xl);
  font-weight: 800;
  line-height: 1;
  color: color-mix(in srgb, var(--rune-lavender) 85%, transparent);
}

.timer-card--urgent .timer-value {
  color: var(--accent-text);
}

.timer-label {
  font-family: var(--font-body);
  font-size: var(--text-xs);
  font-weight: 700;
  letter-spacing: 2px;
  color: color-mix(in srgb, var(--ot-border) 50%, transparent);
}

.timer-card--urgent .timer-label {
  color: color-mix(in srgb, var(--aghanims-fantasy-accent) 70%, transparent);
}
</style>
