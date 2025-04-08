<template>
    <v-container>
        <v-row justify="space-around">
            <v-col class="available-team ma-2 pa-2" v-for="(team, teamIndex) in fantasyTeams" :key="teamIndex"
                :style="{ 'min-width': isDesktop ? '600px' : '340px', 'max-width': isDesktop ? '600px' : '340px' }">
                <v-row class="available-team-title">
                    <img :src="getImageUrl(team.teamId)" />
                    <span>{{ team.name }}</span>
                </v-row>
                <v-row>
                    <v-col class="available-player ma-1 pa-0"
                        :class="{ 'disabled-player': disabledPlayer(player.fantasyPlayer), 'selected-player': selectedPlayerCheck(player.fantasyPlayer.id) }"
                        v-for="(player, playerIndex) in fantasyPlayersByTeam(team.teamId)" :key="playerIndex"
                        :style="{ 'min-width': isDesktop ? '110px' : '60px', 'max-width': isDesktop ? '110px' : '60px' }"
                        @click="selectPlayer(player.fantasyPlayer)">
                        <draft-pick-card size="small" :fantasyPlayer="player.fantasyPlayer"
                            :fantasyPoints="player.totalMatchFantasyPoints"
                            :fantasyLeagueActive="!leagueStore.isDraftOpen(leagueStore.selectedFantasyLeague)"
                            :fantasyPlayerCost="fantasyPlayerCost(player.fantasyPlayerId)"
                            :fantasyPlayerBudget="600 + draftSlotCost(player.fantasyPlayer) - draftCost" />
                    </v-col>
                </v-row>
                <!-- overlay without scroll-strategy bricks the page scrolling: https://github.com/vuetifyjs/vuetify/issues/15653 -->
                <v-overlay :model-value="disabledTeam(team.teamId)"
                    class="ma-6 align-center justify-center disabled-team" contained persistent no-click-animation
                    scroll-strategy="none" z-index="2">
                    <span>Max 2 players per team</span>
                </v-overlay>
            </v-col>
        </v-row>
    </v-container>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { fantasyDraftState, type FantasyPlayer, type FantasyPlayerPoints } from '../fantasyDraft';
import { VContainer, VRow, VCol, VOverlay } from 'vuetify/components';
import DraftPickCard from '@/components/Fantasy/DraftPickCard.vue';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';

const { selectedPlayer, fantasyPlayerPointsAvailable, disabledPlayer, disabledTeam, totalDraftCost, currentDraftSlotCost } = fantasyDraftState();
const leagueStore = useFantasyLeagueStore();

const isDesktop = ref(window.outerWidth >= 600);

const fantasyTeams = computed(() => {
    // We want the distinct teams
    let teams = fantasyPlayerPointsAvailable.value.map(item => ({ teamId: item.fantasyPlayer.teamId, ...item.fantasyPlayer.team }))
    return [...new Map(teams.map(item => [item.teamId, item])).values()]
})

const draftCost = computed(() => {
    return totalDraftCost(leagueStore.fantasyPlayersStats);
})

const draftSlotCost = (fantasyPlayer: FantasyPlayer) => {
    return currentDraftSlotCost(leagueStore.fantasyPlayersStats, fantasyPlayer.teamPosition);
}

const fantasyPlayerCost = (fantasyPlayerId: number) => {
    return leagueStore.fantasyPlayersStats.find(fps => fps.fantasy_player.id == fantasyPlayerId)?.cost ?? 0
}

const fantasyPlayersByTeam = (teamId: number) => {
    return fantasyPlayerPointsAvailable.value.filter(player => player.fantasyPlayer.teamId == teamId).sort((playerA: FantasyPlayerPoints, playerB: FantasyPlayerPoints) => {
        if (playerA.fantasyPlayer.teamPosition < playerB.fantasyPlayer.teamPosition) return -1;
        if (playerA.fantasyPlayer.teamPosition > playerB.fantasyPlayer.teamPosition) return 1;
        return 0;
    })
}

const getImageUrl = (teamLogoId: number) => {
    if (teamLogoId == 0) return undefined;
    return `logos/teams_logo_${teamLogoId}.png`
}

const selectPlayer = (newPlayer: FantasyPlayer) => {
    selectedPlayer.value = newPlayer
}

const selectedPlayerCheck = (fantasyPlayerId: number) => {
    if (!selectedPlayer.value) return false;
    return selectedPlayer.value!.id == fantasyPlayerId;
}


</script>

<style scoped>
.available-team {
    position: relative;
}

.disabled-team {
    pointer-events: none;
    border-radius: 10px;
}

.available-team-title {
    margin-right: 5px;
    margin-top: 5px;
}

.available-team-title img {
    height: 40px;
    margin-right: 10px;
}

.available-player {
    overflow: hidden;
    margin-bottom: 5px;
    /* border: 1px solid white; */
}

.available-player img {
    object-fit: cover;
}

.available-player-caption {
    color: #FFF;
    text-align: center;
}

.disabled-player {
    background: rgba(122, 122, 122);
    /* pointer-events: none; */
    opacity: 0.5;
    border-radius: 5px;
}

.selected-player {
    box-shadow:
        0 0 2px var(--aghanims-fantasy-main-1),
        0 0 4px var(--aghanims-fantasy-main-1),
        0 0 6px var(--aghanims-fantasy-main-1),
        0 0 8px var(--aghanims-fantasy-main-1);
    border-radius: 8px;
}
</style>