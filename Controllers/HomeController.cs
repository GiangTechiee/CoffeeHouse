using CoffeeHouse;
using CoffeeHouse.Models;
using CoffeeHouse.Models.ViewModels;
using CoffeeHouse.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CoffeeHouse.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CoffeeHouseContext _context;

        public HomeController(ILogger<HomeController> logger, CoffeeHouseContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            HomeViewModel value = new HomeViewModel();
            // Lấy 8 sản phẩm nổi bật và 3 tin tức mới nhất
            var lstProducts = _context.TbSanPhams.AsNoTracking().OrderBy(x => x.MaSanPham).Take(8).ToList();
            var lstNews = _context.TbTinTucs.AsNoTracking().OrderByDescending(x => x.NgayDang).Take(3).ToList();
            value.lstSanPham = lstProducts;
            value.lstTinTuc = lstNews;

            // Kiểm tra mã khách hàng trong Session và load giỏ hàng từ database
            string maKhachHangStr = HttpContext.Session.GetString("MaKhachHang");
            if (!string.IsNullOrEmpty(maKhachHangStr) && int.TryParse(maKhachHangStr, out int maKhachHang))
            {
                value.CartItems = _context.TbGioHangs
                    .AsNoTracking()
                    .Include(g => g.MaSanPhamNavigation)
                    .Where(g => g.MaKhachHang == maKhachHang)
                    .ToList();

                ViewData["cartCount"] = value.CartItems.Sum(item => item.SoLuong);
                ViewData["total"] = value.CartItems.Sum(item => item.SoLuong * item.MaSanPhamNavigation.GiaBan)
                                                .ToString("n0");
            }
            else
            {
                value.CartItems = new List<TbGioHang>();
                ViewData["cartCount"] = 0;
                ViewData["total"] = "0";
            }

            return View(value);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
