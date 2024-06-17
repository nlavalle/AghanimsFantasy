using Microsoft.AspNetCore.Mvc;
using csharp_ef_webapi.Data;
using csharp_ef_webapi.Utilities;
using csharp_ef_webapi.Models.ProMetadata;
using csharp_ef_webapi.Models.WebApi;
using csharp_ef_webapi.Models.GameCoordinator;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeagueController : ControllerBase
    {
        private readonly DiscordRepository _discordRepository;
        private readonly ProMetadataRepository _proMetadataRepository;
        private readonly WebApiRepository _webApiRepository;
        private readonly GameCoordinatorRepository _gameCoordinatorRepository;

        public LeagueController(DiscordRepository discordRepository, ProMetadataRepository proMetadataRepository, WebApiRepository webApiRepository, GameCoordinatorRepository gameCoordinatorRepository)
        {
            _discordRepository = discordRepository;
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

        // POST: api/League
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<League>> PostLeague(League league)
        {
            // Admin only operation
            var nameId = HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (nameId == null) return BadRequest("Invalid User Claims please contact admin");

            bool getAccountId = long.TryParse(nameId.Value, out long userDiscordAccountId);
            bool AdminCheck = await _discordRepository.IsUserAdminAsync(userDiscordAccountId);

            if (!AdminCheck)
            {
                return Unauthorized();
            }


            await _proMetadataRepository.AddLeagueAsync(league);

            return CreatedAtAction("GetLeague", new { id = league.Id }, league);
        }

        // DELETE: api/League/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeague(int id)
        {
            // Admin only operation
            var nameId = HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (nameId == null) return BadRequest("Invalid User Claims please contact admin");

            bool getAccountId = long.TryParse(nameId.Value, out long userDiscordAccountId);
            bool AdminCheck = await _discordRepository.IsUserAdminAsync(userDiscordAccountId);

            if (!AdminCheck)
            {
                return Unauthorized();
            }

            var league = await _proMetadataRepository.GetLeagueAsync(id);
            if (league == null)
            {
                return NotFound();
            }

            await _proMetadataRepository.DeleteLeagueAsync(league);

            return NoContent();
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
