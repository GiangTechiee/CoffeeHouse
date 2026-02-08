using CoffeeHouse.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeHouse.Controllers
{
    public abstract class BaseController : Controller
    {
        protected string? CurrentUsername => HttpContext.Session.GetString(Constants.SessionKeys.Username);
        
        protected string? CurrentRole => HttpContext.Session.GetString(Constants.SessionKeys.Role);
        
        protected int? CurrentCustomerId
        {
            get
            {
                var idStr = HttpContext.Session.GetString(Constants.SessionKeys.CustomerId);
                return int.TryParse(idStr, out int id) ? id : null;
            }
        }
        
        protected int? CurrentEmployeeId
        {
            get
            {
                var idStr = HttpContext.Session.GetString(Constants.SessionKeys.EmployeeId);
                return int.TryParse(idStr, out int id) ? id : null;
            }
        }
        
        protected bool IsAuthenticated => !string.IsNullOrEmpty(CurrentUsername);
        
        protected bool IsAdmin => CurrentRole == Constants.Roles.Admin;
        
        protected bool IsEmployee => CurrentRole == Constants.Roles.Employee;
        
        protected bool IsUser => CurrentRole == Constants.Roles.User;
        
        protected IActionResult RedirectToLogin()
        {
            return RedirectToAction("Login", "Auth");
        }
    }
}

