<template>
  <div class="draft-card" :style="{ width: isDesktop ? '300px' : '150px', height: isDesktop ? '380px' : '190px' }"
    @mouseenter="handleMouseEnter" @mouseleave="handleMouseLeave" @mousemove="handleMouseMove">
    <v-card class="card-container" height="100%">
      <v-row :style="{ height: isDesktop ? '130px' : '70px' }">
        <v-col>
          <img :height="isDesktop ? '115px' : '58px'" width="38%" :src="props.playerImageSrc" />
          <img :height="isDesktop ? '115px' : '58px'" width="62%" :src="getImageUrl()" />
        </v-col>
      </v-row>
      <v-row align-content="center" :style="{ height: isDesktop ? '100px' : '50px' }">
        <v-col class="pa-0">
          <v-card-title :style="{ 'font-size': isDesktop ? '20px' : '12px' }">
            <v-row justify="center">
              {{ props.name }}
            </v-row>
          </v-card-title>
          <v-card-subtitle :style="{ 'font-size': isDesktop ? '18px' : '11px' }">
            <v-row justify="center">
              <v-col style="text-align: center;">
                {{ props.team }}
                <img :src=getPositionIcon(props.role) height="20px" width="20px" />
              </v-col>
            </v-row>
          </v-card-subtitle>
        </v-col>
      </v-row>
      <v-row style="height:100%">
        <v-card-text class="draft-body pt-1">
          <div class="draft-body-main" :style="{ 'font-size': isDesktop ? '1.5rem' : '1rem' }">
            {{ props.fantasyPoints.toFixed(2) }}
          </div>
          <div class="draft-body-details" :style="{ 'font-size': isDesktop ? '1rem' : '0.8rem' }">
            Fantasy Points
          </div>
        </v-card-text>
      </v-row>

    </v-card>
  </div>
</template>

<script setup lang="ts">
import { ref, defineProps } from 'vue';
import { VCard, VCardTitle, VCardSubtitle, VCardText, VRow, VCol } from 'vuetify/components';

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
    type: Number,
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

const isDesktop = ref(window.outerWidth >= 600);

const getPositionIcon = (positionInt: number) => {
  if (positionInt == 0) return undefined;
  return `icons/pos_${positionInt}.png`
}

const getImageUrl = () => {
  if (props.teamImageSrc == 0) return undefined;
  return `logos/teams_logo_${props.teamImageSrc}.png`
}

const cardBoundingRef = ref<DOMRect | null>(null);

const handleMouseEnter = (event: MouseEvent) => {
  const target = event.currentTarget as HTMLElement;
  cardBoundingRef.value = target.getBoundingClientRect();
}

const handleMouseLeave = () => {
  cardBoundingRef.value = null;
}

const handleMouseMove = (event: MouseEvent) => {
  if (!cardBoundingRef.value) return;
  const x = event.clientX - cardBoundingRef.value.left;
  const y = event.clientY - cardBoundingRef.value.top;
  const xPercentage = x / cardBoundingRef.value.width;
  const yPercentage = y / cardBoundingRef.value.height;
  const xRotation = (xPercentage - 0.5) * 20;
  const yRotation = (0.5 - yPercentage) * 20;

  const target = event.currentTarget as HTMLElement;
  target.style.setProperty("--x-rotation", `${yRotation}deg`);
  target.style.setProperty("--y-rotation", `${xRotation}deg`);
  target.style.setProperty("--x", `${xPercentage * 100}%`);
  target.style.setProperty("--y", `${yPercentage * 100}%`);
}

</script>


<style scoped>
.draft-card {
  perspective: 800px;
}

.draft-card:hover {
  transform: rotateX(var(--x-rotation)) rotateY(var(--y-rotation));
  scale: 1.1;
}

.card-container:hover {
  background: radial-gradient(at var(--x) var(--y), rgba(200, 200, 200, 0.1) 10%, transparent 90%);
  background-color: var(--aghanims-fantasy-blue-1);
}

.card-container {
  border-radius: 8px;
  background-color: var(--aghanims-fantasy-blue-1);
  border: 6px solid var(--aghanims-fantasy-accent-dark);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  overflow: hidden;
  font-family: 'Merriweather', serif;
}

.card-container q-img {
  border-top-left-radius: 8px;
  border-top-right-radius: 8px;
}

.draft-body {
  background: linear-gradient(to bottom, var(--aghanims-fantasy-accent-dark), var(--aghanims-fantasy-blue-1));
  border-top: 3px solid var(--aghanims-fantasy-accent-dark);
  height: 50%;
  text-align: center
}

.draft-body:hover {
  background:
    radial-gradient(at var(--x) var(--y), rgba(200, 200, 200, 0.1) 10%, transparent 90%),
    linear-gradient(to bottom, var(--aghanims-fantasy-accent-dark), var(--aghanims-fantasy-blue-1))
}

.draft-body-main {
  font-weight: bold;
}

.draft-body-details {
  margin-top: 8px;
}
</style>