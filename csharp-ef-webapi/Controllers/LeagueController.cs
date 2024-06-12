using Microsoft.AspNetCore.Mvc;
using csharp_ef_webapi.Data;
using csharp_ef_webapi.Utilities;
using csharp_ef_webapi.Models.ProMetadata;
using csharp_ef_webapi.Models.WebApi;
using csharp_ef_webapi.Models.GameCoordinator;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeagueController : ControllerBase
    {
        private readonly ProMetadataRepository _proMetadataRepository;
        private readonly WebApiRepository _webApiRepository;
        private readonly GameCoordinatorRepository _gameCoordinatorRepository;

        public LeagueController(ProMetadataRepository proMetadataRepository, WebApiRepository webApiRepository, GameCoordinatorRepository gameCoordinatorRepository)
        {
            _proMetadataRepository = proMetadataRepository;
            _webApiRepository = webApiRepository;
            _gameCoordinatorRepository = gameCoordinatorRepository;
        }

        // GET: api/League
        [HttpGet]
        public async Task<ActionResult<IEnumerable<League>>> GetLeagues(bool? is_active = null)
        {
            return Ok(await _proMetadataRepository.GetLeaguesAsync(is_active));
        }

        // GET: api/League/5
        [HttpGet("{id}")]
        public async Task<ActionResult<League>> GetLeague(int id)
        {
            var league = await _proMetadataRepository.GetLeagueAsync(id);

            if (league == null)
            {
                return NotFound();
            }

            return Ok(league);
        }

        // GET: api/League/5/Match/History
        [HttpGet("{leagueId}/match/history")]
        public async Task<ActionResult<IEnumerable<MatchHistory>>> GetLeagueMatchHistory(int fantasyLeagueId)
        {

            var matches = await _webApiRepository.GetMatchHistoryByFantasyLeagueAsync(fantasyLeagueId);

            if (matches == null || matches.Count() == 0)
            {
                return NotFound();
            }

            return Ok(matches);
        }

        // GET: api/League/5/Match/Details
        [HttpGet("{leagueId}/match/details")]
        public async Task<ActionResult<List<MatchDetail>>> GetLeagueMatchDetails(int fantasyLeagueId)
        {
            var matches = await _webApiRepository.GetMatchDetailsByFantasyLeagueAsync(fantasyLeagueId);

            if (matches == null || matches.Count() == 0)
            {
                return NotFound();
            }

            return Ok(matches);
        }

        // GET: api/League/5/Match/6/Details
        [HttpGet("{leagueId}/match/{matchId}/details")]
        public async Task<ActionResult<MatchDetail>> GetLeagueMatchIdDetails(int leagueId, long matchId)
        {
            var matches = await _webApiRepository.GetMatchDetailAsync(matchId);

            if (matches == null)
            {
                return NotFound();
            }

            return Ok(matches);
        }

        // GET: api/League/5/Match/Metadata
        [HttpGet("{leagueId}/match/metadata")]
        public async Task<ActionResult<List<GcMatchMetadata>>> GetLeagueMatchMetadata(int leagueId, int pageIndex = 1, int pageSize = 50)
        {
            // Limit pageSize max
            if (pageSize > 100) { pageSize = 100; }

            var matches = await _gameCoordinatorRepository.GetLeagueMetadataAsync(leagueId);

            if (matches == null || matches.Count() == 0)
            {
                return NotFound();
            }

            // Order matches so most recent show up first, we'll typically want to get highlights from the most recent
            matches = matches.OrderByDescending(m => m.MatchId);

            var paginatedMatches = PaginatedList<GcMatchMetadata>.Create(matches, pageIndex, pageSize);

            return Ok(paginatedMatches);
        }
    }
}
