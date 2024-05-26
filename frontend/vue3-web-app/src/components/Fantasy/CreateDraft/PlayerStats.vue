<template>
    <v-container class="player-stats pl-1">
        <v-row v-if="selectedPlayer" class="ml-0">
            <v-col>
                <PlayerStatsBio :selected-player="selectedPlayer">
                </PlayerStatsBio>
                <v-row class="mt-4">
                    <v-btn class="mr-1" style="width:55%;" color="primary" @click="draftPlayer()">Draft Player</v-btn>
                    <v-btn class="ml-1" style="width:40%;" color="primary" @click="randomPlayer()">
                        <font-awesome-icon :icon="faDice" />
                        <span class="ml-1">Random</span>
                    </v-btn>
                </v-row>
                <PlayerTopHeroes class="player-top-heroes mt-5 pa-1" :selected-player="selectedPlayer">
                </PlayerTopHeroes>
                <v-row class="mt-4" style="border:1px solid black">
                    <v-col>
                        <PlayerRadarChart class="player-radar-chart pa-1" title="Fantasy Point Breakdown"
                            :chart-labels="fantasyLabels" :chart-dataset="fantasyDataset" />
                    </v-col>
                    <v-col>
                        <PlayerRadarChart class="player-radar-chart pa-1" title="Dota Scores"
                            :chart-labels="scoreLabels" :chart-dataset="scoreDataset" />
                    </v-col>
                </v-row>
            </v-col>
        </v-row>
    </v-container>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { VContainer, VRow, VCol, VBtn } from 'vuetify/components';
import type { FantasyPlayer, FantasyPlayerTopHeroes } from '../fantasyDraft';
import { localApiService } from '@/services/localApiService'
import { fantasyDraftState } from '../fantasyDraft';
import PlayerStatsBio from '@/components/Fantasy/PlayerStats/PlayerStatsBio.vue'
import PlayerTopHeroes from '@/components/Fantasy/PlayerStats/PlayerTopHeroes.vue'
import PlayerRadarChart from '@/components/Fantasy/PlayerStats/PlayerRadarChart.vue'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import { faDice } from '@fortawesome/free-solid-svg-icons';



const { fantasyPlayersAvailable, fantasyDraftPicks, setFantasyPlayer } = fantasyDraftState();
const selectedPlayer = defineModel<FantasyPlayer>();


const fantasyLabels = [
    'K/D/A',
    'Farming',
    'Supporting',
    'Damage/Healing',
    'Games Played'
];

const fantasyDataset = ref([0, 0, 0, 0, 0]);

const scoreLabels = [
    'Fighting',
    'Farming',
    'Supporting',
    'Pushing'
];

const scoreDataset = ref([0, 0, 0, 0]);

const playerTopHeroes = ref<FantasyPlayerTopHeroes>();

watch(selectedPlayer, (newPlayer) => {
    if (newPlayer) {
        localApiService.getPlayerTopHeroes(newPlayer.id)
            .then((result) => (playerTopHeroes.value = result));

        localApiService.getPlayerFantasyAverages(newPlayer.id)
            .then((result) => (formatPlayerAverages(result)))
    }
});


const formatPlayerAverages = (playerAverages: any) => {
    var firstPlayerAverages = playerAverages[0];
    //KDA
    var killsAverage = firstPlayerAverages.avgKillsPoints;
    var deathsAverage = firstPlayerAverages.avgDeathsPoints;
    var assistsAverage = firstPlayerAverages.avgAssistsPoints;
    var kdaTotal = (killsAverage * 100 + deathsAverage * 100 + assistsAverage * 100) / 3
    //Farm
    //last hits
    var lastHitsAverage = firstPlayerAverages.avgLastHitsPoints;
    //gold per min
    var goldPerMinAverage = firstPlayerAverages.avgGoldPerMinPoints;
    //xp per min
    var xpPerMinAverage = firstPlayerAverages.avgXpPerMinPoints;
    var farmTotal = (lastHitsAverage * 100 + goldPerMinAverage * 100 + xpPerMinAverage * 100) / 3
    //Supp
    //ob wards placed
    var obsWardsAverage = firstPlayerAverages.avgObserverWardsPlacedPoints;
    //camps stacked
    var campsStacked = firstPlayerAverages.avgCampsStackedPoints;
    var supportTotal = (obsWardsAverage * 100 + campsStacked * 100) / 2
    //damage healing
    //stun damage
    var damageHealingTotal = firstPlayerAverages.avgStunDurationPoints * 100;

    var matchesPlayed = firstPlayerAverages.totalMatches * 100;

    fantasyDataset.value = [
        kdaTotal,
        farmTotal,
        supportTotal,
        damageHealingTotal,
        matchesPlayed
    ]

    //Scores
    var fightScore = firstPlayerAverages.avgFightScore * 100;
    var farmScore = firstPlayerAverages.avgFarmScore * 100;
    var supportScore = firstPlayerAverages.avgSupportScore * 100;
    var pushScore = firstPlayerAverages.avgPushScore * 100;

    scoreDataset.value = [fightScore, farmScore, supportScore, pushScore]

}

const draftPlayer = () => {
    if (selectedPlayer.value) {
        setFantasyPlayer(selectedPlayer.value);
    }
}

const randomPlayer = () => {
    var availablePicks = fantasyPlayersAvailable.value.filter(pa => !fantasyDraftPicks.value.includes(pa))
    var randomPick = availablePicks[(Math.floor(Math.random() * availablePicks.length))];
    selectedPlayer.value = randomPick;
}




</script>

<style scoped>
.player-stats {
    background-color: rgba(27, 38, 59, 0.4);
    height: 100%;
}

.player-top-heroes {
    background: linear-gradient(to bottom, var(--aghanims-fantasy-main-3), var(--aghanims-fantasy-main-4));
    border: 1px solid black;
}

.player-radar-chart {
    background: linear-gradient(to bottom, black, var(--aghanims-fantasy-main-4));
}
</style>