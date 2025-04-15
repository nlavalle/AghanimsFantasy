<template>
    <div class="total-winnings">
        <v-row v-if="fantasyLeagueStore.isDraftOpen(fantasyLeagueStore.selectedFantasyLeague)"
            class="d-flex justify-left align-center pr-2">
            <v-col class="v-col-9">
                <h2>Games haven't started</h2>
            </v-col>
        </v-row>
        <v-row v-else class="d-flex justify-left align-center pr-2">
            <v-col class="v-col-9">
                <h1>Draft Winnings</h1>
            </v-col>
            <v-col class="v-col-3">
                <GoldSpan :animated="true" :bold="true" :font-size="1.5" :gold-value="totalWinnings().toFixed(0)" />
            </v-col>
        </v-row>
    </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { VRow, VCol } from 'vuetify/components';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';
import type { FantasyPlayer, FantasyPlayerPoints } from '../fantasyDraft';
import GoldSpan from '@/components/Dom/GoldSpan.vue';

const fantasyLeagueStore = useFantasyLeagueStore();

const playerFantasyStatsIndexed = computed(() => {
    return fantasyLeagueStore.fantasyPlayerPoints
        .map((player: FantasyPlayerPoints, index) => ({
            ...player,
            position: index + 1
        }))
        .filter(pfsi => fantasyLeagueStore.selectedFantasyDraftPoints?.fantasyDraft.draftPickPlayers.some(dpp => dpp.fantasyPlayerId == pfsi.fantasyPlayerId))
})

const totalWinnings = () => {
    return Math.max(
        playerFantasyStatsIndexed.value.reduce(function (total, current) {
            return total + getWinnings(current.position, current.fantasyPlayer);
        }, 0),
        0 // Don't let winnings fall below 0
    );
}

const getWinnings = (position: number, fantasyPlayer: FantasyPlayer) => {
    let quintileSize = fantasyLeagueStore.fantasyPlayerPoints.length / 5;
    let quintile = Math.ceil(position / quintileSize);

    let cost = fantasyLeagueStore.fantasyPlayersStats.find(fps => fps.fantasy_player.id == fantasyPlayer.id)?.cost ?? 0;
    let winnings = 300 - 60 * quintile
    return winnings - cost
}

</script>

<style scoped>
/*-------------------- Player Winnings --------------------*/
.total-winnings {
    background-color: var(--aghanims-fantasy-main-4);
    border: 5px solid var(--aghanims-fantasy-main-2);
    border-radius: 15px;
    box-shadow: 0 7px 30px rgba(62, 9, 11, 0.3);
    margin-bottom: 10px;
    margin-left: 5px;
    height: max-content;
    flex: 1 0 300px;
}

.total-winnings h1 {
    font-size: 24px;
    color: var(--aghanims-fantasy-white);
    padding: 0px 13px 0px;
    line-height: 3rem;
}

.total-winnings h2 {
    font-size: 16px;
    color: var(--aghanims-fantasy-white);
    padding: 0px 10px;
    line-height: 3rem;
}
</style>
