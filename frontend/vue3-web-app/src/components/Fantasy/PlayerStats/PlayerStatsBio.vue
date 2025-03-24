<template>
    <v-row class="player-stats-bio" v-if="selectedPlayer">
        <v-col class="v-col-auto">
            <v-row>
                <img style="height:250px;" :src="selectedPlayer!.dotaAccount.steamProfilePicture"
                    :style="{ height: isDesktop ? '180px' : '140px' }" />
            </v-row>
        </v-col>
        <v-col style="width:100%" :class="isDesktop ? 'bio-text-desktop' : 'bio-text-mobile'">
            <v-row class="d-flex flex-column" style="width:100%;height:100%" justify="space-evenly">
                <span>Name: <b>{{ selectedPlayer!.dotaAccount.name }}</b></span>
                <span>Team: <b>{{ selectedPlayer!.team.name }}</b></span>
                <span>Role: <img :src=getPositionIcon(selectedPlayer!.teamPosition) height="20px" width="20px" />
                    ({{ selectedPlayer!.teamPosition }})
                </span>
                <span>Cost: {{leagueStore.fantasyPlayersStats.find(fps => fps.fantasy_player.id ==
                    selectedPlayer!.id)?.cost.toFixed(0) ?? 0}} Gold</span>
            </v-row>
        </v-col>
    </v-row>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { VRow, VCol } from 'vuetify/components';
import { fantasyDraftState } from '../fantasyDraft';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';

const isDesktop = ref(window.outerWidth >= 600);

const { selectedPlayer } = fantasyDraftState();
const leagueStore = useFantasyLeagueStore();

const getPositionIcon = (positionInt: number) => {
    if (positionInt == 0) return undefined;
    return `icons/pos_${positionInt}.png`
}
</script>

<style scoped>
.bio-text-desktop {
    font-size: 1rem;
}

.bio-text-mobile {
    font-size: 0.8rem;
}
</style>