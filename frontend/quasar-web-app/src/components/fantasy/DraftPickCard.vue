<template>
  <q-card class="card-container">
    <q-card-section class="draft-image">
      <div class="flex-container">
        <q-img height="115px" width="38%" :src="playerImageSrc" />
        <q-img height="115px" width="62%" :src="getImageUrl()" />
      </div>
    </q-card-section>
    <q-card-section class="draft-header">
      <div>
        {{ name }}
      </div>
      <div>
        {{ team }}
      </div>
    </q-card-section>
    <q-card-section class="draft-body">
      <div class="draft-body-main">
        {{ fantasyPoints }}
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
    </q-card-section>
  </q-card>
</template>

<script>
import { defineComponent } from 'vue';

export default defineComponent({
  name: 'DraftPickCard',
  props: {
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
  },
  methods: {
    getImageUrl() {
      if (this.teamImageSrc == 0) return null;
      return new URL(`../../assets/logos/teams_logo_${this.teamImageSrc}.png`, import.meta.url).toString();
    }
  }
});
</script>

<style scoped>
.card-container {
  margin: 20px;
  border-radius: 8px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.card-container q-img {
  border-top-left-radius: 8px;
  border-top-right-radius: 8px;
}

.draft-image {
  background-color: #3a404d;
  padding: 0px;
}

.draft-header {
  font-size: 1.5rem;
  font-weight: bold;
  background-color: #3a404d;
  color: #fff;
  padding: 16px;
  border-top-left-radius: 8px;
  border-top-right-radius: 8px;
}

.draft-body {
  padding: 16px;
}

.draft-body-main {
  font-size: 1.5rem;
  font-weight: bold;
  /* color: #333; */
}

.draft-body-details {
  margin-top: 8px;
  /* color: #555; */
}

.flex-container {
  display: flex;
  flex-flow: row wrap;
  max-width: 100%;
  /* padding-left: 20px;
  padding-right: 20px; */
}

</style>