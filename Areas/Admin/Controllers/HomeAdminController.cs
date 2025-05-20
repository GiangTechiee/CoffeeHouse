using CoffeeHouse.Models;
using CoffeeHouse.Models.Authentication;
using CoffeeHouse.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace CoffeeHouse.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("HomeAdmin")]
    public class HomeAdminController : Controller
    {
        private readonly CoffeeHouseContext _context;
        IWebHostEnvironment _hostEnvironment;

        public HomeAdminController(CoffeeHouseContext context, IWebHostEnvironment hc)
        {
            _context = context;
            _hostEnvironment = hc;
        }

        [Route("")]
        [Route("Index")]
        [Authentication]
        public IActionResult Index(int? page)
        {
            int pageSize = 30;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var listItem = (from product in _context.TbSanPhams
                            join type in _context.TbNhomSanPhams on product.MaNhomSp equals type.MaNhomSp
                            orderby product.MaSanPham
                            select new ProductViewModel
                            {
                                MaSanPham = product.MaSanPham,
                                TenSanPham = product.TenSanPham,
                                GiaBan = product.GiaBan,
                                MoTa = product.MoTa,
                                HinhAnh = product.HinhAnh,
                                GhiChu = product.GhiChu,
                                LoaiSanPham = type.TenNhomSp
                            }).ToList();

            PagedList<ProductViewModel> pagedListItem = new PagedList<ProductViewModel>(listItem, pageNumber, pageSize);

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

            var listItem = _context.TbSanPhams.AsNoTracking().Where(x => x.TenSanPham.ToLower().Contains(search)).OrderBy(x => x.MaSanPham).ToList();
            PagedList<TbSanPham> pagedListItem = new PagedList<TbSanPham>(listItem, pageNumber, pageSize);

            return View(pagedListItem);
        }

        // GET: /Auth/Create
        [Route("Create")]
        [HttpGet]
        public IActionResult Create()
        {
            // Nạp danh sách nhóm sản phẩm vào ViewBag để tạo dropdown
            ViewBag.MaNhomSp = new SelectList(_context.TbNhomSanPhams.ToList(), "MaNhomSp", "TenNhomSp");
            return View();
        }

        
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SanPhamViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.MaNhomSp = new SelectList(_context.TbNhomSanPhams.ToList(), "MaNhomSp", "TenNhomSp");
                return View(model);
            }

            // Nếu tên sản phẩm đã tồn tại, thêm timestamp để tạo tên riêng biệt
            string tenSanPham = model.TenSanPham;
            if (_context.TbSanPhams.Any(p => p.TenSanPham.ToLower() == tenSanPham.ToLower()))
            {
                tenSanPham = $"{tenSanPham}_{DateTime.Now:yyyyMMddHHmmssfff}";
            }


            TbSanPham sanPham = new TbSanPham
            {
                TenSanPham = tenSanPham,
                GiaBan = model.GiaBan,
                MoTa = model.MoTa,
                GhiChu = model.GhiChu,
                MaNhomSp = model.MaNhomSp
                // Hình ảnh sẽ được gán sau khi xử lý file upload
            };

            // Xử lý file upload (nếu có)
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                string uploadFolder = Path.Combine(_hostEnvironment.WebRootPath, "img", "products");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.ImageFile.FileName);
                string filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImageFile.CopyTo(stream);
                }

                sanPham.HinhAnh = uniqueFileName;
            }

            _context.TbSanPhams.Add(sanPham);
            int kq = _context.SaveChanges();

            TempData["Message"] = kq > 0 ? "Thêm sản phẩm thành công" : "Không thêm được sản phẩm";
            return RedirectToAction("Index", "HomeAdmin");
        }



        [Route("Details")]
        [Authentication]
        [HttpGet]
        public IActionResult Details(int id, string name)
        {
            var productItem = (from product in _context.TbSanPhams
                            join type in _context.TbNhomSanPhams on product.MaNhomSp equals type.MaNhomSp
                            where product.MaSanPham == id
                            select new ProductViewModel
                            {
                                MaSanPham = product.MaSanPham,
                                TenSanPham = product.TenSanPham,
                                GiaBan = product.GiaBan,
                                MoTa = product.MoTa,
                                HinhAnh = product.HinhAnh,
                                GhiChu = product.GhiChu,
                                LoaiSanPham = type.TenNhomSp
                            }).SingleOrDefault();

            ViewBag.name = name;

            return View(productItem);
        }

        [Route("Edit")]
        [Authentication]
        [HttpGet]
        public IActionResult Edit(int id, string name)
        {
            var sanPham = _context.TbSanPhams.Find(id);

            ViewBag.MaNhomSp = new SelectList(_context.TbNhomSanPhams.ToList(), "MaNhomSp", "TenNhomSp");
            ViewBag.name = name;

            return View(sanPham);
        }

        [Route("Edit")]
        [Authentication]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CreateProductViewModel createProduct)
        {
            string fileName = "";

            if (createProduct.HinhAnh != null)
            {
                string uploadFolder = Path.Combine(Path.Combine(_hostEnvironment.WebRootPath, "img"), "products");
                fileName = createProduct.HinhAnh.FileName;
                string filePath = Path.Combine(uploadFolder, fileName);
                createProduct.HinhAnh.CopyTo(new FileStream(filePath, FileMode.Create));
            }

            var product = new TbSanPham
            {
                MaSanPham = createProduct.MaSanPham,
                TenSanPham = createProduct.TenSanPham,
                GiaBan = (decimal)createProduct.GiaBan,
                MoTa = createProduct.MoTa,
                HinhAnh = fileName,
                GhiChu = createProduct.GhiChu,
                MaNhomSp = createProduct.MaLoaiSanPham
            };

            _context.Entry(product).State = EntityState.Modified;
            _context.SaveChanges();
            TempData["Message"] = "Sửa sản phẩm thành công";
            return RedirectToAction("Index", "HomeAdmin");
        }

        [Route("Delete")]
        [Authentication]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            TempData["Message"] = "";
            var chiTietHoaDon = _context.TbChiTietHoaDonBans.Where(x => x.MaSanPham == id).ToList();

            if (chiTietHoaDon.Count() > 0)
            {
                TempData["Message"] = "Không xoá được sản phẩm";

                return RedirectToAction("Index", "HomeAdmin");
            }

            _context.Remove(_context.TbSanPhams.Find(id));
            _context.SaveChanges();

            TempData["Message"] = "Sản phẩm đã được xoá";

            return RedirectToAction("Index", "HomeAdmin");
        }
    }
}
