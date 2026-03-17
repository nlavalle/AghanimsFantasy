<template>
  <div class="page-header">
    <div class="header-left">
      <font-awesome-icon :icon="['fas', 'trophy']" class="header-icon" />
      <h1 class="page-title">Leaderboard</h1>
    </div>
    <div class="header-right" v-if="authStore.isAuthenticated">
      <span class="shard-label">Shards</span>
      <span class="shard-balance">{{ fantasyDraftStore.fantasyLeaderboardStats?.totalDrafts ?? 0 }}</span>
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
.page-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.header-left {
  display: flex;
  align-items: center;
  gap: var(--space-sm);
}

.header-icon {
  font-size: var(--text-xl);
  color: var(--rune-blue-light);
}

.page-title {
  font-family: var(--font-heading);
  font-size: var(--text-xl);
  font-weight: 700;
  color: var(--rune-gold);
  margin: 0;
}

.header-right {
  display: flex;
  align-items: center;
  gap: var(--space-xs);
}

.shard-label {
  font-size: var(--text-sm);
  color: var(--rune-gray);
  font-family: var(--font-body);
}

.shard-balance {
  font-size: var(--text-md);
  font-weight: 600;
  color: var(--rune-blue-light);
  font-family: var(--font-stats);
}
</style>
