<template>
  <div class="draft-card">
    <v-card v-if="props.size == 'small'" class="card-container">
      <div class="card-title-small">
        <v-card-title class="ma-0 pa-0" :style="{ 'font-size': isDesktop ? '0.9rem' : '0.7rem' }">
          {{ props.fantasyPlayer?.dotaAccount?.name || '' }}
        </v-card-title>
      </div>
      <div class="card-images">
        <img class="player-image" :style="{ 'max-width': isDesktop ? '70px' : '60px' }" :src="getPlayerLogo()" />
        <img class="team-image" :src="getTeamLogo()" />
      </div>
      <div class="draft-body-small">
        <v-card-text class="pt-1 pl-2" style="min-height: 3rem;">
          <div v-if="props.fantasyLeagueActive">
            <span :style="{ 'font-size': '0.8rem', 'font-weight': 'bold' }">
              {{ props.fantasyPoints?.toFixed(2) ?? 0 }}
            </span>
            <br />
            <span :style="{ 'font-size': '0.8rem' }">
              Fantasy Pts
            </span>
          </div>
        </v-card-text>
      </div>
    </v-card>
    <v-card v-else class="card-container">
      <div class="card-title">
        <v-card-title class="ma-0 pl-1 pa-0" :style="{ 'font-size': isDesktop ? '1.2rem' : '1rem' }">
          {{ props.fantasyPlayer?.dotaAccount?.name || '' }}
        </v-card-title>
      </div>
      <div class="card-images">
        <img class="player-image" :src="getPlayerLogo()" />
        <img class="team-image" :src="getTeamLogo()" />
      </div>
      <div class="draft-body">
        <v-card-subtitle class="pt-1" :style="{ 'font-size': isDesktop ? '1rem' : '0.8rem' }">
          <div class="team-title">
            <span style="min-height: 20px">
              {{ props.fantasyPlayer?.team?.name || '' }}
            </span>
            <img :src=getPositionIcon(props.fantasyPlayer?.teamPosition!) height="20px" width="20px" />
          </div>
        </v-card-subtitle>
        <v-card-text class="pt-1" style="min-height: 3rem;">
          <div v-if="props.fantasyPoints != undefined">
            <span :style="{ 'font-size': isDesktop ? '1.0rem' : '1rem', 'font-weight': 'bold' }">
              {{ props.fantasyPoints.toFixed(2) }}
            </span>
            <span :style="{ 'font-size': isDesktop ? '0.8rem' : '0.8rem' }">
              Fantasy Points
            </span>
          </div>
        </v-card-text>
      </div>
    </v-card>
  </div>
</template>

<script setup lang="ts">
import { ref, type PropType } from 'vue';
import { VCard, VCardTitle, VCardSubtitle, VCardText } from 'vuetify/components';
import type { FantasyPlayer } from './fantasyDraft';

const props = defineProps({
  size: {
    type: String,
    required: false
  },
  fantasyPlayer: {
    type: Object as PropType<FantasyPlayer>,
    required: false
  },
  fantasyPoints: {
    type: Number,
    required: false
  },
  fantasyLeagueActive: {
    type: Boolean,
    required: false
  }
})

const isDesktop = ref(window.outerWidth >= 600);

const getPositionIcon = (positionInt: number) => {
  if (positionInt == 0) return `logos/unknown.png`;
  return `icons/pos_${positionInt}.png`
}

const getPlayerLogo = () => {
  if (!props.fantasyPlayer?.dotaAccount.steamProfilePicture) return undefined;
  return props.fantasyPlayer?.dotaAccount.steamProfilePicture;
}

const getTeamLogo = () => {
  if (!props.fantasyPlayer?.teamId) return undefined;
  return `logos/teams_logo_${props.fantasyPlayer?.teamId}.png`
}

</script>


<style scoped>
.draft-card {
  aspect-ratio: 0.7;
}

.card-container {
  border-radius: 8px;
  background-color: var(--aghanims-fantasy-blue-1);
  border: 6px solid var(--aghanims-fantasy-main-2);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  overflow: hidden;
  font-family: 'Merriweather', serif;
  max-height: 100%;
}

.card-container q-img {
  border-top-left-radius: 8px;
  border-top-right-radius: 8px;
}

.card-title {
  background-color: var(--aghanims-fantasy-main-2);
}

.card-title-small {
  background-color: var(--aghanims-fantasy-main-2);
  font-family: 'Roboto', sans-serif;
}

.card-images {
  display: flex;
  justify-content: center;
}


.draft-body {
  background: linear-gradient(to bottom, var(--aghanims-fantasy-main-2), var(--aghanims-fantasy-blue-1));
  border-top: 3px solid var(--aghanims-fantasy-main-2);
  height: 50%;
  text-align: start;
}

.draft-body-small {
  background: linear-gradient(to bottom, var(--aghanims-fantasy-main-2), var(--aghanims-fantasy-blue-1));
  border-top: 3px solid var(--aghanims-fantasy-main-2);
  height: 50%;
  text-align: start;
  font-family: 'Roboto', sans-serif;
}

.draft-body-main {
  font-weight: bold;
}

.draft-body-details {
  margin-top: 8px;
}

.player-image {
  max-width: 100%;
  max-height: 100%;
}

.team-image {
  position: absolute;
  top: 40px;
  right: 2px;
  max-width: 30%;
  max-height: 30%;
}

.team-title {
  display: flex;
  align-items: center;
}

.team-name {
  color: rgba(var(--v-theme-on-surface), var(--v-high-emphasis-opacity))
}
</style>