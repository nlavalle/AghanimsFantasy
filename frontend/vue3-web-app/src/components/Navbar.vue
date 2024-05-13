<script setup lang="ts">
import { RouterLink } from 'vue-router'
import { toggleSidebar } from '@/components/sidebar/state'
</script>

<template>
    <div class="navbar">
        <nav>
            <span class="sidebarToggle" @click="toggleSidebar">
                <i class="icon fa-solid fa-bars"></i>
            </span>
            <router-link to="/about">About</router-link>
            <router-link to="/fantasy">Fantasy</router-link>
            <router-link to="/stats">Stats</router-link>
            <router-link to="/leaderboard">Leaderboard</router-link>
            <router-link to="/highlights">Highlights</router-link>
        </nav>
        <hr />
    </div>
</template>

<style scoped>
.navbar {
    position: fixed;
    top: 0;
    left: 0;
}

nav {
    font-size: 14px;
    text-align: left;
    text-transform: uppercase;
    font-family: Avenir, Helvetica, Arial, sans-serif;
    margin-top: 0.5rem;
}

nav a.router-link-exact-active {
    color: var(--color-text);
}

nav a.router-link-exact-active:hover {
    background-color: transparent;
}

nav a {
    display: inline-block;
    padding: 0 1rem;
}

.sidebarToggle {
    margin-left: 1rem;
}

.sidebarToggle:hover {
    color: var(--sidebar-item-hover)
}

.sidebarToggle .icon {
    width: 15px;
    height: 15px;
}
</style>
