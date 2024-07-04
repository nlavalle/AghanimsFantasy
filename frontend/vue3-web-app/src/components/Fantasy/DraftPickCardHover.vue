<template>
  <div class="parent" @mouseenter="handleMouseEnter" @mouseleave="handleMouseLeave" @mousemove="handleMouseMove">
    <div v-if="cardBoundingRef" class="hover-filter">
    </div>
    <draft-pick-card class="draft-pick-card-hover" :fantasyPlayer="props.fantasyPlayer"
      :fantasyPoints="props.fantasyPoints" />
  </div>
</template>

<script setup lang="ts">
import { ref, type PropType } from 'vue';
import DraftPickCard from '@/components/Fantasy/DraftPickCard.vue';
import type { FantasyPlayer } from './fantasyDraft';

const props = defineProps({
  fantasyPlayer: {
    type: Object as PropType<FantasyPlayer>,
    required: false
  },
  fantasyPoints: {
    type: Number,
    required: false
  },
})

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
  const xRotation = -1 * (xPercentage - 0.5) * 20;
  const yRotation = -1 * (0.5 - yPercentage) * 20;

  const target = event.currentTarget as HTMLElement;
  target.style.setProperty("--x-rotation", `${yRotation}deg`);
  target.style.setProperty("--y-rotation", `${xRotation}deg`);
  target.style.setProperty("--x", `${xPercentage * 100}%`);
  target.style.setProperty("--y", `${yPercentage * 100}%`);
}

</script>

<style scoped>
.parent {
  position: relative;
  perspective: 800px;
  aspect-ratio: 0.7;
}

.draft-pick-card-hover {
  z-index: 1;
}

.hover-filter {
  position: absolute;
  left: 0;
  top: 0;
  background: radial-gradient(at var(--x) var(--y), rgba(200, 200, 200, 0.2) 10%, transparent 90%);
  width: 100%;
  height: 100%;
  z-index: 2;
  pointer-events: none;
  border-radius: 10px;
}

.parent:hover {
  transform: rotateX(var(--x-rotation)) rotateY(var(--y-rotation));
  scale: 1.1;
}

.draft-pick-card-hover:hover {
  transform: rotateX(var(--x-rotation)) rotateY(var(--y-rotation));
}

.hover-filter:hover {
  transform: rotateX(var(--x-rotation)) rotateY(var(--y-rotation));
}
</style>