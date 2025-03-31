using csharp_ef_webapi.Services;
using DataAccessLibrary.Models.Discord;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscordController : ControllerBase
    {

        private readonly DiscordWebApiService _discordWebApiService;
        private readonly FantasyService _fantasyService;

        public DiscordController(
            DiscordWebApiService discordWebApiService,
            FantasyService fantasyService
        )
        {
            _discordWebApiService = discordWebApiService;
            _fantasyService = fantasyService;
        }

        // GET: api/fantasydraft/5
        [Authorize]
        [HttpGet("balance")]
        public async Task<ActionResult<decimal>> GetUserBalance()
        {
            try
            {
                DiscordUser? discordUser = await _discordWebApiService.LookupHttpContextUser(HttpContext);

                if (discordUser == null)
                {
                    return NotFound();
                }

                return Ok(await _fantasyService.GetUserBalance(discordUser));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
