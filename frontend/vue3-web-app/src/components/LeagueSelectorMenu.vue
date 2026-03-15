<template>
    <v-menu>
        <template v-slot:activator="{ props: menuProps }">
            <div class="league-selector" v-bind="menuProps">
                <span class="league-name">{{ leagueStore.selectedLeague.name }}</span>
                <font-awesome-icon :icon="['fas', 'caret-down']" class="league-caret" />
            </div>
        </template>
        <v-list density="compact">
            <v-list-item v-for="league in leagueStore.leagues" :key="league.league_id"
                :class="{ 'league-list-item--active': league.league_id === leagueStore.selectedLeague.league_id }"
                @click="leagueStore.setSelectedLeague(league)">
                <v-list-item-title class="league-list-title">{{ league.name }}</v-list-item-title>
            </v-list-item>
        </v-list>
    </v-menu>
</template>

<script setup lang="ts">
import { VMenu, VList, VListItem, VListItemTitle } from 'vuetify/components';
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';

const leagueStore = useFantasyLeagueStore();
</script>

<style scoped>
.league-selector {
    display: flex;
    align-items: center;
    gap: 4px;
    cursor: pointer;
    width: fit-content;
}

.league-name {
    font-family: var(--font-body);
    font-size: var(--text-xs);
    font-weight: 400;
    color: color-mix(in srgb, var(--sg-heading) 50%, transparent);
    letter-spacing: 0.5px;
    line-height: 1;
    transition: color 0.15s;
}

.league-selector:hover .league-name {
    color: white;
}

.league-caret {
    font-size: 9px;
    color: color-mix(in srgb, white 30%, transparent);
    transition: color 0.15s;
}

.league-selector:hover .league-caret {
    color: color-mix(in srgb, white 60%, transparent);
}

.league-list-title {
    font-family: var(--font-body);
    font-size: var(--text-sm);
}

:deep(.league-list-item--active .v-list-item-title) {
    color: var(--rune-purple-light);
}
</style>
