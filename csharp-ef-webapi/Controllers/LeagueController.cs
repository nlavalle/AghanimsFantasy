using Microsoft.AspNetCore.Mvc;
using csharp_ef_webapi.Models;
using csharp_ef_webapi.Data;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeagueController : ControllerBase
    {
        private readonly FantasyRepository _service;

        public LeagueController(FantasyRepository service)
        {
            _service = service;
        }

        // GET: api/League
        [HttpGet]
        public async Task<ActionResult<IEnumerable<League>>> GetLeagues(bool? is_active = null)
        {
            return Ok(await _service.GetLeaguesAsync(is_active));
        }

        // GET: api/League/5
        [HttpGet("{id}")]
        public async Task<ActionResult<League>> GetLeague(int id)
        {
            var league = await _service.GetLeagueAsync(id);

            if (league == null)
            {
                return NotFound();
            }

            return Ok(league);
        }

        // GET: api/League/5/Match/History
        [HttpGet("{leagueId}/match/history")]
        public async Task<ActionResult<IEnumerable<MatchHistory>>> GetLeagueMatchHistory(int leagueId)
        {

            var matches = await _service.GetMatchHistoryAsync(leagueId);

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
            var matches = await _service.GetMatchDetailsAsync(leagueId);

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
            var matches = await _service.GetMatchDetailAsync(leagueId, matchId);

            if (matches == null)
            {
                return NotFound();
            }

            return Ok(matches);
        }

        // // PUT: api/League/5
        // // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [HttpPut("{id}")]
        // public async Task<IActionResult> PutLeague(int id, League league)
        // {
        //     if (id != league.id)
        //     {
        //         return BadRequest();
        //     }

        //     _dbContext.Entry(league).State = EntityState.Modified;

        //     try
        //     {
        //         await _dbContext.SaveChangesAsync();
        //     }
        //     catch (DbUpdateConcurrencyException)
        //     {
        //         if (!LeagueExists(id))
        //         {
        //             return NotFound();
        //         }
        //         else
        //         {
        //             throw;
        //         }
        //     }

        //     return NoContent();
        // }

        // // POST: api/League
        // // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [HttpPost]
        // public async Task<ActionResult<League>> PostLeague(League league)
        // {
        //     _dbContext.Leagues.Add(league);
        //     await _dbContext.SaveChangesAsync();

        //     return CreatedAtAction("GetLeague", new { id = league.id }, league);
        // }

        // // DELETE: api/League/5
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteLeague(int id)
        // {
        //     var league = await _dbContext.Leagues.FindAsync(id);
        //     if (league == null)
        //     {
        //         return NotFound();
        //     }

        //     _dbContext.Leagues.Remove(league);
        //     await _dbContext.SaveChangesAsync();

        //     return NoContent();
        // }

        // private bool LeagueExists(int id)
        // {
        //     return _dbContext.Leagues.Any(e => e.id == id);
        // }
    }
}
