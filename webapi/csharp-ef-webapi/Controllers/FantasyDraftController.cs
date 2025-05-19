using csharp_ef_webapi.Extensions;
using csharp_ef_webapi.Services;
using DataAccessLibrary.Models;
using DataAccessLibrary.Models.Fantasy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace csharp_ef_webapi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FantasyDraftController : ControllerBase
    {
        private readonly FantasyService _fantasyService;

        public FantasyDraftController(
            FantasyService fantasyService
        )
        {
            _fantasyService = fantasyService;
        }

        // GET: api/fantasydraft/5
        [HttpGet("{fantasyLeagueId}")]
        [AuthenticatedETag]
        public async Task<IActionResult> GetUserDraft(int fantasyLeagueId)
        {
            try
            {
                return Ok(await _fantasyService.GetFantasyDraft(HttpContext.User, fantasyLeagueId));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/fantasydraft/5/drafts/points
        [HttpGet("{leagueId}/drafts/points")]
        [AuthenticatedETag]
        public async Task<ActionResult<IEnumerable<FantasyDraftPointTotals>>> GetUserDraftFantasyPoints(int leagueId)
        {
            try
            {
                if (HttpContext.User == null)
                {
                    return Ok(new List<FantasyDraftPointTotals>());
                }

                IEnumerable<FantasyDraftPointTotals> fantasyDraftPointTotals = await _fantasyService.GetFantasyDraftPointTotals(HttpContext.User, leagueId);

                return Ok(fantasyDraftPointTotals);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/fantasydraft/5/matches/points
        [HttpGet("{fantasyLeagueId}/matches/points")]
        [AuthenticatedETag]
        public async Task<ActionResult<List<FantasyPlayerPoints>>> GetDraftFantasyPlayersPointsByMatch(int? fantasyLeagueId, [FromQuery] int? limit)
        {
            if (fantasyLeagueId == null)
            {
                return BadRequest("Please provide a League ID to fetch fantasy player points of");
            }

            limit = limit ?? 10;

            limit = limit > 100 ? 100 : limit;

            try
            {
                return Ok(await _fantasyService.GetFantasyPlayerPoints(HttpContext.User, fantasyLeagueId.Value, limit.Value));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/fantasydraft
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostUserDraft(FantasyDraft fantasyDraft)
        {
            try
            {
                var fantasyDraftPostResponse = await _fantasyService.UpdateFantasyDraft(HttpContext.User, fantasyDraft);
                return CreatedAtAction(nameof(GetUserDraft), new { fantasyLeagueId = fantasyDraft.FantasyLeagueId }, fantasyDraftPostResponse);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
