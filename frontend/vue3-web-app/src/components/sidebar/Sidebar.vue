<template>
    <div class="sidebar" :style="{ width: sidebarWidth }">
        <h3>
            <div style="display:flex">
                <div>
                    <img src="@/assets/BannerAvatar.png" height="38px" width="38px" style="margin-right:5px;" />
                </div>
                <div>
                    <span>Aghanim's Fantasy</span>
                </div>
            </div>
        </h3>

        <SidebarLink class="link" to="/" icon="fas fa-home">Home</SidebarLink>
        <SidebarLink class="link" to="/fantasy" icon="fas fa-columns">Fantasy Draft</SidebarLink>

        <LeagueSelector class="dropdown" />

        <LoginDiscord class="link" />

        <div v-if="authenticated" class="userWelcome">
            <span>
                Welcome {{ user.name }}
            </span>
        </div>
    </div>
</template>

<script setup lang="ts">
import SidebarLink from './SidebarLink.vue'
import LoginDiscord from '@/components/LoginDiscord.vue'
import LeagueSelector from '@/components/LeagueSelector.vue'
import { sidebarWidth } from './state'
import { useAuthStore } from '@/stores/auth'
import { computed } from 'vue'

const authStore = useAuthStore();

const authenticated = computed(() => {
    return authStore.authenticated;
});

const user = computed(() => {
    return authStore.user ?? {};
});

</script>

<style scoped>
.sidebar {
    color: white;
    background-color: var(--highlight-bg);

    float: left;
    position: fixed;
    z-index: 1;
    top: 0;
    left: 0;
    bottom: 0;
    padding: 0.5em;

    display: flex;
    flex-direction: column;
}

.link {
    display: flex;
    align-items: center;

    cursor: pointer;
    position: relative;
    font-weight: 400;
    user-select: none;

    margin: 0.1em 0;
    padding: 0.4em;
    border-radius: 0.25em;
    height: 1.5em;

    color: white;
    text-decoration: none;
}

.link:hover {
    background-color: var(--highlight-bg)
}

.link.active {
    background-color: var(--primary-color);
}

.dropdown {
    display: flex;

    /* cursor: pointer; */
    position: relative;
    font-weight: 400;
    user-select: none;

    margin: 0.1em 0;
    padding-left: 0.4em;
    border-radius: 0.25em;

    color: white;
    text-decoration: none;
}

.userWelcome {
    position: absolute;
    bottom: 0;
    padding: 0.75em;
}
</style>