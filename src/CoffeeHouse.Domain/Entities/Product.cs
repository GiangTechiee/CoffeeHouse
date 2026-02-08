using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CoffeeHouse.Domain.Entities
{
    public partial class Product : BaseEntity
    {
        [Key]
        public int ProductId { get; set; }

        public Product() { }

        public Product(string productName, decimal price, int categoryId)
        {
            ProductName = productName;
            Price = price;
            CategoryId = categoryId;
        }

        [StringLength(255)]
        [Required]
        public string ProductName { get; set; }

        [Column(TypeName = "NUMERIC(12,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "TEXT")]
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        [Column(TypeName = "TEXT")]
        public string? Notes { get; set; }

        public int CategoryId { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Active";

        [ForeignKey("CategoryId")]
        public ProductCategory? Category { get; set; }


        public ICollection<SalesOrderItem> SalesOrderItems { get; set; } = new List<SalesOrderItem>();


        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
