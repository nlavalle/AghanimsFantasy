#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public AuthController()
        {
        }

        // GET: api/auth/authenticated
        [HttpGet("authenticated")]
        public IActionResult DiscordIsAuthenicated()
        {
            bool IsAuthenticated = HttpContext.User.Identity.IsAuthenticated;
            return Ok(IsAuthenticated);
        }

        // GET: api/auth/authorize
        [Authorize]
        [HttpGet("authorize")]
        public IActionResult DiscordAuthorize()
        {
            var userClaims = HttpContext.User.Claims
                .Select(c => new { c.Type, c.Value })
                .ToList();

            return Ok(userClaims);
        }

        // GET: api/auth/signout
        [Authorize]
        [HttpGet("signout")]
        public async Task<ActionResult> DiscordSignOut()
        {
            await HttpContext.SignOutAsync();

            return Ok();
        }
    }
}
