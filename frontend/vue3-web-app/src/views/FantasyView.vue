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
import PlayerPicksAvailable from '@/components/Fantasy/Draft/PlayerPool/PlayerPicksAvailable.vue';
import CreateDraftPicks from '@/components/Fantasy/Draft/PickBar/CreateDraftPicks.vue';
import { fantasyDraftState, DRAFT_BUDGET } from '@/components/Fantasy/fantasyDraft';
import AlertDialog from '@/components/AlertDialog.vue';
import ErrorDialog from '@/components/ErrorDialog.vue';
import { useFantasyDraftStore } from '@/stores/fantasyDraft';
import FantasyAlertBanner from '@/components/Fantasy/FantasyAlertBanner.vue';
import PlayerStats from '@/components/Fantasy/Draft/PlayerPanel/PlayerStats.vue';

const authStore = useAuthStore();
const fantasyLeagueStore = useFantasyLeagueStore();
const fantasyDraftStore = useFantasyDraftStore();
const { fantasyDraftPicks, setFantasyDraftPicks, setFantasyPlayerPoints, clearFantasyDraftPicks, totalDraftCost } = fantasyDraftState();

const showSuccessModal = ref(false);
const showErrorModal = ref(false);
const errorDetails = ref<Error>();

const isMounted = ref(false);

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
      fantasyLeagueStore.fetchFantasyDraftPoints();
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

// Runs immediately on mount and re-runs if currentFantasyLeague becomes
// available after mount (cold load where leagues are fetched asynchronously).
watch(() => fantasyLeagueStore.currentFantasyLeague, async (fl) => {
  if (!fl.id) return

  const fetches = [
    fantasyLeagueStore.fetchFantasyPlayerViewModels(),
    fantasyLeagueStore.fetchFantasyPlayerPoints(),
    ...(authStore.authenticated ? [fantasyLeagueStore.fetchFantasyDraftPoints()] : [])
  ]

  await Promise.all(fetches).then(() => {
    setFantasyPlayerPoints(fantasyLeagueStore.fantasyPlayerPoints)

    if (authStore.authenticated) {
      // await fantasyDraftStore.fetchLeaderboard()
      if (fantasyLeagueStore.selectedFantasyDraftPoints && (fantasyLeagueStore.selectedFantasyDraftPoints?.fantasyDraft.draftPickPlayers.length ?? 0 > 0)) {
        setFantasyDraftPicks(fantasyLeagueStore.selectedFantasyDraftPoints.fantasyDraft.draftPickPlayers);
      }
    }

    if (!isMounted.value) {
      isMounted.value = true;
    }
  })
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
  height: calc(100vh - 203px);
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
