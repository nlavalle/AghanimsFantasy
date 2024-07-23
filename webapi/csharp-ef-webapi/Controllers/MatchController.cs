using Microsoft.AspNetCore.Mvc;
using DataAccessLibrary.Data;
using DataAccessLibrary.Models.WebApi;
using DataAccessLibrary.Models.GameCoordinator;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly WebApiRepository _webApiRepository;
        private readonly GameCoordinatorRepository _gameCoordinatorRepository;

        public MatchController(WebApiRepository webApiRepository, GameCoordinatorRepository gameCoordinatorRepository)
        {
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
