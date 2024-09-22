namespace DataAccessLibrary.Models.WebApi;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAccessLibrary.Models.ProMetadata;
using Newtonsoft.Json;

[Table("dota_match_history")]
public class MatchHistory
{
    [Key]
    [Column("match_id")]
    [JsonProperty("match_id")]
    public long MatchId { get; set; }

    [Column("series_id")]
    [JsonProperty("series_id")]
    public int SeriesId { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public League League { get; set; } = null!;

    [Column("series_type")]
    [JsonProperty("series_type")]
    public int SeriesType { get; set; }

    [Column("match_seq_num")]
    [JsonProperty("match_seq_num")]
    public long MatchSeqNum { get; set; }

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

    [Column("lobby_type")]
    [JsonProperty("lobby_type")]
    public int LobbyType { get; set; }

    [Column("radiant_team_id")]
    [JsonProperty("radiant_team_id")]
    public long RadiantTeamId { get; set; }

    [Column("dire_team_id")]
    [JsonProperty("dire_team_id")]
    public long DireTeamId { get; set; }

    [JsonProperty("players")]
    public List<MatchHistoryPlayer> Players { get; set; } = new List<MatchHistoryPlayer>();
}