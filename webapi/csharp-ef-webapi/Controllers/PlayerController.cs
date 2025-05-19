using csharp_ef_webapi.Extensions;
using csharp_ef_webapi.Services;
using DataAccessLibrary.Data;
using DataAccessLibrary.Models.ProMetadata;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [OutputCache(Tags = new[] { "player" })]
    public class PlayerController : ControllerBase
    {
        private readonly AghanimsFantasyContext _dbContext;
        private readonly FantasyServiceAdmin _fantasyServiceAdmin;
        private readonly IOutputCacheStore _cache;

        public PlayerController(
            AghanimsFantasyContext dbContext,
            FantasyServiceAdmin fantasyServiceAdmin,
            IOutputCacheStore cache
        )
        {
            _dbContext = dbContext;
            _fantasyServiceAdmin = fantasyServiceAdmin;
            _cache = cache;
        }

        // GET: api/Player/account/{accountId}
        [HttpGet("account/{accountId}")]
        [ResponseETag]
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
        [ResponseETag]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            return Ok(await _dbContext.Accounts.ToListAsync());
        }

        // POST: api/Player
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")] // Admin only operation
        [HttpPost]
        public async Task<ActionResult<Account>> PostAccount(Account account)
        {
            try
            {
                await _fantasyServiceAdmin.AddAccountAsync(account);
                await _cache.EvictByTagAsync("player", default);
                return CreatedAtAction("GetAccount", new { accountId = account.Id }, account);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        // PUT: api/Player/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "Admin")] // Admin only operation
        [HttpPut("{accountId}")]
        public async Task<IActionResult> PutAccount(long accountId, Account updateAcount)
        {
            try
            {
                await _fantasyServiceAdmin.UpdateAccountAsync(accountId, updateAcount);
                await _cache.EvictByTagAsync("player", default);
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
        [Authorize(Roles = "Admin")] // Admin only operation
        [HttpDelete("{accountId}")]
        public async Task<IActionResult> DeleteAccount(long accountId)
        {
            try
            {
                await _fantasyServiceAdmin.DeleteAccountAsync(accountId);
                await _cache.EvictByTagAsync("player", default);
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
