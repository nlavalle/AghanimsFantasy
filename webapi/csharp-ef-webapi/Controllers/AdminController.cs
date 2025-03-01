using Microsoft.AspNetCore.Mvc;
using DataAccessLibrary.Models.Fantasy;
using Microsoft.AspNetCore.Authorization;
using csharp_ef_webapi.Services;
using DataAccessLibrary.Models.Discord;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly DiscordWebApiService _discordWebApiService;
        private readonly FantasyServiceAdmin _fantasyServiceAdmin;

        public AdminController(
            DiscordWebApiService discordWebApiService,
            FantasyServiceAdmin fantasyServiceAdmin
        )
        {
            _discordWebApiService = discordWebApiService;
            _fantasyServiceAdmin = fantasyServiceAdmin;
        }

        // GET: api/Admin/FantasyLeague
        [Authorize]
        [HttpGet("fantasyleague")]
        public async Task<ActionResult<IEnumerable<FantasyLeague>>> GetAdminFantasyLeagues()
        {
            // Custom URL for admin page to always fetch everything incl. private leagues
            try
            {
                DiscordUser? discordUser = await _discordWebApiService.LookupHttpContextUser(HttpContext);

                if (discordUser == null)
                {
                    return Unauthorized();
                }

                if (!discordUser.IsAdmin)
                {
                    return Unauthorized();
                }

                return Ok(await _fantasyServiceAdmin.GetFantasyLeaguesAsync(discordUser));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: api/admin/fantasyleague/5/team/1
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost("fantasyleague/{fantasyLeagueId}/team/{teamId}")]
        public async Task<ActionResult> PostFantasyPlayersByTeam(int fantasyLeagueId, long teamId)
        {
            // Admin only operation
            DiscordUser? discordUser = await _discordWebApiService.LookupHttpContextUser(HttpContext);

            if (discordUser == null)
            {
                return Unauthorized();
            }

            if (!discordUser.IsAdmin)
            {
                return Unauthorized();
            }

            try
            {
                await _fantasyServiceAdmin.AddFantasyPlayersByTeam(discordUser, teamId, fantasyLeagueId);
                return Created();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }
    }
}