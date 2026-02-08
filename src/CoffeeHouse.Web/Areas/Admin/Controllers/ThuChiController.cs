
using CoffeeHouse.Infrastructure.Persistence;
using CoffeeHouse.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeHouse.Controllers
{
    [Area("Admin")]
    [Route("Admin/ThuChi")]
    public class ThuChiController : Controller
    {
        private readonly CoffeeHouseContext _context;
        public ThuChiController(CoffeeHouseContext context)
        {
            _context = context;
        }

        // GET: Admin/ThuChi/All
        [HttpGet("All")]
        public IActionResult All()
        {
            // Giá tr? m?c d?nh: t? ngày hôm qua d?n hôm nay
            DateTime start = DateTime.Today.AddDays(-30);
            DateTime end = DateTime.Today; // Vì OrderDate dã là ki?u Date (ho?c có th?i gian 00:00:00)
            var model = new AllThuChiViewModel
            {
                StartDate = start,
                EndDate = end,
                Items = GetAllThuChi(start, end)
            };

            return View(model);
        }

        // POST: Admin/ThuChi/All
        [HttpPost("All")]
        public IActionResult All(AllThuChiViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Dùng .Date d? d?m b?o ch? so sánh ph?n Date (không có gi?)
                DateTime dateStart = model.StartDate.Date;
                DateTime dateEnd = model.EndDate.Date;

                // Tính toán thu – chi theo kho?ng th?i gian l?c
                model.Items = GetAllThuChi(dateStart, dateEnd);
            }
            return View(model);
        }

        // Phuong th?c n?i b? d? tính toán thu – chi c?a t?t c? các quán
        private System.Collections.Generic.List<ThuChiItemViewModel> GetAllThuChi(DateTime startDate, DateTime endDate)
        {
            // Chuy?n d?i các giá tr? nh?n vào sang .Date (lo?i ph?n gi?)
            DateTime dateStart = startDate.Date;
            DateTime dateEnd = endDate.Date;

            var quanCafes = _context.CafeStores.ToList();
            var result = new System.Collections.Generic.List<ThuChiItemViewModel>();

            foreach (var quan in quanCafes)
            {
                var hoadon = _context.SalesOrders
                    .Where(hd => hd.StoreId == quan.StoreId &&
                                 hd.OrderDate.Date >= dateStart &&
                                 hd.OrderDate.Date <= dateEnd)
                    .Sum(hd => (decimal?)hd.TotalAmount) ?? 0;

                var luong = _context.Employees
                    .Where(nv => nv.StoreId == quan.StoreId)
                    .Sum(nv => (decimal?)(nv.BaseSalary * nv.SalaryCoefficient)) ?? 0;

                var nhap = (from ph in _context.PurchaseOrders
                            join ct in _context.PurchaseOrderItems on ph.PurchaseOrderId equals ct.PurchaseOrderId
                            where ph.StoreId == quan.StoreId &&
                                  ph.OrderDate.Date >= dateStart &&
                                  ph.OrderDate.Date <= dateEnd
                            select (decimal?)ct.LineTotal).Sum() ?? 0;

                var thu = hoadon;
                var chi = nhap + luong;
                var profit = thu - chi;

                result.Add(new ThuChiItemViewModel
                {
                    StoreId = quan.StoreId,
                    StoreName = quan.StoreName,
                    TotalInvoice = hoadon,
                    TotalSalary = luong,
                    TotalRevenue = thu,
                    TotalImport = chi,
                    Profit = profit
                });
            }

            return result;
        }

        // Action chuy?n sang trang Chi ti?t Thu – Chi c?a m?t quán
        [HttpGet("All/Detail")]
        public IActionResult AllDetail(int maQuan, DateTime startDate, DateTime endDate)
        {
            return RedirectToAction("ThuChiChiTiet", new { cafeId = maQuan, startDate, endDate });
        }

        // Action Chi ti?t Thu – Chi c?a m?t quán
        [HttpGet("ThuChiChiTiet")]
        public IActionResult ThuChiChiTiet(int cafeId, DateTime startDate, DateTime endDate)
        {
            DateTime dateStart = startDate.Date;
            DateTime dateEnd = endDate.Date;

            var detailModel = new ThuChiDetailViewModel
            {
                CafeId = cafeId,
                StartDate = startDate,
                EndDate = endDate,
                Employees = _context.Employees.Where(nv => nv.StoreId == cafeId).ToList(),
                Invoices = _context.SalesOrders
                    .Where(hd => hd.StoreId == cafeId &&
                                 hd.OrderDate.Date >= dateStart &&
                                 hd.OrderDate.Date <= dateEnd)
                    .Include(hd => hd.SalesOrderItems)
                    .ToList(),
                PhieuNhapHangs = _context.PurchaseOrders
                    .Where(pn => pn.StoreId == cafeId &&
                                 pn.OrderDate.Date >= dateStart &&
                                 pn.OrderDate.Date <= dateEnd)
                    .Include(pn => pn.PurchaseOrderItems)
                    .Include(pn => pn.Supplier)
                    .ToList()
            };

            return View(detailModel);
        }
    }
}



