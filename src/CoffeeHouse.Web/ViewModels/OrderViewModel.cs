namespace CoffeeHouse.ViewModels
{
    public class OrderViewModel
    {
        public Guid OrderId { get; set; }

        public string OrderNumber { get; set; } = null!;

        public DateTime? OrderDate { get; set; }

        public decimal? TotalAmount { get; set; }

        public Guid CustomerId { get; set; }
    }
}
