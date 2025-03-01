<script setup lang="ts">
import { RouterView } from 'vue-router'
import Navbar from '@/components/TheNavbar.vue'
import Footer from '@/components/TheFooter.vue'
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
    <footer>
      <Footer />
    </footer>
  </v-app>
</template>

<style scoped>
main {
  margin-top: 3rem;
}
</style>
