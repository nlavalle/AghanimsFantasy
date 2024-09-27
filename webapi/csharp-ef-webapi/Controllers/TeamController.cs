using Microsoft.AspNetCore.Mvc;
using DataAccessLibrary.Data;
using DataAccessLibrary.Models.ProMetadata;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly IProMetadataRepository _proMetadataRepository;

        public TeamController(IProMetadataRepository proMetadataRepository)
        {
            _proMetadataRepository = proMetadataRepository;
        }

        // GET: api/Team
        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
        {
            return Ok(await _proMetadataRepository.GetTeamsAsync());
        }
    }
}
