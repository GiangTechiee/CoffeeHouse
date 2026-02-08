using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CoffeeHouse.Domain.Entities
{
    [PrimaryKey("OrderId", "ProductId")]
    public partial class SalesOrderItem : BaseEntity
    {
        [Key]
        public Guid OrderId { get; set; }

        [ForeignKey("OrderId")]
        public SalesOrder Order { get; set; }


        [Key]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "NUMERIC(12,2)")]
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Computed column - LineTotal = Quantity * UnitPrice
        /// PostgreSQL GENERATED ALWAYS AS STORED column (3NF compliance)
        /// This is computed and stored by the database, not by application code
        /// </summary>
        [Column(TypeName = "NUMERIC(12,2)")]
        public decimal LineTotal { get; set; }

    }
}
