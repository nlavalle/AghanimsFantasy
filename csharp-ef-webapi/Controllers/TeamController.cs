using Microsoft.AspNetCore.Mvc;
using csharp_ef_webapi.Models;
using csharp_ef_webapi.Data;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly FantasyRepository _service;

        public TeamController(FantasyRepository service)
        {
            _service = service;
        }

        // GET: api/Team/teams
        [HttpGet("teams")]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
        {
            return Ok(await _service.GetTeamsAsync());
        }
    }
}
