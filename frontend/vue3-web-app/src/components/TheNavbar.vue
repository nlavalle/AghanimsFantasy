<template>
    <v-app-bar density="compact" scroll-behavior="elevate" color="surface">
        <template v-slot:prepend>
            <router-link to="/" class="app-logo-link">
                <img src="/BannerAvatar.png" alt="Aghanim's Fantasy" class="app-logo" />
            </router-link>
        </template>

        <v-app-bar-title class="app-title d-none d-md-block">
            Aghanim's Fantasy
        </v-app-bar-title>

        <v-tabs v-model="selectedTab" slider-color="var(--rune-purple)">
            <v-tab to="/" class="d-none" />
            <v-tab to="/fantasy">Fantasy</v-tab>
            <v-tab to="/stats">Stats</v-tab>
            <v-tab to="/about">About</v-tab>
            <v-tab v-show="authStore.isAuthenticated" to="/prizes">Prizes</v-tab>
            <v-tab v-show="authStore.currentUser?.isAdmin ?? false" to="/admin">Admin</v-tab>
            <v-tab v-show="authStore.currentUser?.isPrivateFantasyAdmin ?? false" to="/privatefantasy">Private
                Admin</v-tab>
        </v-tabs>

        <v-spacer />

        <template v-slot:append>
            <LoginModal />
        </template>
    </v-app-bar>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { VAppBar, VAppBarTitle, VTab, VTabs, VSpacer } from 'vuetify/components';
import LoginModal from '@/components/Auth/LoginModal.vue';
import { useAuthStore } from '@/stores/auth';
import { useRouter } from 'vue-router';

const selectedTab = ref(0)

const authStore = useAuthStore();

const router = useRouter();

onMounted(async () => {
    await router.isReady();
    if (router.currentRoute.value.name == 'fantasy' || router.currentRoute.value.name == 'leaderboard') {
        selectedTab.value = 1;
    }
})

</script>

<style scoped>
.app-logo-link {
    display: flex;
    align-items: center;
    text-decoration: none;
}

.app-logo {
    height: 32px;
    width: 32px;
    border-radius: 4px;
}

.app-title {
    font-family: var(--font-heading);
    font-weight: 600;
    font-size: 1.1rem;
    white-space: nowrap;
    flex: none;
    margin-right: var(--space-md);
}
</style>
