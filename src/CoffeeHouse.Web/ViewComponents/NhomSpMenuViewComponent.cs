using CoffeeHouse.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeHouse.Web.ViewComponents
{
    public class NhomSpMenuViewComponent : ViewComponent
    {
        private readonly IProductCategoryRepository _productCategoryRepository;
        public NhomSpMenuViewComponent(IProductCategoryRepository productCategoryRepository)
        {
            _productCategoryRepository = productCategoryRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = (await _productCategoryRepository.GetAllAsync()).OrderBy(x => x.CategoryName);
            return View(categories);
        }
    }
}


