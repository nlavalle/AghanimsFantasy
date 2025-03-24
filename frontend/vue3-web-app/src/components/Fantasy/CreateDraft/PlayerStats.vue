<template>
    <v-container class="player-stats pl-1">
        <v-row v-if="selectedPlayer" class="ml-0">
            <v-col>
                <PlayerStatsBio>
                </PlayerStatsBio>
                <v-row class="mt-4">
                    <v-btn class="mr-1" style="height:40px;width:55%;" color="primary" @click="draftPlayer()"
                        :disabled="disabledPlayer(selectedPlayer)">
                        <span class="ml-1">Draft Player <br> ({{ playerCost }}<img class="cost-coin"
                                :src="coinSpinGif" />)</span>
                    </v-btn>
                    <v-btn class="ml-1" style="height:40px;width:40%;" color="primary" @click="randomPlayer()">
                        <font-awesome-icon :icon="faDice" />
                        <span class="ml-1">Random</span>
                    </v-btn>
                </v-row>
                <PlayerTopHeroes v-if="playerTopHeroes" class="player-top-heroes mt-5 pa-1"
                    :heroesPlayer="playerTopHeroes">
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
import coinSpinGif from '@/assets/fantasy/coin/golden-coin.gif'
import { ref, watch } from 'vue';
import { VContainer, VRow, VCol, VBtn } from 'vuetify/components';
import type { FantasyPlayerTopHeroes } from '../fantasyDraft';
import { fantasyDraftState } from '../fantasyDraft';
import PlayerStatsBio from '@/components/Fantasy/PlayerStats/PlayerStatsBio.vue'
import PlayerTopHeroes from '@/components/Fantasy/PlayerStats/PlayerTopHeroes.vue'
import PlayerRadarChart from '@/components/Fantasy/PlayerStats/PlayerRadarChart.vue'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import { faDice } from '@fortawesome/free-solid-svg-icons';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';

const { selectedPlayer, fantasyPlayerPointsAvailable, setFantasyPlayer, disabledPlayer } = fantasyDraftState();

const leagueStore = useFantasyLeagueStore();

const emit = defineEmits(['savePlayer']);

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

const playerCost = ref();

watch(selectedPlayer, (newPlayer) => {
    if (newPlayer) {
        let playerStats = leagueStore.fantasyPlayersStats.find(fps => fps.fantasy_player.id == newPlayer.id);
        playerTopHeroes.value = playerStats?.top_heroes[0];

        playerCost.value = playerStats?.cost.toFixed(0) ?? 0;

        formatPlayerAverages(playerStats?.player_stats)
    }
});


const formatPlayerAverages = (playerAverages: any) => {
    if (playerAverages) {
        //KDA
        var killsAverage = playerAverages.avgKillsPoints;
        var deathsAverage = playerAverages.avgDeathsPoints;
        var assistsAverage = playerAverages.avgAssistsPoints;
        var kdaTotal = (killsAverage * 100 + deathsAverage * 100 + assistsAverage * 100) / 3
        //Farm
        //last hits
        var lastHitsAverage = playerAverages.avgLastHitsPoints;
        //gold per min
        var goldPerMinAverage = playerAverages.avgGoldPerMinPoints;
        //xp per min
        var xpPerMinAverage = playerAverages.avgXpPerMinPoints;
        var farmTotal = (lastHitsAverage * 100 + goldPerMinAverage * 100 + xpPerMinAverage * 100) / 3
        //Supp
        //ob wards placed
        var obsWardsAverage = playerAverages.avgObserverWardsPlacedPoints;
        //camps stacked
        var campsStacked = playerAverages.avgCampsStackedPoints;
        var supportTotal = (obsWardsAverage * 100 + campsStacked * 100) / 2
        //damage healing
        //stun damage
        var damageHealingTotal = playerAverages.avgStunDurationPoints * 100;

        var matchesPlayed = playerAverages.totalMatches * 100;

        fantasyDataset.value = [
            kdaTotal,
            farmTotal,
            supportTotal,
            damageHealingTotal,
            matchesPlayed
        ]

        //Scores
        var fightScore = playerAverages.avgFightScore * 100;
        var farmScore = playerAverages.avgFarmScore * 100;
        var supportScore = playerAverages.avgSupportScore * 100;
        var pushScore = playerAverages.avgPushScore * 100;

        scoreDataset.value = [fightScore, farmScore, supportScore, pushScore]
    } else {
        // Player has no games yet
        fantasyDataset.value = [0, 0, 0, 0, 0]

        scoreDataset.value = [0, 0, 0, 0]
    }

}

const draftPlayer = () => {
    if (selectedPlayer.value) {
        setFantasyPlayer(selectedPlayer.value);
        emit('savePlayer');
    }
}

const randomPlayer = () => {
    var availablePicks = fantasyPlayerPointsAvailable.value.filter(pa => !disabledPlayer(pa.fantasyPlayer))
    var randomPick = availablePicks[(Math.floor(Math.random() * availablePicks.length))];
    selectedPlayer.value = randomPick.fantasyPlayer;
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

.cost-coin {
    display: inline-block;
    vertical-align: middle;
}
</style>