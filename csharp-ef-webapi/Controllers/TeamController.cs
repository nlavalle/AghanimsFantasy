using Microsoft.AspNetCore.Mvc;
using csharp_ef_webapi.Data;
using csharp_ef_webapi.Models.ProMetadata;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly ProMetadataRepository _proMetadataRepository;

        public TeamController(ProMetadataRepository proMetadataRepository)
        {
            _proMetadataRepository = proMetadataRepository;
        }

        // GET: api/Team/teams
        [HttpGet("teams")]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
        {
            return Ok(await _proMetadataRepository.GetTeamsAsync());
        }
    }
}
