<script setup lang="ts">
import { RouterView } from 'vue-router'
import Navbar from '@/components/TheNavbar.vue'
import { VApp, VMain } from 'vuetify/components'
</script>

<template>
  <v-app>
    <header class="bg-surface">
      <Navbar />
    </header>
    <v-main>
      <RouterView />
    </v-main>
  </v-app>
</template>

<style scoped>
.navbar {
  height: 3rem;
  width: 100%;
  position: fixed;
  top: 0;
  left: 0;
}

main {
  margin-top: 3rem;
}
</style>
