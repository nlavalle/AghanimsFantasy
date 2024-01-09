<template>
  <div class="flex-container">
    <div v-if="authenticated" style="width:100%">
      <div class="row">
        <div class="col col-2 draft-player">
          <select-expansion-group v-model="draftedPlayerOne" :select-options="fantasyTeamPlayers"
            select-label="Draft First Player" />
        </div>
        <div class="col col-2 draft-player">
          <select-expansion-group v-model="draftedPlayerTwo" :select-options="fantasyTeamPlayers"
            select-label="Draft Second Player" />
        </div>
        <div class="col col-2 draft-player">
          <select-expansion-group v-model="draftedPlayerThree" :select-options="fantasyTeamPlayers"
            select-label="Draft Third Player" />
        </div>
        <div class="col col-2 draft-player">
          <select-expansion-group v-model="draftedPlayerFour" :select-options="fantasyTeamPlayers"
            select-label="Draft Fourth Player" />
        </div>
        <div class="col col-2 draft-player">
          <select-expansion-group v-model="draftedPlayerFive" :select-options="fantasyTeamPlayers"
            select-label="Draft Fifth Player" />
        </div>
      </div>
      <div class="row">
        <q-space />
        <q-btn class="text-white" @click="saveDraft()">Save Draft</q-btn>
      </div>

      <AlertDialog v-model="showSuccessModal" />
    </div>
    <div v-else class="text-white">
      Not Authenticated
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

export default {
  name: 'FantasyPage',
  components: {
    SelectExpansionGroup,
    AlertDialog
  },
  setup() {
    const authStore = useAuthStore();
    const leagueStore = useLeagueStore();

    const fantasyPlayers = ref([]);
    const fantasyDraft = ref([]);
    const selectedFantasyPlayer = ref(null);
    const draftedPlayerOne = ref(null);
    const draftedPlayerTwo = ref(null);
    const draftedPlayerThree = ref(null);
    const draftedPlayerFour = ref(null);
    const draftedPlayerFive = ref(null);

    const showSuccessModal = ref(false);

    // Define a computed property to generate a grouped list of players per team
    const fantasyTeamPlayers = computed(() => {
      return Array.from(new Set(fantasyPlayers.value.map(opt => opt.team.name))).map(teamName => {
        return {
          label: teamName,
          options: fantasyPlayers.value
            .filter(opt => opt.team.name === teamName)
            .map(player => (
              {
                id: player.id,
                name: player.dotaAccount.name
              }
            )),
        };
      })
    });

    const fetchFantasyData = async () => {
      await authStore.checkAuthenticatedAsync();
      if (authStore.authenticated && leagueStore.selectedLeague) {
        localApiService.getFantasyPlayers(leagueStore.selectedLeague.id)
          .then((result) => {
            fantasyPlayers.value = result;
          });
        localApiService.getFantasyDraft(leagueStore.selectedLeague.id)
          .then((result) => {
            fantasyDraft.value = result;
            if (fantasyDraft.value.length > 0) {
              draftedPlayerOne.value = { id: fantasyDraft.value[0].players[0].id, name: fantasyDraft.value[0].players[0].dotaAccount.name }
              draftedPlayerTwo.value = { id: fantasyDraft.value[0].players[1].id, name: fantasyDraft.value[0].players[1].dotaAccount.name }
              draftedPlayerThree.value = { id: fantasyDraft.value[0].players[2].id, name: fantasyDraft.value[0].players[2].dotaAccount.name }
              draftedPlayerFour.value = { id: fantasyDraft.value[0].players[3].id, name: fantasyDraft.value[0].players[3].dotaAccount.name }
              draftedPlayerFive.value = { id: fantasyDraft.value[0].players[4].id, name: fantasyDraft.value[0].players[4].dotaAccount.name }
            }
          })
      }
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
      showSuccessModal.value = true;
    };

    onMounted(fetchFantasyData);

    watch(() => authStore.authenticated, (newValue) => {
      if (newValue) {
        fetchFantasyData();
      }
    });

    watch(() => leagueStore.selectedLeague, (newValue) => {
      if (newValue) {
        fetchFantasyData();
      }
    });

    return {
      authStore, fantasyPlayers, fantasyTeamPlayers, selectedFantasyPlayer,
      draftedPlayerOne, draftedPlayerTwo, draftedPlayerThree, draftedPlayerFour, draftedPlayerFive,
      showSuccessModal,
      fetchFantasyData, saveDraft
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

.draft-player {
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
</style>