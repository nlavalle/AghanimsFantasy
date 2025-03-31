using System.Text.Json.Serialization;
using DataAccessLibrary.Models;
using DataAccessLibrary.Models.Fantasy;

namespace csharp_ef_webapi.ViewModels;
public class FantasyPlayerViewModel
{
    [JsonPropertyName("fantasy_player")]
    public required FantasyPlayer fantasyPlayer { get; set; }

    [JsonPropertyName("cost")]
    public required decimal cost { get; set; }

    [JsonPropertyName("player_stats")]
    public FantasyNormalizedAveragesTable? normalizedAverages { get; set; }

    [JsonPropertyName("top_heroes")]
    public List<FantasyPlayerTopHeroes>? topHeroes { get; set; }
}