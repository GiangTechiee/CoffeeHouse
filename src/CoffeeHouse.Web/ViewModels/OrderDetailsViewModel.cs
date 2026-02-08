namespace CoffeeHouse.ViewModels
{
    public class OrderDetailsViewModel
    {
        public Guid OrderId { get; set; }

        public int ProductId { get; set; }

        public decimal? UnitPrice { get; set; }

        public int? Discount { get; set; }

        public int? Quantity { get; set; }

        public decimal? LineTotal { get; set; }
    }
}
