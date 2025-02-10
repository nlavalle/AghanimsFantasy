<template>
    <v-container>
        <v-row>
            <v-col>
                <v-select label="Fantasy League" v-model="selectedFantasyLeague" :items="availableFantasyLeagues"
                    item-value="id" variant="underlined" return-object>
                    <template v-slot:selection="{ item }" slot-scope="data">
                        <p>{{ fullFantasyName(item.raw.leagueId, item.raw.name) }}</p>
                    </template>
                    <template v-slot:item="{ props, item }">
                        <v-list-item v-bind="props" class="league-selector"
                            :title="fullFantasyName(item.raw.leagueId, item.raw.name)"></v-list-item>
                    </template>
                </v-select>
            </v-col>
        </v-row>
        <v-row>
            <v-col>
                <v-row v-if="selectedFantasyLeague">
                    <v-col style="max-width: 500px">
                        <v-row>
                            <v-col>
                                <p>Add Team</p>
                            </v-col>
                            <v-col>
                                <v-btn @click="addTeam()">Add</v-btn>
                            </v-col>
                        </v-row>
                        <v-row>
                            <v-autocomplete v-model="selectedTeam" :items="teams" item-title="name" max-width="300"
                                return-object />
                        </v-row>
                    </v-col>
                    <v-col class="available-team ma-2 pa-2" v-for="(team, teamIndex) in fantasyTeams" :key="teamIndex"
                        :style="{ 'min-width': isDesktop ? '600px' : '340px', 'max-width': isDesktop ? '600px' : '340px' }">
                        <v-row class="available-team-title">
                            <v-col>
                                <img :src="getImageUrl(team.id)" />
                                <span>{{ team.name }}</span>
                            </v-col>
                            <v-col v-if="fantasyPlayersByTeam(team.id).length == 0">
                                <v-btn @click="deleteTeam(team)">Delete Team</v-btn>
                            </v-col>
                        </v-row>
                        <v-row>
                            <v-col class="available-player ma-1"
                                v-for="(player, playerIndex) in fantasyPlayersByTeam(team.id)" :key="playerIndex"
                                :style="{ 'min-width': isDesktop ? '110px' : '60px', 'max-width': isDesktop ? '110px' : '60px' }">
                                <v-row justify="center">
                                    <img :src="player.dotaAccount!.steamProfilePicture" :alt="player.dotaAccount!.name"
                                        :style="{ width: isDesktop ? '80px' : '40px', height: isDesktop ? '80px' : '40px' }" />
                                </v-row>
                                <v-row class="available-player-caption">
                                    <span style="width: 100%" :style="{ 'font-size': isDesktop ? '0.8em' : '0.5em' }">{{
                                        player.dotaAccount!.name }}</span>
                                </v-row>
                                <v-row>
                                    <font-awesome-icon :icon="faDeleteLeft" class="me-2"
                                        @click="deleteFantasyPlayer(player)" />
                                </v-row>
                            </v-col>
                            <v-col v-if="fantasyPlayersByTeam(team.id).length < 5">
                                <v-autocomplete v-model="selectedPlayer" :items="players" item-title="name"
                                    max-width="300" return-object />
                                <v-btn
                                    @click="addFantasyPlayer(team.id, selectedPlayer?.id!, fantasyPlayersByTeam(team.id).length + 1)">Add
                                    Fantasy Player</v-btn>
                            </v-col>
                        </v-row>
                    </v-col>
                </v-row>
            </v-col>
        </v-row>
        <v-dialog v-model="dialogAddFantasyPlayer" max-width="500px">
            <v-card>
                <v-card-title class="text-h7">Are you sure you want to add this Fantasy Player?</v-card-title>
                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="blue-darken-1" variant="text" @click="closeAddFantasyPlayer">Cancel</v-btn>
                    <v-btn color="blue-darken-1" variant="text" @click="addFantasyPlayerConfirm">OK</v-btn>
                    <v-spacer></v-spacer>
                </v-card-actions>
            </v-card>
        </v-dialog>
        <v-dialog v-model="dialogDeleteFantasyPlayer" max-width="500px">
            <v-card>
                <v-card-title class="text-h7">Are you sure you want to delete this Fantasy Player?</v-card-title>
                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="blue-darken-1" variant="text" @click="closeDeleteFantasyPlayer">Cancel</v-btn>
                    <v-btn color="blue-darken-1" variant="text" @click="deleteFantasyPlayerConfirm">OK</v-btn>
                    <v-spacer></v-spacer>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </v-container>
</template>

<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue';
import { VContainer, VRow, VCol, VListItem, VSelect, VAutocomplete, VBtn, VDialog, VCard, VCardTitle, VCardActions, VSpacer } from 'vuetify/components';
import { localApiService } from '@/services/localApiService';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';
import type { FantasyLeague } from '@/types/FantasyLeague';
import type { FantasyPlayer } from '../Fantasy/fantasyDraft';
import type { DotaAccount, DotaTeam } from '@/types/Dota';
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import { faDeleteLeft } from '@fortawesome/free-solid-svg-icons';

const isDesktop = ref(window.outerWidth >= 600);

const leagueStore = useFantasyLeagueStore();
const selectedFantasyLeague = ref<FantasyLeague>();
const fantasyPlayers = ref<FantasyPlayer[]>([]);
const fantasyTeams = ref<any[]>([]);
const teams = ref<DotaTeam[]>([]);
const selectedTeam = ref<DotaTeam>();
const players = ref<DotaAccount[]>([]);
const selectedPlayer = ref<DotaAccount>();

//AddPlayer
const dialogAddFantasyPlayer = ref(false);
const addFantasyPlayerTeamId = ref();
const addFantasyPlayerAccountId = ref();
const addFantasyPlayerTeamPositionId = ref();

//DeletePlayer
const dialogDeleteFantasyPlayer = ref(false);
const deleteFantasyPlayerItem = ref<FantasyPlayer>();

const availableFantasyLeagues = computed(() => {
    return leagueStore.activeFantasyLeagues
})

const fantasyPlayersByTeam = (teamId: number) => {
    return fantasyPlayers.value.filter(player => player.teamId == teamId).sort((playerA: FantasyPlayer, playerB: FantasyPlayer) => {
        if (playerA.teamPosition < playerB.teamPosition) return -1;
        if (playerA.teamPosition > playerB.teamPosition) return 1;
        return 0;
    })
}

onMounted(() => {
    if (leagueStore.selectedFantasyLeague) {
        selectedFantasyLeague.value = leagueStore.selectedFantasyLeague;
    } else {
        localApiService.getLeagues().then((result: any) => {
            leagueStore.setLeagues(result);
            localApiService.getFantasyLeagues().then((result: any) => {
                leagueStore.setFantasyLeagues(result);
            })
        })
    }
    localApiService.getTeams().then((result: any) => {
        teams.value = result.sort((teamA: any, teamB: any) => {
            if (teamA.name < teamB.name) return -1;
            if (teamA.name > teamB.name) return 1;
            return 0;
        });
    })
    localApiService.getAccounts().then((result: any) => {
        players.value = result.sort((playerA: any, playerB: any) => {
            if (playerA.name < playerB.name) return -1;
            if (playerA.name > playerB.name) return 1;
            return 0;
        });
    })
})

watch(selectedFantasyLeague, () => {
    if (selectedFantasyLeague.value) {
        localApiService.getFantasyPlayers(selectedFantasyLeague.value.id).then((result: any) => {
            fantasyPlayers.value = result;
            setFantasyTeams()
        })
    }
});

function fullFantasyName(fantasyLeagueId: number, fantasyLeagueName: string) {
    const league = leagueStore.activeLeagues.find(l => l.league_id === fantasyLeagueId);
    return `${league?.name ?? ''} - ${fantasyLeagueName}`
}

const setFantasyTeams = () => {
    let teams = fantasyPlayers.value.map(item => ({ teamId: item.teamId, ...item.team }))
    fantasyTeams.value = [...new Map(teams.map(item => [item.teamId, item])).values()]
}

const addTeam = () => {
    fantasyTeams.value.unshift(selectedTeam.value);
}

const deleteTeam = (deleteTeam: DotaTeam) => {
    fantasyTeams.value = fantasyTeams.value.filter(team => team != deleteTeam);
}

const getImageUrl = (teamLogoId: number) => {
    if (teamLogoId == 0) return undefined;
    return `logos/teams_logo_${teamLogoId}.png`
}

const addFantasyPlayer = (teamId: number, accountId: number, teamPosition: number) => {
    dialogAddFantasyPlayer.value = true;
    addFantasyPlayerTeamId.value = teamId;
    addFantasyPlayerAccountId.value = accountId;
    addFantasyPlayerTeamPositionId.value = teamPosition;
}

const addFantasyPlayerConfirm = () => {
    if (selectedFantasyLeague.value) {
        let addFantasyPlayer: Partial<FantasyPlayer> = {
            fantasyLeagueId: selectedFantasyLeague.value!.id,
            teamId: addFantasyPlayerTeamId.value,
            dotaAccountId: addFantasyPlayerAccountId.value,
            teamPosition: addFantasyPlayerTeamPositionId.value
        }

        localApiService.postFantasyPlayer(addFantasyPlayer).then((result: any) => {
            localApiService.getFantasyPlayers(selectedFantasyLeague.value!.id).then((result: any) => {
                fantasyPlayers.value = result;
                setFantasyTeams()
            })
        })
    }

    closeAddFantasyPlayer();

}

const deleteFantasyPlayer = (deleteFantasyPlayer: FantasyPlayer) => {
    dialogDeleteFantasyPlayer.value = true;
    deleteFantasyPlayerItem.value = deleteFantasyPlayer;
}

const deleteFantasyPlayerConfirm = () => {
    if (deleteFantasyPlayerItem.value) {
        localApiService.deleteFantasyPlayer(deleteFantasyPlayerItem.value)!.then(() => {
            localApiService.getFantasyPlayers(selectedFantasyLeague.value!.id).then((result: any) => {
                fantasyPlayers.value = result;
                setFantasyTeams()
            })
        })
    }
    closeDeleteFantasyPlayer();
}

const closeAddFantasyPlayer = () => {
    dialogAddFantasyPlayer.value = false
}

const closeDeleteFantasyPlayer = () => {
    dialogDeleteFantasyPlayer.value = false
}

</script>