using DataAccessLibrary.Data;
using DataAccessLibrary.Data.Facades;
using DataAccessLibrary.Models.Discord;
using DataAccessLibrary.Models.Fantasy;
using DataAccessLibrary.Models.ProMetadata;
using Microsoft.EntityFrameworkCore;

namespace csharp_ef_webapi.Services;
public class FantasyServiceAdmin
{
    private readonly ILogger<FantasyService> _logger;
    private readonly AuthFacade _authFacade;
    private readonly AghanimsFantasyContext _dbContext;

    public FantasyServiceAdmin(
        ILogger<FantasyService> logger,
        AuthFacade authFacade,
        AghanimsFantasyContext dbContext
    )
    {
        _logger = logger;
        _authFacade = authFacade;
        _dbContext = dbContext;
    }

    public async Task AddLeagueAsync(League addLeague)
    {
        await _dbContext.Leagues.AddAsync(addLeague);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateLeagueAsync(int leagueId, League updateLeague)
    {
        if (leagueId != updateLeague.Id)
        {
            throw new ArgumentException("League ID to Update League mismatch");
        }

        _dbContext.Entry(updateLeague).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteLeagueAsync(int deleteLeagueId)
    {
        League? deleteLeague = await _dbContext.Leagues.FindAsync(deleteLeagueId);

        if (deleteLeague == null)
        {
            throw new ArgumentException("League ID Not Found");
        }

        _dbContext.Leagues.Remove(deleteLeague);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<FantasyLeague>> GetFantasyLeaguesAsync()
    {
        return await _dbContext.FantasyLeagues.ToListAsync();
    }

    public async Task AddFantasyLeagueAsync(FantasyLeague addFantasyLeague)
    {
        if (addFantasyLeague.League == null)
        {
            addFantasyLeague.League = await _dbContext.Leagues.FindAsync(addFantasyLeague.LeagueId) ?? throw new ArgumentException("Invalid parent League ID");
        }

        await _dbContext.FantasyLeagues.AddAsync(addFantasyLeague);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateFantasyLeagueAsync(int fantasyLeagueId, FantasyLeague updateFantasyLeague)
    {
        if (fantasyLeagueId != updateFantasyLeague.Id)
        {
            throw new ArgumentException("Fantasy League ID to Update FantasyLeague mismatch");
        }

        if (updateFantasyLeague.League == null)
        {
            updateFantasyLeague.League = await _dbContext.Leagues.FindAsync(updateFantasyLeague.LeagueId) ?? throw new ArgumentException("Invalid parent League ID");
        }

        _dbContext.Entry(updateFantasyLeague).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteFantasyLeagueAsync(int deleteFantasyLeagueId)
    {
        FantasyLeague? deleteFantasyLeague = await _dbContext.FantasyLeagues.FindAsync(deleteFantasyLeagueId);

        if (deleteFantasyLeague == null)
        {
            throw new ArgumentException("Fantasy League Id Not Found");
        }

        _dbContext.FantasyLeagues.Remove(deleteFantasyLeague);
        await _dbContext.SaveChangesAsync();
    }


    public async Task AddFantasyLeagueWeightAsync(FantasyLeagueWeight addFantasyLeagueWeight)
    {
        await _dbContext.FantasyLeagueWeights.AddAsync(addFantasyLeagueWeight);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateFantasyLeagueWeightAsync(int fantasyLeagueWeightId, FantasyLeagueWeight updateFantasyLeagueWeight)
    {
        if (fantasyLeagueWeightId != updateFantasyLeagueWeight.Id)
        {
            throw new ArgumentException("League ID to Update League mismatch");
        }

        _dbContext.Entry(updateFantasyLeagueWeight).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteFantasyLeagueWeightAsync(int deleteFantasyLeagueWeightId)
    {
        FantasyLeagueWeight? deleteFantasyLeagueWeight = await _dbContext.FantasyLeagueWeights.FindAsync(deleteFantasyLeagueWeightId);

        if (deleteFantasyLeagueWeight == null)
        {
            throw new ArgumentException("League ID Not Found");
        }

        _dbContext.FantasyLeagueWeights.Remove(deleteFantasyLeagueWeight);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddFantasyPlayerAsync(FantasyPlayer addFantasyPlayer)
    {
        await _dbContext.FantasyPlayers.AddAsync(addFantasyPlayer);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateFantasyPlayerAsync(long fantasyPlayerId, FantasyPlayer updateFantasyPlayer)
    {
        if (fantasyPlayerId != updateFantasyPlayer.Id)
        {
            throw new ArgumentException("Fantasy Player ID to Update FantasyPlayer mismatch");
        }

        _dbContext.Entry(updateFantasyPlayer).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateFantasyPlayersAsync(IEnumerable<FantasyPlayer> updateFantasyPlayers)
    {
        foreach (FantasyPlayer updateFantasyPlayer in updateFantasyPlayers)
        {
            _dbContext.Entry(updateFantasyPlayer).State = EntityState.Modified;
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteFantasyPlayerAsync(long deleteFantasyPlayerId)
    {
        FantasyPlayer? deleteFantasyPlayer = await _dbContext.FantasyPlayers.FindAsync(deleteFantasyPlayerId);

        if (deleteFantasyPlayer == null)
        {
            throw new ArgumentException("Fantasy Player Id Not Found");
        }

        _dbContext.FantasyPlayers.Remove(deleteFantasyPlayer);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddAccountAsync(Account addAccount)
    {
        await _dbContext.Accounts.AddAsync(addAccount);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAccountAsync(long accountId, Account updateAccount)
    {
        if (accountId != updateAccount.Id)
        {
            throw new ArgumentException("Account ID to Update Account mismatch");
        }

        _dbContext.Entry(updateAccount).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAccountAsync(long deleteAccountId)
    {
        Account? deleteAccount = await _dbContext.Accounts.FindAsync(deleteAccountId);

        if (deleteAccount == null)
        {
            throw new ArgumentException("Account Id Not Found");
        }

        _dbContext.Accounts.Remove(deleteAccount);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddTeamAsync(Team addTeam)
    {
        await _dbContext.Teams.AddAsync(addTeam);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateTeamAsync(long teamId, Team updateTeam)
    {
        if (teamId != updateTeam.Id)
        {
            throw new ArgumentException("Team ID to Update Team mismatch");
        }

        _dbContext.Entry(updateTeam).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteTeamAsync(long deleteTeamId)
    {
        Team? deleteTeam = await _dbContext.Teams.FindAsync(deleteTeamId);

        if (deleteTeam == null)
        {
            throw new ArgumentException("Team Id Not Found");
        }

        _dbContext.Teams.Remove(deleteTeam);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddFantasyPlayersByTeam(long getTeamId, int newFantasyLeagueId)
    {
        var mostRecentFantasyLeague = await _dbContext.FantasyPlayers
            .Where(fp => fp.TeamId == getTeamId)
            .OrderByDescending(fp => fp.FantasyLeagueId)
            .Select(fp => fp.FantasyLeagueId)
            .FirstOrDefaultAsync();
        if (mostRecentFantasyLeague != 0)
        {
            var lastLeagueFantasyPlayers = await _dbContext.FantasyPlayers
               .Where(fp => fp.TeamId == getTeamId && fp.FantasyLeagueId == mostRecentFantasyLeague)
               .OrderBy(fp => fp.TeamPosition)
               .ToListAsync();

            foreach (FantasyPlayer recentFantasyPlayer in lastLeagueFantasyPlayers)
            {
                FantasyPlayer newFantasyPlayer = new FantasyPlayer()
                {
                    FantasyLeagueId = newFantasyLeagueId,
                    TeamId = recentFantasyPlayer.TeamId,
                    DotaAccountId = recentFantasyPlayer.DotaAccountId,
                    TeamPosition = recentFantasyPlayer.TeamPosition
                };
                await _dbContext.FantasyPlayers.AddAsync(newFantasyPlayer);
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}