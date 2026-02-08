using CoffeeHouse.Domain.Entities;

namespace CoffeeHouse.ViewModels
{
    public class HomeViewModel
    {
        public List<Product> Products { get; set; } = new List<Product>();
        public List<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();

        // Thêm gi? hàng t? db (tbGioHang)
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
