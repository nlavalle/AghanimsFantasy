using csharp_ef_webapi.Extensions;
using csharp_ef_webapi.Services;
using csharp_ef_webapi.Utilities;
using DataAccessLibrary.Data;
using DataAccessLibrary.Models;
using DataAccessLibrary.Models.GameCoordinator;
using DataAccessLibrary.Models.ProMetadata;
using DataAccessLibrary.Models.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [OutputCache(Tags = new[] { "league" })]
    public class LeagueController : ControllerBase
    {
        private readonly FantasyService _fantasyService;
        private readonly FantasyServiceAdmin _fantasyServiceAdmin;
        private readonly AghanimsFantasyContext _dbContext;
        private readonly IOutputCacheStore _cache;

        public LeagueController(
            FantasyService fantasyService,
            FantasyServiceAdmin fantasyServiceAdmin,
            AghanimsFantasyContext dbContext,
            IOutputCacheStore cache
        )
        {
            _fantasyService = fantasyService;
            _fantasyServiceAdmin = fantasyServiceAdmin;
            _dbContext = dbContext;
            _cache = cache;
        }

        // GET: api/League
        [HttpGet]
        [ResponseETag]
        public async Task<ActionResult<IEnumerable<League>>> GetLeagues(bool include_inactive = false)
        {
            return Ok(await _dbContext.Leagues.Where(l => include_inactive || l.IsActive).ToListAsync());
        }

        // GET: api/League/Schedule
        [HttpGet("schedule")]
        public async Task<ActionResult<IEnumerable<League>>> GetLeaguesSchedule()
        {
            // Get all the scheduled leagues including the last 2 weeks of scheduled leagues
            return Ok(await _dbContext.Leagues.Where(l => l.IsScheduled && DateTime.UnixEpoch.AddSeconds(l.StartTimestamp) >= DateTime.UtcNow.AddDays(-14)).ToListAsync());
        }

        // GET: api/League/5
        [HttpGet("{id}")]
        public async Task<ActionResult<League>> GetLeague(int id)
        {
            var league = await _dbContext.Leagues.FindAsync(id);

            if (league == null)
            {
                return NotFound();
            }

            return Ok(league);
        }

        // POST: api/League
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")] // Admin only operation
        [HttpPost]
        public async Task<ActionResult<League>> PostLeague(League league)
        {
            try
            {
                await _fantasyServiceAdmin.AddLeagueAsync(league);
                await _cache.EvictByTagAsync("league", default);
                return CreatedAtAction("GetLeague", new { id = league.Id }, league);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        // PUT: api/League/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")] // Admin only operation
        [HttpPut("{leagueId}")]
        public async Task<IActionResult> PutFantasyLeague(int leagueId, League league)
        {
            try
            {
                await _fantasyServiceAdmin.UpdateLeagueAsync(leagueId, league);
                await _cache.EvictByTagAsync("league", default);
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

        // DELETE: api/League/5
        [Authorize(Roles = "Admin")] // Admin only operation
        [HttpDelete("{leagueId}")]
        public async Task<IActionResult> DeleteLeague(int leagueId)
        {
            try
            {
                await _fantasyServiceAdmin.DeleteLeagueAsync(leagueId);
                await _cache.EvictByTagAsync("league", default);
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

        // GET: api/League/5/Match/History
        [HttpGet("{leagueId}/match/history")]
        public async Task<ActionResult<IEnumerable<MatchHistory>>> GetLeagueMatchHistory(int leagueId)
        {
            League? league = await _dbContext.Leagues.FindAsync(leagueId);

            if (league == null)
            {
                return NotFound("League ID not found");
            }

            var matches = await _dbContext.MatchHistory.Where(mh => mh.LeagueId == league.Id).ToListAsync();

            if (matches == null || matches.Count() == 0)
            {
                return NotFound();
            }

            return Ok(matches);
        }

        // GET: api/League/5/Match/Details
        [HttpGet("{leagueId}/match/details")]
        public async Task<ActionResult<List<MatchDetail>>> GetLeagueMatchDetails(int leagueId)
        {
            League? league = await _dbContext.Leagues.FindAsync(leagueId);

            if (league == null)
            {
                return NotFound("League ID not found");
            }

            var matches = await _dbContext.MatchHistory.Where(mh => mh.LeagueId == league.Id).ToListAsync();

            if (matches == null || matches.Count() == 0)
            {
                return NotFound();
            }

            return Ok(matches);
        }

        // GET: api/League/5/Match/6/Details
        [HttpGet("{leagueId}/match/{matchId}/details")]
        public async Task<ActionResult<MatchDetail>> GetLeagueMatchIdDetails(int leagueId, long matchId)
        {
            League? league = await _dbContext.Leagues.FindAsync(leagueId);

            if (league == null)
            {
                return NotFound("League ID not found");
            }

            var matches = await _dbContext.MatchDetails.FindAsync(matchId);

            if (matches == null)
            {
                return NotFound();
            }

            return Ok(matches);
        }

        // GET: api/League/5/Match/Metadata
        [HttpGet("{leagueId}/match/metadata")]
        public async Task<ActionResult<List<GcMatchMetadata>>> GetLeagueMatchMetadata(int leagueId, int pageIndex = 1, int pageSize = 50)
        {
            // Limit pageSize max
            if (pageSize > 100) { pageSize = 100; }

            League? league = await _dbContext.Leagues.FindAsync(leagueId);

            if (league == null)
            {
                return NotFound("League ID not found");
            }

            var matches = await _dbContext.GcMatchMetadata.Where(gmm => gmm.LeagueId == league.Id).ToListAsync();

            if (matches == null || matches.Count() == 0)
            {
                return NotFound();
            }

            // Order matches so most recent show up first, we'll typically want to get highlights from the most recent
            matches = matches.OrderByDescending(m => m.MatchId).ToList();

            var paginatedMatches = PaginatedList<GcMatchMetadata>.Create(matches, pageIndex, pageSize);

            return Ok(paginatedMatches);
        }

        // GET: api/league/5/drafts/points
        [HttpGet("{leagueId}/drafts/points")]
        [AuthenticatedETag]
        public async Task<ActionResult<IEnumerable<FantasyDraftPointTotals>>> GetUserDraftFantasyPoints(int leagueId)
        {
            try
            {
                if (HttpContext.User == null)
                {
                    return Ok(new List<FantasyDraftPointTotals>());
                }

                IEnumerable<FantasyDraftPointTotals> fantasyDraftPointTotals = await _fantasyService.GetFantasyDraftPointTotals(HttpContext.User, leagueId);

                return Ok(fantasyDraftPointTotals);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
