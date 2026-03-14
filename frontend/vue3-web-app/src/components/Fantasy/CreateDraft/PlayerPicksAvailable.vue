<template>
    <div class="player-pool">
        <div class="pool-scroll">
            <TeamBlock
                v-for="(team, i) in fantasyTeams" :key="i"
                :team="team"
                :players="fantasyPlayersByTeam(team.teamId)" />
        </div>
    </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { fantasyDraftState, type FantasyPlayerPoints } from '../fantasyDraft'
import TeamBlock from './TeamBlock.vue'

const { fantasyPlayerPointsAvailable } = fantasyDraftState()

const fantasyTeams = computed(() => {
    const teams = fantasyPlayerPointsAvailable.value.map(item => ({
        teamId: item.fantasyPlayer.teamId,
        ...item.fantasyPlayer.team
    }))
    return [...new Map(teams.map(item => [item.teamId, item])).values()]
})

const fantasyPlayersByTeam = (teamId: number) =>
    fantasyPlayerPointsAvailable.value
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
