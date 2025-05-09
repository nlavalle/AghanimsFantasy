import type { FantasyDraft } from "@/components/Fantasy/fantasyDraft";

export interface LeaderboardItem {
    id: number,
    userName: string,
    value: number,
    fantasyDraft: FantasyDraft,
    playerPoints: number[]
}