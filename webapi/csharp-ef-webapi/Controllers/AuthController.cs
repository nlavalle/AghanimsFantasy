using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using csharp_ef_webapi.Extensions;
using DataAccessLibrary.Data.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using csharp_ef_webapi.ViewModels;
using System.Text.Json;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly SignInManager<AghanimsFantasyUser> _signInManager;
        private readonly UserManager<AghanimsFantasyUser> _userManager;
        public AuthController(
            ILogger<AuthController> logger,
            SignInManager<AghanimsFantasyUser> signInManager,
            UserManager<AghanimsFantasyUser> userManager)
        {
            _logger = logger;
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
                if (info.LoginProvider == "Discord") MapOldDiscordToNewUser(user, info);
                if (createResult.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: true);
                    await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                    return Ok("Login successful, you can close this window if it doesn't automatically close.");
                }
            }

            return Redirect(Url.Action(nameof(FailedOAuthAccessDenied)) ?? "/");
        }

        // POST: api/auth/change-display-name
        [Authorize]
        [HttpPost("change-display-name")]
        public async Task<IActionResult> ChangeDisplayName(ChangeDisplayName changeRequest)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return BadRequest("Unknown user");
            }

            user.DisplayName = changeRequest.DisplayName;
            await _userManager.UpdateAsync(user);

            return Ok("Display name successfully updated");
        }

        // GET: api/auth/authenticated
        [HttpGet("authenticated")]
        public IActionResult IsAuthenicated()
        {
            bool IsAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;
            return Ok(new { authenticated = IsAuthenticated });
        }

        // GET: api/auth/authorization
        [Authorize]
        [HttpGet("authorization")]
        public async Task<IActionResult> Authorization()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(currentUser);

            var responseObject = new
            {
                currentUser.Id,
                currentUser.DisplayName,
                currentUser.DiscordHandle,
                roles = userRoles.ToList()
            };

            return Ok(responseObject);
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
        public async Task<IActionResult> AuthSignOut()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        // POST: /api/auth/download-data
        /***
        * Directly lifting DownloadPersonalData from the aspnetcore identity library: https://github.com/dotnet/aspnetcore/blob/main/src/Identity/UI/src/Areas/Identity/Pages/V4/Account/Manage/DownloadPersonalData.cshtml.cs
        * because I'm not sure why the AddIdentityEndpoints doesn't include it/allow it?
        ***/
        [Authorize]
        [HttpPost("download-data")]
        public async Task<IActionResult> DownloadPersonalData()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(HttpContext.User)}");
            }

            _logger.LogInformation($"User '{_userManager.GetUserId(HttpContext.User)} requested to download their personal data");

            // Only include personal data for download
            var personalData = new Dictionary<string, string?>();
            var personalDataProps = typeof(AghanimsFantasyUser).GetProperties().Where(
                            prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            foreach (var p in personalDataProps)
            {
                personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
            }

            var logins = await _userManager.GetLoginsAsync(user);
            foreach (var l in logins)
            {
                personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
            }

            personalData.Add($"Authenticator Key", await _userManager.GetAuthenticatorKeyAsync(user));

            Response.Headers.Append("Content-Disposition", "attachment; filename=PersonalData.json");
            return new FileContentResult(JsonSerializer.SerializeToUtf8Bytes(personalData), "application/json");
        }

        // POST: /api/auth/delete-data
        /***
        * Mostly lifting DeletePersonalData from the aspnetcore identity library: https://github.com/dotnet/aspnetcore/blob/main/src/Identity/UI/src/Areas/Identity/Pages/V4/Account/Manage/DeletePersonalData.cshtml.cs
        * because I'm not sure why the AddIdentityEndpoints doesn't include it/allow it?
        ***/
        [Authorize]
        [HttpPost("delete-data")]
        public async Task<IActionResult> DeletePersonalData(DeletePersonalData deletePersonalData)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            bool requirePassword = await _userManager.HasPasswordAsync(user);
            if (requirePassword)
            {
                if (!await _userManager.CheckPasswordAsync(user, deletePersonalData.Password))
                {
                    return BadRequest("Incorrect password");
                }
            }

            var result = await _userManager.DeleteAsync(user);
            await _userManager.GetUserIdAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException("Unexpected error occurred deleting user.");
            }

            await _signInManager.SignOutAsync();

            _logger.LogInformation($"User '{user.Id} {user.UserName}' deleted themselves.");

            return Redirect("~/");
        }

        private void MapOldDiscordToNewUser(AghanimsFantasyUser user, ExternalLoginInfo loginInfo)
        {
            // Temporary function to map things

            // TODO: Map fantasy drafts
            // TODO: Map ledger
        }
    }
}
