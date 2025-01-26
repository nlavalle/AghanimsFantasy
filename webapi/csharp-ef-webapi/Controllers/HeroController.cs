using DataAccessLibrary.Data;
using DataAccessLibrary.Models.ProMetadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeroController : ControllerBase
    {
        private readonly AghanimsFantasyContext _dbContext;

        public HeroController(AghanimsFantasyContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/Hero
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Hero>>> GetHeroes()
        {
            return Ok(await _dbContext.Heroes.ToListAsync());
        }
    }
}
