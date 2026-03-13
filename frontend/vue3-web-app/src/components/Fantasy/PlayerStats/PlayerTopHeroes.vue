<template>
  <v-row class="player-top-heroes">
    <v-col :class="!display.mobile.value ? 'tophero-text-desktop' : 'tophero-text-mobile'">
      <v-row>
        <span class="section-label">MOST PLAYED HEROES</span>
      </v-row>
      <v-row>
        <v-col v-for="(hero, index) in props.heroesPlayer.topHeroes" :key="index">
          <v-row justify="center">
            <img :style="{ width: !display.mobile.value ? '90px' : '72px' }" :src="getHeroIcon(hero.hero.name)" />
          </v-row>
          <v-row justify="center">
            <span>{{ hero.count }} (<span class="text-success">{{ hero.wins }}W</span> - <span class="text-error">{{
              hero.losses }}L</span>)</span>
          </v-row>
        </v-col>
      </v-row>
    </v-col>
  </v-row>
</template>

<script setup lang="ts">
import { type PropType } from 'vue';
import { VRow, VCol } from 'vuetify/components';
import type { FantasyPlayerTopHeroes } from '../fantasyDraft';
import { useDisplay } from 'vuetify';

const props = defineProps({
  heroesPlayer: {
    type: Object as PropType<FantasyPlayerTopHeroes>,
    required: true,
  }
})

const display = useDisplay()

const getHeroIcon = (heroIconString: string) => {
  if (heroIconString == '') return undefined;
  var formattedString = heroIconString.replace('npc_dota_hero_', '');
  return `https://cdn.cloudflare.steamstatic.com/apps/dota2/images/dota_react/heroes/${formattedString}.png`
}

</script>

<style scoped>
.player-top-heroes {
  margin: 0;
}

.tophero-text-desktop {
  font-size: var(--text-sm);
}

.tophero-text-mobile {
  font-size: var(--text-xs);
}

.section-label {
  font-family: var(--font-body);
  font-size: var(--text-xs);
  font-weight: 700;
  letter-spacing: 1px;
  color: rgba(232, 224, 200, 0.5);
}

img {
  border-radius: 6px;
  border: 1px solid rgba(123, 47, 190, 0.4);
  box-shadow: 0 0 6px rgba(123, 47, 190, 0.2);
  object-fit: cover;
  aspect-ratio: 16 / 9;
}

span {
  font-family: var(--font-body);
  color: rgba(232, 224, 200, 0.5);
}
</style>