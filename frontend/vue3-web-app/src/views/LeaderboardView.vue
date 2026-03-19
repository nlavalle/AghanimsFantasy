<template>
  <div class="leaderboard-page">
    <LeaderboardPageHeader />
    <LeaderboardControlBar
      :rounds="rounds"
      :show-all-rounds="showAllRounds"
      :active-round-index="activeRoundIndex"
      @select-all-rounds="selectAllRounds"
      @select-round="selectRound"
    />
    <LeaderboardTable :drafters="sortedDrafters" :show-all-rounds="showAllRounds" :is-loading="isLoading" :tooltip-enabled="tooltipEnabled" />
    <LeaderboardStatsBar />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import { useFantasyDraftStore } from '@/stores/fantasyDraft';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';
import { isDraftOpen } from '@/stores/fantasyLeagueUtils';
import type { Leaderboard } from '@/types/Leaderboard';
import LeaderboardPageHeader from '@/components/Leaderboard/LeaderboardPageHeader.vue';
import LeaderboardControlBar from '@/components/Leaderboard/LeaderboardControlBar.vue';
import LeaderboardTable from '@/components/Leaderboard/LeaderboardTable.vue';
import LeaderboardStatsBar from '@/components/Leaderboard/LeaderboardStatsBar.vue';

const fantasyDraftStore = useFantasyDraftStore();
const leagueStore = useFantasyLeagueStore();

const showAllRounds = ref(false)
const isLoading = ref(false)

const rounds = computed(() =>
  [...leagueStore.fantasyLeagues]
    .filter(fl => fl.leagueId === leagueStore.selectedLeague?.league_id)
    .sort((a, b) => a.leagueStartTime - b.leagueStartTime)
)

const activeRoundIndex = computed(() => {
  if (fantasyDraftStore.selectedRound !== null) return fantasyDraftStore.selectedRound
  const idx = rounds.value.findIndex(fl => fl.id === leagueStore.currentFantasyLeague.id)
  return idx !== -1 ? idx : rounds.value.length - 1
})

const activeRound = computed(() => {
  const fl = rounds.value[activeRoundIndex.value]
  if (!fl) return null
  return fantasyDraftStore.leagueLeaderboard?.rounds.find(r => r.fantasyLeagueId === fl.id) ?? null
})

const roundDrafters = computed(() => {
  if (!activeRound.value) return []
  return [...activeRound.value.fantasyDrafts].sort((a, b) => b.draftTotalFantasyPoints - a.draftTotalFantasyPoints)
})

const allRoundsDrafters = computed<Leaderboard[]>(() => {
  const allRounds = fantasyDraftStore.leagueLeaderboard?.rounds ?? []
  const totals = new Map<string, Leaderboard>()
  for (const round of allRounds) {
    for (const d of round.fantasyDrafts) {
      if (totals.has(d.userName)) {
        const existing = totals.get(d.userName)!
        totals.set(d.userName, { ...existing, draftTotalFantasyPoints: existing.draftTotalFantasyPoints + d.draftTotalFantasyPoints })
      } else {
        totals.set(d.userName, { ...d })
      }
    }
  }
  return [...totals.values()].sort((a, b) => b.draftTotalFantasyPoints - a.draftTotalFantasyPoints)
})

const sortedDrafters = computed(() => showAllRounds.value ? allRoundsDrafters.value : roundDrafters.value)

// Tooltip is only shown when the relevant draft window(s) are locked.
// For a single round: the active round's FL must be locked.
// For All Rounds: every round must be locked (any open round would leak picks).
const tooltipEnabled = computed(() => {
  if (showAllRounds.value) {
    return rounds.value.every(fl => !isDraftOpen(fl))
  }
  const activeFL = rounds.value[activeRoundIndex.value]
  return activeFL ? !isDraftOpen(activeFL) : false
})

function selectAllRounds() {
  showAllRounds.value = true
  fantasyDraftStore.selectedRound = null
}

function selectRound(idx: number) {
  showAllRounds.value = false
  fantasyDraftStore.selectedRound = idx
}

watch(() => leagueStore.selectedLeague, () => {
  showAllRounds.value = false
  isLoading.value = true
  fantasyDraftStore.fetchLeagueLeaderboard()?.finally(() => { isLoading.value = false })
}, { immediate: true })
</script>

<style scoped>
.leaderboard-page {
  max-width: 720px;
  margin: 0 auto;
  padding: var(--space-lg) var(--space-md);
  display: flex;
  flex-direction: column;
  gap: var(--space-lg);
}
</style>
