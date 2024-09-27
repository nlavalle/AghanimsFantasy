namespace DataAccessLibrary.Data;

using SteamKit2.GC.Dota.Internal;

public interface IGcDotaMatchRepository : IRepository<CMsgDOTAMatch, ulong>
{
    Task<List<(CMsgDOTAMatch, CMsgDOTAMatch.Player)>> GetNotInFantasyMatchPlayers(int takeAmount);
    Task<List<CMsgDOTAMatch>> GetNotInGcMetadata(int takeAmount);
}