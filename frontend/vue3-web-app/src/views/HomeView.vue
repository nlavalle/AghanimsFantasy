<template>
  <v-container>
    <v-row>
      <h1><i>Welcome to Aghanim's Fantasy League for Dota 2</i></h1>
    </v-row>
    <v-row>
      <v-card class="ma-2 pa-1" variant="elevated" color="primary" to="/fantasy">
        <v-card-title>
          <h2>Start Drafting!</h2>
        </v-card-title>
      </v-card>
    </v-row>
  </v-container>
</template>

<script setup lang="ts">
import { VContainer, VRow, VCard, VCardTitle } from 'vuetify/components';

</script>

<style scoped>
.attentionTitle {
  color: var(--aghanims-fantasy-main-2)
}
</style>