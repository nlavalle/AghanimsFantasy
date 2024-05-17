<script setup lang="ts">
import { RouterView } from 'vue-router'
import Navbar from '@/components/Navbar.vue'
import Sidebar from '@/components/sidebar/Sidebar.vue';
import { sidebarWidth, collapsed } from '@/components/sidebar/state'
</script>

<template>
  <header>
    <Navbar class="navbar" :style="{ 'margin-left': sidebarWidth }" />
  </header>
  <transition name="slide">
    <Sidebar v-if="!collapsed" />
  </transition>
  <main class="main">
    <RouterView :style="{ 'margin-left': sidebarWidth }" />
  </main>
</template>

<style scoped>
.navbar {
  width: 100%;
  height: 3.1rem;
  position: fixed;
  top: 0;
  left: 0;
}

.main {
  margin-top: 1rem;
}

.slide-leave-active,
.slide-enter-active {
  transition: 0.5s ease;
}

.slide-enter-from {
  transform: translate(-100%, 0);
}

.slide-leave-to {
  transform: translate(-100%, 0);
}

header {
  line-height: 1.5;
}
</style>
