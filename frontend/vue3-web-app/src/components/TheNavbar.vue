<template>
    <div class="bg-surface" style="display:flex">
        <v-tabs v-model="selectedTab">
            <v-tab to="/" min-width="50px" max-width="50px">
                <font-awesome-icon :icon="faHouse" />
            </v-tab>
            <v-tab to="/fantasy" min-width="90px" width="90px">Fantasy</v-tab>
            <v-tab to="/stats" min-width="70px" width="70px">Stats</v-tab>
            <v-tab to="/about" min-width="80px" width="80px">About</v-tab>
            <v-tab v-show="authStore.currentUser?.isAdmin ?? false" to="/admin" min-width="80px"
                width="80px">Admin</v-tab>
            <v-tab v-show="authStore.currentUser?.isPrivateFantasyAdmin ?? false" to="/privatefantasy" min-width="140px"
                width="140px">Private Admin</v-tab>
        </v-tabs>
        <v-spacer />
        <LoginModal class="login-modal" />
    </div>
    <div>
        <FantasyNavbar />
    </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { VTab, VTabs, VSpacer } from 'vuetify/components';
import FantasyNavbar from '@/components/Fantasy/FantasyNavbar.vue';
import LoginModal from '@/components/Auth/LoginModal.vue';
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import { faHouse } from '@fortawesome/free-solid-svg-icons';
import { useAuthStore } from '@/stores/auth';
import { useRouter } from 'vue-router';

const selectedTab = ref(0)

const authStore = useAuthStore();

const router = useRouter();

onMounted(async () => {
    // If a user is refreshing the leaderboard page we want to navigate to it and show FantasyNavbar to pick up the league
    await router.isReady();
    if (router.currentRoute.value.name == 'fantasy' || router.currentRoute.value.name == 'leaderboard') {
        selectedTab.value = 1;
    }
})

</script>

<style scoped>
.login-modal {
    cursor: pointer;
}
</style>
