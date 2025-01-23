namespace csharp_ef_data_loader.Services;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Reflection;
using Discord.Interactions;

public class DiscordBotService : IHostedService
{
    private readonly ILogger<DiscordBotService> _logger;
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly InteractionService _interactionService;
    IServiceProvider _serviceProvider;

    public DiscordBotService(ILogger<DiscordBotService> logger, DiscordSocketClient discordSocketClient, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;

        _client = discordSocketClient;
        _commands = new CommandService();
        _interactionService = new InteractionService(_client.Rest);

        _logger.LogInformation("Discord Bot Service constructed");
    }

    public async Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Discord Bot Service running.");
        // Discord Bot setup things
        string discordToken = Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN") ?? throw new Exception("Missing Discord Token");

        await _commands.AddModulesAsync(Assembly.GetExecutingAssembly(), _serviceProvider);
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);

        await _client.LoginAsync(TokenType.Bot, discordToken);
        await _client.StartAsync();

        _client.Log += LogAsync;
        _client.Ready += ReadyAsync;
        _client.MessageReceived += MessageReceivedAsync;
        _client.InteractionCreated += InteractionCreatedAsync;
    }

    public async Task StopAsync(CancellationToken stoppingToken)
    {
        if (_client != null)
        {
            await _client.LogoutAsync();
            await _client.StopAsync();
        }
    }

    private Task LogAsync(LogMessage log)
    {
        _logger.LogInformation($"Log {log}");
        return Task.CompletedTask;
    }

    private async Task ReadyAsync()
    {
        _logger.LogInformation($"{_client.CurrentUser} is connected!");
        await _interactionService.RegisterCommandsGloballyAsync();
    }

    private async Task MessageReceivedAsync(SocketMessage message)
    {
        // The bot should never respond to itself or other bots.
        if (message.Author.Id == _client.CurrentUser.Id || message.Author.IsBot)
            return;

        if (message.Content == "!sacrifice")
        {
            await message.Channel.SendMessageAsync("Kali is pleased");
        }
    }

    // private async Task SlashCommandExecutedAsync(SocketSlashCommand command)
    // {
    //     await command.RespondAsync($"You executed {command.Data.Name}");
    // }

    private async Task InteractionCreatedAsync(SocketInteraction interaction)
    {
        try
        {
            var context = new SocketInteractionContext(_client, interaction);
            var result = await _interactionService.ExecuteCommandAsync(context, _serviceProvider);

            if (!result.IsSuccess)
                switch (result.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        // implement
                        break;
                    default:
                        break;
                }
        }
        catch
        {
            if (interaction.Type is InteractionType.ApplicationCommand)
                await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
        }
    }
}