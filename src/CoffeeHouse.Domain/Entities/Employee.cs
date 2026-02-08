using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CoffeeHouse.Domain.Entities
{

    public partial class Employee : BaseEntity
    {
        [Key]
        public int EmployeeId { get; set; }

        public int StoreId { get; set; }

        [StringLength(255)]
        [Required]
        public string FullName { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [Column(TypeName = "BOOLEAN")]
        public bool? Gender { get; set; }

        [StringLength(50)]
        [Required]
        public string Position { get; set; }


        [StringLength(20)]
        [Unicode(false)]
        [Required]
        public string PhoneNumber { get; set; }


        [StringLength(50)]
        [Unicode(false)]
        [Required]
        public string IdentityCardNumber { get; set; }

        [StringLength(255)]
        [EmailAddress]
        public string? Email { get; set; }

        [Column(TypeName = "NUMERIC(12,2)")]
        public decimal BaseSalary { get; set; }

        [Column(TypeName = "NUMERIC(4,2)")]
        public decimal SalaryCoefficient { get; set; }

        [ForeignKey("StoreId")]
        public CafeStore? Store { get; set; }


        public ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();


        public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();


        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
