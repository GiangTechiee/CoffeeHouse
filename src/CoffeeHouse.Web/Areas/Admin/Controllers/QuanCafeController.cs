using CoffeeHouse.Infrastructure.Persistence;
using CoffeeHouse.Domain.Entities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace CoffeeHouse.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/QuanCafe")]
    [Authentication(Roles = "Admin,Employee")]
    public class QuanCafeController : Controller
    {
        private readonly CoffeeHouseContext _context;

        public QuanCafeController(CoffeeHouseContext context)
        {
            _context = context;
        }

        // GET: /Admin/QuanCafe or /Admin/QuanCafe/Index
        [Route("")]
        [Route("Index")]
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 30;
            int pageNumber = (page == null || page < 0) ? 1 : page.Value;
            var listItem = await _context.CafeStores
                                   .OrderBy(q => q.StoreName)
                                   .ToListAsync();

            // S? d?ng PagedList n?u c?n, n?u không ch? tr? v? listItem
            IPagedList<CafeStore> pagedList = listItem.ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }

        // GET: /Admin/QuanCafe/Details/{id}
        [Route("Details/{id}")]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var quanCafe = await _context.CafeStores
                   .Include(nv => nv.Employees)
                     .Include(pn => pn.PurchaseOrders)
                      .ThenInclude(hd => hd.PurchaseOrderItems)
                 .Include(q => q.SalesOrders) // N?p danh sách hóa don bán c?a quán
                     .ThenInclude(hd => hd.SalesOrderItems) // N?p chi ti?t c?a m?i hóa don
                         .ThenInclude(ct => ct.Product) // N?p thông tin s?n ph?m c?a chi ti?t hóa don
                 .FirstOrDefaultAsync(q => q.StoreId == id);

            if (quanCafe == null)
            {
                TempData["Message"] = "Không tìm th?y chi ti?t quán café.";
                return RedirectToAction("Index", "QuanCafe");
            }
            return View(quanCafe);
        }



        // GET: /Admin/QuanCafe/Create
        [Route("Create")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Admin/QuanCafe/Create
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CafeStore quanCafe)
        {
            if (ModelState.IsValid)
            {
                _context.Add(quanCafe);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Thêm quán cafe thành công";
                return RedirectToAction(nameof(Index));
            }
            return View(quanCafe);
        }

        // GET: /Admin/QuanCafe/Edit/{id}
        [Route("Edit/{id:int}")]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var quanCafe = await _context.CafeStores.FindAsync(id);
            if (quanCafe == null)
                return NotFound();

            return View(quanCafe);
        }

        // POST: /Admin/QuanCafe/Edit/{id}
        [Route("Edit/{id:int}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CafeStore quanCafe)
        {
            if (id != quanCafe.StoreId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(quanCafe);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "C?p nh?t thành công";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuanCafeExists(quanCafe.StoreId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(quanCafe);
        }

        // GET: /Admin/QuanCafe/Delete/{id}
        [Route("Delete/{id:int}")]
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var quanCafe = await _context.CafeStores
                .FirstOrDefaultAsync(q => q.StoreId == id);
            if (quanCafe == null)
                return NotFound();

            return View(quanCafe);
        }

        // POST: /Admin/QuanCafe/Delete/{id}
        [Route("Delete/{id:int}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var quanCafe = await _context.CafeStores.FindAsync(id);
            if (quanCafe != null)
            {
                _context.CafeStores.Remove(quanCafe);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Xoá thành công";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool QuanCafeExists(int id)
        {
            return _context.CafeStores.Any(e => e.StoreId == id);
        }
    }
}



