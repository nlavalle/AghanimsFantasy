using Microsoft.AspNetCore.Mvc;
using DataAccessLibrary.Models.Fantasy;
using Microsoft.AspNetCore.Authorization;
using csharp_ef_webapi.Services;
using DataAccessLibrary.Models.Discord;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FantasyPlayerController : ControllerBase
    {
        private readonly DiscordWebApiService _discordWebApiService;
        private readonly FantasyService _fantasyService;
        private readonly FantasyServiceAdmin _fantasyServiceAdmin;

        public FantasyPlayerController(
            DiscordWebApiService discordWebApiService,
            FantasyService fantasyService,
            FantasyServiceAdmin fantasyServiceAdmin
        )
        {
            _discordWebApiService = discordWebApiService;
            _fantasyService = fantasyService;
            _fantasyServiceAdmin = fantasyServiceAdmin;
        }

        // GET: api/FantasyPlayer/5
        [HttpGet("{fantasyPlayerId}")]
        public async Task<ActionResult<FantasyPlayer>> GetFantasyPlayer(int fantasyPlayerId)
        {
            try
            {
                DiscordUser? discordUser = await _discordWebApiService.LookupHttpContextUser(HttpContext);

                var fantasyPlayer = await _fantasyService.GetFantasyPlayerAsync(discordUser, fantasyPlayerId);

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

        // POST: api/FantasyPlayer
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<FantasyLeague>> PostFantasyPlayer(FantasyPlayer fantasyPlayer)
        {
            // Admin only operation
            if (!await _discordWebApiService.CheckAdminUser(HttpContext))
            {
                return Unauthorized();
            }

            try
            {
                DiscordUser? discordUser = await _discordWebApiService.LookupHttpContextUser(HttpContext);
                if (discordUser == null)
                {
                    return Unauthorized();
                }

                await _fantasyServiceAdmin.AddFantasyPlayerAsync(discordUser, fantasyPlayer);
                return CreatedAtAction("GetFantasyPlayer", new { fantasyPlayerId = fantasyPlayer.Id }, fantasyPlayer);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        // PUT: api/FantasyPlayer/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{fantasyPlayerId}")]
        public async Task<IActionResult> PutFantasyPlayer(int fantasyPlayerId, FantasyPlayer fantasyPlayer)
        {
            // Admin only operation
            if (!await _discordWebApiService.CheckAdminUser(HttpContext))
            {
                return Unauthorized();
            }

            try
            {
                DiscordUser? discordUser = await _discordWebApiService.LookupHttpContextUser(HttpContext);
                if (discordUser == null)
                {
                    return Unauthorized();
                }

                await _fantasyServiceAdmin.UpdateFantasyPlayerAsync(discordUser, fantasyPlayerId, fantasyPlayer);
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
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> PutFantasyPlayers(IEnumerable<FantasyPlayer> fantasyPlayers)
        {
            // Admin only operation
            if (!await _discordWebApiService.CheckAdminUser(HttpContext))
            {
                return Unauthorized();
            }

            try
            {
                DiscordUser? discordUser = await _discordWebApiService.LookupHttpContextUser(HttpContext);
                if (discordUser == null)
                {
                    return Unauthorized();
                }

                await _fantasyServiceAdmin.UpdateFantasyPlayersAsync(discordUser, fantasyPlayers);
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
            // Admin only operation
            if (!await _discordWebApiService.CheckAdminUser(HttpContext))
            {
                return Unauthorized();
            }

            try
            {
                DiscordUser? discordUser = await _discordWebApiService.LookupHttpContextUser(HttpContext);
                if (discordUser == null)
                {
                    return Unauthorized();
                }

                await _fantasyServiceAdmin.DeleteFantasyPlayerAsync(discordUser, fantasyPlayerId);
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
