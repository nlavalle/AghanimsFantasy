using System.Text.Json.Serialization;

namespace csharp_ef_webapi.ViewModels;
public class DeletePersonalData
{
    [JsonPropertyName("password")]
    public required string Password { get; set; }
}