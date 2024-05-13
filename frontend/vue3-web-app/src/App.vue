<script setup lang="ts">
import { RouterView } from 'vue-router'
import Navbar from '@/components/Navbar.vue'
import Sidebar from '@/components/sidebar/Sidebar.vue';
import { sidebarWidth } from '@/components/sidebar/state'
</script>

<template>
  <header :style="{ 'margin-left': sidebarWidth }">
    <Navbar style="width: 100%" />
  </header>
  <Sidebar />
  <main :style="{ 'margin-left': sidebarWidth }">
    <RouterView />
  </main>
</template>

<style scoped>
.layout {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  width: 100%;
}

header {
  line-height: 1.5;
  max-height: 100vh;
}

</style>
