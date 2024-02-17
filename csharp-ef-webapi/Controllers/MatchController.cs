using Microsoft.AspNetCore.Mvc;
using csharp_ef_webapi.Models;
using csharp_ef_webapi.Data;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly FantasyRepository _service;

        public MatchController(FantasyRepository service)
        {
            _service = service;
        }

        // GET: api/Match/5/players
        [HttpGet("{leagueId}/players")]
        public async Task<ActionResult<IEnumerable<MatchDetailsPlayer>>> GetMatchPlayers(int? leagueId)
        {
            return Ok(await _service.GetMatchDetailPlayersByLeagueAsync(leagueId));
        }

        // GET: api/Match/5/Metadata
        [HttpGet("{matchId}/metadata")]
        public async Task<ActionResult<GcMatchMetadata>> GetLeagueMatchIdMetadata(long matchId)
        {
            var match = await _service.GetMatchMetadataAsync(matchId);

            if (match == null)
            {
                return NotFound();
            }

            return Ok(match);
        }
    }
}
