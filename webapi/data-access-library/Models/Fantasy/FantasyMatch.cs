namespace DataAccessLibrary.Models.Fantasy;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAccessLibrary.Models.ProMetadata;
using Newtonsoft.Json;

[Table("fantasy_match")]
public class FantasyMatch
{
    [Key]
    [Column("match_id")]
    public long MatchId { get; set; }

    [Column("league_id")]
    public required League League { get; set; }

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

    [Column("match_history_parsed")]
    [JsonProperty("match_history_parsed")]
    public bool MatchHistoryParsed { get; set; }

    [Column("match_detail_parsed")]
    [JsonProperty("match_detail_parsed")]
    public bool MatchDetailParsed { get; set; }

    [Column("gc_metadata_parsed")]
    [JsonProperty("gc_metadata_parsed")]
    public bool GcMetadataParsed { get; set; }

    #region MatchHistoryData

    [Column("lobby_type")]
    [JsonProperty("lobby_type")]
    public int LobbyType { get; set; }

    [Column("radiant_team_id")]
    [JsonProperty("radiant_team_id")]
    public Team? RadiantTeam { get; set; }

    [Column("dire_team_id")]
    [JsonProperty("dire_team_id")]
    public Team? DireTeam { get; set; }

    #endregion MatchHistoryData

    [Column("radiant_win")]
    [JsonProperty("radiant_win")]
    public bool? RadiantWin { get; set; }

    [Column("duration")]
    [JsonProperty("duration")]
    public int? Duration { get; set; }

    [Column("pre_game_duration")]
    [JsonProperty("pre_game_duration")]
    public int? PreGameDuration { get; set; }

    [Column("first_blood_time")]
    [JsonProperty("first_blood_time")]
    public int? FirstBloodTime { get; set; }

    [Column("game_mode")]
    [JsonProperty("game_mode")]
    public int? GameMode { get; set; }

    [Column("radiant_score")]
    [JsonProperty("radiant_score")]
    public int? RadiantScore { get; set; }

    [Column("dire_score")]
    [JsonProperty("dire_score")]
    public int? DireScore { get; set; }

    public ICollection<FantasyMatchPlayer> Players { get; set; } = new List<FantasyMatchPlayer>();
}