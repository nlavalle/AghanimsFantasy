<template>
    <v-container>
        <v-row justify="space-around">
            <v-col class="available-team ma-2 pa-2" v-for="(team, teamIndex) in fantasyTeams" :key="teamIndex"
                :style="{ 'min-width': isDesktop ? '600px' : '340px', 'max-width': isDesktop ? '600px' : '340px' }">
                <v-row class="available-team-title">
                    <img :src="getImageUrl(team.id)" />
                    <span>{{ team.name }}</span>
                </v-row>
                <v-row>
                    <v-col class="available-player ma-1" :class="{ 'disabled-player': disabledPlayer(player.id) }"
                        v-for="(player, playerIndex) in fantasyPlayersByTeam(team.id)" :key="playerIndex"
                        :style="{ 'min-width': isDesktop ? '110px' : '60px', 'max-width': isDesktop ? '110px' : '60px' }"
                        @click="selectPlayer(player)">
                        <v-row justify="center">
                            <img :src="player.dotaAccount.steamProfilePicture" :alt="player.dotaAccount.name"
                                :style="{ width: isDesktop ? '80px' : '40px', height: isDesktop ? '80px' : '40px' }" />
                        </v-row>
                        <v-row class="available-player-caption">
                            <span style="width: 100%" :style="{ 'font-size': isDesktop ? '0.8em' : '0.5em' }">{{
                                player.dotaAccount.name }}</span>
                        </v-row>
                    </v-col>
                </v-row>
            </v-col>
        </v-row>
    </v-container>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { fantasyDraftState, type FantasyPlayer } from '../fantasyDraft';
import { VContainer, VRow, VCol } from 'vuetify/components';

const selectedPlayer = defineModel();

const { fantasyPlayersAvailable, fantasyDraftPicks } = fantasyDraftState();

const isDesktop = ref(window.outerWidth >= 600);

const fantasyTeams = computed(() => {
    // We want the distinct teams
    var teams = fantasyPlayersAvailable.value.map(item => item.team)
    return [...new Map(teams.map(item => [item['id'], item])).values()]
})

const fantasyPlayersByTeam = (teamId: number) => {
    return fantasyPlayersAvailable.value.filter(player => player.teamId == teamId)
}

const getImageUrl = (teamLogoId: number) => {
    if (teamLogoId == 0) return undefined;
    return `logos/teams_logo_${teamLogoId}.png`
}

const selectPlayer = (newPlayer: FantasyPlayer) => {
    selectedPlayer.value = newPlayer
}

const disabledPlayer = (fantasyPlayerId: number) => {
    return fantasyDraftPicks.value.filter(picks => picks.id == fantasyPlayerId).length > 0;
}

</script>

<style scoped>
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
    border: 1px solid white;
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
    pointer-events: none;
    opacity: 0.5;
}
</style>