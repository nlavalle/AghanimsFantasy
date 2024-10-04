export interface Leaderboard {
    fantasyDraft: {
        id: number,
    },
    isTeam: boolean,
    discordName: string,
    teamId: number,
    draftTotalFantasyPoints: number
}

export interface LeaderboardStats {
    totalDrafts: number,
    drafterPercentile: number
}