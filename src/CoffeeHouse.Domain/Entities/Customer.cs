using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CoffeeHouse.Domain.Entities
{
    public partial class Customer : BaseEntity
    {
        [Key]
        public int CustomerId { get; set; }

        [StringLength(255)]
        [Required]
        public string CustomerName { get; set; }


        [StringLength(20)]
        [Unicode(false)]
        public string PhoneNumber { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();


        public ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();


        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
