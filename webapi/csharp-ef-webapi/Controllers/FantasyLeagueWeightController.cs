using csharp_ef_webapi.Extensions;
using csharp_ef_webapi.Services;
using DataAccessLibrary.Models.Fantasy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthenticatedETag]
    public class FantasyLeagueWeightController : ControllerBase
    {
        private readonly FantasyService _fantasyService;
        private readonly FantasyServiceAdmin _fantasyServiceAdmin;

        public FantasyLeagueWeightController(
            FantasyService fantasyService,
            FantasyServiceAdmin fantasyServiceAdmin
        )
        {
            _fantasyService = fantasyService;
            _fantasyServiceAdmin = fantasyServiceAdmin;
        }

        // GET: api/FantasyLeagueWeight
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FantasyLeagueWeight>>> GetFantasyLeagueWeights()
        {
            try
            {
                return Ok(await _fantasyService.GetFantasyLeagueWeightsAsync(HttpContext.User));
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
                return Ok(await _fantasyService.GetFantasyLeagueWeightAsync(HttpContext.User, fantasyLeagueWeightId));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/FantasyLeagueWeight
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")] // Admin only operation
        [HttpPost]
        public async Task<ActionResult<FantasyLeagueWeight>> PostFantasyLeagueWeight(FantasyLeagueWeight fantasyLeagueWeight)
        {
            try
            {
                await _fantasyServiceAdmin.AddFantasyLeagueWeightAsync(fantasyLeagueWeight);
                return CreatedAtAction("GetFantasyLeagueWeight", new { fantasyLeagueWeightId = fantasyLeagueWeight.Id }, fantasyLeagueWeight);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        // PUT: api/FantasyLeagueWeight/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")] // Admin only operation
        [HttpPut("{fantasyLeagueWeightId}")]
        public async Task<IActionResult> PutFantasyLeagueWeight(int fantasyLeagueWeightId, FantasyLeagueWeight fantasyLeagueWeight)
        {
            try
            {
                await _fantasyServiceAdmin.UpdateFantasyLeagueWeightAsync(fantasyLeagueWeightId, fantasyLeagueWeight);
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
        [Authorize(Roles = "Admin")] // Admin only operation
        [HttpDelete("{fantasyLeagueWeightId}")]
        public async Task<IActionResult> DeleteFantasyLeagueWeight(int fantasyLeagueWeightId)
        {
            try
            {
                await _fantasyServiceAdmin.DeleteFantasyLeagueWeightAsync(fantasyLeagueWeightId);
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
