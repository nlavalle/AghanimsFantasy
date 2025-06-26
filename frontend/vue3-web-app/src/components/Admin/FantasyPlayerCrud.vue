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
                            <v-col>
                                <v-btn @click="calculateFantasyPlayerCosts()">Recalc Costs</v-btn>
                            </v-col>
                        </v-row>
                        <v-row>
                            <v-autocomplete v-model="selectedTeam" :items="teams" item-title="name" max-width="300"
                                return-object />
                        </v-row>
                        <v-row class="available-team ma-2 pa-2" v-for="(team, teamIndex) in fantasyTeams"
                            :key="teamIndex" :style="{ 'min-width': !display.mobile.value ? '800px' : '400px' }">
                            <v-col><v-row class="available-team-title">
                                    <v-col>
                                        <img :src="getImageUrl(team.id)" />
                                        <span>{{ team.name }}</span>
                                    </v-col>
                                    <v-col>
                                        <v-btn @click="deleteFantasyTeam(team)">Delete Team</v-btn>
                                    </v-col>
                                </v-row>
                                <v-row :style="{ 'flex-wrap': 'nowrap' }">
                                    <v-col class="ma-1" v-for="(player, playerIndex) in fantasyPlayersByTeam(team.id)"
                                        :key="playerIndex">
                                        <v-row>
                                            <v-col class="available-player ma-1 pa-0"
                                                :style="{ 'min-width': !display.mobile.value ? '110px' : '60px', 'max-width': !display.mobile.value ? '110px' : '60px' }">
                                                <draft-pick-card size="small" :fantasyPlayer="player" :fantasyPoints="0"
                                                    :fantasyLeagueActive="false"
                                                    :fantasyPlayerCost="fantasyPlayerCost(player.id)"
                                                    :fantasyPlayerBudget="600" />
                                            </v-col>
                                        </v-row>
                                        <v-row>
                                            <span>Pos: {{ player.teamPosition }}</span>
                                        </v-row>
                                        <v-row>
                                            <span>Sub: {{ player.substitution }}</span>
                                        </v-row>
                                        <v-row>
                                            <font-awesome-icon :icon="faPencil" class="me-2"
                                                @click="editFantasyPlayer(player)" />
                                            <font-awesome-icon :icon="faDeleteLeft" class="me-2"
                                                @click="deleteFantasyPlayer(player)" />
                                        </v-row>
                                    </v-col>
                                    <v-col>
                                        <v-autocomplete v-model="selectedPlayer" :items="players" item-title="name"
                                            max-width="300" return-object />
                                        <v-text-field v-model="selectedPlayerPosition" label="Position"></v-text-field>
                                        <v-checkbox v-model="selectedPlayerSub" label="Sub?"></v-checkbox>
                                        <v-btn
                                            @click="addFantasyPlayer(team.id, selectedPlayer?.id!, selectedPlayerPosition, selectedPlayerSub)">Add
                                            Fantasy Player</v-btn>
                                    </v-col>
                                </v-row>
                            </v-col>
                        </v-row>
                    </v-col>
                </v-row>
            </v-col>
        </v-row>
        <v-dialog v-model="dialogAddFantasyPlayer" max-width="500px">
            <v-card>
                <v-card-title class="text-h7">Confirm add Player {{ addFantasyPlayerAccountId }} ?</v-card-title>
                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="blue-darken-1" variant="text" @click="closeAddFantasyPlayer">Cancel</v-btn>
                    <v-btn color="blue-darken-1" variant="text" @click="addFantasyPlayerConfirm">OK</v-btn>
                    <v-spacer></v-spacer>
                </v-card-actions>
            </v-card>
        </v-dialog>
        <v-dialog v-model="dialogAddFantasyTeam" max-width="500px">
            <v-card>
                <v-card-title class="text-h7">Confirm add Team {{ selectedTeam?.name }} ?</v-card-title>
                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="blue-darken-1" variant="text" @click="closeAddFantasyTeam">Cancel</v-btn>
                    <v-btn color="blue-darken-1" variant="text" @click="addFantasyTeamConfirm">OK</v-btn>
                    <v-spacer></v-spacer>
                </v-card-actions>
            </v-card>
        </v-dialog>
        <v-dialog v-model="dialogDeleteFantasyPlayer" max-width="500px">
            <v-card>
                <v-card-title class="text-h7">Confirm delete Player {{ deleteFantasyPlayerItem?.dotaAccount.name ?? ''
                    }}?</v-card-title>
                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="blue-darken-1" variant="text" @click="closeDeleteFantasyPlayer">Cancel</v-btn>
                    <v-btn color="blue-darken-1" variant="text" @click="deleteFantasyPlayerConfirm">OK</v-btn>
                    <v-spacer></v-spacer>
                </v-card-actions>
            </v-card>
        </v-dialog>
        <v-dialog v-model="dialogDeleteFantasyTeam" max-width="500px">
            <v-card>
                <v-card-title class="text-h7">Confirm delete Team {{
                    deleteFantasyTeamItem?.name ?? ''
                }}?</v-card-title>
                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="blue-darken-1" variant="text" @click="closeDeleteFantasyTeam">Cancel</v-btn>
                    <v-btn color="blue-darken-1" variant="text" @click="deleteFantasyTeamConfirm">OK</v-btn>
                    <v-spacer></v-spacer>
                </v-card-actions>
            </v-card>
        </v-dialog>
        <v-dialog v-model="dialogEditFantasyPlayer" max-width="500px">
            <v-card>
                <v-card-title class="text-h7">Confirm edit player {{
                    editFantasyPlayerItem?.dotaAccount.name ?? ''
                }} - {{
                        editFantasyPlayerItem?.dotaAccount.id ?? ''
                    }}?</v-card-title>
                <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-autocomplete v-model="editSelectedPlayer" :items="players" item-title="name" max-width="300"
                        return-object />
                    <v-btn color="blue-darken-1" variant="text" @click="editFantasyPlayerConfirm">OK</v-btn>
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
import { localApiAdminService } from '@/services/localApiAdminService';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';
import type { FantasyLeague } from '@/types/FantasyLeague';
import type { FantasyPlayer } from '../Fantasy/fantasyDraft';
import type { DotaAccount, DotaTeam } from '@/types/Dota';
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import { faDeleteLeft, faPencil } from '@fortawesome/free-solid-svg-icons';
import { useDisplay } from 'vuetify';
import DraftPickCard from '../Fantasy/DraftPickCard.vue';

const display = useDisplay()

const leagueStore = useFantasyLeagueStore();
const selectedFantasyLeague = ref<FantasyLeague>();
const fantasyPlayers = ref<FantasyPlayer[]>([]);
const fantasyTeams = ref<any[]>([]);
const teams = ref<DotaTeam[]>([]);
const selectedTeam = ref<DotaTeam>();
const players = ref<DotaAccount[]>([]);
const selectedPlayer = ref<DotaAccount>();
const selectedPlayerPosition = ref<number>();
const selectedPlayerSub = ref<boolean>();

//AddPlayer
const dialogAddFantasyPlayer = ref(false);
const addFantasyPlayerTeamId = ref();
const addFantasyPlayerAccountId = ref();
const addFantasyPlayerTeamPositionId = ref();
const addFantasyPlayerSubstitute = ref(false);

//DeletePlayer
const dialogDeleteFantasyPlayer = ref(false);
const deleteFantasyPlayerItem = ref<FantasyPlayer>();

//EditPlayer
const dialogEditFantasyPlayer = ref(false);
const editFantasyPlayerItem = ref<FantasyPlayer>();
const editSelectedPlayer = ref<DotaAccount>();

//AddTeam
const dialogAddFantasyTeam = ref(false);

//DeleteTeam
const dialogDeleteFantasyTeam = ref(false);
const deleteFantasyTeamItem = ref<DotaTeam>();

const availableFantasyLeagues = computed(() => {
    return leagueStore.allFantasyLeagues
})

const fantasyPlayersByTeam = (teamId: number) => {
    return fantasyPlayers.value.filter(player => player.teamId == teamId).sort((playerA: FantasyPlayer, playerB: FantasyPlayer) => {
        if (playerA.teamPosition < playerB.teamPosition) return -1;
        if (playerA.teamPosition > playerB.teamPosition) return 1;
        if (!playerA.substitution && playerB.substitution) return -1;
        if (playerA.substitution && !playerB.substitution) return 1;
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
        leagueStore.setSelectedFantasyLeague(selectedFantasyLeague.value)
        localApiService.getFantasyPlayers(selectedFantasyLeague.value.id).then((result: any) => {
            fantasyPlayers.value = result;
            setFantasyTeams()
            leagueStore.fetchFantasyPlayerViewModels()
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

const getImageUrl = (teamLogoId: number) => {
    if (teamLogoId == 0) return undefined;
    return `logos/teams_logo_${teamLogoId}.png`
}

const fantasyPlayerCost = (fantasyPlayerId: number) => {
    return leagueStore.fantasyPlayersStats.find(fps => fps.fantasy_player.id == fantasyPlayerId)?.cost ?? 0
}

const addTeam = () => {
    dialogAddFantasyTeam.value = true;
}

const calculateFantasyPlayerCosts = () => {
    if (selectedFantasyLeague.value) {
        localApiAdminService.calculateFantasyPlayerCosts(selectedFantasyLeague.value.id)?.then(() => {
            leagueStore.fetchFantasyPlayerViewModels()
        });
    }
}

const addFantasyTeamConfirm = () => {
    if (selectedTeam.value && selectedFantasyLeague.value) {
        localApiAdminService.addFantasyPlayersByTeam(selectedTeam.value.id, selectedFantasyLeague.value.id)?.then(() => {
            localApiService.getFantasyPlayers(selectedFantasyLeague.value!.id).then((result: any) => {
                fantasyPlayers.value = result;
                setFantasyTeams()
                // If team had nothing to add, manually add an empty team so we can add initial fantasy players
                if (fantasyTeams.value.filter(t => t.id == selectedTeam!.value!.id).length == 0) {
                    fantasyTeams.value.unshift(selectedTeam.value);
                }
            })
        });
    }
    closeAddFantasyTeam();
}

const addFantasyPlayer = (teamId: number, accountId: number, teamPosition: number | undefined, isSub: boolean | undefined) => {
    if(!teamPosition) return
    dialogAddFantasyPlayer.value = true;
    addFantasyPlayerTeamId.value = teamId;
    addFantasyPlayerAccountId.value = accountId;
    addFantasyPlayerTeamPositionId.value = teamPosition;
    addFantasyPlayerSubstitute.value = isSub ?? false;
}

const addFantasyPlayerConfirm = () => {
    if (selectedFantasyLeague.value) {
        let addFantasyPlayer: Partial<FantasyPlayer> = {
            fantasyLeagueId: selectedFantasyLeague.value!.id,
            teamId: addFantasyPlayerTeamId.value,
            dotaAccountId: addFantasyPlayerAccountId.value,
            teamPosition: addFantasyPlayerTeamPositionId.value,
            substitution: addFantasyPlayerSubstitute.value
        }

        localApiService.postFantasyPlayer(addFantasyPlayer).then(() => {
            localApiService.getFantasyPlayers(selectedFantasyLeague.value!.id).then((result: any) => {
                fantasyPlayers.value = result;
                setFantasyTeams()
            })
        })
    }

    closeAddFantasyPlayer();

}

const editFantasyPlayer = (editFantasyPlayer: FantasyPlayer) => {
    dialogEditFantasyPlayer.value = true;
    editFantasyPlayerItem.value = editFantasyPlayer;
}

const editFantasyPlayerConfirm = () => {
    if (editFantasyPlayerItem.value && editSelectedPlayer.value) {
        let updatedFantasyPlayer = editFantasyPlayerItem.value;
        updatedFantasyPlayer.dotaAccountId = editSelectedPlayer.value.id;
        localApiService.putFantasyPlayer(updatedFantasyPlayer)!.then(() => {
            localApiService.getFantasyPlayers(selectedFantasyLeague.value!.id).then((result: any) => {
                fantasyPlayers.value = result;
                setFantasyTeams()
            })
        })
    }
    closeEditFantasyPlayer();
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

const deleteFantasyTeam = (deleteFantasyTeam: DotaTeam) => {
    dialogDeleteFantasyTeam.value = true;
    deleteFantasyTeamItem.value = deleteFantasyTeam;
}

const deleteFantasyTeamConfirm = () => {
    if (deleteFantasyTeamItem.value) {
        let deleteFantasyPlayers = fantasyPlayersByTeam(deleteFantasyTeamItem.value.id);
        deleteFantasyPlayers.forEach((fantasyPlayer) => {
            localApiService.deleteFantasyPlayer(fantasyPlayer);
        })
        localApiService.getFantasyPlayers(selectedFantasyLeague.value!.id).then((result: any) => {
            fantasyPlayers.value = result;
            setFantasyTeams();
        })
    }
    closeDeleteFantasyTeam();
}

const closeAddFantasyPlayer = () => {
    dialogAddFantasyPlayer.value = false
}

const closeDeleteFantasyPlayer = () => {
    dialogDeleteFantasyPlayer.value = false
}

const closeEditFantasyPlayer = () => {
    dialogEditFantasyPlayer.value = false
}

const closeAddFantasyTeam = () => {
    dialogAddFantasyTeam.value = false
}

const closeDeleteFantasyTeam = () => {
    dialogDeleteFantasyTeam.value = false
}

</script>

<style scoped>
.available-team {
    flex-wrap: nowrap;
}

.available-team-title {
    margin-right: 5px;
    margin-top: 5px;
}

.available-team-title img {
    height: 40px;
    margin-right: 10px;
}
</style>