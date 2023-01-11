using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using power_usage_monitor.Models;

namespace power_usage_monitor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AbnormalsController : ControllerBase
    {
        private readonly power_usage_monitorContext _context;

        public AbnormalsController(power_usage_monitorContext context)
        {
            _context = context;
        }

        // GET: api/Abnormals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Abnormal>>> GetAbnormals()
        {
            return await _context.Abnormals.ToListAsync();
        }

        // GET: api/Abnormals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Abnormal>> GetAbnormal(int id)
        {
            var abnormal = await _context.Abnormals.FindAsync(id);

            if (abnormal == null)
            {
                return NotFound();
            }

            return abnormal;
        }

        // PUT: api/Abnormals/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAbnormal(int id, Abnormal abnormal)
        {
            if (id != abnormal.AbnormalId)
            {
                return BadRequest();
            }

            _context.Entry(abnormal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AbnormalExists(id))
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

        // POST: api/Abnormals
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Abnormal>> PostAbnormal(Abnormal abnormal)
        {
            _context.Abnormals.Add(abnormal);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AbnormalExists(abnormal.AbnormalId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAbnormal", new { id = abnormal.AbnormalId }, abnormal);
        }

        // DELETE: api/Abnormals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAbnormal(int id)
        {
            var abnormal = await _context.Abnormals.FindAsync(id);
            if (abnormal == null)
            {
                return NotFound();
            }

            _context.Abnormals.Remove(abnormal);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AbnormalExists(int id)
        {
            return _context.Abnormals.Any(e => e.AbnormalId == id);
        }
    }
}
