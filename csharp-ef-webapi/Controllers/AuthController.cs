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
            return Ok(new { authenticated = IsAuthenticated });
        }

        // GET: api/auth/authorization
        [Authorize]
        [HttpGet("authorization")]
        public IActionResult DiscordAuthorization()
        {
            var userClaims = HttpContext.User.Claims
                .Select(c => new { c.Type, c.Value })
                .ToList();

            return Ok(userClaims);
        }

        // GET: api/auth/login
        [Authorize]
        [HttpGet("login")]
        public IActionResult DiscordLogin()
        {
            return Ok("Login Successful");
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
