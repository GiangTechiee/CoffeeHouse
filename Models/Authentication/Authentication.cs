using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace CoffeeHouse.Models.Authentication
{
    public class Authentication : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Lấy thông tin đăng nhập và role từ session
            string tenTaiKhoan = context.HttpContext.Session.GetString("TenTaiKhoan");
            string role = context.HttpContext.Session.GetString("Role");

            // Kiểm tra nếu chưa đăng nhập hoặc role không phải là Admin
            if (string.IsNullOrEmpty(tenTaiKhoan) && (role != "Admin" || role != "Employee"))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "controller", "auth" },
                    { "action", "login" }
                });
            }

            base.OnActionExecuting(context);
        }
    }
}
