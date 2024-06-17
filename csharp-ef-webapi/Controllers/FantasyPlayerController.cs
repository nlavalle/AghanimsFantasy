using Microsoft.AspNetCore.Mvc;
using csharp_ef_webapi.Data;
using csharp_ef_webapi.Models.Fantasy;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FantasyPlayerController : ControllerBase
    {
        private readonly DiscordRepository _discordRepository;
        private readonly FantasyRepository _fantasyRepository;

        public FantasyPlayerController(DiscordRepository discordRepository, FantasyRepository fantasyRepository)
        {
            _discordRepository = discordRepository;
            _fantasyRepository = fantasyRepository;
        }

        // GET: api/FantasyPlayer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FantasyPlayer>>> GetFantasyPlayers(int? FantasyLeagueId)
        {
            return Ok(await _fantasyRepository.GetFantasyPlayersAsync(FantasyLeagueId));
        }

        // GET: api/FantasyPlayer/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FantasyPlayer>> GetFantasyPlayer(int FantasyPlayerId)
        {
            var fantasyPlayer = await _fantasyRepository.GetFantasyPlayerAsync(FantasyPlayerId);

            if (fantasyPlayer == null)
            {
                return NotFound();
            }

            return Ok(fantasyPlayer);
        }

        // POST: api/FantasyPlayer
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<FantasyLeague>> PostFantasyPlayer(FantasyPlayer FantasyPlayer)
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


            await _fantasyRepository.AddFantasyPlayerAsync(FantasyPlayer);

            return CreatedAtAction("GetFantasyPlayer", new { id = FantasyPlayer.Id }, FantasyPlayer);
        }

        // DELETE: api/FantasyPlayer/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFantasyPlayer(int FantasyPlayerId)
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


            var fantasyPlayer = await _fantasyRepository.GetFantasyPlayerAsync(FantasyPlayerId);
            if (fantasyPlayer == null)
            {
                return NotFound();
            }

            await _fantasyRepository.DeleteFantasyPlayerAsync(fantasyPlayer);

            return NoContent();
        }
    }
}
