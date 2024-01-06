#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using csharp_ef_webapi.Models;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeagueController : ControllerBase
    {
        private readonly AghanimsFantasyContext _dbContext;

        public LeagueController(AghanimsFantasyContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/League
        [HttpGet]
        public async Task<ActionResult<IEnumerable<League>>> GetLeagues(bool? is_active = null)
        {
            return await _dbContext.Leagues
                .Where(l => is_active == null || l.isActive == is_active)
                .ToListAsync();
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

            return league;
        }

        // GET: api/League/5/Match/History
        [HttpGet("{leagueId}/match/history")]
        public async Task<ActionResult<List<MatchHistory>>> GetLeagueMatchHistory(int leagueId)
        {
            var matches = await _dbContext.MatchHistory
                .Where(md => md.LeagueId == leagueId)
                .Include(md => md.Players)
                .ToListAsync();

            if (matches == null || matches.Count == 0)
            {
                return NotFound();
            }

            return matches;
        }

        // GET: api/League/5/Match/Details
        [HttpGet("{leagueId}/match/details")]
        public async Task<ActionResult<List<MatchDetail>>> GetLeagueMatchDetails(int leagueId)
        {
            var matches = await _dbContext.MatchDetails
                .Where(md => md.LeagueId == leagueId)
                .Include(md => md.PicksBans)
                .ToListAsync();

            if (matches == null || matches.Count == 0)
            {
                return NotFound();
            }

            return matches;
        }

        // GET: api/League/5/Match/6/Details
        [HttpGet("{leagueId}/match/{matchId}/details")]
        public async Task<ActionResult<MatchDetail>> GetLeagueMatchIdDetails(int leagueId, long matchId)
        {
            var matches = await _dbContext.MatchDetails
                .Where(md => md.LeagueId == leagueId && md.MatchId == matchId)
                .Include(md => md.PicksBans)
                .Include(md => md.Players).ThenInclude(p => p.AbilityUpgrades)
                .FirstOrDefaultAsync();

            if (matches == null)
            {
                return NotFound();
            }

            return matches;
        }

        // PUT: api/League/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLeague(int id, League league)
        {
            if (id != league.id)
            {
                return BadRequest();
            }

            _dbContext.Entry(league).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeagueExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/League
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<League>> PostLeague(League league)
        {
            _dbContext.Leagues.Add(league);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction("GetLeague", new { id = league.id }, league);
        }

        // DELETE: api/League/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeague(int id)
        {
            var league = await _dbContext.Leagues.FindAsync(id);
            if (league == null)
            {
                return NotFound();
            }

            _dbContext.Leagues.Remove(league);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        private bool LeagueExists(int id)
        {
            return _dbContext.Leagues.Any(e => e.id == id);
        }

        /*
        *** TEAM Actions
        */
        // GET: api/League/Teams
        [HttpGet("teams")]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
        {
            return await _dbContext.Teams
                .ToListAsync();
        }

        /*
        *** HERO Actions
        */
        // GET: api/League/Heroes
        [HttpGet("heroes")]
        public async Task<ActionResult<IEnumerable<Hero>>> GetHeroes()
        {
            return await _dbContext.Heroes
                .ToListAsync();
        }

        /*
        *** PLAYER Actions
        */
        // GET: api/League/5/Players
        [HttpGet("{leagueId}/players")]
        public async Task<ActionResult<IEnumerable<MatchDetailsPlayer>>> GetMatchPlayers(int? leagueId)
        {
            return await _dbContext.MatchDetails
                .Where(md => md.LeagueId == leagueId || leagueId == null)
                .SelectMany(md => md.Players)
                .Where(p => p.LeaverStatus != 1) // Filter out games players left (typically false starts)
                .ToListAsync();
        }

        /*
        *** ACCOUNTS Actions
        */
        // GET: api/League/accounts
        [HttpGet("accounts")]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            return await _dbContext.Accounts
                .ToListAsync();
        }
    }
}
