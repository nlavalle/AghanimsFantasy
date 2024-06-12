namespace csharp_ef_webapi.Models.WebApi;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using csharp_ef_webapi.Models.ProMetadata;
using Newtonsoft.Json;

// https://api.steampowered.com/IDOTA2Match_570/GetMatchDetails/v1?
[Table("dota_match_details")]
public class MatchDetail
{
    [Key]
    [Column("match_id")]
    public long MatchId { get; set; }

    [JsonProperty("players")]
    public List<MatchDetailsPlayer> Players { get; set; } = new List<MatchDetailsPlayer>();

    [Column("radiant_win")]
    [JsonProperty("radiant_win")]
    public bool RadiantWin { get; set; }

    [Column("duration")]
    [JsonProperty("duration")]
    public int Duration { get; set; }

    [Column("pre_game_duration")]
    [JsonProperty("pre_game_duration")]
    public int PreGameDuration { get; set; }

    [Column("start_time")]
    [JsonProperty("start_time")]
    public long StartTime { get; set; }
    public DateTime StartTimeFormatted
    {
        get
        {
            return DateTimeOffset.FromUnixTimeSeconds(StartTime).DateTime;
        }
    }

    [Column("match_seq_num")]
    [JsonProperty("match_seq_num")]
    public long MatchSeqNum { get; set; }

    [Column("tower_status_radiant")]
    [JsonProperty("tower_status_radiant")]
    public int TowerStatusRadiant { get; set; }

    [Column("tower_status_dire")]
    [JsonProperty("tower_status_dire")]
    public int TowerStatusDire { get; set; }

    [Column("barracks_status_radiant")]
    [JsonProperty("barracks_status_radiant")]
    public int BarracksStatusRadiant { get; set; }

    [Column("barracks_status_dire")]
    [JsonProperty("barracks_status_dire")]
    public int BarracksStatusDire { get; set; }

    [Column("cluster")]
    [JsonProperty("cluster")]
    public int Cluster { get; set; }

    [Column("first_blood_time")]
    [JsonProperty("first_blood_time")]
    public int FirstBloodTime { get; set; }

    [Column("lobby_type")]
    [JsonProperty("lobby_type")]
    public int LobbyType { get; set; }

    [Column("human_players")]
    [JsonProperty("human_players")]
    public int HumanPlayers { get; set; }

    [Column("league_id")]
    [JsonProperty("leagueid")]
    public int LeagueId { get; set; }
    public League League { get; set; } = null!;

    [Column("game_mode")]
    [JsonProperty("game_mode")]
    public int GameMode { get; set; }

    [Column("flags")]
    [JsonProperty("flags")]
    public int Flags { get; set; }

    [Column("engine")]
    [JsonProperty("engine")]
    public int Engine { get; set; }

    [Column("radiant_score")]
    [JsonProperty("radiant_score")]
    public int RadiantScore { get; set; }

    [Column("dire_score")]
    [JsonProperty("dire_score")]
    public int DireScore { get; set; }

    [JsonProperty("picks_bans")]
    public List<MatchDetailsPicksBans> PicksBans { get; set; } = new List<MatchDetailsPicksBans>();
}