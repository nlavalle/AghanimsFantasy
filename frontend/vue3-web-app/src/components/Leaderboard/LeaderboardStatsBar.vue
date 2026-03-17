<template>
  <div
    class="stats-bar"
    v-if="authStore.isAuthenticated && fantasyDraftStore.fantasyLeaderboardStats?.totalDrafts"
  >
    <div class="stat-item">
      <span class="stat-label">Total Drafts</span>
      <span class="stat-value">{{ fantasyDraftStore.fantasyLeaderboardStats.totalDrafts }}</span>
    </div>
    <div class="stat-item" v-if="fantasyDraftStore.fantasyLeaderboardStats.drafterPercentile != null">
      <span class="stat-label">Your Percentile</span>
      <span class="stat-value">{{ fantasyDraftStore.fantasyLeaderboardStats.drafterPercentile.toFixed(0) }}th</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/auth';
import { useFantasyDraftStore } from '@/stores/fantasyDraft';

const authStore = useAuthStore();
const fantasyDraftStore = useFantasyDraftStore();
</script>

<style scoped>
.stats-bar {
  display: flex;
  gap: var(--space-xl);
  padding: var(--space-md) var(--space-lg);
  background: var(--aghanims-fantasy-main-4);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 12px;
}

.stat-item {
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.stat-label {
  font-family: var(--font-body);
  font-size: var(--text-xs);
  color: var(--rune-gray);
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.stat-value {
  font-family: var(--font-stats);
  font-size: var(--text-lg);
  font-weight: 700;
  color: var(--rune-blue-light);
}
</style>
