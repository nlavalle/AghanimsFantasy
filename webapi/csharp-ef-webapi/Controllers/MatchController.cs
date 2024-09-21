using Microsoft.AspNetCore.Mvc;
using DataAccessLibrary.Data;
using DataAccessLibrary.Models.WebApi;
using DataAccessLibrary.Models.GameCoordinator;
using DataAccessLibrary.Models.ProMetadata;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly GameCoordinatorRepository _gameCoordinatorRepository;
        private readonly IMatchDetailRepository _matchDetailRepository;
        private readonly ProMetadataRepository _proMetadataRepository;

        public MatchController(
            GameCoordinatorRepository gameCoordinatorRepository,
            IMatchDetailRepository matchDetailRepository,
            ProMetadataRepository proMetadataRepository
        )
        {
            _gameCoordinatorRepository = gameCoordinatorRepository;
            _matchDetailRepository = matchDetailRepository;
            _proMetadataRepository = proMetadataRepository;
        }

        // GET: api/Match/5/players
        [HttpGet("{leagueId}/players")]
        public async Task<ActionResult<IEnumerable<MatchDetailsPlayer>>> GetMatchPlayers(int leagueId)
        {
            League? league = await _proMetadataRepository.GetLeagueAsync(leagueId);

            if (league == null)
            {
                return NotFound("League ID not found");
            }

            return Ok(await _matchDetailRepository.GetByLeagueAsync(league));
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
