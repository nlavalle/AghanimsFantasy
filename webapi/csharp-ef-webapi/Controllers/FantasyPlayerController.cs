using csharp_ef_webapi.Extensions;
using csharp_ef_webapi.Services;
using csharp_ef_webapi.ViewModels;
using DataAccessLibrary.Models.Fantasy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FantasyPlayerController : ControllerBase
    {
        private readonly FantasyService _fantasyService;
        private readonly FantasyServiceAdmin _fantasyServiceAdmin;

        public FantasyPlayerController(
            FantasyService fantasyService,
            FantasyServiceAdmin fantasyServiceAdmin
        )
        {
            _fantasyService = fantasyService;
            _fantasyServiceAdmin = fantasyServiceAdmin;
        }

        // GET: api/FantasyPlayer/5
        [HttpGet("{fantasyPlayerId}")]
        [AuthenticatedETag]
        public async Task<ActionResult<FantasyPlayer>> GetFantasyPlayer(int fantasyPlayerId)
        {
            try
            {
                var fantasyPlayer = await _fantasyService.GetFantasyPlayerAsync(HttpContext.User, fantasyPlayerId);

                if (fantasyPlayer == null)
                {
                    return NotFound();
                }

                return Ok(fantasyPlayer);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/FantasyPlayer/5
        [HttpGet("FantasyLeague/{fantasyLeagueId}")]
        [AuthenticatedETag]
        public async Task<ActionResult<IEnumerable<FantasyPlayerViewModel>>> GetFantasyPlayersByFantasyLeague(int fantasyLeagueId)
        {
            try
            {
                List<FantasyPlayerViewModel> fantasyPlayers = await _fantasyService.GetFantasyPlayerViewModelsAsync(HttpContext.User, fantasyLeagueId);

                if (fantasyPlayers == null || fantasyPlayers.Count == 0)
                {
                    return NotFound();
                }

                return Ok(fantasyPlayers);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/FantasyPlayer
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")] // Admin only operation
        [HttpPost]
        public async Task<ActionResult<FantasyLeague>> PostFantasyPlayer(FantasyPlayer fantasyPlayer)
        {
            try
            {
                await _fantasyServiceAdmin.AddFantasyPlayerAsync(fantasyPlayer);
                return CreatedAtAction("GetFantasyPlayer", new { fantasyPlayerId = fantasyPlayer.Id }, fantasyPlayer);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        // PUT: api/FantasyPlayer/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")] // Admin only operation
        [HttpPut("{fantasyPlayerId}")]
        public async Task<IActionResult> PutFantasyPlayer(int fantasyPlayerId, FantasyPlayer fantasyPlayer)
        {
            try
            {
                await _fantasyServiceAdmin.UpdateFantasyPlayerAsync(fantasyPlayerId, fantasyPlayer);
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

        // PUT: api/FantasyPlayer
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")] // Admin only operation
        [HttpPut]
        public async Task<IActionResult> PutFantasyPlayers(IEnumerable<FantasyPlayer> fantasyPlayers)
        {
            try
            {
                await _fantasyServiceAdmin.UpdateFantasyPlayersAsync(fantasyPlayers);
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

        // DELETE: api/FantasyPlayer/5
        [Authorize]
        [HttpDelete("{fantasyPlayerId}")]
        public async Task<IActionResult> DeleteFantasyPlayer(long fantasyPlayerId)
        {
            try
            {
                await _fantasyServiceAdmin.DeleteFantasyPlayerAsync(fantasyPlayerId);
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
