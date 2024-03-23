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

        // GET: api/fantasy/leagues
        [HttpGet("leagues")]
        public async Task<ActionResult<IEnumerable<FantasyLeague>>> GetFantasyLeagues(bool? is_active = null)
        {
            return Ok(await _service.GetFantasyLeaguesAsync(is_active));
        }

        // GET: api/fantasy/players/5
        [HttpGet("players/{leagueId}")]
        public async Task<ActionResult<List<FantasyPlayer>>> GetFantasyPlayers(int? leagueId)
        {
            var players = await _service.FantasyPlayersByFantasyLeagueAsync(leagueId);
            return Ok(players);
        }

        // GET: api/fantasy/players/5/points
        [HttpGet("players/{leagueId}/points")]
        public async Task<ActionResult<List<FantasyPlayerPointTotals>>> GetFantasyPlayersPoints(int? leagueId)
        {
            if (leagueId == null)
            {
                return BadRequest("Please provide a League ID to fetch fantasy player points of");
            }

            var fantasyPlayerPoints = await _service.FantasyPlayerPointsByFantasyLeagueAsync(leagueId.Value);
            var playerTotals = _service.AggregateFantasyPlayerPoints(fantasyPlayerPoints).ToList();
            return Ok(playerTotals);
        }

        // // GET: api/fantasy/players/5/match/metadata
        // [HttpGet("players/{leagueId}/match/metadata")]
        // public async Task<ActionResult<List<GcMatchMetadata>>> GetFantasyLeagueMatchMetadata(int fantasyLeagueId)
        // {
        //     var matches = await _service.GetFantasyLeagueMetadataAsync(fantasyLeagueId);

        //     if (matches == null || matches.Count() == 0)
        //     {
        //         return NotFound();
        //     }

        //     // Order matches so most recent show up first, we'll typically want to get highlights from the most recent
        //     matches = matches.OrderByDescending(m => m.MatchId);

        //     return Ok(matches);
        // }

        // GET: api/fantasy/league/5/metadata
        [HttpGet("league/{fantasyLeagueId}/metadata")]
        public async Task<ActionResult<List<MetadataSummary>>> GetFantasyLeagueMatchMetadata(int fantasyLeagueId)
        {
            var matchSummary = await _service.AggregateMetadataAsync(fantasyLeagueId);

            if (matchSummary == null || matchSummary.Count() == 0)
            {
                return NotFound();
            }

            matchSummary = matchSummary.OrderBy(m => m.Player.DotaAccount.Name);

            return Ok(matchSummary);
        }

        // GET: api/fantasy/players/5/top10
        [Authorize]
        [HttpGet("players/{leagueId}/top10")]
        public async Task<IActionResult> GetTop10FantasyPoints(int? leagueId)
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

            var fantasyPoints = await _service.FantasyDraftPointsByFantasyLeagueAsync(leagueId.Value);

            if (fantasyPoints.Count() == 0)
            {
                // League doesn't have fantasy players/points yet
                return Ok(new { });
            }

            var fantasyTeams = await _service.GetTeamsAsync();

            var top10Players = fantasyPoints.Where(fp => !fp.IsTeam).OrderByDescending(fp => fp.DraftTotalFantasyPoints).Take(10).ToList();

            // We want the user included even if they're not top 10
            if (!top10Players.Any(tp => tp.FantasyDraft.DiscordAccountId == userDiscordAccountId))
            {
                var currentPlayer = fantasyPoints.Where(fp => fp.FantasyDraft.DiscordAccountId == userDiscordAccountId).FirstOrDefault();
                if (currentPlayer != null)
                {
                    top10Players.Add(currentPlayer);
                }
            }

            var teamsScores = fantasyPoints.Where(fp => fp.IsTeam).OrderByDescending(fp => fp.DraftTotalFantasyPoints).ToList();

            var unionedLeaderboard = top10Players.Union(teamsScores);

            unionedLeaderboard = unionedLeaderboard.Select(
                lb => new FantasyDraftPointTotals
                {
                    //We're doing this to mask the DiscordAccountId
                    FantasyDraft = new FantasyDraft
                    {
                        Id = lb.FantasyDraft.Id,
                        DraftCreated = lb.FantasyDraft.DraftCreated,
                        DraftLastUpdated = lb.FantasyDraft.DraftLastUpdated,
                        FantasyLeagueId = lb.FantasyDraft.FantasyLeagueId,
                        DraftPickPlayers = lb.FantasyDraft.DraftPickPlayers
                    },
                    DiscordName = lb.DiscordName,
                    IsTeam = lb.IsTeam,
                    TeamId = lb.TeamId,
                    FantasyPlayerPoints = lb.FantasyPlayerPoints
                }
            )
            .OrderByDescending(fp => fp.DraftTotalFantasyPoints)
            .ToList();

            return Ok(unionedLeaderboard);
        }

        // GET: api/fantasy/5/highlights/3
        [HttpGet("{fantasyLeagueId}/highlights/{matchCount}")]
        public async Task<ActionResult<List<GcMatchMetadata>>> GetMatchHighlights(int fantasyLeagueId, int matchCount)
        {
            var matchHighlights = await _service.GetLastNMatchHighlights(fantasyLeagueId, matchCount);

            if (matchHighlights == null || matchHighlights.Count() == 0)
            {
                return NotFound();
            }

            return Ok(matchHighlights);
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

            return Ok(await _service.FantasyDraftsByUserLeagueAsync(userDiscordAccountId, leagueId.Value));
        }

        // GET: api/fantasy/draft/5/points
        [Authorize]
        [HttpGet("draft/{leagueId}/points")]
        public async Task<IActionResult> GetUserDraftFantasyPoints(int? leagueId)
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

            var fantasyPoints = await _service.FantasyPlayersByFantasyLeagueAsync(leagueId.Value);
            if (fantasyPoints.Count() == 0)
            {
                // League doesn't have fantasy players/points yet
                return Ok(new { });
            }

            var userDraftPoints = await _service.FantasyDraftPointsByUserLeagueAsync(userDiscordAccountId, leagueId.Value);
            if (userDraftPoints == null)
            {
                // User has no draft yet so return an empty okay
                return Ok(new { });
            }

            return Ok(userDraftPoints);
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

            var existingUserDraft = await _service.FantasyDraftsByUserLeagueAsync(userDiscordAccountId, fantasyDraft.FantasyLeagueId);

            var draftLockedDate = await _service.GetLeagueLockedDate(fantasyDraft.FantasyLeagueId);
            if (existingUserDraft.Count() > 0 && DateTime.UtcNow > draftLockedDate)
            {
                // If a user hasn't drafted yet let them add it in late, if they already have a draft though return a bad request cannot update
                return BadRequest("Draft is locked for this league");
            }

            object fantasyDraftPostResponse = null;

            // Fantasy Draft may be incomplete, so go through and add the IDs passed
            await _service.ClearUserFantasyPlayersAsync(userDiscordAccountId, fantasyDraft.FantasyLeagueId);
            for (int i = 0; i <= 4; i++)
            {
                fantasyDraftPostResponse = await _service.AddNewUserFantasyPlayerAsync(userDiscordAccountId, fantasyDraft.FantasyLeagueId, fantasyDraft.DraftPickPlayers[i].FantasyPlayerId, i + 1);
            }

            return CreatedAtAction(nameof(GetUserDraft), new { leagueId = fantasyDraft.FantasyLeagueId }, fantasyDraftPostResponse);
        }
    }
}
