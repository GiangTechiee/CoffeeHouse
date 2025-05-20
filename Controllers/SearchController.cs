using CoffeeHouse;
using CoffeeHouse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace CoffeeHouse.Controllers
{
    public class SearchController : Controller
    {
        private readonly CoffeeHouseContext _context;

        public SearchController(CoffeeHouseContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? page, string search)
        {
            int pageSize = 9;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            search = search.ToLower();
            ViewBag.search = search;

            var listItem = _context.TbSanPhams.AsNoTracking().Where(x => x.TenSanPham.ToLower().Contains(search)).OrderBy(x => x.TenSanPham).ToList();
            PagedList<TbSanPham> pagedListItem = new PagedList<TbSanPham>(listItem, pageNumber, pageSize);

            return View(pagedListItem);
        }
    }
}
