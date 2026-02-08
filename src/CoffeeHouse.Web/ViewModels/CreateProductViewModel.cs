namespace CoffeeHouse.ViewModels
{
    public class CreateProductViewModel
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public decimal? Price { get; set; }

        public string? Description { get; set; }

        public IFormFile ImageUrl { get; set; } = null!;

        public string? Notes { get; set; }

        public int CategoryId { get; set; }
    }
}
