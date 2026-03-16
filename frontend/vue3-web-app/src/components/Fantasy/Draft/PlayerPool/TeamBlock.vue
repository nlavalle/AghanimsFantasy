<template>
    <div class="team-block">
        <div class="team-header" :class="{ 'team-header--full': disabledTeam(team.teamId) }">
            <img v-if="team.teamId" :src="`logos/teams_logo_${team.teamId}.png`" class="team-logo" />
            <div v-else class="team-logo-placeholder" />
            <span class="team-name">{{ team.name }}</span>
        </div>
        <div class="team-players">
            <div class="available-player" v-for="(player, i) in players" :key="i"
                @click="selectPlayer(player.fantasyPlayer)">
                <PlayerPoolCard :fantasyPlayer="player.fantasyPlayer" :fantasyPoints="player.totalMatchFantasyPoints"
                    :showCost="leagueStore.isDraftOpen(leagueStore.currentFantasyLeague)"
                    :fantasyPlayerCost="fantasyPlayerCost(player.fantasyPlayerId)"
                    :fantasyPlayerBudget="DRAFT_BUDGET + draftSlotCost(player.fantasyPlayer) - draftCost"
                    :isSelected="selectedPlayerCheck(player.fantasyPlayer.id)"
                    :isDisabled="disabledPlayer(player.fantasyPlayer) || disabledTeam(team.teamId)"
                    :isDrafted="isDrafted(player.fantasyPlayer.id)" />
            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { fantasyDraftState, DRAFT_BUDGET, type FantasyPlayer, type FantasyPlayerPoints } from '@/components/Fantasy/fantasyDraft'
import { useFantasyLeagueStore } from '@/stores/fantasyLeague'
import PlayerPoolCard from './PlayerPoolCard.vue'

const props = defineProps<{
    team: { teamId: number; name: string }
    players: FantasyPlayerPoints[]
}>()

const { selectedPlayer, currentDraftSlotSelected, fantasyDraftPicks, disabledPlayer, disabledTeam, totalDraftCost, currentDraftSlotCost } = fantasyDraftState()

const isDrafted = (fantasyPlayerId: number) =>
    fantasyDraftPicks.value.some(dp => dp?.id == fantasyPlayerId)
const leagueStore = useFantasyLeagueStore()

const draftCost = computed(() => totalDraftCost(leagueStore.fantasyPlayersStats))

const draftSlotCost = (fantasyPlayer: FantasyPlayer) =>
    currentDraftSlotCost(leagueStore.fantasyPlayersStats, fantasyPlayer.teamPosition)

const fantasyPlayerCost = (fantasyPlayerId: number) =>
    leagueStore.fantasyPlayersStats.find(fps => fps.fantasy_player.id == fantasyPlayerId)?.cost ?? 0

const selectPlayer = (player: FantasyPlayer) => {
    currentDraftSlotSelected.value = player.teamPosition
    selectedPlayer.value = player
}

const selectedPlayerCheck = (fantasyPlayerId: number) => {
    if (!selectedPlayer.value) return false
    return selectedPlayer.value.id == fantasyPlayerId
}
</script>

<style scoped>
.team-block {
    position: relative;
    display: flex;
    flex-direction: column;
    gap: 0;
    padding-top: 40px;
    /* header height (32px) + gap (8px) */
    padding-bottom: 12px;
    width: fit-content;
    border-bottom: 1px solid color-mix(in srgb, var(--sg-divider) 7%, transparent);
}

.team-header {
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    display: flex;
    align-items: center;
    gap: 8px;
    height: 32px;
    padding: 0 6px;
    background: color-mix(in srgb, var(--ot-border) 5%, transparent);
    border-left: 3px solid color-mix(in srgb, var(--ot-border) 60%, transparent);
    transition: background 0.2s, border-color 0.2s;
    overflow: hidden;
}

.team-header--full {
    background: color-mix(in srgb, var(--rune-purple) 12%, transparent);
    border-left-color: color-mix(in srgb, var(--rune-purple) 60%, transparent);
}

.team-logo {
    width: 18px;
    height: 18px;
    border-radius: 2px;
    object-fit: contain;
}

.team-logo-placeholder {
    width: 18px;
    height: 18px;
    border-radius: 2px;
    background: color-mix(in srgb, var(--sg-border) 8%, transparent);
    border: 1px solid color-mix(in srgb, var(--sg-border) 20%, transparent);
}

.team-name {
    font-family: var(--font-heading);
    font-size: var(--text-sm);
    font-weight: 700;
    color: var(--sg-text);
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
    min-width: 0;
    flex: 1;
}


/* Player row */
.team-players {
    display: flex;
    gap: 6px;
    height: 140px;
    overflow: visible;
}

.available-player {
    width: 110px;
    flex-shrink: 0;
    cursor: pointer;
    transition: transform 0.25s cubic-bezier(0.34, 1.56, 0.64, 1);
    will-change: transform;
    z-index: 1;
}

.available-player :deep(.pool-card) {
    height: 100%;
}

.available-player:hover {
    transform: scale(1.12);
    z-index: 10;
}

.available-player:has(+ .available-player:hover) {
    transform: translateX(-6px);
}

.available-player:hover+.available-player {
    transform: translateX(6px);
}
</style>
