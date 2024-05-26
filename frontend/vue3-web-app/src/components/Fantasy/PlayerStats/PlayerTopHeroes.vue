<template>
    <v-row class="player-top-heroes">
        <v-col :class="isDesktop ? 'tophero-text-desktop' : 'tophero-text-mobile'">
            <v-row>
                <span>Most played heroes:</span>
            </v-row>
            <v-row>
                <v-col v-for="(hero, index) in playerTopHeroes?.topHeroes" :key="index">
                    <v-row justify="center">
                        <img :style="{ width: isDesktop ? '128px' : '72px', height: isDesktop ? '64px' : '36px' }"
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
import { onMounted, ref, watch, type PropType } from 'vue';
import { VRow, VCol } from 'vuetify/components';
import { localApiService } from '@/services/localApiService';
import type { FantasyPlayer, FantasyPlayerTopHeroes } from '../fantasyDraft';

const props = defineProps({
    selectedPlayer: {
        type: Object as PropType<FantasyPlayer>,
        required: true,
    }
})

const playerTopHeroes = ref<FantasyPlayerTopHeroes>();

const isDesktop = ref(window.outerWidth >= 600);

const getHeroIcon = (heroIconString: string) => {
    if (heroIconString == '') return undefined;
    var formattedString = heroIconString.replace('npc_dota_hero_', '');
    return `https://cdn.cloudflare.steamstatic.com/apps/dota2/images/dota_react/heroes/${formattedString}.png`
}

onMounted(() => {
    if (props.selectedPlayer) {
        localApiService.getPlayerTopHeroes(props.selectedPlayer.id)
            .then((result) => (playerTopHeroes.value = result));
    }
})

watch(() => props.selectedPlayer, (newPlayer) => {
    if (newPlayer) {
        localApiService.getPlayerTopHeroes(newPlayer.id)
            .then((result) => (playerTopHeroes.value = result));
    }
})

</script>

<style scoped>
.tophero-text-desktop {
    font-size: 1rem;
}

.tophero-text-mobile {
    font-size: 0.8rem;
}
</style>