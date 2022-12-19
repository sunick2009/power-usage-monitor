using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using power_usage_monitor.Models;


namespace power_usage_monitor.Views.Device
{
    public class IndexModel : PageModel
    {
        private readonly power_usage_monitor.Models.power_usage_monitorContext _context;

        public IndexModel(power_usage_monitor.Models.power_usage_monitorContext context)
        {
            _context = context;
        }

        public IList<power_usage_monitor.Models.Device> Device { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Devices != null)
            {
                Device = await _context.Devices
                .Include(d => d.Category).ToListAsync();
            }
        }
    }
}
