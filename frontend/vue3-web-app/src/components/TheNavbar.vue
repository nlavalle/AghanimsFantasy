<template>
    <div class="bg-surface" style="display:flex">
        <v-tabs>
            <v-tab to="/" @click="hideFantasyNavbar"><span class="fa-solid fa-house"></span></v-tab>
            <v-tab to="/about" @click="hideFantasyNavbar">About</v-tab>
            <v-tab @click="showFantasyNavbar">Fantasy</v-tab>
            <v-tab to="/stats" @click="hideFantasyNavbar">Stats</v-tab>
        </v-tabs>
        <v-spacer />
        <LoginDiscord class="ma-3 login-discord" />
    </div>
    <div>
        <FantasyNavbar v-show="!collapsed" />
    </div>
</template>

<script setup lang="ts">
import { VTab, VTabs, VSpacer } from 'vuetify/components'
import FantasyNavbar from '@/components/Fantasy/FantasyNavbar.vue'
import { collapsed, hideFantasyNavbar, showFantasyNavbar } from './Fantasy/FantasyState'
import LoginDiscord from '@/components/LoginDiscord.vue'
</script>

<style scoped>
.login-discord {
    cursor: pointer;
}
</style>
