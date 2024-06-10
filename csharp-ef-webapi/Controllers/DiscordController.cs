#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using csharp_ef_webapi.Data;
using csharp_ef_webapi.Models.Discord;

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
        public async Task<ActionResult<IEnumerable<DiscordUser>>> GetDiscordUsers()
        {
            return Ok(await _context.DiscordUsers.ToListAsync());
        }

        // GET: api/DiscordId/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DiscordUser>> GetDiscordUser(long id)
        {
            var discordUser = await _context.DiscordUsers.FindAsync(id);

            if (discordUser == null)
            {
                return NotFound();
            }

            return Ok(discordUser);
        }

        // PUT: api/DiscordId/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDiscordUsers(long id, DiscordUser discordUser)
        {
            if (id != discordUser.Id)
            {
                return BadRequest();
            }

            _context.Entry(discordUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiscordUserExists(id))
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
        public async Task<ActionResult<DiscordUser>> PostDiscordUsers(DiscordUser discordUsers)
        {
            _context.DiscordUsers.Add(discordUsers);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDiscordIds", new { id = discordUsers.Id }, discordUsers);
        }

        // DELETE: api/DiscordId/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiscordUsers(long id)
        {
            var discordUser = await _context.DiscordUsers.FindAsync(id);
            if (discordUser == null)
            {
                return NotFound();
            }

            _context.DiscordUsers.Remove(discordUser);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DiscordUserExists(long id)
        {
            return _context.DiscordUsers.Any(e => e.Id == id);
        }
    }
}
