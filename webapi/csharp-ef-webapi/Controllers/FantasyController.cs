using DataAccessLibrary.Models;
using DataAccessLibrary.Models.Fantasy;
using DataAccessLibrary.Models.GameCoordinator;
using csharp_ef_webapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DataAccessLibrary.Models.Discord;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FantasyController : ControllerBase
    {
        private readonly FantasyService _fantasyService;
        private readonly DiscordWebApiService _discordWebApiService;
        public FantasyController(
            FantasyService fantasyService,
            DiscordWebApiService discordWebApiService
        )
        {
            _fantasyService = fantasyService;
            _discordWebApiService = discordWebApiService;
        }

        // GET: api/fantasy/leagues
        [HttpGet("leagues")]
        public async Task<ActionResult<IEnumerable<FantasyLeague>>> GetFantasyLeagues()
        {
            try
            {
                DiscordUser? discordUser = await _discordWebApiService.LookupHttpContextUser(HttpContext);

                return Ok(await _fantasyService.GetAccessibleFantasyLeaguesAsync(discordUser));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/fantasy/5/players
        [HttpGet("{fantasyLeagueId}/players")]
        public async Task<ActionResult<List<FantasyPlayer>>> GetFantasyPlayers(int fantasyLeagueId)
        {
            try
            {
                DiscordUser? discordUser = await _discordWebApiService.LookupHttpContextUser(HttpContext);

                return Ok(await _fantasyService.GetFantasyLeaguePlayersAsync(discordUser, fantasyLeagueId));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/fantasy/5/players/points
        [HttpGet("{fantasyLeagueId}/players/points")]
        public async Task<ActionResult<List<FantasyPlayerPointTotals>>> GetFantasyPlayersPoints(int fantasyLeagueId)
        {
            try
            {
                DiscordUser? discordUser = await _discordWebApiService.LookupHttpContextUser(HttpContext);

                return Ok(await _fantasyService.GetFantasyLeaguePlayerPointTotalsAsync(discordUser, fantasyLeagueId));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/fantasy/5/players/matches/points
        [HttpGet("{fantasyLeagueId}/players/matches/points")]
        public async Task<ActionResult<List<FantasyPlayerPoints>>> GetFantasyPlayersPointsByMatch(int? fantasyLeagueId, [FromQuery] int? limit)
        {
            if (fantasyLeagueId == null)
            {
                return BadRequest("Please provide a League ID to fetch fantasy player points of");
            }

            limit = limit ?? 10;

            limit = limit > 100 ? 100 : limit;

            try
            {
                DiscordUser? discordUser = await _discordWebApiService.LookupHttpContextUser(HttpContext);

                return Ok(await _fantasyService.GetFantasyLeaguePlayerPointsByMatchAsync(discordUser, fantasyLeagueId.Value, limit.Value));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/fantasy/5/metadata
        [HttpGet("{fantasyLeagueId}/metadata")]
        public async Task<ActionResult<List<MetadataSummary>>> GetFantasyLeagueMatchMetadata(int fantasyLeagueId)
        {
            try
            {
                DiscordUser? discordUser = await _discordWebApiService.LookupHttpContextUser(HttpContext);

                return Ok(await _fantasyService.GetMetadataSummariesByFantasyLeagueAsync(discordUser, fantasyLeagueId));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/fantasy/5/drafters/top10
        [Authorize]
        [HttpGet("{fantasyLeagueId}/drafters/top10")]
        public async Task<IActionResult> GetTop10FantasyPoints(int? fantasyLeagueId)
        {
            try
            {
                if (fantasyLeagueId == null || !fantasyLeagueId.HasValue)
                {
                    return BadRequest("Please provide a League ID to fetch a draft of");
                }

                DiscordUser? discordUser = await _discordWebApiService.LookupHttpContextUser(HttpContext);

                if (discordUser == null)
                {
                    return NotFound();
                }

                var top10Drafts = await _fantasyService.GetTop10FantasyDraftsAsync(discordUser!, fantasyLeagueId.Value);
                if (top10Drafts == null)
                {
                    return Ok(new { });
                }
                return Ok(top10Drafts);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/fantasy/5/highlights/3
        [HttpGet("{fantasyLeagueId}/highlights/{matchCount}")]
        public async Task<ActionResult<List<MatchHighlights>>> GetMatchHighlights(int fantasyLeagueId, int matchCount)
        {
            try
            {
                DiscordUser? discordUser = await _discordWebApiService.LookupHttpContextUser(HttpContext);

                return Ok(await _fantasyService.GetMatchHighlightsAsync(discordUser, fantasyLeagueId, matchCount));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
