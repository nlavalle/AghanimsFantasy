<template>
  <div class="round-card">
    <div class="round-label">ROUND</div>
    <div class="round-value">{{ current }}<span class="round-denom"> / {{ total }}</span></div>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  current: number
  total: number
}>()
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
  background: #0C0C14;
  border: 1px solid rgba(138, 138, 158, 0.25);
  border-radius: 8px;
}

.round-label {
  font-family: var(--font-body);
  font-size: 8px;
  font-weight: 700;
  letter-spacing: 2px;
  color: rgba(138, 138, 158, 0.5);
}

.round-value {
  font-family: 'Space Grotesk', var(--font-heading);
  font-size: 22px;
  font-weight: 800;
  line-height: 1;
  color: rgba(232, 208, 255, 0.85);
}

.round-denom {
  font-size: 14px;
  font-weight: 600;
  color: rgba(138, 138, 158, 0.5);
}
</style>
