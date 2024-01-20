#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using csharp_ef_webapi.Data;
using csharp_ef_webapi.Models;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscordCommandsController : ControllerBase
    {
        private readonly AghanimsFantasyContext _context;

        public DiscordCommandsController(AghanimsFantasyContext context)
        {
            _context = context;
        }

        // GET: api/MostGamesPlayed
        [HttpGet("MostGamesPlayed")]
        public async Task<ActionResult<string>> GetMostGamesPlayed()
        {
            var discordIds = await _context.DiscordIds.ToListAsync();
            var validPlayers = await _context.DiscordIds.Select(x => x.AccountId).Distinct().ToListAsync();
            var validGames = await _context.PlayerMatchDetails
                                    .Where(pmd => validPlayers.Contains(pmd.AccountId))
                                    .Join(_context.MatchStatus,
                                    matchDetail => matchDetail.MatchId,
                                    matchStatus => matchStatus.MatchId,
                                    (matchDetail, matchStatus) => new
                                    {
                                        matchDetail,
                                        matchStatus
                                    }
                                    ).Where(x => x.matchStatus.Status == 2 || x.matchStatus.Status == 3)
                                    .OrderByDescending(x => x.matchDetail.MatchId)
                                    .Take(100) // Last 100 games so it's a rolling amount
                                    .GroupBy(x => x.matchDetail.AccountId)
                                    .Select(group => new
                                    {
                                        AccountId = group.Key,
                                        GamesPlayed = group.Count(x => x.matchDetail.MatchId > 0)
                                    })
                                    .ToListAsync();
            var mostGamesPlayedAccount = validGames.Where(x => x.GamesPlayed == validGames.Max(games => games.GamesPlayed)).FirstOrDefault().AccountId;
            var playerName = discordIds.Where(di => di.AccountId == mostGamesPlayedAccount).FirstOrDefault().DiscordName;

            if (playerName == null)
            {
                playerName = "Unmapped Player";
            }
            return Ok(playerName);
        }

        // GET: api/Feederboard
        [HttpGet("Feederboard")]
        public async Task<ActionResult<IEnumerable<Feederboard>>> GetFeederboard()
        {
            List<Feederboard> feederResponse = new List<Feederboard>();
            var discordIds = await _context.DiscordIds.ToListAsync();
            var validPlayers = await _context.DiscordIds.Select(x => x.AccountId).Distinct().ToListAsync();
            foreach (var player in validPlayers)
            {
                int lastNDeaths = _context.PlayerMatchDetails
                                        .Where(pmd => pmd.AccountId == player)
                                        .Join(_context.MatchStatus,
                                        matchDetail => matchDetail.MatchId,
                                        matchStatus => matchStatus.MatchId,
                                        (matchDetail, matchStatus) => new
                                        {
                                            matchDetail,
                                            matchStatus
                                        }
                                        ).Where(x => x.matchStatus.Status == 2 || x.matchStatus.Status == 3)
                                        .OrderByDescending(x => x.matchDetail.MatchId)
                                        .Take(20)
                                        .Sum(x => x.matchDetail.Deaths.HasValue ? (int)x.matchDetail.Deaths : 0);
                var feederRow = new Feederboard
                {
                    Deaths = lastNDeaths,
                    DiscordName = discordIds.Where(di => di.AccountId == player).Select(di => di.DiscordName).FirstOrDefault()
                };
                feederResponse.Add(feederRow);
            }
            feederResponse = feederResponse.OrderByDescending(x => x.Deaths).ToList();
            // Set Ranks
            for (int i = 0; i < feederResponse.Count; i++)
            {
                feederResponse[i].Rank = i + 1;
            }

            return Ok(feederResponse);
        }

        // GET: api/Bromance
        [HttpGet("Bromance")]
        public async Task<ActionResult<IEnumerable<Bromance>>> GetBromance()
        {
            return Ok(await _context.Bromance.ToListAsync());
        }

        // GET: api/BetStreaks
        [HttpGet("BetStreaks")]
        public async Task<ActionResult<IEnumerable<BetStreak>>> GetBetStreaks()
        {
            return Ok(await _context.BetStreaks.ToListAsync());
        }

        // GET: api/MatchStreaks
        [HttpGet("MatchStreaks")]
        public async Task<ActionResult<IEnumerable<MatchStreak>>> GetMatchStreaks()
        {
            return Ok(await _context.MatchStreaks.ToListAsync());
        }
    }
}
