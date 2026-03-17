<template>
  <div class="control-bar">
    <span class="league-name">{{ leagueStore.selectedLeague?.name ?? '—' }}</span>
    <div class="round-pills" v-if="rounds.length > 0">
      <button class="round-pill" :class="{ active: showAllRounds }" @click="emit('selectAllRounds')">
        All Rounds
      </button>
      <button
        v-for="(round, idx) in rounds"
        :key="round.id"
        class="round-pill"
        :class="{ active: !showAllRounds && activeRoundIndex === idx }"
        @click="emit('selectRound', idx)"
      >
        Round {{ idx + 1 }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';
import type { FantasyLeague } from '@/types/FantasyLeague';

defineProps<{
  rounds: FantasyLeague[]
  showAllRounds: boolean
  activeRoundIndex: number
}>()

const emit = defineEmits<{
  selectAllRounds: []
  selectRound: [idx: number]
}>()

const leagueStore = useFantasyLeagueStore();
</script>

<style scoped>
.control-bar {
  display: flex;
  align-items: center;
  gap: var(--space-md);
  flex-wrap: wrap;
}

.league-name {
  font-family: var(--font-heading);
  font-size: var(--text-base);
  font-weight: 600;
  color: var(--rune-gold-dim);
}

.round-pills {
  display: flex;
  gap: var(--space-xs);
  flex-wrap: wrap;
}

.round-pill {
  padding: 4px 12px;
  border-radius: 99px;
  border: 1px solid rgba(255, 255, 255, 0.12);
  background: transparent;
  color: var(--rune-gray-light);
  font-family: var(--font-body);
  font-size: var(--text-sm);
  cursor: pointer;
  transition: background 0.15s, color 0.15s, border-color 0.15s;
}

.round-pill:hover {
  border-color: var(--rune-blue-mid);
  color: var(--rune-blue-light);
}

.round-pill.active {
  background: var(--rune-blue);
  border-color: var(--rune-blue-mid);
  color: #fff;
  font-weight: 600;
}
</style>
