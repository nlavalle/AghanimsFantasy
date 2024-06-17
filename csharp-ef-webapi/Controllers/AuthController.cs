using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using csharp_ef_webapi.Data;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DiscordRepository _discordRepository;
        public AuthController(DiscordRepository discordRepository)
        {
            _discordRepository = discordRepository;
        }


        // GET: api/auth/authenticated
        [HttpGet("authenticated")]
        public IActionResult DiscordIsAuthenicated()
        {
            bool IsAuthenticated = HttpContext.User?.Identity?.IsAuthenticated ?? false;
            return Ok(new { authenticated = IsAuthenticated });
        }

        // GET: api/auth/authorization
        [Authorize]
        [HttpGet("authorization")]
        public async Task<IActionResult> DiscordAuthorization()
        {
            var userClaims = HttpContext.User.Claims
                .Select(c => new { c.Type, c.Value })
                .ToList();

            // Check database and assign user or admin role claim depending
            var nameId = HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (nameId == null) return BadRequest("Invalid User Claims please contact admin");

            bool getAccountId = long.TryParse(nameId.Value, out long userDiscordAccountId);
            var discordUser = await _discordRepository.GetDiscordUserAsync(userDiscordAccountId);

            if (discordUser != null && discordUser.IsAdmin)
            {
                userClaims.Add(new { Type = ClaimTypes.Role, Value = "admin" });
            }
            else
            {
                userClaims.Add(new { Type = ClaimTypes.Role, Value = "user" });
            }


            return Ok(userClaims);
        }

        // GET: api/auth/login
        [Authorize]
        [HttpGet("login")]
        public IActionResult DiscordLogin()
        {
            return NoContent();
        }

        // GET: api/auth/accessdenied
        [HttpGet("accessdenied")]
        public IActionResult DiscordLoginAccessDenied()
        {
            return BadRequest("OAuth Challenge failed");
        }

        // GET: api/auth/signout
        [Authorize]
        [HttpGet("signout")]
        public async Task<IActionResult> DiscordSignOut()
        {
            await HttpContext.SignOutAsync();

            return Ok();
        }
    }
}
