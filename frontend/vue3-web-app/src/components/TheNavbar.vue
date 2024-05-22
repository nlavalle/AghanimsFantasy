<template>
    <div class="bg-surface" style="display:flex">
        <v-tabs v-model="selectedTab">
            <v-tab to="/" min-width="50px" max-width="50px"><span class="fa-solid fa-house"></span></v-tab>
            <v-tab to="/about">About</v-tab>
            <v-tab>Fantasy</v-tab>
            <v-tab to="/stats">Stats</v-tab>
        </v-tabs>
        <v-spacer />
        <LoginDiscord class="ma-3 login-discord" />
    </div>
    <div>
        <FantasyNavbar v-if="selectedTab === 2" />
    </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { VTab, VTabs, VSpacer } from 'vuetify/components';
import FantasyNavbar from '@/components/Fantasy/FantasyNavbar.vue';
import LoginDiscord from '@/components/LoginDiscord.vue';

const selectedTab = ref(0)

</script>

<style scoped>
.login-discord {
    cursor: pointer;
}
</style>
