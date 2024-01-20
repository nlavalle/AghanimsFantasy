using Microsoft.AspNetCore.Mvc;
using csharp_ef_webapi.Models;
using csharp_ef_webapi.Data;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly FantasyRepository _service;

        public PlayerController(FantasyRepository service)
        {
            _service = service;
        }

        // GET: api/Player/accounts
        [HttpGet("accounts")]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            return Ok(await _service.GetPlayerAccounts());
        }
    }
}
