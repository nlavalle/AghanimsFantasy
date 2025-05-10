<template>
    <v-row class="player-top-heroes">
        <v-col :class="!display.mobile.value ? 'tophero-text-desktop' : 'tophero-text-mobile'">
            <v-row>
                <span>Most played heroes (last 30 games):</span>
            </v-row>
            <v-row>
                <v-col v-for="(hero, index) in props.heroesPlayer.topHeroes" :key="index">
                    <v-row justify="center">
                        <img :style="{ width: !display.mobile.value ? '128px' : '72px', height: !display.mobile.value ? '64px' : '36px' }"
                            :src="getHeroIcon(hero.hero.name)" />
                    </v-row>
                    <v-row justify="center">
                        <span>{{ hero.count }} (<span style="color: green">{{ hero.wins }}W</span> - <span
                                style="color: red">{{
                                    hero.losses }}L</span>)</span>
                    </v-row>
                </v-col>
            </v-row>
        </v-col>
    </v-row>
</template>

<script setup lang="ts">
import { ref, type PropType } from 'vue';
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
.tophero-text-desktop {
    font-size: 1rem;
}

.tophero-text-mobile {
    font-size: 0.8rem;
}
</style>