using DataAccessLibrary.Models;
using DataAccessLibrary.Models.Discord;
using DataAccessLibrary.Models.Fantasy;

namespace DataAccessLibrary.Data;

public interface IFantasyDraftRepository : IRepository<FantasyDraft, long>
{
    Task<FantasyDraft> AddPlayerPickAsync(FantasyDraft fantasyDraft, FantasyPlayer fantasyPlayerPick);
    Task<FantasyDraft> ClearPositionPick(FantasyDraft fantasyDraft, int PickPosition);
    Task ClearPicksAsync(FantasyDraft fantasyDraft);
    Task<FantasyDraft?> GetByUserFantasyLeague(FantasyLeague fantasyLeague, DiscordUser user);
    Task<FantasyDraftPointTotals?> DraftPointTotalAsync(FantasyDraft fantasyDraft);
    Task<List<FantasyPlayerPointTotals>> FantasyPlayerPointTotalsByFantasyLeagueAsync(FantasyLeague FantasyLeague);
    Task<List<FantasyDraftPointTotals>> FantasyLeaguePointTotalsAsync(FantasyLeague FantasyLeague);
    Task<IEnumerable<FantasyPlayerPoints>> FantasyPlayerPointsAsync(FantasyDraft FantasyDraft, int limit);
}