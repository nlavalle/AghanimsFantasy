import type { FantasyDraft } from "@/components/Fantasy/fantasyDraft";

export interface LeaderboardItem {
    id: number,
    isTeam: boolean,
    teamId: number,
    description: string,
    value: number,
    fantasyDraft: FantasyDraft,
    playerPoints: number[]
}