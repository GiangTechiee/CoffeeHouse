namespace CoffeeHouse.ViewModels
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public decimal? Price { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public string? Notes { get; set; }

        public string CategoryName { get; set; } = null!;
    }
}
