import type { FantasyPlayer } from '@/components/Fantasy/fantasyDraft.ts';

export interface LeagueMetadata {
    fantasyLeagueId: number,
    fantasyPlayerId: number,
    fantasyPlayer: FantasyPlayer,
    matchesPlayed: number,
    kills: number,
    killsAverage: number,
    deaths: number,
    deathsAverage: number,
    assists: number,
    assistsAverage: number,
    lastHits: number,
    lastHitsAverage: number,
    denies: number,
    deniesAverage: number,
    goldPerMin: number,
    goldPerMinAverage: number,
    xpPerMin: number,
    xpPerMinAverage: number,
    supportGoldSpent: number,
    supportGoldSpentAverage: number,
    observerWardsPlaced: number,
    observerWardsPlacedAverage: number,
    sentryWardsPlaced: number,
    sentryWardsPlacedAverage: number,
    wardsDewarded: number,
    wardsDewardedAverage: number,
    campsStacked: number,
    campsStackedAverage: number,
    stunDuration: number,
    stunDurationAverage: number,
    networth: number,
    networthAverage: number,
    heroDamage: number,
    heroDamageAverage: number,
    towerDamage: number,
    towerDamageAverage: number,
    heroHealing: number,
    heroHealingAverage: number,
    gold: number,
    goldAverage: number
}