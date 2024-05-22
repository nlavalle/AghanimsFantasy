using Microsoft.AspNetCore.Mvc;
using csharp_ef_webapi.Models;
using csharp_ef_webapi.Data;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly FantasyRepository _service;

        public PlayerController(FantasyRepository service)
        {
            _service = service;
        }

        // GET: api/player/accounts
        [HttpGet("accounts")]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            return Ok(await _service.GetPlayerAccounts());
        }

        // GET: api/player/1/topheroes
        [HttpGet("{fantasyPlayerId}/topheroes")]
        public async Task<ActionResult<FantasyPlayerTopHeroes>> GetTopHeroes(long? fantasyPlayerId)
        {
            if (fantasyPlayerId == null)
            {
                return BadRequest("Please specify a Fantasy Player Id");
            }

            return Ok(await _service.GetFantasyPlayerTopHeroesAsync(fantasyPlayerId.Value));
        }

        // GET: api/player/1/fantasyaverages
        [HttpGet("{fantasyPlayerId}/fantasyaverages")]
        public async Task<ActionResult<IEnumerable<FantasyNormalizedAverages>>> GetFantasyPlayerAverages(long? fantasyPlayerId)
        {
            if (fantasyPlayerId == null)
            {
                return BadRequest("Please specify a Fantasy Player Id");
            }

            return Ok(await _service.GetFantasyNormalizedAveragesAsync(fantasyPlayerId.Value));
        }
    }
}
