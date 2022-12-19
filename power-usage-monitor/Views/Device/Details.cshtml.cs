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
    public class DetailsModel : PageModel
    {
        private readonly power_usage_monitor.Models.power_usage_monitorContext _context;

        public DetailsModel(power_usage_monitor.Models.power_usage_monitorContext context)
        {
            _context = context;
        }

      public power_usage_monitor.Models.Device Device { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Devices == null)
            {
                return NotFound();
            }

            var device = await _context.Devices.FirstOrDefaultAsync(m => m.DeviceId == id);
            if (device == null)
            {
                return NotFound();
            }
            else 
            {
                Device = device;
            }
            return Page();
        }
    }
}
