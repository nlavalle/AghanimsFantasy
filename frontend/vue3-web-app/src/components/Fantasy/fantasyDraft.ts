import type { DotaAccount, DotaTeam } from '@/types/Dota';
import type { FantasyMatchPlayer } from '@/types/FantasyMatchPlayer';
import { ref } from 'vue';

export interface FantasyDraftPoints {
    fantasyDraft: FantasyDraft,
    isTeam: boolean,
    teamId: number | null,
    discordName: string | null,
    fantasyPlayerPoints: FantasyPlayerPoints[],
    draftPickOnePoints: number,
    draftPickTwoPoints: number,
    draftPickThreePoints: number,
    draftPickFourPoints: number,
    draftPickFivePoints: number,
    draftTotalFantasyPoints: number
}

export interface FantasyPlayerPoints {
    fantasyLeagueId: number,
    fantasyPlayerId: number,
    fantasyPlayer: FantasyPlayer,
    totalMatches: number,
    totalKills: number,
    totalKillsPoints: number,
    totalDeaths: number,
    totalDeathsPoints: number,
    totalAssists: number,
    totalAssistsPoints: number,
    totalLastHits: number,
    totalLastHitsPoints: number,
    avgGoldPerMin: number,
    totalGoldPerMinPoints: number,
    avgXpPerMin: number,
    totalXpPerMinPoints: number,
    totalNetworth: number,
    totalNetworthPoints: number,
    totalHeroDamage: number,
    totalHeroDamagePoints: number,
    totalTowerDamage: number,
    totalTowerDamagePoints: number,
    totalHeroHealing: number,
    totalHeroHealingPoints: number,
    totalGold: number,
    totalGoldPoints: number,
    totalFightScore: number,
    totalFightScorePoints: number,
    totalFarmScore: number,
    totalFarmScorePoints: number,
    totalSupportScore: number,
    totalSupportScorePoints: number,
    totalPushScore: number,
    totalPushScorePoints: number,
    totalHeroXp: number,
    totalHeroXpPoints: number,
    totalCampsStacked: number,
    totalCampsStackedPoints: number,
    totalRampages: number,
    totalRampagesPoints: number,
    totalTripleKills: number,
    totalTripleKillsPoints: number,
    totalAegisSnatched: number,
    totalAegisSnatchedPoints: number,
    totalRapiersPurchased: number,
    totalRapiersPurchasedPoints: number,
    totalCouriersKilled: number,
    totalCouriersKilledPoints: number,
    totalSupportGoldSpent: number,
    totalSupportGoldSpentPoints: number,
    totalObserverWardsPlaced: number,
    totalObserverWardsPlacedPoints: number,
    totalSentryWardsPlaced: number,
    totalSentryWardsPlacedPoints: number,
    totalWardsDewarded: number,
    totalWardsDewardedPoints: number,
    totalStunDuration: number,
    totalStunDurationPoints: number,
    totalMatchFantasyPoints: number
}

export interface FantasyPlayerMatchPoints {
    fantasyLeagueId: number,
    fantasyPlayer: FantasyPlayer,
    fantasyMatchPlayer: FantasyMatchPlayer,
    kills: number,
    killsPoints: number,
    deaths: number,
    deathsPoints: number,
    assists: number,
    assistsPoints: number,
    lastHits: number,
    lastHitsPoints: number,
    goldPerMin: number,
    goldPerMinPoints: number,
    xpPerMin: number,
    xpPerMinPoints: number,
    networth: number,
    networthPoints: number,
    heroDamage: number,
    heroDamagePoints: number,
    towerDamage: number,
    towerDamagePoints: number,
    heroHealing: number,
    heroHealingPoints: number,
    gold: number,
    goldPoints: number,
    fightScore: number,
    fightScorePoints: number,
    farmScore: number,
    farmScorePoints: number,
    supportScore: number,
    supportScorePoints: number,
    pushScore: number,
    pushScorePoints: number,
    heroXp: number,
    heroXpPoints: number,
    campsStacked: number,
    campsStackedPoints: number,
    rampages: number,
    rampagesPoints: number,
    tripleKills: number,
    tripleKillsPoints: number,
    aegisSnatched: number,
    aegisSnatchedPoints: number,
    rapiersPurchased: number,
    rapiersPurchasedPoints: number,
    couriersKilled: number,
    couriersKilledPoints: number,
    supportGoldSpent: number,
    supportGoldSpentPoints: number,
    observerWardsPlaced: number,
    observerWardsPlacedPoints: number,
    sentryWardsPlaced: number,
    sentryWardsPlacedPoints: number,
    wardsDewarded: number,
    wardsDewardedPoints: number,
    stunDuration: number,
    stunDurationPoints: number,
    totalMatchFantasyPoints: number
}

export interface FantasyDraft {
    id: number,
    fantasyLeagueId: number,
    discordAccountId: number,
    draftCreated: Date,
    draftLastUpdated: Date,
    draftPickPlayers: DraftPickPlayer[]
}

export interface FantasyPlayer {
    id: number,
    fantasyLeagueId: number,
    teamId: number,
    team: DotaTeam,
    dotaAccountId: number,
    dotaAccount: DotaAccount,
    teamPosition: number
}

export interface FantasyPlayerTopHeroes {
    fantasyPlayerId: number,
    topHeroes: [{
        hero: {
            id: number,
            name: string
        },
        count: number,
        wins: number,
        losses: number
    }
    ]
}

export interface DraftPickPlayer {
    fantasyDraftId: number,
    fantasyPlayerId: number,
    fantasyPlayer: FantasyPlayer,
    draftOrder: number
}

const selectedPlayer = ref<FantasyPlayer>();
const currentDraftSlotSelected = ref<number>(1);
const fantasyDraftPicks = ref<FantasyPlayer[]>([]);
const fantasyPlayerPointsAvailable = ref<FantasyPlayerPoints[]>([]);

export function serializeFantasyDraftToDraftPick(fantasyDraftPlayer: any) {
    return {
        id: fantasyDraftPlayer.fantasyPlayer?.id,
        name: fantasyDraftPlayer.fantasyPlayer?.dotaAccount.name,
        steamProfilePicture: fantasyDraftPlayer.fantasyPlayer?.dotaAccount.steamProfilePicture,
    } as DotaAccount
}

export function fantasyDraftState() {
    const setFantasyDraftPicks = (fantasyDraftPickPlayers: DraftPickPlayer[]) => {
        fantasyDraftPickPlayers.forEach((draftPick: DraftPickPlayer) => {
            fantasyDraftPicks.value[draftPick.draftOrder] = draftPick.fantasyPlayer
        })
    }

    const setFantasyPlayerPoints = (fantasyPlayerPoints: FantasyPlayerPoints[]) => {
        fantasyPlayerPointsAvailable.value = fantasyPlayerPoints;
    }

    const setFantasyPlayer = (fantasyPlayer: FantasyPlayer) => {
        if (!currentDraftSlotSelected.value || currentDraftSlotSelected.value > 5 || currentDraftSlotSelected.value < 1) {
            currentDraftSlotSelected.value = 1;
        }

        // If we already have this player then return function
        if (fantasyDraftPicks.value.filter(dp => dp.id == fantasyPlayer.id).length > 0) {
            return
        }

        fantasyDraftPicks.value[currentDraftSlotSelected.value] = fantasyPlayer;

        // Increment current draft selected to next slot, roll over if > 5
        currentDraftSlotSelected.value++;
        if (currentDraftSlotSelected.value > 5) {
            currentDraftSlotSelected.value = 1;
        }
    }

    const clearFantasyDraftPicks = () => {
        fantasyDraftPicks.value = [];
    }

    const disabledPlayer = (fantasyPlayer: FantasyPlayer) => {
        // Disable currently selected player
        const pickedPlayer = fantasyDraftPicks.value.filter(picks => picks.id == fantasyPlayer.id).length > 0;

        // Can't draft a player that isn't the valid position of what's currently selected
        const correctDraftSlotRoleSelected = fantasyPlayer.teamPosition == currentDraftSlotSelected.value;

        // Can only draft 2 players from a given team
        const teamCounts = fantasyDraftPicks.value.reduce((acc: any, fp: FantasyPlayer) => {
            acc[fp.teamId] = (acc[fp.teamId] || 0) + 1;
            return acc;
        }, {})
        const maxTeamCheck = fantasyPlayerPointsAvailable.value.filter(player => player.fantasyPlayer.id == fantasyPlayer.id && (teamCounts[player.fantasyPlayer.teamId] ?? 0) < 2).length == 0;
        const minTeamsCheck = new Set(fantasyPlayerPointsAvailable.value.flatMap(player => player.fantasyPlayer.teamId)).size > 2;
        return pickedPlayer || !correctDraftSlotRoleSelected || (maxTeamCheck && minTeamsCheck);
    }

    const disabledTeam = (teamId: any) => {
        const teamCounts = fantasyDraftPicks.value.reduce((acc: any, fp: FantasyPlayer) => {
            acc[fp.teamId] = (acc[fp.teamId] || 0) + 1;
            return acc;
        }, {})
        const maxTeamCheck = (teamCounts[teamId] ?? 0) >= 2;
        const minTeamsCheck = new Set(fantasyPlayerPointsAvailable.value.flatMap(player => player.fantasyPlayer.teamId)).size > 2;
        return maxTeamCheck && minTeamsCheck;
    }

    return {
        selectedPlayer,
        currentDraftSlotSelected,
        fantasyDraftPicks,
        fantasyPlayerPointsAvailable,
        setFantasyDraftPicks,
        setFantasyPlayerPoints,
        setFantasyPlayer,
        clearFantasyDraftPicks,
        disabledPlayer,
        disabledTeam
    }
}