#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccessLibrary.Data;
using DataAccessLibrary.Models.Discord;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscordIdController : ControllerBase
    {
        private readonly DiscordRepository _discordRepository;

        public DiscordIdController(DiscordRepository discordRepository)
        {
            _discordRepository = discordRepository;
        }
    }
}
