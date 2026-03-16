<template>
    <v-app-bar density="compact" scroll-behavior="elevate" color="surface">
        <template v-slot:prepend>
            <router-link to="/" class="app-logo-link">
                <img src="/BannerAvatar.png" alt="Aghanim's Fantasy" class="app-logo" />
            </router-link>
        </template>

        <v-app-bar-title class="app-title d-none d-md-block">
            <div class="title-row">
                <span :style="{ color: activeColor }">Aghanim's Fantasy</span>
                <LeagueSelectorMenu v-if="leagueStore.leagues.length > 1" />
            </div>
        </v-app-bar-title>

        <v-tabs v-model="selectedTab" :slider-color="activeColor">
            <v-tab to="/" class="d-none" />
            <v-tab to="/fantasy">
                <font-awesome-icon :icon="['fas', 'table-cells']" class="tab-icon" />
                Draft
            </v-tab>
            <v-tab to="/stats">
                <font-awesome-icon :icon="['fas', 'chart-line']" class="tab-icon" />
                Stats
            </v-tab>
            <v-tab to="/about">
                <font-awesome-icon :icon="['fas', 'circle-info']" class="tab-icon" />
                About
            </v-tab>
            <v-tab v-show="authStore.isAuthenticated" to="/prizes">
                <font-awesome-icon :icon="['fas', 'trophy']" class="tab-icon" />
                Prizes
            </v-tab>
            <v-tab v-show="authStore.currentUser?.isAdmin ?? false" to="/admin">
                <font-awesome-icon :icon="['fas', 'gear']" class="tab-icon" />
                Admin
            </v-tab>
            <v-tab v-show="authStore.currentUser?.isPrivateFantasyAdmin ?? false" to="/privatefantasy">
                <font-awesome-icon :icon="['fas', 'gear']" class="tab-icon" />
                Private Admin
            </v-tab>
        </v-tabs>

        <v-spacer />

        <template v-slot:append>
            <LoginModal />
        </template>
    </v-app-bar>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { VAppBar, VAppBarTitle, VTab, VTabs, VSpacer } from 'vuetify/components';
import LoginModal from '@/components/Auth/LoginModal.vue';
import LeagueSelectorMenu from '@/components/LeagueSelectorMenu.vue';
import { useAuthStore } from '@/stores/auth';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';
import { useRouter, useRoute } from 'vue-router';

const selectedTab = ref(0)

const authStore = useAuthStore();
const leagueStore = useFantasyLeagueStore();

const router = useRouter();
const route = useRoute();

const tabColors: Record<string, string> = {
    '/fantasy': 'var(--rune-purple)',
    '/stats': 'var(--rune-gold)',
    '/about': 'var(--ot-blue)',
    '/prizes': 'var(--rune-gold-rarity)',
    '/admin': 'var(--rune-red)',
    '/privatefantasy': 'var(--rune-red)',
};

const activeColor = computed(() => tabColors[route.path] ?? 'var(--rune-purple)');

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

.title-row {
    display: flex;
    flex-direction: column;
    align-items: start;
}

.tab-icon {
    margin-right: 6px;
    font-size: 13px;
}

:deep(.v-tab--selected .tab-icon) {
    color: v-bind(activeColor);
}
</style>
