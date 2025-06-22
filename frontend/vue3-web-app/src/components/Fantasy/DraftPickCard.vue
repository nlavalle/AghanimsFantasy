<template>
  <v-card v-if="props.size == 'small'"
    :class="{ 'card-container': true, 'draft-card': !display.mobile.value, 'draft-card-small': display.mobile.value }">
    <v-card-title class="ma-0 pa-0 card-title-small"
      :style="{ 'font-size': !display.mobile.value ? '0.9rem' : '0.7rem' }">
      {{ props.fantasyPlayer?.dotaAccount?.name || '' }}
    </v-card-title>
    <div class="card-images-small">
      <img class="player-image" :style="{ 'max-width': !display.mobile.value ? '70px' : '60px' }"
        :src="getPlayerLogo()" />
      <img class="team-image" :src="getTeamLogo()" />
    </div>
    <div class="draft-body-small">
      <v-card-text class="pt-1 pl-1 pr-1" style="min-height: 3rem;">
        <div v-if="props.fantasyLeagueActive">
          <span :style="{ 'font-weight': 'bold', 'font-size': '0.8rem' }">
            {{ props.fantasyPoints?.toFixed(!display.mobile.value ? 2 : 0) ?? 0 }}
          </span>
          <br v-if="!display.mobile.value" />
          <span :style="{ 'font-size': !display.mobile.value ? '0.8rem' : '0.6rem' }">
            {{ !display.mobile.value ? 'Fantasy Pts' : 'Pts' }}
          </span>
        </div>
        <div v-else>
          <GoldSpan :font-size="0.8" :validation="(fantasyPlayerBudget ?? 0) - (fantasyPlayerCost ?? 0) < 0"
            :gold-value="props.fantasyPlayerCost?.toFixed(0) ?? '0'" />
        </div>
      </v-card-text>
    </div>
  </v-card>
  <v-card v-else class="card-container draft-card">
    <v-card-title class="ma-0 pl-1 pa-0 card-title" :style="{ 'font-size': !display.mobile.value ? '1.2rem' : '1rem' }">
      <span class="card-title-text">{{ props.fantasyPlayer?.dotaAccount?.name || '' }}</span>
    </v-card-title>
    <div class="card-images">
      <img class="player-image" :src="getPlayerLogo()" />
      <img class="team-image" :src="getTeamLogo()" />
      <div v-if="props.killStreakEffect" class="kill-streak-fire">
        <div class="kill-streak-particle" v-for="n in 50" :key="n" />
      </div>
    </div>
    <div class="draft-body">
      <v-card-subtitle class="pt-1" :style="{ 'font-size': !display.mobile.value ? '1rem' : '0.8rem' }">
        <div class="team-title">
          <span style="min-height: 20px">
            {{ props.fantasyPlayer?.team?.name || '' }}
          </span>
          <img :src=getPositionIcon(props.fantasyPlayer?.teamPosition!) height="20px" width="20px" />
        </div>
      </v-card-subtitle>
      <v-card-text class="pt-1" style="min-height: 3rem;">
        <div v-if="props.fantasyPoints != undefined">
          <span :style="{ 'font-size': !display.mobile.value ? '1.0rem' : '1rem', 'font-weight': 'bold' }">
            {{ props.fantasyPoints.toFixed(2) }}
          </span>
          <span :style="{ 'font-size': !display.mobile.value ? '0.8rem' : '0.8rem' }">
            Fantasy Points
          </span>
        </div>
      </v-card-text>
    </div>
  </v-card>
</template>

<script setup lang="ts">
import { type PropType } from 'vue';
import { VCard, VCardTitle, VCardSubtitle, VCardText } from 'vuetify/components';
import type { FantasyPlayer } from './fantasyDraft';
import GoldSpan from '../Dom/GoldSpan.vue';
import { useDisplay } from 'vuetify';

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
  },
  fantasyPlayerCost: {
    type: Number,
    required: false
  },
  fantasyPlayerBudget: {
    type: Number,
    required: false
  },
  killStreakEffect: {
    type: Boolean,
    required: false,
    default: false
  }
})

const display = useDisplay()

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

.draft-card-small {
  aspect-ratio: 0.5;
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

.card-title-text {
  position: relative;
  z-index: 5;
}

.card-title-small {
  background-color: var(--aghanims-fantasy-main-2);
  font-family: 'Roboto', sans-serif;
}

.card-images {
  display: flex;
  justify-content: center;
  height: 68.75%;
}

.card-images-small {
  display: flex;
  justify-content: center;
}

.draft-body {
  background: linear-gradient(to bottom, var(--aghanims-fantasy-main-2), var(--aghanims-fantasy-blue-1));
  border-top: 3px solid var(--aghanims-fantasy-main-2);
  height: 50%;
  text-align: start;
  position: relative;
  z-index: 5;
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
  margin-top: auto;
  max-width: 90%;
  max-height: 90%;
  z-index: 5;
  border-radius: 10px
}

.team-image {
  position: absolute;
  top: 40px;
  right: 2px;
  max-width: 30%;
  max-height: 30%;
  z-index: 5;
}

.team-title {
  display: flex;
  align-items: center;
}

.team-name {
  color: rgba(var(--v-theme-on-surface), var(--v-high-emphasis-opacity))
}
</style>


<style lang="scss" scoped>
@use "sass:math";
$fireColor: rgb(255, 80, 0);
$fireColorT: rgba(255, 80, 0, 0);
$dur: 1s;
$blur: 0.02em;
$fireRad: 3em;
$fireHeight: 12em;
$parts: 50;
$partSize: 5em;

body {
  background-color: rgb(48, 8, 8);
  margin: 0;
}

.kill-streak-fire {
  font-size: 24px;
  filter: blur($blur);
  -webkit-filter: blur($blur);
  // margin: 1em auto 0 auto;
  position: absolute;
  top: 0px;
  width: 11em;
  height: $fireHeight;
  z-index: 3;
}

.kill-streak-particle {
  animation: rise $dur ease-in infinite;
  background-image: radial-gradient($fireColor 20%, $fireColorT 70%);
  border-radius: 50%;
  mix-blend-mode: screen;
  opacity: 0;
  position: absolute;
  bottom: 0;
  width: $partSize;
  height: $partSize;

  // spread particles out in time and x-position to get desired effect
  @for $p from 1 through $parts {
    &:nth-of-type(#{$p}) {
      animation-delay: $dur * math.random();
      left: calc((100% - #{$partSize}) * #{calc(($p - 1)/$parts)});
    }
  }
}

@keyframes rise {
  from {
    opacity: 0;
    transform: translateY(0) scale(1);
  }

  25% {
    opacity: 1;
  }

  to {
    opacity: 0;
    transform: translateY($fireHeight * -1) scale(0);
  }
}
</style>