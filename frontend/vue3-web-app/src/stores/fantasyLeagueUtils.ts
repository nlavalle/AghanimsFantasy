import type { FantasyLeague } from '@/types/FantasyLeague'
import type { League } from '@/types/League'

export function isDraftOpen(fantasyLeague: FantasyLeague): boolean {
  return new Date() < new Date(fantasyLeague.fantasyDraftLocked * 1000)
}

export function isDraftActive(fantasyLeague: FantasyLeague): boolean {
  return new Date() >= new Date(fantasyLeague.leagueStartTime * 1000) &&
    new Date() <= new Date(fantasyLeague.leagueEndTime * 1000)
}

export function isDraftFinished(fantasyLeague: FantasyLeague): boolean {
  return new Date() > new Date(fantasyLeague.leagueEndTime * 1000)
}

export function isLeagueActive(league: League): boolean {
  return new Date() >= new Date(league.start_timestamp * 1000) &&
    new Date() <= new Date(league.end_timestamp * 1000)
}

export function isLeagueFinished(league: League): boolean {
  return new Date() > new Date(league.end_timestamp * 1000)
}
