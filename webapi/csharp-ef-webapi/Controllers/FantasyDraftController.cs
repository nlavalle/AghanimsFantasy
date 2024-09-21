using Microsoft.AspNetCore.Mvc;
using DataAccessLibrary.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using DataAccessLibrary.Models;
using DataAccessLibrary.Models.Discord;
using DataAccessLibrary.Models.Fantasy;
using csharp_ef_webapi.Services;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FantasyDraftController : ControllerBase
    {
        private readonly IFantasyDraftRepository _fantasyDraftRepository;
        private readonly IFantasyLeagueRepository _fantasyLeagueRepository;
        private readonly IFantasyPlayerRepository _fantasyPlayerRepository;
        private readonly DiscordRepository _discordRepository;
        private readonly DiscordWebApiService _discordWebApiService;
        private readonly FantasyRepository _fantasyRepository;

        public FantasyDraftController(
            IFantasyDraftRepository fantasyDraftRepository,
            IFantasyLeagueRepository fantasyLeagueRepository,
            IFantasyPlayerRepository fantasyPlayerRepository,
            DiscordRepository discordRepository,
            DiscordWebApiService discordWebApiService,
            FantasyRepository fantasyRepository
        )
        {
            _fantasyDraftRepository = fantasyDraftRepository;
            _fantasyLeagueRepository = fantasyLeagueRepository;
            _fantasyPlayerRepository = fantasyPlayerRepository;
            _discordRepository = discordRepository;
            _discordWebApiService = discordWebApiService;
            _fantasyRepository = fantasyRepository;
        }

        // GET: api/draft/5/matches/points
        [Authorize]
        [HttpGet("{fantasyLeagueId}/matches/points")]
        public async Task<ActionResult<List<FantasyPlayerPoints>>> GetDraftFantasyPlayersPointsByMatch(int? fantasyLeagueId, [FromQuery] int? limit)
        {
            if (!HttpContext?.User?.Identity?.IsAuthenticated ?? false)
            {
                // Authorize should take care of this but just in case
                return BadRequest("User not authenticated");
            }

            if (fantasyLeagueId == null)
            {
                return BadRequest("Please provide a League ID to fetch fantasy player points of");
            }

            if (limit == null)
            {
                limit = 10;
            }

            if (limit > 100)
            {
                limit = 100;
            }

            var nameId = HttpContext!.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            DiscordUser? discordUser = nameId != null ? await _discordRepository.GetDiscordUserAsync(long.Parse(nameId.Value)) : null;

            if (discordUser == null)
            {
                return BadRequest("Invalid Discord User");
            }

            FantasyLeague? fantasyLeague = await _fantasyLeagueRepository.GetAccessibleFantasyLeague(fantasyLeagueId.Value, discordUser);

            if (fantasyLeague == null)
            {
                return BadRequest("Fantasy League Id not found.");
            }

            FantasyDraft? fantasyDraft = await _fantasyDraftRepository.GetByUserFantasyLeague(fantasyLeague, discordUser);

            if (fantasyDraft == null)
            {
                return BadRequest("Fantasy Draft not found.");
            }

            var fantasyPlayerPointTotals = await _fantasyDraftRepository.FantasyPlayerPointsAsync(fantasyDraft, limit.Value);
            return Ok(fantasyPlayerPointTotals);
        }


        // GET: api/draft/5
        [Authorize]
        [HttpGet("draft/{fantasyLeagueId}")]
        public async Task<IActionResult> GetUserDraft(int fantasyLeagueId)
        {
            if (!HttpContext.User?.Identity?.IsAuthenticated ?? false)
            {
                // Authorize should take care of this but just in case
                return BadRequest("User not authenticated");
            }

            var nameId = HttpContext!.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            bool getAccountId = long.TryParse(HttpContext!.User!.Claims!.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault()!.Value, out long userDiscordAccountId);

            if (!getAccountId)
            {
                return BadRequest("Could not retrieve user's discord ID");
            }

            DiscordUser? discordUser = nameId != null ? await _discordRepository.GetDiscordUserAsync(long.Parse(nameId.Value)) : null;

            if (discordUser == null)
            {
                return BadRequest("Invalid Discord User");
            }

            FantasyLeague? fantasyLeague = await _fantasyLeagueRepository.GetAccessibleFantasyLeague(fantasyLeagueId, discordUser);

            if (fantasyLeague == null)
            {
                return BadRequest("Fantasy League Id not found.");
            }

            return Ok(await _fantasyDraftRepository.GetByUserFantasyLeague(fantasyLeague, discordUser));
        }

        // GET: api/draft/5/points
        [Authorize]
        [HttpGet("{fantasyLeagueId}/points")]
        public async Task<IActionResult> GetUserDraftFantasyPoints(int fantasyLeagueId)
        {
            if (!HttpContext.User?.Identity?.IsAuthenticated ?? false)
            {
                // Authorize should take care of this but just in case
                return BadRequest("User not authenticated");
            }

            var nameId = HttpContext!.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            bool getAccountId = long.TryParse(HttpContext!.User!.Claims!.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault()!.Value, out long userDiscordAccountId);

            DiscordUser? discordUser = nameId != null ? await _discordRepository.GetDiscordUserAsync(long.Parse(nameId.Value)) : null;

            if (discordUser == null)
            {
                return BadRequest("Invalid Discord User");
            }

            FantasyLeague? fantasyLeague = await _fantasyLeagueRepository.GetAccessibleFantasyLeague(fantasyLeagueId, discordUser);

            if (fantasyLeague == null)
            {
                return BadRequest("Fantasy League Id not found.");
            }

            var fantasyPoints = await _fantasyPlayerRepository.GetFantasyLeaguePlayersAsync(fantasyLeague);
            if (fantasyPoints.Count() == 0)
            {
                // League doesn't have fantasy players/points yet
                return Ok(new { });
            }

            FantasyDraft? fantasyDraft = await _fantasyDraftRepository.GetByUserFantasyLeague(fantasyLeague, discordUser);
            if (fantasyDraft == null)
            {
                return Ok(new { });
            }

            var userDraftPoints = await _fantasyDraftRepository.DraftPointTotalAsync(fantasyDraft);

            return Ok(userDraftPoints);
        }

        // POST: api/draft
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost("")]
        public async Task<ActionResult> PostUserDraft(FantasyDraft fantasyDraft)
        {
            if (!HttpContext.User?.Identity?.IsAuthenticated ?? false)
            {
                // Authorize should take care of this but just in case
                return BadRequest("User not authenticated");
            }

            var nameId = HttpContext!.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            bool getAccountId = long.TryParse(HttpContext!.User!.Claims!.Where(c => c.Type == ClaimTypes.NameIdentifier).FirstOrDefault()!.Value, out long userDiscordAccountId);

            if (!getAccountId)
            {
                return BadRequest("Could not retrieve user's discord ID");
            }

            DiscordUser? discordUser = await _discordRepository.GetDiscordUserAsync(userDiscordAccountId);
            if (discordUser == null)
            {
                // New discord user, run call to look them up
                await _discordWebApiService.GetDiscordByIdAsync(userDiscordAccountId);
            }

            FantasyDraft? existingUserDraft = await _fantasyDraftRepository.GetByUserFantasyLeague(fantasyDraft.FantasyLeague, discordUser!);
            var draftLockedDate = await _fantasyLeagueRepository.GetLeagueLockedDate(fantasyDraft.FantasyLeague.Id);

            if (DateTime.UtcNow > draftLockedDate)
            {
                // TODO: Set this up so that a user can draft late, but then the points only count starting from that time
                // If a user hasn't drafted yet let them add it in late, if they already have a draft though return a bad request cannot update
                return BadRequest("Draft is locked for this league");
            }

            // Ensure player has posted a draft that is one of each team position, if there's 2 of the same position then reject it as a bad request
            var fantasyPlayers = await _fantasyPlayerRepository.GetFantasyLeaguePlayersAsync(fantasyDraft.FantasyLeague);
            if (fantasyPlayers.Where(fp => fantasyDraft.DraftPickPlayers.Where(dpp => dpp.FantasyPlayer != null).Any(dpp => dpp.FantasyPlayer!.Id == fp.Id)).GroupBy(fp => fp.TeamPosition).Where(grp => grp.Count() > 1).Count() > 0)
            {
                return BadRequest("Can only draft one of each team position");
            };

            object fantasyDraftPostResponse = fantasyDraft;

            // Fantasy Draft may be incomplete, so go through and add the IDs passed
            await _fantasyDraftRepository.ClearPicksAsync(fantasyDraft);
            for (int i = 0; i <= 4; i++)
            {
                if (fantasyDraft.DraftPickPlayers[i].FantasyPlayer != null)
                {
                    fantasyDraftPostResponse = await _fantasyDraftRepository.AddPlayerPickAsync(fantasyDraft, fantasyDraft.DraftPickPlayers[i].FantasyPlayer!);
                }
            }

            return CreatedAtAction(nameof(GetUserDraft), new { fantasyLeagueId = fantasyDraft.FantasyLeague.Id }, fantasyDraftPostResponse);
        }
    }
}
