import type { DotaAccount, DotaTeam } from '@/types/Dota';
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
    fantasyPlayer: FantasyPlayer,
    topHeroes: [{
        hero: {
            id: number,
            name: string
        },
        count: number
    }
    ]
}

export interface DraftPickPlayer {
    fantasyDraftId: number,
    fantasyPlayerId: number,
    fantasyPlayer: FantasyPlayer,
    draftOrder: number
}

const currentDraftSlotSelected = ref<number>(1);

const fantasyDraftPicks = ref<FantasyPlayer[]>([]);

const fantasyPlayersAvailable = ref<FantasyPlayer[]>([]);

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

    const setFantasyPlayers = (fantasyPlayers: FantasyPlayer[]) => {
        fantasyPlayersAvailable.value = fantasyPlayers;
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

    return {
        currentDraftSlotSelected,
        fantasyDraftPicks,
        fantasyPlayersAvailable,
        setFantasyDraftPicks,
        setFantasyPlayers,
        setFantasyPlayer,
        clearFantasyDraftPicks
    }
}