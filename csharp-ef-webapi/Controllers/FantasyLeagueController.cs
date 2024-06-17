using Microsoft.AspNetCore.Mvc;
using csharp_ef_webapi.Data;
using csharp_ef_webapi.Models.Fantasy;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FantasyLeagueController : ControllerBase
    {
        private readonly DiscordRepository _discordRepository;
        private readonly FantasyRepository _fantasyRepository;

        public FantasyLeagueController(DiscordRepository discordRepository, FantasyRepository fantasyRepository)
        {
            _discordRepository = discordRepository;
            _fantasyRepository = fantasyRepository;
        }

        // GET: api/FantasyLeague
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FantasyLeague>>> GetFantasyLeagues(bool? is_active = null)
        {
            return Ok(await _fantasyRepository.GetFantasyLeaguesAsync(is_active));
        }

        // GET: api/FantasyLeague/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FantasyLeague>> GetFantasyLeague(int id)
        {
            var fantasyLeague = await _fantasyRepository.GetFantasyLeagueAsync(id);

            if (fantasyLeague == null)
            {
                return NotFound();
            }

            return Ok(fantasyLeague);
        }

        // POST: api/FantasyLeague
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<FantasyLeague>> PostFantasyLeague(FantasyLeague fantasyLeague)
        {
            // Admin only operation
            var nameId = HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (nameId == null) return BadRequest("Invalid User Claims please contact admin");

            bool getAccountId = long.TryParse(nameId.Value, out long userDiscordAccountId);
            bool AdminCheck = await _discordRepository.IsUserAdminAsync(userDiscordAccountId);

            if (!AdminCheck)
            {
                return Unauthorized();
            }


            await _fantasyRepository.AddFantasyLeagueAsync(fantasyLeague);

            return CreatedAtAction("GetFantasyLeague", new { id = fantasyLeague.Id }, fantasyLeague);
        }

        // DELETE: api/FantasyLeague/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFantasyLeague(int id)
        {
            // Admin only operation
            var nameId = HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (nameId == null) return BadRequest("Invalid User Claims please contact admin");

            bool getAccountId = long.TryParse(nameId.Value, out long userDiscordAccountId);
            bool AdminCheck = await _discordRepository.IsUserAdminAsync(userDiscordAccountId);

            if (!AdminCheck)
            {
                return Unauthorized();
            }

            var fantasyLeague = await _fantasyRepository.GetFantasyLeagueAsync(id);
            if (fantasyLeague == null)
            {
                return NotFound();
            }

            await _fantasyRepository.DeleteFantasyLeagueAsync(fantasyLeague);

            return NoContent();
        }
    }
}
