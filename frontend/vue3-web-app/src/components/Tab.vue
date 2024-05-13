<template>
  <div class="tab-content" v-show="title == selectedTitle">
    <slot></slot>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  title: string
}>()

import { inject } from 'vue'

const selectedTitle = inject("selectedTitle")

</script>

<style scoped>
.tab-content {
  min-height: 400px;
  padding: 20px;
  border-radius: 5px;
  box-shadow: 0 3px 5px 5px rgba(0, 0, 0, 0.05);
}
</style>