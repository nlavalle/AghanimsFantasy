export interface Leaderboard {
    fantasyDraft: {
        id: number,
    },
    isTeam: boolean,
    discordName: string,
    teamId: number,
    draftTotalFantasyPoints: number,
    draftPickOnePoints: number,
    draftPickTwoPoints: number,
    draftPickThreePoints: number,
    draftPickFourPoints: number,
    draftPickFivePoints: number
}

export interface LeaderboardStats {
    totalDrafts: number,
    drafterPercentile: number
}