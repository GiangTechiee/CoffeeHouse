using CoffeeHouse.Application.Products.Commands.CreateProduct;
using CoffeeHouse.Application.Products.Commands.UpdateProduct;
using CoffeeHouse.Application.Products.Commands.DeleteProduct;
using CoffeeHouse.Application.Products.Queries.GetProducts;
using CoffeeHouse.Application.Products.Queries.GetProductById;
using CoffeeHouse.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

using Microsoft.AspNetCore.Authorization;
using CoffeeHouse.Helpers;

namespace CoffeeHouse.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int? page)
        {
            try
            {
                int pageSize = 18;
                int pageNumber = page == null || page < 0 ? 1 : page.Value;

                // Send query through MediatR
                var query = new GetProductsQuery
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var result = await _mediator.Send(query);

                // Map DTOs to ViewModels for the view
                var viewModels = result.Items.Select(dto => new ProductViewModel
                {
                    ProductId = dto.Id,
                    ProductName = dto.Name,
                    Price = dto.Price,
                    Description = dto.Description,
                    ImageUrl = dto.ImageUrl,
                    Notes = dto.Notes,
                    CategoryName = dto.CategoryName ?? string.Empty
                }).ToList();

                // Create PagedList for view compatibility
                var pagedListItem = new StaticPagedList<ProductViewModel>(
                    viewModels,
                    pageNumber,
                    pageSize,
                    result.TotalCount
                );

                return View(pagedListItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving products for Index page");
                TempData["Error"] = "An error occurred while loading products. Please try again.";
                return View(new StaticPagedList<ProductViewModel>(
                    new List<ProductViewModel>(),
                    1,
                    18,
                    0
                ));
            }
        }

        public async Task<IActionResult> Modern(int? page)
        {
            try
            {
                int pageSize = 12;
                int pageNumber = page == null || page < 0 ? 1 : page.Value;

                // Send query through MediatR
                var query = new GetProductsQuery
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var result = await _mediator.Send(query);

                // Map DTOs to ViewModels for the view
                var viewModels = result.Items.Select(dto => new ProductViewModel
                {
                    ProductId = dto.Id,
                    ProductName = dto.Name,
                    Price = dto.Price,
                    Description = dto.Description,
                    ImageUrl = dto.ImageUrl,
                    Notes = dto.Notes,
                    CategoryName = dto.CategoryName ?? string.Empty
                }).ToList();

                // Create PagedList for view compatibility
                var pagedListItem = new StaticPagedList<ProductViewModel>(
                    viewModels,
                    pageNumber,
                    pageSize,
                    result.TotalCount
                );

                return View(pagedListItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving products for Modern page");
                TempData["Error"] = "An error occurred while loading products. Please try again.";
                return View(new StaticPagedList<ProductViewModel>(
                    new List<ProductViewModel>(),
                    1,
                    12,
                    0
                ));
            }
        }

        public async Task<IActionResult> Type(int target, string targetName, int? page)
        {
            try
            {
                int pageSize = 9;
                int pageNumber = page == null || page < 0 ? 1 : page.Value;

                // Send query through MediatR with category filter
                var query = new GetProductsQuery
                {
                    CategoryId = target,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var result = await _mediator.Send(query);

                // Map DTOs to ViewModels for the view
                var viewModels = result.Items.Select(dto => new ProductViewModel
                {
                    ProductId = dto.Id,
                    ProductName = dto.Name,
                    Price = dto.Price,
                    Description = dto.Description,
                    ImageUrl = dto.ImageUrl,
                    Notes = dto.Notes,
                    CategoryName = dto.CategoryName ?? string.Empty
                }).ToList();

                // Create PagedList for view compatibility
                var pagedListItem = new StaticPagedList<ProductViewModel>(
                    viewModels,
                    pageNumber,
                    pageSize,
                    result.TotalCount
                );

                ViewBag.target = target;
                ViewBag.targetName = targetName;

                return View(pagedListItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving products for category {CategoryId}", target);
                TempData["Error"] = "An error occurred while loading products. Please try again.";
                
                ViewBag.target = target;
                ViewBag.targetName = targetName;
                
                return View(new StaticPagedList<ProductViewModel>(
                    new List<ProductViewModel>(),
                    1,
                    9,
                    0
                ));
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                // Send query through MediatR
                var query = new GetProductByIdQuery { Id = id };
                var productDto = await _mediator.Send(query);

                // Map DTO to ViewModel for the view
                var viewModel = new ProductViewModel
                {
                    ProductId = productDto.Id,
                    ProductName = productDto.Name,
                    Price = productDto.Price,
                    Description = productDto.Description,
                    ImageUrl = productDto.ImageUrl,
                    Notes = productDto.Notes,
                    CategoryName = productDto.CategoryName ?? string.Empty
                };

                return View(viewModel);
            }
            catch (Domain.Exceptions.NotFoundException ex)
            {
                _logger.LogWarning(ex, "Product with ID {ProductId} not found", id);
                TempData["Error"] = "Product not found.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving product details for ID {ProductId}", id);
                TempData["Error"] = "An error occurred while loading product details. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// GET: Products/Create
        /// Displays the form to create a new product
        /// </summary>
        [HttpGet]
        [Authorize(Roles = Constants.Roles.Admin + "," + Constants.Roles.Employee)]
        public IActionResult Create()
        {
            return View(new SanPhamViewModel());
        }

        /// <summary>
        /// POST: Products/Create
        /// Creates a new product
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constants.Roles.Admin + "," + Constants.Roles.Employee)]
        public async Task<IActionResult> Create(SanPhamViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Send command through MediatR
                var command = new CreateProductCommand
                {
                    Name = model.ProductName,
                    Price = model.Price,
                    Description = model.Description,
                    Notes = model.Description,
                    CategoryId = model.CategoryId,
                    ImageUrl = null // Handle file upload separately if needed
                };

                var result = await _mediator.Send(command);

                _logger.LogInformation("Product created successfully with ID: {ProductId}", result.Id);
                TempData["Success"] = "Product created successfully.";
                return RedirectToAction(nameof(Details), new { id = result.Id });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error while creating product");
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating product");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the product. Please try again.");
                return View(model);
            }
        }

        /// <summary>
        /// GET: Products/Edit/{id}
        /// Displays the form to edit an existing product
        /// </summary>
        [HttpGet]
        [Authorize(Roles = Constants.Roles.Admin + "," + Constants.Roles.Employee)]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                // Get product details
                var query = new GetProductByIdQuery { Id = id };
                var productDto = await _mediator.Send(query);

                // Map to ViewModel
                var viewModel = new SanPhamViewModel
                {
                    ProductName = productDto.Name,
                    Price = productDto.Price,
                    Description = productDto.Description,
                    Notes = productDto.Notes,
                    CategoryId = productDto.CategoryId
                };

                ViewBag.ProductId = id;
                return View(viewModel);
            }
            catch (Domain.Exceptions.NotFoundException ex)
            {
                _logger.LogWarning(ex, "Product with ID {ProductId} not found", id);
                TempData["Error"] = "Product not found.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading product for edit, ID: {ProductId}", id);
                TempData["Error"] = "An error occurred while loading the product. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// POST: Products/Edit/{id}
        /// Updates an existing product
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constants.Roles.Admin + "," + Constants.Roles.Employee)]
        public async Task<IActionResult> Edit(int id, SanPhamViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.ProductId = id;
                    return View(model);
                }

                // Send command through MediatR
                var command = new UpdateProductCommand
                {
                    Id = id,
                    Name = model.ProductName,
                    Price = model.Price,
                    Description = model.Description,
                    Notes = model.Description,
                    CategoryId = model.CategoryId,
                    ImageUrl = null // Handle file upload separately if needed
                };

                var result = await _mediator.Send(command);

                _logger.LogInformation("Product updated successfully with ID: {ProductId}", result.Id);
                TempData["Success"] = "Product updated successfully.";
                return RedirectToAction(nameof(Details), new { id = result.Id });
            }
            catch (Domain.Exceptions.NotFoundException ex)
            {
                _logger.LogWarning(ex, "Product with ID {ProductId} not found", id);
                TempData["Error"] = "Product not found.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error while updating product with ID {ProductId}", id);
                ModelState.AddModelError(string.Empty, ex.Message);
                ViewBag.ProductId = id;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating product with ID {ProductId}", id);
                ModelState.AddModelError(string.Empty, "An error occurred while updating the product. Please try again.");
                ViewBag.ProductId = id;
                return View(model);
            }
        }
        /// <summary>
        /// POST: Products/Delete/{id}
        /// Deletes a product
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Constants.Roles.Admin + "," + Constants.Roles.Employee)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var command = new DeleteProductCommand { Id = id };
                await _mediator.Send(command);

                _logger.LogInformation("Product deleted successfully with ID: {ProductId}", id);
                TempData["Success"] = "Product deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Domain.Exceptions.NotFoundException ex)
            {
                _logger.LogWarning(ex, "Product with ID {ProductId} not found", id);
                TempData["Error"] = "Product not found.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting product with ID {ProductId}", id);
                TempData["Error"] = "An error occurred while deleting the product. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}



