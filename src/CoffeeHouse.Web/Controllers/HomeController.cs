using CoffeeHouse.Infrastructure.Persistence;
using CoffeeHouse.Domain.Entities;

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
            // L?y 8 s?n ph?m n?i b?t vï¿½ 3 tin t?c m?i nh?t
            var lstProducts = _context.Products.AsNoTracking().OrderBy(x => x.ProductId).Take(8).ToList();
            var lstNews = _context.NewsArticles.AsNoTracking().OrderByDescending(x => x.PublishedAt).Take(3).ToList();
            value.Products = lstProducts;
            value.NewsArticles = lstNews;

            // Ki?m tra mï¿½ khï¿½ch hï¿½ng trong Session vï¿½ load gi? hï¿½ng t? database
            string maKhachHangStr = HttpContext.Session.GetString("CustomerId");
            if (!string.IsNullOrEmpty(maKhachHangStr) && int.TryParse(maKhachHangStr, out int maKhachHang))
            {
                value.CartItems = _context.CartItems
                    .AsNoTracking()
                    .Include(g => g.Product)
                    .Where(g => g.CustomerId == maKhachHang)
                    .ToList();

                ViewData["cartCount"] = value.CartItems.Sum(item => item.Quantity);
                ViewData["total"] = value.CartItems.Sum(item => item.Quantity * item.Product.Price)
                                                .ToString("n0");
            }
            else
            {
                value.CartItems = new List<CartItem>();
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



