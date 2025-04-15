<template>
    <div class="player-winnings">
        <v-row class="d-flex justify-center align-center">
            <h1>All Player Winnings</h1>
        </v-row>
        <v-row class="justify-end">
            <v-tooltip v-model="show" location="end" class="transparentbg-tooltip">
                <template v-slot:activator="{ props }">
                    <v-btn class="mr-8" v-bind="props" variant="outlined" @click="show = !show">
                        Breakdown
                    </v-btn>
                </template>
                <winnings-breakdown class="winningsBreakdown" />
            </v-tooltip>
        </v-row>
        <v-row class="justify-end">
            <v-checkbox class="pr-8 ma-0" density="compact" v-model="showAllPlayerWinnings"
                label="Filter Draft"></v-checkbox>
        </v-row>
        <li class="player-winnings-header">
            <div class="d-flex justify-evenly">
                <span class="player-header-name">Player</span>
                <span class="player-header-value">Net Gold</span>
            </div>
        </li>

        <ol class="quintile-list">
            <div v-for="quintile in [1, 2, 3, 4, 5]" :class="{
                'quintile1': quintile == 1,
                'quintile2': quintile == 2,
                'quintile3': quintile == 3,
                'quintile4': quintile == 4,
                'quintile5': quintile == 5,
            }">
                <span v-if="getQuintile(quintile).length > 0" class="quintile-top-span">{{ rankTitles[quintile - 1]
                }}</span>
                <li class="pt-0 player-winnings-item"
                    :class="{ 'drafted-player': !isDraftedPlayer(item.fantasyPlayer) }"
                    v-for="item in getQuintile(quintile)" :key="item.position" :data-rank="item.position">
                    <div class="d-flex justify-around align-center">
                        <img class="player-winnings-portrait" height="48px" width="48px"
                            :src="item.fantasyPlayer.dotaAccount.steamProfilePicture" />
                        <span v-if="item.fantasyPlayer" class="player-descriptors pl-2"
                            :style="{ fontWeight: isDraftedPlayer(item.fantasyPlayer) ? 'bold' : 'normal' }">
                            {{ item.fantasyPlayer.dotaAccount.name }}
                        </span>
                        <v-tooltip>
                            <template v-slot:activator="{ props }">
                                <span v-if="item.fantasyPlayer" v-bind="props" class="player-data pr-5"
                                    :style="{ fontWeight: isDraftedPlayer(item.fantasyPlayer) ? 'bold' : 'normal' }">
                                    {{ getWinnings(quintile, item.fantasyPlayer).toFixed(0) }}<img class="gold-coin"
                                        :src="coinStatic" /></span>
                            </template>
                            <span>{{ getBreakdown(quintile, item.fantasyPlayer) }}</span>
                        </v-tooltip>
                    </div>
                </li>
            </div>
        </ol>
    </div>
</template>

<script setup lang="ts">
import { computed, ref, type PropType } from 'vue';
import coinStatic from '@/assets/fantasy/coin/golden-coin.png'
import { VRow, VCol, VBtn, VCheckbox, VTooltip } from 'vuetify/components';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';
import type { FantasyDraftPoints, FantasyPlayer, FantasyPlayerPoints } from '../fantasyDraft';
import WinningsBreakdown from '@/components/Fantasy/Winnings/WinningsBreakdown.vue';

const props = defineProps({
    selectedDraft: {
        type: Object as PropType<FantasyDraftPoints>,
        required: false
    }
})

const showAllPlayerWinnings = ref(true);
const show = ref(false);

const fantasyLeagueStore = useFantasyLeagueStore();

const rankTitles = [
    'Top 20%',
    'Top 40%',
    'Top 60%',
    'Bottom 40%',
    'Bottom 20%'
]

const playerFantasyStatsIndexed = computed(() => {
    return fantasyLeagueStore.fantasyPlayerPoints
        .map((player: FantasyPlayerPoints, index) => ({
            ...player,
            position: index + 1
        }))
})

const isDraftedPlayer = (fantasyPlayer: FantasyPlayer) => {
    return props.selectedDraft?.fantasyDraft.draftPickPlayers.some(dpp => dpp.fantasyPlayerId == fantasyPlayer.id) ?? false;
}

const getQuintile = (quintile: number) => {
    let quintileSize = playerFantasyStatsIndexed.value.length / 5;
    return playerFantasyStatsIndexed.value
        .filter(pfsi => pfsi.position <= quintile * quintileSize && pfsi.position > (quintile - 1) * quintileSize)
        .filter(pfsi => !showAllPlayerWinnings.value || isDraftedPlayer(pfsi.fantasyPlayer));
}

const getWinnings = (quintile: number, fantasyPlayer: FantasyPlayer) => {
    let cost = fantasyLeagueStore.fantasyPlayersStats.find(fps => fps.fantasy_player.id == fantasyPlayer.id)?.cost ?? 0;
    let winnings = 300 - 60 * quintile
    return winnings - cost
}

const getBreakdown = (quintile: number, fantasyPlayer: FantasyPlayer) => {
    let cost = fantasyLeagueStore.fantasyPlayersStats.find(fps => fps.fantasy_player.id == fantasyPlayer.id)?.cost ?? 0;
    let winnings = 300 - 60 * quintile
    return `(${winnings.toFixed(0)} Winnings - ${cost.toFixed(0)} Cost)`
}

</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.transparentbg-tooltip :deep(.v-overlay__content) {
    padding: 0.3rem !important;
    background: var(--v-tooltip-bg, rgba(97, 97, 97, 0.9)) !important;
}

div ::v-deep(.player-header-name) {
    font-style: normal;
    font-weight: 400;
    font-size: 18px;
    text-align: left;
    vertical-align: middle;
    margin-top: 4px;
    margin-left: 40px;
    margin-right: 40px;
    color: var(--aghanims-fantasy-white);
    flex: 1 0;
}

div ::v-deep(.player-header-value) {
    font-style: normal;
    font-weight: 400;
    font-size: 18px;
    text-align: end;
    vertical-align: middle;
    margin-top: 4px;
    margin-left: 40px;
    margin-right: 40px;
    color: var(--aghanims-fantasy-white);
    flex: 0.5 1;
}

div ::v-deep(.player-descriptors) {
    font-style: normal;
    font-weight: 400;
    font-size: 16px;
    text-align: left;
    vertical-align: middle;
    margin-top: 4px;
    flex: 0.6 0;
    height: 48px;
}

div ::v-deep(.player-data) {
    font-family: system-ui;
    font-style: normal;
    font-size: 18px;
    text-align: end;
    vertical-align: middle;
    flex: 0.4 1;
}

.gold-coin {
    display: inline-block;
    vertical-align: middle;
}

.quintile-img {
    height: 40px;
    display: inline-block;
    vertical-align: middle;
}

.quintile-list {
    border-radius: 0px 0px 10px 10px;
}

.quintile1 {
    position: relative;
    background-image: url('/quintile/quintile1.png'), linear-gradient(#c73008, #491505);
    background-size: 30%, cover;
    background-position: center;
    box-shadow: 0 -2.5px 0 rgba(0, 0, 0, 0.12);
    border-radius: 0px 0px 10px 10px;
}

.quintile2 {
    position: relative;
    background-image: url('/quintile/quintile2.png'), linear-gradient(#8290c8, #171d2d);
    background-size: 30%, cover;
    background-position: center;
    box-shadow: 0 -2.5px 0 rgba(0, 0, 0, 0.12);
    border-radius: 0px 0px 10px 10px;
}

.quintile3 {
    position: relative;
    background-image: url('/quintile/quintile3.png'), linear-gradient(#8290c8, #8d00c3);
    background-size: 30%, cover;
    background-position: center;
    box-shadow: 0 -2.5px 0 rgba(0, 0, 0, 0.12);
    border-radius: 0px 0px 10px 10px;
}

.quintile4 {
    position: relative;
    background-image: url('/quintile/quintile4.png'), linear-gradient(#767674, #26a185);
    background-size: 30%, cover;
    background-position: center;
    box-shadow: 0 -2.5px 0 rgba(0, 0, 0, 0.12);
    border-radius: 0px 0px 10px 10px;
}

.quintile5 {
    position: relative;
    background-image: url('/quintile/quintile5.png'), linear-gradient(#9ebb68, #1d2c15);
    background-size: 30%, cover;
    background-position: center;
    box-shadow: 0 -2.5px 0 rgba(0, 0, 0, 0.12);
    border-radius: 0px 0px 10px 10px;
}

.quintile-top-span {
    font-size: large;
    font-weight: bold;
    position: absolute;
    top: 50%;
    left: 40%;
    color: rgb(163, 163, 163);
    text-shadow:
        2px 2px 2px black,
        -2px -2px 2px black,
        2px -2px 2px black,
        -2px 2px 2px black;
}

.drafted-player {
    background: rgba(191, 191, 191, 0.4);
}

.winningsBreakdown {
    max-width: 300px;
}

/*-------------------- Player Winnings --------------------*/
.player-winnings {
    background-color: var(--aghanims-fantasy-main-4);
    border: 5px solid var(--aghanims-fantasy-main-2);
    border-radius: 15px;
    box-shadow: 0 7px 30px rgba(62, 9, 11, 0.3);
    margin-bottom: 10px;
    margin-left: 5px;
    margin-right: 5px;
    margin-top: 10px;
    height: max-content;
    flex: 1 0 300px;
}

.player-winnings-header {
    background-color: var(--aghanims-fantasy-main-2);
    padding-bottom: 5px;
    list-style: none;
}

.player-winnings-item {
    height: 3rem;
}

.player-winnings-portrait {
    background: radial-gradient(rgba(176, 176, 176, 0.12), rgba(0, 0, 0, 0.3));
}

.player-winnings h1 {
    font-size: 24px;
    color: var(--aghanims-fantasy-white);
    padding: 0px 13px 0px;
    line-height: 6rem;
}

.player-winnings h1 svg {
    width: 25px;
    height: 26px;
    position: relative;
    top: 3px;
    margin-right: 6px;
    vertical-align: baseline;
}

.player-winnings ol {
    margin-top: 0px;
    list-style-type: none;
    padding: 0;
}

.player-winnings ol li {
    position: relative;
    font-size: 16px;
    font-family: Arial, Helvetica, sans-serif;
    padding: 12px 12px 12px 50px;
    backface-visibility: hidden;
    background-repeat: no-repeat;
    background-image: linear-gradient(rgba(191, 191, 191, 0.1), rgba(83, 83, 83, 0.1));
}

.player-winnings ol li::before {
    content: attr(data-rank);
    position: absolute;
    z-index: 2;
    top: 15px;
    left: 15px;
    width: 25px;
    height: 25px;
    line-height: 25px;
    color: var(--gradient-blue-6);
    background: #fff;
    border-radius: 25px;
    text-align: center;

}

.player-winnings ol li span {
    z-index: 2;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    margin: 0;
    background: none;
    color: #fff;
    text-shadow:
        1px 1px 0px var(--gradient-blue-10),
        -1px -1px 0px var(--gradient-blue-10),
        1px -1px 0px var(--gradient-blue-10),
        -1px 1px 0px var(--gradient-blue-10);
}

.player-winnings ol li small {
    z-index: 2;
    height: 100%;
    text-align: right;
    font-family: system-ui;
    font-style: normal;
    font-size: 18px;
    flex: 0.5 1 100px;
    color: #fff;
}

.player-winnings ol li:nth-last-child(1) {
    border-radius: 0px 0px 10px 10px;
}
</style>
