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
    public class UsageController : ControllerBase
    {
        private readonly power_usage_monitorContext _context;

        public UsageController(power_usage_monitorContext context)
        {
            _context = context;
        }

        // GET: api/Usage
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usage>>> GetUsages()
        {
            return await _context.Usages.ToListAsync();
        }

        // GET: api/Usage/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usage>> GetUsage(DateTime id)
        {
            var usage = await _context.Usages.FindAsync(id);

            if (usage == null)
            {
                return NotFound();
            }

            return usage;
        }


    }
}
