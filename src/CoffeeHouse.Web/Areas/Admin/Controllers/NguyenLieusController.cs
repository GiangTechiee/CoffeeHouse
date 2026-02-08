using CoffeeHouse.Infrastructure.Persistence;
using CoffeeHouse.Domain.Entities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace CoffeeHouse.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/NguyenLieus")]
    public class NguyenLieusController : Controller
    {
        private readonly CoffeeHouseContext _context;

        public NguyenLieusController(CoffeeHouseContext context)
        {
            _context = context;
        }

        // Hi?n th? danh sách khách hàng v?i phân trang
        [Route("")]
        [Route("Index")]
        [Authentication]
        public IActionResult Index(int? page)
        {
            int pageSize = 5;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var listItem = _context.Ingredients.AsNoTracking()
                                .OrderBy(x => x.IngredientId)
                                .ToList();
            var pagedListItem = new PagedList<Ingredient>(listItem, pageNumber, pageSize);

            return View(pagedListItem);
        }

        // Tìm ki?m khách hàng theo tên
        [Route("Search")]
        [Authentication]
        [HttpGet]
        public IActionResult Search(int? page, string search)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            if (string.IsNullOrEmpty(search))
            {
                return RedirectToAction("Index");
            }
            search = search.ToLower();
            ViewBag.search = search;

            var listItem = _context.Ingredients.AsNoTracking()
                                .Where(x => x.IngredientName.ToLower().Contains(search))
                                .OrderBy(x => x.IngredientId)
                                .ToList();
            var pagedListItem = new PagedList<Ingredient>(listItem, pageNumber, pageSize);

            return View(pagedListItem);
        }
        // GET: Admin/NguyenLieus/Details/5
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var nguyenLieu = await _context.Ingredients
                .FirstOrDefaultAsync(nl => nl.IngredientId == id);
            if (nguyenLieu == null)
                return NotFound();

            return View(nguyenLieu);
        }

        // GET: Admin/NguyenLieus/Create
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/NguyenLieus/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create([Bind("IngredientName,Quantity,Unit,ExpirationDate,UnitPrice,MinimumQuantity")] Ingredient nguyenLieu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nguyenLieu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nguyenLieu);
        }

        // GET: Admin/NguyenLieus/Edit/5
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var nguyenLieu = await _context.Ingredients.FindAsync(id);
            if (nguyenLieu == null)
                return NotFound();

            return View(nguyenLieu);
        }

        // POST: Admin/NguyenLieus/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, [Bind("IngredientId,IngredientName,Quantity,Unit,ExpirationDate,UnitPrice,MinimumQuantity")] Ingredient nguyenLieu)
        {
            if (id != nguyenLieu.IngredientId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nguyenLieu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NguyenLieuExists(nguyenLieu.IngredientId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(nguyenLieu);
        }

        // GET: Admin/NguyenLieus/Delete/5
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var nguyenLieu = await _context.Ingredients
                .FirstOrDefaultAsync(nl => nl.IngredientId == id);
            if (nguyenLieu == null)
                return NotFound();

            return View(nguyenLieu);
        }

        // POST: Admin/NguyenLieus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nguyenLieu = await _context.Ingredients.FindAsync(id);
            if (nguyenLieu != null)
            {
                _context.Ingredients.Remove(nguyenLieu);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool NguyenLieuExists(int id)
        {
            return _context.Ingredients.Any(nl => nl.IngredientId == id);
        }
    }
}



