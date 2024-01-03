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
    }
}
