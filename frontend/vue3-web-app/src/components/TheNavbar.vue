<template>
    <div class="bg-surface" style="display:flex">
        <v-tabs v-model="selectedTab">
            <v-tab to="/" min-width="50px" max-width="50px">
                <font-awesome-icon :icon="faHouse" />
            </v-tab>
            <v-tab to="/fantasy" min-width="90px" width="90px">Fantasy</v-tab>
            <v-tab to="/stats" min-width="70px" width="70px">Stats</v-tab>
            <v-tab to="/about" min-width="80px" width="80px">About</v-tab>
            <v-tab v-show="authStore.user?.isAdmin ?? false" to="/admin" min-width="80px" width="80px">Admin</v-tab>
        </v-tabs>
        <v-spacer />
        <LoginDiscord class="login-discord" />
    </div>
    <div>
        <FantasyNavbar v-if="selectedTab === 1" />
    </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { VTab, VTabs, VSpacer } from 'vuetify/components';
import FantasyNavbar from '@/components/Fantasy/FantasyNavbar.vue';
import LoginDiscord from '@/components/LoginDiscord.vue';
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import { faHouse } from '@fortawesome/free-solid-svg-icons';
import { useAuthStore } from '@/stores/auth';

const selectedTab = ref(0)

const authStore = useAuthStore();

</script>

<style scoped>
.login-discord {
    cursor: pointer;
}
</style>
