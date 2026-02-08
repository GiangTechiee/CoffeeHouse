using CoffeeHouse.Infrastructure.Persistence;
using CoffeeHouse.Domain.Entities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace CoffeeHouse.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/News")]
    public class NewsManageController : Controller
    {
        private readonly CoffeeHouseContext _context;

        public NewsManageController(CoffeeHouseContext context)
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
            var listItem = _context.NewsArticles.AsNoTracking().OrderBy(x => x.ArticleId).ToList();
            PagedList<NewsArticle> pagedListItem = new PagedList<NewsArticle>(listItem, pageNumber, pageSize);

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

            var listItem = _context.NewsArticles.AsNoTracking().Where(x => x.Title.ToLower().Contains(search)).OrderBy(x => x.ArticleId).ToList();
            PagedList<NewsArticle> pagedListItem = new PagedList<NewsArticle>(listItem, pageNumber, pageSize);

            return View(pagedListItem);
        }

        [Route("Create")]
        [Authentication]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.NguoiDang = new SelectList(_context.Accounts.ToList(), "Username", "Username");

            return View();
        }

        [Route("Create")]
        [Authentication]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(NewsArticle tinTuc)
        {
            _context.NewsArticles.Add(tinTuc);
            _context.SaveChanges();

            TempData["Message"] = "Thêm thành công";

            return RedirectToAction("Index", "NewsManage");
        }

        [Route("Details")]
        [Authentication]
        [HttpGet]
        public IActionResult Details(int id, string name)
        {
            var tinTuc = _context.NewsArticles.SingleOrDefault(x => x.ArticleId == id);
            ViewBag.name = name;

            return View(tinTuc);
        }

        [Route("Edit")]
        [Authentication]
        [HttpGet]
        public IActionResult Edit(int id, string name)
        {
            var tinTuc = _context.NewsArticles.Find(id);

            ViewBag.NguoiDang = new SelectList(_context.Accounts.ToList(), "Username", "Username");
            ViewBag.name = name;

            return View(tinTuc);
        }

        [Route("Edit")]
        [Authentication]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(NewsArticle tinTuc)
        {
            _context.Entry(tinTuc).State = EntityState.Modified;
            _context.SaveChanges();

            TempData["Message"] = "S?a thành công";

            return RedirectToAction("Index", "NewsManage");
        }

        [Route("Delete")]
        [Authentication]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            TempData["Message"] = "";

            _context.Remove(_context.NewsArticles.Find(id));
            _context.SaveChanges();

            TempData["Message"] = "Xoá thành công";

            return RedirectToAction("Index", "NewsManage");
        }
    }
}



