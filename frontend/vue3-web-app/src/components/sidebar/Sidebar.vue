<template>
    <div class="sidebar" :style="{ width: sidebarWidth }">
        <h1>
            <span>Vue Sidebar</span>
        </h1>

        <SidebarLink to="/" icon="fas fa-home">Home</SidebarLink>
        <SidebarLink to="/dashboard" icon="fas fa-columns">Fantasy Draft</SidebarLink>
        <SidebarLink to="/analytics" icon="fas fa-chart-bar">Login</SidebarLink>
        <SidebarLink to="/friends" icon="fas fa-users">Logout</SidebarLink>

        <span class="collapse-icon" @click="toggleSidebar" :class="{ 'rotate-180': collapsed }">
            <i class="fas fa-angle-double-left" />
        </span>
    </div>
</template>

<script setup>
import SidebarLink from './SidebarLink.vue'
import { collapsed, toggleSidebar, sidebarWidth } from './state'
</script>

<style>
:root {
    --sidebar-bg-color: #2f855a;
    --sidebar-item-hover: #38a169;
    --sidebar-item-active: #276749;
}
</style>

<style scoped>
.sidebar {
    color: white;
    background-color: var(--sidebar-bg-color);

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

.collapse-icon {
    position: absolute;
    bottom: 0;
    padding: 0.75em;

    color: rgba(255,255,255,0.7);

    transition: 0.2s linear;
}

.rotate-180 {
    transform: rotate(180deg);
    transition: 0.2s linear;
}
</style>