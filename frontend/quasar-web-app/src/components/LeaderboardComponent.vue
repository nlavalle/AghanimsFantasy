<template>
    <div style="flex: 1 0 300px;" class="leaderboard">
        <h1>
            <trophy-svg /> {{ leaderboardTitle }}
        </h1>
        <li class="flex-row" style="background-color: #474b53;">
            <div class="row justify-evenly">
                <span v-if="headerName" class="player-header">{{ headerName }}</span>
                <span v-if="headerValue" class="player-header">{{ headerValue }}</span>
            </div>
        </li>
        <ol>
            <li class="flex-row" v-for="item in boardData" :key="item.id">
                <div class="row justify-around">
                    <span v-if="item.description" class="player-descriptors">{{ item.description }}</span>
                    <span v-if="item.value" class="player-data">{{ item.value }}</span>
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
    },
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

div ::v-deep(.player-header) {
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
    background: linear-gradient(to bottom, #3a404d, #181c26);
    border-radius: 10px;
    box-shadow: 0 7px 30px rgba(62, 9, 11, .3);
    margin-bottom: 10px;
    margin-left: 5px;
    margin-right: 5px;
    height: max-content;
}

.leaderboard h1 {
    font-size: 24px;
    color: #e1e1e1;
    padding: 12px 13px 18px;
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
    counter-reset: leaderboard;
    list-style-type: none;
    padding: 0;
}

.leaderboard ol li {
    z-index: 1;
    font-size: 20px;
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
    color: #6492f3;
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
    border-top: 10px solid #5a86e4;
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
    background: #5a86e4;
    box-shadow: 0 3px 0 rgba(0, 0, 0, .08);
    transition: all 0.3s ease-in-out;
    opacity: 0;
}

.leaderboard ol li:nth-child(1) {
    background: #84abff;
}

.leaderboard ol li:nth-child(1)::after {
    background: #84abff;
}

.leaderboard ol li:nth-child(2) {
    background: #739cf3;
}

.leaderboard ol li:nth-child(2)::after {
    background: #739cf3;
    box-shadow: 0 2px 0 rgba(0, 0, 0, .08);
}

.leaderboard ol li:nth-child(2) span::before,
.leaderboard ol li:nth-child(2) span::after {
    border-top: 6px solid #739cf3;
    bottom: -7px;
}

.leaderboard ol li:nth-child(3) {
    background: #5985e6;
}

.leaderboard ol li:nth-child(3)::after {
    background: #5985e6;
    box-shadow: 0 1px 0 rgba(0, 0, 0, .11);
}

.leaderboard ol li:nth-child(3) span::before,
.leaderboard ol li:nth-child(3) span::after {
    border-top: 2px solid #5985e6;
    bottom: -3px;
}

.leaderboard ol li:nth-child(4) {
    background: #4877dd;
}

.leaderboard ol li:nth-child(4)::after {
    background: #4877dd;
    box-shadow: 0 -1px 0 rgba(0, 0, 0, .15);
}

.leaderboard ol li:nth-child(4) span::before,
.leaderboard ol li:nth-child(4) span::after {
    top: -7px;
    bottom: auto;
    border-top: none;
    border-bottom: 6px solid #4877dd;
}

.leaderboard ol li:nth-child(n+5) {
    background: #3366d3;
}

.leaderboard ol li:nth-child(n+5)::after {
    background: #3366d3;
    box-shadow: 0 -1px 0 rgba(0, 0, 0, .15);
}

.leaderboard ol li:nth-child(n+5) span::before,
.leaderboard ol li:nth-child(n+5) span::after {
    top: -7px;
    bottom: auto;
    border-top: none;
    border-bottom: 6px solid #3366d3;
}

.leaderboard ol li:nth-last-child(1) {
    background: #2459ca;
    border-radius: 0 0 10px 10px;
}

.leaderboard ol li:nth-last-child(1)::after {
    background: #2459ca;
    box-shadow: 0 -2.5px 0 rgba(0, 0, 0, .12);
    border-radius: 0 0 10px 10px;
}

.leaderboard ol li:nth-last-child(1) span::before,
.leaderboard ol li:nth-last-child(1) span::after {
    top: -9px;
    bottom: auto;
    border-top: none;
    border-bottom: 8px solid #2459ca;
}
</style>