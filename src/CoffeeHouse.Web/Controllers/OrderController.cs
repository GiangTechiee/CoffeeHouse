using Microsoft.AspNetCore.Mvc;
using MediatR;
using CoffeeHouse.Application.Orders.Queries.GetUserOrders;
using CoffeeHouse.Application.Orders.Queries.GetOrderDetails;
using System;
using System.Threading.Tasks;

namespace CoffeeHouse.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: /Order/Index
        public async Task<IActionResult> Index()
        {
            if (!CurrentCustomerId.HasValue)
                return RedirectToLogin();

            var query = new GetUserOrdersQuery { CustomerId = CurrentCustomerId.Value };
            var orders = await _mediator.Send(query);

            return View(orders);
        }

        // GET: /Order/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            if (!CurrentCustomerId.HasValue)
                return RedirectToLogin();

            var query = new GetOrderDetailsQuery 
            { 
                OrderId = id, 
                CustomerId = CurrentCustomerId.Value 
            };
            
            var order = await _mediator.Send(query);

            if (order == null)
            {
                return NotFound("Hóa đơn không tồn tại hoặc không thuộc về tài khoản của bạn.");
            }

            return View(order);
        }
    }
}
