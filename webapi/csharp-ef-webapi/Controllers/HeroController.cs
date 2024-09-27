using Microsoft.AspNetCore.Mvc;
using DataAccessLibrary.Data;
using DataAccessLibrary.Models.ProMetadata;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeroController : ControllerBase
    {
        private readonly IProMetadataRepository _proMetadataRepository;

        public HeroController(IProMetadataRepository proMetadataRepository)
        {
            _proMetadataRepository = proMetadataRepository;
        }

        // GET: api/Hero
        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<Hero>>> GetHeroes()
        {
            return Ok(await _proMetadataRepository.GetHeroesAsync());
        }
    }
}
