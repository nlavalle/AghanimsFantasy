using System.Text.Json.Serialization;

namespace csharp_ef_webapi.ViewModels;
public class ChangeDisplayName
{
    [JsonPropertyName("display_name")]
    public required string DisplayName { get; set; }
}