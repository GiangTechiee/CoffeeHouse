using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Domain.Entities
{
    [PrimaryKey("CustomerId", "ProductId")]
    public partial class CartItem : BaseEntity
    {
        [Key]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }


        [Key]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }


        public int Quantity { get; set; }

    }
}
