using csharp_ef_webapi.Services;
using DataAccessLibrary.Data;
using DataAccessLibrary.Models.Discord;
using DataAccessLibrary.Models.ProMetadata;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly AghanimsFantasyContext _dbContext;
        private readonly DiscordWebApiService _discordWebApiService;
        private readonly FantasyServiceAdmin _fantasyServiceAdmin;

        public TeamController(
            AghanimsFantasyContext dbContext,
            DiscordWebApiService discordWebApiService,
            FantasyServiceAdmin fantasyServiceAdmin
        )
        {
            _dbContext = dbContext;
            _discordWebApiService = discordWebApiService;
            _fantasyServiceAdmin = fantasyServiceAdmin;
        }

        // GET: api/Team/5
        [HttpGet("{teamId}")]
        public async Task<ActionResult<Team?>> GetTeam(long? teamId)
        {
            if (teamId == null)
            {
                return BadRequest("Please specify a Team Id");
            }

            return Ok(await _dbContext.Teams.FindAsync(teamId.Value));
        }

        // GET: api/Team
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
        {
            return Ok(await _dbContext.Teams.ToListAsync());
        }

        // POST: api/Team
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Team>> PostTeam(Team team)
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

                await _fantasyServiceAdmin.AddTeamAsync(discordUser, team);
                return CreatedAtAction("GetTeam", new { teamId = team.Id }, team);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        // PUT: api/Team/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{teamId}")]
        public async Task<IActionResult> PutTeam(long teamId, Team updateTeam)
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

                await _fantasyServiceAdmin.UpdateTeamAsync(discordUser, teamId, updateTeam);
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

        // DELETE: api/Team/5
        [Authorize]
        [HttpDelete("{teamId}")]
        public async Task<IActionResult> DeleteTeam(long teamId)
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

                await _fantasyServiceAdmin.DeleteTeamAsync(discordUser, teamId);
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
