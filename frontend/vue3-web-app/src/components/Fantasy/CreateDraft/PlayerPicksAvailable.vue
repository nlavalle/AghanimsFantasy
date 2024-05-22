<template>
    <v-container fluid>
        <v-row justify="space-around">
            <v-col class="available-team ma-2 pa-2" v-for="(team, teamIndex) in fantasyTeams" :key="teamIndex">
                <v-row>
                    <v-col>
                        <v-row class="available-team-title">
                            <img :src="getImageUrl(team.id)" />
                            <span>{{ team.name }}</span>
                        </v-row>
                        <v-row>
                            <v-col :class="{ 'disabled-player': disabledPlayer(player.id) }"
                                v-for="(player, playerIndex) in fantasyPlayersByTeam(team.id)" :key="playerIndex"
                                @click="selectPlayer(player)">
                                <v-row class="available-player justify-center">
                                    <img :src="player.dotaAccount.steamProfilePicture" :alt="player.dotaAccount.name" />
                                </v-row>
                                <v-row class="available-player-caption">
                                    <span style="width: 100%">{{ player.dotaAccount.name }}</span>
                                </v-row>
                            </v-col>
                        </v-row>
                    </v-col>
                </v-row>
            </v-col>
        </v-row>

    </v-container>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { fantasyDraftState, type FantasyPlayer } from '../fantasyDraft';
import { VContainer, VRow, VCol } from 'vuetify/components';

const selectedPlayer = defineModel();

const { fantasyPlayersAvailable, fantasyDraftPicks } = fantasyDraftState();

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
.available-team {
    min-width: 500px;
    max-width: 500px;
    margin-bottom: 15px;
}

.available-team-title {
    width: 250px;
    height: 50px;
    margin-right: 5px;
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
    width: 80px;
    height: 80px;
    object-fit: cover;
}

.available-player-caption {
    font-size: 0.8em;
    color: #FFF;
    text-align: center;
}

.disabled-player {
    background: rgba(122, 122, 122);
    pointer-events: none;
    opacity: 0.5;
}
</style>