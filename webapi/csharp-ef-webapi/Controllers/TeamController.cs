using csharp_ef_webapi.Extensions;
using csharp_ef_webapi.Services;
using DataAccessLibrary.Data;
using DataAccessLibrary.Models.ProMetadata;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [OutputCache(Tags = new[] { "team" })]
    public class TeamController : ControllerBase
    {
        private readonly AghanimsFantasyContext _dbContext;
        private readonly FantasyServiceAdmin _fantasyServiceAdmin;
        private readonly IOutputCacheStore _cache;

        public TeamController(
            AghanimsFantasyContext dbContext,
            FantasyServiceAdmin fantasyServiceAdmin,
            IOutputCacheStore cache
        )
        {
            _dbContext = dbContext;
            _fantasyServiceAdmin = fantasyServiceAdmin;
            _cache = cache;
        }

        // GET: api/Team/5
        [HttpGet("{teamId}")]
        [ResponseETag]
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
        [ResponseETag]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
        {
            return Ok(await _dbContext.Teams.ToListAsync());
        }

        // POST: api/Team
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Team>> PostTeam(Team team)
        {
            try
            {
                await _fantasyServiceAdmin.AddTeamAsync(team);
                await _cache.EvictByTagAsync("team", default);
                return CreatedAtAction("GetTeam", new { teamId = team.Id }, team);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        // PUT: api/Team/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")]
        [HttpPut("{teamId}")]
        public async Task<IActionResult> PutTeam(long teamId, Team updateTeam)
        {
            try
            {
                await _fantasyServiceAdmin.UpdateTeamAsync(teamId, updateTeam);
                await _cache.EvictByTagAsync("team", default);
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
        [Authorize(Roles = "Admin")]
        [HttpDelete("{teamId}")]
        public async Task<IActionResult> DeleteTeam(long teamId)
        {
            try
            {
                await _fantasyServiceAdmin.DeleteTeamAsync(teamId);
                await _cache.EvictByTagAsync("team", default);
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
