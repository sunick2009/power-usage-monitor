using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using power_usage_monitor.Models;
using System.Diagnostics;

namespace power_usage_monitor.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		private readonly power_usage_monitorContext _context;
		public HomeController(ILogger<HomeController> logger, power_usage_monitorContext context)
        {
            _logger = logger;
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
			var power_usage_monitorContext = _context.Devices.Include(d => d.Category).OrderBy(e => e.DeviceId);
            foreach (var item in power_usage_monitorContext)
            {
                ViewData["plug"+(item.DeviceId-1).ToString()] = item.DeviceName;
            }
			return View(await power_usage_monitorContext.ToListAsync());
        }

        public IActionResult Abnormal()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}