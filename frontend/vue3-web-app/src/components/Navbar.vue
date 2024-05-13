<script setup lang="ts">
import { RouterLink } from 'vue-router'
</script>

<template>
    <div class="navbar">
        <nav>
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
    /* width: 100%; */
    height: 40px;
}

nav {
    font-size: 14px;
    text-align: center;
    text-transform: uppercase;
    font-family: Avenir, Helvetica, Arial, sans-serif;
    margin-top: 2rem;
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
</style>
