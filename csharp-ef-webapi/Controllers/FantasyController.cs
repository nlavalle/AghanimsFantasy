#nullable disable
using System.Security.Claims;
using csharp_ef_webapi.Data;
using csharp_ef_webapi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FantasyController : ControllerBase
    {
        private readonly FantasyRepository _service;
        public FantasyController(FantasyRepository service)
        {
            _service = service;
        }

        // GET: api/fantasy/players/5
        [HttpGet("players/{leagueId}")]
        public async Task<ActionResult<List<FantasyPlayer>>> GetFantasyPlayers(int? leagueId)
        {
            var players = await _service.GetFantasyPlayersAsync(leagueId);
            return Ok(players);
        }

        // GET: api/fantasy/draft/5
        [Authorize]
        [HttpGet("draft/{leagueId}")]
        public async Task<IActionResult> GetUserDraft(int? leagueId)
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                // Authorize should take care of this but just in case
                return BadRequest("User not authenticated");
            }

            bool getAccountId = long.TryParse(HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value, out long userDiscordAccountId);

            if (leagueId == null)
            {
                return BadRequest("Please provide a League ID to fetch a draft of");
            }

            return Ok(await _service.GetUserFantasyDraftsByLeagueAsync(userDiscordAccountId, leagueId.Value));
        }

        // GET: api/fantasy/draft/5
        [Authorize]
        [HttpGet("draft/{leagueId}/points")]
        public async Task<IActionResult> GetUserDraftPoints(int? leagueId)
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                // Authorize should take care of this but just in case
                return BadRequest("User not authenticated");
            }

            bool getAccountId = long.TryParse(HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value, out long userDiscordAccountId);

            if (leagueId == null || !leagueId.HasValue)
            {
                return BadRequest("Please provide a League ID to fetch a draft of");
            }

            var fantasyPoints = await _service.GetFantasyPlayerPointsAsync(leagueId.Value);
            if (fantasyPoints.Count() == 0)
            {
                // League doesn't have fantasy players/points yet
                return Ok(new { });
            }

            var userDraftWithPoints = await _service.GetUserTotalFantasyPointsByLeagueAsync(userDiscordAccountId, leagueId.Value);
            if (userDraftWithPoints == null)
            {
                // User has no draft yet so return an empty okay
                return Ok(new { });
            }
            return Ok(userDraftWithPoints);
        }

        // POST: api/fantasy/draft
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost("draft")]
        public async Task<ActionResult> PostUserDraft(FantasyDraft fantasyDraft)
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                // Authorize should take care of this but just in case
                return BadRequest("User not authenticated");
            }

            bool getAccountId = long.TryParse(HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value, out long userDiscordAccountId);

            if (!getAccountId)
            {
                return BadRequest("Could not retrieve user's discord ID");
            }

            var existingUserDraft = await _service.GetUserFantasyDraftsByLeagueAsync(userDiscordAccountId, fantasyDraft.LeagueId);

            var draftLockedDate = await _service.GetLeagueLockedDate(fantasyDraft.LeagueId);
            if(existingUserDraft.Count() > 0 && DateTime.UtcNow > draftLockedDate)
            {
                // If a user hasn't drafted yet let them add it in late, if they already have a draft though return a bad request cannot update
                return BadRequest("Draft is locked for this league");
            }

            var fantasyDraftPostResponse = await _service.AddNewUserFantasyDraftAsync(userDiscordAccountId, fantasyDraft);

            return CreatedAtAction(nameof(GetUserDraft), new { leagueId = fantasyDraft.LeagueId }, fantasyDraftPostResponse);
        }
    }
}
