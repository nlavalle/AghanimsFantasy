#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using csharp_ef_webapi.Models;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscordIdController : ControllerBase
    {
        private readonly AghanimsFantasyContext _context;

        public DiscordIdController(AghanimsFantasyContext context)
        {
            _context = context;
        }

        // GET: api/DiscordId
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiscordIds>>> GetDiscordIds()
        {
            return await _context.DiscordIds.ToListAsync();
        }

        // GET: api/DiscordId/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DiscordIds>> GetDiscordIds(long id)
        {
            var discordIds = await _context.DiscordIds.FindAsync(id);

            if (discordIds == null)
            {
                return NotFound();
            }

            return discordIds;
        }

        // PUT: api/DiscordId/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDiscordIds(long id, DiscordIds discordIds)
        {
            if (id != discordIds.DiscordId)
            {
                return BadRequest();
            }

            _context.Entry(discordIds).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiscordIdsExists(id))
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

        // POST: api/DiscordId
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DiscordIds>> PostDiscordIds(DiscordIds discordIds)
        {
            _context.DiscordIds.Add(discordIds);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDiscordIds", new { id = discordIds.DiscordId }, discordIds);
        }

        // DELETE: api/DiscordId/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiscordIds(long id)
        {
            var discordIds = await _context.DiscordIds.FindAsync(id);
            if (discordIds == null)
            {
                return NotFound();
            }

            _context.DiscordIds.Remove(discordIds);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DiscordIdsExists(long id)
        {
            return _context.DiscordIds.Any(e => e.DiscordId == id);
        }
    }
}
