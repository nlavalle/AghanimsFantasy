using csharp_ef_webapi.Services;
using DataAccessLibrary.Models;
using DataAccessLibrary.Models.Fantasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using csharp_ef_webapi.Extensions;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthenticatedETag]
    public class FantasyLeagueController : ControllerBase
    {
        private readonly FantasyService _fantasyService;
        private readonly FantasyServiceAdmin _fantasyServiceAdmin;

        public FantasyLeagueController(
            FantasyService fantasyService,
            FantasyServiceAdmin fantasyServiceAdmin
        )
        {
            _fantasyService = fantasyService;
            _fantasyServiceAdmin = fantasyServiceAdmin;
        }

        // GET: api/FantasyLeague
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FantasyLeague>>> GetFantasyLeagues()
        {
            try
            {
                return Ok(await _fantasyService.GetAccessibleFantasyLeaguesAsync(HttpContext.User));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/FantasyLeague/5
        [HttpGet("{fantasyLeagueId}")]
        public async Task<ActionResult<FantasyLeague>> GetFantasyLeague(int fantasyLeagueId)
        {
            try
            {
                return Ok(await _fantasyService.GetAccessibleFantasyLeagueAsync(HttpContext.User, fantasyLeagueId));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/fantasyleague/5/players
        [HttpGet("{fantasyLeagueId}/players")]
        public async Task<ActionResult<List<FantasyPlayer>>> GetFantasyPlayers(int fantasyLeagueId)
        {
            try
            {
                return Ok(await _fantasyService.GetFantasyLeaguePlayersAsync(HttpContext.User, fantasyLeagueId));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/fantasyleague/5/players/points
        [HttpGet("{fantasyLeagueId}/players/points")]
        public async Task<ActionResult<List<FantasyPlayerPointTotals>>> GetFantasyPlayersPoints(int fantasyLeagueId)
        {
            try
            {
                return Ok(await _fantasyService.GetFantasyLeaguePlayerPointTotalsAsync(HttpContext.User, fantasyLeagueId));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/fantasyleague/5/players/matches/points
        [HttpGet("{fantasyLeagueId}/players/matches/points")]
        public async Task<ActionResult<List<FantasyPlayerPoints>>> GetFantasyPlayersPointsByMatch(int? fantasyLeagueId, [FromQuery] int? limit)
        {
            if (fantasyLeagueId == null)
            {
                return BadRequest("Please provide a League ID to fetch fantasy player points of");
            }

            limit ??= 10;

            limit = limit > 100 ? 100 : limit;

            try
            {
                return Ok(await _fantasyService.GetFantasyLeaguePlayerPointsByMatchAsync(HttpContext.User, fantasyLeagueId.Value, limit.Value));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/fantasyleague/5/metadata
        [HttpGet("{fantasyLeagueId}/metadata")]
        public async Task<ActionResult<List<MetadataSummary>>> GetFantasyLeagueMatchMetadata(int fantasyLeagueId)
        {
            try
            {
                return Ok(await _fantasyService.GetMetadataSummariesByFantasyLeagueAsync(HttpContext.User, fantasyLeagueId));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/fantasyleague/5/drafters/top10
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

                var top10Drafts = await _fantasyService.GetTop10FantasyDraftsAsync(HttpContext.User, fantasyLeagueId.Value);
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

        // GET: api/fantasyleague/5/drafters/stats
        [Authorize]
        [HttpGet("{fantasyLeagueId}/drafters/stats")]
        public async Task<ActionResult<LeaderboardStats>> GetLeaderboardStats(int? fantasyLeagueId)
        {
            try
            {
                if (fantasyLeagueId == null || !fantasyLeagueId.HasValue)
                {
                    return BadRequest("Please provide a League ID to fetch a draft of");
                }

                LeaderboardStats leaderboardStats = await _fantasyService.GetLeaderboardStatsAsync(HttpContext.User, fantasyLeagueId.Value);
                if (leaderboardStats == null)
                {
                    return Ok(new { });
                }
                return Ok(leaderboardStats);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/fantasyleague/5/highlights/3
        [HttpGet("{fantasyLeagueId}/highlights/{matchCount}")]
        public async Task<ActionResult<List<MatchHighlights>>> GetMatchHighlights(int fantasyLeagueId, int matchCount)
        {
            try
            {
                return Ok(await _fantasyService.GetMatchHighlightsAsync(HttpContext.User, fantasyLeagueId, matchCount));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/FantasyLeague
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")] // Admin only operation
        [HttpPost]
        public async Task<ActionResult<FantasyLeague>> PostFantasyLeague(FantasyLeague fantasyLeague)
        {
            try
            {
                await _fantasyServiceAdmin.AddFantasyLeagueAsync(fantasyLeague);
                return CreatedAtAction("GetFantasyLeague", new { fantasyLeagueId = fantasyLeague.Id }, fantasyLeague);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        // PUT: api/FantasyLeague/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")] // Admin only operation
        [HttpPut("{fantasyLeagueId}")]
        public async Task<IActionResult> PutFantasyLeague(int fantasyLeagueId, FantasyLeague fantasyLeague)
        {
            try
            {
                await _fantasyServiceAdmin.UpdateFantasyLeagueAsync(fantasyLeagueId, fantasyLeague);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/FantasyLeague/5
        [Authorize(Roles = "Admin")] // Admin only operation
        [HttpDelete("{fantasyLeagueId}")]
        public async Task<IActionResult> DeleteFantasyLeague(int fantasyLeagueId)
        {
            try
            {
                await _fantasyServiceAdmin.DeleteFantasyLeagueAsync(fantasyLeagueId);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
