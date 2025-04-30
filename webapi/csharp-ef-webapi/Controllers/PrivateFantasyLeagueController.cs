using csharp_ef_webapi.Services;
using DataAccessLibrary.Data.Identity;
using DataAccessLibrary.Models.Fantasy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrivateFantasyLeagueController : ControllerBase
    {
        private readonly FantasyServicePrivateFantasyAdmin _fantasyServicePrivateFantasyAdmin;
        private readonly UserManager<AghanimsFantasyUser> _userManager;

        public PrivateFantasyLeagueController(
            FantasyServicePrivateFantasyAdmin fantasyServicePrivateFantasyAdmin,
            UserManager<AghanimsFantasyUser> userManager
        )
        {
            _fantasyServicePrivateFantasyAdmin = fantasyServicePrivateFantasyAdmin;
            _userManager = userManager;
        }

        // GET: api/PrivateFantasyLeague/5
        [Authorize]
        [HttpGet("{privateFantasyPlayerId}")]
        public async Task<ActionResult<FantasyPrivateLeaguePlayer>> GetPrivateFantasyPlayer(int privateFantasyPlayerId)
        {
            try
            {
                var fantasyPlayer = await _fantasyServicePrivateFantasyAdmin.GetFantasyPrivateLeaguePlayerAsync(HttpContext.User, privateFantasyPlayerId);

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
                return Ok(await _fantasyServicePrivateFantasyAdmin.GetFantasyPrivateLeaguePlayersAsync(HttpContext.User, fantasyLeagueId));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/PrivateFantasyLeague/Validate/DiscordUsername
        [Authorize(Roles = "PrivateFantasyLeagueAdmin")]
        [HttpGet("validate/{discordUsername}")]
        public async Task<ActionResult<string>> ValidateUsername(string userName)
        {
            try
            {
                var userLookup = await _userManager.FindByNameAsync(userName);

                if (userLookup == null)
                {
                    return Ok(0.ToString());
                }

                return Ok(userLookup.Id.ToString());
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
                return Ok(await _fantasyServicePrivateFantasyAdmin.GetPrivateFantasyLeagues(HttpContext.User));
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
                return Ok(await _fantasyServicePrivateFantasyAdmin.GetFantasyLeagueWeightsAsync(HttpContext.User));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/PrivateFantasyLeague
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "PrivateFantasyLeagueAdmin")] // Private Admin only operation
        [HttpPost]
        public async Task<ActionResult<FantasyLeague>> PostPrivateFantasyPlayers(FantasyPrivateLeaguePlayer fantasyPrivateLeaguePlayer)
        {
            try
            {
                await _fantasyServicePrivateFantasyAdmin.AddPrivateFantasyPlayerAsync(HttpContext.User, fantasyPrivateLeaguePlayer);
                return CreatedAtAction("GetPrivateFantasyPlayer", new { privateFantasyPlayerId = fantasyPrivateLeaguePlayer.Id }, fantasyPrivateLeaguePlayer);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        // PUT: api/PrivateFantasyLeague/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "PrivateFantasyLeagueAdmin")] // Private Admin only operation
        [HttpPut("{privateFantasyPlayerId}")]
        public async Task<IActionResult> PutPrivateFantasyPlayer(int privateFantasyPlayerId, FantasyPrivateLeaguePlayer fantasyPrivateLeaguePlayer)
        {
            try
            {

                await _fantasyServicePrivateFantasyAdmin.UpdatePrivateFantasyPlayerAsync(HttpContext.User, privateFantasyPlayerId, fantasyPrivateLeaguePlayer);
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
        [Authorize(Roles = "PrivateFantasyLeagueAdmin")] // Private Admin only operation
        [HttpPut("fantasyleagueweight/{fantasyLeagueWeightId}")]
        public async Task<IActionResult> PutFantasyLeagueWeight(int fantasyLeagueWeightId, FantasyLeagueWeight fantasyLeagueWeight)
        {
            try
            {
                await _fantasyServicePrivateFantasyAdmin.UpdateFantasyLeagueWeightAsync(HttpContext.User, fantasyLeagueWeightId, fantasyLeagueWeight);
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
        [Authorize(Roles = "PrivateFantasyLeagueAdmin")] // Private Admin only operation
        [HttpDelete("{privateFantasyPlayerId}")]
        public async Task<IActionResult> DeletePrivateFantasyPlayer(int privateFantasyPlayerId)
        {
            try
            {
                await _fantasyServicePrivateFantasyAdmin.DeletePrivateFantasyPlayerAsync(HttpContext.User, privateFantasyPlayerId);
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
