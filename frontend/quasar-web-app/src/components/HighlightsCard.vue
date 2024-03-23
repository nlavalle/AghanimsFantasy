<template>
  <q-card class="card-container">
    <q-card-section class="highlight-image">
      <div class="flex-container">
        <q-img height="115px" width="38%" :src="highlight.fantasyPlayer.dotaAccount.steamProfilePicture" />
        <q-img height="115px" width="62%" :src="getTeamImageUrl()" />
      </div>
    </q-card-section>
    <q-card-section class="highlight-header">
      <div>
        {{ highlight.fantasyPlayer.dotaAccount.name }}
      </div>
      <div>
        {{ highlight.fantasyPlayer.team.name }}
      </div>
    </q-card-section>
    <q-card-section class="highlight-body">
      <div>
        <table>
          <thead>
            <tr>
              <th style="width:70px">Stat</th>
              <th>Amount</th>
              <th>Points</th>
              <th>vs Avg</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="highlight.killsPointsDeviation">
              <td>Kills</td>
              <td>{{ highlight.kills }}</td>
              <td>{{ highlight.killsPoints.toFixed(2) }}</td>
              <td :style="{ color: highlight.killsDiff > 0 ? 'green' : 'red' }">{{ highlight.killsDiff > 0 ? '+' +
          highlight.killsDiff : highlight.killsDiff }}</td>
            </tr>
            <tr v-if="highlight.deathsPointsDeviation">
              <td>Deaths</td>
              <td>{{ highlight.deaths }}</td>
              <td>{{ highlight.deathsPoints.toFixed(2) }}</td>
              <td :style="{ color: highlight.deathsDiff > 0 ? 'green' : 'red' }">{{ highlight.deathsDiff > 0 ? '+' +
          highlight.deathsDiff : highlight.deathsDiff }}</td>
            </tr>
            <tr v-if="highlight.assistsPointsDeviation">
              <td>Assists</td>
              <td>{{ highlight.assists }}</td>
              <td>{{ highlight.assistsPoints.toFixed(2) }}</td>
              <td :style="{ color: highlight.assistsDiff > 0 ? 'green' : 'red' }">{{ highlight.assistsDiff > 0 ? '+' +
          highlight.assistsDiff : highlight.assistsDiff }}</td>
            </tr>
            <tr v-if="highlight.lastHitsPointsDeviation">
              <td>Last Hits</td>
              <td>{{ highlight.lastHits }}</td>
              <td>{{ highlight.lastHitsPoints.toFixed(2) }}</td>
              <td :style="{ color: highlight.lastHitsDiff > 0 ? 'green' : 'red' }">{{ highlight.lastHitsDiff > 0 ? '+' +
          highlight.lastHitsDiff : highlight.lastHitsDiff }}</td>
            </tr>
            <tr v-if="highlight.goldPerMinPointsDeviation">
              <td>Gold/Min</td>
              <td>{{ highlight.goldPerMin }}</td>
              <td>{{ highlight.goldPerMinPoints.toFixed(2) }}</td>
              <td :style="{ color: highlight.goldPerMinDiff > 0 ? 'green' : 'red' }">{{ highlight.goldPerMinDiff > 0 ? '+' +
          highlight.goldPerMinDiff : highlight.goldPerMinDiff }}</td>
            </tr>
            <tr v-if="highlight.xpPerMinPointsDeviation">
              <td>Xp/Min</td>
              <td>{{ highlight.xpPerMin }}</td>
              <td>{{ highlight.xpPerMinPoints.toFixed(2) }}</td>
              <td :style="{ color: highlight.xpPerMinDiff > 0 ? 'green' : 'red' }">{{ highlight.xpPerMinDiff > 0 ? '+' +
                highlight.xpPerMinDiff : highlight.xpPerMinDiff }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </q-card-section>
  </q-card>
</template>

<script>
import { defineComponent } from 'vue';

export default defineComponent({
  name: 'HighlightCard',
  props: {
    highlight: {
      type: Object,
      required: true
    },
  },
  methods: {
    getTeamImageUrl() {
      if (this.highlight.fantasyPlayer.team.id == 0) return null;
      return new URL(`../assets/logos/teams_logo_${this.highlight.fantasyPlayer.team.id}.png`, import.meta.url).toString();
    }
  }
});
</script>

<style scoped>
.card-container {
  margin: 10px;
  border-radius: 8px;
  background-color: var(--nadcl-main-4);
  border: 5px solid var(--nadcl-main-2);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  overflow: hidden;
  font-family: 'Merriweather', serif;
  max-width: 280px;
}

.highlight-image {
  padding: 0px;
}

.highlight-header {
  font-size: 20px;
  font-weight: bold;

  color: #fff;
  padding: 16px;
  border-top-left-radius: 8px;
  border-top-right-radius: 8px;
}

.highlight-body {
  padding: 5px;
  background: linear-gradient(to bottom, var(--nadcl-main-2), var(--nadcl-main-4));
  border-top: 3px solid var(--nadcl-main-2);
}

</style>