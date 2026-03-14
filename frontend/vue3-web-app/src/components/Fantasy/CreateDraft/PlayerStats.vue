<template>
  <div class="detail-panel">

    <!-- Header -->
    <div class="detail-header">
      <font-awesome-icon :icon="['far', 'user']" class="header-icon" />
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
          <font-awesome-icon :icon="['fas', 'circle-plus']" class="draft-btn-icon" />
          <span>Draft Player · {{ playerCost }} Gold</span>
        </button>
        <button class="random-btn" @click="randomPlayer()">
          <font-awesome-icon :icon="['fas', 'dice']" />
          <span>Random Pick</span>
        </button>
      </div>

    </div>

    <!-- Empty state -->
    <div v-else class="detail-empty">
      <font-awesome-icon :icon="['fas', 'circle-question']" class="empty-icon" />
      <span>Select a player to view details</span>
    </div>

  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import type { FantasyPlayerTopHeroes } from '../fantasyDraft';
import { fantasyDraftState } from '../fantasyDraft';
import PlayerStatsBio from '@/components/Fantasy/PlayerStats/PlayerStatsBio.vue'
import FantasyBarChart from '@/components/Fantasy/PlayerStats/FantasyBarChart.vue'
import PlayerTopHeroes from '@/components/Fantasy/PlayerStats/PlayerTopHeroes.vue'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
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
  const playerStats = leagueStore.fantasyPlayersStats.find(fps => fps.fantasy_player.id === newPlayer.id);
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
  background: var(--ot-bg-deep);
  border: 1px solid color-mix(in srgb, var(--rune-purple-dark) 20%, transparent);
  border-radius: 8px;
  overflow: hidden;
}

.detail-header {
  display: flex;
  align-items: center;
  gap: 10px;
  height: 56px;
  padding: 0 16px;
  background: var(--ot-bg-deep);
  border-bottom: 1px solid color-mix(in srgb, var(--sg-border) 14%, transparent);
  flex-shrink: 0;
}

.detail-header-label {
  font-family: var(--font-heading);
  font-size: var(--text-sm);
  font-weight: 800;
  letter-spacing: 2px;
  color: color-mix(in srgb, var(--sg-border) 85%, transparent);
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
  border-bottom: 1px solid color-mix(in srgb, var(--rune-purple-dark) 13%, transparent);
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
  background: var(--sg-bg-deep);
  border: 1px solid color-mix(in srgb, var(--rune-purple-dark) 20%, transparent);
}

.stat-primary {
  border-color: color-mix(in srgb, var(--rune-purple-dark) 50%, transparent);
}

.stat-value {
  font-family: var(--font-heading);
  font-size: var(--text-md);
  font-weight: 700;
  color: var(--rune-purple-light);
}

.stat-value.muted {
  color: color-mix(in srgb, var(--rune-purple-light) 60%, transparent);
  font-size: var(--text-base);
}

.stat-label {
  font-family: var(--font-body);
  font-size: var(--text-xs);
  font-weight: 600;
  letter-spacing: 1px;
  color: color-mix(in srgb, var(--sg-border) 40%, transparent);
}

.section-divider {
  height: 1px;
  background: color-mix(in srgb, var(--rune-purple-dark) 10%, transparent);
}

.section-label {
  font-family: var(--font-body);
  font-size: var(--text-xs);
  font-weight: 700;
  letter-spacing: 1px;
  color: color-mix(in srgb, var(--sg-border) 50%, transparent);
}

/* Breakdown */
.breakdown-section {
  display: flex;
  flex-direction: column;
  gap: 10px;
  padding: 16px;
  background: var(--sg-bg-deep);
  border-bottom: 1px solid color-mix(in srgb, var(--rune-purple-dark) 13%, transparent);
}

.header-icon {
  font-size: 16px;
  color: color-mix(in srgb, var(--sg-border) 70%, transparent);
}

.draft-btn-icon {
  font-size: 18px;
  color: var(--rune-purple-text);
}

.empty-icon {
  font-size: 48px;
  color: color-mix(in srgb, var(--ot-border) 20%, transparent);
}

/* Draft Button Area */
.draft-btn-area {
  display: flex;
  flex-direction: column;
  gap: 8px;
  padding: 16px;
  background: color-mix(in srgb, var(--rune-purple-deep) 8%, var(--ot-bg-deep));
}

.draft-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  width: 100%;
  height: 52px;
  border-radius: 8px;
  border: 2px solid var(--rune-purple-dark);
  background: linear-gradient(135deg, var(--rune-purple-dark), var(--rune-purple-deep));
  box-shadow: 0 0 24px color-mix(in srgb, var(--rune-purple-dark) 50%, transparent);
  cursor: pointer;
  font-family: var(--font-body);
  font-size: var(--text-base);
  font-weight: 800;
  color: var(--rune-purple-text);
  transition: all 0.2s;
}

.draft-btn:hover:not(:disabled) {
  background: linear-gradient(135deg,
    color-mix(in srgb, var(--rune-purple-dark) 100%, white 10%),
    color-mix(in srgb, var(--rune-purple-deep) 100%, white 10%));
  box-shadow: 0 0 36px color-mix(in srgb, var(--rune-purple-dark) 70%, transparent);
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
  border: 1px solid color-mix(in srgb, var(--rune-purple-dark) 40%, transparent);
  background: transparent;
  cursor: pointer;
  font-family: var(--font-body);
  font-size: var(--text-sm);
  font-weight: 600;
  color: color-mix(in srgb, var(--rune-purple-light) 70%, transparent);
  transition: all 0.2s;
}

.random-btn:hover {
  border-color: color-mix(in srgb, var(--rune-purple-dark) 70%, transparent);
  color: var(--rune-purple-light);
}

/* Empty state */
.detail-empty {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 12px;
  padding: 48px 16px;
  color: color-mix(in srgb, var(--ot-border) 30%, transparent);
  font-family: var(--font-body);
  font-size: var(--text-sm);
}
</style>
