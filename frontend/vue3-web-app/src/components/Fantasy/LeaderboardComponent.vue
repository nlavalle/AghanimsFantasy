<template>
    <div class="leaderboard">
        <v-row class="d-flex justify-center">
            <h1><trophy-svg /> {{ props.leaderboardTitle }}</h1>
        </v-row>
        <li class="leaderboard-header">
            <div class="d-flex justify-evenly">
                <span v-if="headerName" class="player-header-name">{{ props.headerName }}</span>
                <span v-if="headerValue" class="player-header-value">{{ props.headerValue }}</span>
            </div>
        </li>

        <ol>
            <li class="leaderboard-item" v-for="item in props.boardData" :key="item.id">
                <div class="d-flex justify-around">
                    <span v-if="item.userName" class="player-descriptors"
                        :style="{ fontWeight: item.userName == props.authenticatedUser?.name ? 'bold' : 'normal' }">
                        {{ item.userName }}
                    </span>
                    <span v-if="item.value" class="player-data"
                        :style="{ fontWeight: item.userName == props.authenticatedUser?.name ? 'bold' : 'normal' }">
                        {{ item.value.toFixed(2).toLocaleString() }}</span>
                </div>
                <div class="leaderboard-details bg-surface"
                    v-if="!fantasyLeagueStore.isDraftOpen(fantasyLeagueStore.selectedFantasyLeague)">
                    <p v-for="pick in item.fantasyDraft.draftPickPlayers.sort((a, b) => a.draftOrder - b.draftOrder)"
                        style="text-align:right">
                        {{ pick.fantasyPlayer.dotaAccount.name }}: {{ item.playerPoints[pick.draftOrder - 1].toFixed(2)
                            ?? 0 }}
                    </p>
                </div>
            </li>
        </ol>
    </div>
</template>

<script setup lang="ts">
import type { PropType } from 'vue';
import type { User } from '@/stores/auth';
import type { LeaderboardItem } from '@/types/LeaderboardItem';
import TrophySvg from '@/components/icons/TrophySvg.vue'
import { VRow } from 'vuetify/components';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';

const props = defineProps({
    leaderboardTitle: {
        type: String
    },
    headerName: {
        type: String
    },
    headerValue: {
        type: String
    },
    boardData: {
        type: Array as PropType<LeaderboardItem[]>,
        required: true
    },
    authenticatedUser: {
        type: Object as PropType<Partial<User>>,
        required: false
    }
})

const fantasyLeagueStore = useFantasyLeagueStore();

</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
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
    text-align: center;
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
    flex: 1 0;
    height: 48px;
}

div ::v-deep(.player-data) {
    font-family: system-ui;
    font-style: normal;
    font-size: 18px;
    text-align: center;
    vertical-align: middle;
    flex: 0.5 1;
}

/*-------------------- Leaderboard --------------------*/
.leaderboard {
    background-color: var(--aghanims-fantasy-main-4);
    border: 5px solid var(--aghanims-fantasy-main-2);
    border-radius: 15px;
    box-shadow: 0 7px 30px rgba(62, 9, 11, 0.3);
    margin-bottom: 10px;
    margin-left: 5px;
    margin-right: 5px;
    height: max-content;
    flex: 1 0 300px;
}

.leaderboard-header {
    background-color: var(--aghanims-fantasy-main-2);
    padding-bottom: 5px;
    list-style: none;
}

.leaderboard-item {
    height: 3rem;
}

.leaderboard h1 {
    font-size: 24px;
    color: var(--aghanims-fantasy-white);
    padding: 0px 13px 0px;
    line-height: 6rem;
}

.leaderboard h1 svg {
    width: 25px;
    height: 26px;
    position: relative;
    top: 3px;
    margin-right: 6px;
    vertical-align: baseline;
}

.leaderboard ol {
    margin-top: 0px;
    counter-reset: leaderboard;
    list-style-type: none;
    padding: 0;
}

.leaderboard ol li {
    position: relative;
    font-size: 16px;
    font-family: Arial, Helvetica, sans-serif;
    counter-increment: leaderboard;
    padding: 12px 12px 12px 50px;
    backface-visibility: hidden;
}

.leaderboard ol li::before {
    content: counter(leaderboard);
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

.leaderboard ol li span {
    z-index: 2;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    margin: 0;
    background: none;
    color: #fff;
}

.leaderboard ol li span::before,
.leaderboard ol li span::after {
    content: '';
    position: absolute;
    z-index: 1;
    bottom: -11px;
    left: -9px;
    border-left: 10px solid transparent;
    transition: all 0.1s ease-in-out;
    opacity: 0;
}

.leaderboard ol li span::after {
    left: auto;
    right: -9px;
    border-left: none;
    border-right: 10px solid transparent;
}

.leaderboard ol li small {
    z-index: 2;
    height: 100%;
    text-align: right;
    font-family: system-ui;
    font-style: normal;
    font-size: 18px;
    flex: 0.5 1 100px;
    color: #fff;
}

.leaderboard ol li::after {
    z-index: 1;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    box-shadow: 0 3px 0 rgba(0, 0, 0, 0.08);
    transition: all 0.3s ease-in-out;
    opacity: 0;
}

.leaderboard ol li:nth-child(1) {
    background: var(--gradient-blue-3);
}

.leaderboard ol li:nth-child(1)::after {
    background: var(--gradient-blue-3);
}

.leaderboard ol li:nth-child(2) {
    background: var(--gradient-blue-4);
}

.leaderboard ol li:nth-child(2)::after {
    background: var(--gradient-blue-4);
    box-shadow: 0 2px 0 rgba(0, 0, 0, 0.08);
}

.leaderboard ol li:nth-child(2) span::before,
.leaderboard ol li:nth-child(2) span::after {
    border-top: 6px solid var(--gradient-blue-4);
    bottom: -7px;
}

.leaderboard ol li:nth-child(3) {
    background: var(--gradient-blue-5);
}

.leaderboard ol li:nth-child(3)::after {
    background: var(--gradient-blue-5);
    box-shadow: 0 1px 0 rgba(0, 0, 0, 0.11);
}

.leaderboard ol li:nth-child(3) span::before,
.leaderboard ol li:nth-child(3) span::after {
    border-top: 2px solid var(--gradient-blue-5);
    bottom: -3px;
}

.leaderboard ol li:nth-child(4) {
    background: var(--gradient-blue-6);
}

.leaderboard ol li:nth-child(4)::after {
    background: var(--gradient-blue-6);
    box-shadow: 0 -1px 0 rgba(0, 0, 0, 0.15);
}

.leaderboard ol li:nth-child(4) span::before,
.leaderboard ol li:nth-child(4) span::after {
    top: -7px;
    bottom: auto;
    border-top: none;
    border-bottom: 6px solid var(--gradient-blue-6);
}

.leaderboard ol li:nth-child(n + 5) {
    background: var(--gradient-blue-7);
}

.leaderboard ol li:nth-child(n + 5)::after {
    background: var(--gradient-blue-7);
    box-shadow: 0 -1px 0 rgba(0, 0, 0, 0.15);
}

.leaderboard ol li:nth-child(n + 5) span::before,
.leaderboard ol li:nth-child(n + 5) span::after {
    top: -7px;
    bottom: auto;
    border-top: none;
    border-bottom: 6px solid var(--gradient-blue-7);
}

.leaderboard ol li:nth-last-child(1) {
    background: var(--gradient-blue-8);
    border-radius: 0 0 10px 10px;
}

.leaderboard ol li:nth-last-child(1)::after {
    background: var(--gradient-blue-8);
    box-shadow: 0 -2.5px 0 rgba(0, 0, 0, 0.12);
    border-radius: 0 0 10px 10px;
}

.leaderboard ol li:nth-last-child(1) span::before,
.leaderboard ol li:nth-last-child(1) span::after {
    top: -9px;
    bottom: auto;
    border-top: none;
    border-bottom: 8px solid var(--gradient-blue-8);
}

.leaderboard-details {
    display: none;
    position: absolute;
    top: 20px;
    right: 140px;
    background-color: white;
    z-index: 10;
    padding: 10px;
    box-shadow: 0px 4px 8px rgba(0, 0, 0, 0.1);
    pointer-events: none;
}

li:hover .leaderboard-details {
    display: block;
}
</style>
