using Microsoft.AspNetCore.Mvc;
using csharp_ef_webapi.Data;
using csharp_ef_webapi.Models.WebApi;
using csharp_ef_webapi.Models.GameCoordinator;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly FantasyRepository _fantasyRepository;
        private readonly WebApiRepository _webApiRepository;
        private readonly GameCoordinatorRepository _gameCoordinatorRepository;

        public MatchController(FantasyRepository fantasyRepository, WebApiRepository webApiRepository, GameCoordinatorRepository gameCoordinatorRepository)
        {
            _fantasyRepository = fantasyRepository;
            _webApiRepository = webApiRepository;
            _gameCoordinatorRepository = gameCoordinatorRepository;
        }

        // GET: api/Match/5/players
        [HttpGet("{leagueId}/players")]
        public async Task<ActionResult<IEnumerable<MatchDetailsPlayer>>> GetMatchPlayers(int? leagueId)
        {
            return Ok(await _webApiRepository.GetMatchDetailPlayersByLeagueAsync(leagueId));
        }

        // GET: api/Match/5/Metadata
        [HttpGet("{matchId}/metadata")]
        public async Task<ActionResult<GcMatchMetadata>> GetLeagueMatchIdMetadata(long matchId)
        {
            var match = await _gameCoordinatorRepository.GetMatchMetadataAsync(matchId);

            if (match == null)
            {
                return NotFound();
            }

            return Ok(match);
        }
    }
}
