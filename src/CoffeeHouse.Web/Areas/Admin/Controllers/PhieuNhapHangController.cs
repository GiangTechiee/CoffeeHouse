
using CoffeeHouse.Infrastructure.Persistence;
using CoffeeHouse.Helpers;
using CoffeeHouse.Domain.Entities;

using CoffeeHouse.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace CoffeeHouse.Areas.Admin.Controllers
{
    [Area(Constants.Areas.Admin)]
    [Route("Admin/PhieuNhapHang")]
    public class PhieuNhapHangController : Controller
    {
        private readonly CoffeeHouseContext _context;

        public PhieuNhapHangController(CoffeeHouseContext context)
        {
            _context = context;
        }


        // GET: Admin/PhieuNhapHang
        [Route("")]
        [Route("Index")]
        [Authentication]
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 30;
            int pageNumber = (page == null || page < 0) ? 1 : page.Value;

            var listItem = await _context.PurchaseOrders
                 .Include(p => p.Store)
                 .Include(p => p.Employee)
                 .Include(p => p.Supplier)
                 .Include(p => p.PurchaseOrderItems) // Bao g?m chi ti?t phi?u nh?p
                 .AsNoTracking()
                 .OrderByDescending(p => p.OrderDate)
                 .ToListAsync();

        

      

            var pagedList = new PagedList<PurchaseOrder>(listItem, pageNumber, pageSize);

            return View(pagedList);
        }


        // GET: Admin/PhieuNhapHang/Search?search=...
        [Route("Search")]
        [Authentication]
        [HttpGet]
        public IActionResult Search(int? page, string search)
        {
            int pageSize = 30;
            int pageNumber = (page == null || page < 0) ? 1 : page.Value;

            // N?u search có th? chuy?n thành ngày, tìm theo ngày l?p
            List<PurchaseOrder> listItem;
            if (!string.IsNullOrEmpty(search) && DateTime.TryParse(search, out DateTime searchDate))
            {
                listItem = _context.PurchaseOrders
                    .AsNoTracking()
                    .Where(p => p.OrderDate.Date == searchDate.Date)
                    .OrderByDescending(p => p.OrderDate)
                    .ToList();
            }
            else
            {
                // N?u không có giá tr? tìm ki?m h?p l?, tr? v? t?t c?
                listItem = _context.PurchaseOrders
                    .AsNoTracking()
                    .OrderByDescending(p => p.OrderDate)
                    .ToList();
            }

            var pagedList = new PagedList<PurchaseOrder>(listItem, pageNumber, pageSize);
            ViewBag.search = search;
            return View(pagedList);
        }

        // GET: Admin/PhieuNhapHang/Details/{id}
        [Route("Details/{id}")]
        [Authentication]
        [HttpGet]
        public async Task<IActionResult> Details(Guid? id, int? page)
        {
            if (id == null)
            {
                TempData["Message"] = "Mã phi?u nh?p không h?p l?.";
                return RedirectToAction("Index");
            }
            int pageSize = 30;
            int pageNumber = (page == null || page < 0) ? 1 : page.Value;

            // Include các Chi ti?t và thông tin navigation (Nguyên li?u,…)
            var phieuNhap = await _context.PurchaseOrders
                .Include(p => p.Store)
                .Include(p => p.Employee)
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseOrderItems)
                    .ThenInclude(ct => ct.Ingredient)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PurchaseOrderId == id);

            if (phieuNhap == null)
            {
                TempData["Message"] = "Không tìm th?y phi?u nh?p.";
                return RedirectToAction("Index");
            }
            return View(phieuNhap);
        }


        [Route("Nhap")]
        [HttpGet]
        public IActionResult Nhap()
        {
            // L?y mã nhân viên t? session
            var maNhanVienStr = HttpContext.Session.GetString(Constants.SessionKeys.EmployeeId);
            int maNhanVien = 0;
            string tenNhanVien = string.Empty;
            if (!string.IsNullOrEmpty(maNhanVienStr))
            {
                maNhanVien = int.Parse(maNhanVienStr);
                // Truy v?n d? l?y tên nhân viên
                var nhanVien = _context.Employees.FirstOrDefault(x => x.EmployeeId == maNhanVien);
                if (nhanVien != null)
                {
                    tenNhanVien = nhanVien.FullName;
                }
            }

            // Kh?i t?o view model cho phi?u nh?p
            var vm = new PhieuNhapViewModel
            {
                OrderDate = DateTime.Now,
                Description = string.Empty,
                ChiTietNhap = _context.Ingredients
                                       .AsNoTracking()
                                       .OrderBy(nl => nl.IngredientName)
                                       .Select(nl => new PhieuNhapChiTietViewModel
                                       {
                                           IngredientId = nl.IngredientId,
                                           IngredientName = nl.IngredientName,
                                           QuantityNhap = 0,
                                           UnitPriceNhap = nl.UnitPrice
                                       }).ToList()
            };

            // L?y các SelectList cho dropdown ch? d?i v?i Quán và Nhà cung c?p
            ViewBag.StoreId = new SelectList(_context.CafeStores, "StoreId", "StoreName");
            ViewBag.SupplierId = new SelectList(_context.Suppliers, "SupplierId", "SupplierName");

            // Luu thông tin nhân viên vào ViewBag
            ViewBag.EmployeeId = maNhanVien;
            ViewBag.FullName = tenNhanVien;

            return View(vm);
        }


        [Route("Nhap")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Nhap(PhieuNhapViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.StoreId = new SelectList(_context.CafeStores, "StoreId", "StoreName");
                ViewBag.SupplierId = new SelectList(_context.Suppliers, "SupplierId", "SupplierName");
                return View(model);
            }

            // L?y mã nhân viên t? session
            var maNhanVienStr = HttpContext.Session.GetString(Constants.SessionKeys.EmployeeId);
            if (string.IsNullOrEmpty(maNhanVienStr))
            {
                // N?u không tìm th?y, báo l?i phù h?p.
                ModelState.AddModelError("", "Không th? xác d?nh du?c nhân viên dang nh?p.");
                ViewBag.StoreId = new SelectList(_context.CafeStores, "StoreId", "StoreName");
                ViewBag.SupplierId = new SelectList(_context.Suppliers, "SupplierId", "SupplierName");
                return View(model);
            }
            int maNhanVien = int.Parse(maNhanVienStr);

            // T?o header phi?u nh?p s? d?ng thông tin t? view model và mã nhân viên t? session
            var phieuNhap = new PurchaseOrder
            {

                PurchaseOrderId = Guid.NewGuid(),
                OrderDate = model.OrderDate,
                Description = model.Description,
                EmployeeId = maNhanVien,
                StoreId = model.StoreId,
                SupplierId = model.SupplierId

            };

            _context.PurchaseOrders.Add(phieuNhap);
            await _context.SaveChangesAsync();

            // X? lý danh sách chi ti?t phi?u nh?p
            foreach (var ct in model.ChiTietNhap)
            {
                if (ct.QuantityNhap > 0)
                {
                    var detail = new PurchaseOrderItem
                    {
                        PurchaseOrderId = phieuNhap.PurchaseOrderId,
                        IngredientId = ct.IngredientId,
                        Quantity = ct.QuantityNhap,
                        UnitPrice = ct.UnitPriceNhap
                    };
                    _context.PurchaseOrderItems.Add(detail);

                    // C?p nh?t s? lu?ng t?n kho c?a nguyên li?u
                    var nguyenLieu = await _context.Ingredients.FindAsync(ct.IngredientId);
                    if (nguyenLieu != null)
                    {
                        nguyenLieu.Quantity += ct.QuantityNhap;
                        _context.Ingredients.Update(nguyenLieu);
                    }
                }
            }

            await _context.SaveChangesAsync();

            TempData["Message"] = "Phi?u nh?p dã du?c luu thành công.";
            return RedirectToAction("Index", "PhieuNhapHang");
        }

        // GET: Admin/PhieuNhapHang/Edit/{id}
        [Route("Edit/{id}")]
        [Authentication]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return NotFound();
            var phieuNhap = await _context.PurchaseOrders.FindAsync(id);
            if (phieuNhap == null)
                return NotFound();
            ViewData["StoreId"] = new SelectList(_context.CafeStores, "StoreId", "StoreName", phieuNhap.StoreId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FullName", phieuNhap.EmployeeId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "SupplierName", phieuNhap.SupplierId);
            return View(phieuNhap);
        }

        // POST: Admin/PhieuNhapHang/Edit/{id}
        [Route("Edit/{id}")]
        [Authentication]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("PurchaseOrderId,StoreId,OrderDate,EmployeeId,SupplierId,Description")] PurchaseOrder phieuNhap)
        {
            if (id != phieuNhap.PurchaseOrderId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phieuNhap);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhieuNhapExists(phieuNhap.PurchaseOrderId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["StoreId"] = new SelectList(_context.CafeStores, "StoreId", "StoreName", phieuNhap.StoreId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FullName", phieuNhap.EmployeeId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "SupplierName", phieuNhap.SupplierId);
            return View(phieuNhap);
        }

        // GET: Admin/PhieuNhapHang/Delete/{id}
        [Route("Delete/{id}")]
        [Authentication]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
                return NotFound();

            var phieuNhap = await _context.PurchaseOrders
                .Include(p => p.Store)
                .Include(p => p.Employee)
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.PurchaseOrderId == id);
            if (phieuNhap == null)
                return NotFound();
            return View(phieuNhap);
        }

        // POST: Admin/PhieuNhapHang/Delete/{id}
        [Route("Delete/{id}")]
        [Authentication]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var phieuNhap = await _context.PurchaseOrders.FindAsync(id);
            if (phieuNhap != null)
            {
                // Xoá các chi ti?t liên quan tru?c
                var details = _context.PurchaseOrderItems.Where(ct => ct.PurchaseOrderId == id);
                _context.PurchaseOrderItems.RemoveRange(details);
                _context.PurchaseOrders.Remove(phieuNhap);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PhieuNhapExists(Guid id)
        {
            return _context.PurchaseOrders.Any(e => e.PurchaseOrderId == id);
        }
    }
}




