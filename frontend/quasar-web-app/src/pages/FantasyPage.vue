<template>
  <div class="flex-container">
    <div v-if="authenticated" style="width:100%">
      <div v-if="userDraftPoints">
        <current-draft :FantasyPoints="userDraftPoints" />
      </div>
      <div v-if="updateDraftVisibility || updateDisabled" class="row">
        <q-space />
        <q-btn class="btn-fantasy" :disabled="updateDisabled" @click="toggleUpdateDraft()">
          {{ updateDisabled ? "Draft Locked" : "Update Draft" }}
        </q-btn>
      </div>
      <div v-else class="row">
        <div class="row">
          <div class="col col-2 draft-player">
            <select-expansion-group v-model="draftedPlayerOne" :select-options="fantasyTeamPlayers"
              @update:model-value="updateSelectedPlayers" select-label="Draft First Player" />
          </div>
          <div class="col col-2 draft-player">
            <select-expansion-group v-model="draftedPlayerTwo" :select-options="fantasyTeamPlayers"
              @update:model-value="updateSelectedPlayers" select-label="Draft Second Player" />
          </div>
          <div class="col col-2 draft-player">
            <select-expansion-group v-model="draftedPlayerThree" :select-options="fantasyTeamPlayers"
              @update:model-value="updateSelectedPlayers" select-label="Draft Third Player" />
          </div>
          <div class="col col-2 draft-player">
            <select-expansion-group v-model="draftedPlayerFour" :select-options="fantasyTeamPlayers"
              @update:model-value="updateSelectedPlayers" select-label="Draft Fourth Player" />
          </div>
          <div class="col col-2 draft-player">
            <select-expansion-group v-model="draftedPlayerFive" :select-options="fantasyTeamPlayers"
              @update:model-value="updateSelectedPlayers" select-label="Draft Fifth Player" />
          </div>
        </div>
        <div class="row">
          <q-space />
          <q-btn class="btn-fantasy" @click="toggleUpdateDraft()">Cancel</q-btn>
          <q-btn class="btn-fantasy" @click="saveDraft()">Save Draft</q-btn>
        </div>
      </div>
      <AlertDialog v-model="showSuccessModal" @ok="scrollAfterAlertDialog" />
      <ErrorDialog v-model="showErrorModal" :error="errorDetails" @ok="scrollAfterAlertDialog" />
    </div>
    <div v-else class="row text-white">
      <span class="not-authenticated">
        Not Authenticated
      </span>
    </div>
  </div>
</template>
  
<script>
import { computed, onMounted, ref, watch } from 'vue';
import { useAuthStore } from 'stores/auth';
import { useLeagueStore } from 'src/stores/league';
import { localApiService } from 'src/services/localApiService';
import SelectExpansionGroup from 'src/components/SelectExpansionGroup.vue';
import AlertDialog from 'src/components/AlertDialog.vue'
import ErrorDialog from 'src/components/ErrorDialog.vue';
import CurrentDraft from 'src/components/fantasy/CurrentDraft.vue'

export default {
  name: 'FantasyPage',
  components: {
    SelectExpansionGroup,
    AlertDialog,
    ErrorDialog,
    CurrentDraft
  },
  setup() {
    const authStore = useAuthStore();
    const leagueStore = useLeagueStore();

    const updateDraftVisibility = ref(false);
    const fantasyPlayers = ref([]);
    const fantasyDraft = ref([]);
    const userDraftPoints = ref({});
    const selectedFantasyPlayer = ref(null);
    const selectedPlayerIds = ref([]);
    const draftedPlayerOne = ref(null);
    const draftedPlayerTwo = ref(null);
    const draftedPlayerThree = ref(null);
    const draftedPlayerFour = ref(null);
    const draftedPlayerFive = ref(null);

    const showSuccessModal = ref(false);
    const showErrorModal = ref(false);
    const errorDetails = ref(null);

    // Define a computed property to generate a grouped list of players per team
    const fantasyTeamPlayers = computed(() => {
      return Array.from(new Set(fantasyPlayers.value.map(opt => opt.team.name))).map(teamName => {
        return {
          label: teamName,
          options: fantasyPlayers.value
            .filter(opt => opt.team.name === teamName) // Filter team
            .filter(opt => !selectedPlayerIds.value.some((sel) => sel == opt.id)) // Filter selected players
            .map(player => (
              {
                id: player.id,
                name: player.dotaAccount.name
              }
            )),
        };
      })
    });

    const updateDisabled = computed(() => {
      var currentDate = new Date();
      var lockDate = new Date(leagueStore.selectedLeague?.fantasyDraftLocked * 1000 ?? new Date());
      return currentDate > lockDate;
    });

    const updateSelectedPlayers = () => {
      selectedPlayerIds.value[0] = draftedPlayerOne.value?.id ?? 0;
      selectedPlayerIds.value[1] = draftedPlayerTwo.value?.id ?? 0;
      selectedPlayerIds.value[2] = draftedPlayerThree.value?.id ?? 0;
      selectedPlayerIds.value[3] = draftedPlayerFour.value?.id ?? 0;
      selectedPlayerIds.value[4] = draftedPlayerFive.value?.id ?? 0;
    };

    const clearSelectedPlayers = () => {
      selectedPlayerIds.value[0] = 0;
      selectedPlayerIds.value[1] = 0;
      selectedPlayerIds.value[2] = 0;
      selectedPlayerIds.value[3] = 0;
      selectedPlayerIds.value[4] = 0;
      draftedPlayerOne.value = null;
      draftedPlayerTwo.value = null;
      draftedPlayerThree.value = null;
      draftedPlayerFour.value = null;
      draftedPlayerFive.value = null;
    }

    const fetchFantasyData = async () => {
      await authStore.checkAuthenticatedAsync();
      if (authStore.authenticated && leagueStore.selectedLeague) {
        localApiService.getFantasyPlayers(leagueStore.selectedLeague.id)
          .then((result) => {
            fantasyPlayers.value = result;
          });
        if (userDraftPoints.value.length > 0) {
          draftedPlayerOne.value = {
            id: userDraftPoints.value[0].fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 1)[0]?.fantasyPlayer?.id,
            name: userDraftPoints.value[0].fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 1)[0]?.fantasyPlayer?.dotaAccount.name,
            steamProfilePicture: userDraftPoints.value[0].fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 1)[0]?.fantasyPlayer?.dotaAccount.steamProfilePicture,
          }
          draftedPlayerTwo.value = {
            id: userDraftPoints.value[0].fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 2)[0]?.fantasyPlayer?.id,
            name: userDraftPoints.value[0].fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 2)[0]?.fantasyPlayer?.dotaAccount.name,
            steamProfilePicture: userDraftPoints.value[0].fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 2)[0]?.fantasyPlayer?.dotaAccount.steamProfilePicture,
          }
          draftedPlayerThree.value = {
            id: userDraftPoints.value[0].fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 3)[0]?.fantasyPlayer?.id,
            name: userDraftPoints.value[0].fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 3)[0]?.fantasyPlayer?.dotaAccount.name,
            steamProfilePicture: userDraftPoints.value[0].fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 3)[0]?.fantasyPlayer?.dotaAccount.steamProfilePicture,
          }
          draftedPlayerFour.value = {
            id: userDraftPoints.value[0].fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 4)[0]?.fantasyPlayer?.id,
            name: userDraftPoints.value[0].fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 4)[0]?.fantasyPlayer?.dotaAccount.name,
            steamProfilePicture: userDraftPoints.value[0].fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 4)[0]?.fantasyPlayer?.dotaAccount.steamProfilePicture,
          }
          draftedPlayerFive.value = {
            id: userDraftPoints.value[0].fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 5)[0]?.fantasyPlayer?.id,
            name: userDraftPoints.value[0].fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 5)[0]?.fantasyPlayer?.dotaAccount.name,
            steamProfilePicture: userDraftPoints.value[0].fantasyDraft.draftPickPlayers.filter(dpp => dpp.draftOrder == 5)[0]?.fantasyPlayer?.dotaAccount.steamProfilePicture,
          }
        }
      }
    }

    const toggleUpdateDraft = () => {
      updateDraftVisibility.value = !updateDraftVisibility.value
    };

    const scrollAfterAlertDialog = () => {
      setTimeout(function () {
        window.scrollTo({
          top: 0,
          left: 0,
          behavior: 'smooth'
        });
      }, 200);
    }

    const saveDraft = async () => {
      await localApiService.saveFantasyDraft(
        authStore.user,
        leagueStore.selectedLeague,
        [
          draftedPlayerOne.value,
          draftedPlayerTwo.value,
          draftedPlayerThree.value,
          draftedPlayerFour.value,
          draftedPlayerFive.value
        ]
      )
        .then(() => {
          showSuccessModal.value = true;
          window.scrollTo({
            top: 0,
            left: 0,
            behavior: 'smooth'
          });
          localApiService.getUserDraftPoints(leagueStore.selectedLeague.id).then(result => {
            userDraftPoints.value = result;
            fetchFantasyData();
          });
        })
        .catch(error => {
          errorDetails.value = error;
          showErrorModal.value = true;
          window.scrollTo({
            top: 0,
            left: 0,
            behavior: 'smooth'
          });
        })


    };

    onMounted(() => {
      if (authStore.authenticated && leagueStore.selectedLeague) {
        localApiService.getUserDraftPoints(leagueStore.selectedLeague.id).then(result => {
          userDraftPoints.value = result;
          fetchFantasyData();
        });
      }

    });

    watch(() => authStore.authenticated, (newValue) => {
      if (newValue) {
        if (authStore.authenticated && leagueStore.selectedLeague) {
          localApiService.getUserDraftPoints(leagueStore.selectedLeague.id).then(result => {
            userDraftPoints.value = result;
            fetchFantasyData();
          });
        }
      }
    });

    watch(() => leagueStore.selectedLeague, (newValue) => {
      if (newValue) {
        clearSelectedPlayers(); // We don't want to persist any players in dropdowns
        if (authStore.authenticated && leagueStore.selectedLeague) {
          localApiService.getUserDraftPoints(leagueStore.selectedLeague.id).then(result => {
            userDraftPoints.value = result;
            fetchFantasyData();
          });
        }
      }
    });

    return {
      authStore, fantasyDraft, fantasyPlayers, fantasyTeamPlayers, selectedFantasyPlayer, selectedPlayerIds,
      draftedPlayerOne, draftedPlayerTwo, draftedPlayerThree, draftedPlayerFour, draftedPlayerFive, userDraftPoints,
      showSuccessModal, showErrorModal, errorDetails, updateDraftVisibility, updateDisabled,
      fetchFantasyData, saveDraft, toggleUpdateDraft, updateSelectedPlayers, clearSelectedPlayers, scrollAfterAlertDialog
    };
  },
  computed: {
    authenticated() { return this.authStore.authenticated; },
    user() { return this.authStore.user ?? {}; }
  },
}
</script>
  
  <!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.debug {
  border: 1px solid red;
  padding: 10px;
}

.not-authenticated {
  margin: 20px;
  font-size: 16px;
}

.draft-player {
  min-width: 250px;
  padding: 10px;
}

.btn-fantasy {
  color: var(--nadcl-white);
  background-color: var(--nadcl-main-2);
  margin: 10px;
}

.left-fixed {
  flex: 0 0 300px;
}

.flex-container {
  display: flex;
  flex-flow: row wrap;
  max-width: 100%;
  padding-left: 20px;
  padding-right: 20px;
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
</style>