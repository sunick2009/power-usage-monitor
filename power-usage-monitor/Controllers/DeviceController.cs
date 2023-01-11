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
        public async Task<IActionResult> Index(string? message)
        {
            if (!(message is null))
            {
                TempData["message"] = message;
            }
            var power_usage_monitorContext = _context.Devices.AsNoTracking().ToListAsync();
            foreach (var item in await power_usage_monitorContext)
            {
                item.Category = (from m in _context.Categories
                                         where m.CategoryId == item.CategoryId
                                         select m).SingleOrDefault();
            }
            //插座清單
            List<SelectListItem> deviceList = new List<SelectListItem>();
            for (int i = 1; i <= 6; i++)
            {
                var device = (from m in _context.Devices
                              where m.DeviceId == i
                              select m).FirstOrDefault();
                if (device == null)
                {
                    deviceList.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }
            ViewBag.deviceList = deviceList;
            ViewBag.deviceTotal = 6 - deviceList.Count;
            return View(await power_usage_monitorContext);
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

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory([Bind("CategoryId,EngneryName,DeviceCategoryName,CategoryAvgPower")] Category category)
        {
            ModelState.Clear();
            category.EngneryName = (_context.Categories.Count() + 1).ToString();
            if (TryValidateModel(category))
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction("Create", "Device", new { message = "已建立分類" });
            }
            return RedirectToAction("Create", "Device", new { message = "建立失敗" }); 
        }
        // GET: Device/Create
        public IActionResult Create(string? message)
        {
            if (!(message is null))
            {
                TempData["message"] = message;
            }
            //插座清單
            List<SelectListItem> deviceList = new List<SelectListItem>();
            for (int i = 1; i <= 6; i++)
            {
                var device = (from m in _context.Devices
                              where m.DeviceId == i
                              select m).FirstOrDefault();
                if (device == null)
                {
                    deviceList.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }
            ViewBag.deviceList = deviceList;
            //分類清單
            var categories = _context.Categories.ToList();
            List<SelectListItem> categoryList = new List<SelectListItem>();
            foreach (var item in categories)
            {
                categoryList.Add(new SelectListItem { Text = item.DeviceCategoryName, Value = item.CategoryId.ToString() });
            }
            ViewBag.categoryList = categoryList;
            //用戶清單
            var users = _context.Users.ToList();
            List<SelectListItem> userList = new List<SelectListItem>();
            foreach (var item in users)
            {
                userList.Add(new SelectListItem { Text = item.UserName.ToString(), Value = item.UserName.ToString() });
            }
            ViewBag.userList = userList;
            return View();
        }

        // POST: Device/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("DeviceId,DeviceName,StandbyTime,UseTime,Status,CategoryId,UserName")] Device device)
        {
            ModelState.Clear();
            device.StandbyTime = "00:00-23:59";
            device.Category = await (from m in _context.Categories
                                     where m.CategoryId == device.CategoryId
                                     select m).SingleOrDefaultAsync();
            if (TryValidateModel(device))
            {
                _context.Add(device);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Device", new { message = "新增成功" });
            }
            TempData["message"] = "新增失敗";
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
            //分類清單
            var categories = _context.Categories.ToList();
            List<SelectListItem> categoryList = new List<SelectListItem>();
            foreach (var item in categories)
            {
                categoryList.Add(new SelectListItem { Text = item.DeviceCategoryName, Value = item.CategoryId.ToString() });
            }
            ViewBag.categoryList = categoryList;
            //用戶清單
            var users = _context.Users.ToList();
            List<SelectListItem> userList = new List<SelectListItem>();
            foreach (var item in users)
            {
                userList.Add(new SelectListItem { Text = item.UserName.ToString(), Value = item.UserName.ToString() });
            }
            ViewBag.userList = userList;
            return View(device);
        }

        // POST: Device/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DeviceId,DeviceName,StandbyTime,UseTime,Status,CategoryId,UserName")] Device device)
        {
            if (id != device.DeviceId)
            {
                return NotFound();
            }
            ModelState.Clear();
            device.StandbyTime = await (from m in _context.Devices where m.DeviceId == device.DeviceId
                select m.StandbyTime).SingleOrDefaultAsync();
            device.Category = await (from m in _context.Categories
                                     where m.CategoryId == device.CategoryId
                                     select m).SingleOrDefaultAsync();
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
            //回復刪除設備
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { message = "已刪除該設備" });
        }

        private bool DeviceExists(int id)
        {
            return _context.Devices.Any(e => e.DeviceId == id);
        }
    }
}
