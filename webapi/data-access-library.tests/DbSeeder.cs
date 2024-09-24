using DataAccessLibrary.Models.Fantasy;
using DataAccessLibrary.Models.GameCoordinator;
using DataAccessLibrary.Models.ProMetadata;
using DataAccessLibrary.Models.WebApi;

namespace DataAccessLibrary.UnitTests.Data;

public static class DbSeeder
{
    /**
    Helper functions to seed data in the db to make it easier to test scenarios
    **/

    public static League NewLeague(int leagueId)
    {
        return new League
        {
            Id = leagueId,
            Name = $"Test League {leagueId}",
            IsActive = true
        };
    }

    public static Team NewTeam(int TeamId)
    {
        return new Team
        {
            Id = TeamId
        };
    }

    public static Account NewAccount(int AccountId)
    {
        return new Account
        {
            Id = AccountId
        };
    }

    public static FantasyPlayer NewFantasyPlayer(int FantasyLeagueId, long teamId, long accountId)
    {
        return new FantasyPlayer
        {
            Id = Convert.ToInt32(string.Format("{0}{1}", FantasyLeagueId, accountId)),
            TeamId = teamId,
            DotaAccountId = accountId,
            FantasyLeagueId = FantasyLeagueId
        };
    }

    public static MatchHistoryPlayer NewMatchHistoryPlayer(long matchId, Account account)
    {
        return new MatchHistoryPlayer
        {
            Id = Convert.ToInt32(string.Format("{0}{1}", matchId, account.Id)),
            MatchId = matchId,
            AccountId = account.Id,
            PlayerSlot = 0,
            HeroId = 1,
            TeamNumber = 0,
            TeamSlot = 0,
        };
    }

    public static MatchDetailsPlayer NewMatchDetailPlayer(long matchId, Account account)
    {
        return new MatchDetailsPlayer
        {
            Id = Convert.ToInt32(string.Format("{0}{1}", matchId, account.Id)),
            MatchId = matchId,
            AccountId = account.Id,
            AbilityUpgrades = new List<MatchDetailsPlayersAbilityUpgrade>(),
            AghanimsScepter = 0,
            AghanimsShard = 0,
            Assists = 0,
            Backpack0 = 0,
            Backpack1 = 0,
            Backpack2 = 0,
            Deaths = 0,
            Denies = 0,
            Gold = 0,
            GoldPerMin = 0,
            GoldSpent = 0,
            HeroDamage = 0,
            HeroHealing = 0,
            HeroId = 0,
            Item0 = 0,
            Item1 = 0,
            Item2 = 0,
            Item3 = 0,
            Item4 = 0,
            Item5 = 0,
            ItemNeutral = 0,
            Kills = 0,
            LastHits = 0,
            LeaverStatus = 0,
            Level = 0,
            Moonshard = 0,
            Networth = 0,
            PlayerSlot = 0,
            ScaledHeroDamage = 0,
            ScaledHeroHealing = 0,
            ScaledTowerDamage = 0,
            TeamNumber = 0,
            TeamSlot = 0,
            TowerDamage = 0,
            XpPerMin = 0
        };
    }

    public static GcMatchMetadata PopulatedMatchMetadata(int leagueId, int gcMatchId)
    {
        // Full 10 players on 2 teams
        return new GcMatchMetadata
        {
            Id = gcMatchId,
            MatchId = gcMatchId,
            LeagueId = leagueId,
            Teams = [
                new GcMatchMetadataTeam {
                    Players = [
                        new GcMatchMetadataPlayer {
                            Id = Convert.ToInt32(string.Format("{0}{1}", gcMatchId, 1))
                        },
                        new GcMatchMetadataPlayer {
                            Id = Convert.ToInt32(string.Format("{0}{1}", gcMatchId, 2))
                        },
                        new GcMatchMetadataPlayer {
                            Id = Convert.ToInt32(string.Format("{0}{1}", gcMatchId, 3))
                        },
                        new GcMatchMetadataPlayer {
                            Id = Convert.ToInt32(string.Format("{0}{1}", gcMatchId, 4))
                        },
                        new GcMatchMetadataPlayer {
                            Id = Convert.ToInt32(string.Format("{0}{1}", gcMatchId, 5))
                        }
                    ]
                },
                new GcMatchMetadataTeam {
                    Players = [
                        new GcMatchMetadataPlayer {
                            Id = Convert.ToInt32(string.Format("{0}{1}", gcMatchId, 6))
                        },
                        new GcMatchMetadataPlayer {
                            Id = Convert.ToInt32(string.Format("{0}{1}", gcMatchId, 7))
                        },
                        new GcMatchMetadataPlayer {
                            Id = Convert.ToInt32(string.Format("{0}{1}", gcMatchId, 8))
                        },
                        new GcMatchMetadataPlayer {
                            Id = Convert.ToInt32(string.Format("{0}{1}", gcMatchId, 9))
                        },
                        new GcMatchMetadataPlayer {
                            Id = Convert.ToInt32(string.Format("{0}{1}", gcMatchId, 10))
                        }
                    ]
                }
            ]
        };
    }

    public static MatchHistory PopulatedMatchHistory(int leagueId, long matchId, List<Account> players)
    {
        MatchHistory matchHistory = new MatchHistory
        {
            LeagueId = leagueId,
            MatchId = matchId,
            RadiantTeamId = 3,
            DireTeamId = 2,
            LobbyType = 0,
            MatchSeqNum = 0,
            StartTime = new DateTimeOffset(new DateTime(2024, 1, 1)).ToUnixTimeSeconds(),
            SeriesId = 0,
            SeriesType = 0,
        };

        foreach (Account player in players)
        {
            matchHistory.Players.Add(NewMatchHistoryPlayer(matchId, player));
        }

        return matchHistory;
    }

    public static MatchDetail PopulatedMatchDetail(int leagueId, int matchId, List<Account> players)
    {
        MatchDetail matchDetail = new MatchDetail
        {
            MatchId = matchId,
            BarracksStatusDire = 0,
            BarracksStatusRadiant = 0,
            Cluster = 0,
            DireScore = 0,
            Duration = 0,
            Engine = 0,
            FirstBloodTime = 0,
            Flags = 0,
            GameMode = 0,
            HumanPlayers = 10,
            LeagueId = leagueId,
            LobbyType = 0,
            MatchSeqNum = 0,
            PicksBans = new List<MatchDetailsPicksBans>(),
            Players = [],
            PreGameDuration = 0,
            RadiantScore = 0,
            RadiantWin = true,
            StartTime = new DateTimeOffset(new DateTime(2024, 1, 1)).ToUnixTimeSeconds(),
            TowerStatusDire = 0,
            TowerStatusRadiant = 0
        };

        foreach (Account player in players)
        {
            matchDetail.Players.Add(NewMatchDetailPlayer(matchId, player));
        }

        return matchDetail;
    }

    public static League FullPopulatedMatch(int leagueId)
    {
        List<Account> players = new List<Account>();
        List<FantasyPlayer> fantasyPlayers = new List<FantasyPlayer>();
        for (int i = 1; i < 11; i++)
        {
            Account player = NewAccount(i);
            players.Add(player);
            fantasyPlayers.Add(NewFantasyPlayer(leagueId, 1, i));
        };

        League league = new League
        {
            Id = leagueId,
            Name = $"Test League {leagueId}",
            IsActive = true,
            FantasyLeagues = [
            new FantasyLeague
                {
                    Id = leagueId,
                    LeagueId = leagueId,
                    Name = $"test fantasy league {leagueId}",
                    IsActive = true,
                    IsPrivate = false,
                    FantasyDraftLocked = new DateTimeOffset(new DateTime(2024, 1, 1)).ToUnixTimeSeconds(),
                    LeagueStartTime = new DateTimeOffset(new DateTime(2024, 1, 1)).ToUnixTimeSeconds(),
                    LeagueEndTime = new DateTimeOffset(new DateTime(2024, 12, 31)).ToUnixTimeSeconds(),
                    FantasyLeagueWeight = new FantasyLeagueWeight
                    {
                        FantasyLeagueId = leagueId,
                        KillsWeight = 0.03M,
                        DeathsWeight = -0.03M,
                        AssistsWeight = 0.15M,
                        LastHitsWeight = 0.003M,
                        GoldPerMinWeight = 0.002M,
                        XpPerMinWeight = 0.002M
                    },
                    FantasyPlayers = fantasyPlayers
                }
            ],
            MatchHistories = [
                PopulatedMatchHistory(leagueId, leagueId, players)
            ],
            MatchDetails = [
                PopulatedMatchDetail(leagueId, leagueId, players)
            ],
            MatchMetadatas = [
                PopulatedMatchMetadata(leagueId, leagueId)
            ]
        };
        return league;
    }
}