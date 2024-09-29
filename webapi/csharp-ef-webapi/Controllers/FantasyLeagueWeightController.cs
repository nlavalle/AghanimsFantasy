using Microsoft.AspNetCore.Mvc;
using DataAccessLibrary.Models.Fantasy;
using Microsoft.AspNetCore.Authorization;
using csharp_ef_webapi.Services;
using DataAccessLibrary.Models.Discord;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FantasyLeagueWeightController : ControllerBase
    {
        private readonly DiscordWebApiService _discordWebApiService;
        private readonly FantasyService _fantasyService;
        private readonly FantasyServiceAdmin _fantasyServiceAdmin;

        public FantasyLeagueWeightController(
            DiscordWebApiService discordWebApiService,
            FantasyService fantasyService,
            FantasyServiceAdmin fantasyServiceAdmin
        )
        {
            _discordWebApiService = discordWebApiService;
            _fantasyService = fantasyService;
            _fantasyServiceAdmin = fantasyServiceAdmin;
        }

        // GET: api/FantasyLeagueWeight
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FantasyLeagueWeight>>> GetFantasyLeagueWeights()
        {
            try
            {
                DiscordUser? discordUser = await _discordWebApiService.LookupHttpContextUser(HttpContext);

                return Ok(await _fantasyService.GetFantasyLeagueWeightsAsync(discordUser));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/FantasyLeagueWeight/5
        [HttpGet("{fantasyLeagueWeightId}")]
        public async Task<ActionResult<FantasyLeague>> GetFantasyLeagueWeight(int fantasyLeagueWeightId)
        {
            try
            {
                DiscordUser? discordUser = await _discordWebApiService.LookupHttpContextUser(HttpContext);

                return Ok(await _fantasyService.GetFantasyLeagueWeightAsync(discordUser, fantasyLeagueWeightId));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/FantasyLeagueWeight
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<FantasyLeagueWeight>> PostFantasyLeagueWeight(FantasyLeagueWeight fantasyLeagueWeight)
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

                await _fantasyServiceAdmin.AddFantasyLeagueWeightAsync(discordUser, fantasyLeagueWeight);
                return CreatedAtAction("GetFantasyLeagueWeight", new { fantasyLeagueWeightId = fantasyLeagueWeight.Id }, fantasyLeagueWeight);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        // PUT: api/FantasyLeagueWeight/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{fantasyLeagueWeightId}")]
        public async Task<IActionResult> PutFantasyLeagueWeight(int fantasyLeagueWeightId, FantasyLeagueWeight fantasyLeagueWeight)
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

                await _fantasyServiceAdmin.UpdateFantasyLeagueWeightAsync(discordUser, fantasyLeagueWeightId, fantasyLeagueWeight);
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

        // DELETE: api/FantasyLeagueWeight/5
        [Authorize]
        [HttpDelete("{fantasyLeagueWeightId}")]
        public async Task<IActionResult> DeleteFantasyLeagueWeight(int fantasyLeagueWeightId)
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

                await _fantasyServiceAdmin.DeleteFantasyLeagueWeightAsync(discordUser, fantasyLeagueWeightId);
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
