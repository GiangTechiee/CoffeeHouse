using CoffeeHouse.Application.Employees.Commands.CreateEmployee;
using CoffeeHouse.Application.Employees.Commands.DeleteEmployee;
using CoffeeHouse.Application.Employees.Commands.UpdateEmployee;
using CoffeeHouse.Application.Employees.Queries.GetEmployeeById;
using CoffeeHouse.Application.Employees.Queries.GetEmployees;
using CoffeeHouse.Application.Common.Queries.GetCafeStores;
using CoffeeHouse.Domain.Exceptions;
using CoffeeHouse.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace CoffeeHouse.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NhanViensController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<NhanViensController> _logger;

        public NhanViensController(IMediator mediator, ILogger<NhanViensController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        // GET: Admin/NhanViens
        public async Task<IActionResult> Index(int? page, string searchString, int? StoreId)
        {
            var pageNumber = page ?? 1;
            var pageSize = 20;

            var query = new GetEmployeesQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchString,
                CoffeeShopId = StoreId
            };

            var result = await _mediator.Send(query);

            // Populate Dropdown for Stores
            var stores = await _mediator.Send(new GetCafeStoresQuery());
            ViewData["StoreId"] = new SelectList(stores, "StoreId", "StoreName", StoreId);
            ViewData["CurrentFilter"] = searchString;

            // Map DTOs to PagedList
            var pagedList = new StaticPagedList<CoffeeHouse.Application.Employees.DTOs.EmployeeDto>(
                result.Items, pageNumber, pageSize, result.TotalCount);

            return View(pagedList);
        }

        // GET: Admin/NhanViens/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var query = new GetEmployeeByIdQuery { Id = id };
            var employee = await _mediator.Send(query);

            if (employee == null)
            {
                return NotFound("Employee not found.");
            }

            return View(employee);
        }

        // GET: Admin/NhanViens/Create
        public async Task<IActionResult> Create()
        {
            var stores = await _mediator.Send(new GetCafeStoresQuery());
            ViewData["StoreId"] = new SelectList(stores, "StoreId", "StoreName");
            return View();
        }

        // POST: Admin/NhanViens/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEmployeeCommand command)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _mediator.Send(command);
                    TempData["Message"] = "Successfully created employee.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error creating employee: " + ex.Message);
                }
            }

            var stores = await _mediator.Send(new GetCafeStoresQuery());
            ViewData["StoreId"] = new SelectList(stores, "StoreId", "StoreName", command.CoffeeShopId);
            return View(command);
        }

        // GET: Admin/NhanViens/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var query = new GetEmployeeByIdQuery { Id = id };
            var employee = await _mediator.Send(query);

            if (employee == null)
            {
                return NotFound("Employee not found.");
            }

            var model = new UpdateEmployeeCommand
            {
                Id = employee.Id,
                FullName = employee.FullName,
                Address = employee.Address,
                Gender = employee.Gender,
                DateOfBirth = employee.DateOfBirth,
                Position = employee.Position,
                PhoneNumber = employee.PhoneNumber,
                Email = employee.Email,
                BaseSalary = employee.BaseSalary,
                SalaryCoefficient = employee.SalaryCoefficient,
                CoffeeShopId = employee.CoffeeShopId
            };

            var stores = await _mediator.Send(new GetCafeStoresQuery());
            ViewData["StoreId"] = new SelectList(stores, "StoreId", "StoreName", employee.CoffeeShopId);
            return View(model);
        }

        // POST: Admin/NhanViens/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateEmployeeCommand command)
        {
            if (id != command.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _mediator.Send(command);
                    TempData["Message"] = "Successfully updated employee.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error updating employee: " + ex.Message);
                }
            }

            var stores = await _mediator.Send(new GetCafeStoresQuery());
            ViewData["StoreId"] = new SelectList(stores, "StoreId", "StoreName", command.CoffeeShopId);
            return View(command);
        }

        // POST: Admin/NhanViens/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _mediator.Send(new DeleteEmployeeCommand(id));
                if (result.IsSuccess)
                {
                    TempData["Message"] = "Successfully deleted employee.";
                }
                else
                {
                    TempData["Error"] = "Error deleting employee: " + result.Error;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error deleting employee: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

