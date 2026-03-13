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
  background: #0C0C14;
  border: 1px solid rgba(138, 138, 158, 0.25);
  border-radius: 8px;
  transition: border-color 0.3s, box-shadow 0.3s, background 0.3s;
}

.timer-card--urgent {
  background: #0A0300;
  border: 2px solid #E53935;
  box-shadow: 0 0 16px rgba(229, 57, 53, 0.4);
}

.timer-row {
  display: flex;
  align-items: center;
  gap: 6px;
}

.timer-icon {
  font-size: 14px;
  color: rgba(138, 138, 158, 0.5);
}

.timer-card--urgent .timer-icon {
  color: #FF6B6B;
}

.timer-value {
  font-family: 'Space Grotesk', var(--font-heading);
  font-size: 20px;
  font-weight: 800;
  line-height: 1;
  color: rgba(232, 208, 255, 0.85);
}

.timer-card--urgent .timer-value {
  color: #FFD0CC;
}

.timer-label {
  font-family: var(--font-body);
  font-size: 8px;
  font-weight: 700;
  letter-spacing: 2px;
  color: rgba(138, 138, 158, 0.5);
}

.timer-card--urgent .timer-label {
  color: rgba(229, 57, 53, 0.7);
}
</style>
