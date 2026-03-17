import type { DraftPickPlayer } from '@/components/Fantasy/fantasyDraft'

export interface Leaderboard {
    fantasyDraft: {
        id: number,
        draftPickPlayers: DraftPickPlayer[]
    },
    isTeam: boolean,
    userName: string,
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