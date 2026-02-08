using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using CoffeeHouse.Helpers;

namespace CoffeeHouse.Web.Attributes;

public class AuthenticationAttribute : ActionFilterAttribute
{
    public string? Roles { get; set; }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var username = context.HttpContext.Session.GetString(Constants.SessionKeys.Username);
        if (username == null)
        {
            context.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    { "Controller", "Auth" },
                    { "Action", "Login" }
                });
            return;
        }

        if (!string.IsNullOrEmpty(Roles))
        {
            var userRole = context.HttpContext.Session.GetString(Constants.SessionKeys.Role);
            var allowedRoles = Roles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                  .Select(r => r.Trim());

            if (userRole == null || !allowedRoles.Contains(userRole, StringComparer.OrdinalIgnoreCase))
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "Controller", "Auth" },
                        { "Action", "AccessDenied" }
                    });
            }
        }

        base.OnActionExecuting(context);
    }
}

