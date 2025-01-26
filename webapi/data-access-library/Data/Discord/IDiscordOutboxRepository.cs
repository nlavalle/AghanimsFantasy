namespace DataAccessLibrary.Data;

using DataAccessLibrary.Models.Discord;

public interface IDiscordOutboxRepository : IRepository<DiscordOutbox, long>
{
}