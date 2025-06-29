using csharp_ef_webapi.Extensions;
using csharp_ef_webapi.Services;
using csharp_ef_webapi.ViewModels;
using DataAccessLibrary.Data.Facades;
using DataAccessLibrary.Data.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
        private readonly FantasyDraftFacade _fantasyDraftFacade;
        private readonly DiscordFacade _discordFacade;
        private readonly FantasyService _fantasyService;
        public AuthController(
            ILogger<AuthController> logger,
            SignInManager<AghanimsFantasyUser> signInManager,
            UserManager<AghanimsFantasyUser> userManager,
            FantasyDraftFacade fantasyDraftFacade,
            DiscordFacade discordFacade,
            FantasyService fantasyService)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _fantasyDraftFacade = fantasyDraftFacade;
            _discordFacade = discordFacade;
            _fantasyService = fantasyService;
        }

        [HttpGet("login-provider")]
        public async Task<IActionResult> LoginProvider(string provider, string returnUrl = "/")
        {
            if (string.IsNullOrWhiteSpace(provider))
            {
                return BadRequest("Provider query parameter missing.");
            }

            if (!await HttpContext.IsProviderSupportedAsync(provider))
            {
                return BadRequest("Provider not supported.");
            }

            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Auth", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        [HttpGet("callback-provider")]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = "/")
        {
            var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (externalLoginInfo == null)
            {
                return Redirect(Url.Action(nameof(FailedOAuthAccessDenied)) ?? returnUrl);
            }

            // Check if user is already logged in, if so add it to the current user
            var existingUser = await _userManager.GetUserAsync(HttpContext.User);
            if (existingUser != null)
            {
                return await AddExternalLoginToUserAsync(existingUser, externalLoginInfo, returnUrl);
            }

            // Check if a user already exists for this external login info
            var externalLoginLookup = await _userManager.FindByLoginAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey);
            if (externalLoginLookup != null)
            {
                return await LoginExternalUserAsync(externalLoginLookup, externalLoginInfo, returnUrl);
            }

            // User doesn't exist with that external login, create a new user
            return await CreateUserFromExternalLoginAsync(externalLoginInfo, returnUrl);
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

        // GET: api/auth/external-logins
        [Authorize]
        [HttpGet("external-logins")]
        public async Task<IActionResult> GetExternalLogins()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return BadRequest("Unknown user");
            }

            var logins = await _userManager.GetLoginsAsync(user);
            if (await _userManager.HasPasswordAsync(user))
            {
                logins.Add(new UserLoginInfo("Email", user.Id, user.Email));
            }
            return Ok(logins);
        }

        // POST: api/auth/add-email
        [Authorize]
        [HttpPost("add-email")]
        public async Task<IActionResult> AddEmailLogin(AddEmail addEmailRequest)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return BadRequest("Unknown user");
            }

            if (await _userManager.HasPasswordAsync(user))
            {
                return BadRequest("User already has email/password registered");
            }

            user.UserName = addEmailRequest.Email;
            user.Email = addEmailRequest.Email;
            var addPasswordResult = await _userManager.AddPasswordAsync(user, addEmailRequest.Password);

            if (!addPasswordResult.Succeeded)
            {
                return BadRequest(string.Join(',', addPasswordResult.Errors.Select(err => err.Description).ToList()));
            }

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return BadRequest(string.Join(',', updateResult.Errors.Select(err => err.Description).ToList()));
            }

            var logins = await _userManager.GetLoginsAsync(user);
            if (await _userManager.HasPasswordAsync(user))
            {
                logins.Add(new UserLoginInfo("Email", user.Id, user.Email));
            }
            return Ok(logins);
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

            var userPrizes = await _fantasyService.GetUserPrizes(HttpContext.User);

            var responseObject = new
            {
                currentUser.Id,
                currentUser.DisplayName,
                currentUser.DiscordHandle,
                roles = userRoles.ToList(),
                prizes = userPrizes.ToList(),
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

        private async Task<IActionResult> CreateUserFromExternalLoginAsync(ExternalLoginInfo externalLoginInfo, string returnUrl)
        {
            var user = new AghanimsFantasyUser();

            switch (externalLoginInfo.LoginProvider)
            {
                case "Google":
                    user.UserName = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
                    user.Email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email);
                    break;
                case "Discord":
                    user.UserName = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Name);
                    if (long.TryParse(externalLoginInfo.Principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty, out long discordId))
                    {
                        user.DiscordId = discordId;
                    }
                    user.DiscordHandle = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
                    user.Email = "";
                    // We don't get email from discord privileges
                    break;
            }

            var createResult = await _userManager.CreateAsync(user);
            if (createResult.Succeeded)
            {
                createResult = await _userManager.AddLoginAsync(user, externalLoginInfo);
                if (externalLoginInfo.LoginProvider == "Discord") await MapOldDiscordToNewUser(user, externalLoginInfo);
                if (createResult.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: true);
                    await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                    return Redirect($"/oauth-callback.html?success=true&origin={Uri.EscapeDataString(returnUrl)}");
                }
            }
            else
            {
                _logger.LogError($"Unexpected error during UserManager.CreateAsync: {string.Join(',', createResult.Errors.Select(err => err.Description).ToList())}");
                return Redirect($"/oauth-callback.html?success=false&origin={Uri.EscapeDataString(returnUrl)}");
            }

            return Redirect($"/oauth-callback.html?success=false&origin={Uri.EscapeDataString(returnUrl)}");
        }

        private async Task<IActionResult> AddExternalLoginToUserAsync(AghanimsFantasyUser externalLoginUser, ExternalLoginInfo externalLoginInfo, string returnUrl)
        {
            var addLoginResult = await _userManager.AddLoginAsync(externalLoginUser, externalLoginInfo);
            if (!addLoginResult.Succeeded)
            {
                _logger.LogError("Unexpected error during AddLoginAsync: " + string.Join(',', addLoginResult.Errors.Select(err => err.Description).ToList()));
                return BadRequest("Error: " + string.Join(',', addLoginResult.Errors.Select(err => err.Description).ToList()));
            }
            if (externalLoginInfo.LoginProvider == "Discord")
            {
                if (long.TryParse(externalLoginInfo.Principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty, out long discordId))
                {
                    externalLoginUser.DiscordId = discordId;
                }
                externalLoginUser.DiscordHandle = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
                await MapOldDiscordToNewUser(externalLoginUser, externalLoginInfo);
                await _userManager.UpdateAsync(externalLoginUser);
            }
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme); // clean up identity external cookie now that they have aspnet cookie
            return Redirect($"/oauth-callback.html?success=true&origin={Uri.EscapeDataString(returnUrl)}");
        }

        private async Task<RedirectResult> LoginExternalUserAsync(AghanimsFantasyUser externalLoginUser, ExternalLoginInfo externalLoginInfo, string returnUrl)
        {
            var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(externalLoginUser);

            // Try to login the user into identity with the external login
            var result = await _signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey, isPersistent: true);
            if (result.Succeeded)
            {
                // User signed in with external login
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme); // clean up identity external cookie now that they have aspnet cookie
                if (!isEmailConfirmed)
                {
                    // We can log them on but display the email confirmation pending
                    return Redirect($"/oauth-callback.html?success=true&error=EmailNotConfirmed&email={externalLoginUser.NormalizedEmail}&origin={Uri.EscapeDataString(returnUrl)}");
                }
                else
                {
                    return Redirect($"/oauth-callback.html?success=true&origin={Uri.EscapeDataString(returnUrl)}");
                }
            }
            else if (result.IsNotAllowed)
            {
                return Redirect($"/oauth-callback.html?success=false&error=SignInNotAllowed&origin={Uri.EscapeDataString(returnUrl)}");
            }
            else
            {
                return Redirect($"/oauth-callback.html?success=false&origin={Uri.EscapeDataString(returnUrl)}");
            }
        }

        private async Task MapOldDiscordToNewUser(AghanimsFantasyUser user, ExternalLoginInfo loginInfo)
        {
            // Temporary function to map things for historical doto playos
            if (long.TryParse(loginInfo.ProviderKey, out var discordId))
            {
                _logger.LogInformation($"Linking previous Discord Id {discordId} to new user {user.UserName}");
                // Map fantasy drafts
                await _fantasyDraftFacade.HistoricalUpdateDiscordUserDraftsAsync(discordId, user);

                // Map ledger
                await _discordFacade.HistoricalUpdateDiscordFantasyLedgersAsync(discordId, user);
            }
        }
    }
}
