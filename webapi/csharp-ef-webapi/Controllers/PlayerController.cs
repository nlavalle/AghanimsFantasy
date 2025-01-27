using Microsoft.AspNetCore.Mvc;
using DataAccessLibrary.Models;
using DataAccessLibrary.Data;
using DataAccessLibrary.Data.Facades;
using DataAccessLibrary.Models.ProMetadata;
using DataAccessLibrary.Models.Fantasy;
using Microsoft.AspNetCore.Authorization;
using csharp_ef_webapi.Services;
using DataAccessLibrary.Models.Discord;
using Microsoft.EntityFrameworkCore;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly AghanimsFantasyContext _dbContext;
        private readonly DiscordWebApiService _discordWebApiService;
        private readonly FantasyServiceAdmin _fantasyServiceAdmin;
        private readonly FantasyPointsFacade _fantasyPointsFacade;

        public PlayerController(
            AghanimsFantasyContext dbContext,
            DiscordWebApiService discordWebApiService,
            FantasyServiceAdmin fantasyServiceAdmin,
            FantasyPointsFacade fantasyPointsFacade
        )
        {
            _dbContext = dbContext;
            _discordWebApiService = discordWebApiService;
            _fantasyServiceAdmin = fantasyServiceAdmin;
            _fantasyPointsFacade = fantasyPointsFacade;
        }

        // GET: api/Player/account/{accountId}
        [HttpGet("account/{accountId}")]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccount(long? accountId)
        {
            if (accountId == null)
            {
                return BadRequest("Please specify an Account Id");
            }

            return Ok(await _dbContext.Accounts.FindAsync(accountId.Value));
        }

        // GET: api/Player/accounts
        [HttpGet("accounts")]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            return Ok(await _dbContext.Accounts.ToListAsync());
        }

        // GET: api/Player/1/topheroes
        [HttpGet("{fantasyPlayerId}/topheroes")]
        public async Task<ActionResult<FantasyPlayerTopHeroes>> GetTopHeroes(long? fantasyPlayerId)
        {
            if (fantasyPlayerId == null)
            {
                return BadRequest("Please specify a Fantasy Player Id");
            }

            return Ok(await _fantasyPointsFacade.GetFantasyPlayerTopHeroesAsync(fantasyPlayerId.Value));
        }

        // GET: api/player/1/fantasyaverages
        [HttpGet("{fantasyPlayerId}/fantasyaverages")]
        public async Task<ActionResult<IEnumerable<FantasyNormalizedAveragesTable>>> GetFantasyPlayerAverages(long? fantasyPlayerId)
        {
            if (fantasyPlayerId == null)
            {
                return BadRequest("Please specify a Fantasy Player Id");
            }

            return Ok(await _fantasyPointsFacade.GetFantasyNormalizedAveragesAsync(fantasyPlayerId.Value));
        }

        // POST: api/Player
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Account>> PostAccount(Account account)
        {
            // Admin only operation
            if (!await _discordWebApiService.CheckAdminUser(HttpContext))
            {
                return Unauthorized();
            }

            try
            {
                DiscordUser? discordUser = await _discordWebApiService.LookupHttpContextUser(HttpContext);
                if (discordUser == null)
                {
                    return Unauthorized();
                }

                await _fantasyServiceAdmin.AddAccountAsync(discordUser, account);
                return CreatedAtAction("GetAccount", new { accountId = account.Id }, account);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        // PUT: api/Player/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{accountId}")]
        public async Task<IActionResult> PutAccount(long accountId, Account updateAcount)
        {
            // Admin only operation
            if (!await _discordWebApiService.CheckAdminUser(HttpContext))
            {
                return Unauthorized();
            }

            try
            {
                DiscordUser? discordUser = await _discordWebApiService.LookupHttpContextUser(HttpContext);
                if (discordUser == null)
                {
                    return Unauthorized();
                }

                await _fantasyServiceAdmin.UpdateAccountAsync(discordUser, accountId, updateAcount);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Player/5
        [Authorize]
        [HttpDelete("{accountId}")]
        public async Task<IActionResult> DeleteAccount(long accountId)
        {
            // Admin only operation
            if (!await _discordWebApiService.CheckAdminUser(HttpContext))
            {
                return Unauthorized();
            }

            try
            {
                DiscordUser? discordUser = await _discordWebApiService.LookupHttpContextUser(HttpContext);
                if (discordUser == null)
                {
                    return Unauthorized();
                }

                await _fantasyServiceAdmin.DeleteAccountAsync(discordUser, accountId);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
