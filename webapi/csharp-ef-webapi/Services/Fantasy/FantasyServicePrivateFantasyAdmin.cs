using DataAccessLibrary.Data;
using DataAccessLibrary.Data.Facades;
using DataAccessLibrary.Models.Discord;
using DataAccessLibrary.Models.Fantasy;
using Microsoft.EntityFrameworkCore;

namespace csharp_ef_webapi.Services;
public class FantasyServicePrivateFantasyAdmin
{
    private readonly ILogger<FantasyService> _logger;
    private readonly AghanimsFantasyContext _dbContext;
    private readonly AuthFacade _authFacade;
    private readonly DiscordFacade _discordFacade;

    public FantasyServicePrivateFantasyAdmin(
        ILogger<FantasyService> logger,
        AghanimsFantasyContext dbContext,
        AuthFacade authFacade,
        DiscordFacade discordFacade
    )
    {
        _logger = logger;
        _dbContext = dbContext;
        _authFacade = authFacade;
        _discordFacade = discordFacade;
    }

    public async Task<bool> IsUserPrivateFantasyAdminAsync(DiscordUser discordUser)
    {
        return await _authFacade.IsUserPrivateFantasyAdminAsync(discordUser.Id);
    }

    public async Task<List<FantasyLeague>> GetPrivateFantasyLeagues(DiscordUser discordUser)
    {
        var privateFantasyLeagues = await _dbContext.FantasyPrivateLeaguePlayers
            .Include(fplp => fplp.FantasyLeague)
            .Where(fplp => fplp.DiscordUserId == discordUser.Id)
            .ToListAsync();

        return privateFantasyLeagues.Where(pfl => pfl.FantasyLeague != null).Select(pfl => pfl.FantasyLeague!).Distinct().ToList();
    }

    public async Task<FantasyPrivateLeaguePlayer> GetFantasyPrivateLeaguePlayerAsync(DiscordUser discordUser, int privateFantasyPlayerId)
    {
        FantasyPrivateLeaguePlayer? getPrivateFantasyPlayer = await _dbContext.FantasyPrivateLeaguePlayers
            .Include(fplp => fplp.DiscordUser)
            .FirstOrDefaultAsync(fplp => fplp.Id == privateFantasyPlayerId);

        if (getPrivateFantasyPlayer == null)
        {
            throw new ArgumentException("Private Fantasy Player Not Found");
        }

        if (!await _authFacade.IsUserPrivateFantasyAdminAsync(discordUser.Id, getPrivateFantasyPlayer.FantasyLeagueId))
        {
            throw new UnauthorizedAccessException();
        }

        return getPrivateFantasyPlayer;
    }

    public async Task<List<FantasyLeagueWeight>> GetFantasyLeagueWeightsAsync(DiscordUser siteUser)
    {
        IEnumerable<FantasyLeague> fantasyLeagues = await GetPrivateFantasyLeagues(siteUser);
        IEnumerable<FantasyLeagueWeight> fantasyLeagueWeights = await _dbContext.FantasyLeagueWeights.ToListAsync();

        return fantasyLeagueWeights.Where(flw => fantasyLeagues.Contains(flw.FantasyLeague)).ToList();
    }

    public async Task UpdateFantasyLeagueWeightAsync(DiscordUser privateAdminUser, int fantasyLeagueWeightId, FantasyLeagueWeight updateFantasyLeagueWeight)
    {
        if (!await _authFacade.IsUserPrivateFantasyAdminAsync(privateAdminUser.Id))
        {
            throw new UnauthorizedAccessException();
        }

        if (fantasyLeagueWeightId != updateFantasyLeagueWeight.Id)
        {
            throw new ArgumentException("League ID to Update League mismatch");
        }

        _dbContext.Entry(updateFantasyLeagueWeight).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<FantasyPrivateLeaguePlayer>> GetFantasyPrivateLeaguePlayersAsync(DiscordUser discordUser, int FantasyLeagueId)
    {
        if (!await _authFacade.IsUserPrivateFantasyAdminAsync(discordUser.Id, FantasyLeagueId))
        {
            throw new UnauthorizedAccessException();
        }

        return await _dbContext.FantasyPrivateLeaguePlayers.Include(fplp => fplp.DiscordUser).Where(fplp => fplp.FantasyLeagueId == FantasyLeagueId).ToListAsync();
    }

    public async Task AddPrivateFantasyPlayerAsync(DiscordUser adminUser, FantasyPrivateLeaguePlayer newPrivateFantasyPlayer)
    {
        if (!await _authFacade.IsUserPrivateFantasyAdminAsync(adminUser.Id, newPrivateFantasyPlayer.FantasyLeagueId))
        {
            throw new UnauthorizedAccessException();
        }

        if (newPrivateFantasyPlayer.DiscordUser == null)
        {
            newPrivateFantasyPlayer.DiscordUser = await _dbContext.DiscordUsers.FindAsync(newPrivateFantasyPlayer.DiscordUserId);
        }

        await _dbContext.FantasyPrivateLeaguePlayers.AddAsync(newPrivateFantasyPlayer);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdatePrivateFantasyPlayerAsync(DiscordUser adminUser, int privateFantasyPlayerId, FantasyPrivateLeaguePlayer updatePrivateFantasyPlayer)
    {
        if (!await _authFacade.IsUserPrivateFantasyAdminAsync(adminUser.Id, updatePrivateFantasyPlayer.FantasyLeagueId))
        {
            throw new UnauthorizedAccessException();
        }

        if (privateFantasyPlayerId != updatePrivateFantasyPlayer.Id)
        {
            throw new ArgumentException("Private Fantasy Player ID to Update PrivateFantasyPlayer mismatch");
        }

        _dbContext.Entry(updatePrivateFantasyPlayer).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeletePrivateFantasyPlayerAsync(DiscordUser adminUser, int deletePrivateFantasyPlayerId)
    {
        FantasyPrivateLeaguePlayer? deletePrivateFantasyPlayer = await _dbContext.FantasyPrivateLeaguePlayers.FindAsync(deletePrivateFantasyPlayerId);

        if (deletePrivateFantasyPlayer == null)
        {
            throw new ArgumentException("Private Fantasy Player Not Found");
        }

        if (!await _authFacade.IsUserPrivateFantasyAdminAsync(adminUser.Id, deletePrivateFantasyPlayer.FantasyLeagueId))
        {
            throw new UnauthorizedAccessException();
        }

        _dbContext.FantasyPrivateLeaguePlayers.Remove(deletePrivateFantasyPlayer);
        await _dbContext.SaveChangesAsync();
    }
}