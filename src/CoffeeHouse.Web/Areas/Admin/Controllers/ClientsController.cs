using CoffeeHouse.Infrastructure.Persistence;
using CoffeeHouse.Domain.Entities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace CoffeeHouse.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Clients")]
    public class ClientsController : Controller
    {
        private readonly CoffeeHouseContext _context;

        public ClientsController(CoffeeHouseContext context)
        {
            _context = context;
        }

        // Hi?n th? danh sách khách hàng v?i phân trang
        [Route("")]
        [Route("Index")]
        [Authentication]
        public IActionResult Index(int? page)
        {
            int pageSize = 30;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var listItem = _context.Customers.AsNoTracking()
                                .OrderBy(x => x.PhoneNumber)
                                .ToList();
            var pagedListItem = new PagedList<Customer>(listItem, pageNumber, pageSize);

            return View(pagedListItem);
        }

        // Tìm ki?m khách hàng theo tên
        [Route("Search")]
        [Authentication]
        [HttpGet]
        public IActionResult Search(int? page, string search)
        {
            int pageSize = 30;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            if (string.IsNullOrEmpty(search))
            {
                return RedirectToAction("Index");
            }
            search = search.ToLower();
            ViewBag.search = search;

            var listItem = _context.Customers.AsNoTracking()
                                .Where(x => x.CustomerName.ToLower().Contains(search))
                                .OrderBy(x => x.PhoneNumber)
                                .ToList();
            var pagedListItem = new PagedList<Customer>(listItem, pageNumber, pageSize);

            return View(pagedListItem);
        }

        // Form t?o khách hàng m?i
        [Route("Create")]
        [Authentication]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Route("Create")]
        [Authentication]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Customer khachHang)
        {
            if (ModelState.IsValid)
            {
                _context.Customers.Add(khachHang);
                _context.SaveChanges();
                TempData["Message"] = "Thêm thành công";
                return RedirectToAction("Index", "Clients");
            }
            return View(khachHang);
        }

        // Form ch?nh s?a thông tin khách hàng
        // (N?u không c?n thi?t s? d?ng bi?n name thì có th? lo?i b? tham s? này)
        [Route("Edit")]
        [Authentication]
        [HttpGet]
        public IActionResult Edit(int id, string name)
        {
            var khachHang = _context.Customers.Find(id);
            if (khachHang == null)
            {
                TempData["Message"] = "Không tìm th?y khách hàng c?n s?a.";
                return RedirectToAction("Index", "Clients");
            }
            ViewBag.name = name;
            return View(khachHang);
        }

        [Route("Edit")]
        [Authentication]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Customer khachHang)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(khachHang).State = EntityState.Modified;
                _context.SaveChanges();
                TempData["Message"] = "S?a thành công";
                return RedirectToAction("Index", "Clients");
            }
            return View(khachHang);
        }

        // Hi?n th? chi ti?t c?a khách hàng, bao g?m:
        // 1. Thông tin khách hàng (tbKhachHang)
        // 2. Tài kho?n dang nh?p c?a khách hàng (tbTaiKhoanKH)
        // 3. L?ch s? giao d?ch và chi ti?t hóa don (tbHoaDonBan và tbChiTietHoaDonBan)
        // 4. Gi? hàng (CartItems) n?u c?n
        [Route("Details/{id}")]
        [Authentication]
        [HttpGet]
        public IActionResult Details(int id)
        {
            var khachHang = _context.Customers
                .Include(x => x.Accounts)
                  .Include(x => x.CartItems)
                .Include(x => x.SalesOrders)
               
                    .ThenInclude(hd => hd.SalesOrderItems)
           .ThenInclude(ct => ct.Product) // include thông tin s?n ph?m
                .FirstOrDefault(x => x.CustomerId == id);

            if (khachHang == null)
            {
                TempData["Message"] = "Không tìm th?y chi ti?t khách hàng.";
                return RedirectToAction("Index", "Clients");
            }
            return View(khachHang);
        }

        // GET: Admin/Clients/Delete?id=10
        [Route("Delete")]
        [Authentication]
        [HttpGet]
        public IActionResult Delete(string id)
        {
            TempData["Message"] = "";

            if (!int.TryParse(id, out int customerId))
            {
                TempData["Message"] = "Sai d?nh d?ng mã khách hàng.";
                return RedirectToAction("Index", "Clients");
            }

            // Load khách hàng kèm thông tin liên quan c?n thi?t
            var khachHang = _context.Customers
                .Include(x => x.Accounts)
                .Include(x => x.SalesOrders)
                    .ThenInclude(hd => hd.SalesOrderItems)
                .FirstOrDefault(x => x.CustomerId == customerId);

            if (khachHang == null)
            {
                TempData["Message"] = "Không tìm th?y khách hàng c?n xóa.";
                return RedirectToAction("Index", "Clients");
            }

            // Hi?n th? view xác nh?n xoá cho khách hàng này
            return View(khachHang);
        }

        // POST: Admin/Clients/Delete
        [Route("Delete")]
        [Authentication]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            // Load khách hàng kèm các collection liên quan, bao g?m tài kho?n, hóa don (và chi ti?t hóa don) và gi? hàng
            var khachHang = _context.Customers
                .Include(x => x.Accounts)
                .Include(x => x.SalesOrders)
                    .ThenInclude(hd => hd.SalesOrderItems)
                .Include(x => x.CartItems)
                .FirstOrDefault(x => x.CustomerId == id);

            if (khachHang == null)
            {
                TempData["Message"] = "Không tìm th?y khách hàng c?n xóa.";
                return RedirectToAction("Index", "Clients");
            }

            // Xoá các tài kho?n truy c?p c?a khách hàng
            foreach (var account in khachHang.Accounts.ToList())
            {
                _context.Accounts.Remove(account);
            }

            // Xoá các hóa don bán và chi ti?t hóa don liên quan
            foreach (var invoice in khachHang.SalesOrders.ToList())
            {
                foreach (var detail in invoice.SalesOrderItems.ToList())
                {
                    _context.SalesOrderItems.Remove(detail);
                }
                _context.SalesOrders.Remove(invoice);
            }

            // Xoá gi? hàng c?a khách hàng
            foreach (var gioHang in khachHang.CartItems.ToList())
            {
                _context.CartItems.Remove(gioHang);
            }

            // Cu?i cùng xoá khách hàng
            _context.Customers.Remove(khachHang);
            _context.SaveChanges();

            TempData["Message"] = "Xoá khách hàng thành công.";
            return RedirectToAction("Index", "Clients");
        }


    }
}




