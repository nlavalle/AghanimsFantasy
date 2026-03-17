import type { Leaderboard, LeaderboardStats } from '@/types/Leaderboard'

export interface LeagueLeaderboardRound {
    fantasyLeagueId: number
    fantasyDrafts: Leaderboard[]
    allRoundsStats: LeaderboardStats | null
}

export interface LeagueLeaderboard {
    leagueId: number
    rounds: LeagueLeaderboardRound[]
    allRoundsStats: LeaderboardStats | null
}
