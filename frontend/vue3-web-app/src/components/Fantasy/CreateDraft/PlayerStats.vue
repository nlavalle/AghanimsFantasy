<template>
    <v-container class="player-stats">
        <v-row v-if="selectedPlayer">
            <v-col>
                <v-row>
                    <v-col>
                        <v-row>
                            <img style="height:250px;" :src="selectedPlayer.dotaAccount.steamProfilePicture" />
                        </v-row>
                    </v-col>
                    <v-col>
                        <v-row>
                            <v-btn color="primary" @click="draftPlayer()">Draft Player</v-btn>
                        </v-row>
                        <v-row>
                            <span>Name: {{ selectedPlayer.dotaAccount.name }}</span>
                        </v-row>
                        <v-row>
                            <span>Team: {{ selectedPlayer.team.name }}</span>
                        </v-row>
                        <v-row>
                            <span>Role: {{ selectedPlayer.teamPosition }}</span>
                        </v-row>
                    </v-col>
                </v-row>
                <v-row>
                    <span>Top Heroes played</span>
                </v-row>
                <v-row v-if="playerTopHeroes">
                    <v-col v-for="(hero, index) in playerTopHeroes.topHeroes" :key="index">
                        <img style="width:64px;height:36px" :src="getHeroIcon(hero.hero.name)" />
                        <p>{{ hero.count }}</p>
                    </v-col>
                </v-row>
                <v-row>
                    <v-col>
                        <v-row>
                            <p>Fantasy Point Breakdown</p>
                            <div>
                                <Radar style="height:200px;width:200px" :data="fantasyChartData" :options="options">
                                </Radar>
                            </div>
                        </v-row>
                    </v-col>
                    <v-col>
                        <v-row>
                            <p>Dota Scores</p>
                            <div>
                                <Radar style="height:200px;width:200px" :data="scoreChartData" :options="options">
                                </Radar>
                            </div>
                        </v-row>
                    </v-col>
                </v-row>
                <v-row>
                    <p>Match Fantasy Point Avg</p>
                </v-row>
            </v-col>
        </v-row>
    </v-container>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { VContainer, VRow, VCol, VBtn } from 'vuetify/components';
import {
    Chart as ChartJS,
    RadialLinearScale,
    PointElement,
    LineElement,
    Filler,
    Tooltip,
    Legend
} from 'chart.js'
import { Radar } from 'vue-chartjs'
import type { FantasyPlayer, FantasyPlayerTopHeroes } from '../fantasyDraft';
import { localApiService } from '@/services/localApiService'
import { fantasyDraftState } from '../fantasyDraft';

ChartJS.register(
    RadialLinearScale,
    PointElement,
    LineElement,
    Filler,
    Tooltip,
    Legend
)

const { setFantasyPlayer } = fantasyDraftState();
const selectedPlayer = defineModel<FantasyPlayer>();

const options = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
        legend: {
            display: false
        }
    },
    scales: {
        r: {
            backgroundColor: 'rgba(255, 99, 132, 0.2)',
            pointLabels: {
                color: '#fff'
            },
            angleLines: {
                color: 'rgba(255, 99, 132, 0.2)' // Custom color for angle lines
            },
            grid: {
                color: 'rgba(255, 99, 132, 0.2)' // Custom color for grid lines
            },
            ticks: {
                display: false
            }
        }
    }
}

const fantasyChartData = ref({
    labels: [
        'K/D/A',
        'Farming',
        'Supporting',
        'Damage/Healing',
        'Games Played'
    ],
    datasets: [
        {
            data: [0, 0, 0, 0, 0],
            fill: true,
            backgroundColor: 'rgba(255, 99, 132, 0.2)',
            borderColor: 'rgb(255, 99, 132)',
            pointBackgroundColor: 'rgb(255, 99, 132)',
            pointBorderColor: '#fff',
            pointHoverBackgroundColor: '#fff',
            pointHoverBorderColor: 'rgb(255, 99, 132)'
        },
    ]
})

const scoreChartData = ref({
    labels: [
        'Fighting',
        'Farming',
        'Supporting',
        'Pushing'
    ],
    datasets: [
        {
            data: [0, 0, 0, 0],
            fill: true,
            backgroundColor: 'rgba(255, 99, 132, 0.2)',
            borderColor: 'rgb(255, 99, 132)',
            pointBackgroundColor: 'rgb(255, 99, 132)',
            pointBorderColor: '#fff',
            pointHoverBackgroundColor: '#fff',
            pointHoverBorderColor: 'rgb(255, 99, 132)'
        },
    ]
})

const playerTopHeroes = ref<FantasyPlayerTopHeroes>();
// const playerAverages = ref(null);

watch(selectedPlayer, (newPlayer) => {
    if (newPlayer) {
        localApiService.getPlayerTopHeroes(newPlayer.id)
            .then((result) => (playerTopHeroes.value = result));

        localApiService.getPlayerFantasyAverages(newPlayer.id)
            .then((result) => (formatPlayerAverages(result)))
    }
});

const draftPlayer = () => {
    if (selectedPlayer.value) {
        setFantasyPlayer(selectedPlayer.value);
    }
}

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

    fantasyChartData.value = {
        labels: [
            'K/D/A',
            'Farming',
            'Supporting',
            'Damage/Healing',
            'Games Played'
        ],
        datasets: [
            {
                data: [
                    kdaTotal,
                    farmTotal,
                    supportTotal,
                    damageHealingTotal,
                    matchesPlayed
                ],
                fill: true,
                backgroundColor: 'rgba(255, 99, 132, 0.2)',
                borderColor: 'rgb(255, 99, 132)',
                pointBackgroundColor: 'rgb(255, 99, 132)',
                pointBorderColor: '#fff',
                pointHoverBackgroundColor: '#fff',
                pointHoverBorderColor: 'rgb(255, 99, 132)'
            },
        ]
    }

    //Scores
    var fightScore = firstPlayerAverages.avgFightScore * 100;
    var farmScore = firstPlayerAverages.avgFarmScore * 100;
    var supportScore = firstPlayerAverages.avgSupportScore * 100;
    var pushScore = firstPlayerAverages.avgPushScore * 100;

    scoreChartData.value = {
        labels: [
            'Fighting',
            'Farming',
            'Supporting',
            'Pushing'
        ],
        datasets: [
            {
                data: [fightScore, farmScore, supportScore, pushScore],
                fill: true,
                backgroundColor: 'rgba(255, 99, 132, 0.2)',
                borderColor: 'rgb(255, 99, 132)',
                pointBackgroundColor: 'rgb(255, 99, 132)',
                pointBorderColor: '#fff',
                pointHoverBackgroundColor: '#fff',
                pointHoverBorderColor: 'rgb(255, 99, 132)'
            },
        ]
    }

}

const getHeroIcon = (heroIconString: string) => {
    if (heroIconString == '') return undefined;
    return `icons/heroes/${heroIconString}.png`
}

</script>

<style scoped>
.player-stats {
    height: 100%;
    background-color: rgba(27, 38, 59, 0.4);
    /* hex for agh-fan-main-3: #1b263b */
}
</style>