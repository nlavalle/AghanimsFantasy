using Microsoft.AspNetCore.Mvc;
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
        private readonly DiscordWebApiService _discordWebApiService;
        private readonly FantasyService _fantasyService;

        public FantasyDraftController(
            DiscordWebApiService discordWebApiService,
            FantasyService fantasyService
        )
        {
            _discordWebApiService = discordWebApiService;
            _fantasyService = fantasyService;
        }

        // GET: api/draft/5
        [Authorize]
        [HttpGet("{fantasyLeagueId}")]
        public async Task<IActionResult> GetUserDraft(int fantasyLeagueId)
        {
            try
            {
                DiscordUser? discordUser = await _discordWebApiService.LookupHttpContextUser(HttpContext);

                if (discordUser == null)
                {
                    return NotFound();
                }

                return Ok(await _fantasyService.GetFantasyDraft(discordUser, fantasyLeagueId));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/draft/5/points
        [Authorize]
        [HttpGet("{fantasyLeagueId}/points")]
        public async Task<IActionResult> GetUserDraftFantasyPoints(int fantasyLeagueId)
        {
            try
            {
                DiscordUser? discordUser = await _discordWebApiService.LookupHttpContextUser(HttpContext);

                if (discordUser == null)
                {
                    return NotFound();
                }

                FantasyDraftPointTotals? fantasyDraftPointTotals = await _fantasyService.GetFantasyDraftPointTotal(discordUser, fantasyLeagueId);
                if (fantasyDraftPointTotals == null)
                {
                    return Ok(new { });
                }

                return Ok(fantasyDraftPointTotals);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/draft/5/matches/points
        [Authorize]
        [HttpGet("{fantasyLeagueId}/matches/points")]
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
                DiscordUser? discordUser = await _discordWebApiService.LookupHttpContextUser(HttpContext);

                if (discordUser == null)
                {
                    return NotFound();
                }

                return Ok(await _fantasyService.GetFantasyPlayerPoints(discordUser, fantasyLeagueId.Value, limit.Value));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/draft
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost("")]
        public async Task<ActionResult> PostUserDraft(FantasyDraft fantasyDraft)
        {
            try
            {
                DiscordUser? discordUser = await _discordWebApiService.LookupHttpContextUser(HttpContext);

                if (discordUser == null)
                {
                    return NotFound();
                }

                var fantasyDraftPostResponse = await _fantasyService.UpdateFantasyDraft(discordUser, fantasyDraft);
                return CreatedAtAction(nameof(GetUserDraft), new { fantasyLeagueId = fantasyDraft.FantasyLeague.Id }, fantasyDraftPostResponse);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}