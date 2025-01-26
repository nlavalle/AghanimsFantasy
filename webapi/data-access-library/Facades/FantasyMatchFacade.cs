using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
using DataAccessLibrary.Models.Fantasy;
using DataAccessLibrary.Models.WebApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SteamKit2.GC.Dota.Internal;

namespace DataAccessLibrary.Facades;

public class FantasyMatchFacade
{
    private readonly ILogger<FantasyMatchFacade> _logger;
    private readonly IProMetadataRepository _proMetadataRepository;
    private readonly IFantasyViewsRepository _fantasyViewsRepository;
    private readonly IFantasyMatchPlayerRepository _fantasyMatchPlayerRepository;
    private readonly IFantasyMatchRepository _fantasyMatchRepository;
    private readonly IGcDotaMatchRepository _gcDotaMatchRepository;
    private readonly IGcMatchMetadataRepository _gcMatchMetadataRepository;
    private readonly IMatchDetailRepository _matchDetailRepository;
    private readonly IMatchHistoryRepository _matchHistoryRepository;

    public FantasyMatchFacade(
        ILogger<FantasyMatchFacade> logger,
        IProMetadataRepository proMetadataRepository,
        IFantasyViewsRepository fantasyViewsRepository,
        IFantasyMatchPlayerRepository fantasyMatchPlayerRepository,
        IFantasyMatchRepository fantasyMatchRepository,
        IGcDotaMatchRepository gcDotaMatchRepository,
        IGcMatchMetadataRepository gcMatchMetadataRepository,
        IMatchDetailRepository matchDetailRepository,
        IMatchHistoryRepository matchHistoryRepository
    )
    {
        _logger = logger;
        _proMetadataRepository = proMetadataRepository;
        _fantasyViewsRepository = fantasyViewsRepository;
        _fantasyMatchPlayerRepository = fantasyMatchPlayerRepository;
        _fantasyMatchRepository = fantasyMatchRepository;
        _gcDotaMatchRepository = gcDotaMatchRepository;
        _gcMatchMetadataRepository = gcMatchMetadataRepository;
        _matchDetailRepository = matchDetailRepository;
        _matchHistoryRepository = matchHistoryRepository;
    }

    public async Task<List<(CMsgDOTAMatch, CMsgDOTAMatch.Player)>> GetNotInFantasyMatchPlayers(int takeAmount)
    {
        _logger.LogInformation($"Getting new GcDotaMatches not loaded into Fantasy Match Players");

        var newGcDotaMatchQuery = _gcDotaMatchRepository.GetQueryable()
                .SelectMany(match => match.players,
                (left, right) => new { Match = left, MatchPlayer = right })
                .Where(gcmp =>
                    !_fantasyMatchPlayerRepository.GetQueryable().Any(fmp => fmp.FantasyMatchId == (long)gcmp.Match.match_id && fmp.Account != null && fmp.Account.Id == gcmp.MatchPlayer.account_id)
                    && (gcmp.Match.match_outcome == EMatchOutcome.k_EMatchOutcome_RadVictory || gcmp.Match.match_outcome == EMatchOutcome.k_EMatchOutcome_DireVictory) // Filter out cancelled games
                )
                .Take(takeAmount);

        _logger.LogDebug($"GetGcDotaMatchesNotInFantasyMatchPlayers SQL Query: {newGcDotaMatchQuery.ToQueryString()}");

        var result = await newGcDotaMatchQuery.ToListAsync();

        return result.Select(rs => (rs.Match, rs.MatchPlayer)).ToList();
    }

    public async Task<List<FantasyMatchPlayer>> GetMatchPlayerNotGcDetailParsed(int takeAmount)
    {
        _logger.LogInformation($"Getting Fantasy Match Players not GC Match Detail Parsed");

        var matchDetailsToParse = _fantasyMatchPlayerRepository.GetQueryable()
                .Where(
                    fm => !fm.GcMetadataPlayerParsed &&
                    _gcMatchMetadataRepository.GetQueryable().Any(md => md.MatchId == fm.FantasyMatchId))
                .Take(takeAmount);

        _logger.LogDebug($"GetFantasyMatchPlayersNotGcDetailParsed SQL Query: {matchDetailsToParse.ToQueryString()}");

        return await matchDetailsToParse.ToListAsync();
    }

    public async Task<List<FantasyMatch>> GetMatchNotGcDetailParsed(int takeAmount)
    {
        _logger.LogInformation($"Getting Fantasy Matches not GC Match Detail Parsed");

        var matchDetailsToParse = _fantasyMatchRepository.GetQueryable()
                .Where(
                    fm => !fm.MatchDetailParsed && _gcDotaMatchRepository.GetQueryable().Any(md => (long)md.match_id == fm.MatchId))
                .Take(takeAmount);

        _logger.LogDebug($"GetFantasyMatchesNotGcDetailParsed SQL Query: {matchDetailsToParse.ToQueryString()}");

        return await matchDetailsToParse.ToListAsync();
    }

    public async Task<List<FantasyMatch>> GetNotDetailParsed(int takeAmount)
    {
        _logger.LogInformation($"Getting Fantasy Matches not Match Detail Parsed");

        var matchDetailsToParse = _fantasyMatchRepository.GetQueryable()
                .Where(
                    fm => !fm.MatchDetailParsed && _matchDetailRepository.GetQueryable().Any(md => md.MatchId == fm.MatchId))
                .Take(takeAmount);

        _logger.LogDebug($"GetFantasyMatchesNotDetailParsed SQL Query: {matchDetailsToParse.ToQueryString()}");

        return await matchDetailsToParse.ToListAsync();
    }


    public async Task<List<MatchHistory>> GetNotInFantasyMatches(int takeAmount)
    {
        _logger.LogInformation($"Getting new Match Histories not loaded into Fantasy Match");

        var newMatchHistoryQuery = _matchHistoryRepository.GetQueryable()
                .Include(mh => mh.League)
                .Where(
                    mh => !_fantasyMatchRepository.GetQueryable().Any(fm => fm.MatchId == mh.MatchId))
                .OrderBy(mh => mh.MatchId)
                .Take(takeAmount);

        _logger.LogDebug($"Match History SQL Query: {newMatchHistoryQuery.ToQueryString()}");

        return await newMatchHistoryQuery.ToListAsync();
    }
}