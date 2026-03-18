<template>
  <div>
    <div v-if="isLoading" class="d-flex justify-center align-center" style="min-height: 200px;">
      <v-progress-circular color="primary" indeterminate />
    </div>
    <div v-else class="leaderboard-table-wrap">
      <table class="leaderboard-table" v-if="drafters.length">
        <thead>
          <tr>
            <th class="col-rank">#</th>
            <th class="col-player">Player</th>
            <th v-if="!showAllRounds" class="col-pts">Round Pts</th>
            <th class="col-pts">Total Pts</th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="(drafter, idx) in drafters"
            :key="showAllRounds ? drafter.userName : drafter.fantasyDraft.id"
            :class="{ 'current-user': drafter.userName === authStore.currentUser?.discordName }"
            class="drafter-row"
            @mouseenter="(e) => { if (props.tooltipEnabled) onRowEnter(e, idx) }"
            @mouseleave="hoveredIdx = null"
          >
            <td class="col-rank">
              <span class="rank-badge" :class="rankClass(idx + 1)">{{ idx + 1 }}</span>
            </td>
            <td class="col-player">{{ drafter.userName }}</td>
            <td v-if="!showAllRounds" class="col-pts">{{ drafter.draftTotalFantasyPoints.toFixed(1) }}</td>
            <td class="col-pts">{{ allRoundsTotal(drafter).toFixed(1) }}</td>
          </tr>
        </tbody>
      </table>
      <div class="empty-state" v-else>
        <font-awesome-icon :icon="['fas', 'trophy']" class="empty-icon" />
        <p>No leaderboard data yet</p>
      </div>
    </div>

    <!-- Teleported tooltip — escapes overflow:hidden table wrapper -->
    <Teleport to="body">
      <div
        v-if="hoveredIdx !== null && tooltipPos"
        class="pick-tooltip-fixed"
        :style="{ top: tooltipPos.y + 'px', left: tooltipPos.x + 'px' }"
      >
        <template v-if="!showAllRounds">
          <div
            class="pick-tooltip-row"
            v-for="pick in [...drafters[hoveredIdx].fantasyDraft.draftPickPlayers].sort((a, b) => a.draftOrder - b.draftOrder)"
            :key="pick.draftOrder"
          >
            <span class="pick-label">{{ pick.fantasyPlayer?.dotaAccount?.name ?? `Pick ${pick.draftOrder}` }}</span>
            <span class="pick-pts">{{ pickPoints(drafters[hoveredIdx], pick.draftOrder).toFixed(1) }}</span>
          </div>
        </template>
        <template v-else>
          <div
            class="pick-tooltip-row"
            v-for="(pts, rIdx) in allRoundBreakdown(drafters[hoveredIdx].userName)"
            :key="rIdx"
          >
            <span class="pick-label">Round {{ rIdx + 1 }}</span>
            <span class="pick-pts">{{ pts.toFixed(1) }}</span>
          </div>
        </template>
      </div>
    </Teleport>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useFantasyDraftStore } from '@/stores/fantasyDraft';
import type { Leaderboard } from '@/types/Leaderboard';
import { VProgressCircular } from 'vuetify/components';

const props = defineProps<{
  drafters: Leaderboard[]
  showAllRounds: boolean
  isLoading: boolean
  tooltipEnabled: boolean
}>()

const authStore = useAuthStore();
const fantasyDraftStore = useFantasyDraftStore();

const hoveredIdx = ref<number | null>(null)
const tooltipPos = ref<{ x: number; y: number } | null>(null)

function onRowEnter(e: MouseEvent, idx: number) {
  hoveredIdx.value = idx
  const rect = (e.currentTarget as HTMLElement).getBoundingClientRect()
  tooltipPos.value = {
    x: rect.right + 8,
    y: rect.top + rect.height / 2,
  }
}

function rankClass(rank: number) {
  if (rank === 1) return 'gold'
  if (rank === 2) return 'silver'
  if (rank === 3) return 'bronze'
  return 'default'
}

function pickPoints(drafter: Leaderboard, n: number): number {
  const map: Record<number, number> = {
    1: drafter.draftPickOnePoints,
    2: drafter.draftPickTwoPoints,
    3: drafter.draftPickThreePoints,
    4: drafter.draftPickFourPoints,
    5: drafter.draftPickFivePoints,
  }
  return map[n] ?? 0
}

function allRoundBreakdown(userName: string): number[] {
  return (fantasyDraftStore.leagueLeaderboard?.rounds ?? []).map(round => {
    const d = round.fantasyDrafts.find(d => d.userName === userName)
    return d?.draftTotalFantasyPoints ?? 0
  })
}

function allRoundsTotal(drafter: Leaderboard): number {
  if (props.showAllRounds) return drafter.draftTotalFantasyPoints
  return (fantasyDraftStore.leagueLeaderboard?.rounds ?? []).reduce((sum, round) => {
    const d = round.fantasyDrafts.find(d => d.userName === drafter.userName)
    return sum + (d?.draftTotalFantasyPoints ?? 0)
  }, 0)
}
</script>

<style scoped>
.leaderboard-table-wrap {
  background: var(--aghanims-fantasy-main-4);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 12px;
  overflow: hidden;
}

.leaderboard-table {
  width: 100%;
  border-collapse: collapse;
}

.leaderboard-table thead tr {
  background: var(--rune-blue-deep);
  border-bottom: 2px solid var(--rune-blue-dark);
}

.leaderboard-table th {
  padding: var(--space-sm) var(--space-md);
  font-family: var(--font-body);
  font-size: var(--text-xs);
  font-weight: 600;
  color: var(--rune-blue-muted);
  text-transform: uppercase;
  letter-spacing: 0.05em;
  text-align: left;
}

.leaderboard-table td {
  padding: var(--space-sm) var(--space-md);
  font-family: var(--font-body);
  font-size: var(--text-base);
  color: var(--ot-text);
  border-top: 1px solid rgba(255, 255, 255, 0.05);
}

.drafter-row:hover {
  background: rgba(255, 255, 255, 0.03);
}

.leaderboard-table tr.current-user td {
  background: rgba(21, 101, 192, 0.15);
}

.col-rank {
  width: 48px;
  text-align: center;
}

.col-pts {
  text-align: right !important;
  font-family: var(--font-stats);
  font-weight: 600;
}

.col-player {
  position: relative;
}

.rank-badge {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 28px;
  height: 28px;
  border-radius: 50%;
  font-family: var(--font-stats);
  font-size: var(--text-sm);
  font-weight: 700;
  border: 2px solid transparent;
}

.rank-badge.gold {
  background: rgba(255, 184, 0, 0.15);
  border-color: #FFB800;
  color: #FFB800;
}

.rank-badge.silver {
  background: rgba(138, 138, 158, 0.15);
  border-color: #C0C0C0;
  color: #C0C0C0;
}

.rank-badge.bronze {
  background: rgba(176, 141, 87, 0.15);
  border-color: #CD7F32;
  color: #CD7F32;
}

.rank-badge.default {
  background: transparent;
  border-color: rgba(255, 255, 255, 0.12);
  color: var(--rune-gray);
}

.pick-tooltip-fixed {
  position: fixed;
  transform: translateY(-50%);
  z-index: 1000;
  background: var(--aghanims-fantasy-main-2, #0d1117);
  border: 1px solid rgba(255, 255, 255, 0.12);
  border-radius: 8px;
  padding: var(--space-sm) var(--space-md);
  min-width: 160px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.5);
  pointer-events: none;
}

.pick-tooltip-row {
  display: flex;
  justify-content: space-between;
  gap: var(--space-md);
  padding: 2px 0;
}

.pick-label {
  font-size: var(--text-xs);
  color: var(--rune-blue-muted);
  font-family: var(--font-body);
}

.pick-pts {
  font-size: var(--text-xs);
  font-family: var(--font-stats);
  font-weight: 600;
  color: var(--rune-gold-light);
}

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: var(--space-sm);
  padding: var(--space-3xl) var(--space-md);
  color: var(--rune-gray);
}

.empty-icon {
  font-size: var(--text-2xl);
  opacity: 0.3;
}
</style>
