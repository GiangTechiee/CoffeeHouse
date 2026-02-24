using CoffeeHouse.Application.Interfaces;
using CoffeeHouse.Infrastructure.Persistence;
using CoffeeHouse.Domain.Entities;

using CoffeeHouse.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace CoffeeHouse.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("HomeAdmin")]
    [Authentication(Roles = "Admin,Employee")]
    public class HomeAdminController : Controller
    {
        private readonly CoffeeHouseContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IImageService _imageService;

        public HomeAdminController(CoffeeHouseContext context, IWebHostEnvironment hc, IImageService imageService)
        {
            _context = context;
            _hostEnvironment = hc;
            _imageService = imageService;
        }

        [Route("")]
        [Route("Index")]
        public IActionResult Index(int? page)
        {
            int pageSize = 30;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var listItem = (from product in _context.Products
                            join type in _context.ProductCategories on product.CategoryId equals type.CategoryId
                            orderby product.ProductId
                            select new ProductViewModel
                            {
                                ProductId = product.ProductId,
                                ProductName = product.ProductName,
                                Price = product.Price,
                                Description = product.Description,
                                ImageUrl = product.ImageUrl,
                                Notes = product.Notes,
                                CategoryName = type.CategoryName
                            }).ToList();

            PagedList<ProductViewModel> pagedListItem = new PagedList<ProductViewModel>(listItem, pageNumber, pageSize);

            return View(pagedListItem);
        }

        [Route("Search")]
        [HttpGet]
        public IActionResult Search(int? page, string search)
        {
            int pageSize = 30;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            if (string.IsNullOrWhiteSpace(search))
            {
                return RedirectToAction(nameof(Index));
            }

            search = search.ToLower();
            ViewBag.search = search;

            var listItem = (from product in _context.Products
                            join type in _context.ProductCategories on product.CategoryId equals type.CategoryId
                            where product.ProductName.ToLower().Contains(search)
                            orderby product.ProductId
                            select new ProductViewModel
                            {
                                ProductId = product.ProductId,
                                ProductName = product.ProductName,
                                Price = product.Price,
                                Description = product.Description,
                                ImageUrl = product.ImageUrl,
                                Notes = product.Notes,
                                CategoryName = type.CategoryName
                            }).ToList();

            PagedList<ProductViewModel> pagedListItem = new PagedList<ProductViewModel>(listItem, pageNumber, pageSize);

            return View(pagedListItem);
        }

        // GET: /Auth/Create
        [Route("Create")]
        [HttpGet]
        public IActionResult Create()
        {
            // N?p danh sách nhóm s?n ph?m vào ViewBag d? t?o dropdown
            ViewBag.CategoryId = new SelectList(_context.ProductCategories.ToList(), "CategoryId", "CategoryName");
            return View();
        }

        
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SanPhamViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoryId = new SelectList(_context.ProductCategories.ToList(), "CategoryId", "CategoryName");
                return View(model);
            }

            // N?u tên s?n ph?m dã t?n t?i, thêm timestamp d? t?o tên riêng bi?t
            string tenSanPham = model.ProductName;
            if (_context.Products.Any(p => p.ProductName.ToLower() == tenSanPham.ToLower()))
            {
                tenSanPham = $"{tenSanPham}_{DateTime.Now:yyyyMMddHHmmssfff}";
            }


            Product sanPham = new Product
            {
                ProductName = tenSanPham,
                Price = model.Price,
                Description = model.Description,
                Notes = model.Notes,
                CategoryId = model.CategoryId
                // Hình ?nh s? du?c gán sau khi x? lý file upload
            };

            // Xử lý file upload (nếu có)
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                if (!IsValidImage(model.ImageFile, out string errorMessage))
                {
                    ModelState.AddModelError("ImageFile", errorMessage);
                    ViewBag.CategoryId = new SelectList(_context.ProductCategories.ToList(), "CategoryId", "CategoryName");
                    return View(model);
                }

                // Xử lý file upload qua ImageKit
                try 
                {
                    sanPham.ImageUrl = await _imageService.UploadImageAsync(model.ImageFile, "products");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("ImageFile", "Lỗi khi upload ảnh lên cloud: " + ex.Message);
                    ViewBag.CategoryId = new SelectList(_context.ProductCategories.ToList(), "CategoryId", "CategoryName");
                    return View(model);
                }
            }

            _context.Products.Add(sanPham);
            int kq = _context.SaveChanges();

            TempData["Message"] = kq > 0 ? "Thêm s?n ph?m thành công" : "Không thêm du?c s?n ph?m";
            return RedirectToAction("Index", "HomeAdmin");
        }



        [Route("Details")]
        [HttpGet]
        public IActionResult Details(int id, string name)
        {
            var productItem = (from product in _context.Products
                            join type in _context.ProductCategories on product.CategoryId equals type.CategoryId
                            where product.ProductId == id
                            select new ProductViewModel
                            {
                                ProductId = product.ProductId,
                                ProductName = product.ProductName,
                                Price = product.Price,
                                Description = product.Description,
                                ImageUrl = product.ImageUrl,
                                Notes = product.Notes,
                                CategoryName = type.CategoryName
                            }).SingleOrDefault();

            ViewBag.name = name;

            return View(productItem);
        }

        [Route("Edit")]
        [HttpGet]
        public IActionResult Edit(int id, string name)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new SanPhamViewModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Price = product.Price,
                Description = product.Description,
                Notes = product.Notes,
                CategoryId = product.CategoryId
                // We don't map ImageFile because it's for upload
            };

            ViewBag.CategoryId = new SelectList(_context.ProductCategories.ToList(), "CategoryId", "CategoryName");
            ViewBag.name = name;
            ViewBag.CurrentImageUrl = product.ImageUrl; // Keep track of current image

            return View(viewModel);
        }

        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SanPhamViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoryId = new SelectList(_context.ProductCategories.ToList(), "CategoryId", "CategoryName");
                return View(_context.Products.Find(model.ProductId));
            }

            var product = _context.Products.Find(model.ProductId);
            if (product == null)
            {
                return NotFound();
            }

            // Update basic info
            product.ProductName = model.ProductName;
            product.Price = model.Price;
            product.Description = model.Description;
            product.Notes = model.Notes;
            product.CategoryId = model.CategoryId;

            // Handle image upload if a new file is provided
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                if (!IsValidImage(model.ImageFile, out string errorMessage))
                {
                    ModelState.AddModelError("ImageFile", errorMessage);
                    ViewBag.CategoryId = new SelectList(_context.ProductCategories.ToList(), "CategoryId", "CategoryName");
                    return View(model);
                }

                // Xử lý file upload qua ImageKit
                try 
                {
                    product.ImageUrl = await _imageService.UploadImageAsync(model.ImageFile, "products");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("ImageFile", "Lỗi khi cập nhật ảnh: " + ex.Message);
                    ViewBag.CategoryId = new SelectList(_context.ProductCategories.ToList(), "CategoryId", "CategoryName");
                    return View(model);
                }
            }

            _context.Update(product);
            int result = _context.SaveChanges();

            TempData["Message"] = result > 0 ? "Sửa sản phẩm thành công" : "Không có thay đổi nào được lưu";
            return RedirectToAction("Index", "HomeAdmin");
        }

        [Route("Delete")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            TempData["Message"] = "";
            var chiTietHoaDon = _context.SalesOrderItems.Where(x => x.ProductId == id).ToList();

            if (chiTietHoaDon.Count() > 0)
            {
                TempData["Message"] = "Không xoá du?c s?n ph?m";

                return RedirectToAction("Index", "HomeAdmin");
            }

            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Remove(product);
                _context.SaveChanges();
                TempData["Message"] = "Sản phẩm đã được xóa";
            }
            else
            {
               TempData["Message"] = "Không tìm thấy sản phẩm";
            }

            return RedirectToAction("Index", "HomeAdmin");
        }
        private bool IsValidImage(IFormFile file, out string errorMessage)
        {
            errorMessage = string.Empty;
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                errorMessage = "Chỉ chấp nhận các định dạng ảnh: .jpg, .jpeg, .png, .gif";
                return false;
            }

            if (file.Length > 5 * 1024 * 1024) // 5MB
            {
                errorMessage = "Kích thước ảnh không được vượt quá 5MB";
                return false;
            }

            return true;
        }
    }
}



