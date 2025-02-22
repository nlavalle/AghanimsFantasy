using csharp_ef_webapi.Services;
using DataAccessLibrary.Models.Fantasy;
using DataAccessLibrary.Models.Discord;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrivateFantasyLeagueController : ControllerBase
    {
        private readonly DiscordWebApiService _discordWebApiService;
        private readonly FantasyServicePrivateFantasyAdmin _fantasyServicePrivateFantasyAdmin;

        public PrivateFantasyLeagueController(
            DiscordWebApiService discordWebApiService,
            FantasyServicePrivateFantasyAdmin fantasyServicePrivateFantasyAdmin
        )
        {
            _discordWebApiService = discordWebApiService;
            _fantasyServicePrivateFantasyAdmin = fantasyServicePrivateFantasyAdmin;
        }

        // GET: api/PrivateFantasyLeague/5
        [Authorize]
        [HttpGet("{privateFantasyPlayerId}")]
        public async Task<ActionResult<FantasyPrivateLeaguePlayer>> GetPrivateFantasyPlayer(int privateFantasyPlayerId)
        {
            try
            {
                DiscordUser? discordUser = await _discordWebApiService.LookupHttpContextUser(HttpContext);

                if (discordUser == null)
                {
                    return NotFound();
                }

                var fantasyPlayer = await _fantasyServicePrivateFantasyAdmin.GetFantasyPrivateLeaguePlayerAsync(discordUser, privateFantasyPlayerId);

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

        // GET: api/PrivateFantasyLeague/FantasyLeague/5
        [Authorize]
        [HttpGet("fantasyleague/{fantasyLeagueId}")]
        public async Task<ActionResult<IEnumerable<FantasyPrivateLeaguePlayer>>> GetPrivateFantasyPlayers(int fantasyLeagueId)
        {
            try
            {
                DiscordUser? discordUser = await _discordWebApiService.LookupHttpContextUser(HttpContext);

                if (discordUser == null)
                {
                    return NotFound();
                }

                return Ok(await _fantasyServicePrivateFantasyAdmin.GetFantasyPrivateLeaguePlayersAsync(discordUser, fantasyLeagueId));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/PrivateFantasyLeague/Validate/DiscordUsername
        [Authorize]
        [HttpGet("validate/{discordUsername}")]
        public async Task<ActionResult<string>> ValidateDiscordUsername(string discordUsername)
        {
            try
            {
                DiscordUser? discordUser = await _discordWebApiService.LookupHttpContextUser(HttpContext);

                if (discordUser == null)
                {
                    return NotFound();
                }

                if (await _discordWebApiService.CheckPrivateFantasyAdminUser(discordUser.Id) == false)
                {
                    return NotFound();
                }

                var discordUserLookup = await _discordWebApiService.GetDiscordUserAsync(discordUsername);

                if (discordUserLookup == null)
                {
                    return Ok(0.ToString());
                }

                return Ok(discordUserLookup.Id.ToString());
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/PrivateFantasyLeague/FantasyLeague
        [Authorize]
        [HttpGet("fantasyleague")]
        public async Task<ActionResult<IEnumerable<FantasyPrivateLeaguePlayer>>> GetPrivateFantasyLeagues()
        {
            try
            {
                DiscordUser? discordUser = await _discordWebApiService.LookupHttpContextUser(HttpContext);

                if (discordUser == null)
                {
                    return NotFound();
                }

                return Ok(await _fantasyServicePrivateFantasyAdmin.GetPrivateFantasyLeagues(discordUser));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/PrivateFantasyLeague/FantasyLeagueWeight
        [Authorize]
        [HttpGet("fantasyleagueweight")]
        public async Task<ActionResult<IEnumerable<FantasyLeagueWeight>>> GetFantasyLeagueWeights()
        {
            try
            {
                DiscordUser? discordUser = await _discordWebApiService.LookupHttpContextUser(HttpContext);

                if (discordUser == null)
                {
                    return NotFound();
                }

                return Ok(await _fantasyServicePrivateFantasyAdmin.GetFantasyLeagueWeightsAsync(discordUser));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/PrivateFantasyLeague
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<FantasyLeague>> PostPrivateFantasyPlayers(FantasyPrivateLeaguePlayer fantasyPrivateLeaguePlayer)
        {
            DiscordUser? discordUser = await _discordWebApiService.LookupHttpContextUser(HttpContext);

            if (discordUser == null)
            {
                return Unauthorized();
            }

            // Private Fantasy Admin only operation
            if (!await _discordWebApiService.CheckPrivateFantasyAdminUser(discordUser.Id))
            {
                return Unauthorized();
            }

            try
            {
                await _fantasyServicePrivateFantasyAdmin.AddPrivateFantasyPlayerAsync(discordUser, fantasyPrivateLeaguePlayer);
                return CreatedAtAction("GetPrivateFantasyPlayer", new { privateFantasyPlayerId = fantasyPrivateLeaguePlayer.Id }, fantasyPrivateLeaguePlayer);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        // PUT: api/PrivateFantasyLeague/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{privateFantasyPlayerId}")]
        public async Task<IActionResult> PutPrivateFantasyPlayer(int privateFantasyPlayerId, FantasyPrivateLeaguePlayer fantasyPrivateLeaguePlayer)
        {
            DiscordUser? discordUser = await _discordWebApiService.LookupHttpContextUser(HttpContext);

            if (discordUser == null)
            {
                return Unauthorized();
            }

            // Private Fantasy Admin only operation
            if (!await _discordWebApiService.CheckPrivateFantasyAdminUser(discordUser.Id))
            {
                return Unauthorized();
            }

            try
            {

                await _fantasyServicePrivateFantasyAdmin.UpdatePrivateFantasyPlayerAsync(discordUser, privateFantasyPlayerId, fantasyPrivateLeaguePlayer);
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

        // PUT: api/PrivateFantasyLeague/FantasyLeagueWeight/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("fantasyleagueweight/{fantasyLeagueWeightId}")]
        public async Task<IActionResult> PutFantasyLeagueWeight(int fantasyLeagueWeightId, FantasyLeagueWeight fantasyLeagueWeight)
        {
            try
            {
                DiscordUser? discordUser = await _discordWebApiService.LookupHttpContextUser(HttpContext);
                if (discordUser == null)
                {
                    return Unauthorized();
                }

                bool isPrivateAdmin = await _fantasyServicePrivateFantasyAdmin.IsUserPrivateFantasyAdminAsync(discordUser);

                if (!isPrivateAdmin)
                {
                    return Unauthorized();
                }

                await _fantasyServicePrivateFantasyAdmin.UpdateFantasyLeagueWeightAsync(discordUser, fantasyLeagueWeightId, fantasyLeagueWeight);
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

        // DELETE: api/PrivateFantasyLeague/5
        [Authorize]
        [HttpDelete("{privateFantasyPlayerId}")]
        public async Task<IActionResult> DeletePrivateFantasyPlayer(int privateFantasyPlayerId)
        {
            DiscordUser? discordUser = await _discordWebApiService.LookupHttpContextUser(HttpContext);

            if (discordUser == null)
            {
                return Unauthorized();
            }

            // Private Fantasy Admin only operation
            if (!await _discordWebApiService.CheckPrivateFantasyAdminUser(discordUser.Id))
            {
                return Unauthorized();
            }

            try
            {
                await _fantasyServicePrivateFantasyAdmin.DeletePrivateFantasyPlayerAsync(discordUser, privateFantasyPlayerId);
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
