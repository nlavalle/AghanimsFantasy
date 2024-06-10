using Microsoft.AspNetCore.Mvc;
using csharp_ef_webapi.Data;
using csharp_ef_webapi.Models.ProMetadata;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeroController : ControllerBase
    {
        private readonly ProMetadataRepository _proMetadataRepository;

        public HeroController(ProMetadataRepository proMetadataRepository)
        {
            _proMetadataRepository = proMetadataRepository;
        }

        // GET: api/Hero/Heroes
        [HttpGet("heroes")]
        public async Task<ActionResult<IEnumerable<Hero>>> GetHeroes()
        {
            return Ok(await _proMetadataRepository.GetHeroesAsync());
        }
    }
}
