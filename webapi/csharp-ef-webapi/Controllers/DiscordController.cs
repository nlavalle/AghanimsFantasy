using csharp_ef_webapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscordController : ControllerBase
    {
        private readonly FantasyService _fantasyService;

        public DiscordController(
            FantasyService fantasyService
        )
        {
            _fantasyService = fantasyService;
        }

        // GET: api/fantasydraft/5
        [Authorize]
        [HttpGet("balance")]
        public async Task<ActionResult<decimal>> GetUserBalance()
        {
            try
            {
                return Ok(await _fantasyService.GetUserBalance(HttpContext.User));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
