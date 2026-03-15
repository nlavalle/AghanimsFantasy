<template>
  <div class="bar-row">
    <span class="bar-label">{{ label }}</span>
    <div class="bar-track">
      <div class="bar-fill" :style="{ width: percentage + '%' }" :data-percentage="getPercentageRange(percentage)" />
    </div>
    <span class="bar-value">{{ value.toFixed(1) }}</span>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps<{
  label: string
  value: number
  max: number
}>()

const percentage = computed(() => Math.min(100, (props.value / 100) * 100))

const getPercentageRange = (percentage: number) => {
  if (percentage < 33) return "0-33";
  if (percentage < 66) return "33-66";
  return "66-100";
};

</script>

<style scoped>
.bar-row {
  display: flex;
  align-items: center;
  gap: 8px;
  width: 100%;
}

.bar-label {
  width: 60px;
  font-family: var(--font-body);
  font-size: 10px;
  font-weight: 400;
  color: rgba(232, 224, 200, 0.47);
  flex-shrink: 0;
}

.bar-track {
  flex: 1;
  height: 6px;
  border-radius: 3px;
  background: #1A0A2E;
  overflow: hidden;
}

.bar-fill {
  height: 100%;
  border-radius: 3px;
  /* background: var(--rune-purple); */
  transition: width 0.4s ease;

  /* Default to dark purple */
  background-color: var(--rune-purple-dark);

  /* Change color based on percentage */
  &[data-percentage="0-33"] {
    background-color: var(--rune-purple-dark);
    /* Dark purple */
  }

  &[data-percentage="33-66"] {
    background-color: var(--rune-purple);
    /* Medium purple */
  }

  &[data-percentage="66-100"] {
    background-color: var(--rune-purple-light);
    /* Light purple */
  }
}

.bar-value {
  font-family: var(--font-stats);
  font-size: 10px;
  font-weight: 600;
  color: var(--rune-purple-light);
  min-width: 30px;
  text-align: right;
}
</style>
