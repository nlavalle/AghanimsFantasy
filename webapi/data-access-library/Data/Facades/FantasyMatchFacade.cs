namespace DataAccessLibrary.Data.Facades;

using DataAccessLibrary.Data;
using DataAccessLibrary.Models.Fantasy;
using DataAccessLibrary.Models.WebApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SteamKit2.GC.Dota.Internal;

public class FantasyMatchFacade
{
    private readonly ILogger<FantasyMatchFacade> _logger;
    private readonly AghanimsFantasyContext _dbContext;

    public FantasyMatchFacade(
        ILogger<FantasyMatchFacade> logger,
        AghanimsFantasyContext dbContext
    )
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<List<(CMsgDOTAMatch, CMsgDOTAMatch.Player)>> GetNotInFantasyMatchPlayers(int takeAmount)
    {
        _logger.LogInformation($"Getting new GcDotaMatches not loaded into Fantasy Match Players");

        var newGcDotaMatchQuery = _dbContext.GcDotaMatches
                .SelectMany(match => match.players,
                (left, right) => new { Match = left, MatchPlayer = right })
                .Where(gcmp =>
                    !_dbContext.FantasyMatchPlayers.Any(fmp => fmp.FantasyMatchId == (long)gcmp.Match.match_id && fmp.Account != null && fmp.Account.Id == gcmp.MatchPlayer.account_id)
                    && (gcmp.Match.match_outcome == EMatchOutcome.k_EMatchOutcome_RadVictory || gcmp.Match.match_outcome == EMatchOutcome.k_EMatchOutcome_DireVictory) // Filter out cancelled games
                )
                .OrderBy(gcm => gcm.Match.match_id)
                .Take(takeAmount);

        _logger.LogDebug($"GetGcDotaMatchesNotInFantasyMatchPlayers SQL Query: {newGcDotaMatchQuery.ToQueryString()}");

        var result = await newGcDotaMatchQuery.ToListAsync();

        return result.Select(rs => (rs.Match, rs.MatchPlayer)).ToList();
    }

    public async Task<List<FantasyMatchPlayer>> GetMatchPlayerNotGcDetailParsed(int takeAmount)
    {
        _logger.LogInformation($"Getting Fantasy Match Players not GC Match Detail Parsed");

        var matchDetailsToParse = _dbContext.FantasyMatchPlayers
                .Where(
                    fm => !fm.GcMetadataPlayerParsed &&
                    _dbContext.GcMatchMetadata.Any(md => md.MatchId == fm.FantasyMatchId))
                .Where(fmp => fmp.Match != null)
                .OrderBy(fmp => fmp.Match!.MatchId)
                .Take(takeAmount);

        _logger.LogDebug($"GetFantasyMatchPlayersNotGcDetailParsed SQL Query: {matchDetailsToParse.ToQueryString()}");

        return await matchDetailsToParse.ToListAsync();
    }

    public async Task<List<FantasyMatch>> GetMatchNotGcDetailParsed(int takeAmount)
    {
        _logger.LogInformation($"Getting Fantasy Matches not GC Match Detail Parsed");

        var matchDetailsToParse = _dbContext.FantasyMatches
                .Where(
                    fm => !fm.MatchDetailParsed && _dbContext.GcDotaMatches.Any(md => (long)md.match_id == fm.MatchId))
                .OrderBy(fm => fm.MatchId)
                .Take(takeAmount);

        _logger.LogDebug($"GetFantasyMatchesNotGcDetailParsed SQL Query: {matchDetailsToParse.ToQueryString()}");

        return await matchDetailsToParse.ToListAsync();
    }

    public async Task<List<FantasyMatch>> GetNotDetailParsed(int takeAmount)
    {
        _logger.LogInformation($"Getting Fantasy Matches not Match Detail Parsed");

        var matchDetailsToParse = _dbContext.FantasyMatches
                .Where(
                    fm => !fm.MatchDetailParsed && _dbContext.MatchDetails.Any(md => md.MatchId == fm.MatchId))
                .OrderBy(fm => fm.MatchId)
                .Take(takeAmount);

        _logger.LogDebug($"GetFantasyMatchesNotDetailParsed SQL Query: {matchDetailsToParse.ToQueryString()}");

        return await matchDetailsToParse.ToListAsync();
    }


    public async Task<List<MatchHistory>> GetNotInFantasyMatches(int takeAmount)
    {
        _logger.LogInformation($"Getting new Match Histories not loaded into Fantasy Match");

        var newMatchHistoryQuery = _dbContext.MatchHistory
                .Include(mh => mh.League)
                .Where(
                    mh => !_dbContext.FantasyMatches.Any(fm => fm.MatchId == mh.MatchId))
                .OrderBy(mh => mh.MatchId)
                .Take(takeAmount);

        _logger.LogDebug($"Match History SQL Query: {newMatchHistoryQuery.ToQueryString()}");

        return await newMatchHistoryQuery.ToListAsync();
    }
}