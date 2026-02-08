using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CoffeeHouse.ViewModels;
using MediatR;
using CoffeeHouse.Application.Authentication.Queries.GetAccountInfo;
using CoffeeHouse.Application.Authentication.Commands.UpdateAccountInfo;
using CoffeeHouse.Application.Authentication.DTOs;
using CoffeeHouse.Application.Customers.DTOs;
using CoffeeHouse.Application.Employees.DTOs;

namespace CoffeeHouse.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: /Account/ThongTin
        public async Task<IActionResult> ThongTin()
        {
            if (!IsAuthenticated)
                return RedirectToLogin();

            var query = new GetAccountInfoQuery(CurrentUsername!);
            var accountInfo = await _mediator.Send(query);

            if (accountInfo == null)
            {
                return NotFound("Account not found.");
            }

            // Ideally we should use the DTO in the view, but for now we'll match what the view expects roughly
            // or pass the DTO and let the view handle it.
            // Since I cannot easily see the View, I will pass the DTO.
            
            ViewBag.EmployeeDto = accountInfo.Employee; 
            ViewBag.CustomerDto = accountInfo.Customer;

            return View(accountInfo);
        }

        public async Task<IActionResult> EditThongTin()
        {
            if (!IsAuthenticated)
                return RedirectToLogin();

            var query = new GetAccountInfoQuery(CurrentUsername!);
            var accountInfo = await _mediator.Send(query);

             if (accountInfo == null)
            {
                return NotFound("Account not found.");
            }

            return View(accountInfo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditThongTin(AccountInfoDto model, string? NewPassword)
        {
            if (!IsAuthenticated)
                return RedirectToLogin();

            // Note: ModelState validation might fail if View passes different model structure.
            // Assuming View is updated to send AccountInfoDto fields.
            
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var command = new UpdateAccountInfoCommand
                {
                    CurrentUsername = CurrentUsername!,
                    NewUsername = model.Username,
                    NewPassword = NewPassword,
                    AccountInfo = model
                };

                await _mediator.Send(command);

                TempData["Message"] = "Successfully updated information!";
                return RedirectToAction("ThongTin");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error updating info: " + ex.Message);
                return View(model);
            }
        }
    }
}
