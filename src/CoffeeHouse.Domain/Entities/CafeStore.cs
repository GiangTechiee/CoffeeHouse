using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CoffeeHouse.Domain.Entities
{
    public partial class CafeStore
    {
        [Key]
        public int StoreId { get; set; }

        [StringLength(255)]
        [Required]
        public string StoreName { get; set; }

        [StringLength(255)]
        [Required]
        public string Address { get; set; }


        [StringLength(20)]
        [Unicode(false)]
        [Required]
        public string PhoneNumber { get; set; }

        [StringLength(255)]
        [EmailAddress]
        public string? Email { get; set; }


        public ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();


        public ICollection<Employee> Employees { get; set; } = new List<Employee>();


        public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    }
}
