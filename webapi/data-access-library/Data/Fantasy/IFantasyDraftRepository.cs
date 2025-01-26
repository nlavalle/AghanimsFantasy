using DataAccessLibrary.Models.Discord;
using DataAccessLibrary.Models.Fantasy;

namespace DataAccessLibrary.Data;

public interface IFantasyDraftRepository : IRepository<FantasyDraft, long>
{
    Task<FantasyDraft?> GetByUserFantasyLeague(FantasyLeague fantasyLeague, DiscordUser user);
}