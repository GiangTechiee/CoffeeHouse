using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeHouse.Domain.Entities
{
    public class Account : BaseEntity
    {
        [Key]
        public int AccountId { get; set; }

        [StringLength(50)]
        [Required]
        public string Username { get; set; }

        [StringLength(255)]
        [Required]
        public string Password { get; set; }

        public int RoleId { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Active";

        public int? EmployeeId { get; set; }
        public int? CustomerId { get; set; }

        [ForeignKey("RoleId")]
        public Role Role { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }

        [ForeignKey("CustomerId")]
        public Customer? Customer { get; set; }
    }
}

