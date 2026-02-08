using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CoffeeHouse.Domain.Entities
{
    public partial class Supplier
    {
        [Key]
        public int SupplierId { get; set; }

        [StringLength(255)]
        [Required]
        public string SupplierName { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }


        [StringLength(20)]
        [Unicode(false)]
        [Required]
        public string PhoneNumber { get; set; }


        [StringLength(50)]
        [Unicode(false)]
        public string? Stk { get; set; }

        /// <summary>
        /// Audit fields
        /// </summary>
        [StringLength(20)]
        public string Status { get; set; } = "Active";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    }
}
