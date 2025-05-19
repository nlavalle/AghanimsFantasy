using csharp_ef_webapi.Services;
using DataAccessLibrary.Models.Fantasy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace csharp_ef_webapi.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly FantasyServiceAdmin _fantasyServiceAdmin;

        public AdminController(
            FantasyServiceAdmin fantasyServiceAdmin
        )
        {
            _fantasyServiceAdmin = fantasyServiceAdmin;
        }

        // GET: api/Admin/FantasyLeague
        [HttpGet("fantasyleague")]
        public async Task<ActionResult<IEnumerable<FantasyLeague>>> GetAdminFantasyLeagues()
        {
            // Custom URL for admin page to always fetch everything incl. private leagues
            try
            {
                return Ok(await _fantasyServiceAdmin.GetFantasyLeaguesAsync());
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST: api/admin/fantasyleague/5/team/1
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("fantasyleague/{fantasyLeagueId}/team/{teamId}")]
        public async Task<ActionResult> PostFantasyPlayersByTeam(int fantasyLeagueId, long teamId)
        {
            try
            {
                await _fantasyServiceAdmin.AddFantasyPlayersByTeam(teamId, fantasyLeagueId);
                return Created();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }
    }
}