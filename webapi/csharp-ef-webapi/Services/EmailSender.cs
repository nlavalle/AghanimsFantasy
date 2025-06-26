using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace csharp_ef_webapi.Services;

public class EmailSender : IEmailSender
{
    private readonly ILogger _logger;
    private readonly string _mailgunDomain;
    private readonly string _mailgunKey;
    private readonly HttpClient _httpClient;
    public EmailSender(ILogger<EmailSender> logger, HttpClient httpClient)
    {
        _mailgunKey = Environment.GetEnvironmentVariable("MAILGUN_KEY") ?? throw new Exception("MAILGUN_KEY missing from Env variables");
        _mailgunDomain = Environment.GetEnvironmentVariable("MAILGUN_DOMAIN") ?? throw new Exception("MAILGUN_DOMAIN missing from Env variables");
        _logger = logger;
        _httpClient = httpClient;

        // Configure Http Client
        var base64Auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"api:{_mailgunKey}"));
        _httpClient.BaseAddress = new Uri($"https://api.mailgun.net/v3/{_mailgunDomain}/messages");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64Auth);
    }

    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        if (string.IsNullOrEmpty(_mailgunKey))
        {
            throw new Exception("Null Mailgun Key");
        }

        var content = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("from", $"Aghanim's Fantasy <postmaster@{_mailgunDomain}>"),
            new KeyValuePair<string, string>("to", toEmail),
            new KeyValuePair<string, string>("subject", subject),
            new KeyValuePair<string, string>("html", message)
        ]);

        var response = await _httpClient.PostAsync(
            string.Empty,
            content);

        response.EnsureSuccessStatusCode();
    }
}