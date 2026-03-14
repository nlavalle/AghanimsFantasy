<template>
    <div class="team-block">
        <div class="team-header">
            <img v-if="team.teamId" :src="`logos/teams_logo_${team.teamId}.png`" class="team-logo" />
            <div v-else class="team-logo-placeholder" />
            <span class="team-name">{{ team.name }}</span>
            <div class="team-divider" />
            <span class="team-rule">MAX 2 PER TEAM</span>
        </div>
        <div class="team-players">
            <div class="available-player" v-for="(player, i) in players" :key="i"
                @click="selectPlayer(player.fantasyPlayer)">
                <PlayerPoolCard :fantasyPlayer="player.fantasyPlayer" :fantasyPoints="player.totalMatchFantasyPoints"
                    :showCost="leagueStore.isDraftOpen(leagueStore.selectedFantasyLeague)"
                    :fantasyPlayerCost="fantasyPlayerCost(player.fantasyPlayerId)"
                    :fantasyPlayerBudget="DRAFT_BUDGET + draftSlotCost(player.fantasyPlayer) - draftCost"
                    :isSelected="selectedPlayerCheck(player.fantasyPlayer.id)"
                    :isDisabled="disabledPlayer(player.fantasyPlayer)"
                    :isDrafted="isDrafted(player.fantasyPlayer.id)" />
            </div>
        </div>
        <!-- overlay without scroll-strategy bricks the page scrolling: https://github.com/vuetifyjs/vuetify/issues/15653 -->
        <v-overlay :model-value="disabledTeam(team.teamId)" class="align-center justify-center disabled-team" contained
            persistent no-click-animation scroll-strategy="none" z-index="2">
            <span>Max 2 players per team</span>
        </v-overlay>
    </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { VOverlay } from 'vuetify/components'
import { fantasyDraftState, DRAFT_BUDGET, type FantasyPlayer, type FantasyPlayerPoints } from '../fantasyDraft'
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
    gap: 8px;
    padding-bottom: 12px;
    width: fit-content;
    border-bottom: 1px solid color-mix(in srgb, var(--sg-divider) 7%, transparent);
}

.team-header {
    display: flex;
    align-items: center;
    gap: 8px;
    height: 32px;
    padding: 0 6px;
    background: color-mix(in srgb, var(--ot-border) 5%, transparent);
    border-left: 3px solid color-mix(in srgb, var(--ot-border) 60%, transparent);
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
}

.team-divider {
    flex: 1;
    height: 1px;
    background: color-mix(in srgb, var(--sg-divider) 10%, transparent);
}

.team-rule {
    font-family: var(--font-body);
    font-size: var(--text-xs);
    font-weight: 400;
    letter-spacing: 1px;
    color: color-mix(in srgb, var(--ot-text-dim) 33%, transparent);
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

.disabled-team {
    pointer-events: none;
    border-radius: 8px;
    font-family: var(--font-body);
    font-size: var(--text-sm);
    font-weight: 600;
    letter-spacing: 1px;
    color: color-mix(in srgb, var(--ot-text-dim) 50%, transparent);
}
</style>
