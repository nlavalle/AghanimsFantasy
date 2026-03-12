<template>
    <div class="total-winnings af-panel">
        <div v-if="fantasyLeagueStore.isDraftOpen(fantasyLeagueStore.selectedFantasyLeague)" class="winnings-row">
            <h2>Games haven't started</h2>
        </div>
        <div v-else class="winnings-row">
            <h2>Draft Winnings</h2>
            <ShardSpan :animated="true" :bold="true" :font-size="1.5" :gold-value="totalWinnings.toFixed(0)" />
        </div>
    </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useFantasyLeagueStore } from '@/stores/fantasyLeague';
import type { FantasyPlayer, FantasyPlayerPoints } from '../fantasyDraft';
import ShardSpan from '@/components/Dom/ShardSpan.vue';

const fantasyLeagueStore = useFantasyLeagueStore();

const playerFantasyStatsIndexed = computed(() => {
    return fantasyLeagueStore.fantasyPlayerPoints
        .map((player: FantasyPlayerPoints, index) => ({
            ...player,
            position: index + 1
        }))
        .filter(pfsi => fantasyLeagueStore.selectedFantasyDraftPoints?.fantasyDraft.draftPickPlayers.some(dpp => dpp.fantasyPlayerId == pfsi.fantasyPlayerId))
})

const totalWinnings = computed(() => {
    return Math.max(
        playerFantasyStatsIndexed.value
            .filter(player => player.totalMatches > 0)
            .reduce(function (total, current) {
                return total + getWinnings(current.position, current.fantasyPlayer);
            }, 0),
        0 // Don't let winnings fall below 0
    );
})

const getWinnings = (position: number, fantasyPlayer: FantasyPlayer) => {
    let quintileSize = fantasyLeagueStore.fantasyPlayerPoints.length / 5;
    let quintile = Math.ceil(position / quintileSize);

    let cost = fantasyLeagueStore.fantasyPlayersStats.find(fps => fps.fantasy_player.id == fantasyPlayer.id)?.cost ?? 0;
    let winnings = 300 - 60 * quintile
    return winnings - cost
}

</script>

<style scoped>
/*-------------------- Player Winnings --------------------*/
.total-winnings {
    margin-bottom: var(--space-sm);
    margin-left: var(--space-xs);
    height: max-content;
    flex: 1 0 300px;
}

.winnings-row {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: var(--space-md);
    gap: var(--space-md);
}

.winnings-row h2 {
    white-space: nowrap;
}

.total-winnings h2 {
    font-size: var(--text-lg);
    color: var(--aghanims-fantasy-white);
}
</style>
