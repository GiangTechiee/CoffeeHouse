using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CoffeeHouse.ViewModels;
using MediatR;
using CoffeeHouse.Application.Carts.Commands.AddToCart;
using CoffeeHouse.Application.Carts.Commands.RemoveFromCart;
using CoffeeHouse.Application.Carts.Commands.Checkout;
using CoffeeHouse.Application.Carts.Commands.UpdateCart;
using CoffeeHouse.Application.Carts.Queries.GetCart;
using CoffeeHouse.Application.Carts.Queries.GetCartCount;
using CoffeeHouse.Application.Customers.Queries.GetCustomerById;
using CoffeeHouse.Application.Common.Queries.GetCafeStores;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoffeeHouse.Controllers
{
    public class CartController : BaseController
    {
        private readonly IMediator _mediator;

        public CartController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: /Cart/Index
        public async Task<IActionResult> Index()
        {
            if (!CurrentCustomerId.HasValue)
                return RedirectToLogin();

            var cart = await _mediator.Send(new GetCartQuery { CustomerId = CurrentCustomerId.Value });
            ViewData["total"] = cart.TotalAmount.ToString("n0");

            return View(cart.Items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int id, int quantity)
        {
            if (!CurrentCustomerId.HasValue)
                return RedirectToLogin();

            try
            {
                var command = new AddToCartCommand
                {
                    CustomerId = CurrentCustomerId.Value,
                    ProductId = id,
                    Quantity = quantity
                };

                var result = await _mediator.Send(command);

                if (result.Success)
                {
                    return RedirectToAction("Index");
                }

                TempData["Error"] = result.Message;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            if (!CurrentCustomerId.HasValue)
                return Json(0);

            var count = await _mediator.Send(new GetCartCountQuery { CustomerId = CurrentCustomerId.Value });
            return Json(count);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([FromBody] List<UpdateQuantityRequest> updates)
        {
            if (updates == null || !updates.Any())
                return BadRequest("Dá»¯ liá»‡u cáº­p nháº­t khÃ´ng há»£p lá»‡.");

            if (!CurrentCustomerId.HasValue)
                return BadRequest("Vui lÃ²ng Ä‘Äƒng nháº­p.");

            try
            {
                var command = new UpdateCartCommand
                {
                    CustomerId = CurrentCustomerId.Value,
                    Updates = updates.Select(u => new UpdateCartItemRequest 
                    { 
                        ProductId = u.ProductId, 
                        Quantity = u.Quantity 
                    }).ToList()
                };

                var result = await _mediator.Send(command);

                return Json(new
                {
                    success = result.Success,
                    message = result.Message,
                    totalAmount = result.TotalAmount,
                    totalItems = result.TotalItems
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lá»—i há»‡ thá»‘ng: {ex.Message}");
            }
        }

        public class UpdateQuantityRequest
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int maSp)
        {
            try
            {
                if (!CurrentCustomerId.HasValue)
                    return Json(new { success = false, message = "KhÃ´ng Ä‘Äƒng nháº­p." });

                var command = new RemoveFromCartCommand
                {
                    CustomerId = CurrentCustomerId.Value,
                    ProductId = maSp
                };

                var result = await _mediator.Send(command);

                return Json(new
                {
                    success = result.Success,
                    message = result.Message,
                    totalAmount = result.TotalAmount,
                    totalItems = result.TotalItems
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "CÃ³ lá»—i xáº£y ra khi xÃ³a sáº£n pháº©m.", error = ex.Message });
            }
        }

        public async Task<IActionResult> Checkout()
        {
            if (!CurrentCustomerId.HasValue)
                return RedirectToLogin();

            var stores = await _mediator.Send(new GetCafeStoresQuery());
            ViewBag.Quan = new SelectList(stores, "StoreId", "StoreName");

            var cart = await _mediator.Send(new GetCartQuery { CustomerId = CurrentCustomerId.Value });
            if (cart.Items == null || !cart.Items.Any())
                return RedirectToAction("Index", "Home");

            var customerResult = await _mediator.Send(new GetCustomerByIdQuery { Id = CurrentCustomerId.Value });
            var customer = customerResult.Value;

            var model = new CheckoutViewModel
            {
                // Note: CheckoutViewModel might need adaptation if it expects List<CartItem> (Domain entity)
                // but for now we follow the structure.
                Total = cart.TotalAmount.ToString("n0")
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirmation(string customerName, string phoneNumber, string address, string checkoutMethod, int StoreId)
        {
            if (!CurrentCustomerId.HasValue)
                return RedirectToLogin();

            try
            {
                var command = new CheckoutCommand
                {
                    CustomerId = CurrentCustomerId.Value,
                    CustomerName = customerName,
                    PhoneNumber = phoneNumber,
                    Address = address,
                    CoffeeShopId = StoreId,
                    PaymentMethod = checkoutMethod,
                    EmployeeId = null
                };

                var result = await _mediator.Send(command);

                if (result.Success)
                {
                    return RedirectToAction("Success");
                }

                TempData["Error"] = result.Message;
                return RedirectToAction("Checkout");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"CÃ³ lá»—i xáº£y ra: {ex.Message}";
                return RedirectToAction("Checkout");
            }
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}

