using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using csharp_ef_webapi.Extensions;
using DataAccessLibrary.Data.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<AghanimsFantasyUser> _signInManager;
        private readonly UserManager<AghanimsFantasyUser> _userManager;
        public AuthController(SignInManager<AghanimsFantasyUser> signInManager, UserManager<AghanimsFantasyUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet("login-provider")]
        public async Task<IActionResult> LoginProvider(string provider)
        {
            if (string.IsNullOrWhiteSpace(provider))
            {
                return BadRequest("Provider query parameter missing.");
            }

            if (!await HttpContext.IsProviderSupportedAsync(provider))
            {
                return BadRequest("Provider not supported.");
            }

            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Auth", new { returnUrl = "/" });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet("callback-provider")]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = "/")
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return Redirect(Url.Action(nameof(FailedOAuthAccessDenied)) ?? "/");
            }

            // Try to login the user into identity with the external login
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true);

            if (result.Succeeded)
            {
                // User signed in with external login
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme); // clean up identity external cookie now that they have aspnet cookie
                return Ok("Login successful, you can close this window if it doesn't automatically close.");
            }

            // User doesn't exist with that external login, create a new user
            var user = new AghanimsFantasyUser();

            switch (info.LoginProvider)
            {
                case "Google":
                    user.UserName = info.Principal.FindFirstValue(ClaimTypes.Name);
                    user.Email = info.Principal.FindFirstValue(ClaimTypes.Email);
                    break;
                case "Discord":
                    user.UserName = info.Principal.FindFirstValue(ClaimTypes.Name);
                    if (long.TryParse(info.Principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty, out long discordId))
                    {
                        user.DiscordId = discordId;
                    }
                    user.DiscordHandle = info.Principal.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
                    // We don't get email from discord privileges
                    break;
            }

            var createResult = await _userManager.CreateAsync(user);
            if (createResult.Succeeded)
            {
                createResult = await _userManager.AddLoginAsync(user, info);
                if (createResult.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: true);
                    await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                    return Ok("Login successful, you can close this window if it doesn't automatically close.");
                }
            }

            return Redirect(Url.Action(nameof(FailedOAuthAccessDenied)) ?? "/");
        }

        // GET: api/auth/authenticated
        [HttpGet("authenticated")]
        public IActionResult IsAuthenicated()
        {
            bool IsAuthenticated = HttpContext.User?.Identity?.IsAuthenticated ?? false;
            return Ok(new { authenticated = IsAuthenticated });
        }

        // GET: api/auth/authorization
        [Authorize]
        [HttpGet("authorization")]
        public IActionResult Authorization()
        {
            List<string> userClaims = HttpContext.User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();
            return Ok(userClaims);
        }

        // GET: api/auth/accessdenied
        [HttpGet("accessdenied")]
        public IActionResult FailedOAuthAccessDenied()
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
