using DataAccessLibrary.Data;
using DataAccessLibrary.Models.Discord;
using DataAccessLibrary.Models.Fantasy;
using DataAccessLibrary.Models.ProMetadata;

namespace csharp_ef_webapi.Services;
public class FantasyServiceAdmin
{
    private readonly ILogger<FantasyService> _logger;
    private readonly IProMetadataRepository _proMetadataRepository;
    private readonly IFantasyLeagueRepository _fantasyLeagueRepository;
    private readonly IFantasyPlayerRepository _fantasyPlayerRepository;
    private readonly IDiscordRepository _discordRepository;

    public FantasyServiceAdmin(
        ILogger<FantasyService> logger,
        IProMetadataRepository proMetadataRepository,
        IFantasyLeagueRepository fantasyLeagueRepository,
        IFantasyPlayerRepository fantasyPlayerRepository,
        IDiscordRepository discordRepository
    )
    {
        _logger = logger;
        _proMetadataRepository = proMetadataRepository;
        _fantasyLeagueRepository = fantasyLeagueRepository;
        _fantasyPlayerRepository = fantasyPlayerRepository;
        _discordRepository = discordRepository;
    }

    public async Task AddLeagueAsync(DiscordUser adminUser, League addLeague)
    {
        if (!await _discordRepository.IsUserAdminAsync(adminUser.Id))
        {
            throw new UnauthorizedAccessException();
        }

        await _proMetadataRepository.AddLeagueAsync(addLeague);
    }

    public async Task UpdateLeagueAsync(DiscordUser adminUser, int leagueId, League updateLeague)
    {
        if (!await _discordRepository.IsUserAdminAsync(adminUser.Id))
        {
            throw new UnauthorizedAccessException();
        }

        if (leagueId != updateLeague.Id)
        {
            throw new ArgumentException("League ID to Update League mismatch");
        }

        await _proMetadataRepository.UpdateLeagueAsync(updateLeague);
    }

    public async Task DeleteLeagueAsync(DiscordUser adminUser, int deleteLeagueId)
    {
        if (!await _discordRepository.IsUserAdminAsync(adminUser.Id))
        {
            throw new UnauthorizedAccessException();
        }

        League? deleteLeague = await _proMetadataRepository.GetLeagueAsync(deleteLeagueId);

        if (deleteLeague == null)
        {
            throw new ArgumentException("League ID Not Found");
        }

        await _proMetadataRepository.DeleteLeagueAsync(deleteLeague);
    }

    public async Task AddFantasyLeagueAsync(DiscordUser adminUser, FantasyLeague addFantasyLeague)
    {
        if (!await _discordRepository.IsUserAdminAsync(adminUser.Id))
        {
            throw new UnauthorizedAccessException();
        }

        if (addFantasyLeague.League == null)
        {
            addFantasyLeague.League = await _proMetadataRepository.GetLeagueAsync(addFantasyLeague.LeagueId) ?? throw new ArgumentException("Invalid parent League ID");
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

        if (updateFantasyLeague.League == null)
        {
            updateFantasyLeague.League = await _proMetadataRepository.GetLeagueAsync(updateFantasyLeague.LeagueId) ?? throw new ArgumentException("Invalid parent League ID");
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

}