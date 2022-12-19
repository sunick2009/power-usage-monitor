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
        public async Task<ActionResult<IEnumerable<Usage?>>> GetUsages()
        {
            var curDataTime = await _context.Usages.OrderBy(e => e.Time).LastAsync();
            DateTime curTime = curDataTime.Time;
            var curUsage =  _context.Usages.GroupBy(e => e.DeviceId)
                .Select(gr => 
                gr.Where(e => e.Time == curTime)
                .FirstOrDefault())
                .AsEnumerable().ToList();
            return curUsage;
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
