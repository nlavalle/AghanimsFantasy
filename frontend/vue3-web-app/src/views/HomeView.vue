<template>
  <div>
    <!-- Hero Section -->
    <section class="hero">
      <div class="hero-content">
        <h1 class="hero-title">Aghanim's Fantasy</h1>
        <p class="hero-tagline">Draft your dream team. Track pro stats. Compete with friends.</p>
        <v-btn to="/fantasy" color="primary" size="large" class="hero-cta">
          Start Drafting
        </v-btn>
      </div>
    </section>

    <v-container>
      <!-- How It Works Section -->
      <section class="section-gap-lg">
        <h2 class="section-title">How It Works</h2>
        <v-row>
          <v-col cols="12" md="4" class="text-center how-it-works-step">
            <font-awesome-icon :icon="faCheckSquare" class="step-icon" />
            <h3 class="step-title">Draft</h3>
            <p class="step-description text-medium-emphasis">Pick your players before each tournament day</p>
          </v-col>
          <v-col cols="12" md="4" class="text-center how-it-works-step">
            <font-awesome-icon :icon="faArrowUp" class="step-icon" />
            <h3 class="step-title">Compete</h3>
            <p class="step-description text-medium-emphasis">Earn points based on real pro match performance</p>
          </v-col>
          <v-col cols="12" md="4" class="text-center how-it-works-step">
            <font-awesome-icon :icon="faTrophy" class="step-icon" />
            <h3 class="step-title">Win</h3>
            <p class="step-description text-medium-emphasis">Climb the leaderboard and claim bragging rights</p>
          </v-col>
        </v-row>
      </section>

      <!-- Tournaments Section -->
      <section class="section-gap-lg">
        <h2 class="section-title">Tournament Schedule</h2>
        <v-row v-if="leagueSchedules.length === 0">
          <v-col>
            <p class="text-medium-emphasis">No upcoming tournaments scheduled.</p>
          </v-col>
        </v-row>
        <v-row v-else>
          <v-col v-for="league in leagueSchedules" :key="league.league_id" cols="12" sm="6" md="4">
            <v-card class="tournament-card" variant="outlined" height="100%">
              <v-card-title class="tournament-name">{{ league.name }}</v-card-title>
              <v-card-subtitle>
                {{ formatDate(league.start_timestamp) }} – {{ formatDate(league.end_timestamp) }}
              </v-card-subtitle>
              <v-card-text>
                <div class="tournament-chips">
                  <v-chip size="small" variant="tonal">
                    {{ regions[league.region] }}
                  </v-chip>
                  <v-chip v-if="isFantasyOpen(league.league_id) === 1" size="small" color="success" variant="tonal"
                    prepend-icon="fa-solid fa-lock-open">
                    Open
                  </v-chip>
                  <v-chip v-else-if="isFantasyOpen(league.league_id) === 2" size="small" variant="tonal"
                    prepend-icon="fa-solid fa-lock">
                    Locked
                  </v-chip>
                  <v-chip v-else size="small" variant="tonal">
                    Coming Soon
                  </v-chip>
                </div>
              </v-card-text>
            </v-card>
          </v-col>
        </v-row>
      </section>
    </v-container>
  </div>
</template>

<script setup lang="ts">
import { VContainer, VRow, VCol, VCard, VCardTitle, VCardSubtitle, VCardText, VBtn, VChip } from 'vuetify/components';
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import { faCheckSquare, faArrowUp, faTrophy } from '@fortawesome/free-solid-svg-icons';
import { localApiService } from '@/services/localApiService';
import type { League } from '@/types/League';
import { onMounted, ref } from 'vue';
import router from '@/router';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';

const leagueSchedules = ref<League[]>([]);
const fantasyLeagueStore = useFantasyLeagueStore();

onMounted(() => {
  router.isReady()
    .then(() => localApiService.getLeagueSchedules().then((result: any) => {
      leagueSchedules.value = result.sort((a: League, b: League) => a.start_timestamp - b.start_timestamp);
    }));
})

const formatDate = (timestamp: number) => {
  return new Date(timestamp * 1000).toLocaleDateString(undefined, {
    month: 'short',
    day: 'numeric',
  });
}

const isFantasyOpen = (leagueId: number) => {
  let fantasyLeagues = fantasyLeagueStore.activeFantasyLeagues.filter(fl => fl.leagueId == leagueId);
  if (fantasyLeagues.some(fl => fantasyLeagueStore.isDraftOpen(fl))) {
    return 1
  } else if (fantasyLeagues.some(fl => !fantasyLeagueStore.isDraftOpen(fl))) {
    return 2
  } else {
    return 3
  }
}

const regions = [
  "Global", "NA", "SA", "WEU", "EEU", "CN", "SEA"
]
</script>

<style scoped>
.hero {
  background: linear-gradient(135deg, var(--aghanims-fantasy-main-4) 0%, var(--aghanims-fantasy-main-3) 100%);
  padding: var(--space-3xl) var(--space-lg);
  text-align: center;
}

.hero-content {
  max-width: 700px;
  margin: 0 auto;
}

.hero-title {
  font-size: var(--text-2xl);
  font-weight: 700;
  margin-bottom: var(--space-md);
}

.hero-tagline {
  font-size: var(--text-lg);
  margin-bottom: var(--space-xl);
}

.hero-cta {
  font-size: var(--text-md);
}

.section-title {
  font-size: var(--text-xl);
  margin-bottom: var(--space-lg);
}

.tournament-card {
  border-color: rgba(255, 255, 255, 0.08);
}

.tournament-name {
  white-space: normal;
  line-height: 1.3;
}

.tournament-chips {
  display: flex;
  gap: var(--space-sm);
  flex-wrap: wrap;
}

.how-it-works-step {
  padding: var(--space-lg);
}

.step-icon {
  font-size: var(--text-2xl);
  color: var(--aghanims-fantasy-main-2);
  margin-bottom: var(--space-md);
}

.step-title {
  font-size: var(--text-lg);
  margin-bottom: var(--space-sm);
}

.step-description {
  font-size: var(--text-base);
}
</style>
