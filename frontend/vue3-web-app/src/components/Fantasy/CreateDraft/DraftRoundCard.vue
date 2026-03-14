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
  background: var(--ot-bg-deep);
  border: 1px solid color-mix(in srgb, var(--ot-border) 25%, transparent);
  border-radius: 8px;
}

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
</style>
