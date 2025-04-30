using System.Security.Claims;
using DataAccessLibrary.Data;
using DataAccessLibrary.Data.Facades;
using DataAccessLibrary.Data.Identity;
using DataAccessLibrary.Models.Fantasy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace csharp_ef_webapi.Services;
public class FantasyServicePrivateFantasyAdmin
{
    private readonly ILogger<FantasyService> _logger;
    private readonly AghanimsFantasyContext _dbContext;
    private readonly AuthFacade _authFacade;
    private readonly UserManager<AghanimsFantasyUser> _userManager;

    public FantasyServicePrivateFantasyAdmin(
        ILogger<FantasyService> logger,
        AghanimsFantasyContext dbContext,
        AuthFacade authFacade,
        UserManager<AghanimsFantasyUser> userManager
    )
    {
        _logger = logger;
        _dbContext = dbContext;
        _authFacade = authFacade;
        _userManager = userManager;
    }

    public async Task<List<FantasyLeague>> GetPrivateFantasyLeagues(ClaimsPrincipal siteUser)
    {
        AghanimsFantasyUser user = await GetUserFromContext(siteUser);
        var privateFantasyLeagues = await _dbContext.FantasyPrivateLeaguePlayers
            .Include(fplp => fplp.FantasyLeague)
            .Where(fplp => fplp.UserId == user.Id)
            .ToListAsync();

        return privateFantasyLeagues.Where(pfl => pfl.FantasyLeague != null).Select(pfl => pfl.FantasyLeague!).Distinct().ToList();
    }

    public async Task<FantasyPrivateLeaguePlayer> GetFantasyPrivateLeaguePlayerAsync(ClaimsPrincipal siteUser, int privateFantasyPlayerId)
    {
        FantasyPrivateLeaguePlayer? getPrivateFantasyPlayer = await _dbContext.FantasyPrivateLeaguePlayers
            .Include(fplp => fplp.User)
            .FirstOrDefaultAsync(fplp => fplp.Id == privateFantasyPlayerId);

        if (getPrivateFantasyPlayer == null)
        {
            throw new ArgumentException("Private Fantasy Player Not Found");
        }

        AghanimsFantasyUser user = await GetUserFromContext(siteUser);
        if (!await _authFacade.IsUserPrivateFantasyAdminAsync(user.Id, getPrivateFantasyPlayer.FantasyLeagueId))
        {
            throw new UnauthorizedAccessException();
        }

        return getPrivateFantasyPlayer;
    }

    public async Task<List<FantasyLeagueWeight>> GetFantasyLeagueWeightsAsync(ClaimsPrincipal siteUser)
    {
        IEnumerable<FantasyLeague> fantasyLeagues = await GetPrivateFantasyLeagues(siteUser);
        IEnumerable<FantasyLeagueWeight> fantasyLeagueWeights = await _dbContext.FantasyLeagueWeights.ToListAsync();

        return fantasyLeagueWeights.Where(flw => fantasyLeagues.Contains(flw.FantasyLeague)).ToList();
    }

    public async Task UpdateFantasyLeagueWeightAsync(ClaimsPrincipal privateAdminUser, int fantasyLeagueWeightId, FantasyLeagueWeight updateFantasyLeagueWeight)
    {
        AghanimsFantasyUser user = await GetUserFromContext(privateAdminUser);
        if (!await _authFacade.IsUserPrivateFantasyAdminAsync(user.Id, updateFantasyLeagueWeight.FantasyLeagueId))
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

    public async Task<List<FantasyPrivateLeaguePlayer>> GetFantasyPrivateLeaguePlayersAsync(ClaimsPrincipal siteUser, int FantasyLeagueId)
    {
        AghanimsFantasyUser user = await GetUserFromContext(siteUser);
        if (!await _authFacade.IsUserPrivateFantasyAdminAsync(user.Id, FantasyLeagueId))
        {
            throw new UnauthorizedAccessException();
        }

        return await _dbContext.FantasyPrivateLeaguePlayers.Include(fplp => fplp.User).Where(fplp => fplp.FantasyLeagueId == FantasyLeagueId).ToListAsync();
    }

    public async Task AddPrivateFantasyPlayerAsync(ClaimsPrincipal adminUser, FantasyPrivateLeaguePlayer newPrivateFantasyPlayer)
    {
        AghanimsFantasyUser user = await GetUserFromContext(adminUser);
        if (!await _authFacade.IsUserPrivateFantasyAdminAsync(user.Id, newPrivateFantasyPlayer.FantasyLeagueId))
        {
            throw new UnauthorizedAccessException();
        }

        if (newPrivateFantasyPlayer.User == null)
        {
            newPrivateFantasyPlayer.User = await _dbContext.Users.FindAsync(newPrivateFantasyPlayer.UserId);
        }

        await _dbContext.FantasyPrivateLeaguePlayers.AddAsync(newPrivateFantasyPlayer);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdatePrivateFantasyPlayerAsync(ClaimsPrincipal adminUser, int privateFantasyPlayerId, FantasyPrivateLeaguePlayer updatePrivateFantasyPlayer)
    {
        AghanimsFantasyUser user = await GetUserFromContext(adminUser);
        if (!await _authFacade.IsUserPrivateFantasyAdminAsync(user.Id, updatePrivateFantasyPlayer.FantasyLeagueId))
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

    public async Task DeletePrivateFantasyPlayerAsync(ClaimsPrincipal adminUser, int deletePrivateFantasyPlayerId)
    {
        AghanimsFantasyUser user = await GetUserFromContext(adminUser);
        FantasyPrivateLeaguePlayer? deletePrivateFantasyPlayer = await _dbContext.FantasyPrivateLeaguePlayers.FindAsync(deletePrivateFantasyPlayerId);

        if (deletePrivateFantasyPlayer == null)
        {
            throw new ArgumentException("Private Fantasy Player Not Found");
        }

        if (!await _authFacade.IsUserPrivateFantasyAdminAsync(user.Id, deletePrivateFantasyPlayer.FantasyLeagueId))
        {
            throw new UnauthorizedAccessException();
        }

        _dbContext.FantasyPrivateLeaguePlayers.Remove(deletePrivateFantasyPlayer);
        await _dbContext.SaveChangesAsync();
    }

    private async Task<AghanimsFantasyUser> GetUserFromContext(ClaimsPrincipal siteUser)
    {
        return await _userManager.GetUserAsync(siteUser) ?? throw new Exception("Invalid user");
    }
}