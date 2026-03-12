<template>
  <div>
    <v-container>
      <v-row class="align-center">
        <h3>Prizes</h3>
        <v-spacer />
        <UserBalance />
      </v-row>
      <v-row>
        <v-col>
          <ol>
            <li :class="{ 'disabled-item': isKillStreakDisabled }" class="prize-item">
              <div v-if="isKillStreakDisabled" class="sold-overlay">
                <span>SOLD</span>
              </div>
              <div class="d-flex align-center">
                <span><b>Killstreak Card Effect</b> price: </span>
                <ShardSpan :animated="true" :font-size="1.0" :bold="true" :gold-value="'1400'" />
              </div>
              <div>
                <span>When a player has 3 or more consecutive wins their card will gain this flame
                  effect until their next loss.<br />This effect is permanent across all current and
                  future fantasy leagues.</span>
              </div>
              <draft-pick-card-hover class="draft-pick-card" :killStreakEffect="true"
                :fantasyPlayer="exampleFantasyPlayer" :fantasyPoints="12.34" />
              <v-btn :disabled="!canAffordKillstreak" @click="showConfirmDialog = !showConfirmDialog">Purchase</v-btn>
            </li>
          </ol>
        </v-col>
      </v-row>
    </v-container>
    <ConfirmDialog v-model="showConfirmDialog" title="Purchase Card Effect?"
      body="Are you sure you want to purchase the Killstreak Card Effect for 1400 shards?" @ok="purchaseKillstreak()" />
    <ErrorDialog v-model="showErrorModal" :error="errorDetails!" />
  </div>
</template>

<script setup lang="ts">
import type { FantasyPlayer } from '@/components/Fantasy/fantasyDraft';
import { localApiService } from '@/services/localApiService';
import { computed, onMounted, ref } from 'vue';
import { VContainer, VRow, VCol, VSpacer, VBtn } from 'vuetify/components';
import DraftPickCardHover from '@/components/Fantasy/DraftPickCardHover.vue';
import UserBalance from '@/components/Fantasy/UserBalance.vue';
import ShardSpan from '@/components/Dom/ShardSpan.vue';
import { useAuthStore } from '@/stores/auth';
import ConfirmDialog from '@/components/ConfirmDialog.vue';
import ErrorDialog from '@/components/ErrorDialog.vue';

const authStore = useAuthStore()

const availablePrizes = ref()

const showConfirmDialog = ref(false)
const showErrorModal = ref(false);
const errorDetails = ref<Error>();

const isKillStreakDisabled = computed(() => {
  return authStore.user.prizes?.some(p => p.prize_type == 0) ?? false
})

const canAffordKillstreak = computed(() => {
  return (authStore.currentUser.stashBalance ?? 0) >= 1400
});

const exampleFantasyPlayer = ref<FantasyPlayer>({
  dotaAccount: {
    id: 0,
    name: "Best Player",
    steamProfilePicture: "https://aghanimsfantasy.com/logos/unknown.png"
  },
  dotaAccountId: 0,
  fantasyLeagueId: 0,
  id: 0,
  team: {
    abbreviation: "BT",
    id: 0,
    logo: "0",
    logoSponsor: "0",
    name: "Best Team",
    tag: "BT"
  },
  teamId: 0,
  teamPosition: 1,
  substitution: false
})

onMounted(() => {
  localApiService.getAllPrizes().then((result: any) => {
    availablePrizes.value = result
  })
})

const purchaseKillstreak = () => {
  localApiService.buyPrize('KILL_STREAK_FLAMES').then((result: any) => {
    availablePrizes.value = result
    authStore.loadUser()
  })?.catch((error: Error) => {
    showError(error)
  })
}

const showError = (error: Error) => {
  errorDetails.value = error;
  showErrorModal.value = true;
  window.scrollTo({
    top: 0,
    left: 0,
    behavior: 'smooth'
  });
}
</script>

<style scoped>
.draft-pick-card {
  height: 300px;
  margin: 20px;
}

.disabled-item {
  pointer-events: none;
  opacity: 0.5;
}

.prize-item {
  position: relative;
}

.sold-overlay {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%) rotate(-30deg);
  font-size: 3rem;
  font-weight: bold;
  color: red;
  text-shadow: 0 0 4px white;
  pointer-events: none;
  z-index: 2;
  opacity: 0.8;
  border: 3px solid red;
  padding: 0.5rem 1rem;
  border-radius: 8px;
  background-color: rgba(255, 255, 255, 0.6);
}
</style>
