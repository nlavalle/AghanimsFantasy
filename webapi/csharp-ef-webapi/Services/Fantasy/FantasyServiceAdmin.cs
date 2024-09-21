using DataAccessLibrary.Data;
using DataAccessLibrary.Models.Discord;
using DataAccessLibrary.Models.Fantasy;

namespace csharp_ef_webapi.Services;
public class FantasyServiceAdmin
{
    private readonly ILogger<FantasyService> _logger;
    private readonly IFantasyLeagueRepository _fantasyLeagueRepository;
    private readonly IFantasyPlayerRepository _fantasyPlayerRepository;
    private readonly IDiscordRepository _discordRepository;

    public FantasyServiceAdmin(
        ILogger<FantasyService> logger,
        IFantasyLeagueRepository fantasyLeagueRepository,
        IFantasyPlayerRepository fantasyPlayerRepository,
        IDiscordRepository discordRepository
    )
    {
        _logger = logger;
        _fantasyLeagueRepository = fantasyLeagueRepository;
        _fantasyPlayerRepository = fantasyPlayerRepository;
        _discordRepository = discordRepository;

        _logger.LogInformation("Fantasy Service Admin constructed");
    }

    public async Task AddFantasyPlayerAsync(DiscordUser adminUser, FantasyPlayer addFantasyPlayer)
    {
        if (!await _discordRepository.IsUserAdminAsync(adminUser.Id))
        {
            throw new UnauthorizedAccessException();
        }

        await _fantasyPlayerRepository.AddAsync(addFantasyPlayer);
    }

    public async Task UpdateFantasyPlayerAsync(DiscordUser adminUser, int fantasyPlayerId, FantasyPlayer updateFantasyPlayer)
    {
        if (!await _discordRepository.IsUserAdminAsync(adminUser.Id))
        {
            throw new UnauthorizedAccessException();
        }

        if (fantasyPlayerId != updateFantasyPlayer.Id)
        {
            throw new ArgumentException("Fantasy Player ID to Update FantasyPlayer mismatch");
        }

        await _fantasyPlayerRepository.UpdateAsync(updateFantasyPlayer);
    }

    public async Task DeleteFantasyPlayerAsync(DiscordUser adminUser, int deleteFantasyPlayerId)
    {
        if (!await _discordRepository.IsUserAdminAsync(adminUser.Id))
        {
            throw new UnauthorizedAccessException();
        }

        FantasyPlayer? deleteFantasyPlayer = await _fantasyPlayerRepository.GetByIdAsync(deleteFantasyPlayerId);

        if (deleteFantasyPlayer == null)
        {
            throw new ArgumentException("Fantasy Player Id Not Found");
        }

        await _fantasyPlayerRepository.DeleteAsync(deleteFantasyPlayer);
    }

    public async Task AddFantasyLeagueAsync(DiscordUser adminUser, FantasyLeague addFantasyLeague)
    {
        if (!await _discordRepository.IsUserAdminAsync(adminUser.Id))
        {
            throw new UnauthorizedAccessException();
        }

        await _fantasyLeagueRepository.AddAsync(addFantasyLeague);
    }

    public async Task UpdateFantasyLeagueAsync(DiscordUser adminUser, int fantasyLeagueId, FantasyLeague updateFantasyLeague)
    {
        if (!await _discordRepository.IsUserAdminAsync(adminUser.Id))
        {
            throw new UnauthorizedAccessException();
        }

        if (fantasyLeagueId != updateFantasyLeague.Id)
        {
            throw new ArgumentException("Fantasy League ID to Update FantasyLeague mismatch");
        }

        await _fantasyLeagueRepository.UpdateAsync(updateFantasyLeague);
    }

    public async Task DeleteFantasyLeagueAsync(DiscordUser adminUser, int deleteFantasyLeagueId)
    {
        if (!await _discordRepository.IsUserAdminAsync(adminUser.Id))
        {
            throw new UnauthorizedAccessException();
        }

        FantasyLeague? deleteFantasyLeague = await _fantasyLeagueRepository.GetByIdAsync(deleteFantasyLeagueId);

        if (deleteFantasyLeague == null)
        {
            throw new ArgumentException("Fantasy League Id Not Found");
        }

        await _fantasyLeagueRepository.DeleteAsync(deleteFantasyLeague);
    }
}