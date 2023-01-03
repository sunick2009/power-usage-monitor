using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using power_usage_monitor.Models;

namespace power_usage_monitor.Controllers
{
    public class DeviceController : Controller
    {
        private readonly power_usage_monitorContext _context;

        public DeviceController(power_usage_monitorContext context)
        {
            _context = context;
        }

        // GET: Device
        public async Task<IActionResult> Index()
        {

            var power_usage_monitorContext = _context.Devices;
            return View(await power_usage_monitorContext.ToListAsync());

        }

        // GET: Device/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Devices == null)
            {
                return NotFound();
            }

            var device = await _context.Devices
                .FirstOrDefaultAsync(m => m.DeviceId == id);
            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }

        // GET: Device/Create
        public IActionResult Create()
        {
           /* List<SelectListItem> CategoryId = new()
            {
                new SelectListItem { Value = "1", Text = "1" },
                new SelectListItem { Value = "2", Text = "2" },
                new SelectListItem { Value = "3", Text = "3" },
                new SelectListItem { Value = "4", Text = "4" },
                new SelectListItem { Value = "5", Text = "5" },
                new SelectListItem { Value = "6", Text = "6" },
                new SelectListItem { Value = "7", Text = "7" },
                new SelectListItem { Value = "8", Text = "8" },
                new SelectListItem { Value = "9", Text = "9" },
                new SelectListItem { Value = "10", Text = "10" }
            };

            //assigning SelectListItem to view Bag
            ViewBag.CategoryId = CategoryId;
           
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId");*/
            return View();
        }

        // POST: Device/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DeviceId,DeviceName,StandbyTime,UseTime,Status,CategoryId")] Device device)
        {
            if (ModelState.IsValid)
            {
                _context.Add(device);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", device.CategoryId);
            return View(device);
        }

        // GET: Device/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Devices == null)
            {
                return NotFound();
            }

            var device = await _context.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", device.CategoryId);
            return View(device);
        }

        // POST: Device/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DeviceId,DeviceName,StandbyTime,UseTime,Status,CategoryId")] Device device)
        {
            if (id != device.DeviceId)
            {
                return NotFound();
            }
            ModelState.Clear();
            device.StandbyTime = await (from m in _context.Devices where m.DeviceId == device.DeviceId
                select m.StandbyTime).SingleOrDefaultAsync();
            //device.Category = await (from m in _context.Categories
            //                         where m.CategoryId == device.CategoryId
            //                         select m).SingleOrDefaultAsync();
            if (TryValidateModel(device))
            {
                try
                {
                    _context.Update(device);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeviceExists(device.DeviceId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", device.CategoryId);
            return View(device);
        }

        // GET: Device/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Devices == null)
            {
                return NotFound();
            }

            var device = await _context.Devices
                .FirstOrDefaultAsync(m => m.DeviceId == id);
            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }

        // POST: Device/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Devices == null)
            {
                return Problem("Entity set 'power_usage_monitorContext.Devices'  is null.");
            }
            var device = await _context.Devices.FindAsync(id);
            if (device != null)
            {
                _context.Devices.Remove(device);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeviceExists(int id)
        {
          return _context.Devices.Any(e => e.DeviceId == id);
        }
    }
}
