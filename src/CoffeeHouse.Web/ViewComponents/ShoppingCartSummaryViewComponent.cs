namespace CoffeeHouse.Web.ViewComponents;

public class ShoppingCartSummaryViewComponent : Microsoft.AspNetCore.Mvc.ViewComponent
{
    private readonly CoffeeHouse.Infrastructure.Persistence.CoffeeHouseContext _context;

    public ShoppingCartSummaryViewComponent(CoffeeHouse.Infrastructure.Persistence.CoffeeHouseContext context)
    {
        _context = context;
    }

    public async Task<Microsoft.AspNetCore.Mvc.IViewComponentResult> InvokeAsync()
    {
        string maKhachHangStr = HttpContext.Session.GetString("CustomerId");
        if (string.IsNullOrEmpty(maKhachHangStr))
        {
            return View(0);
        }

        if (!int.TryParse(maKhachHangStr, out int maKhachHang))
        {
            return View(0);
        }

        var cartItems = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(
            Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.Include(
                System.Linq.Queryable.Where(
                    _context.CartItems,
                    g => g.CustomerId == maKhachHang
                ),
                g => g.Product
            )
        );

        int cartItemCount = System.Linq.Enumerable.Sum(cartItems, item => (int)item.Quantity);

        return View(cartItemCount);
    }
}
