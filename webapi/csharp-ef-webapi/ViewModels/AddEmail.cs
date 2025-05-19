using System.Text.Json.Serialization;

namespace csharp_ef_webapi.ViewModels;
public class AddEmail
{
    [JsonPropertyName("email")]
    public required string Email { get; set; }

    [JsonPropertyName("password")]
    public required string Password { get; set; }
}