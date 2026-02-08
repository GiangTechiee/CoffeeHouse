using CoffeeHouse.Domain.Entities;

namespace CoffeeHouse.ViewModels
{
    public class CheckoutViewModel
    {
        public IEnumerable<CartItem> CartItems { get; set; }
        public Customer Customer { get; set; }
        public string Total { get; set; }
    }
}
