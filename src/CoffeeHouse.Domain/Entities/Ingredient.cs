using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Domain.Entities
{
    public partial class Ingredient : BaseEntity
    {
        [Key]
        public int IngredientId { get; set; }

        [StringLength(255)]
        [Required]
        public string IngredientName { get; set; }

        [Column(TypeName = "NUMERIC(12,3)")]
        public decimal Quantity { get; set; }

        [StringLength(50)]
        [Required]
        public string Unit { get; set; }

        public DateTime? ExpirationDate { get; set; }

        [Column(TypeName = "NUMERIC(12,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "NUMERIC(12,3)")]
        public decimal MinimumQuantity { get; set; }


        public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();
    }
}
