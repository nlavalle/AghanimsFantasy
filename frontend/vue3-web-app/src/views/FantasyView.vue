<template>
  <v-progress-circular style="position:fixed;top:50%;left:50%;" v-if="!isMounted" color="primary"
    indeterminate></v-progress-circular>
  <v-container v-if="isMounted">
    <v-row style="width:100%">
      <v-col>
        <v-row class="align-center">
          <v-tabs v-model="fantasyTab" center-active show-arrows>
            <v-tab value="current">Current Draft</v-tab>
            <v-tab value="draft">Draft Players</v-tab>
            <v-tab value="leaderboard">Leaderboard</v-tab>
            <v-tab value="match">Fantasy Matches</v-tab>
          </v-tabs>
          <v-spacer />
          <UserBalance />
        </v-row>
        <v-row v-if="fantasyLeagueStore.selectedFantasyLeague && !updateDisabled">
          <fantasy-lock-timer class="ma-3" :target-time="fantasyLeagueStore.selectedFantasyLeague.fantasyDraftLocked" />
        </v-row>
        <v-row>
          <v-tabs-window v-model="fantasyTab" style="width:100%;overflow: visible">
            <v-tabs-window-item value="current">
              <div v-if="fantasyLeagueStore.selectedFantasyDraftPoints">
                <CurrentDraft :FantasyPoints="fantasyLeagueStore.selectedFantasyDraftPoints" />
              </div>
            </v-tabs-window-item>
            <v-tabs-window-item value="draft" style="overflow: visible !important">
              <v-col>
                <v-row v-if="!authStore.isAuthenticated">
                  <v-card class="ma-5">
                    <v-card-title style="text-wrap:wrap">
                      <v-row>
                        <v-col>
                          <span class="not-authenticated">Please login to save your draft</span>
                        </v-col>
                        <v-col cols="4" class="mr-1" align-self="center">
                          <LoginModal class="login-modal" />
                        </v-col>
                      </v-row>
                    </v-card-title>
                  </v-card>
                </v-row>
                <v-row v-if="updateDraftVisibility || updateDisabled">
                  <v-card class="ma-5" disabled>
                    <v-card-title style="text-wrap:wrap">
                      {{ `Drafting for Fantasy League: ${fantasyLeagueStore.selectedLeague!.name} is locked` }}
                    </v-card-title>
                  </v-card>
                </v-row>
                <v-row>
                  <v-col>
                    <v-row v-if="fantasyLeagueStore.selectedFantasyLeague">
                      <CreateDraft @saveDraft="saveDraft" />
                    </v-row>
                  </v-col>
                </v-row>
              </v-col>
            </v-tabs-window-item>
            <v-tabs-window-item value="leaderboard">
              <v-col v-if="authStore.isAuthenticated">
                <v-row class="mt-1">
                  <leaderboard-component class="leaderboardComponent" leaderboardTitle="Fantasy Leaderboard"
                    headerName="Draft Player" headerValue="Points" :authenticatedUser="authStore.currentUser"
                    :boardData="fantasyDraftStore.fantasyLeaderboardData" />
                </v-row>
                <v-row class="ma-1" style="max-width: 600px">
                  <v-col>
                    <p>Total Drafts: {{ fantasyDraftStore.fantasyLeaderboardStats?.totalDrafts ?? 0 }}</p>
                  </v-col>
                  <v-col>
                    <p>You're in the {{ fantasyDraftStore.fantasyLeaderboardStats?.drafterPercentile?.toFixed(0) ?? 0
                    }}th percentile
                    </p>
                  </v-col>
                </v-row>
              </v-col>
              <v-col v-else>
                <v-card class="ma-5">
                  <v-card-title style="text-wrap:wrap">
                    <v-row>
                      <v-col>
                        <span class="not-authenticated">Please login to view the leaderboard</span>
                      </v-col>
                      <v-col cols="4" class="mr-1" align-self="center">
                        <LoginModal class="login-modal" />
                      </v-col>
                    </v-row>
                  </v-card-title>
                </v-card>
              </v-col>
            </v-tabs-window-item>
            <v-tabs-window-item value="match">
              <v-col>
                <v-row v-if="fantasyLeagueStore.selectedFantasyDraftPoints && fantasyTab == 'match'">
                  <MatchDataTable v-model:selectedFantasyLeague="fantasyLeagueStore.selectedFantasyLeague"
                    v-model:draftFiltered="draftFiltered">
                  </MatchDataTable>
                </v-row>
              </v-col>
            </v-tabs-window-item>
          </v-tabs-window>
        </v-row>
      </v-col>
    </v-row>
  </v-container>

  <AlertDialog v-model="showSuccessModal" @ok="scrollAfterAlertDialog" />
  <ErrorDialog v-model="showErrorModal" :error="errorDetails!" @ok="scrollAfterAlertDialog" />
</template>

<script setup lang="ts">
import { computed, onBeforeMount, ref, watch } from 'vue';
import { VCard, VCardTitle, VContainer, VRow, VCol, VTabs, VTab, VTabsWindow, VTabsWindowItem, VSpacer, VProgressCircular } from 'vuetify/components';
import { useAuthStore, type User } from '@/stores/auth';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';
import CurrentDraft from '@/components/Fantasy/CurrentDraft.vue';
import CreateDraft from '@/components/Fantasy/CreateDraft/CreateDraft.vue';
import MatchDataTable from '@/components/Stats/MatchDataTable.vue';
import { fantasyDraftState } from '@/components/Fantasy/fantasyDraft';
import LoginModal from '@/components/Auth/LoginModal.vue';
import AlertDialog from '@/components/AlertDialog.vue';
import ErrorDialog from '@/components/ErrorDialog.vue';
import { useFantasyDraftStore } from '@/stores/fantasyDraft';
import LeaderboardComponent from '@/components/Fantasy/LeaderboardComponent.vue'
import FantasyLockTimer from '@/components/Fantasy/FantasyLockTimer.vue';
import UserBalance from '@/components/Fantasy/UserBalance.vue';

const authStore = useAuthStore();
const fantasyLeagueStore = useFantasyLeagueStore();
const fantasyDraftStore = useFantasyDraftStore();
const { fantasyDraftPicks, setFantasyDraftPicks, setFantasyPlayerPoints, clearFantasyDraftPicks } = fantasyDraftState();
const draftFiltered = true;

const showSuccessModal = ref(false);
const showErrorModal = ref(false);
const errorDetails = ref<Error>();

const fantasyTab = ref('current')
const updateDraftVisibility = ref(false);

const isMounted = ref(false);

const updateDisabled = computed(() => {
  var currentDate = new Date();
  var draftLockEpoch: number = fantasyLeagueStore.selectedFantasyLeague ? fantasyLeagueStore.selectedFantasyLeague.fantasyDraftLocked : 0;
  var lockDate = new Date(draftLockEpoch * 1000);
  //return currentDate > lockDate && userDraftPoints.value != {}; // TODO: Rethink logic on people who draft late
  return currentDate > lockDate;
});

const saveDraft = async () => {
  if (!authStore.authenticated) return;
  fantasyDraftStore.saveFantasyDraft(fantasyDraftPicks.value)
    .then(() => {
      showSuccessModal.value = true;
      window.scrollTo({
        top: 0,
        left: 0,
        behavior: 'smooth'
      });
      fantasyLeagueStore.fetchFantasyDraftPoints()?.then(() => fantasyDraftStore.fetchLeaderboard());
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

const scrollAfterAlertDialog = () => {
  setTimeout(function () {
    window.scrollTo({
      top: 0,
      left: 0,
      behavior: 'smooth'
    });
  }, 200);
}

onBeforeMount(async () => {
  if (authStore.authenticated && fantasyLeagueStore.selectedFantasyLeague) {
    await fantasyDraftStore.fetchLeaderboard()
    await fantasyLeagueStore.fetchFantasyPlayerViewModels()
    await fantasyLeagueStore.fetchFantasyPlayerPoints()
    if (fantasyLeagueStore.selectedFantasyDraftPoints && (fantasyLeagueStore.selectedFantasyDraftPoints?.fantasyDraft.draftPickPlayers.length ?? 0 > 0)) {
      setFantasyDraftPicks(fantasyLeagueStore.selectedFantasyDraftPoints.fantasyDraft.draftPickPlayers);
    }
    isMounted.value = true;
  } else if (!authStore.authenticated && fantasyLeagueStore.selectedFantasyLeague) {
    fantasyTab.value = 'draft';
    isMounted.value = true;
  }
  fantasyDraftStore.startFantasyDraftPolling();
  fantasyLeagueStore.startFantasyViewPolling();
});

watch(() => fantasyLeagueStore.selectedFantasyLeague, () => {
  fantasyLeagueStore.fetchFantasyPlayerPoints()?.then(() => setFantasyPlayerPoints(fantasyLeagueStore.fantasyPlayerPoints))
    .then(() => {
      if (authStore.authenticated) fantasyDraftStore.fetchLeaderboard()
      fantasyLeagueStore.fetchFantasyPlayerViewModels();
    })
})

watch(() => fantasyLeagueStore.selectedFantasyDraftPoints, () => {
  if (authStore.authenticated && fantasyLeagueStore.selectedFantasyDraftPoints?.fantasyDraft && fantasyLeagueStore.selectedFantasyDraftPoints.fantasyDraft.draftPickPlayers.length > 0) {
    setFantasyDraftPicks(fantasyLeagueStore.selectedFantasyDraftPoints.fantasyDraft.draftPickPlayers);
  } else {
    clearFantasyDraftPicks()
  }
})

</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.not-authenticated {
  font-size: 16px;
}

.login-modal {
  cursor: pointer;
}

.leaderboardComponent {
  max-width: 600px;
}
</style>