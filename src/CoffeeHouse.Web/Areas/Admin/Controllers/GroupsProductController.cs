using CoffeeHouse.Infrastructure.Persistence;
using CoffeeHouse.Domain.Entities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace CoffeeHouse.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/GroupsProduct")]
    public class GroupsProductController : Controller
    {
        private readonly CoffeeHouseContext _context;

        public GroupsProductController(CoffeeHouseContext context)
        {
            _context = context;
        }

        [Route("")]
        [Route("Index")]
        [Authentication]
        public IActionResult Index(int? page)
        {
            int pageSize = 30;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var listItem = _context.ProductCategories.AsNoTracking().OrderBy(x => x.CategoryId).ToList();
            PagedList<ProductCategory> pagedListItem = new PagedList<ProductCategory>(listItem, pageNumber, pageSize);

            return View(pagedListItem);
        }

        [Route("Search")]
        [Authentication]
        [HttpGet]
        public IActionResult Search(int? page, string search)
        {
            int pageSize = 30;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            search = search.ToLower();
            ViewBag.search = search;

            var listItem = _context.ProductCategories.AsNoTracking().Where(x => x.CategoryName.ToLower().Contains(search)).OrderBy(x => x.CategoryId).ToList();
            PagedList<ProductCategory> pagedListItem = new PagedList<ProductCategory>(listItem, pageNumber, pageSize);

            return View(pagedListItem);
        }

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
        public IActionResult Create(ProductCategory nhomSp)
        {
            _context.ProductCategories.Add(nhomSp);
            _context.SaveChanges();

            TempData["Message"] = "Thêm thành công";

            return RedirectToAction("Index", "GroupsProduct");
        }

        [Route("Edit")]
       // [Authentication]
        [HttpGet]
        public IActionResult Edit(int id, string name)
        {
            var nhomSp = _context.ProductCategories.Find(id);
            ViewBag.name = name;

            return View(nhomSp);
        }

        [Route("Edit")]
        [Authentication]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductCategory nhomSp)
        {
            _context.Entry(nhomSp).State = EntityState.Modified;
            _context.SaveChanges();

            TempData["Message"] = "S?a thành công";

            return RedirectToAction("Index", "GroupsProduct");
        }

        [Route("Delete")]
        [Authentication]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            TempData["Message"] = "";

            var sanPham = _context.Products.Where(x => x.CategoryId == id).ToList();

            if (sanPham.Count() > 0)
            {
                TempData["Message"] = "Xoá không thành công";
                return RedirectToAction("Index", "GroupsProduct");
            }

            _context.Remove(_context.ProductCategories.Find(id));
            _context.SaveChanges();

            TempData["Message"] = "Xoá thành công";

            return RedirectToAction("Index", "GroupsProduct");
        }
    }
}



