<template>
  <div class="highlights">
    <div class="row justify-evenly">
      <div class="col col-4-grow">
        <h5 class="text-white">
          Highlights from Past Games
        </h5>
      </div>
    </div>
    <div class="flex-container">
      <div v-for="match in HighlightMatches" :key="match.matchId">
        <q-card class="match-container">
          <q-card-section class="highlight-header">
            <div>
              Match: {{ match.matchId }}
            </div>
            <div>
              Start Time: {{ new Date(match.startTime * 1000).toISOString().split('T')[0] }}
            </div>
          </q-card-section>
          <q-card-section class="highlight-matches">
            <div class="flex-container">
              <div v-for="highlight in Highlights.filter(hl => hl.matchId == match.matchId)" :key="highlight">
                <highlights-card class="highlight-card" :highlight="highlight" />
              </div>
            </div>
          </q-card-section>
        </q-card>
      </div>
    </div>
  </div>
</template>

<script>
import HighlightsCard from 'src/components/HighlightsCard.vue'

export default {
  name: 'HighlightsPage',
  components: {
    HighlightsCard
  }
}
</script>

<script setup>
import { ref, onMounted, computed, watch } from 'vue';
import { localApiService } from 'src/services/localApiService';
import { useLeagueStore } from 'src/stores/league';

const leagueStore = useLeagueStore();
const Highlights = ref([]);
const highlightAmount = 10;

const HighlightMatches = computed(() => {
  const uniqueMatches = Array.from(
    new Set(Highlights.value.map(highlight => highlight.matchId))
  )
    .map(id => Highlights.value.find(item => item.matchId == id));
  return uniqueMatches;
});

onMounted(() => {
  if (leagueStore.selectedLeague) {
    localApiService.getHighlights(leagueStore.selectedLeague.id, highlightAmount)
      .then(result => Highlights.value = result);
  }
});

watch(() => leagueStore.selectedLeague, (newValue) => {
  if (newValue) {
    if (leagueStore.selectedLeague) {
      localApiService.getHighlights(leagueStore.selectedLeague.id, highlightAmount)
        .then(result => Highlights.value = result);
    }
  }
});

</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.highlight-card {
  margin: 10px;
  width: 400px;
}

.match-container {
  margin: 10px;
  border-radius: 8px;
  background-color: var(--nadcl-main-4);
  border: 5px solid var(--nadcl-main-2);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  overflow: hidden;
  font-family: 'Merriweather', serif;
}

.highlight-header {
  font-size: 20px;
  font-weight: bold;

  color: #fff;
  padding: 16px;
  border-top-left-radius: 8px;
  border-top-right-radius: 8px;
}

.flex-container {
  display: flex;
  flex-flow: row wrap;
  max-width: 100%;
  padding-left: 4px;
  padding-right: 4px;
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