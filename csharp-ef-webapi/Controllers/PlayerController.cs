using Microsoft.AspNetCore.Mvc;
using csharp_ef_webapi.Models;
using csharp_ef_webapi.Data;
using csharp_ef_webapi.Models.ProMetadata;
using csharp_ef_webapi.Models.Fantasy;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly FantasyRepository _fantasyRepository;
        private readonly ProMetadataRepository _proMetadataRepository;

        public PlayerController(FantasyRepository fantasyRepository, ProMetadataRepository proMetadataRepository)
        {
            _fantasyRepository = fantasyRepository;
            _proMetadataRepository = proMetadataRepository;
        }

        // GET: api/player/accounts
        [HttpGet("accounts")]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            return Ok(await _proMetadataRepository.GetPlayerAccounts());
        }

        // GET: api/player/1/topheroes
        [HttpGet("{fantasyPlayerId}/topheroes")]
        public async Task<ActionResult<FantasyPlayerTopHeroes>> GetTopHeroes(long? fantasyPlayerId)
        {
            if (fantasyPlayerId == null)
            {
                return BadRequest("Please specify a Fantasy Player Id");
            }

            return Ok(await _fantasyRepository.GetFantasyPlayerTopHeroesAsync(fantasyPlayerId.Value));
        }

        // GET: api/player/1/fantasyaverages
        [HttpGet("{fantasyPlayerId}/fantasyaverages")]
        public async Task<ActionResult<IEnumerable<FantasyNormalizedAveragesTable>>> GetFantasyPlayerAverages(long? fantasyPlayerId)
        {
            if (fantasyPlayerId == null)
            {
                return BadRequest("Please specify a Fantasy Player Id");
            }

            return Ok(await _fantasyRepository.GetFantasyNormalizedAveragesAsync(fantasyPlayerId.Value));
        }
    }
}
