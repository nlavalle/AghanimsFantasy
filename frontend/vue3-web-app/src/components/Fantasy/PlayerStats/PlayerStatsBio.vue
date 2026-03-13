<template>
  <div class="player-card" v-if="selectedPlayer">
    <div class="player-card-top">
      <div class="player-avatar-zone">
        <div class="player-avatar-frame">
          <div class="player-avatar-inner">
            <img :src="selectedPlayer.dotaAccount.steamProfilePicture" :alt="selectedPlayer.dotaAccount.name"
              class="player-avatar-img" />
          </div>
        </div>
      </div>
      <div class="player-meta">
        <span class="player-name">{{ selectedPlayer.dotaAccount.name }}</span>
        <div class="player-role-row">
          <img :src="getPositionIcon(selectedPlayer.teamPosition)" class="role-icon" />
          <span class="role-label">Position {{ selectedPlayer.teamPosition }}</span>
        </div>
        <span class="player-team">{{ selectedPlayer.team.name }}</span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { fantasyDraftState } from '../fantasyDraft';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';

const { selectedPlayer } = fantasyDraftState();
const leagueStore = useFantasyLeagueStore();

const playerCost = ref<string | number>(0);

const getPositionIcon = (positionInt: number) => {
  if (positionInt == 0) return undefined;
  return `icons/pos_${positionInt}.png`
}

watch(selectedPlayer, (newPlayer) => {
  if (!newPlayer) return;
  const playerStats = leagueStore.fantasyPlayersStats.find(fps => fps.fantasy_player.id == newPlayer.id);
  playerCost.value = playerStats?.cost.toFixed(0) ?? 0;
});

</script>

<style scoped>
/* Player Card */
.player-card {
  background: linear-gradient(160deg, #0A0A0C, #0E0E10 50%, #0A0A0C);
  border-bottom: 1px solid rgba(232, 224, 200, 0.08);
}

.player-card-top {
  display: flex;
  height: 140px;
}

.player-avatar-zone {
  width: 140px;
  flex-shrink: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  background: radial-gradient(circle, rgba(232, 224, 200, 0.09) 0%, transparent 70%);
}

.player-avatar-frame {
  width: 130px;
  height: 130px;
  border-radius: 12px;
  background: linear-gradient(180deg, #E8E0C8, #F0EBE0);
  padding: 2px;
  flex-shrink: 0;
}

.player-avatar-inner {
  width: 100%;
  height: 100%;
  border-radius: 10px;
  background: #0A0A0E;
  overflow: hidden;
}

.player-avatar-img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  object-position: top center;
}

.player-meta {
  display: flex;
  flex-direction: column;
  justify-content: center;
  gap: 6px;
  flex: 1;
  padding: 12px 16px;
}

.player-role-row {
  display: flex;
  align-items: center;
  gap: 6px;
}

.role-icon {
  width: 16px;
  height: 16px;
  opacity: 0.7;
}

.role-label {
  font-family: var(--font-body);
  font-size: var(--text-xs);
  font-weight: 600;
  letter-spacing: 1px;
  color: rgba(157, 78, 221, 0.7);
}

.player-name {
  font-family: var(--font-heading);
  font-size: var(--text-lg);
  font-weight: 800;
  color: var(--aghanims-fantasy-white);
}

.player-team {
  font-family: var(--font-body);
  font-size: var(--text-xs);
  color: rgba(232, 224, 200, 0.4);
}
</style>