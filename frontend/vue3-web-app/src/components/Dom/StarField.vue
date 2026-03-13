<template>
  <canvas ref="canvas" class="star-field" />
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, nextTick } from 'vue'
import { registerCanvas, unregisterCanvas } from './starEngine'

const canvas = ref<HTMLCanvasElement>()

onMounted(async () => {
  await nextTick()
  const el = canvas.value!
  const parent = el.parentElement!
  el.width = parent.offsetWidth
  el.height = parent.offsetHeight
  registerCanvas(el)
})

onUnmounted(() => {
  if (canvas.value) unregisterCanvas(canvas.value)
})
</script>

<style scoped>
.star-field {
  position: absolute;
  inset: 0;
  pointer-events: none;
  z-index: 0;
}
</style>
