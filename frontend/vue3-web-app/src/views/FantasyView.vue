<template>
  <div class="purple-rune">
    <div v-if="!isMounted" class="d-flex justify-center align-center" style="min-height: 200px;">
      <v-progress-circular color="primary" indeterminate />
    </div>
    <div v-else>
      <FantasyAlertBanner />
      <CreateDraftPicks class="draft-pick-bar" :canSave="canSave" :saveDisabledReason="saveDisabledReason"
        @clearDraft="clearFantasyDraftPicks" @saveDraft="saveDraft" />
      <PlayerPicksAvailable style="padding-right: 396px" />
      <PlayerStats class="player-stats-fixed" />

      <!-- <v-container v-if="isMounted" :style="fantasyTab === 'draft' ? 'padding-right: 396px' : ''"
      style="max-width: 100%">
      <v-row style="width:100%">
        <v-col>
          <v-row class="align-center">
            <v-tabs v-model="fantasyTab" center-active show-arrows>
              <v-tab value="current">Current Draft</v-tab>
              <v-tab value="draft">Draft Players</v-tab>
              <v-tab value="leaderboard">Leaderboard</v-tab>
              <v-tab value="match">Fantasy Matches</v-tab>
            </v-tabs>
            <v-btn icon="fa-refresh" @click="refreshFantasy" :loading="!loaded" :disabled="!loaded" size="small"
              variant="outlined" aria-label="Refresh fantasy data">
            </v-btn>
            <v-spacer />
            <UserBalance />
          </v-row>
          <v-row v-if="fantasyLeagueStore.currentFantasyLeague && !updateDisabled">
            <fantasy-lock-timer class="ma-3"
              :target-time="fantasyLeagueStore.currentFantasyLeague.fantasyDraftLocked" />
          </v-row>
          <v-row>
            <v-tabs-window v-model="fantasyTab" style="width:100%;overflow: visible" transition="fade-transition"
              reverse-transition="fade-transition">
              <v-tabs-window-item value="current">
                <div v-if="fantasyLeagueStore.selectedFantasyDraftPoints">
                  <CurrentDraft :FantasyPoints="fantasyLeagueStore.selectedFantasyDraftPoints" />
                </div>
              </v-tabs-window-item>
              <v-tabs-window-item value="draft" style="overflow: visible !important">
                <v-col>
                  <v-row v-if="!authStore.isAuthenticated" class="section-gap">
                    <v-card class="pa-4">
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
                  <v-row v-if="updateDraftVisibility || updateDisabled" class="section-gap">
                    <v-card class="pa-4" disabled>
                      <v-card-title style="text-wrap:wrap">
                        {{ `Drafting for Fantasy League: ${fantasyLeagueStore.selectedLeague!.name} is locked` }}
                      </v-card-title>
                    </v-card>
                  </v-row>
                  <v-row>
                    <v-col>
                      <v-row v-if="fantasyLeagueStore.currentFantasyLeague">
                        <CreateDraft @saveDraft="saveDraft" />
                      </v-row>
                    </v-col>
                  </v-row>
                </v-col>
              </v-tabs-window-item>
              <v-tabs-window-item value="leaderboard">
                <v-col>
                  <v-row class="mt-1">
                    <leaderboard-component class="leaderboardComponent" leaderboardTitle="Fantasy Leaderboard"
                      headerName="Draft Player" headerValue="Points" :authenticatedUser="authStore.currentUser"
                      :boardData="fantasyDraftStore.fantasyLeaderboardData" />
                  </v-row>
                  <v-row class="section-gap leaderboard-stats" v-if="authStore.isAuthenticated">
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
              </v-tabs-window-item>
              <v-tabs-window-item value="match">
                <v-col>
                  <v-row v-if="fantasyLeagueStore.selectedFantasyDraftPoints && fantasyTab == 'match'">
                    <MatchDataTable v-model:currentFantasyLeague="fantasyLeagueStore.currentFantasyLeague"
                      v-model:draftFiltered="draftFiltered">
                    </MatchDataTable>
                  </v-row>
                </v-col>
              </v-tabs-window-item>
            </v-tabs-window>
          </v-row>
        </v-col>
      </v-row>
    </v-container> -->

    </div>

    <AlertDialog v-model="showSuccessModal" @ok="scrollAfterAlertDialog" />
    <ErrorDialog v-model="showErrorModal" :error="errorDetails!" @ok="scrollAfterAlertDialog" />
  </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue';
import { VProgressCircular } from 'vuetify/components';
import { useAuthStore } from '@/stores/auth';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';
// import CurrentDraft from '@/components/Fantasy/CurrentDraft.vue';
import PlayerPicksAvailable from '@/components/Fantasy/Draft/PlayerPool/PlayerPicksAvailable.vue';
import CreateDraftPicks from '@/components/Fantasy/Draft/PickBar/CreateDraftPicks.vue';
// import MatchDataTable from '@/components/Stats/MatchDataTable.vue';
import { fantasyDraftState, DRAFT_BUDGET } from '@/components/Fantasy/fantasyDraft';
// import LoginModal from '@/components/Auth/LoginModal.vue';
import AlertDialog from '@/components/AlertDialog.vue';
import ErrorDialog from '@/components/ErrorDialog.vue';
import { useFantasyDraftStore } from '@/stores/fantasyDraft';
// import LeaderboardComponent from '@/components/Fantasy/Leaderboard/LeaderboardComponent.vue'
// import FantasyLockTimer from '@/components/Fantasy/FantasyLockTimer.vue';
// import UserBalance from '@/components/Fantasy/UserBalance.vue';
import FantasyAlertBanner from '@/components/Fantasy/FantasyAlertBanner.vue';
import PlayerStats from '@/components/Fantasy/Draft/PlayerPanel/PlayerStats.vue';

const authStore = useAuthStore();
const fantasyLeagueStore = useFantasyLeagueStore();
const fantasyDraftStore = useFantasyDraftStore();
const { fantasyDraftPicks, setFantasyDraftPicks, setFantasyPlayerPoints, clearFantasyDraftPicks, totalDraftCost } = fantasyDraftState();
// const draftFiltered = ref(true);

const showSuccessModal = ref(false);
const showErrorModal = ref(false);
const errorDetails = ref<Error>();

// const fantasyTab = ref('current')
// const updateDraftVisibility = ref(false);

const isMounted = ref(false);
// const loaded = ref(true);

// const viewMode = computed(() => fantasyLeagueStore.viewMode)

// const updateDisabled = computed(() => {
//   var currentDate = new Date();
//   var draftLockEpoch: number = fantasyLeagueStore.currentFantasyLeague ? fantasyLeagueStore.currentFantasyLeague.fantasyDraftLocked : 0;
//   var lockDate = new Date(draftLockEpoch * 1000);
//   //return currentDate > lockDate && userDraftPoints.value != {}; // TODO: Rethink logic on people who draft late
//   return currentDate > lockDate;
// });

const canSave = computed(() => {
  const totalGold = totalDraftCost(fantasyLeagueStore.fantasyPlayersStats);
  return !!(authStore.authenticated
    && fantasyLeagueStore.currentFantasyLeague
    && fantasyLeagueStore.isDraftOpen(fantasyLeagueStore.currentFantasyLeague)
    && totalGold <= DRAFT_BUDGET);
});

const saveDisabledReason = computed((): string | undefined => {
  if (!authStore.authenticated) return 'You must be logged in to save a draft'
  if (!fantasyLeagueStore.currentFantasyLeague) return 'No fantasy league selected'
  if (!fantasyLeagueStore.isDraftOpen(fantasyLeagueStore.currentFantasyLeague)) return 'Draft window is closed'
  if (totalDraftCost(fantasyLeagueStore.fantasyPlayersStats) > DRAFT_BUDGET) return 'Draft is over budget'
  return undefined
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
      // fantasyTab.value = 'current';
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

// const refreshFantasy = () => {
//   loaded.value = false
//   Promise.all([
//     fantasyLeagueStore.fetchFantasyPlayerViewModels(),
//     fantasyLeagueStore.fetchFantasyPlayerPoints(),
//     fantasyDraftStore.fetchLeaderboard()
//   ])
//     ?.then(() => {
//       loaded.value = true
//     })
//     .catch(() => {
//       loaded.value = true
//     })
// }

// Runs immediately on mount and re-runs if currentFantasyLeague becomes
// available after mount (cold load where leagues are fetched asynchronously).
watch(() => fantasyLeagueStore.currentFantasyLeague, async (fl) => {
  if (!fl) return

  if (!isMounted.value) {
    // Initial load: fetch everything needed to render the page
    if (authStore.authenticated) {
      // await fantasyDraftStore.fetchLeaderboard()
      await fantasyLeagueStore.fetchFantasyPlayerViewModels()
      await fantasyLeagueStore.fetchFantasyPlayerPoints()
      setFantasyPlayerPoints(fantasyLeagueStore.fantasyPlayerPoints)
      if (fantasyLeagueStore.selectedFantasyDraftPoints && (fantasyLeagueStore.selectedFantasyDraftPoints?.fantasyDraft.draftPickPlayers.length ?? 0 > 0)) {
        setFantasyDraftPicks(fantasyLeagueStore.selectedFantasyDraftPoints.fantasyDraft.draftPickPlayers);
      } else {
        // fantasyTab.value = 'draft';
      }
    } else {
      // await fantasyDraftStore.fetchLeaderboard()
      // fantasyTab.value = 'draft';
    }
    isMounted.value = true;
  } else {
    // League switch: refresh player data for the newly selected league
    fantasyLeagueStore.fetchFantasyPlayerPoints()?.then(() => setFantasyPlayerPoints(fantasyLeagueStore.fantasyPlayerPoints))
      .then(() => {
        // fantasyDraftStore.fetchLeaderboard()
        fantasyLeagueStore.fetchFantasyPlayerViewModels();
      })
  }
}, { immediate: true })

watch(() => fantasyLeagueStore.selectedFantasyDraftPoints, () => {
  if (authStore.authenticated && fantasyLeagueStore.selectedFantasyDraftPoints?.fantasyDraft && fantasyLeagueStore.selectedFantasyDraftPoints.fantasyDraft.draftPickPlayers.length > 0) {
    setFantasyDraftPicks(fantasyLeagueStore.selectedFantasyDraftPoints.fantasyDraft.draftPickPlayers);
  } else {
    clearFantasyDraftPicks()
  }
})

</script>

<style scoped>
.draft-pick-bar {
  position: sticky;
  top: 48px;
  z-index: 10;
}

.player-stats-fixed {
  position: fixed;
  top: 203px;
  right: 0;
  width: 380px;
  height: calc(100vh - 48px);
  z-index: 9;
  overflow-y: auto;
  box-shadow: -8px 0 32px rgba(0, 0, 0, 0.7), -2px 0 8px rgba(0, 0, 0, 0.5);
}

.not-authenticated {
  font-size: 16px;
}

.login-modal {
  cursor: pointer;
}

.leaderboardComponent {
  max-width: 600px;
}

.leaderboard-stats {
  max-width: 600px;
}
</style>
