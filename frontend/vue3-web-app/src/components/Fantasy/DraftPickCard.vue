<template>
  <v-card class="card-container">
    <v-card-text class="draft-image">
      <div class="flex-container">
        <img height="115px" width="38%" :src="props.playerImageSrc" />
        <img height="115px" width="62%" :src="getImageUrl()" />
      </div>
    </v-card-text>
    <v-card-text class="draft-header">
      <div>
        {{ props.name }}
      </div>
      <div>
        {{ props.team }}
        <img :src=getPositionIcon(props.role) height="25px" width="25px" />
      </div>
    </v-card-text>
    <v-card-text class="draft-body">
      <div class="draft-body-main">
        {{ props.fantasyPoints.toFixed(2) }}
      </div>
      <div class="draft-body-details">
        Fantasy Points
      </div>
      <!-- <div class="draft-body-main">
        {{ role }}
      </div>
      <div class="draft-body-details">
        {{ description }}
      </div> -->
    </v-card-text>
  </v-card>
</template>

<script setup lang="ts">
import { defineProps } from 'vue';
import { VCard, VCardText } from 'vuetify/components';

const props = defineProps({
  name: {
    type: String,
    required: true
  },
  team: {
    type: String,
    required: true
  },
  fantasyPoints: {
    type: Number,
    required: true
  },
  role: {
    type: String,
    required: true
  },
  description: {
    type: String,
    required: true
  },
  playerImageSrc: {
    type: String,
    required: false
  },
  teamImageSrc: {
    type: Number,
    required: false
  },
})

const getPositionIcon = (positionInt: any) => {
  if (positionInt == 0) return undefined;
  return `icons/pos_${positionInt}.png`
}

const getImageUrl = () => {
  if (props.teamImageSrc == 0) return undefined;
  return `logos/teams_logo_${props.teamImageSrc}.png`
}

</script>


<style scoped>
.card-container {
  margin: 20px;
  border-radius: 8px;
  background-color: var(--aghanims-fantasy-blue-1);
  border: 5px solid var(--aghanims-fantasy-accent-dark);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  overflow: hidden;
  font-family: 'Merriweather', serif;
  max-width: 300px;
}

.card-container q-img {
  border-top-left-radius: 8px;
  border-top-right-radius: 8px;
}

.draft-image {
  padding: 0px;
}

.draft-header {
  font-size: 20px;
  font-weight: bold;

  color: #fff;
  padding: 16px;
  border-top-left-radius: 8px;
  border-top-right-radius: 8px;
}

.draft-body {
  padding: 16px;
  background: linear-gradient(to bottom, var(--aghanims-fantasy-accent-dark), var(--aghanims-fantasy-blue-1));
  border-top: 3px solid var(--aghanims-fantasy-accent-dark);
}

.draft-body-main {
  font-size: 1.5rem;
  font-weight: bold;
}

.draft-body-details {
  margin-top: 8px;
}

.flex-container {
  display: flex;
  flex-flow: row wrap;
  max-width: 100%;
}
</style>