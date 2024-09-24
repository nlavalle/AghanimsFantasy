namespace DataAccessLibrary.Models.WebApi;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using DataAccessLibrary.Models.ProMetadata;

// https://api.steampowered.com/IDOTA2Match_570/GetMatchDetails/v1?
[Table("dota_match_details")]
public class MatchDetail
{
    [Key]
    [Column("match_id")]
    public long MatchId { get; set; }

    [JsonPropertyName("players")]
    public List<MatchDetailsPlayer> Players { get; set; } = new List<MatchDetailsPlayer>();

    [Column("radiant_win")]
    [JsonPropertyName("radiant_win")]
    public bool RadiantWin { get; set; }

    [Column("duration")]
    [JsonPropertyName("duration")]
    public int Duration { get; set; }

    [Column("pre_game_duration")]
    [JsonPropertyName("pre_game_duration")]
    public int PreGameDuration { get; set; }

    [Column("start_time")]
    [JsonPropertyName("start_time")]
    public long StartTime { get; set; }
    public DateTime StartTimeFormatted
    {
        get
        {
            return DateTimeOffset.FromUnixTimeSeconds(StartTime).DateTime;
        }
    }

    [Column("match_seq_num")]
    [JsonPropertyName("match_seq_num")]
    public long MatchSeqNum { get; set; }

    [Column("tower_status_radiant")]
    [JsonPropertyName("tower_status_radiant")]
    public int TowerStatusRadiant { get; set; }

    [Column("tower_status_dire")]
    [JsonPropertyName("tower_status_dire")]
    public int TowerStatusDire { get; set; }

    [Column("barracks_status_radiant")]
    [JsonPropertyName("barracks_status_radiant")]
    public int BarracksStatusRadiant { get; set; }

    [Column("barracks_status_dire")]
    [JsonPropertyName("barracks_status_dire")]
    public int BarracksStatusDire { get; set; }

    [Column("cluster")]
    [JsonPropertyName("cluster")]
    public int Cluster { get; set; }

    [Column("first_blood_time")]
    [JsonPropertyName("first_blood_time")]
    public int FirstBloodTime { get; set; }

    [Column("lobby_type")]
    [JsonPropertyName("lobby_type")]
    public int LobbyType { get; set; }

    [Column("human_players")]
    [JsonPropertyName("human_players")]
    public int HumanPlayers { get; set; }

    [Column("league_id")]
    [JsonPropertyName("leagueid")]
    public int LeagueId { get; set; }
    public League League { get; set; } = null!;

    [Column("game_mode")]
    [JsonPropertyName("game_mode")]
    public int GameMode { get; set; }

    [Column("flags")]
    [JsonPropertyName("flags")]
    public int Flags { get; set; }

    [Column("engine")]
    [JsonPropertyName("engine")]
    public int Engine { get; set; }

    [Column("radiant_score")]
    [JsonPropertyName("radiant_score")]
    public int RadiantScore { get; set; }

    [Column("dire_score")]
    [JsonPropertyName("dire_score")]
    public int DireScore { get; set; }

    [JsonPropertyName("picks_bans")]
    public List<MatchDetailsPicksBans> PicksBans { get; set; } = new List<MatchDetailsPicksBans>();
}