export interface PrivateFantasyPlayer {
    id: number,
    fantasy_league_id: number,
    discord_user_id: string,
    discord_user: DiscordUser,
    is_admin: boolean,
    fantasy_league_join_date: number
}

export interface DiscordUser {
    id: number,
    username: string
}