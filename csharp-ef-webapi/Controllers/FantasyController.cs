#nullable disable
using System.Security.Claims;
using csharp_ef_webapi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FantasyController : ControllerBase
    {
        private readonly AghanimsFantasyContext _dbContext;
        public FantasyController(AghanimsFantasyContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/fantasy/players
        [HttpGet("players")]
        public async Task<List<FantasyPlayer>> GetFantasyPlayers()
        {
            var players = await _dbContext.FantasyPlayers
                            .Include(fp => fp.Team)
                            .Include(fp => fp.DotaAccount)
                            .OrderBy(fp => fp.Team.Name)
                            .ThenBy(fp => fp.DotaAccount.Name)
                            .ToListAsync();
            return players;
        }

        // GET: api/fantasy/players/5
        [HttpGet("players/{leagueId}")]
        public async Task<List<FantasyPlayer>> GetFantasyPlayers(int? leagueId)
        {
            var players = await _dbContext.FantasyPlayers
                            .Where(fp => fp.LeagueId == leagueId)
                            .Include(fp => fp.Team)
                            .Include(fp => fp.DotaAccount)
                            .OrderBy(fp => fp.Team.Name)
                            .ThenBy(fp => fp.DotaAccount.Name)
                            .ToListAsync();
            return players;
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

            var userDraft = await _dbContext.FantasyDrafts
                                    .Where(fd => fd.LeagueId == leagueId && fd.DiscordAccountId == userDiscordAccountId)
                                    .Include(fd => fd.DraftPickPlayers)
                                        .ThenInclude(fp => fp.FantasyPlayer)
                                        .ThenInclude(fp => fp.Team)
                                    .Include(fd => fd.DraftPickPlayers)
                                        .ThenInclude(fp => fp.FantasyPlayer)
                                        .ThenInclude(fp => fp.DotaAccount)
                                    .Select(fdp => new
                                    {
                                        fdp.DiscordAccountId,
                                        fdp.LeagueId,
                                        fdp.DraftCreated,
                                        fdp.DraftLastUpdated,
                                        Players = fdp.DraftPickPlayers.OrderBy(dpp => dpp.DraftOrder).Select(dpp => dpp.FantasyPlayer).ToList()
                                    })
                                    .ToListAsync();
            return Ok(userDraft);
        }

        // POST: api/fantasy/draft
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost("draft/{leagueId}")]
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

            fantasyDraft.DraftLastUpdated = DateTime.UtcNow;
            fantasyDraft.DiscordAccountId = userDiscordAccountId;
            FantasyPlayer pickOne = await _dbContext.FantasyPlayers.FindAsync(fantasyDraft.DraftPickOne);
            FantasyPlayer pickTwo = await _dbContext.FantasyPlayers.FindAsync(fantasyDraft.DraftPickTwo);
            FantasyPlayer pickThree = await _dbContext.FantasyPlayers.FindAsync(fantasyDraft.DraftPickThree);
            FantasyPlayer pickFour = await _dbContext.FantasyPlayers.FindAsync(fantasyDraft.DraftPickFour);
            FantasyPlayer pickFive = await _dbContext.FantasyPlayers.FindAsync(fantasyDraft.DraftPickFive);

            var updateDraft = await _dbContext.FantasyDrafts.Where(fd => fd.LeagueId == fantasyDraft.LeagueId && fd.DiscordAccountId == userDiscordAccountId).FirstOrDefaultAsync();
            fantasyDraft.DraftCreated = updateDraft?.DraftCreated ?? DateTime.UtcNow;

            List<FantasyDraftPlayer> newDraftPlayers = new List<FantasyDraftPlayer>
            {
                new FantasyDraftPlayer() { FantasyPlayer = pickOne, DraftOrder = 1 },
                new FantasyDraftPlayer() { FantasyPlayer = pickTwo, DraftOrder = 2  },
                new FantasyDraftPlayer() { FantasyPlayer = pickThree, DraftOrder = 3  },
                new FantasyDraftPlayer() { FantasyPlayer = pickFour, DraftOrder = 4  },
                new FantasyDraftPlayer() { FantasyPlayer = pickFive, DraftOrder = 5  }
            };
            fantasyDraft.DraftPickPlayers = newDraftPlayers;

            // Update draft if it exists else insert a new one
            if (updateDraft == null)
            {
                _dbContext.FantasyDrafts.Add(fantasyDraft);
            }
            else
            {
                List<FantasyDraftPlayer> fantasyDraftPlayers = await _dbContext.FantasyDraftPlayers.Where(fdp => fdp.FantasyDraftId == updateDraft.Id).ToListAsync();
                _dbContext.FantasyDraftPlayers.RemoveRange(fantasyDraftPlayers);

                updateDraft.DraftPickOne = fantasyDraft.DraftPickOne;
                updateDraft.DraftPickTwo = fantasyDraft.DraftPickTwo;
                updateDraft.DraftPickThree = fantasyDraft.DraftPickThree;
                updateDraft.DraftPickFour = fantasyDraft.DraftPickFour;
                updateDraft.DraftPickFive = fantasyDraft.DraftPickFive;
                updateDraft.DraftLastUpdated = fantasyDraft.DraftLastUpdated;
                updateDraft.DraftPickPlayers = fantasyDraft.DraftPickPlayers;
                _dbContext.FantasyDrafts.Update(updateDraft);
            }

            await _dbContext.SaveChangesAsync();

            var createOutputFormatted = new
            {
                fantasyDraft.DiscordAccountId,
                fantasyDraft.LeagueId,
                fantasyDraft.DraftCreated,
                fantasyDraft.DraftLastUpdated,
                Players = fantasyDraft.DraftPickPlayers.Select(dpp => dpp.FantasyPlayer).ToList()
            };

            return CreatedAtAction(nameof(GetUserDraft), new { leagueId = fantasyDraft.LeagueId }, createOutputFormatted);
        }
    }
}
