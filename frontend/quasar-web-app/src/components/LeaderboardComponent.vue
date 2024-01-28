<template>
    <div style="flex: 1 0 300px;" class="leaderboard">
        <h1>
            <trophy-svg /> {{ leaderboardTitle }}
        </h1>
        <li class="flex-row leaderboard-header">
            <div class="row justify-evenly">
                <span v-if="headerName" class="player-header-name">{{ headerName }}</span>
                <span v-if="headerValue" class="player-header-value">{{ headerValue }}</span>
            </div>
        </li>
        <ol>
            <li class="flex-row" v-for="item in boardData" :key="item.id">
                <div class="row justify-around">
                    <span v-if="item.description" class="player-descriptors"
                        :style="{ fontWeight: item.description == authenticatedUser.name ? 'bold' : 'normal' }">
                        <q-img v-if="item.isTeam" height="25px" width="25px" :src="getImageUrl(item.teamId)" />
                        {{ item.description }}
                    </span>
                    <span v-if="item.value" class="player-data"
                        :style="{ fontWeight: item.description == authenticatedUser.name ? 'bold' : 'normal' }">
                        {{ item.value.toFixed(2).toLocaleString() }}</span>
                </div>
            </li>
        </ol>
    </div>
</template>

<script>
import {
    defineComponent,
} from 'vue';
import TrophySvg from 'src/components/TrophySvg.vue';

export default defineComponent({
    name: 'LeaderboardComponent',
    components: {
        TrophySvg,
    },
    props: {
        leaderboardTitle: {
            type: String,
        },
        headerName: {
            type: String,
        },
        headerValue: {
            type: String,
        },
        boardData: {
            type: Array,
            required: true
        },
        authenticatedUser: {
            type: String,
            required: false,
        }
    },
    methods: {
        getImageUrl(teamId) {
            if (teamId == 0) return null;
            return new URL(`../assets/logos/teams_logo_${teamId}.png`, import.meta.url).toString();
        }
    }
});
</script>

  <!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.debug {
    border: 1px solid red;
    padding: 10px;
}

.left-fixed {
    flex: 0 0 300px;
}

.flex-container {
    display: flex;
    flex-flow: row wrap;
    max-width: 100%;
    padding: 20px;
}

.flex-break {
    flex: 1 0 100% !important
}

.row {
    width: 100%;
}

.row .flex-break {
    height: 0 !important
}

.column .flex-break {
    width: 0 !important
}

div ::v-deep(.player-header-name) {
    font-family: Arial, Helvetica, sans-serif;
    font-style: normal;
    font-weight: 400;
    font-size: 18px;
    text-align: left;
    vertical-align: middle;
    margin-top: 4px;
    margin-left: 40px;
    margin-right: 40px;
    color: white;
    flex: 1 0;
}

div ::v-deep(.player-header-value) {
    font-family: Arial, Helvetica, sans-serif;
    font-style: normal;
    font-weight: 400;
    font-size: 18px;
    text-align: center;
    vertical-align: middle;
    margin-top: 4px;
    margin-left: 40px;
    margin-right: 40px;
    color: white;
    flex: 0.5 1;
}


div ::v-deep(.player-descriptors) {
    font-family: Arial, Helvetica, sans-serif;
    font-style: normal;
    font-weight: 400;
    font-size: 16px;
    text-align: left;
    vertical-align: middle;
    margin-top: 4px;
    flex: 1 0;
}

div ::v-deep(.player-data) {
    font-family: system-ui;
    font-style: normal;
    font-size: 18px;
    text-align: center;
    vertical-align: middle;
    flex: 0.5 1;
}

div ::v-deep(.ico-cup-reverse) {
    transform: rotate(180deg);
}

/*-------------------- Leaderboard --------------------*/
.leaderboard {
    /* background: linear-gradient(to bottom, #3a404d, #181c26); */
    background-color: var(--nadcl-main-4);
    border: 5px solid var(--nadcl-accent-dark);
    border-radius: 10px;
    box-shadow: 0 7px 30px rgba(62, 9, 11, .3);
    margin-bottom: 10px;
    margin-left: 5px;
    margin-right: 5px;
    height: max-content;
}

.leaderboard-header {
    background-color: var(--nadcl-accent-dark);
    padding-bottom: 5px;
    list-style: none;
}

.leaderboard h1 {
    font-size: 24px;
    color: var(--nadcl-white);
    padding: 0px 13px 0px;
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
    border-top: 5px solid var(--nadcl-main-3);
    margin-top: 0px;
    counter-reset: leaderboard;
    list-style-type: none;
    padding: 0;
}

.leaderboard ol li {
    z-index: 1;
    font-size: 16px;
    font-family: Arial, Helvetica, sans-serif;
    counter-increment: leaderboard;
    padding: 12px 12px 12px 50px;
    backface-visibility: hidden;
    transform: translateZ(0) scale(1, 1);
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
    color: var(--gradient-purple-1);
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
    box-shadow: 0 3px 0 rgba(0, 0, 0, .08);
    transition: all 0.3s ease-in-out;
    opacity: 0;
}

.leaderboard ol li:nth-child(1) {
    background: var(--gradient-purple-1);
}

.leaderboard ol li:nth-child(1)::after {
    background: var(--gradient-purple-1);
}

.leaderboard ol li:nth-child(2) {
    background: var(--gradient-purple-2);
}

.leaderboard ol li:nth-child(2)::after {
    background: var(--gradient-purple-2);
    box-shadow: 0 2px 0 rgba(0, 0, 0, .08);
}

.leaderboard ol li:nth-child(2) span::before,
.leaderboard ol li:nth-child(2) span::after {
    border-top: 6px solid var(--gradient-purple-2);
    bottom: -7px;
}

.leaderboard ol li:nth-child(3) {
    background: var(--gradient-purple-3);
}

.leaderboard ol li:nth-child(3)::after {
    background: var(--gradient-purple-3);
    box-shadow: 0 1px 0 rgba(0, 0, 0, .11);
}

.leaderboard ol li:nth-child(3) span::before,
.leaderboard ol li:nth-child(3) span::after {
    border-top: 2px solid var(--gradient-purple-3);
    bottom: -3px;
}

.leaderboard ol li:nth-child(4) {
    background: var(--gradient-purple-4);
}

.leaderboard ol li:nth-child(4)::after {
    background: var(--gradient-purple-4);
    box-shadow: 0 -1px 0 rgba(0, 0, 0, .15);
}

.leaderboard ol li:nth-child(4) span::before,
.leaderboard ol li:nth-child(4) span::after {
    top: -7px;
    bottom: auto;
    border-top: none;
    border-bottom: 6px solid var(--gradient-purple-4);
}

.leaderboard ol li:nth-child(n+5) {
    background: var(--gradient-purple-5);
}

.leaderboard ol li:nth-child(n+5)::after {
    background: var(--gradient-purple-5);
    box-shadow: 0 -1px 0 rgba(0, 0, 0, .15);
}

.leaderboard ol li:nth-child(n+5) span::before,
.leaderboard ol li:nth-child(n+5) span::after {
    top: -7px;
    bottom: auto;
    border-top: none;
    border-bottom: 6px solid var(--gradient-purple-5);
}

.leaderboard ol li:nth-last-child(1) {
    background: var(--gradient-purple-8);
    border-radius: 0 0 10px 10px;
}

.leaderboard ol li:nth-last-child(1)::after {
    background: var(--gradient-purple-8);
    box-shadow: 0 -2.5px 0 rgba(0, 0, 0, .12);
    border-radius: 0 0 10px 10px;
}

.leaderboard ol li:nth-last-child(1) span::before,
.leaderboard ol li:nth-last-child(1) span::after {
    top: -9px;
    bottom: auto;
    border-top: none;
    border-bottom: 8px solid var(--gradient-purple-8);
}
</style>