namespace CoffeeHouse.ViewModels
{
    public class SanPhamViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? Notes { get; set; }
        public int CategoryId { get; set; }
    }

}
