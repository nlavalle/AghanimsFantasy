<template>
  <div>
    <div class="new-pick-bar">
      <DraftPickSlot v-for="index in 5" :key="index" :index="index" :fantasy-player="fantasyDraftPicks[index]"
        :cost="fantasyDraftPicks[index] ? Number(getPlayerCost(fantasyDraftPicks[index].dotaAccount.id)) : undefined"
        :points="getPlayerPoints(index)" :is-selected="currentActiveDraftPlayerCheck(index)"
        @select="changeActiveDraftPlayer(index)" />

      <div class="bar-info-cards">
        <DraftRoundCard :current="3" :total="5" />
        <DraftTimerCard v-if="leagueStore.selectedFantasyLeague"
          :target-time="leagueStore.selectedFantasyLeague.fantasyDraftLocked" />
      </div>
    </div>
    <!-- <v-row class="gallery" style="max-width:800px">
      <v-col v-for="index in 5" :key="index" class="gallery-item">
        <v-row v-if="fantasyDraftPicks[index]" @click="changeActiveDraftPlayer(index)">
          <v-col>
            <v-row v-if="display.mobile.value">
              <GoldSpan :gold-value="getPlayerCost(fantasyDraftPicks[index].dotaAccount.id).toString()" />
            </v-row>
            <v-row class="parallelogram" justify="center"
              :style="{ width: !display.mobile.value ? '120px' : '60px', height: !display.mobile.value ? '80px' : '40px', 'margin-left': !display.mobile.value ? '5px' : '' }"
              :class="{ 'glow-active-slot': currentActiveDraftPlayerCheck(index) }">
              <GoldSpan v-if="!display.mobile.value" class="gold"
                :gold-value="getPlayerCost(fantasyDraftPicks[index].dotaAccount.id).toString()" />
              <img :src="fantasyDraftPicks[index].dotaAccount.steamProfilePicture"
                :alt="fantasyDraftPicks[index].dotaAccount.name" />
            </v-row>
            <v-row class="caption" justify="center"
              :style="{ 'font-size': !display.mobile.value ? '1rem' : '0.8rem', 'max-width': !display.mobile.value ? '140px' : '60px' }">{{
                fantasyDraftPicks[index].dotaAccount.name }}</v-row>
          </v-col>
        </v-row>
        <v-row v-else @click="changeActiveDraftPlayer(index)">
          <v-col>
            <v-row v-if="display.mobile.value">
              <GoldSpan :gold-value="'0'" />
            </v-row>
            <v-row class="parallelogram" justify="center" align="center"
              :style="{ 'max-width': !display.mobile.value ? '120px' : '60px', 'min-width': !display.mobile.value ? '120px' : '60px', 'min-height': !display.mobile.value ? '80px' : '40px', 'margin-left': !display.mobile.value ? '5px' : '' }"
              :class="{ 'glow-active-slot': currentActiveDraftPlayerCheck(index) }">
              <img
                :style="{ width: !display.mobile.value ? '70px' : '20px', height: !display.mobile.value ? '70px' : '20px' }"
                :src="`icons/pos_${index}.png`" />
            </v-row>
            <v-row class="caption" justify="center"
              :style="{ 'min-height': !display.mobile.value ? '1.5rem' : '1.2rem', 'max-width': !display.mobile.value ? '120px' : '60px', 'padding-left': !display.mobile.value ? '30px' : '10px' }"></v-row>
          </v-col>
        </v-row>
      </v-col>
    </v-row> -->
  </div>
</template>

<script setup lang="ts">
import { fantasyDraftState } from '../fantasyDraft';
import { VRow, VCol } from 'vuetify/components';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';
import GoldSpan from '@/components/Dom/GoldSpan.vue';
import { useDisplay } from 'vuetify';
import DraftPickSlot from '@/components/Fantasy/CreateDraft/DraftPickSlot.vue';
import DraftRoundCard from '@/components/Fantasy/CreateDraft/DraftRoundCard.vue';
import DraftTimerCard from '@/components/Fantasy/CreateDraft/DraftTimerCard.vue';

const display = useDisplay()

const { selectedPlayer, currentDraftSlotSelected, fantasyDraftPicks, fantasyPlayerPointsAvailable } = fantasyDraftState();
const leagueStore = useFantasyLeagueStore();

const changeActiveDraftPlayer = (activeDraftPlayerSlot: number) => {
  currentDraftSlotSelected.value = activeDraftPlayerSlot;
  selectedPlayer.value = fantasyDraftPicks.value[activeDraftPlayerSlot];
}

const currentActiveDraftPlayerCheck = (draftSlot: number) => {
  return draftSlot == currentDraftSlotSelected.value;
}

const getPlayerCost = (accountId: number) => {
  return leagueStore.fantasyPlayersStats.find(fps => fps.fantasy_player.dotaAccountId == accountId)?.cost.toFixed(0) ?? 0;
}

const getPlayerPoints = (slotIndex: number) => {
  const player = fantasyDraftPicks.value[slotIndex]
  if (!player) return undefined
  return fantasyPlayerPointsAvailable.value.find(pp => pp.fantasyPlayer.id === player.id)?.totalMatchFantasyPoints
}

</script>

<style scoped>
.gallery {
  display: flex;
  justify-content: space-evenly;
  align-items: center;
  margin-left: 10px;
}

.gallery-item {
  text-align: center;
}

.parallelogram {
  position: relative;
  overflow: hidden;
  transform: skew(20deg);
  border: 1px solid white;
  background: radial-gradient(at center, #474747 10%, #323232 90%);
}

.parallelogram img {
  height: 100%;
  object-fit: cover;
  transform: skew(-20deg);
}

.glow-active-slot {
  border: 1px solid #ffd700;
  box-shadow: 0 0 5px #ffd700, 0 0 10px #ffd700, 0 0 15px #ffd700, 0 0 20px #ffd700;
  border-radius: 10px;
}

.caption {
  font-size: 1em;
  color: #AAA;
  text-overflow: ellipsis;
  white-space: nowrap;
  overflow: hidden;
}

.new-pick-bar {
  position: relative;
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 12px 32px;
  background: linear-gradient(180deg, #22222E 0%, #171720 60%, #111118 100%);
  border-bottom: 1px solid transparent;
  border-image: linear-gradient(90deg, transparent, rgba(232, 224, 200, 0.13), transparent) 1;
  box-shadow: 0 6px 24px rgba(0, 0, 0, 0.8), 0 12px 40px rgba(0, 0, 0, 0.5);
  overflow: hidden;
}

.bar-info-cards {
  margin-left: auto;
  display: flex;
  gap: 8px;
  align-items: center;
  flex-shrink: 0;
}

.gold {
  position: absolute;
  top: 5px;
  left: 10px;
  transform: skew(-20deg);
  overflow: hidden;
  z-index: 2;
}
</style>