<template>
  <div class="budget-card" :class="{ 'budget-card--over': isOver }">
    <div class="budget-row">
      <font-awesome-icon :icon="['fas', 'coins']" class="budget-icon" />
      <span class="budget-value">{{ remaining }}</span>
      <span class="budget-denom">/ {{ budget }}</span>
    </div>
    <div class="budget-label">GOLD LEFT</div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { fantasyDraftState, DRAFT_BUDGET } from '../fantasyDraft'
import { useFantasyLeagueStore } from '@/stores/fantasyLeague'

const BUDGET = DRAFT_BUDGET

const leagueStore = useFantasyLeagueStore()
const { totalDraftCost } = fantasyDraftState()

const spent = computed(() => totalDraftCost(leagueStore.fantasyPlayersStats))
const remaining = computed(() => Math.max(BUDGET - spent.value, 0).toFixed(0))
const budget = BUDGET
const isOver = computed(() => spent.value > BUDGET)
</script>

<style scoped>
.budget-card {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 4px;
  width: 130px;
  height: 64px;
  background: var(--ot-bg-deep);
  border: 1px solid color-mix(in srgb, var(--ot-border) 25%, transparent);
  border-radius: 8px;
  transition: border-color 0.3s, box-shadow 0.3s, background 0.3s;
}

.budget-card--over {
  background: color-mix(in srgb, var(--aghanims-fantasy-accent) 3%, black);
  border-color: var(--aghanims-fantasy-accent);
  box-shadow: 0 0 16px color-mix(in srgb, var(--aghanims-fantasy-accent) 40%, transparent);
}

.budget-row {
  display: flex;
  align-items: center;
  gap: 5px;
}

.budget-icon {
  font-size: var(--text-sm);
  color: var(--sg-text-dim);
}

.budget-card--over .budget-icon {
  color: var(--accent-light);
}

.budget-value {
  font-family: var(--font-heading);
  font-size: var(--text-xl);
  font-weight: 800;
  line-height: 1;
  color: color-mix(in srgb, var(--rune-lavender) 85%, transparent);
}

.budget-card--over .budget-value {
  color: var(--accent-text);
}

.budget-denom {
  font-size: var(--text-sm);
  font-weight: 600;
  color: color-mix(in srgb, var(--ot-border) 50%, transparent);
  align-self: flex-end;
  padding-bottom: 2px;
}

.budget-label {
  font-family: var(--font-body);
  font-size: var(--text-xs);
  font-weight: 700;
  letter-spacing: 2px;
  color: color-mix(in srgb, var(--ot-border) 50%, transparent);
}

.budget-card--over .budget-label {
  color: color-mix(in srgb, var(--aghanims-fantasy-accent) 70%, transparent);
}
</style>
