<template>
    <div v-if="alerts.length > 0" class="alert-banner-stack">
        <v-alert
            v-for="alert in alerts"
            :key="alert.league.league_id"
            :color="alert.type === 'draft' ? 'warning' : 'info'"
            variant="tonal"
            density="compact"
            class="alert-banner"
        >
            <div class="alert-content">
                <font-awesome-icon
                    :icon="alert.type === 'draft' ? ['fas', 'clock'] : ['fas', 'gamepad']"
                    class="alert-icon"
                />
                <span class="alert-text">{{ alert.message }}</span>
                <v-btn
                    size="small"
                    variant="outlined"
                    :color="alert.type === 'draft' ? 'warning' : 'info'"
                    class="alert-switch-btn"
                    @click="switchLeague(alert.league)"
                >
                    Switch
                </v-btn>
            </div>
        </v-alert>
    </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { VAlert, VBtn } from 'vuetify/components';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';
import type { League } from '@/types/League';

const leagueStore = useFantasyLeagueStore();

interface AlertItem {
    league: League;
    type: 'draft' | 'live';
    message: string;
}

function formatCountdown(lockEpoch: number): string {
    const secondsLeft = Math.max(lockEpoch - Math.floor(Date.now() / 1000), 0);
    const h = Math.floor(secondsLeft / 3600);
    const m = Math.floor((secondsLeft % 3600) / 60);
    if (h > 0) return `${h}h ${m}m`
    return `${m}m`
}

const alerts = computed((): AlertItem[] => {
    return leagueStore.otherUrgentLeagues.map(league => {
        const leagueFLs = leagueStore.activeFantasyLeagues.filter(fl => fl.leagueId === league.league_id);
        const openDraft = leagueFLs
            .filter(fl => leagueStore.isDraftOpen(fl))
            .reduce<typeof leagueFLs[0] | undefined>((min, fl) =>
                !min || fl.fantasyDraftLocked < min.fantasyDraftLocked ? fl : min, undefined);

        if (openDraft) {
            return {
                league,
                type: 'draft' as const,
                message: `${league.name}: draft closes in ${formatCountdown(openDraft.fantasyDraftLocked)}`
            };
        }

        return {
            league,
            type: 'live' as const,
            message: `${league.name}: round is live`
        };
    });
});

function switchLeague(league: League) {
    leagueStore.setSelectedLeague(league);
}
</script>

<style scoped>
.alert-banner-stack {
    display: flex;
    flex-direction: column;
    gap: var(--space-xs);
    margin-bottom: var(--space-sm);
}

.alert-banner {
    width: 100%;
}

.alert-content {
    display: flex;
    align-items: center;
    gap: var(--space-sm);
}

.alert-icon {
    flex-shrink: 0;
}

.alert-text {
    flex: 1;
    font-size: var(--text-sm);
}

.alert-switch-btn {
    flex-shrink: 0;
}
</style>
