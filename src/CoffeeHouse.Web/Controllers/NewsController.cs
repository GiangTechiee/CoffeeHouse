using CoffeeHouse.Infrastructure.Persistence;
using CoffeeHouse.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace CoffeeHouse.Controllers
{
    public class NewsController : Controller
    {
        private readonly CoffeeHouseContext _context;

        public NewsController(CoffeeHouseContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? page)
        {
            int pageSize = 9;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var listItem = _context.NewsArticles.AsNoTracking().OrderByDescending(x => x.PublishedAt).ToList();
            PagedList<NewsArticle> pagedListItem = new PagedList<NewsArticle>(listItem, pageNumber, pageSize);

            return View(pagedListItem);
        }

        public IActionResult Details(int? maTinTuc)
        {
            var tinTuc = _context.NewsArticles.SingleOrDefault(x => x.ArticleId == maTinTuc);

            return View(tinTuc);
        }
    }
}
