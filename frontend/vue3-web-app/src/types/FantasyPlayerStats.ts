import type { FantasyPlayer, FantasyPlayerTopHeroes } from "@/components/Fantasy/fantasyDraft";

export interface FantasyPlayerStats {
    fantasy_player: FantasyPlayer,
    cost: number,
    player_stats: Object,
    top_heroes: FantasyPlayerTopHeroes[]
}