using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using System.Linq;
using CoffeeHouse.Infrastructure.Persistence;

using CoffeeHouse.Domain.Entities;

namespace CoffeeHouse.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/NhaCungCap")]
    public class NhaCungCapController : Controller
    {
        private readonly CoffeeHouseContext _context;

        public NhaCungCapController(CoffeeHouseContext context)
        {
            _context = context;
        }

        // Hi?n th? danh sách nhà cung c?p v?i phân trang
        [Route("")]
        [Route("Index")]
        [Authentication]
        public IActionResult Index(int? page)
        {
            int pageSize = 30;
            int pageNumber = (page == null || page < 1) ? 1 : page.Value;
            var listItem = _context.Suppliers.AsNoTracking()
                                 .OrderBy(x => x.SupplierName)
                                 .ToList();
            var pagedListItem = new PagedList<Supplier>(listItem, pageNumber, pageSize);
            return View(pagedListItem);
        }

        // Tìm ki?m nhà cung c?p theo tên
        [Route("Search")]
        [Authentication]
        [HttpGet]
        public IActionResult Search(int? page, string search)
        {
            int pageSize = 30;
            int pageNumber = (page == null || page < 1) ? 1 : page.Value;

            if (string.IsNullOrEmpty(search))
            {
                return RedirectToAction("Index");
            }
            search = search.ToLower();
            ViewBag.search = search;

            var listItem = _context.Suppliers.AsNoTracking()
                                .Where(x => x.SupplierName.ToLower().Contains(search))
                                .OrderBy(x => x.SupplierName)
                                .ToList();
            var pagedListItem = new PagedList<Supplier>(listItem, pageNumber, pageSize);

            return View(pagedListItem);
        }

        // GET: Admin/NhaCungCap/Create
        [Route("Create")]
        [Authentication]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/NhaCungCap/Create
        [Route("Create")]
        [Authentication]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                _context.Suppliers.Add(supplier);
                _context.SaveChanges();
                TempData["Message"] = "Thêm Nhà Cung C?p thành công.";
                return RedirectToAction("Index");
            }
            return View(supplier);
        }

        // GET: Admin/NhaCungCap/Edit
        [Route("Edit")]
        [Authentication]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var supplier = _context.Suppliers.Find(id);
            if (supplier == null)
            {
                TempData["Message"] = "Không tìm th?y Nhà Cung C?p c?n s?a.";
                return RedirectToAction("Index");
            }
            return View(supplier);
        }

        // POST: Admin/NhaCungCap/Edit
        [Route("Edit")]
        [Authentication]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(supplier).State = EntityState.Modified;
                _context.SaveChanges();
                TempData["Message"] = "S?a Nhà Cung C?p thành công.";
                return RedirectToAction("Index");
            }
            return View(supplier);
        }

        // GET: Admin/NhaCungCap/Details/{id}
        [Route("Details/{id}")]
        [Authentication]
        [HttpGet]
        public IActionResult Details(int id)
        {
            var supplier = _context.Suppliers.AsNoTracking()
                                .FirstOrDefault(x => x.SupplierId == id);
            if (supplier == null)
            {
                TempData["Message"] = "Không tìm th?y Nhà Cung C?p.";
                return RedirectToAction("Index");
            }
            return View(supplier);
        }

        // GET: Admin/NhaCungCap/Delete?id=...
        [Route("Delete")]
        [Authentication]
        [HttpGet]
        public IActionResult Delete(string id)
        {
            TempData["Message"] = "";

            if (!int.TryParse(id, out int supplierId))
            {
                TempData["Message"] = "Sai d?nh d?ng mã Nhà Cung C?p.";
                return RedirectToAction("Index");
            }

            var supplier = _context.Suppliers.Find(supplierId);
            if (supplier == null)
            {
                TempData["Message"] = "Không tìm th?y Nhà Cung C?p c?n xóa.";
                return RedirectToAction("Index");
            }
            return View(supplier);
        }

        // POST: Admin/NhaCungCap/Delete
        [Route("Delete")]
        [Authentication]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var supplier = _context.Suppliers.Find(id);
            if (supplier == null)
            {
                TempData["Message"] = "Không tìm th?y Nhà Cung C?p c?n xóa.";
                return RedirectToAction("Index");
            }

            // N?u Nhà Cung C?p có các d? li?u ph? thu?c (ví d?: Phi?u nh?p hàng) thì có th? c?n x? lý
            // ho?c d? Cascade Delete n?u c?u hình ràng bu?c trong DbContext/CSDL.

            _context.Suppliers.Remove(supplier);
            _context.SaveChanges();

            TempData["Message"] = "Xóa Nhà Cung C?p thành công.";
            return RedirectToAction("Index");
        }
    }
}


