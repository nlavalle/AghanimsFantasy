<template>
  <q-card class="card-container">
    <q-img v-if="imageSrc" :src="imageSrc" style="height: 200px" />
    <q-card-section class="award-title">
      <div class="award-name">
        {{ awardTitle }}
      </div>
    </q-card-section>
    <q-card-section class="person-info">
      <div class="person-name">
        {{ name }}
      </div>
      <div class="award-description">
        {{ description }}
      </div>
    </q-card-section>
  </q-card>
</template>

<script lang="ts">
import {
  defineComponent,
} from 'vue';

export default defineComponent({
  name: 'AwardCardComponent',
  props: {
    awardTitle: {
      type: String,
      required: true
    },
    name: {
      type: String,
      required: true
    },
    description: {
      type: String,
      required: true
    },
    imageSrc: {
      type: String,
      required: false
    },
  },
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

.award-name {
  font-size: 1.5rem;
  font-weight: bold;
}

.award-title {
  background-color: #3a404d;
  color: #fff;
  padding: 16px;
  border-top-left-radius: 8px;
  border-top-right-radius: 8px;
}

.person-info {
  padding: 16px;
}

.person-name {
  font-size: 1.5rem;
  font-weight: bold;
  color: #333;
}

.award-description {
  color: #555;
  margin-top: 8px;
}
</style>