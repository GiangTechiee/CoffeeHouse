using CoffeeHouse.Models;

namespace CoffeeHouse.ViewModels
{
    public class CheckoutViewModel
    {
        public IEnumerable<TbGioHang> CartItems { get; set; }
        public TbKhachHang Customer { get; set; }
        public string Total { get; set; }
    }
}
