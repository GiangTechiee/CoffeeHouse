using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CoffeeHouse.Domain.Entities
{
    public partial class Role : BaseEntity
    {
        [Key]
        public int RoleId { get; set; }

        [StringLength(50)] // Optimized length
        [Required]
        public string RoleName { get; set; }

        public string? Description { get; set; }

        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
