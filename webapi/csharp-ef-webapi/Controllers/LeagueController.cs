using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using csharp_ef_webapi.Utilities;
using DataAccessLibrary.Data;
using DataAccessLibrary.Models.ProMetadata;
using DataAccessLibrary.Models.WebApi;
using DataAccessLibrary.Models.GameCoordinator;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeagueController : ControllerBase
    {
        private readonly IDiscordRepository _discordRepository;
        private readonly IProMetadataRepository _proMetadataRepository;
        private readonly IMatchHistoryRepository _matchHistoryRepository;
        private readonly IMatchDetailRepository _matchDetailRepository;
        private readonly IGcMatchMetadataRepository _gcMatchMetadataRepository;

        public LeagueController(
            IDiscordRepository discordRepository,
            IProMetadataRepository proMetadataRepository,
            IMatchHistoryRepository matchHistoryRepository,
            IMatchDetailRepository matchDetailRepository,
            IGcMatchMetadataRepository gcMatchMetadataRepository)
        {
            _discordRepository = discordRepository;
            _proMetadataRepository = proMetadataRepository;
            _matchHistoryRepository = matchHistoryRepository;
            _matchDetailRepository = matchDetailRepository;
            _gcMatchMetadataRepository = gcMatchMetadataRepository;
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
        public async Task<ActionResult<IEnumerable<MatchHistory>>> GetLeagueMatchHistory(int leagueId)
        {
            League? league = await _proMetadataRepository.GetLeagueAsync(leagueId);

            if (league == null)
            {
                return NotFound("League ID not found");
            }

            var matches = await _matchHistoryRepository.GetByLeagueAsync(league);

            if (matches == null || matches.Count() == 0)
            {
                return NotFound();
            }

            return Ok(matches);
        }

        // GET: api/League/5/Match/Details
        [HttpGet("{leagueId}/match/details")]
        public async Task<ActionResult<List<MatchDetail>>> GetLeagueMatchDetails(int leagueId)
        {
            League? league = await _proMetadataRepository.GetLeagueAsync(leagueId);

            if (league == null)
            {
                return NotFound("League ID not found");
            }

            var matches = await _matchDetailRepository.GetByLeagueAsync(league);

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
            League? league = await _proMetadataRepository.GetLeagueAsync(leagueId);

            if (league == null)
            {
                return NotFound("League ID not found");
            }

            var matches = await _matchDetailRepository.GetByIdAsync(matchId);

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

            League? league = await _proMetadataRepository.GetLeagueAsync(leagueId);

            if (league == null)
            {
                return NotFound("League ID not found");
            }

            var matches = await _gcMatchMetadataRepository.GetByLeagueAsync(league);

            if (matches == null || matches.Count() == 0)
            {
                return NotFound();
            }

            // Order matches so most recent show up first, we'll typically want to get highlights from the most recent
            matches = matches.OrderByDescending(m => m.MatchId).ToList();

            var paginatedMatches = PaginatedList<GcMatchMetadata>.Create(matches, pageIndex, pageSize);

            return Ok(paginatedMatches);
        }
    }
}
