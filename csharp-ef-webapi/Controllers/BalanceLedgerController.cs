#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using csharp_ef_webapi.Models;

namespace csharp_ef_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BalanceLedgerController : ControllerBase
    {
        private readonly AghanimsWagerContext _context;

        public BalanceLedgerController(AghanimsWagerContext context)
        {
            _context = context;
        }

        // GET: api/BalanceLedger
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BalanceLedger>>> GetBalanceLedger()
        {
            return await _context.BalanceLedger.ToListAsync();
        }

        // GET: api/BalanceLedger/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BalanceLedger>> GetBalanceLedger(string id)
        {
            var balanceLedger = await _context.BalanceLedger.FindAsync(id);

            if (balanceLedger == null)
            {
                return NotFound();
            }

            return balanceLedger;
        }

        // PUT: api/BalanceLedger/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBalanceLedger(long id, BalanceLedger balanceLedger)
        {
            if (id != balanceLedger.DiscordId)
            {
                return BadRequest();
            }

            _context.Entry(balanceLedger).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BalanceLedgerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/BalanceLedger
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BalanceLedger>> PostBalanceLedger(BalanceLedger balanceLedger)
        {
            _context.BalanceLedger.Add(balanceLedger);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BalanceLedgerExists(balanceLedger.DiscordId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBalanceLedger", new { id = balanceLedger.DiscordId }, balanceLedger);
        }

        // DELETE: api/BalanceLedger/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBalanceLedger(long id)
        {
            var balanceLedger = await _context.BalanceLedger.FindAsync(id);
            if (balanceLedger == null)
            {
                return NotFound();
            }

            _context.BalanceLedger.Remove(balanceLedger);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BalanceLedgerExists(long id)
        {
            return _context.BalanceLedger.Any(e => e.DiscordId == id);
        }
    }
}
