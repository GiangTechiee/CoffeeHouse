using Microsoft.AspNetCore.Mvc;

namespace CoffeeHouse.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
