namespace csharp_ef_webapi.Models.Fantasy;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using csharp_ef_webapi.Models.ProMetadata;
using Newtonsoft.Json;

[Table("fantasy_match")]
public class FantasyMatch
{
    [Key]
    [Column("match_id")]
    public long MatchId { get; set; }

    [Column("league_id")]
    public int LeagueId { get; set; }

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

    [Column("radiant_team_id")]
    [JsonProperty("radiant_team_id")]
    public Team? RadiantTeam { get; set; }

    [Column("dire_team_id")]
    [JsonProperty("dire_team_id")]
    public Team? DireTeam { get; set; }

    [Column("radiant_win")]
    [JsonProperty("radiant_win")]
    public bool RadiantWin { get; set; }

    [Column("duration")]
    [JsonProperty("duration")]
    public int Duration { get; set; }

    [Column("pre_game_duration")]
    [JsonProperty("pre_game_duration")]
    public int PreGameDuration { get; set; }

    [Column("first_blood_time")]
    [JsonProperty("first_blood_time")]
    public int FirstBloodTime { get; set; }

    [Column("lobby_type")]
    [JsonProperty("lobby_type")]
    public int LobbyType { get; set; }

    [Column("game_mode")]
    [JsonProperty("game_mode")]
    public int GameMode { get; set; }

    [Column("radiant_score")]
    [JsonProperty("radiant_score")]
    public int RadiantScore { get; set; }

    [Column("dire_score")]
    [JsonProperty("dire_score")]
    public int DireScore { get; set; }

}