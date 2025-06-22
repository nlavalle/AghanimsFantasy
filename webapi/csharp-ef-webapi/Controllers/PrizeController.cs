using csharp_ef_webapi.Services;
using DataAccessLibrary.Data;
using DataAccessLibrary.Data.Identity;
using DataAccessLibrary.Models.Fantasy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [OutputCache]
    public class PrizeController : ControllerBase
    {
        private readonly AghanimsFantasyContext _dbContext;
        private readonly UserManager<AghanimsFantasyUser> _userManager;
        private readonly FantasyService _fantasyService;

        public PrizeController(AghanimsFantasyContext dbContext, UserManager<AghanimsFantasyUser> userManager, FantasyService fantasyService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _fantasyService = fantasyService;
        }

        // GET: api/prize
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetPrizes()
        {
            string[] allPrizes = Enum.GetNames(typeof(FantasyPrizeOption));
            return Ok(allPrizes);
        }

        // POST: api/prize
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<string>>> PurchasePrize(string Prize)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            FantasyPrizeOption prizeCasted;
            if (Enum.TryParse(Prize, out prizeCasted))
            {
                try
                {
                    await _fantasyService.PurchasePrize(HttpContext.User, prizeCasted);
                }
                catch (ArgumentException ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return BadRequest("Not a valid prize");
            }
            string[] allPrizes = Enum.GetNames(typeof(FantasyPrizeOption));
            return Ok(allPrizes);
        }

        // GET: api/prize/unlocked
        [Authorize]
        [HttpGet("unlocked")]
        public async Task<ActionResult<IEnumerable<FantasyPrize>>> GetUnlockedPrizes()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return Ok(await _dbContext.FantasyPrizes.Where(fp => fp.UserId == user.Id).ToListAsync());
        }
    }
}
