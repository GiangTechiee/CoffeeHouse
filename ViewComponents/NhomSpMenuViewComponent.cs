using CoffeeHouse.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeHouse.ViewComponents
{
    public class NhomSpMenuViewComponent : ViewComponent
    {
        private readonly INhomSpRepository _nhomSp;
        public NhomSpMenuViewComponent(INhomSpRepository nhomSpRepository)
        {
            _nhomSp = nhomSpRepository;
        }

        public IViewComponentResult Invoke()
        {
            var nhomSp = _nhomSp.GetAllNhomSp().OrderBy(x => x.TenNhomSp);
            return View(nhomSp);
        }
    }
}
