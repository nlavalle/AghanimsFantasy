using Microsoft.AspNetCore.Mvc;
using csharp_ef_webapi.Models;
using csharp_ef_webapi.Data;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeroController : ControllerBase
    {
        private readonly FantasyRepository _service;

        public HeroController(FantasyRepository service)
        {
            _service = service;
        }

        // GET: api/Hero/Heroes
        [HttpGet("heroes")]
        public async Task<ActionResult<IEnumerable<Hero>>> GetHeroes()
        {
            return Ok(await _service.GetHeroesAsync());
        }
    }
}
