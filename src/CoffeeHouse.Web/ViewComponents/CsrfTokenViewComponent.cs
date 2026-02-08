using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeHouse.Web.ViewComponents;

public class CsrfTokenViewComponent : ViewComponent
{
    private readonly IAntiforgery _antiforgery;

    public CsrfTokenViewComponent(IAntiforgery antiforgery)
    {
        _antiforgery = antiforgery;
    }

    public IViewComponentResult Invoke()
    {
        var token = _antiforgery.GetAndStoreTokens(HttpContext).RequestToken;
        return View("Default", token);
    }
}
