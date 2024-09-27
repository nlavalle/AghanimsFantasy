namespace DataAccessLibrary.Models.WebApi;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAccessLibrary.Models.ProMetadata;
using System.Text.Json.Serialization;

[Table("dota_match_history")]
public class MatchHistory
{
    [Key]
    [Column("match_id")]
    [JsonPropertyName("match_id")]
    public long MatchId { get; set; }

    [Column("series_id")]
    [JsonPropertyName("series_id")]
    public int SeriesId { get; set; }

    [ForeignKey("League")]
    [Column("league_id")]
    [JsonIgnore]
    public int LeagueId { get; set; }

    [JsonIgnore]
    public League? League { get; set; }

    [Column("series_type")]
    [JsonPropertyName("series_type")]
    public int SeriesType { get; set; }

    [Column("match_seq_num")]
    [JsonPropertyName("match_seq_num")]
    public long MatchSeqNum { get; set; }

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

    [Column("lobby_type")]
    [JsonPropertyName("lobby_type")]
    public int LobbyType { get; set; }

    [Column("radiant_team_id")]
    [JsonPropertyName("radiant_team_id")]
    public long RadiantTeamId { get; set; }

    [Column("dire_team_id")]
    [JsonPropertyName("dire_team_id")]
    public long DireTeamId { get; set; }

    [JsonPropertyName("players")]
    public List<MatchHistoryPlayer> Players { get; set; } = new List<MatchHistoryPlayer>();
}