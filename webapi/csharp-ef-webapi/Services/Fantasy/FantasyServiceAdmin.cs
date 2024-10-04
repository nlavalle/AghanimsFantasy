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
    private readonly IFantasyLeagueWeightRepository _fantasyLeagueWeightRepository;
    private readonly IFantasyPlayerRepository _fantasyPlayerRepository;
    private readonly IDiscordRepository _discordRepository;

    public FantasyServiceAdmin(
        ILogger<FantasyService> logger,
        IProMetadataRepository proMetadataRepository,
        IFantasyLeagueRepository fantasyLeagueRepository,
        IFantasyLeagueWeightRepository fantasyLeagueWeightsRepository,
        IFantasyPlayerRepository fantasyPlayerRepository,
        IDiscordRepository discordRepository
    )
    {
        _logger = logger;
        _proMetadataRepository = proMetadataRepository;
        _fantasyLeagueRepository = fantasyLeagueRepository;
        _fantasyLeagueWeightRepository = fantasyLeagueWeightsRepository;
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


    public async Task AddFantasyLeagueWeightAsync(DiscordUser adminUser, FantasyLeagueWeight addFantasyLeagueWeight)
    {
        if (!await _discordRepository.IsUserAdminAsync(adminUser.Id))
        {
            throw new UnauthorizedAccessException();
        }

        await _fantasyLeagueWeightRepository.AddAsync(addFantasyLeagueWeight);
    }

    public async Task UpdateFantasyLeagueWeightAsync(DiscordUser adminUser, int fantasyLeagueWeightId, FantasyLeagueWeight updateFantasyLeagueWeight)
    {
        if (!await _discordRepository.IsUserAdminAsync(adminUser.Id))
        {
            throw new UnauthorizedAccessException();
        }

        if (fantasyLeagueWeightId != updateFantasyLeagueWeight.Id)
        {
            throw new ArgumentException("League ID to Update League mismatch");
        }

        await _fantasyLeagueWeightRepository.UpdateAsync(updateFantasyLeagueWeight);
    }

    public async Task DeleteFantasyLeagueWeightAsync(DiscordUser adminUser, int deleteFantasyLeagueWeightId)
    {
        if (!await _discordRepository.IsUserAdminAsync(adminUser.Id))
        {
            throw new UnauthorizedAccessException();
        }

        FantasyLeagueWeight? deleteFantasyLeagueWeight = await _fantasyLeagueWeightRepository.GetByIdAsync(deleteFantasyLeagueWeightId);

        if (deleteFantasyLeagueWeight == null)
        {
            throw new ArgumentException("League ID Not Found");
        }

        await _fantasyLeagueWeightRepository.DeleteAsync(deleteFantasyLeagueWeight);
    }

    public async Task AddFantasyPlayerAsync(DiscordUser adminUser, FantasyPlayer addFantasyPlayer)
    {
        if (!await _discordRepository.IsUserAdminAsync(adminUser.Id))
        {
            throw new UnauthorizedAccessException();
        }

        await _fantasyPlayerRepository.AddAsync(addFantasyPlayer);
    }

    public async Task UpdateFantasyPlayerAsync(DiscordUser adminUser, long fantasyPlayerId, FantasyPlayer updateFantasyPlayer)
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

    public async Task UpdateFantasyPlayersAsync(DiscordUser adminUser, IEnumerable<FantasyPlayer> updateFantasyPlayers)
    {
        if (!await _discordRepository.IsUserAdminAsync(adminUser.Id))
        {
            throw new UnauthorizedAccessException();
        }

        foreach (FantasyPlayer updateFantasyPlayer in updateFantasyPlayers)
        {
            await _fantasyPlayerRepository.UpdateAsync(updateFantasyPlayer);
        }
    }

    public async Task DeleteFantasyPlayerAsync(DiscordUser adminUser, long deleteFantasyPlayerId)
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

    public async Task AddAccountAsync(DiscordUser adminUser, Account addAccount)
    {
        if (!await _discordRepository.IsUserAdminAsync(adminUser.Id))
        {
            throw new UnauthorizedAccessException();
        }

        await _proMetadataRepository.AddAccountAsync(addAccount);
    }

    public async Task UpdateAccountAsync(DiscordUser adminUser, long accountId, Account updateAccount)
    {
        if (!await _discordRepository.IsUserAdminAsync(adminUser.Id))
        {
            throw new UnauthorizedAccessException();
        }

        if (accountId != updateAccount.Id)
        {
            throw new ArgumentException("Account ID to Update Account mismatch");
        }

        await _proMetadataRepository.UpdateAccountAsync(updateAccount);
    }

    public async Task DeleteAccountAsync(DiscordUser adminUser, long deleteAccountId)
    {
        if (!await _discordRepository.IsUserAdminAsync(adminUser.Id))
        {
            throw new UnauthorizedAccessException();
        }

        Account? deleteAccount = await _proMetadataRepository.GetPlayerAccount(deleteAccountId);

        if (deleteAccount == null)
        {
            throw new ArgumentException("Account Id Not Found");
        }

        await _proMetadataRepository.DeleteAccountAsync(deleteAccount);
    }

    public async Task AddTeamAsync(DiscordUser adminUser, Team addTeam)
    {
        if (!await _discordRepository.IsUserAdminAsync(adminUser.Id))
        {
            throw new UnauthorizedAccessException();
        }

        await _proMetadataRepository.AddTeamAsync(addTeam);
    }

    public async Task UpdateTeamAsync(DiscordUser adminUser, long teamId, Team updateTeam)
    {
        if (!await _discordRepository.IsUserAdminAsync(adminUser.Id))
        {
            throw new UnauthorizedAccessException();
        }

        if (teamId != updateTeam.Id)
        {
            throw new ArgumentException("Team ID to Update Team mismatch");
        }

        await _proMetadataRepository.UpdateTeamAsync(updateTeam);
    }

    public async Task DeleteTeamAsync(DiscordUser adminUser, long deleteTeamId)
    {
        if (!await _discordRepository.IsUserAdminAsync(adminUser.Id))
        {
            throw new UnauthorizedAccessException();
        }

        Team? deleteTeam = await _proMetadataRepository.GetTeamAsync(deleteTeamId);

        if (deleteTeam == null)
        {
            throw new ArgumentException("Team Id Not Found");
        }

        await _proMetadataRepository.DeleteTeamAsync(deleteTeam);
    }
}