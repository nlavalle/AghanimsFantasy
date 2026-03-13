<template>
  <div class="detail-panel">

    <!-- Header -->
    <div class="detail-header">
      <v-icon size="16" color="rgba(232, 224, 200, 0.7)" icon="fa-regular fa-user" />
      <span class="detail-header-label">SELECTED PLAYER</span>
    </div>

    <div v-if="selectedPlayer" class="detail-scroll">

      <!-- Player Card -->
      <PlayerStatsBio />

      <!-- Stat boxes -->
      <div class="stats-section">
        <div class="stat-row">
          <div class="stat-box stat-primary">
            <span class="stat-value">{{ totalPoints.toFixed(1) }}</span>
            <span class="stat-label">TOTAL PTS</span>
          </div>
          <div class="stat-box">
            <span class="stat-value muted">{{ kda }}</span>
            <span class="stat-label">KDA</span>
          </div>
          <div class="stat-box">
            <span class="stat-value muted">{{ totalMatches }}</span>
            <span class="stat-label">MATCHES</span>
          </div>
        </div>

        <div class="section-divider" />

        <!-- Most Played Heroes -->
        <template v-if="playerTopHeroes">
          <PlayerTopHeroes :heroesPlayer="playerTopHeroes" />
        </template>
      </div>

      <!-- Fantasy PT bars -->
      <div class="breakdown-section">
        <span class="section-label">FANTASY PT BREAKDOWN</span>
        <FantasyBarChart v-for="(item, i) in fantasyBars" :key="i" :label="item.label" :value="item.value"
          :max="item.max" />
      </div>

      <!-- Draft Button -->
      <div class="draft-btn-area">
        <button class="draft-btn" :disabled="disabledPlayer(selectedPlayer)"
          :class="{ 'draft-btn-disabled': disabledPlayer(selectedPlayer) }" @click="draftPlayer()">
          <v-icon size="18" color="#E9D5FF" icon="fa-circle-plus" />
          <span>Draft Player · {{ playerCost }} Gold</span>
        </button>
        <button class="random-btn" @click="randomPlayer()">
          <font-awesome-icon :icon="faDice" />
          <span>Random Pick</span>
        </button>
      </div>

    </div>

    <!-- Empty state -->
    <div v-else class="detail-empty">
      <v-icon size="48" color="rgba(138, 138, 158, 0.2)" icon="fa-circle-question" />
      <span>Select a player to view details</span>
    </div>

  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { VIcon } from 'vuetify/components';
import type { FantasyPlayerTopHeroes } from '../fantasyDraft';
import { fantasyDraftState } from '../fantasyDraft';
import PlayerStatsBio from '@/components/Fantasy/PlayerStats/PlayerStatsBio.vue'
import FantasyBarChart from '@/components/Fantasy/PlayerStats/FantasyBarChart.vue'
import PlayerTopHeroes from '@/components/Fantasy/PlayerStats/PlayerTopHeroes.vue'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import { faDice } from '@fortawesome/free-solid-svg-icons';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';

const { selectedPlayer, fantasyPlayerPointsAvailable, setFantasyPlayer, disabledPlayer } = fantasyDraftState();
const leagueStore = useFantasyLeagueStore();

const emit = defineEmits(['savePlayer']);

interface BarItem { label: string; value: number; max: number }
const EMPTY_BARS: BarItem[] = [
  { label: 'K/D/A', value: 0, max: 1 },
  { label: 'Farming', value: 0, max: 1 },
  { label: 'Support', value: 0, max: 1 },
  { label: 'Damage', value: 0, max: 1 },
];
const fantasyBars = ref<BarItem[]>(EMPTY_BARS.map(b => ({ ...b })));
const playerTopHeroes = ref<FantasyPlayerTopHeroes>();
const playerCost = ref<string | number>(0);
const totalPoints = ref(0);
const totalMatches = ref(0);
const kda = ref('—');

watch(selectedPlayer, (newPlayer) => {
  if (!newPlayer) return;
  const playerStats = leagueStore.fantasyPlayersStats.find(fps => fps.fantasy_player.id == newPlayer.id);
  playerTopHeroes.value = playerStats?.top_heroes[0];
  playerCost.value = playerStats?.cost.toFixed(0) ?? 0;

  const pp = fantasyPlayerPointsAvailable.value.find(p => p.fantasyPlayer.id === newPlayer.id);
  totalPoints.value = pp?.totalMatchFantasyPoints ?? 0;
  totalMatches.value = pp?.totalMatches ?? 0;

  const ps = playerStats?.player_stats as any;
  if (ps) {
    const k = (ps.avgKillsPoints ?? 0).toFixed(1);
    const d = (ps.avgDeathsPoints ?? 0).toFixed(1);
    const a = (ps.avgAssistsPoints ?? 0).toFixed(1);
    kda.value = `${k}/${d}/${a}`;
    formatPlayerAverages(ps);
  } else {
    kda.value = '—';
    resetBars();
  }
});

const resetBars = () => {
  fantasyBars.value = EMPTY_BARS.map(b => ({ ...b }));
};

const formatPlayerAverages = (pa: any) => {
  const kdaTotal = ((pa.avgKillsPoints ?? 0) * 100 + (pa.avgDeathsPoints ?? 0) * 100 + (pa.avgAssistsPoints ?? 0) * 100) / 3;
  const farmTotal = ((pa.avgLastHitsPoints ?? 0) * 100 + (pa.avgGoldPerMinPoints ?? 0) * 100 + (pa.avgXpPerMinPoints ?? 0) * 100) / 3;
  const supportTotal = ((pa.avgObserverWardsPlacedPoints ?? 0) * 100 + (pa.avgCampsStackedPoints ?? 0) * 100) / 2;
  const damageTotal = (pa.avgStunDurationPoints ?? 0) * 100;
  const vals = [kdaTotal, farmTotal, supportTotal, damageTotal];
  const maxVal = Math.max(...vals, 1);
  fantasyBars.value = EMPTY_BARS.map((b, i) => ({ ...b, value: vals[i], max: maxVal }));
};

const draftPlayer = () => {
  if (selectedPlayer.value) {
    setFantasyPlayer(selectedPlayer.value);
    emit('savePlayer');
  }
};

const randomPlayer = () => {
  const available = fantasyPlayerPointsAvailable.value.filter(pa => !disabledPlayer(pa.fantasyPlayer));
  const pick = available[Math.floor(Math.random() * available.length)];
  if (pick) selectedPlayer.value = pick.fantasyPlayer;
};
</script>

<style scoped>
.detail-panel {
  display: flex;
  flex-direction: column;
  background: #0D0D16;
  border: 1px solid rgba(123, 47, 190, 0.2);
  border-radius: 8px;
  overflow: hidden;
}

.detail-header {
  display: flex;
  align-items: center;
  gap: 10px;
  height: 56px;
  padding: 0 16px;
  background: #0D0D16;
  border-bottom: 1px solid rgba(232, 224, 200, 0.14);
  flex-shrink: 0;
}

.detail-header-label {
  font-family: var(--font-heading);
  font-size: var(--text-sm);
  font-weight: 800;
  letter-spacing: 2px;
  color: rgba(232, 224, 200, 0.85);
}

.detail-scroll {
  display: flex;
  flex-direction: column;
  overflow-y: auto;
}

/* Stats */
.stats-section {
  display: flex;
  flex-direction: column;
  gap: 12px;
  padding: 16px;
  border-bottom: 1px solid rgba(123, 47, 190, 0.13);
}

.stat-row {
  display: flex;
  gap: 8px;
}

.stat-box {
  display: flex;
  flex-direction: column;
  gap: 4px;
  flex: 1;
  padding: 8px 10px;
  border-radius: 6px;
  background: #0A0810;
  border: 1px solid rgba(123, 47, 190, 0.2);
}

.stat-primary {
  border-color: rgba(123, 47, 190, 0.5);
}

.stat-value {
  font-family: var(--font-heading);
  font-size: var(--text-md);
  font-weight: 700;
  color: #C084FC;
}

.stat-value.muted {
  color: rgba(192, 132, 252, 0.6);
  font-size: var(--text-base);
}

.stat-label {
  font-family: var(--font-body);
  font-size: var(--text-xs);
  font-weight: 600;
  letter-spacing: 1px;
  color: rgba(232, 224, 200, 0.4);
}

.section-divider {
  height: 1px;
  background: rgba(123, 47, 190, 0.1);
}

.section-label {
  font-family: var(--font-body);
  font-size: var(--text-xs);
  font-weight: 700;
  letter-spacing: 1px;
  color: rgba(232, 224, 200, 0.5);
}

/* Breakdown */
.breakdown-section {
  display: flex;
  flex-direction: column;
  gap: 10px;
  padding: 16px;
  background: #0A0810;
  border-bottom: 1px solid rgba(123, 47, 190, 0.13);
}

.radar-chart {
  background: linear-gradient(to bottom, black, #0d1b2a);
  padding: 4px;
}

/* Draft Button Area */
.draft-btn-area {
  display: flex;
  flex-direction: column;
  gap: 8px;
  padding: 16px;
  background: #0D0A18;
}

.draft-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  width: 100%;
  height: 52px;
  border-radius: 8px;
  border: 2px solid #7B2FBE;
  background: linear-gradient(135deg, #7B2FBE, #5B21B6);
  box-shadow: 0 0 24px rgba(123, 47, 190, 0.5);
  cursor: pointer;
  font-family: var(--font-body);
  font-size: var(--text-base);
  font-weight: 800;
  color: #E9D5FF;
  transition: all 0.2s;
}

.draft-btn:hover:not(:disabled) {
  background: linear-gradient(135deg, #8B3FCE, #6B31C6);
  box-shadow: 0 0 36px rgba(123, 47, 190, 0.7);
}

.draft-btn-disabled {
  opacity: 0.4;
  cursor: not-allowed;
  box-shadow: none;
}

.random-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  width: 100%;
  height: 40px;
  border-radius: 8px;
  border: 1px solid rgba(123, 47, 190, 0.4);
  background: transparent;
  cursor: pointer;
  font-family: var(--font-body);
  font-size: var(--text-sm);
  font-weight: 600;
  color: rgba(192, 132, 252, 0.7);
  transition: all 0.2s;
}

.random-btn:hover {
  border-color: rgba(123, 47, 190, 0.7);
  color: #C084FC;
}

/* Empty state */
.detail-empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 12px;
  padding: 48px 16px;
  color: rgba(138, 138, 158, 0.3);
  font-family: var(--font-body);
  font-size: var(--text-sm);
}
</style>
