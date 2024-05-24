<template>
  <v-container>
    <v-row v-if="authenticated" style="width:100%">
      <v-col>
        <v-row>
          <v-tabs v-model="fantasyTab">
            <v-tab value="current">Current Draft</v-tab>
            <v-tab value="draft">Draft Players</v-tab>
          </v-tabs>
        </v-row>
        <v-row>
          <v-tabs-window v-model="fantasyTab" style="width:100%;overflow: visible">
            <v-tabs-window-item value="current">
              <div v-if="userDraftPoints">
                <CurrentDraft :FantasyPoints="userDraftPoints" />
              </div>
            </v-tabs-window-item>
            <v-tabs-window-item value="draft" style="overflow: visible !important">
              <v-row v-if="updateDraftVisibility || updateDisabled">
                <v-spacer />
                <v-btn class="btn-fantasy" :disabled="updateDisabled" @click="toggleUpdateDraft()">
                  {{ updateDisabled ? "Draft Locked" : "Update Draft" }}
                </v-btn>
              </v-row>
              <v-row v-else>
                <v-col>
                  <v-row>
                    <CreateDraft @saveDraft="saveDraft" />
                  </v-row>
                </v-col>
              </v-row>
            </v-tabs-window-item>
          </v-tabs-window>
        </v-row>
      </v-col>
    </v-row>
    <v-row v-else class="row text-white">
      <span class="not-authenticated">
        Not Authenticated
      </span>
    </v-row>
  </v-container>

  <AlertDialog v-model="showSuccessModal" @ok="scrollAfterAlertDialog" />
  <ErrorDialog v-model="showErrorModal" :error="errorDetails" @ok="scrollAfterAlertDialog" />
</template>

<script setup lang="ts">
import { computed, onMounted, ref, watch } from 'vue';
import { VSpacer, VBtn, VContainer, VRow, VCol, VTabs, VTab, VTabsWindow, VTabsWindowItem } from 'vuetify/components';
import { useAuthStore } from '@/stores/auth';
import { useLeagueStore } from '@/stores/league';
import { localApiService } from '@/services/localApiService';
import CurrentDraft from '@/components/Fantasy/CurrentDraft.vue';
import CreateDraft from '@/components/Fantasy/CreateDraft/CreateDraft.vue';
import { fantasyDraftState, type FantasyDraftPoints, type FantasyPlayer } from '@/components/Fantasy/fantasyDraft';
import AlertDialog from '@/components/AlertDialog.vue'
import ErrorDialog from '@/components/ErrorDialog.vue';

const authStore = useAuthStore();
const leagueStore = useLeagueStore();
const { fantasyDraftPicks, setFantasyDraftPicks, setFantasyPlayers } = fantasyDraftState();

const showSuccessModal = ref(false);
const showErrorModal = ref(false);
const errorDetails = ref(null);

const fantasyTab = ref('current')
const updateDraftVisibility = ref(false);
const fantasyPlayers = ref<FantasyPlayer[]>([]);
const userDraftPoints = ref<FantasyDraftPoints>();

const updateDisabled = computed(() => {
  var currentDate = new Date();
  var draftLockEpoch: number = leagueStore.selectedLeague ? leagueStore.selectedLeague.fantasyDraftLocked : 0;
  var lockDate = new Date(draftLockEpoch * 1000);
  //return currentDate > lockDate && userDraftPoints.value != {}; // TODO: Rethink logic on people who draft late
  return currentDate > lockDate;
});

const fetchFantasyData = async () => {
  await authStore.checkAuthenticatedAsync();
  if (authStore.authenticated && leagueStore.selectedLeague) {
    localApiService.getFantasyPlayers(leagueStore.selectedLeague.id)
      .then((result) => {
        fantasyPlayers.value = result;
        setFantasyPlayers(fantasyPlayers.value);
      });
    if (userDraftPoints.value?.fantasyDraft.draftPickPlayers && userDraftPoints.value.fantasyDraft.draftPickPlayers.length > 0) {
      setFantasyDraftPicks(userDraftPoints.value.fantasyDraft.draftPickPlayers);
    }
  }
}

const saveDraft = async () => {
  await localApiService.saveFantasyDraft(
    authStore.user,
    leagueStore.selectedLeague,
    fantasyDraftPicks.value
  )
    .then(() => {
      showSuccessModal.value = true;
      window.scrollTo({
        top: 0,
        left: 0,
        behavior: 'smooth'
      });
      localApiService.getUserDraftPoints(leagueStore.selectedLeague!.id).then(result => {
        userDraftPoints.value = result;
        fetchFantasyData();
      });
      fantasyTab.value = 'current';
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

onMounted(() => {
  if (authStore.authenticated && leagueStore.selectedLeague) {
    localApiService.getUserDraftPoints(leagueStore.selectedLeague.id).then(result => {
      userDraftPoints.value = result;
      fetchFantasyData();
    });
  }

});

watch([authStore, leagueStore], () => {
  if (authStore.authenticated && leagueStore.selectedLeague) {
    localApiService.getUserDraftPoints(leagueStore.selectedLeague.id).then(result => {
      userDraftPoints.value = result;
      fetchFantasyData();
    });
  }
});

const authenticated = computed(() => {
  return authStore.authenticated
})

</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.not-authenticated {
  margin: 20px;
  font-size: 16px;
}
</style>