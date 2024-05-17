<template>
    <div class="navbar">
        <span class="sidebarToggle" @click="toggleSidebar">
            <i class="icon fa-solid fa-bars"></i>
        </span>
        <TabMenu style="width: 100%; text-transform: uppercase;" :model="items">
            <template #item="{ item, props }">
                <router-link v-slot="{ href, navigate }" :to="item.route" custom>
                    <a :href="href" v-bind="props.action" @click="navigate" style="align-items:center;">
                        <span v-bind="props.icon" style="margin-right: 0" />
                        <span v-bind="props.label">{{ item.label }}</span>
                    </a>
                </router-link>
            </template>
        </TabMenu>
    </div>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import { RouterLink } from 'vue-router'
import { toggleSidebar } from '@/components/sidebar/state';
import TabMenu from 'primevue/tabmenu';

const items = ref([
    {
        icon: 'fa-solid fa-house',
        label: '',
        route: '/'
    },
    {
        label: 'About',
        route: '/about'
    },
    {
        label: 'Fantasy',
        route: '/fantasy'
    },
    {
        label: 'Stats',
        route: '/stats'
    },
    {
        label: 'Leaderboard',
        route: '/leaderboard'
    },
    {
        label: 'Highlights',
        route: '/highlights'
    }
])

</script>

<style scoped>
.navbar {
    display: flex;
    background: rgb(24, 24, 27);

}

.sidebarToggle {
    height: 49px;
    cursor: pointer;
    border: 1px solid #3f3f46;
    border-width: 0 0 1px 0;
}

.sidebarToggle:hover {
    color: var(--primary-color)
}

.sidebarToggle .icon {
    vertical-align: middle;
    margin: 15px;
    width: 20px;
    height: 20px;
}
</style>
