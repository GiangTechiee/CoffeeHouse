using CoffeeHouse.ViewModels;
using CoffeeHouse.Application.Authentication.Commands.AuthenticateUser;
using CoffeeHouse.Application.Authentication.Commands.RegisterCustomer;
using CoffeeHouse.Domain.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CoffeeHouse.Helpers;
using Microsoft.AspNetCore.RateLimiting;

namespace CoffeeHouse.Controllers
{
    [EnableRateLimiting("AuthPolicy")]
    public class AuthController : Controller
    {
        private readonly IMediator _mediator;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public AuthController(IMediator mediator, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _mediator = mediator;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var command = new RegisterCustomerCommand
            {
                Username = model.Username,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword,
                FullName = model.CustomerName,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address
            };

            var result = await _mediator.Send(command);

            if (result.Success)
            {
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", result.Message);
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user != null)
                {
                    // For backward compatibility with legacy code that checks Session
                    HttpContext.Session.SetString(Constants.SessionKeys.Username, user.UserName ?? "");
                    
                    var roles = await _userManager.GetRolesAsync(user);
                    HttpContext.Session.SetString(Constants.SessionKeys.Role, roles.FirstOrDefault() ?? "User");

                    if (user.EmployeeId.HasValue)
                        HttpContext.Session.SetString(Constants.SessionKeys.EmployeeId, user.EmployeeId.Value.ToString());

                    if (user.CustomerId.HasValue)
                        HttpContext.Session.SetString(Constants.SessionKeys.CustomerId, user.CustomerId.Value.ToString());
                }

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }
    }
}
