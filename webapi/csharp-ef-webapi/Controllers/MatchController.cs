using Microsoft.AspNetCore.Mvc;
using DataAccessLibrary.Data;
using DataAccessLibrary.Models.GameCoordinator;
using Microsoft.AspNetCore.OutputCaching;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [OutputCache]
    public class MatchController : ControllerBase
    {
        private readonly AghanimsFantasyContext _dbContext;

        public MatchController(
            AghanimsFantasyContext dbContext
        )
        {
            _dbContext = dbContext;
        }

        // GET: api/Match/5/Metadata
        [HttpGet("{matchId}/metadata")]
        public async Task<ActionResult<GcMatchMetadata>> GetLeagueMatchIdMetadata(long matchId)
        {
            var match = await _dbContext.GcMatchMetadata.FindAsync(matchId);

            if (match == null)
            {
                return NotFound();
            }

            return Ok(match);
        }
    }
}
