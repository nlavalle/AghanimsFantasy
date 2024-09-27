namespace DataAccessLibrary.Models.Fantasy;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using DataAccessLibrary.Models.ProMetadata;

[Table("fantasy_match")]
public class FantasyMatch
{
    [Key]
    [Column("match_id")]
    public long MatchId { get; set; }

    [ForeignKey("League")]
    [Column("league_id")]
    public required int LeagueId { get; set; }
    public League? League { get; set; }

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

    [Column("match_history_parsed")]
    [JsonPropertyName("match_history_parsed")]
    public bool MatchHistoryParsed { get; set; }

    [Column("match_detail_parsed")]
    [JsonPropertyName("match_detail_parsed")]
    public bool MatchDetailParsed { get; set; }

    [Column("gc_metadata_parsed")]
    [JsonPropertyName("gc_metadata_parsed")]
    public bool GcMetadataParsed { get; set; }

    #region MatchHistoryData

    [Column("lobby_type")]
    [JsonPropertyName("lobby_type")]
    public int LobbyType { get; set; }

    [Column("radiant_team_id")]
    [JsonPropertyName("radiant_team_id")]
    public Team? RadiantTeam { get; set; }

    [Column("dire_team_id")]
    [JsonPropertyName("dire_team_id")]
    public Team? DireTeam { get; set; }

    #endregion MatchHistoryData

    [Column("radiant_win")]
    [JsonPropertyName("radiant_win")]
    public bool? RadiantWin { get; set; }

    [Column("duration")]
    [JsonPropertyName("duration")]
    public int? Duration { get; set; }

    [Column("pre_game_duration")]
    [JsonPropertyName("pre_game_duration")]
    public int? PreGameDuration { get; set; }

    [Column("first_blood_time")]
    [JsonPropertyName("first_blood_time")]
    public int? FirstBloodTime { get; set; }

    [Column("game_mode")]
    [JsonPropertyName("game_mode")]
    public int? GameMode { get; set; }

    [Column("radiant_score")]
    [JsonPropertyName("radiant_score")]
    public int? RadiantScore { get; set; }

    [Column("dire_score")]
    [JsonPropertyName("dire_score")]
    public int? DireScore { get; set; }

    public ICollection<FantasyMatchPlayer> Players { get; set; } = new List<FantasyMatchPlayer>();
}