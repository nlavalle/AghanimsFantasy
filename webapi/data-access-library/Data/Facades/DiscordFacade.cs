namespace DataAccessLibrary.Data.Facades;

using DataAccessLibrary.Data;
using DataAccessLibrary.Data.Identity;
using DataAccessLibrary.Models.Fantasy;
using Microsoft.EntityFrameworkCore;

public class DiscordFacade
{
    private readonly AghanimsFantasyContext _dbContext;

    public DiscordFacade(
        AghanimsFantasyContext dbContext
    )
    {
        _dbContext = dbContext;
    }

    public async Task HistoricalUpdateDiscordFantasyLedgersAsync(long DiscordId, AghanimsFantasyUser user)
    {
        List<FantasyLedger> fantasyLedgers = await _dbContext.FantasyLedger
            .Where(fl => fl.DiscordId == DiscordId)
            .ToListAsync();

        fantasyLedgers.ForEach(fl => fl.UserId = user.Id);

        _dbContext.UpdateRange(fantasyLedgers);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<FantasyMatch>> GetFantasyMatchesNotInDiscordOutboxAsync()
    {
        var outboxMessages = await _dbContext.DiscordOutbox.Where(ob => ob.EventObject == "FantasyMatch" && ob.EventType == "Scored").ToListAsync();
        var fantasyMatches = await _dbContext.FantasyPlayerPointsView
            .Where(fppv => fppv.FantasyMatchPlayerId != null && fppv.FantasyMatchPlayer!.Match != null)
            .Where(fppv => fppv.FantasyMatchPlayer != null && fppv.FantasyMatchPlayer.GcMetadataPlayerParsed)
            .Select(fppv => fppv.FantasyMatchPlayer!.Match!)
            .ToListAsync();

        return fantasyMatches
            .Where(fm => !outboxMessages.Any(obm => obm.ObjectKey == fm.MatchId.ToString()))
            .Where(fm => fm.MatchDetailParsed)
            .Distinct()
            .ToList();
    }
}