using CoffeeHouse.Infrastructure.Persistence;
using CoffeeHouse.Domain.Entities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace CoffeeHouse.Areas.Controllers
{
    [Area("Admin")]
    [Route("Admin/Bill")]
    public class BillController : Controller
    {
        private readonly CoffeeHouseContext _context;

        public BillController(CoffeeHouseContext context)
        {
            _context = context;
        }

        // Action Confirm d? c?p nh?t tr?ng thái hóa don
        [HttpGet]
        [Route("Confirm/{id}")]
        [Authentication]
        public async Task<IActionResult> Confirm(Guid id)
        {
            // L?y mã nhân viên dang dang nh?p t? Session ho?c t? User Claims
            string maNhanVienStr = HttpContext.Session.GetString("EmployeeId");
            if (string.IsNullOrEmpty(maNhanVienStr))
            {
                TempData["Message"] = "B?n c?n dang nh?p d? xác nh?n hóa don.";
                return RedirectToAction("Login", "Auth");
            }
            int maNhanVien = int.Parse(maNhanVienStr);

            // Tìm hóa don theo mã hóa don id
            var order = await _context.SalesOrders.FindAsync(id);
            if (order == null)
            {
                TempData["Message"] = "Không tìm th?y hóa don.";
                return RedirectToAction("Index");
            }

            // N?u hóa don dã du?c xác nh?n, không cho xác nh?n l?i
            if (order.Status != "Chua hoàn thành")
            {
                TempData["Message"] = "Hóa don dã du?c xác nh?n.";
                return RedirectToAction("Index");
            }

            // C?p nh?t hóa don: gán EmployeeId và chuy?n tr?ng thái
            order.EmployeeId = maNhanVien;
            order.Status = "Hoàn thành";

            _context.SalesOrders.Update(order);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Xác nh?n hóa don thành công.";
            return RedirectToAction("Index");
        }

        [Route("")]
        [Route("Index")]
        [Authentication]
        public IActionResult Index(int? page)
        {
            int pageSize = 30;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var listItem = _context.SalesOrders
                                   .Include(x => x.Customer)
                                        .Include(x => x.Employee)
                                        .Include(x => x.Store)// Load thông tin khách hàng
                                   .AsNoTracking()
                                   .OrderByDescending(x => x.OrderDate)
                                   .ToList();
            PagedList<SalesOrder> pagedListItem = new PagedList<SalesOrder>(listItem, pageNumber, pageSize);

            return View(pagedListItem);
        }


        [Route("Search")]
        [Authentication]
        [HttpGet]
        public IActionResult Search(int? page, string search)
        {
            int pageSize = 30;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            ViewBag.search = search;

            // N?u giá tr? search không r?ng và có th? chuy?n sang DateTime
            List<SalesOrder> listItem;
            if (!string.IsNullOrEmpty(search) && DateTime.TryParse(search, out DateTime searchDate))
            {
                // So sánh ngày bán (ch? l?y ph?n Date) v?i ngày tìm ki?m
                listItem = _context.SalesOrders
                    .AsNoTracking()
                    .Where(x => x.OrderDate.Date == searchDate.Date)
                    .OrderBy(x => x.OrderId)
                    .ToList();
            }
            else
            {
                // N?u không có giá tr? tìm ki?m ho?c search không h?p l?, tr? v? danh sách t?t c?
                listItem = _context.SalesOrders
                    .AsNoTracking()
                    .OrderBy(x => x.OrderId)
                    .ToList();
            }

            PagedList<SalesOrder> pagedListItem = new PagedList<SalesOrder>(listItem, pageNumber, pageSize);
            return View(pagedListItem);
        }

        [Route("Details")]
        [Authentication]
        [HttpGet]
        public IActionResult Details(int? page, string id, string name)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Message"] = "Mã hóa don không h?p l?.";
                return RedirectToAction("Index");
            }
            if (!Guid.TryParse(id, out Guid billGuid))
            {
                TempData["Message"] = "Sai d?nh d?ng mã hóa don.";
                return RedirectToAction("Index");
            }

            int pageSize = 30;
            int pageNumber = (page == null || page < 0) ? 1 : page.Value;

            // Ð?m b?o Include navigation property c?a hóa don và s?n ph?m
            var listItem = _context.SalesOrderItems
                .Include(ct => ct.Order)
                .Include(ct => ct.Product)
                .AsNoTracking()
                .Where(x => x.OrderId == billGuid)
                .OrderBy(x => x.OrderId)
                .ToList();

            if (!listItem.Any())
            {
                TempData["Message"] = "Không tìm th?y chi ti?t c?a hóa don.";
                return RedirectToAction("Index");
            }

            var pagedListItem = new X.PagedList.PagedList<SalesOrderItem>(listItem, pageNumber, pageSize);

            ViewBag.Name = name;

            return View(pagedListItem);
        }
    }
}


