<template>
    <div class="player-pool">
        <div class="pool-header">
            <span class="pool-notice">Max 2 players per team</span>
            <div class="filter-buttons">
                <button class="filter-btn" :class="{ 'filter-btn--active': roleFilter === null }"
                    @click="roleFilter = null">ALL</button>
                <button class="filter-btn" :class="{ 'filter-btn--active': roleFilter === 'role' }"
                    @click="roleFilter = 'role'">ROLE</button>
            </div>
        </div>
        <div class="pool-scroll">
            <TeamBlock v-for="(team, i) in filteredTeams" :key="i" :team="team"
                :players="fantasyPlayersByTeam(team.teamId)" />
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { fantasyDraftState, type FantasyPlayerPoints } from '@/components/Fantasy/fantasyDraft'
import TeamBlock from './TeamBlock.vue'

const { fantasyPlayerPointsAvailable, currentDraftSlotSelected } = fantasyDraftState()

const roleFilter = ref<null | 'role'>(null)

const filteredPlayers = computed(() => {
    if (roleFilter.value === 'role') {
        return fantasyPlayerPointsAvailable.value.filter(
            p => p.fantasyPlayer.teamPosition === currentDraftSlotSelected.value
        )
    }
    return fantasyPlayerPointsAvailable.value
})

const filteredTeams = computed(() => {
    const teams = filteredPlayers.value.map(item => ({
        teamId: item.fantasyPlayer.teamId,
        ...item.fantasyPlayer.team
    }))
    return [...new Map(teams.map(item => [item.teamId, item])).values()]
})

const fantasyPlayersByTeam = (teamId: number) =>
    filteredPlayers.value
        .filter(p => p.fantasyPlayer.teamId == teamId)
        .sort((a: FantasyPlayerPoints, b: FantasyPlayerPoints) =>
            a.fantasyPlayer.teamPosition - b.fantasyPlayer.teamPosition)
</script>

<style scoped>
.player-pool {
    display: flex;
    flex-direction: column;
    flex: 1;
    min-width: 0;
    background: var(--sg-bg);
    overflow: hidden;
}

.pool-header {
    display: flex;
    justify-content: flex-end;
    padding: 10px 16px 0;
}

.filter-buttons {
    display: flex;
    gap: 6px;
}

.pool-notice {
    font-family: var(--font-body);
    font-size: var(--text-xs);
    font-weight: 400;
    letter-spacing: 1px;
    color: var(--ot-text-dim);
    align-self: center;
    margin-right: var(--space-sm);
}

.filter-btn {
    font-family: var(--font-body);
    font-size: var(--text-xs);
    font-weight: 600;
    letter-spacing: 1px;
    padding: 4px 12px;
    border-radius: 999px;
    border: 1px solid color-mix(in srgb, var(--rune-purple) 50%, transparent);
    background: transparent;
    color: color-mix(in srgb, var(--rune-purple-light) 60%, transparent);
    cursor: pointer;
    transition: background 0.15s, color 0.15s, border-color 0.15s, box-shadow 0.15s;
}

.filter-btn:hover {
    border-color: color-mix(in srgb, var(--rune-purple) 80%, transparent);
    color: var(--rune-purple-light);
}

.filter-btn--active {
    background: color-mix(in srgb, var(--rune-purple) 25%, transparent);
    border-color: color-mix(in srgb, var(--rune-purple) 90%, transparent);
    color: var(--rune-purple-light);
    box-shadow:
        0 0 6px color-mix(in srgb, var(--rune-purple) 40%, transparent),
        0 0 14px color-mix(in srgb, var(--rune-purple) 20%, transparent);
}

.pool-scroll {
    display: flex;
    flex-wrap: wrap;
    justify-content: space-evenly;
    gap: 24px;
    padding: 16px;
    overflow-y: auto;
    flex: 1;
    align-content: flex-start;
}
</style>
